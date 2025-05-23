using AutoGenerator.Conditions;
using LAHJAAPI.Models;
using LAHJAAPI.V1.Validators.Conditions;
using WasmAI.ConditionChecker.Base;

namespace V1.Validators
{
    public class EventRequestValidatorContext : ValidatorContext<EventRequest, EventRequestValidatorStates>, ITValidator
    {
        public EventRequestValidatorContext(IConditionChecker checker) : base(checker)
        {
        }

        protected override void InitializeConditions()
        {
        }

        [RegisterConditionValidator(typeof(EventRequestValidatorStates), EventRequestValidatorStates.IsValid, "Status is required.")]
        private Task<ConditionResult> ValidateStatus(DataFilter<string, EventRequest> f)
        {
            bool isValid = !string.IsNullOrWhiteSpace(f.Share?.Status);
            return isValid ? ConditionResult.ToSuccessAsync(f.Share?.Status) : ConditionResult.ToFailureAsync(f.Share?.Status, "Status is required.");
        }

        [RegisterConditionValidator(typeof(EventRequestValidatorStates), EventRequestValidatorStates.IsValid, "Details cannot be longer than 10000 characters.")]
        private Task<ConditionResult> ValidateDetails(DataFilter<string, EventRequest> f)
        {
            if (f.Share?.Details != null && f.Share.Details.Length > 10000)
            {
                return ConditionResult.ToFailureAsync(f.Share?.Details, "Details cannot be longer than 10000 characters.");
            }
            return ConditionResult.ToSuccessAsync(f.Share?.Details);
        }

        [RegisterConditionValidator(typeof(EventRequestValidatorStates), EventRequestValidatorStates.IsValid, "RequestId is required.")]
        private Task<ConditionResult> ValidateRequestId(DataFilter<string, EventRequest> f)
        {
            bool isValid = !string.IsNullOrWhiteSpace(f.Share?.RequestId);
            return isValid ? ConditionResult.ToSuccessAsync(f.Share?.RequestId) : ConditionResult.ToFailureAsync(f.Share?.RequestId, "RequestId is required.");
        }


    }

    public enum EventRequestValidatorStates
    {
        IsValid,
        HasStatus,
        HasDetails,
        HasRequestId
    }
}