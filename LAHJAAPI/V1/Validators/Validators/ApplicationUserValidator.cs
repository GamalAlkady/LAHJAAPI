using LAHJAAPI.Models;
using LAHJAAPI.V1.Validators.Conditions;
using Microsoft.EntityFrameworkCore;
using Quartz.Util;
using V1.Validators;
using WasmAI.ConditionChecker.Base;

namespace LAHJAAPI.V1.Validators
{
    public enum ApplicationUserValidatorStates
    {
        IsActive = 6500,
        IsFull,
        HasCustomerId,
        HasProfileUrl,
        IsNotArchived,
        HasArchivedDate,
        IsHasModelAi,
        IsModelAssigned,
        CanAssignService,
        IsValid,
        HasRequests,
        HasUserServices,
        HasUserModelAis,
        HasSubscription,
        HasSubscriptionId,
        HasModelGateways,
        HasModelGatewayId,
        IsArchived,
        HasId
    }

    public class ApplicationUserValidator : ValidatorContext<ApplicationUser, ApplicationUserValidatorStates>
    {
        private ApplicationUser? _applicationUser;

        public ApplicationUserValidator(IConditionChecker checker) : base(checker)
        {
        }

        protected override void InitializeConditions()
        {
        }

        [RegisterConditionValidator(typeof(ApplicationUserValidatorStates), ApplicationUserValidatorStates.HasId, "User ID is required.")]
        private Task<ConditionResult> ValidateId(DataFilter<string, ApplicationUser> f)
        {
            bool valid = !string.IsNullOrWhiteSpace(f.Share?.Id);
            return valid ? ConditionResult.ToSuccessAsync(f.Share) : ConditionResult.ToFailureAsync("User ID is required.");
        }

        [RegisterConditionValidator(typeof(ApplicationUserValidatorStates), ApplicationUserValidatorStates.HasCustomerId, "Customer ID is required.")]
        private Task<ConditionResult> ValidateCustomerId(DataFilter<string?, ApplicationUser> f)
        {
            bool valid = string.IsNullOrWhiteSpace(f.Share?.CustomerId);
            return valid ? ConditionResult.ToFailureAsync("Customer ID is required.") : ConditionResult.ToSuccessAsync(f.Share?.CustomerId);
        }

        [RegisterConditionValidator(typeof(ApplicationUserValidatorStates), ApplicationUserValidatorStates.HasProfileUrl, "Profile URL is required.")]
        private Task<ConditionResult> ValidateProfileUrl(DataFilter<string?, ApplicationUser> f)
        {
            bool valid = string.IsNullOrWhiteSpace(f.Share?.ProfileUrl);
            return valid ? ConditionResult.ToFailureAsync("Profile URL is required.") : ConditionResult.ToSuccessAsync(f.Share?.ProfileUrl);
        }



        [RegisterConditionValidator(typeof(ApplicationUserValidatorStates), ApplicationUserValidatorStates.IsArchived, "User is archived.")]
        private Task<ConditionResult> CheckIsArchived(DataFilter<bool, ApplicationUser> f)
        {
            bool valid = f.Share?.IsArchived == true;
            return Task.FromResult(valid ? ConditionResult.ToSuccess(f.Share) : ConditionResult.ToFailure("User is not archived."));
        }



        [RegisterConditionValidator(typeof(ApplicationUserValidatorStates), ApplicationUserValidatorStates.HasModelGatewayId, "Model Gateway ID is required.")]
        private Task<ConditionResult> ValidateModelGatewayId(DataFilter<string?, ApplicationUser> f)
        {
            bool valid = string.IsNullOrWhiteSpace(f.Share?.ModelGatewayId);
            return valid ? ConditionResult.ToFailureAsync("Model Gateway ID is required.") : ConditionResult.ToSuccessAsync(f.Share?.ModelGatewayId);
        }

        [RegisterConditionValidator(typeof(ApplicationUserValidatorStates), ApplicationUserValidatorStates.HasModelGateways, "Model Gateways collection cannot be empty.")]
        private Task<ConditionResult> ValidateModelGateways(DataFilter<ICollection<ModelGateway>?, ApplicationUser> f)
        {
            bool valid = f.Share?.ModelGateways != null && f.Share.ModelGateways.Any();
            return Task.FromResult(valid ? ConditionResult.ToSuccess(f.Share?.ModelGateways) : ConditionResult.ToFailure("Model Gateways collection cannot be empty."));
        }

        [RegisterConditionValidator(typeof(ApplicationUserValidatorStates), ApplicationUserValidatorStates.HasSubscriptionId, "Subscription ID is required.")]
        private Task<ConditionResult> ValidateSubscriptionId(DataFilter<string?, ApplicationUser> f)
        {
            bool valid = string.IsNullOrWhiteSpace(f.Share?.SubscriptionId);
            return valid ? ConditionResult.ToFailureAsync("Subscription ID is required.") : ConditionResult.ToSuccessAsync(f.Share?.SubscriptionId);
        }

        [RegisterConditionValidator(typeof(ApplicationUserValidatorStates), ApplicationUserValidatorStates.HasSubscription, "Subscription object is required.")]
        private Task<ConditionResult> ValidateSubscription(DataFilter<Subscription?, ApplicationUser> f)
        {
            bool valid = f.Share?.Subscription != null;
            return Task.FromResult(valid ? ConditionResult.ToSuccess(f.Share?.Subscription) : ConditionResult.ToFailure("Subscription object is required."));
        }

        //TODO: Test ValidateUserModelAis if working
        [RegisterConditionValidator(typeof(ApplicationUserValidatorStates), ApplicationUserValidatorStates.HasUserModelAis, "User Model AIs collection cannot be empty.")]
        private Task<ConditionResult> ValidateUserModelAis(DataFilter<ICollection<UserModelAi>, ApplicationUser> f)
        {
            bool valid = f.Share?.UserModelAis != null && f.Share.UserModelAis.Any();
            return Task.FromResult(valid ? ConditionResult.ToSuccess(f.Share?.UserModelAis) : ConditionResult.ToFailure("User Model AIs collection cannot be empty."));
        }

        //TODO: Test if UserService is not null and has UserServices
        [RegisterConditionValidator(typeof(ApplicationUserValidatorStates), ApplicationUserValidatorStates.HasUserServices, "User Services collection cannot be empty.")]
        private Task<ConditionResult> ValidateUserServices(DataFilter<ICollection<UserService>, ApplicationUser> f)
        {
            bool valid = f.Share?.UserServices != null && f.Share.UserServices.Any();
            return Task.FromResult(valid ? ConditionResult.ToSuccess(f.Share?.UserServices) : ConditionResult.ToFailure("User Services collection cannot be empty."));
        }

        [RegisterConditionValidator(typeof(ApplicationUserValidatorStates), ApplicationUserValidatorStates.HasRequests, "Requests collection cannot be empty.")]
        private async Task<ConditionResult> ValidateRequests(DataFilter<ICollection<Request>, ApplicationUser> f)
        {
            int count = await QueryCountAsync<Request>(r => r.UserId == f.Id);
            return count > 0 ? ConditionResult.ToSuccess(f.Share?.Requests) : ConditionResult.ToFailure("User requests are empty.");
        }

        [RegisterConditionValidator(typeof(ApplicationUserValidatorStates), ApplicationUserValidatorStates.IsValid, "User data is invalid.")]
        private async Task<ConditionResult> CheckIsValid(DataFilter<object, ApplicationUser> f)
        {
            if (f.Share == null) return ConditionResult.ToFailure(null, "User is null.");

            var idValid = await ValidateId(new DataFilter<string, ApplicationUser> { Share = f.Share });
            if (idValid.Success == false) return idValid;



            // Add other core validity checks here, e.g., base Identity properties like Email if needed
            // Note: Accessing base IdentityUser properties requires casting f.Share or accessing them directly if public
            if (string.IsNullOrWhiteSpace(f.Share?.Email))
            {
                return ConditionResult.ToFailure(f.Share, "User Email is required for basic validity.");
            }


            return ConditionResult.ToSuccess(f.Share, "User data is valid.");
        }

        [RegisterConditionValidator(typeof(ApplicationUserValidatorStates), ApplicationUserValidatorStates.IsFull, "User data is incomplete.")]
        private async Task<ConditionResult> CheckIsFull(DataFilter<object, ApplicationUser> f)
        {
            if (f.Share == null) return ConditionResult.ToFailure(null, "User is null.");

            var isValid = await CheckIsValid(new DataFilter<object, ApplicationUser> { Share = f.Share });
            if (isValid.Success == false) return isValid;

            // Add checks for other properties considered essential for "fullness"
            var customerIdValid = await ValidateCustomerId(new DataFilter<string, ApplicationUser> { Share = f.Share });
            if (customerIdValid.Success == false) return customerIdValid;

            var subscriptionIdValid = await ValidateSubscriptionId(new DataFilter<string, ApplicationUser> { Share = f.Share });
            if (subscriptionIdValid.Success == false) return subscriptionIdValid;

            var subscriptionValid = await ValidateSubscription(new DataFilter<Subscription, ApplicationUser> { Share = f.Share });
            if (subscriptionValid.Success == false) return subscriptionValid;

            // Add more checks for FirstName, LastName, DisplayName etc if required for full state

            return ConditionResult.ToSuccess(f.Share, "User data is complete.");
        }

        [RegisterConditionValidator(typeof(ApplicationUserValidatorStates), ApplicationUserValidatorStates.IsModelAssigned, "Model AI not belong to user.")]
        async Task<ConditionResult> IsModelAssigned(DataFilter<string> data)
        {
            if (data.Id.IsNullOrWhiteSpace()) return ConditionResult.ToError("Id is null.");
            if (data.Value.IsNullOrWhiteSpace()) return ConditionResult.ToError("Value is null.");

            var result = await _injector.ContextFactory.ExecuteInScopeAsync(ctx =>
                ctx.Set<UserModelAi>().Include(um => um.ModelAi).FirstOrDefaultAsync(um => um.UserId == data.Id && um.ModelAiId == data.Value)
            );

            return result != null
                ? ConditionResult.ToSuccess(result, "ModelAi allready assigned to user.")
                : ConditionResult.ToError($"Model AI with ID: ({data.Value}) not assigned to user.");
        }

        [RegisterConditionValidator(typeof(ApplicationUserValidatorStates), ApplicationUserValidatorStates.CanAssignService)]
        async Task<ConditionResult> CanAssignService(DataFilter<string> data)
        {
            if (data.Id.IsNullOrWhiteSpace()) return ConditionResult.ToError("Id is null.");
            if (data.Value.IsNullOrWhiteSpace()) return ConditionResult.ToError("Value is null.");

            if (await _checker.CheckAndResultAsync(UserServiceValidatorStates.IsServiceAssigned, new DataFilter(data.Id) { Value = data.Value }) is { Success: true } res1)
            {
                return ConditionResult.ToFailure(res1.Result, res1.Message);
            }

            var resultService = await _checker.CheckAndResultAsync(ServiceValidatorStates.IsFound, data.Value);
            if (resultService.Success.GetValueOrDefault())
            {
                var service = (Service)resultService.Result!;
                var res = await IsModelAssigned(new DataFilter<string> { Id = data.Id, Value = service.ModelAiId });
                //if (res.Success.GetValueOrDefault())
                //{
                //    service.ModelAi = ((UserModelAi?)res.Result).ModelAi;
                //    return result.Success.GetValueOrDefault()
                //        ? ConditionResult.ToSuccess(service, "Service Already assigned to user.")
                //        : ConditionResult.ToError($"Service with ID: ({data.Value}) not assigned to user.");
                //}
                return res;
            }
            return ConditionResult.ToError(resultService.Message ?? "Service not found!.");

        }

        protected override async Task<ApplicationUser?> GetModel(string? id)
        {
            if (_applicationUser != null && _applicationUser.Id == id)
                return _applicationUser;
            _applicationUser = await base.GetModel(id);
            return _applicationUser;
        }
    }
}
