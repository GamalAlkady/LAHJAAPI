using LAHJAAPI.Models;
using LAHJAAPI.V1.Validators.Conditions;
using Microsoft.EntityFrameworkCore;
using V1.Validators;
using WasmAI.ConditionChecker.Base;

namespace LAHJAAPI.V1.Validators
{
    public enum AuthorizationSessionValidatorStates
    {
        ValidateId = 6400,
        IsActive = 6401,
        HasSessionToken,
        HasAuthorizationType,
        HasStartTime,
        HasUserId,
        HasEndTime,
        IsFull,
        HasMatchingSession,
        IsValidServices,
        IsServicesAsignedToUser
    }

    public class AuthorizationSessionValidator : ValidatorContext<AuthorizationSession, AuthorizationSessionValidatorStates>
    {
        private readonly IConditionChecker _checker;
        AuthorizationSession? Session { get; set; } = null;

        public AuthorizationSessionValidator(IConditionChecker checker) : base(checker)
        {
            _checker = checker;
        }

        protected override void InitializeConditions()
        {
        }

        protected async Task<List<T>> QueryDbSet<T>(Func<DbSet<T>, IQueryable<T>> query)
            where T : class
        {
            return await _injector.ContextFactory.ExecuteInScopeAsync(ctx =>
                query(ctx.Set<T>()).ToListAsync()
            );
        }

        [RegisterConditionValidator(typeof(AuthorizationSessionValidatorStates), AuthorizationSessionValidatorStates.ValidateId, "Session not found.")]
        private Task<ConditionResult> ValidateId(DataFilter<string, AuthorizationSession> f)
        {
            bool valid = !string.IsNullOrWhiteSpace(f.Share?.Id);
            return valid
                ? ConditionResult.ToSuccessAsync(f.Share)
                : ConditionResult.ToFailureAsync("Id is required");
        }

        [RegisterConditionValidator(typeof(AuthorizationSessionValidatorStates), AuthorizationSessionValidatorStates.IsActive, "Session not active.")]
        private Task<ConditionResult> IsActive(DataFilter<string, AuthorizationSession> data)
        {
            if (data.Share == null)
                return ConditionResult.ToErrorAsync("Session not found.");
            return Task.FromResult(new ConditionResult(data.Share!.IsActive, data.Share, "Session is not active."));
        }

        [RegisterConditionValidator(typeof(AuthorizationSessionValidatorStates), AuthorizationSessionValidatorStates.HasMatchingSession, "No session found for the provided user ID and authorization type.")]
        private async Task<ConditionResult> HasMatchingAuthorizationSession(DataFilter<string, AuthorizationSession> data)
        {
            try
            {
                if (data.Items == null || !data.Items.TryGetValue("userId", out object? userId))
                    return ConditionResult.ToError("UserId is required.");

                if (!data.Items.TryGetValue("servicesIds", out object? servicesIdsObj))
                    return ConditionResult.ToError("ServicesIds is required.");

                if (!data.Items.TryGetValue("authorizationType", out object? authorizationTypeObj))
                    return ConditionResult.ToError("AuthorizationType is required.");

                var servicesIds = servicesIdsObj as List<string>;
                var authorizationType = authorizationTypeObj?.ToString();

                if (servicesIds == null || servicesIds.Count == 0)
                    return ConditionResult.ToError("ServicesIds is required.");
                if (string.IsNullOrWhiteSpace(authorizationType))
                    return ConditionResult.ToError("AuthorizationType is required.");


                // الخطوة 1: جلب الجلسات المرتبطة بالمستخدم ونوع الصلاحية
                var candidateSessions = await QueryListAsync<AuthorizationSession>(s =>
                     s.Include(ss => ss.AuthorizationSessionServices)
                     .Where(ss =>
                         ss.UserId == userId.ToString()
                         && ss.AuthorizationType == authorizationType
                     && ss.AuthorizationSessionServices.Count() == servicesIds.Count
                     )
                );

                // الخطوة 2: فلترة الجلسات التي تحتوي على جميع الـ serviceIds المطلوبة
                var session = candidateSessions
                    .Where(sess =>
                        sess.AuthorizationSessionServices != null &&
                        servicesIds.All(requiredId => sess.AuthorizationSessionServices.Any(s => s.ServiceId == requiredId))
                    )
                    .FirstOrDefault();


                //var session = matchingSessions.FirstOrDefault();

                if (session == null)
                {
                    return ConditionResult.ToError("No session found for the provided user ID and authorization type.");
                }

                //var modelAi = session.AuthorizationSessionServices.First().Service.ModelAi;
                ////var services = await QueryListAsync<Service>(s => s.Include(s => s.ModelAi).ThenInclude(m => m.ModelGateway).Where(sv => servicesIds.Contains(sv.Id)));
                //var resultModelGateway = await _checker.CheckAndResultAsync(ModelGatewayValidatorStates.ValidateId, modelAi.ModelGatewayId);
                //if (!resultModelGateway.Success.GetValueOrDefault()) return resultModelGateway;

                //ModelGateway modelGateway = (ModelGateway)resultModelGateway.Result!;
                //modelAi.ModelGateway = modelGateway;
                return ConditionResult.ToSuccess(session, "Matching session found.");
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        [RegisterConditionValidator(typeof(AuthorizationSessionValidatorStates), AuthorizationSessionValidatorStates.IsServicesAsignedToUser)]
        private async Task<ConditionResult> IsServicesAsignedToUser(DataFilter<List<string>> data)
        {
            try
            {
                if (data.Value == null || data.Value.Count == 0)
                    return ConditionResult.ToError("You must pass services as list to value.");
                if (data.Items == null || !data.Items.TryGetValue("userId", out object? userId))
                    return ConditionResult.ToError("UserId is required.");

                var servicesIds = data.Value;

                //HashSet<UserService> services = [];

                foreach (var serviceId in data.Value)
                {
                    var resultService = await _checker.CheckAndResultAsync(UserServiceValidatorStates.IsServiceAssigned,
                        new DataFilter(userId.ToString()) { Value = serviceId });

                    if (!resultService.Success.GetValueOrDefault())
                        return resultService;

                    //services.Add((Service)resultService.Result!);
                }

                //var services = await QueryListAsync<Service>(s => s.Include(s => s.ModelAi).Where(sv => servicesIds.Contains(sv.Id)));

                //var modelGatewayId = services.First().ModelAi.ModelGatewayId;

                //var resultModelGateway = await _checker.CheckAndResultAsync(ModelGatewayValidatorStates.ValidateId, modelGatewayId);
                //if (!resultModelGateway.Success.GetValueOrDefault()) return resultModelGateway;

                //ModelGateway modelGateway = (ModelGateway)resultModelGateway.Result!;
                //services.First().ModelAi.ModelGateway = modelGateway;

                return ConditionResult.ToSuccess(servicesIds, "All services are assigned to the user.");
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
