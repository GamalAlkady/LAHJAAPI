using AutoGenerator.Conditions;
using LAHJAAPI.Models;
using LAHJAAPI.V1.Validators.Conditions;
using WasmAI.ConditionChecker.Base;

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

        [RegisterConditionValidator(typeof(ServiceMethodValidatorStates), ServiceMethodValidatorStates.IsValid, "Method is required")]
        private Task<ConditionResult> ValidateMethod(DataFilter<string, ServiceMethod> f)
        {
            bool valid = !string.IsNullOrWhiteSpace(f.Share?.Method);
            return valid ? ConditionResult.ToSuccessAsync(f.Share?.Method) : ConditionResult.ToFailureAsync(f.Share?.Method, "Method is required");
        }

        [RegisterConditionValidator(typeof(ServiceMethodValidatorStates), ServiceMethodValidatorStates.IsActive, "Input parameters cannot be null or whitespace")]
        private Task<ConditionResult> ValidateInputParameters(DataFilter<string, ServiceMethod> f)
        {
            bool valid = f.Share?.InputParameters == null || !string.IsNullOrWhiteSpace(f.Share?.InputParameters);
            return valid ? ConditionResult.ToSuccessAsync(f.Share?.InputParameters) : ConditionResult.ToFailureAsync(f.Share?.InputParameters, "Input parameters cannot be null or whitespace");
        }

        [RegisterConditionValidator(typeof(ServiceMethodValidatorStates), ServiceMethodValidatorStates.IsFull, "Output parameters cannot be null or whitespace")]
        private Task<ConditionResult> ValidateOutputParameters(DataFilter<string, ServiceMethod> f)
        {
            bool valid = f.Share?.OutputParameters == null || !string.IsNullOrWhiteSpace(f.Share?.OutputParameters);
            return valid ? ConditionResult.ToSuccessAsync(f.Share?.OutputParameters) : ConditionResult.ToFailureAsync(f.Share?.OutputParameters, "Output parameters cannot be null or whitespace");
        }



    }

    public enum ServiceMethodValidatorStates
    {
        IsValid,
        IsActive,
        IsFull
    }
}