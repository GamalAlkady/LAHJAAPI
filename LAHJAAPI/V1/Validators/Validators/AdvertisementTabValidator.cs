using AutoGenerator.Conditions;
using LAHJAAPI.Models;
using LAHJAAPI.V1.Validators.Conditions;
using WasmAI.ConditionChecker.Base;

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


        [RegisterConditionValidator(typeof(AdvertisementTabValidatorStates), AdvertisementTabValidatorStates.IsValid, "Advertisement Tab is invalid")]
        private Task<ConditionResult> ValidateIsValid(DataFilter<bool, AdvertisementTab> f)
        {
            if (f.Share == null) return ConditionResult.ToFailureAsync(f.Share, "Advertisement Tab is null");

            var advertisementIdValid = !string.IsNullOrWhiteSpace(f.Share.AdvertisementId);
            var titleValid = !string.IsNullOrWhiteSpace(f.Share.Title);
            var descriptionValid = !string.IsNullOrWhiteSpace(f.Share.Description);

            return advertisementIdValid && titleValid && descriptionValid ? ConditionResult.ToSuccessAsync(f.Share) : ConditionResult.ToFailureAsync(f.Share, "Advertisement Tab is invalid");
        }


        [RegisterConditionValidator(typeof(AdvertisementTabValidatorStates), AdvertisementTabValidatorStates.IsFull, "Advertisement Tab is missing required fields")]
        private Task<ConditionResult> ValidateIsFull(DataFilter<AdvertisementTab, AdvertisementTab> f)
        {
            if (f.Share == null) return ConditionResult.ToFailureAsync(f.Share, "Advertisement Tab is null");


            bool isValid = !string.IsNullOrEmpty(f.Share.AdvertisementId) &&
                           !string.IsNullOrEmpty(f.Share.Title) &&
                           !string.IsNullOrEmpty(f.Share.Description);

            return isValid ? ConditionResult.ToSuccessAsync(f.Share) : ConditionResult.ToFailureAsync(f.Share, "Advertisement Tab is missing required fields");
        }



        protected override async Task<AdvertisementTab?> GetModel(string? id)
        {
            return await base.GetModel(id);
        }
    }

    public enum AdvertisementTabValidatorStates
    {
        IsFull,
        IsValid,
    }
}