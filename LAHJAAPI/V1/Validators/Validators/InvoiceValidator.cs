using AutoGenerator.Conditions;
using LAHJAAPI.Models;
using LAHJAAPI.V1.Validators.Conditions;
using WasmAI.ConditionChecker.Base;

namespace V1.Validators
{
    public class InvoiceValidatorContext : ValidatorContext<Invoice, InvoiceValidatorStates>, ITValidator
    {
        public InvoiceValidatorContext(IConditionChecker checker) : base(checker)
        {
        }

        protected override void InitializeConditions()
        {
        }

        [RegisterConditionValidator(typeof(InvoiceValidatorStates), InvoiceValidatorStates.IsValid, "Invoice ID is required.")]
        private Task<ConditionResult> ValidateId(DataFilter<string, Invoice> f)
        {
            bool valid = !string.IsNullOrWhiteSpace(f.Share?.Id);
            return valid ? ConditionResult.ToSuccessAsync(f.Share) : ConditionResult.ToFailureAsync("Invoice ID is required.");
        }

        [RegisterConditionValidator(typeof(InvoiceValidatorStates), InvoiceValidatorStates.IsActive, "Customer ID is required.")]
        private Task<ConditionResult> ValidateCustomerId(DataFilter<string, Invoice> f)
        {
            bool valid = !string.IsNullOrWhiteSpace(f.Share?.CustomerId);
            return valid ? ConditionResult.ToSuccessAsync(f.Share) : ConditionResult.ToFailureAsync("Customer ID is required.");
        }

        [RegisterConditionValidator(typeof(InvoiceValidatorStates), InvoiceValidatorStates.IsFull, "Invoice status is required.")]
        private Task<ConditionResult> ValidateStatus(DataFilter<string, Invoice> f)
        {
            bool valid = !string.IsNullOrWhiteSpace(f.Share?.Status);
            return valid ? ConditionResult.ToSuccessAsync(f.Share) : ConditionResult.ToFailureAsync("Invoice status is required.");
        }

        [RegisterConditionValidator(typeof(InvoiceValidatorStates), InvoiceValidatorStates.IsValid, "Invoice URL is required.")]
        private Task<ConditionResult> ValidateUrl(DataFilter<string, Invoice> f)
        {
            bool valid = !string.IsNullOrWhiteSpace(f.Share?.Url);
            return valid ? ConditionResult.ToSuccessAsync(f.Share) : ConditionResult.ToFailureAsync("Invoice URL is required.");
        }

        [RegisterConditionValidator(typeof(InvoiceValidatorStates), InvoiceValidatorStates.IsValid, "Invoice date is required.")]
        private Task<ConditionResult> ValidateInvoiceDate(DataFilter<DateTime?, Invoice> f)
        {
            bool valid = f.Share?.InvoiceDate != null;
            return valid ? ConditionResult.ToSuccessAsync(f.Share) : ConditionResult.ToFailureAsync("Invoice date is required.");
        }


        protected override async Task<Invoice?> GetModel(string? id)
        {
            if (id == null) return null;  //Handle null ID gracefully
            return await base.GetModel(id);
        }
    }

    public enum InvoiceValidatorStates
    {
        IsValid,
        IsActive,
        IsFull
    }
}