using AutoGenerator.Conditions;
using LAHJAAPI.Models;
using LAHJAAPI.V1.Validators.Conditions;

namespace V1.Validators
{
    public class AdvertisementTabValidatorContext : ValidatorContext<AdvertisementTab, AdvertisementTabValidatorStates>, ITValidator
    {
        public AdvertisementTabValidatorContext(IConditionChecker checker) : base(checker)
        {
        }

        protected override void InitializeConditions()
        {
        }
    } //
    //  Base
    public enum AdvertisementTabValidatorStates //
    {
        IsActive, IsFull, IsValid,  //
    }

}