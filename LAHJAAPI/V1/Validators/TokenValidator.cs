using AutoGenerator.Conditions;
using LAHJAAPI.V1.Validators.Conditions;
using V1.DyModels.Dso.Requests;

namespace LAHJAAPI.V1.Validators
{




    public enum TokenValidatorStates
    {
        IsServiceIdFound = 6100,
        IsServiceIdsEmpty = 6101,
        HasName,
    }

    public class TokenValidator : BaseValidator<ServiceRequestDso, TokenValidatorStates>, ITValidator
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

        }
        bool IsServiceIdFound(string idServ)
        {
            if (!string.IsNullOrWhiteSpace(idServ))
            {
                var result = _checker.Injector.UserClaims.ServicesIds?.Any(x => x == idServ);
                return result ?? false;
            }
            return false;
        }

        bool IsServiceIdsEmpty(bool idServ)
        {
            return _checker.Injector.UserClaims.ServicesIds?.Count == 0;
        }

    }
}