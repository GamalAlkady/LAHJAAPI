using AutoGenerator.Conditions;
using LAHJAAPI.Models;
using LAHJAAPI.V1.Validators.Conditions;
using WasmAI.ConditionChecker.Base;

namespace V1.Validators
{
    public class PaymentValidatorContext : ValidatorContext<Payment, PaymentValidatorStates>, ITValidator
    {
        public PaymentValidatorContext(IConditionChecker checker) : base(checker)
        {
        }

        protected override void InitializeConditions()
        {
        }



        [RegisterConditionValidator(typeof(PaymentValidatorStates), PaymentValidatorStates.IsFull, "Payment data is not complete")]
        private Task<ConditionResult> ValidateIsFull(DataFilter<bool?, Payment> f)
        {
            bool? isValid = f.Share?.Id != null &&
                            !string.IsNullOrWhiteSpace(f.Share?.CustomerId) &&
                            !string.IsNullOrWhiteSpace(f.Share?.InvoiceId) &&
                            !string.IsNullOrWhiteSpace(f.Share?.Status) &&
                            !string.IsNullOrWhiteSpace(f.Share?.Amount) &&
                            !string.IsNullOrWhiteSpace(f.Share?.Currency) &&
                            f.Share?.Date != DateOnly.MinValue;

            return (isValid == true)
                ? ConditionResult.ToSuccessAsync(f.Share)
                : ConditionResult.ToFailureAsync("Payment data is not complete");

        }
        [RegisterConditionValidator(typeof(PaymentValidatorStates), PaymentValidatorStates.IsValid, "Payment data is invalid")]
        private Task<ConditionResult> ValidateIsValid(DataFilter<bool?, Payment> f)
        {
            return f.Share == null || string.IsNullOrEmpty(f.Share.Id)
                ? ConditionResult.ToFailureAsync("Payment data is invalid")
                : ConditionResult.ToSuccessAsync(f.Share);


        }
        // Add validation methods for other properties as needed
        [RegisterConditionValidator(typeof(PaymentValidatorStates), PaymentValidatorStates.IsFull, "CustomerId is required")]
        private Task<ConditionResult> ValidateCustomerId(DataFilter<string, Payment> f)
        {
            return string.IsNullOrWhiteSpace(f.Share?.CustomerId)
                ? ConditionResult.ToFailureAsync("CustomerId is required")
                : ConditionResult.ToSuccessAsync(f.Share?.CustomerId);
        }


        [RegisterConditionValidator(typeof(PaymentValidatorStates), PaymentValidatorStates.IsFull, "InvoiceId is required")]
        private Task<ConditionResult> ValidateInvoiceId(DataFilter<string, Payment> f)
        {
            return string.IsNullOrWhiteSpace(f.Share?.InvoiceId)
                ? ConditionResult.ToFailureAsync("InvoiceId is required")
                : ConditionResult.ToSuccessAsync(f.Share?.InvoiceId);
        }


        [RegisterConditionValidator(typeof(PaymentValidatorStates), PaymentValidatorStates.IsFull, "Payment status is required")]
        private Task<ConditionResult> ValidateStatus(DataFilter<string, Payment> f)
        {
            return string.IsNullOrWhiteSpace(f.Share?.Status)
                ? ConditionResult.ToFailureAsync("Payment status is required")
                : ConditionResult.ToSuccessAsync(f.Share?.Status);
        }


        [RegisterConditionValidator(typeof(PaymentValidatorStates), PaymentValidatorStates.IsFull, "Payment Amount is required")]
        private Task<ConditionResult> ValidateAmount(DataFilter<string, Payment> f)
        {
            return string.IsNullOrWhiteSpace(f.Share?.Amount)
                ? ConditionResult.ToFailureAsync("Payment Amount is required")
                : ConditionResult.ToSuccessAsync(f.Share?.Amount);
        }


        [RegisterConditionValidator(typeof(PaymentValidatorStates), PaymentValidatorStates.IsFull, "Payment Currency is required")]
        private Task<ConditionResult> ValidateCurrency(DataFilter<string, Payment> f)
        {
            return string.IsNullOrWhiteSpace(f.Share?.Currency)
                ? ConditionResult.ToFailureAsync("Payment Currency is required")
                : ConditionResult.ToSuccessAsync(f.Share?.Currency);
        }


        [RegisterConditionValidator(typeof(PaymentValidatorStates), PaymentValidatorStates.IsFull, "Payment Date is required")]
        private Task<ConditionResult> ValidateDate(DataFilter<DateOnly, Payment> f)
        {
            return f.Share?.Date == DateOnly.MinValue
                ? ConditionResult.ToFailureAsync("Payment Date is required")
                : ConditionResult.ToSuccessAsync(f.Share?.Date);
        }



        protected override async Task<Payment?> GetModel(string? id)
        {
            if (id == null) return null;  // Handle null ID

            return await base.GetModel(id);
        }

    }

    public enum PaymentValidatorStates
    {
        IsFull,
        IsValid,
        HasCustomerId,
        HasInvoiceId,
        HasStatus,
        HasAmount,
        HasCurrency,
        HasDate
    }
}