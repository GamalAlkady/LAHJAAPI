using AutoGenerator.Conditions;
using LAHJAAPI.Models;
using LAHJAAPI.V1.Validators.Conditions;
using Quartz.Util;
using WasmAI.ConditionChecker.Base;

namespace V1.Validators
{
    public class UserModelAiValidatorContext : ValidatorContext<UserModelAi, UserModelAiValidatorStates>, ITValidator
    {
        public UserModelAiValidatorContext(IConditionChecker checker) : base(checker)
        {
        }

        protected override void InitializeConditions()
        {
        }

        [RegisterConditionValidator(typeof(UserModelAiValidatorStates), UserModelAiValidatorStates.HasUserId, "User ID is required.")]
        private Task<ConditionResult> ValidateUserId(DataFilter<string, UserModelAi> f)
        {
            return string.IsNullOrWhiteSpace(f.Share?.UserId)
                ? ConditionResult.ToFailureAsync("User ID is required.")
                : ConditionResult.ToSuccessAsync(f.Share);
        }

        [RegisterConditionValidator(typeof(UserModelAiValidatorStates), UserModelAiValidatorStates.HasModelAiId, "ModelAi ID is required.")]
        private Task<ConditionResult> ValidateModelId(DataFilter<string, UserModelAi> f)
        {
            return string.IsNullOrWhiteSpace(f.Share?.ModelAiId)
                ? ConditionResult.ToFailureAsync("ModelAi ID is required.")
                : ConditionResult.ToSuccessAsync(f.Share);
        }

        [RegisterConditionValidator(typeof(UserModelAiValidatorStates), UserModelAiValidatorStates.IsModelAiAssigned)]
        async Task<ConditionResult> IsModelAssigned(DataFilter<string> data)
        {
            try
            {
                if (data.Id.IsNullOrWhiteSpace()) return ConditionResult.ToError("Id is null.");
                if (data.Value.IsNullOrWhiteSpace()) return ConditionResult.ToError("Value is null.");

                var result = await FindContextEntityAsync(data.Id, data.Value);

                return result != null
                    ? ConditionResult.ToSuccess(result, "ModelAi is already assigned to user.")
                    : ConditionResult.ToError($"ModelAi with ID: ({data.Value}) not assigned to user.");
            }
            catch (Exception)
            {

                throw;
            }
        }

        [RegisterConditionValidator(typeof(UserModelAiValidatorStates), UserModelAiValidatorStates.HasAssignedAt, "Assigned date is required.")]
        private Task<ConditionResult> ValidateAssignedAt(DataFilter<DateTime, UserModelAi> f)
        {
            return f.Share?.AssignedAt == f.Value
                ? ConditionResult.ToFailureAsync("Assigned date is required.")
                : ConditionResult.ToSuccessAsync(f.Share);
        }

        private UserModelAi? _userModelAi;
        protected override async Task<UserModelAi?> GetModel(string? id)
        {
            if (_userModelAi != null && _userModelAi.UserId == id)
                return _userModelAi;
            _userModelAi = await base.GetModel(id);
            return _userModelAi;
        }
    } //
    //  Base
    public enum UserModelAiValidatorStates //
    {
        HasUserId,
        HasModelAiId,
        IsModelAiAssigned,
        HasAssignedAt
    }

}