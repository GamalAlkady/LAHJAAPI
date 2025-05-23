using AutoGenerator.Conditions;
using LAHJAAPI.Models;
using LAHJAAPI.V1.Validators.Conditions;

namespace V1.Validators
{
    public class CategoryTabValidatorContext : ValidatorContext<CategoryTab, CategoryTabValidatorStates>, ITValidator
    {
        public CategoryTabValidatorContext(IConditionChecker checker) : base(checker)
        {
        }

        protected override void InitializeConditions()
        {
        }
    } //
    //  Base
    public enum CategoryTabValidatorStates //
    {
        IsActive, IsFull, IsValid,  //
    }

}