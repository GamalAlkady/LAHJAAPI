using AutoGenerator.Conditions;
using LAHJAAPI.V1.Validators.Conditions;
using V1.DyModels.Dso.Requests;
using V1.DyModels.Dso.Responses;

namespace LAHJAAPI.V1.Validators
{




    public enum ServiceValidatorStates
    {
        IsFound = 6200,
        IsFull,
        HasName,
        HasAbsolutePath,
        IsCreateSpace,
        IsDashboard,
        HasToken,
        HasValidModelAi,
        HasMethods,
        HasRequests,
        IsLinkedToUsers
    }

    public class ServiceValidator : BaseValidator<ServiceRequestDso, ServiceValidatorStates>, ITValidator
    {
        private readonly IConditionChecker _checker;

        public ServiceValidator(IConditionChecker checker) : base(checker)
        {
            _checker = checker;
        }

        protected override void InitializeConditions()
        {
            _provider.Register(ServiceValidatorStates.HasName,
                new LambdaCondition<ServiceRequestDso>(
                    nameof(ServiceValidatorStates.HasName),
                    s => !string.IsNullOrWhiteSpace(s.Name),
                    "Service name is required"
                )
            );

            _provider.Register(ServiceValidatorStates.HasAbsolutePath,
                new LambdaCondition<ServiceResponseDso>(
                    nameof(ServiceValidatorStates.HasAbsolutePath),
                    s => Uri.IsWellFormedUriString(s.AbsolutePath, UriKind.Absolute),
                    "AbsolutePath must be a valid URL"
                )
            );

            _provider.Register(ServiceValidatorStates.IsCreateSpace,
                new LambdaCondition<string>(
                    nameof(ServiceValidatorStates.IsCreateSpace),
                    context => context.Equals("createspace", StringComparison.CurrentCultureIgnoreCase),
                    "This service not create space"
                )
            );

            _provider.Register(ServiceValidatorStates.IsDashboard,
                new LambdaCondition<string>(
                    nameof(ServiceValidatorStates.IsDashboard),
                    context => context.Equals("dashboard", StringComparison.CurrentCultureIgnoreCase),
                    "This service not dashboard"
                )
            );

            _provider.Register(ServiceValidatorStates.HasToken,
                new LambdaCondition<ServiceRequestDso>(
                    nameof(ServiceValidatorStates.HasToken),
                    s => !string.IsNullOrWhiteSpace(s.Token) && s.Token.Length >= 10,
                    "Token is required and must be at least 10 characters"
                )
            );

            _provider.Register(ServiceValidatorStates.HasValidModelAi,
                new LambdaCondition<ServiceRequestDso>(
                    nameof(ServiceValidatorStates.HasValidModelAi),
                    s => string.IsNullOrWhiteSpace(s.ModelAiId) || s.ModelAi != null,
                    "Model AI reference is invalid"
                )
            );

            _provider.Register(ServiceValidatorStates.HasMethods,
                new LambdaCondition<ServiceRequestDso>(
                    nameof(ServiceValidatorStates.HasMethods),
                    s => s.ServiceMethods != null && s.ServiceMethods.Any(),
                    "Service must have at least one method"
                )
            );

            _provider.Register(ServiceValidatorStates.HasRequests,
                new LambdaCondition<ServiceRequestDso>(
                    nameof(ServiceValidatorStates.HasRequests),
                    s => s.Requests != null && s.Requests.Any(),
                    "Service must have at least one request"
                )
            );

            _provider.Register(ServiceValidatorStates.IsLinkedToUsers,
                new LambdaCondition<ServiceRequestDso>(
                    nameof(ServiceValidatorStates.IsLinkedToUsers),
                    s => s.UserServices != null && s.UserServices.Any(),
                    "Service must be linked to at least one user"
                )
            );
            _provider.Register(ServiceValidatorStates.IsFound,
                new LambdaCondition<string>(
                    nameof(ServiceValidatorStates.IsFound),
                    context => IsFound(context),
                    "You coudn't create space with this session. You need to create session with service create space."
                )
            );

        }
        bool IsFound(string idServ)
        {
            if (!string.IsNullOrWhiteSpace(idServ))
            {
                var result = _checker.Injector.Context.Services?.Any(x => x.Id == idServ);
                return result ?? false;
            }
            return false;
        }

    }
}