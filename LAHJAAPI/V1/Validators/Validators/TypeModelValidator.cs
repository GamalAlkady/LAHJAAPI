using AutoGenerator.Conditions;
using LAHJAAPI.Models;
using LAHJAAPI.V1.Validators.Conditions;
using WasmAI.ConditionChecker.Base;

namespace V1.Validators
{
    public class TypeModelValidatorContext : ValidatorContext<TypeModel, TypeModelValidatorStates>, ITValidator
    {
        public TypeModelValidatorContext(IConditionChecker checker) : base(checker)
        {
        }

        protected override void InitializeConditions()
        {
        }

        [RegisterConditionValidator(typeof(TypeModelValidatorStates), TypeModelValidatorStates.IsActive, "The active flag is required")]
        private Task<ConditionResult> ValidateActive(DataFilter<bool, TypeModel> f)
        {
            return Task.FromResult(f.Share?.Active == true ? ConditionResult.ToSuccess(f.Share?.Active) : ConditionResult.ToFailure("The active flag is required"));
        }

        [RegisterConditionValidator(typeof(TypeModelValidatorStates), TypeModelValidatorStates.IsValid, "The name is required")]
        private async Task<ConditionResult> ValidateName(DataFilter<string, TypeModel> f)
        {
            bool isValid = !string.IsNullOrWhiteSpace(f.Share?.Name);
            return isValid ? ConditionResult.ToSuccess(f.Share?.Name) : ConditionResult.ToFailure("The name is required");
        }

        [RegisterConditionValidator(typeof(TypeModelValidatorStates), TypeModelValidatorStates.IsFull, "The description is required")]
        private async Task<ConditionResult> ValidateDescription(DataFilter<string, TypeModel> f)
        {
            bool isValid = !string.IsNullOrWhiteSpace(f.Share?.Description);
            return isValid ? ConditionResult.ToSuccess(f.Share?.Description) : ConditionResult.ToFailure("The description is required");
        }

        private TypeModel? _userModelAi;
        protected override async Task<TypeModel?> GetModel(string? id)
        {
            if (_userModelAi != null && _userModelAi.Id == id)
                return _userModelAi;
            _userModelAi = await base.GetModel(id);
            return _userModelAi;
        }
    }

    public enum TypeModelValidatorStates
    {
        IsActive,
        IsFull,
        IsValid
    }
}