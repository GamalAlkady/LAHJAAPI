using AutoGenerator.Conditions;
using LAHJAAPI.Models;
using LAHJAAPI.V1.Validators.Conditions;
using Quartz.Util;
using WasmAI.ConditionChecker.Base;

namespace V1.Validators
{
    public class UserServiceValidatorContext : ValidatorContext<UserService, UserServiceValidatorStates>, ITValidator
    {
        private UserService? _userService;

        public UserServiceValidatorContext(IConditionChecker checker) : base(checker)
        {
        }

        protected override void InitializeConditions()
        {
        }

        [RegisterConditionValidator(typeof(UserServiceValidatorStates), UserServiceValidatorStates.HasUserId, "User ID is required.")]
        private Task<ConditionResult> ValidateUserId(DataFilter<string, UserService> f)
        {
            return string.IsNullOrWhiteSpace(f.Share?.UserId)
                ? ConditionResult.ToFailureAsync("User ID is required.")
                : ConditionResult.ToSuccessAsync(f.Share);
        }

        [RegisterConditionValidator(typeof(UserServiceValidatorStates), UserServiceValidatorStates.HasServiceId, "Service ID is required.")]
        private Task<ConditionResult> ValidateServiceId(DataFilter<string, UserService> f)
        {
            return string.IsNullOrWhiteSpace(f.Share?.ServiceId)
                ? ConditionResult.ToFailureAsync("Service ID is required.")
                : ConditionResult.ToSuccessAsync(f.Share);
        }

        [RegisterConditionValidator(typeof(UserServiceValidatorStates), UserServiceValidatorStates.IsServiceAssigned)]
        async Task<ConditionResult> IsServiceAssigned(DataFilter<string> data)
        {
            try
            {
                if (data.Id.IsNullOrWhiteSpace()) return ConditionResult.ToError("Id is null.");
                if (data.Value.IsNullOrWhiteSpace()) return ConditionResult.ToError("Value is null.");

                var result = await FindContextEntityAsync(data.Id, data.Value);

                return result != null
                    ? ConditionResult.ToSuccess(result, "Service is already assigned to user.")
                    : ConditionResult.ToError($"Service with ID: ({data.Value}) not assigned to user.");
            }
            catch (Exception)
            {

                throw;
            }
        }

        [RegisterConditionValidator(typeof(UserServiceValidatorStates), UserServiceValidatorStates.HasAssignedAt, "Assigned date is required.")]
        private Task<ConditionResult> ValidateAssignedAt(DataFilter<DateTime, UserService> f)
        {
            return f.Share?.AssignedAt == f.Value
                ? ConditionResult.ToFailureAsync("Assigned date is required.")
                : ConditionResult.ToSuccessAsync(f.Share);
        }

        protected override async Task<UserService?> GetModel(string? id)
        {
            if (_userService != null && _userService.UserId == id)
                return _userService;
            _userService = await base.GetModel(id);
            return _userService;
        }
    } //
    //  Base
    public enum UserServiceValidatorStates //
    {
        IsServiceAssigned,
        HasUserId,
        HasServiceId,
        HasAssignedAt,
    }

}