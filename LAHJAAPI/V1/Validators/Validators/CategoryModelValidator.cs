using AutoGenerator.Conditions;
using LAHJAAPI.Models;
using LAHJAAPI.V1.Validators.Conditions;
using WasmAI.ConditionChecker.Base;

namespace V1.Validators
{
    public class CategoryModelValidatorContext : ValidatorContext<CategoryModel, CategoryModelValidatorStates>, ITValidator
    {
        public CategoryModelValidatorContext(IConditionChecker checker) : base(checker)
        {
        }

        protected override void InitializeConditions()
        {
        }

        [RegisterConditionValidator(typeof(CategoryModelValidatorStates), CategoryModelValidatorStates.IsValid, "Category name is required.")]
        private Task<ConditionResult> ValidateName(DataFilter<string, CategoryModel> f)
        {
            if (string.IsNullOrWhiteSpace(f.Share?.Name))
            {
                return ConditionResult.ToFailureAsync("Category name is required.");
            }
            return ConditionResult.ToSuccessAsync(f.Share?.Name);
        }


        [RegisterConditionValidator(typeof(CategoryModelValidatorStates), CategoryModelValidatorStates.IsActive, "Category description is required.")]
        private Task<ConditionResult> ValidateDescription(DataFilter<string, CategoryModel> f)
        {
            return string.IsNullOrWhiteSpace(f.Share?.Description) ?
                ConditionResult.ToFailureAsync("Category description is required.") :
                ConditionResult.ToSuccessAsync(f.Share?.Description);
        }
        protected override async Task<CategoryModel?> GetModel(string? id)
        {
            if (id == null) return null;

            return await base.GetModel(id);
        }
    }

    public enum CategoryModelValidatorStates
    {
        IsValid,
        IsActive,
        IsFull
    }
}