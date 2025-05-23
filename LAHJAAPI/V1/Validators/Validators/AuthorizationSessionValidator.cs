using LAHJAAPI.Models;
using LAHJAAPI.V1.Validators.Conditions;
using Microsoft.EntityFrameworkCore;
using V1.Validators;
using WasmAI.ConditionChecker.Base;

namespace LAHJAAPI.V1.Validators
{
    public enum AuthorizationSessionValidatorStates
    {
        HasId = 6400,
        IsActive = 6401,
        HasSessionToken,
        HasAuthorizationType,
        HasStartTime,
        HasUserId,
        HasMatchingSession,
        IsServicesAsignedToUser,
        HasIpAddress,
        HasDeviceInfo
    }

    public class AuthorizationSessionValidator : ValidatorContext<AuthorizationSession, AuthorizationSessionValidatorStates>
    {
        private AuthorizationSession? _authorizationSession;

        public AuthorizationSessionValidator(IConditionChecker checker) : base(checker)
        {
        }

        protected override void InitializeConditions()
        {
        }

        [RegisterConditionValidator(typeof(AuthorizationSessionValidatorStates), AuthorizationSessionValidatorStates.HasId, "Session not found.")]
        private Task<ConditionResult> ValidateId(DataFilter<string, AuthorizationSession> f)
        {
            return !string.IsNullOrWhiteSpace(f.Share?.Id) ? ConditionResult.ToSuccessAsync(f.Share) : ConditionResult.ToFailureAsync("Id is required");
        }

        [RegisterConditionValidator(typeof(AuthorizationSessionValidatorStates), AuthorizationSessionValidatorStates.IsActive, "IsActive is required")]
        private Task<ConditionResult> ValidateIsActive(DataFilter<bool, AuthorizationSession> f)
        {
            return f.Share?.IsActive == true ? ConditionResult.ToSuccessAsync(f.Share) : ConditionResult.ToFailureAsync("IsActive is required");
        }


        [RegisterConditionValidator(typeof(AuthorizationSessionValidatorStates), AuthorizationSessionValidatorStates.HasSessionToken, "SessionToken is required")]
        private Task<ConditionResult> ValidateSessionToken(DataFilter<string, AuthorizationSession> f)
        {
            return !string.IsNullOrWhiteSpace(f.Share?.SessionToken) ? ConditionResult.ToSuccessAsync(f.Share) : ConditionResult.ToFailureAsync("SessionToken is required");
        }

        [RegisterConditionValidator(typeof(AuthorizationSessionValidatorStates), AuthorizationSessionValidatorStates.HasAuthorizationType, "AuthorizationType is required")]
        private Task<ConditionResult> ValidateAuthorizationType(DataFilter<string, AuthorizationSession> f)
        {
            return !string.IsNullOrWhiteSpace(f.Share?.AuthorizationType) ? ConditionResult.ToSuccessAsync(f.Share) : ConditionResult.ToFailureAsync("AuthorizationType is required");
        }

        [RegisterConditionValidator(typeof(AuthorizationSessionValidatorStates), AuthorizationSessionValidatorStates.HasStartTime, "StartTime is required")]
        private Task<ConditionResult> ValidateStartTime(DataFilter<DateTime, AuthorizationSession> f)
        {
            return f.Share?.StartTime != default(DateTime) ? ConditionResult.ToSuccessAsync(f.Share) : ConditionResult.ToFailureAsync("StartTime is required");
        }

        [RegisterConditionValidator(typeof(AuthorizationSessionValidatorStates), AuthorizationSessionValidatorStates.HasUserId, "UserId is required")]
        private Task<ConditionResult> ValidateUserId(DataFilter<string, AuthorizationSession> f)
        {
            return !string.IsNullOrEmpty(f.Share?.UserId) ? ConditionResult.ToSuccessAsync(f.Share) : ConditionResult.ToFailureAsync("UserId is required");
        }

        [RegisterConditionValidator(typeof(AuthorizationSessionValidatorStates), AuthorizationSessionValidatorStates.HasIpAddress, "IpAddress is required")]
        private Task<ConditionResult> ValidateIpAddress(DataFilter<string, AuthorizationSession> f)
        {
            return !string.IsNullOrEmpty(f.Share?.IpAddress) ? ConditionResult.ToSuccessAsync(f.Share) : ConditionResult.ToFailureAsync("IpAddress is required");
        }


        [RegisterConditionValidator(typeof(AuthorizationSessionValidatorStates), AuthorizationSessionValidatorStates.HasDeviceInfo, "DeviceInfo is required")]
        private Task<ConditionResult> ValidateDeviceInfo(DataFilter<string, AuthorizationSession> f)
        {
            return !string.IsNullOrEmpty(f.Share?.DeviceInfo) ? ConditionResult.ToSuccessAsync(f.Share) : ConditionResult.ToFailureAsync("DeviceInfo is required");
        }

        protected override async Task<AuthorizationSession?> GetModel(string? id)
        {
            if (_authorizationSession != null && _authorizationSession.Id == id)
                return _authorizationSession;
            _authorizationSession = await base.GetModel(id);
            return _authorizationSession;
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
