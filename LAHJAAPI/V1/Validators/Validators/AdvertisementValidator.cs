using AutoGenerator.Conditions;
using LAHJAAPI.Models;
using LAHJAAPI.V1.Validators.Conditions;

namespace V1.Validators
{
    public class AdvertisementValidatorContext : ValidatorContext<Advertisement, AdvertisementValidatorStates>, ITValidator
    {
        public AdvertisementValidatorContext(IConditionChecker checker) : base(checker)
        {
        }

        protected override void InitializeConditions()
        {
        }
    } //
    //  Base
    public enum AdvertisementValidatorStates //
    {
        IsActive, IsFull, IsValid,  //
    }

}