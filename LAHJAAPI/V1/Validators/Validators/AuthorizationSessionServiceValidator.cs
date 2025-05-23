using AutoGenerator.Conditions;
using LAHJAAPI.Models;
using LAHJAAPI.V1.Validators.Conditions;
using WasmAI.ConditionChecker.Base;

namespace V1.Validators
{
    public class AuthorizationSessionServiceValidatorContext : ValidatorContext<AuthorizationSessionService, AuthorizationSessionServiceValidatorStates>, ITValidator
    {
        public AuthorizationSessionServiceValidatorContext(IConditionChecker checker) : base(checker)
        {
        }

        protected override void InitializeConditions()
        {
        }


        [RegisterConditionValidator(typeof(AuthorizationSessionServiceValidatorStates), AuthorizationSessionServiceValidatorStates.IsValid, "AuthorizationSession Service is invalid")]
        private Task<ConditionResult> ValidateIsValid(DataFilter<bool, AuthorizationSessionService> f)
        {
            // Implement validation logic for IsValid property
            return Task.FromResult(f.Share?.AuthorizationSession != null ? ConditionResult.ToSuccess(f.Share) : ConditionResult.ToFailure("AuthorizationSession Service is invalid"));
        }

        [RegisterConditionValidator(typeof(AuthorizationSessionServiceValidatorStates), AuthorizationSessionServiceValidatorStates.IsFull, "Authorization Session Service is not valid")]
        private Task<ConditionResult> ValidateIsFull(DataFilter<bool, AuthorizationSessionService> f)
        {
            return Task.FromResult(f.Share?.AuthorizationSession?.IsActive == true ? ConditionResult.ToSuccess(f.Share) : ConditionResult.ToFailure("Authorization Session Service is not valid"));
        }

        [RegisterConditionValidator(typeof(AuthorizationSessionServiceValidatorStates), AuthorizationSessionServiceValidatorStates.AuthorizationSessionId, "Authorization Session Id is required")]
        private Task<ConditionResult> ValidateAuthorizationSessionId(DataFilter<string, AuthorizationSessionService> f)
        {
            return Task.FromResult(!string.IsNullOrWhiteSpace(f.Share?.AuthorizationSessionId) ? ConditionResult.ToSuccess(f.Share) : ConditionResult.ToFailure("Authorization Session Id is required"));
        }

        [RegisterConditionValidator(typeof(AuthorizationSessionServiceValidatorStates), AuthorizationSessionServiceValidatorStates.ServiceId, "Service Id is required")]
        private Task<ConditionResult> ValidateServiceId(DataFilter<string, AuthorizationSessionService> f)
        {
            return Task.FromResult(!string.IsNullOrWhiteSpace(f.Share?.ServiceId) ? ConditionResult.ToSuccess(f.Share) : ConditionResult.ToFailure("Service Id is required"));
        }

        protected override async Task<AuthorizationSessionService?> GetModel(string? id)
        {
            return await base.GetModel(id);
        }
    }

    public enum AuthorizationSessionServiceValidatorStates
    {
        IsFull = 1001,
        IsValid = 1002,
        AuthorizationSessionId = 1003,
        ServiceId = 1004

    }
}