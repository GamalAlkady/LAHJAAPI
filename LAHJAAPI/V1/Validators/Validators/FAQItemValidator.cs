using AutoGenerator;
using AutoGenerator.Helper.Translation;
using AutoGenerator.Conditions;
using LAHJAAPI.Models;
using System;
using LAHJAAPI.V1.Validators.Conditions;

namespace V1.Validators
{
    public class FAQItemValidatorContext : ValidatorContext<FAQItem, FAQItemValidatorStates>, ITValidator
    {
        public FAQItemValidatorContext(IConditionChecker checker) : base(checker)
        {
        }

        protected override void InitializeConditions()
        {
        }
    } //
    //  Base
    public  enum  FAQItemValidatorStates //
    { IsActive ,  IsFull ,  IsValid ,  //
    }

}