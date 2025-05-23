using LAHJAAPI.Models;
using LAHJAAPI.V1.Validators.Conditions;
using Quartz.Util;
using WasmAI.ConditionChecker.Base;

namespace LAHJAAPI.V1.Validators
{
    public enum ServiceValidatorStates
    {
        IsFound = 6200,
        IsFull,
        HasName,
        HasAbsolutePath,
        IsCreateSpace,
        IsDashboard,
        HasToken,
        HasValidModelAi,
        HasMethods,
        HasRequests,
        IsLinkedToUsers,
        HasId,
        HasModelAi,
        HasLinkedUsers,
        IsServiceModel,
        IsServiceIdsEmpty,
        IsInAbsolutePath,
        IsServiceEqual,
        IsCreateSpaceIn,
        IsInName,
    }

    public class ServiceType
    {
        public const string Dashboard = "dashboard";
        public const string CreateSpace = "createspace";
        public const string Service = "service";
    }

    public class ServiceValidator : ValidatorContext<Service, ServiceValidatorStates>
    {
        private Service? _service; // Cache

        public ServiceValidator(IConditionChecker checker) : base(checker)
        {
        }

        protected override void InitializeConditions()
        {
            // Register conditions here if needed
        }

        [RegisterConditionValidator(typeof(ServiceValidatorStates), ServiceValidatorStates.IsFound, "Service is not found")]
        private Task<ConditionResult> ValidateId(DataFilter<string, Service> f)
        {
            bool valid = !string.IsNullOrWhiteSpace(f.Share?.Id);
            return valid
                ? ConditionResult.ToSuccessAsync(f.Share)
                : ConditionResult.ToFailureAsync("Service is not found");
        }

        [RegisterConditionValidator(typeof(ServiceValidatorStates), ServiceValidatorStates.HasName, "Name is required")]
        private Task<ConditionResult> ValidateName(DataFilter<string, Service> f)
        {
            bool valid = !string.IsNullOrWhiteSpace(f.Share?.Name);
            return valid
                ? ConditionResult.ToSuccessAsync(f.Share?.Name)
                : ConditionResult.ToFailureAsync(f.Share?.Name, "Name is required");
        }

        [RegisterConditionValidator(typeof(ServiceValidatorStates), ServiceValidatorStates.HasAbsolutePath, "AbsolutePath is invalid")]
        private Task<ConditionResult> ValidateAbsolutePath(DataFilter<string, Service> f)
        {
            bool valid = Uri.IsWellFormedUriString(f.Share?.AbsolutePath, UriKind.Absolute);
            return valid
                ? ConditionResult.ToSuccessAsync(f.Share?.AbsolutePath)
                : ConditionResult.ToFailureAsync(f.Share?.AbsolutePath, "AbsolutePath is invalid");
        }

        [RegisterConditionValidator(typeof(ServiceValidatorStates), ServiceValidatorStates.HasModelAi, "Model AI is missing")]
        private async Task<ConditionResult> ValidateModelAi(DataFilter<string, Service> f)
        {
            if (f.Share != null)
            {
                var res = await _checker.CheckAndResultAsync(ModelValidatorStates.IsHasModelGateway, f.Share.ModelAiId);

                if (res.Success == true)
                {
                    f.Share.ModelAi = (ModelAi?)res.Result;
                    return ConditionResult.ToSuccess(f.Share);
                }
                else
                {
                    return ConditionResult.ToFailure(f.Share, res.Message);
                }
            }

            return ConditionResult.ToFailure(f.Share, "No found service");
        }

        [RegisterConditionValidator(typeof(ServiceValidatorStates), ServiceValidatorStates.HasMethods, "No methods defined for service")]
        private Task<ConditionResult> ValidateMethods(DataFilter<string, Service> f)
        {
            bool valid = f.Share?.ServiceMethods != null && f.Share.ServiceMethods.Any();
            return valid
                ? ConditionResult.ToSuccessAsync(f.Share?.ServiceMethods)
                : ConditionResult.ToFailureAsync(f.Share?.ServiceMethods, "No methods defined for service");
        }

        [RegisterConditionValidator(typeof(ServiceValidatorStates), ServiceValidatorStates.HasLinkedUsers, "Service is not linked to any user")]
        private Task<ConditionResult> ValidateLinkedUsers(DataFilter<string, Service> f)
        {
            bool valid = f.Share?.UserServices != null && f.Share.UserServices.Any();
            return valid
                ? ConditionResult.ToSuccessAsync(f.Share?.UserServices)
                : ConditionResult.ToFailureAsync(f.Share?.UserServices, "Service is not linked to any user");
        }

        [RegisterConditionValidator(typeof(ServiceValidatorStates), ServiceValidatorStates.IsInAbsolutePath, "Service is in AbsolutePath")]
        private async Task<ConditionResult> IsInAbsolutePath(DataFilter<List<string>, Service> f)
        {
            foreach (var val in f.Value)
            {
                if (f.Share.AbsolutePath == val)
                    return ConditionResult.ToFailure(val, $"Service {val} is in AbsolutePath");
            }
            return ConditionResult.ToSuccess(f.Share);

        }

        [RegisterConditionValidator(typeof(ServiceValidatorStates), ServiceValidatorStates.IsInName, "Service is not in Name")]
        private async Task<ConditionResult> IsInName(DataFilter<List<string>, Service> f)
        {
            return new ConditionResult(f.Value?.Count > 0 && f.Value.Contains(f.Share.Name), f.Share, "Service is not in Name");

        }

        //[RegisterConditionValidator(typeof(ServiceValidatorStates), ServiceValidatorStates.IsServiceIdsEmpty, "User has no services")]
        //private Task<ConditionResult> ValidateServiceIdsEmpty(DataFilter<bool> f)
        //{
        //    // Replace with appropriate logic to check if service IDs are empty
        //    bool isEmpty = false; // Implement logic here
        //    return isEmpty
        //        ? ConditionResult.ToSuccessAsync(isEmpty)
        //        : ConditionResult.ToFailureAsync(isEmpty, "User has services");
        //}

        [RegisterConditionValidator(typeof(ServiceValidatorStates), ServiceValidatorStates.IsServiceEqual, "Not a valid service")]
        private async Task<ConditionResult> IsServiceEqual(DataFilter<string, Service> f)
        {
            if (f.Share == null || (f.Value == null && f.Name == null))
                return ConditionResult.ToError("Both Name and Value are null");

            if (!f.Name.IsNullOrWhiteSpace())
                return new ConditionResult(
                    f.Share.Name.Equals(f.Name, StringComparison.OrdinalIgnoreCase),
                    f.Share,
                    $"Service are not equal {f.Name}.");


            return new ConditionResult(
                f.Share.AbsolutePath.Equals(f.Value, StringComparison.OrdinalIgnoreCase),
                f.Share,
                $"Service are not equal {f.Value}.");
        }

        [RegisterConditionValidator(typeof(ServiceValidatorStates), ServiceValidatorStates.IsCreateSpaceIn)]
        private async Task<ConditionResult> IsCreateSpaceIn(DataFilter<List<string>> f)
        {
            if (f.Value == null || f.Value.Count == 0)
                return ConditionResult.ToError("Value must contain service IDs");

            var service = await QueryFirstOrDefaultAsync<Service>(x => x.AbsolutePath == ServiceType.CreateSpace);

            return new ConditionResult(f.Value.Contains(service?.Id ?? string.Empty), service, $"Service {ServiceType.CreateSpace} {f.Value}.");
        }

        protected override async Task<Service?> GetModel(string? id)
        {
            if (_service != null && _service.Id == id)
                return _service;

            _service = await base.GetModel(id);
            return _service;
        }
    }
}
