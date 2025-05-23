using AutoGenerator.Conditions;
using LAHJAAPI.Models;
using LAHJAAPI.V1.Validators.Conditions;
using WasmAI.ConditionChecker.Base;

namespace V1.Validators
{
    public enum PlanFeatureValidatorStates
    {
        HasName,
        HasDescription,
        HasKey,
        HasPlanId,

    }

    public class PlanFeatureValidatorContext : ValidatorContext<PlanFeature, PlanFeatureValidatorStates>, ITValidator
    {
        public PlanFeatureValidatorContext(IConditionChecker checker) : base(checker)
        {
        }

        protected override void InitializeConditions()
        {
        }

        [RegisterConditionValidator(typeof(PlanFeatureValidatorStates), PlanFeatureValidatorStates.HasName, "Name is required")]
        private Task<ConditionResult> ValidateName(DataFilter<string, PlanFeature> f)
        {
            bool isValid = !string.IsNullOrWhiteSpace(f.Share?.Name);
            return isValid ? ConditionResult.ToSuccessAsync(f.Share?.Name) : ConditionResult.ToFailureAsync("Name is required");
        }

        [RegisterConditionValidator(typeof(PlanFeatureValidatorStates), PlanFeatureValidatorStates.HasDescription, "Description is required")]
        private Task<ConditionResult> ValidateDescription(DataFilter<string, PlanFeature> f)
        {
            bool isValid = !string.IsNullOrWhiteSpace(f.Share?.Description);
            return isValid ? ConditionResult.ToSuccessAsync(f.Share?.Description) : ConditionResult.ToFailureAsync("Description is required");
        }

        [RegisterConditionValidator(typeof(PlanFeatureValidatorStates), PlanFeatureValidatorStates.HasKey, "Key is required")]
        private Task<ConditionResult> ValidateKey(DataFilter<string, PlanFeature> f)
        {
            bool isValid = !string.IsNullOrWhiteSpace(f.Share?.Key);
            return isValid ? ConditionResult.ToSuccessAsync(f.Share?.Key) : ConditionResult.ToFailureAsync("Key is required");
        }

        [RegisterConditionValidator(typeof(PlanFeatureValidatorStates), PlanFeatureValidatorStates.HasPlanId, "PlanId is required")]
        private Task<ConditionResult> ValidatePlanId(DataFilter<string, PlanFeature> f)
        {
            bool isValid = !string.IsNullOrWhiteSpace(f.Share?.PlanId);
            return isValid ? ConditionResult.ToSuccessAsync(f.Share?.PlanId) : ConditionResult.ToFailureAsync("PlanId is required");
        }



        protected override async Task<PlanFeature?> GetModel(string? id)
        {
            return await base.GetModel(id);
        }

    }


}