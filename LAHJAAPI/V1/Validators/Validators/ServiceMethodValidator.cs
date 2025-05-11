using AutoGenerator;
using AutoGenerator.Helper.Translation;
using AutoGenerator.Conditions;
using LAHJAAPI.Models;
using System;
using LAHJAAPI.V1.Validators.Conditions;

namespace V1.Validators
{
    public class ServiceMethodValidatorContext : ValidatorContext<ServiceMethod, ServiceMethodValidatorStates>, ITValidator
    {
        public ServiceMethodValidatorContext(IConditionChecker checker) : base(checker)
        {
        }

        protected override void InitializeConditions()
        {
        }
    } //
    //  Base
    public  enum  ServiceMethodValidatorStates //
    { IsActive ,  IsFull ,  IsValid ,  //
    }

}