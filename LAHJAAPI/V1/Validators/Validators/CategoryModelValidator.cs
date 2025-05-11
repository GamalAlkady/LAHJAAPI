using AutoGenerator;
using AutoGenerator.Helper.Translation;
using AutoGenerator.Conditions;
using LAHJAAPI.Models;
using System;
using LAHJAAPI.V1.Validators.Conditions;

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
    } //
    //  Base
    public  enum  CategoryModelValidatorStates //
    { IsActive ,  IsFull ,  IsValid ,  //
    }

}