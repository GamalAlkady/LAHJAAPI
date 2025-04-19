using AutoGenerator.Conditions;
using FluentResults;
using LAHJAAPI.Data;
using LAHJAAPI.Models;
using LAHJAAPI.V1.Validators;
using LAHJAAPI.V1.Validators.Conditions;
using System.Security.Claims;
using System.Text.Json;
using V1.DyModels.Dso.Requests;
using V1.DyModels.Dso.ResponseFilters;
using V1.DyModels.VMs;

namespace ApiCore.Validators
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
        IsFull,
        ValidateCoreToken,
        ValidatePlatformToken,
        CheckSessionToken
    }

    public class AuthorizationSessionValidator : BaseValidator<AuthorizationSessionResponseFilterDso, SessionValidatorStates>, ITValidator
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
                SessionValidatorStates.ValidateCoreToken,
                new LambdaCondition<string>(
                    nameof(SessionValidatorStates.ValidateCoreToken),
                    context => ValidateCoreToken(context),
                    "Core token is not valid"
                )
            );

            _provider.Register(
                SessionValidatorStates.ValidatePlatformToken,
                new LambdaCondition<string>(
                    nameof(SessionValidatorStates.ValidatePlatformToken),
                    context => ValidatePlatformToken(context),
                    "Platform token is not valid"
                )
            );

            _provider.Register(
                SessionValidatorStates.CheckSessionToken,
                new LambdaCondition<string>(
                    nameof(SessionValidatorStates.CheckSessionToken),
                    context => CheckSessionToken(context),
                    "Platform token is not valid"
                )
            );


            _provider.Register(
                SessionValidatorStates.IsFound,
                new LambdaCondition<AuthorizationSessionFilterVM>(
                    nameof(SessionValidatorStates.IsFound),
                    context => IsFound(context.Id),
                    "Session is not found"
                )
            );

            _provider.Register(
                SessionValidatorStates.IsActive,
                new LambdaCondition<AuthorizationSessionFilterVM>(
                    nameof(SessionValidatorStates.IsActive),
                    context => IsActive(context.Id),
                    "This session has been suspended."
                )
            );

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


        private bool IsActive(string? id)
        {
            if (!IsFound(id)) return false;
            var session = GetSession(id);
            return session!.IsActive;
        }

        private bool CheckCustomerId(string userId)
        {
            return _checker.Check(ApplicationUserValidatorStates.HasCustomerId, userId);
        }

        private DataTokenRequest ValidatePlatformToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) throw new Exception("Token can not be null.");
            string secret = _checker.Injector.AppSettings.Jwt.WebSecret;
            var result = _checker.Injector.TokenService.ValidateToken(token, secret);
            if (result.IsFailed) throw new Exception(result.Errors?.FirstOrDefault()?.Message);
            var claims = result.Value;
            var data = claims.FindFirstValue("Data");
            if (string.IsNullOrWhiteSpace(data)) throw new Exception("Data can not be null.");
            var dataTokenRequest = JsonSerializer.Deserialize<DataTokenRequest>(data);
            dataTokenRequest!.Token = secret;
            return dataTokenRequest;
        }

        private bool CheckSessionToken(string sessionToken)
        {
            if (string.IsNullOrWhiteSpace(sessionToken)) return false;
            var result = _checker.Injector.TokenService.ValidateToken(sessionToken);
            return result.IsSuccess;
        }

        private Result<string> ValidateCoreToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) Result.Fail("Token can not be null.");
            string secret = _checker.Injector.AppSettings.Jwt.WebSecret;
            var result = _checker.Injector.TokenService.ValidateToken(token, secret);
            if (result.IsFailed) return result.ToResult<string>();
            var claims = result.Value;
            var sessionToken = claims.FindFirstValue("SessionToken");
            if (string.IsNullOrWhiteSpace(sessionToken))
            {
                return Result.Fail("Session token is not found in core token.");
            }
            if (CheckSessionToken(sessionToken)) return sessionToken;
            if (result.IsFailed) return result.ToResult<string>();

            return Result.Ok(sessionToken);
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
