using AutoGenerator.Conditions;
using LAHJAAPI.Utilities;
using LAHJAAPI.V1.Validators.Conditions;
using System.Security.Claims;
using System.Text.Json;
using V1.DyModels.Dso.Requests;
using V1.DyModels.VMs;

namespace LAHJAAPI.V1.Validators
{




    public enum TokenValidatorStates
    {
        IsServiceIdFound = 6100,
        IsServiceIdsEmpty = 6101,
        HasName,
        ValidatePlatformToken,
        ValidateCoreToken,
        CheckSessionToken,
    }

    public class TokenValidator : ValidatorContext<ServiceRequestDso, TokenValidatorStates>
    {
        private readonly IConditionChecker _checker;

        public TokenValidator(IConditionChecker checker) : base(checker)
        {
            _checker = checker;
        }

        protected override void InitializeConditions()
        {
            _provider.Register(TokenValidatorStates.HasName,
                new LambdaCondition<ServiceRequestDso>(
                    nameof(TokenValidatorStates.HasName),
                    s => !string.IsNullOrWhiteSpace(s.Name),
                    "Service name is required"
                )
            );


            _provider.Register(TokenValidatorStates.IsServiceIdFound,
                new LambdaCondition<string>(
                    nameof(TokenValidatorStates.IsServiceIdFound),
                    context => IsServiceIdFound(context),
                    "No service found in this token."
                )
            );

            _provider.Register(TokenValidatorStates.IsServiceIdsEmpty,
                new LambdaCondition<bool>(
                    nameof(TokenValidatorStates.IsServiceIdsEmpty),
                    context => IsServiceIdsEmpty(context),
                    "Service Ids is empty."
                )
            );


            _provider.Register(TokenValidatorStates.ValidatePlatformToken,
                new LambdaCondition<string>(
                    nameof(TokenValidatorStates.ValidatePlatformToken),
                    context => ValidatePlatformToken(context),
                    "Service Ids is empty."
                )
            );


            _provider.Register(
                TokenValidatorStates.ValidateCoreToken,
                new LambdaCondition<string>(
                    nameof(TokenValidatorStates.ValidateCoreToken),
                    context => ValidateCoreToken(context),
                    "Core token is not valid"));

            _provider.Register(
                TokenValidatorStates.CheckSessionToken,
                new LambdaCondition<string>(
                    nameof(TokenValidatorStates.CheckSessionToken),
                    context => CheckSessionToken(context),
                    "Platform token is not valid"
                )
            );

        }
        ConditionResult IsServiceIdFound(string idServ)
        {
            if (!string.IsNullOrWhiteSpace(idServ))
            {
                var result = _checker.Injector.UserClaims.ServicesIds?.Any(x => x == idServ);
                return new ConditionResult(result ?? false, null, "No service found in this token.");
            }
            return ConditionResult.ToError("No service found in this token.");
        }

        bool IsServiceIdsEmpty(bool idServ)
        {
            return _checker.Injector.UserClaims.ServicesIds?.Count == 0;
        }

        private ConditionResult ValidatePlatformToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) return ConditionResult.ToError("Token can not be null.");
            string secret = _checker.Injector.AppSettings.Jwt.WebSecret;
            var result = _checker.Injector.TokenService.ValidateToken(token, secret);
            if (result.IsFailed) return ConditionResult.ToError(result.Errors?.FirstOrDefault()?.Message);
            var claims = result.Value;

            var data = claims.FindFirstValue(ClaimTypes2.Data);
            if (string.IsNullOrWhiteSpace(data)) return ConditionResult.ToError("Data can not be null.");
            var dataTokenRequest = JsonSerializer.Deserialize<DataTokenRequest>(data);
            dataTokenRequest!.Token = secret;
            return ConditionResult.ToSuccess(dataTokenRequest);
        }


        private ConditionResult CheckSessionToken(string sessionToken)
        {
            if (string.IsNullOrWhiteSpace(sessionToken))
                return ConditionResult.ToError("Session token is null.");
            var result = _checker.Injector.TokenService.ValidateTemporaryToken(sessionToken);
            if (result.IsFailed) return ConditionResult.ToError(result.Errors.FirstOrDefault()?.Message);
            return ConditionResult.ToSuccess(sessionToken);
        }

        private ConditionResult ValidateCoreToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) ConditionResult.ToError("Token can not be null.");

            string secret = _checker.Injector.AppSettings.Jwt.WebSecret;
            var result = _checker.Injector.TokenService.ValidateToken(token, secret);
            if (result.IsFailed) return ConditionResult.ToError(result.Errors.FirstOrDefault()?.Message ?? "");

            var claims = result.Value;
            var sessionToken = claims.FindFirstValue(ClaimTypes2.SessionToken);
            if (string.IsNullOrWhiteSpace(sessionToken))
                return ConditionResult.ToError("Session token is not found in core token.");

            if (CheckSessionToken(sessionToken) is { Success: false } result2)
                return ConditionResult.ToError(result2.Message);
            return ConditionResult.ToSuccess(sessionToken);

        }

    }
}