using AutoGenerator.Conditions;
using LAHJAAPI.V1.Validators.Conditions;
using V1.DyModels.Dso.ResponseFilters;
using WasmAI.ConditionChecker.Base;

namespace LAHJAAPI.V1.Validators
{
    public class SettingValidator : BaseValidator<SettingResponseFilterDso, SettingValidatorStates>, ITValidator
    {

        public SettingValidator(IConditionChecker checker) : base(checker)
        {
        }

        protected override void InitializeConditions()
        {
            _provider.Register(SettingValidatorStates.IsActive,
                new LambdaCondition<SettingResponseFilterDso>(nameof(SettingValidatorStates.IsActive),
                context => IsActive(context), "Setting is not active"));


        }

        private bool IsActive(SettingResponseFilterDso context)
        {
            if (context != null)
            {
                return true;
            }

            return false;
        }
    } //
    //  Base
    public enum SettingValidatorStates //
    {
        IsActive, IsFull, IsValid,  //
    }

}