using LAHJAAPI.Models;
using LAHJAAPI.V1.Validators.Conditions; // Assuming this contains ConditionResult, DataFilter, etc.
using Microsoft.EntityFrameworkCore;
using WasmAI.ConditionChecker.Base; // Assuming this contains ValidatorContext, IConditionChecker

// Move the Enum definition before the class
namespace LAHJAAPI.V1.Validators
{
    public enum PlanValidatorStates
    {
        // IsActive, // Removed as it's not implemented and not registered
        HasEnoughModels,
        HasAllowedRequests,
        HasSharedProcessor,
        HasEnoughRam,
        HasSpeedLimit,
        SupportDisabled,
        CustomizationDisabled,
        HasAllowedSpaces
    }

    public class PlanFeatureValidatorKeys // Keep this as it defines the keys
    {
        public static readonly string NumberModels = "number_models";
        public static readonly string AllowedRequests = "allowed_requests";
        public static readonly string AllowedSpaces = "allowed_spaces";
        public static readonly string Processor = "processor";
        public static readonly string Ram = "ram";
        public static readonly string Speed = "speed";
        public static readonly string Support = "support";
        public static readonly string Customization = "customization";
    }

    public class PlanValidator : ValidatorContext<Plan, PlanValidatorStates> // Changed TEntity to Plan as validation relates to Plan features
    {

        public PlanValidator(IConditionChecker checker) : base(checker)
        {
        }

        // We no longer register conditions programmatically here.
        // They are registered via Attributes on the methods below.
        protected override void InitializeConditions()
        {
            // No programmatic registration needed here anymore
        }

        // Helper method to get a PlanFeature by Key and PlanId using the new QueryDbSet pattern
        private async Task<PlanFeature?> GetPlanFeatureByKeyAsync(string key, string planId)
        {
            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(planId)) return null;

            var features = await QueryListAsync<PlanFeature>(pf =>
                pf.Where(f => f.Key == key && f.PlanId == planId)
            );
            return features.FirstOrDefault();
        }

        // --- Validation Methods - Now decorated with [RegisterConditionValidator] ---

        // Assuming the DataFilter will contain the required count in Value and PlanId in Id
        [RegisterConditionValidator(typeof(PlanValidatorStates), PlanValidatorStates.HasEnoughModels, "The number of models is less than allowed by the plan.")]
        private async Task<ConditionResult> CheckHasEnoughModels(DataFilter<int> filter)
        {
            if (string.IsNullOrWhiteSpace(filter.Id))
            {
                return ConditionResult.ToError("Plan ID is required for HasEnoughModels check.");
            }

            var planFeature = await GetPlanFeatureByKeyAsync(PlanFeatureValidatorKeys.NumberModels, filter.Id);

            if (planFeature == null)
            {
                // Consider if missing feature means failure or error. Let's assume failure here.
                return ConditionResult.ToFailure("Plan feature 'NumberModels' not found for the specified plan.");
            }

            if (!int.TryParse(planFeature.Value, out var allowedValue))
            {
                return ConditionResult.ToError($"Invalid value format for '{PlanFeatureValidatorKeys.NumberModels}' feature: {planFeature.Value}");
            }

            // filter.Value holds the required number of models
            return allowedValue >= filter.Value
                ? ConditionResult.ToSuccess(planFeature, "Allowed models count is sufficient.") // Optionally pass the feature object
                : ConditionResult.ToFailure("The number of models required exceeds the limit allowed by the plan.");
        }

        [RegisterConditionValidator(typeof(PlanValidatorStates), PlanValidatorStates.HasAllowedRequests, "The number of requests exceeds the limit allowed by the plan.")]
        private async Task<ConditionResult> CheckHasAllowedRequests(DataFilter<int> filter)
        {
            if (string.IsNullOrWhiteSpace(filter.Id))
            {
                return ConditionResult.ToError("Plan ID is required for HasAllowedRequests check.");
            }

            var planFeature = await GetPlanFeatureByKeyAsync(PlanFeatureValidatorKeys.AllowedRequests, filter.Id);

            if (planFeature == null)
            {
                return ConditionResult.ToFailure("Plan feature 'AllowedRequests' not found for the specified plan.");
            }

            if (planFeature.Value?.Equals("Unlimited", StringComparison.OrdinalIgnoreCase) == true)
                return ConditionResult.ToSuccess(planFeature, "Allowed requests are unlimited.");

            if (!int.TryParse(planFeature.Value, out var allowedValue))
            {
                return ConditionResult.ToError($"Invalid value format for '{PlanFeatureValidatorKeys.AllowedRequests}' feature: {planFeature.Value}");
            }

            // filter.Value holds the required number of requests
            return allowedValue >= filter.Value
                ? ConditionResult.ToSuccess(planFeature, "Allowed requests count is sufficient.")
                : ConditionResult.ToFailure("The number of requests required exceeds the limit allowed by the plan.");
        }

        [RegisterConditionValidator(typeof(PlanValidatorStates), PlanValidatorStates.HasAllowedSpaces, "The required space exceeds the limit allowed by the plan.")]
        private async Task<ConditionResult> CheckHasAllowedSpaces(DataFilter<int> filter)
        {
            if (string.IsNullOrWhiteSpace(filter.Id))
            {
                return ConditionResult.ToError("Plan ID is required for HasAllowedSpaces check.");
            }

            var planFeature = await GetPlanFeatureByKeyAsync(PlanFeatureValidatorKeys.AllowedSpaces, filter.Id);

            if (planFeature == null)
            {
                return ConditionResult.ToFailure("Plan feature 'AllowedSpaces' not found for the specified plan.");
            }

            if (planFeature.Value?.Equals("Unlimited", StringComparison.OrdinalIgnoreCase) == true)
                return ConditionResult.ToSuccess(planFeature, "Allowed spaces are unlimited.");

            if (!int.TryParse(planFeature.Value, out var allowedValue))
            {
                return ConditionResult.ToError($"Invalid value format for '{PlanFeatureValidatorKeys.AllowedSpaces}' feature: {planFeature.Value}");
            }

            // filter.Value holds the required space
            return allowedValue >= filter.Value
                ? ConditionResult.ToSuccess(planFeature, "Allowed space is sufficient.")
                : ConditionResult.ToFailure("The required space exceeds the limit allowed by the plan.");
        }

        // Assuming the DataFilter just needs the PlanId (in Id) for this check.
        // The method checks if the PlanFeature.Value == "shared".
        // The DataFilter<int> type matches the original registration type, but Value is unused for comparison.
        [RegisterConditionValidator(typeof(PlanValidatorStates), PlanValidatorStates.HasSharedProcessor, "The plan does not offer a shared processor.")]
        private async Task<ConditionResult> CheckHasSharedProcessor(DataFilter<int> filter)
        {
            if (string.IsNullOrWhiteSpace(filter.Id))
            {
                return ConditionResult.ToError("Plan ID is required for HasSharedProcessor check.");
            }

            var planFeature = await GetPlanFeatureByKeyAsync(PlanFeatureValidatorKeys.Processor, filter.Id);

            if (planFeature == null)
            {
                return ConditionResult.ToFailure("Plan feature 'Processor' not found for the specified plan.");
            }

            // Check if the actual feature value is "shared"
            return planFeature.Value?.Equals("shared", StringComparison.OrdinalIgnoreCase) == true
                ? ConditionResult.ToSuccess(planFeature, "Processor is shared.")
                : ConditionResult.ToFailure("Processor is not shared as required.");
        }

        [RegisterConditionValidator(typeof(PlanValidatorStates), PlanValidatorStates.HasEnoughRam, "The RAM is less than required by the plan.")]
        private async Task<ConditionResult> CheckHasEnoughRam(DataFilter<int> filter)
        {
            if (string.IsNullOrWhiteSpace(filter.Id))
            {
                return ConditionResult.ToError("Plan ID is required for HasEnoughRam check.");
            }

            var planFeature = await GetPlanFeatureByKeyAsync(PlanFeatureValidatorKeys.Ram, filter.Id);

            if (planFeature == null)
            {
                return ConditionResult.ToFailure("Plan feature 'Ram' not found for the specified plan.");
            }


            if (!int.TryParse(planFeature.Value, out var allowedValue))
            {
                return ConditionResult.ToError($"Invalid value format for '{PlanFeatureValidatorKeys.Ram}' feature: {planFeature.Value}");
            }

            // filter.Value holds the required RAM
            return allowedValue >= filter.Value
                ? ConditionResult.ToSuccess(planFeature, "Allowed RAM is sufficient.")
                : ConditionResult.ToFailure("The RAM required exceeds the limit allowed by the plan.");
        }


        [RegisterConditionValidator(typeof(PlanValidatorStates), PlanValidatorStates.HasSpeedLimit, "The required speed exceeds the limit allowed by the plan.")]
        private async Task<ConditionResult> CheckHasSpeedLimit(DataFilter<double> filter)
        {
            if (string.IsNullOrWhiteSpace(filter.Id))
            {
                return ConditionResult.ToError("Plan ID is required for HasSpeedLimit check.");
            }

            var planFeature = await GetPlanFeatureByKeyAsync(PlanFeatureValidatorKeys.Speed, filter.Id);

            if (planFeature == null)
            {
                return ConditionResult.ToFailure("Plan feature 'Speed' not found for the specified plan.");
            }


            if (!double.TryParse(planFeature.Value, out var allowedValue))
            {
                return ConditionResult.ToError($"Invalid value format for '{PlanFeatureValidatorKeys.Speed}' feature: {planFeature.Value}");
            }

            // filter.Value holds the required speed
            return allowedValue >= filter.Value
                ? ConditionResult.ToSuccess(planFeature, "Allowed speed is sufficient.")
                : ConditionResult.ToFailure("The required speed exceeds the limit allowed by the plan.");
        }

        // Assuming the DataFilter just needs the PlanId (in Id) for this check.
        // The method checks if the PlanFeature.Value == "no".
        // The DataFilter<string> type matches the original registration type, but Value is unused for comparison.
        [RegisterConditionValidator(typeof(PlanValidatorStates), PlanValidatorStates.SupportDisabled, "Support is enabled for this plan, but should be disabled.")]
        private async Task<ConditionResult> CheckSupportDisabled(DataFilter<string> filter)
        {
            if (string.IsNullOrWhiteSpace(filter.Id))
            {
                return ConditionResult.ToError("Plan ID is required for SupportDisabled check.");
            }

            var planFeature = await GetPlanFeatureByKeyAsync(PlanFeatureValidatorKeys.Support, filter.Id);

            if (planFeature == null)
            {
                return ConditionResult.ToFailure("Plan feature 'Support' not found for the specified plan.");
            }

            // Check if the actual feature value is "no"
            return planFeature.Value?.Equals("no", StringComparison.OrdinalIgnoreCase) == true
                ? ConditionResult.ToSuccess(planFeature, "Support is disabled.")
                : ConditionResult.ToFailure("Support is not disabled as required.");
        }

        // Assuming the DataFilter just needs the PlanId (in Id) for this check.
        // The method checks if the PlanFeature.Value == "no".
        // The DataFilter<string> type matches the original registration type, but Value is unused for comparison.
        [RegisterConditionValidator(typeof(PlanValidatorStates), PlanValidatorStates.CustomizationDisabled, "Customization is enabled for this plan, but should be disabled.")]
        private async Task<ConditionResult> CheckCustomizationDisabled(DataFilter<string> filter)
        {
            if (string.IsNullOrWhiteSpace(filter.Id))
            {
                return ConditionResult.ToError("Plan ID is required for CustomizationDisabled check.");
            }

            var planFeature = await GetPlanFeatureByKeyAsync(PlanFeatureValidatorKeys.Customization, filter.Id);

            if (planFeature == null)
            {
                return ConditionResult.ToFailure("Plan feature 'Customization' not found for the specified plan.");
            }

            // Check if the actual feature value is "no"
            return planFeature.Value?.Equals("no", StringComparison.OrdinalIgnoreCase) == true
                ? ConditionResult.ToSuccess(planFeature, "Customization is disabled.")
                : ConditionResult.ToFailure("Customization is not disabled as required.");
        }

        // --- Removed old methods not following the new pattern ---
        // Removed: RegisterCondition (helper method)
        // Removed: GetFeatureByKey (inlined logic or replaced by GetPlanFeatureByKeyAsync)
        // Removed: getPlanActive
        // Removed: GetPlanFeatureActive
        // Removed: CheckActive (incomplete)
        // Removed: GetModel (replaced by GetPlanFeatureByKeyAsync)
        // Removed: getPlan (if needed, it should use QueryDbSet) - Let's keep a simple getPlan using QueryDbSet just in case, but it's not strictly needed for the feature checks.
        private async Task<Plan?> GetPlanByIdAsync(string? id)
        {
            if (string.IsNullOrWhiteSpace(id)) return null;

            var plans = await QueryListAsync<Plan>(p =>
                p.Where(x => x.Id == id)
                 .Include(p => p.PlanFeatures) // Include features if needed for other checks
            );

            //var plan = await QuerySingleAsync<Plan>(db => db.Where(p => p.Id == id).Include(p => p.PlanFeatures));

            return plans.FirstOrDefault();
        }
    }
}