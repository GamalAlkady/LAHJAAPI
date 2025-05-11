using AutoGenerator;
using AutoGenerator.Helper.Translation;
using AutoGenerator.Conditions;
using LAHJAAPI.Models;
using System;
using LAHJAAPI.V1.Validators.Conditions;

namespace V1.Validators
{
    public class ServiceValidatorContext : ValidatorContext<Service, ServiceValidatorStates>, ITValidator
    {
        public ServiceValidatorContext(IConditionChecker checker) : base(checker)
        {
        }

        protected override void InitializeConditions()
        {
        }
    } //
    //  Base
    public  enum  ServiceValidatorStates //
    { IsActive ,  IsFull ,  IsValid ,  //
    }

}