using AutoGenerator.Conditions;
using LAHJAAPI.Models;
using LAHJAAPI.V1.Validators.Conditions;
using Quartz.Util;
using WasmAI.ConditionChecker.Base;

namespace V1.Validators
{
    public class UserServiceValidatorContext : ValidatorContext<UserService, UserServiceValidatorStates>, ITValidator
    {
        public UserServiceValidatorContext(IConditionChecker checker) : base(checker)
        {
        }

        protected override void InitializeConditions()
        {
        }

        [RegisterConditionValidator(typeof(UserServiceValidatorStates), UserServiceValidatorStates.IsServiceAssigned, IsCachability = false)]
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

    } //
    //  Base
    public enum UserServiceValidatorStates //
    {
        IsActive, IsFull, IsValid,  //
        IsServiceAssigned,
    }

}