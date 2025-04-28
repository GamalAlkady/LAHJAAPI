using AutoGenerator.Conditions;
using LAHJAAPI.Models;
using LAHJAAPI.V1.Validators.Conditions;

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
        IsInUserClaims,
    }

    public class ServiceType
    {
        public const string Dash = "dashboard";

        public const string Space = "createspace";
        public const string Service = "service";

    }
    public class ServiceValidator : ValidatorContext<Service, ServiceValidatorStates>
    {

        private Service? _service; // Cacha

        public ServiceValidator(IConditionChecker checker) : base(checker)
        {

        }

        protected override void InitializeConditions()
        {
            //RegisterCondition<string>(ServiceValidatorStates.HasId, ValidateId, "Id is required");
            //RegisterCondition<string>(ServiceValidatorStates.IsServiceDashboard, ValidateIsServiceType, "Not a valid dashboard model", ServiceType.Dash);

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
                var res = await _checker.CheckAndResultAsync(ModelValidatorStates.HasService, f.Share.ModelAiId);

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

        [RegisterConditionValidator(typeof(ServiceValidatorStates), ServiceValidatorStates.IsInUserClaims, "Service is not in user claims")]
        private Task<ConditionResult> ValidateServiceInUserClaims(DataFilter<string, Service> f)
        {
            bool valid = _injector.UserClaims.ServicesIds?.Contains(f.Id) ?? false;
            return valid
                ? ConditionResult.ToSuccessAsync(f.Id)
                : ConditionResult.ToFailureAsync(f.Id, "Service is not in user claims");
        }

        [RegisterConditionValidator(typeof(ServiceValidatorStates), ServiceValidatorStates.IsServiceIdsEmpty, "User has no services")]
        private Task<ConditionResult> ValidateServiceIdsEmpty(DataFilter<bool> f)
        {
            bool isEmpty = _injector.UserClaims.ServicesIds?.Count == 0;
            return isEmpty
                ? ConditionResult.ToSuccessAsync(isEmpty)
                : ConditionResult.ToFailureAsync(isEmpty, "User has services");
        }

        [RegisterConditionValidator(typeof(ServiceValidatorStates), ServiceValidatorStates.IsServiceModel, "Not a valid service model")]
        [RegisterConditionValidator(typeof(ServiceValidatorStates), ServiceValidatorStates.IsDashboard, "Not a valid service model", Value = ServiceType.Dash)]
        [RegisterConditionValidator(typeof(ServiceValidatorStates), ServiceValidatorStates.IsCreateSpace, "Not a valid service model", Value = ServiceType.Space)]
        private Task<ConditionResult> ValidateIsServiceType(DataFilter<string, Service> f)
        {
            if (f.Share == null && f.Value == null && f.Name == null)
                return Task.FromResult(ConditionResult.ToFailure(f.Share?.AbsolutePath, "Both Name and Value are null"));
            if (f.Share != null)
            {
                return Task.FromResult(new ConditionResult(f.Share.AbsolutePath.Equals(f.Name ?? f.Value, StringComparison.OrdinalIgnoreCase), f.Share));
            }
            f.Share = _injector.Context.Services.FirstOrDefault(x => x.AbsolutePath.Contains(f.Name ?? f.Value!));
            bool valid = f.Share != null;
            return valid
                ? ConditionResult.ToSuccessAsync(f.Share)
                : ConditionResult.ToFailureAsync(f.Name ?? f.Value);
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