using AutoGenerator.Conditions;
using LAHJAAPI.Data;
using LAHJAAPI.Models;
using LAHJAAPI.V1.Validators.Conditions;
using V1.DyModels.Dso.Requests;
using V1.DyModels.VMs;

namespace LAHJAAPI.V1.Validators
{
    public enum SessionValidatorStates
    {
        IsFound = 6400,
        IsActive = 6401,
        HasSessionToken,
        HasAuthorizationType,
        HasStartTime,
        HasUserId,
        HasEndTime,
        IsFull
    }

    public class AuthorizationSessionValidator : ValidatorContext<AuthorizationSession, SessionValidatorStates>
    {
        DataContext _context;
        private readonly IConditionChecker _checker;

        public AuthorizationSessionValidator(IConditionChecker checker) : base(checker)
        {
            _context = checker.Injector.Context;
            _checker = checker;
        }

        protected override void InitializeConditions()
        {



            _provider.Register(
                SessionValidatorStates.IsFound,
                new LambdaCondition<AuthorizationSessionFilterVM>(
                    nameof(SessionValidatorStates.IsFound),
                    context => IsFound(context.Id),
                    "Session is not found"
                )
            );

            //RegisterCondition<string>(
            //    SessionValidatorStates.IsActive,
            //    IsActive,
            //        "This session has been suspended."

            //);

            _provider.Register(
                SessionValidatorStates.HasSessionToken,
                new LambdaCondition<AuthorizationSessionRequestDso>(
                    nameof(SessionValidatorStates.HasSessionToken),
                    context => !string.IsNullOrWhiteSpace(context.SessionToken),
                    "Session Token is required"
                )
            );

            _provider.Register(
                SessionValidatorStates.HasAuthorizationType,
                new LambdaCondition<AuthorizationSessionRequestDso>(
                    nameof(SessionValidatorStates.HasAuthorizationType),
                    context => !string.IsNullOrWhiteSpace(context.AuthorizationType),
                    "Authorization Type is required"
                )
            );


            _provider.Register(
                SessionValidatorStates.HasStartTime,
                new LambdaCondition<AuthorizationSessionRequestDso>(
                    nameof(SessionValidatorStates.HasStartTime),
                    context => context.StartTime != default,
                    "Start Time is required"
                )
            );

            _provider.Register(
                SessionValidatorStates.IsActive,
                new LambdaCondition<AuthorizationSessionRequestDso>(
                    nameof(SessionValidatorStates.IsActive),
                    context => context.IsActive,
                    "Session is not active"
                )
            );

            _provider.Register(
                SessionValidatorStates.HasUserId,
                new LambdaCondition<AuthorizationSessionRequestDso>(
                    nameof(SessionValidatorStates.HasUserId),
                    context => !string.IsNullOrWhiteSpace(context.UserId),
                    "User ID is required"
                )
            );

            _provider.Register(
                SessionValidatorStates.HasEndTime,
                new LambdaCondition<AuthorizationSessionRequestDso>(
                    nameof(SessionValidatorStates.HasEndTime),
                    context => context.EndTime.HasValue,
                    "End Time is required"
                )
            );

            //_provider.Register(
            //    SessionValidatorStates.IsFull,
            //    new LambdaCondition<AuthorizationSessionRequestDso>(
            //        nameof(SessionValidatorStates.IsFull),
            //        context => IsValidAuthorizationSession(context),
            //        "Authorization session is incomplete"
            //    )
            //);
        }
        AuthorizationSession? Session { get; set; } = null;
        private AuthorizationSession? GetSession(string? id)
        {
            if (Session is not null) return Session;
            if (id == null) id = _checker.Injector.UserClaims.SessionId;
            if (string.IsNullOrWhiteSpace(id)) return null;
            return Session = _context.AuthorizationSessions.FirstOrDefault(s => s.Id == id);
        }

        private bool IsFound(string? id)
        {
            return GetSession(id) is not null;
        }

        [RegisterConditionValidator(typeof(SessionValidatorStates), SessionValidatorStates.IsActive, "Session not active.")]
        private async Task<ConditionResult> IsActive(DataFilter<string, AuthorizationSession> data)
        {
            if (data.Share == null) return ConditionResult.ToError("Session not found.");
            return new ConditionResult(data.Share!.IsActive, data.Share, "Session is not active.");
        }

        private bool CheckCustomerId(string userId)
        {
            return _checker.Check(ApplicationUserValidatorStates.HasCustomerId, userId);
        }



        private bool CheckAuthorizationType(string authorizationType)
        {
            return !string.IsNullOrWhiteSpace(authorizationType);
        }
        //private bool IsValidAuthorizationSession(AuthorizationSessionRequestDso context)
        //{
        //    var conditions = new List<Func<AuthorizationSessionRequestDso, bool>>
        //    {
        //        c =>CheckSessionToken(c.SessionToken),
        //        c =>CheckAuthorizationType(c.AuthorizationType),
        //        c => c.StartTime != default,
        //        c => CheckCustomerId(c.UserId),
        //        c => c.IsActive,
        //        c => c.EndTime.HasValue,
        //    };

        //    return conditions.All(condition => condition(context));
        //}
    }
}
