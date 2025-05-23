using AutoGenerator;
using AutoGenerator.Helper.Translation;
using AutoGenerator.Conditions;
using LAHJAAPI.Models;
using System;
using LAHJAAPI.V1.Validators.Conditions;

namespace V1.Validators
{
    public class DialectValidatorContext : ValidatorContext<Dialect, DialectValidatorStates>, ITValidator
    {
        public DialectValidatorContext(IConditionChecker checker) : base(checker)
        {
        }

        protected override void InitializeConditions()
        {
        }
    } //
    //  Base
    public  enum  DialectValidatorStates //
    { IsActive ,  IsFull ,  IsValid ,  //
    }

}