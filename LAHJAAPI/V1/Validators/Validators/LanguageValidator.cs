using AutoGenerator;
using AutoGenerator.Helper.Translation;
using AutoGenerator.Conditions;
using LAHJAAPI.Models;
using System;
using LAHJAAPI.V1.Validators.Conditions;

namespace V1.Validators
{
    public class LanguageValidatorContext : ValidatorContext<Language, LanguageValidatorStates>, ITValidator
    {
        public LanguageValidatorContext(IConditionChecker checker) : base(checker)
        {
        }

        protected override void InitializeConditions()
        {
        }
    } //
    //  Base
    public  enum  LanguageValidatorStates //
    { IsActive ,  IsFull ,  IsValid ,  //
    }

}