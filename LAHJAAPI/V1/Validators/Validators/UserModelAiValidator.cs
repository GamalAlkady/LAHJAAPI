using AutoGenerator.Conditions;
using LAHJAAPI.Models;
using LAHJAAPI.V1.Validators.Conditions;

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
    } //
    //  Base
    public enum UserModelAiValidatorStates //
    {
        IsActive, IsFull, IsValid,  //
    }

}