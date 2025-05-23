using APILAHJA.Utilities;
using AutoGenerator.Helper;
using AutoMapper;
using FluentResults;
using LAHJAAPI.Exceptions;
using LAHJAAPI.Services2;
using LAHJAAPI.Utilities;
using LAHJAAPI.V1.Validators;
using LAHJAAPI.V1.Validators.Conditions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Quartz.Util;
using System.Security.Claims;
using V1.BPR.Layers.Base;
using V1.DyModels.Dso.Requests;
using V1.DyModels.Dso.Responses;
using V1.DyModels.Dto.Build.Requests;
using V1.DyModels.Dto.Share.Requests;
using V1.DyModels.Dto.Share.Responses;
using V1.DyModels.VMs;
using V1.Repositories.Share;
using WasmAI.ConditionChecker.Base;

namespace V1.Services.Services
{
    public class AuthorizationSessionService : BaseBPRServiceLayer<AuthorizationSessionRequestDso, AuthorizationSessionResponseDso, AuthorizationSessionRequestShareDto, AuthorizationSessionResponseShareDto>,
        IUseAuthorizationSessionService
    {
        private readonly IAuthorizationSessionShareRepository _share;
        private readonly IUserClaimsHelper _userClaims;
        private readonly IUseServiceService _serviceService;
        private readonly IUseUserServiceService _userServiceService;
        private readonly IConditionChecker _checker;
        private readonly LinkGenerator _linkGenerator;
        private readonly TokenService _tokenService;
        private readonly AppSettings _appSettings;
        public AuthorizationSessionService(
            IMapper mapper,
            ILoggerFactory logger,
            IAuthorizationSessionShareRepository buildAuthorizationSessionShareRepository,
            IUserClaimsHelper userClaims,
            IUseServiceService serviceShare,
            IUseUserServiceService userServiceService,
            IConditionChecker checker,
            IOptions<AppSettings> options,
            LinkGenerator linkGenerator,
            TokenService tokenService) : base(mapper, logger, buildAuthorizationSessionShareRepository)
        {
            _share = buildAuthorizationSessionShareRepository;
            _userClaims = userClaims;
            _serviceService = serviceShare;
            _userServiceService = userServiceService;
            _checker = checker;
            _linkGenerator = linkGenerator;
            _tokenService = tokenService;
            _appSettings = options.Value;
        }

        public async Task<Result<AuthorizationSessionResponseDso>> GetSessionByServices(string userId, List<string> servicesIds, string authorizationType)
        {
            var response = await _share.GetAllByAsync([
                new FilterCondition(nameof(AuthorizationSessionResponseDso.UserId) , userId),
                new FilterCondition(nameof(AuthorizationSessionResponseDso.AuthorizationType) ,authorizationType),
                new FilterCondition(nameof(AuthorizationSessionResponseDso.ServicesIds) ,servicesIds[0],FilterOperator.Contains),
                ]);
            if (response.TotalRecords == 0)
            {
                return Result.Fail(new Error("No session found for the provided user ID and authorization type."));
            }
            var session = response.Data
             .Select(s => new AuthorizationSessionResponseDso
             {
                 Id = s.Id,
                 //Services = JsonConvert.DeserializeObject<List<string>>(s.ServicesIds),
                 EndTime = s.EndTime,
                 IsActive = s.IsActive,
                 SessionToken = s.SessionToken,
                 UserId = s.UserId,
                 AuthorizationType = s.AuthorizationType
             })
             .AsEnumerable()
             .LastOrDefault(s => s.AuthorizationSessionServices.Count() == servicesIds.Count() && s.AuthorizationSessionServices.All(s => servicesIds.Contains(s.ServiceId)));


            if (session == null)
                return Result.Fail("session is null");

            return Result.Ok(session);
        }

        private async Task<IEnumerable<ServiceResponseDso>> GetServicesAsync(List<string> servicesIds)
        {
            var response = await _serviceService.GetAllByAsync(new List<FilterCondition>
            {
                new FilterCondition("Id",servicesIds,FilterOperator.In)
            }, new ParamOptions(["ModelAi.ModelGateway"]));

            return response.Data;
        }

        public async Task<AuthorizationSessionResponseDso> PrepareSession(
            DataAuthSession dataAuthSession,
            params Func<Dictionary<string, object>, DataAuthSession, Task<Dictionary<string, object>>>[] conditions)
        {
            try
            {
                // check if platform token is validate 
                DataTokenRequest dataTokenRequest = ValidatePlatform(dataAuthSession.Token);

                List<Claim> claims = [];
                var sessionData = new Dictionary<string, object>();

                foreach (var condition in conditions)
                {
                    sessionData = await condition(sessionData, dataAuthSession);
                }
                // check if service is createspace and spaces are available 
                await CheckCreateSpace(dataAuthSession.ServicesIds);
                var resultSession = await _checker.CheckAndResultAsync(AuthorizationSessionValidatorStates.HasMatchingSession, new DataFilter
                {
                    Items = new Dictionary<string, object>
                    {
                        { "userId", _userClaims.UserId },
                        { "servicesIds", dataAuthSession.ServicesIds },
                        { "authorizationType", dataTokenRequest?.AuthorizationType ?? string.Empty }
                    }
                });

                if (resultSession.Success == true)
                {
                    var session = MapToResponse(resultSession.Result!);
                    return await PrepareSessionTokenAsync(dataAuthSession, dataTokenRequest, claims, sessionData, session);
                }



                var resultServices = await _checker.CheckAndResultAsync(AuthorizationSessionValidatorStates.IsServicesAsignedToUser, new DataFilter
                {
                    Value = dataAuthSession.ServicesIds,
                    Items = new Dictionary<string, object> { { "userId", _userClaims.UserId } }
                });

                if (resultServices.Success == false)
                    throw new ProblemDetailsException(resultServices.Result ?? resultServices.Message!);

                // get model gateway
                //var services = resultServices.Result is not null
                //    ? Map<object, List<ServiceResponseDso>>(resultServices.Result)
                //    : new List<ServiceResponseDso>();
                var newSession = await PrepareSessionToCreate(dataAuthSession.ServicesIds, dataTokenRequest?.AuthorizationType ?? string.Empty, dataTokenRequest?.Expires);
                return await PrepareSessionTokenAsync(dataAuthSession, dataTokenRequest, claims, sessionData, newSession);
            }
            catch
            {
                throw;
            }
        }

        public async Task<AuthorizationSessionResponseDso> CreateForMyAsignedServices(DataAuthSession dataAuthSession)
        {
            var conds = new List<FilterCondition>
            {
                new(nameof(UserServiceRequestDso.UserId), _userClaims.UserId),
                new(nameof(UserServiceRequestDso.ServiceId), dataAuthSession.Except,FilterOperator.NotIn),
            };

            //if(dataAuthSession.)
            var response = await _userServiceService.GetAllByAsync(conds);

            if (response.TotalRecords == 0) throw new Exception("There is no any service assigned to you.");

            dataAuthSession.ServicesIds = response.Data.Select(s => s.ServiceId).ToList()!;
            return await CreateSession(dataAuthSession);
        }

        public async Task<AuthorizationSessionResponseDso> CreateSession(DataAuthSession dataAuthSession)
        {
            try
            {

                var conditions = new List<Func<Dictionary<string, object>, DataAuthSession, Task<Dictionary<string, object>>>>();
                conditions.Add(AllowedRequests());

                if (!dataAuthSession.SpaceId.IsNullOrWhiteSpace()) conditions.Add(SpaceRequired());


                return await PrepareSession(dataAuthSession, [.. conditions]);
            }


            catch
            {
                throw;
            }
        }



        #region Conditions
        private Func<Dictionary<string, object>, DataAuthSession, Task<Dictionary<string, object>>> SpaceRequired()
        {
            return async (dic, dataAuthSession) =>
            {
                //string spaceId = obj.ToString();
                if (string.IsNullOrWhiteSpace(dataAuthSession.SpaceId)) throw new ProblemDetailsException("Space Id is required.");

                var resultSpace = await _checker.CheckAndResultAsync(SpaceValidatorStates.HasSubscriptionId,
                    new DataFilter(dataAuthSession.SpaceId)
                    {
                        Value = _userClaims.SubscriptionId
                    });

                if (resultSpace.Success == false)
                    throw new ProblemDetailsException(resultSpace.Result ?? resultSpace.Message!);

                var space = Map<object, SpaceResponseDso>(resultSpace.Result!);
                dic["Space"] = new { space.Id, space.Name };
                return dic;
            };
        }
        private Func<Dictionary<string, object>, DataAuthSession, Task<Dictionary<string, object>>> AllowedRequests()
        {
            return async (dic, v) =>
            {
                if (!await _checker.CheckAsync(SubscriptionValidatorStates.IsAllowedRequests, _userClaims.SubscriptionId))
                {
                    throw new ProblemDetailsException("You have exhausted all allowed subscription requests.");
                }
                return dic;
            };
        }


        #endregion
        private async Task<AuthorizationSessionResponseDso> PrepareSessionToCreate(List<string> servicesIds, string type, DateTime? expire)
        {
            try
            {
                expire ??= DateTime.UtcNow.AddDays(30);
                string? ipAddress = _userClaims.HttpContext?.Connection?.RemoteIpAddress?.ToString();



                var newSession = new AuthorizationSessionRequestDso
                {
                    UserId = _userClaims.UserId,
                    EndTime = expire,
                    SessionToken = _checker.Injector.TokenService.GenerateTemporary(expires: expire),
                    AuthorizationType = type,
                    IpAddress = ipAddress,
                    DeviceInfo = "",
                    ServicesIds = JsonConvert.SerializeObject(servicesIds),
                };

                foreach (var serviceId in servicesIds)
                {
                    newSession.AuthorizationSessionServices.Add(new AuthorizationSessionServiceRequestBuildDto
                    {
                        ServiceId = serviceId,

                    });
                }

                return await CreateAsync(newSession);
                //var s1 = GetMapper().Map<AuthorizationSession>(newSession);
                //await _checker.Injector.ContextFactory.ExecuteInScopeAsync(async ctx =>
                //{
                //    ctx.AuthorizationSessions.Attach(s1);
                //    await ctx.SaveChangesAsync();
                //    return s1;
                //});

                //return GetMapper().Map<AuthorizationSessionResponseDso>(newSession);
            }
            catch (Exception)
            {

                throw;
            }
        }

        private async Task CheckCreateSpace(List<string> servicesIds)
        {
            if (await _checker.CheckAsync(ServiceValidatorStates.IsCreateSpaceIn, new DataFilter { Value = servicesIds }))
            {
                if (await _checker.CheckAndResultAsync(SubscriptionValidatorStates.HasAllowedSpaces, _userClaims.SubscriptionId)
                is { Success: false } resultSpaces)
                    throw new ProblemDetailsException(resultSpaces.Result ?? resultSpaces.Message!);
            }
        }

        private DataTokenRequest? ValidatePlatform(string token)
        {
            var resultToken = _checker.CheckAndResult(TokenValidatorStates.ValidatePlatformToken, token);
            if (resultToken.Success == false) throw new Exception(resultToken.Message);
            var dataTokenRequest = (DataTokenRequest?)resultToken.Result;
            return dataTokenRequest;
        }

        private async Task<AuthorizationSessionResponseDso> PrepareSessionTokenAsync(DataAuthSession dataAuth, DataTokenRequest dataTokenRequest, List<Claim> claims, Dictionary<string, object>? sessionData, AuthorizationSessionResponseDso newSession)
        {
            sessionData ??= new Dictionary<string, object>();
            var services = (await GetServicesAsync(dataAuth.ServicesIds)).ToList();
            var modelCore = services[0].ModelAi.ModelGateway;

            sessionData["SessionId"] = newSession.Id;

            var urlCore = $"{modelCore.Url}";
            //foreach (var service in services)
            //{
            //if (service.AbsolutePath == ServiceType.Space)
            //{
            urlCore += $"/{services[0].AbsolutePath}";
            //}
            //}
            sessionData["Services"] = services.Select(s => new
            {
                s.Id,
                s.AbsolutePath,
                modelAi = s.ModelAi.AbsolutePath,
                ModelGateway = $"{modelCore.Url}"
            }).ToList();

            claims.AddRange([
       new Claim(ClaimTypes2.SessionToken, newSession.SessionToken),
                    new Claim(ClaimTypes2.ApiUrl, GetApiUrl()),
                    new Claim(ClaimTypes2.WebToken, dataTokenRequest.Token),
                    new Claim(ClaimTypes2.Data,JsonConvert.SerializeObject(sessionData))]);

            var encrptedToken = _checker.Injector.TokenService.GenerateToken(claims, modelCore.Token, newSession.EndTime);

            //string urlCore = $"{modelCore.Url}/{services.AbsolutePath}";
            return new AuthorizationSessionResponseDso()
            {
                SessionToken = encrptedToken,
                URLCore = $"{urlCore}"
            };
        }

        public string SimulationCore(string encrptedToken, string coreToken)
        {
            _logger.LogInformation("Creating core token with data: {@encrptedToken}", encrptedToken);
            var decrptedToken = _tokenService.ValidateToken(encrptedToken, coreToken);
            if (decrptedToken.IsFailed)
                throw new Exception(decrptedToken.Errors.FirstOrDefault().Message);
            var claims = decrptedToken.ValueOrDefault;
            var sessionToken = claims.FindFirstValue(ClaimTypes2.SessionToken);
            var data = claims.FindFirstValue(ClaimTypes2.Data);
            var webToken = claims.FindFirstValue(ClaimTypes2.WebToken);

            var token = _tokenService.GenerateToken([new Claim(ClaimTypes2.SessionToken, sessionToken)], webToken);

            return token;

        }
        public string SimulationPlatForm(EncryptTokenRequest encryptToken)
        {
            _logger.LogInformation("Creating platform token with data: {@encryptToken}", encryptToken);

            var webToken = _appSettings.Jwt.WebSecret;
            List<Claim> claims = [new Claim(ClaimTypes2.Data, JsonConvert.SerializeObject(encryptToken))];
            //if (encryptToken.Expires != null) claims.Add(new Claim("Expires", encryptToken!.Expires!.ToString()!));
            var encrptedToken = _tokenService.GenerateToken(claims, webToken!, encryptToken.Expires);
            return encrptedToken;
        }
        public string? GetApiUrl()
        {
            var request = _userClaims.HttpContext?.Request;
            if (request == null)
                return null;

            return _linkGenerator.GetUriByAction(
                action: "Validate",
                controller: "AuthorizationSession",
                values: null,
                scheme: request.Scheme,
                host: request.Host);
        }

    }
}