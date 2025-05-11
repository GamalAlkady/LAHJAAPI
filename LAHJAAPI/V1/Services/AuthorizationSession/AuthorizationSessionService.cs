using APILAHJA.Utilities;
using AutoGenerator;
using AutoGenerator.Helper;
using AutoGenerator.Services.Base;
using AutoMapper;
using FluentResults;
using LAHJAAPI.Exceptions;
using LAHJAAPI.Services2;
using LAHJAAPI.Utilities;
using LAHJAAPI.V1.Validators;
using LAHJAAPI.V1.Validators.Conditions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Security.Claims;
using V1.DyModels.Dso.Requests;
using V1.DyModels.Dso.Responses;
using V1.DyModels.Dto.Build.Requests;
using V1.DyModels.Dto.Share.Requests;
using V1.DyModels.VMs;
using V1.Repositories.Share;
using WasmAI.ConditionChecker.Base;

namespace V1.Services.Services
{
    public class AuthorizationSessionService : BaseService<AuthorizationSessionRequestDso, AuthorizationSessionResponseDso>, IUseAuthorizationSessionService
    {
        private readonly IAuthorizationSessionShareRepository _share;
        private readonly IUserClaimsHelper _userClaims;
        private readonly IUseServiceService _serviceShare;
        private readonly IConditionChecker _checker;
        private readonly LinkGenerator _linkGenerator;
        private readonly TokenService _tokenService;
        private readonly AppSettings _appSettings;
        public AuthorizationSessionService(
            IAuthorizationSessionShareRepository buildAuthorizationSessionShareRepository,
            IUserClaimsHelper userClaims,
            IUseServiceService serviceShare,
            IConditionChecker checker,
            IOptions<AppSettings> options,
            IMapper mapper,
            ILoggerFactory logger,
            LinkGenerator linkGenerator,
            TokenService tokenService) : base(mapper, logger)
        {
            _share = buildAuthorizationSessionShareRepository;
            _userClaims = userClaims;
            _serviceShare = serviceShare;
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

        public async Task<AuthorizationSessionResponseDso> GetOrCreateSession(List<string> servicesIds, string type, DateTime? expire, string modelAiId)
        {
            //var resultSession = await _authorizationsessionService.GetSessionByServices(_userClaims.UserId, servicesIds, type);
            var result = await _checker.CheckAndResultAsync(AuthorizationSessionValidatorStates.HasMatchingSession, new DataFilter
            {
                Items = new Dictionary<string, object>
                {
                    {"userId" ,_userClaims.UserId},
                    { "ServicesIds", servicesIds },
                    { "AuthorizationType", type }
                }
            });

            if (result.Success == true)
            {
                return GetMapper().Map<AuthorizationSessionResponseDso>(result.Result);
            }
            else
            {
                var services = GetMapper().Map<List<ServiceRequestBuildDto>>(result.Result);

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
                    //Services = services
                };


                return await CreateAsync(newSession);
            }
        }


        private async Task<AuthorizationSessionResponseDso> PrepareCreateSession(List<ServiceResponseDso> services, string token, List<string> servicesIds, string ApiUrl, string? spaceId, bool isSpaceRequired = true)
        {
            try
            {
                // check if platform token is validate 
                var resultToken = _checker.CheckAndResult(TokenValidatorStates.ValidatePlatformToken, token);
                if (resultToken.Success == false) throw new Exception(resultToken.Message);
                var dataTokenRequest = (DataTokenRequest?)resultToken.Result;

                List<Claim> claims = [];

                var sessionData = new Dictionary<string, object>
                {
                    { "Services", services.Select(s => new { s.Id, s.AbsolutePath }).ToList() },
                };

                // check if service is createspace and spaces are available 
                //var serviceCreateSpace = services.FirstOrDefault(s => s.AbsolutePath == ServiceType.Space);
                if (services.Exists(s => s.AbsolutePath == ServiceType.Space))
                {
                    if (await _checker.CheckAndResultAsync(SubscriptionValidatorStates.IsAvailableSpaces, _userClaims.SubscriptionId)
                    is { Success: false } resultSpaces)
                        throw new ProblemDetailsException(resultSpaces.Result ?? resultSpaces.Message!);
                }
                else
                {

                    //check if services is not dashboard and check if requests allowed
                    //if (services.Count > 1 && serviceCreateSpace == null)
                    {
                        if (!services.Exists(s => s.AbsolutePath == ServiceType.Dash)
                            && !await _checker.CheckAsync(SubscriptionValidatorStates.IsAllowedRequests, _userClaims.SubscriptionId))
                        {
                            throw new ProblemDetailsException("You have exhausted all allowed subscription requests.");
                            //return ConditionResult.ToError("You have exhausted all allowed subscription requests.");
                        }

                        //if (isSpaceRequired)
                        {
                            if (string.IsNullOrWhiteSpace(spaceId)) throw new ProblemDetailsException("Space Id is required.");
                            var resultSpace = await _checker.CheckAndResultAsync(SpaceValidatorStates.HasSubscriptionId,
                                new DataFilter(spaceId)
                                {
                                    Value = _userClaims.SubscriptionId
                                });

                            if (resultSpace.Success == false)
                                throw new ProblemDetailsException(resultSpace.Result ?? resultSpace.Message!);

                            var space = (SpaceResponseDso)resultSpace.Result!;
                            sessionData["Space"] = new { space.Id, space.Name };
                        }
                    }
                }

                var modelAiId = services[0].ModelAiId;
                var result = await _checker.CheckAndResultAsync(ModelValidatorStates.IsHasModelGateway, modelAiId);
                if (result.Success == false) throw new ProblemDetailsException(result.Result ?? result.Message);


                //var modelAi = await _modelAiRepository.GetOneByAsync([new FilterCondition("Id", services[0].ModelAiId)], new ParamOptions(["ModelGateway"]));
                var modelAi = GetMapper().Map<ModelAiResponseDso>(result.Result);

                var modelCore = modelAi.ModelGateway;
                //if (modelCore == null) throw new Exception("This model ai not belong to model gateway.");


                //AuthorizationSessionResponseDso session = await GetOrCreateSession(servicesIds, dataTokenRequest.AuthorizationType, dataTokenRequest.Expires, modelAiId);
                var session = await GetOrCreateSession(servicesIds, dataTokenRequest.AuthorizationType, dataTokenRequest.Expires, modelAiId);
                sessionData["SessionId"] = session.Id;
                claims.AddRange([new Claim(ClaimTypes2.SessionToken, session.SessionToken),
                    new Claim(ClaimTypes2.ApiUrl, ApiUrl),
                    new Claim(ClaimTypes2.WebToken, dataTokenRequest.Token),
                    new Claim(ClaimTypes2.Data,JsonConvert.SerializeObject(sessionData))]);

                var encrptedToken = _checker.Injector.TokenService.GenerateToken(claims, modelCore.Token, session.EndTime);

                string urlCore = $"{modelCore.Url}";
                if (services.Count == 1) urlCore += $"/{services[0].AbsolutePath}";
                //string urlCore = $"{modelCore.Url}/{services.AbsolutePath}";
                return new AuthorizationSessionResponseDso()
                {
                    SessionToken = encrptedToken,
                    URLCore = urlCore
                };
            }
            catch
            {
                throw;
            }
        }

        public async Task<AuthorizationSessionResponseDso> CreateGeneralSession(DataAuthSession dataAuthSession)
        {
            try
            {
                // check if platform token is validate 
                DataTokenRequest? dataTokenRequest = ValidatePlatform(dataAuthSession.Token);

                List<Claim> claims = [];


                // check if service is createspace and spaces are available 
                await CheckCreateSpace(dataAuthSession.ServicesIds);

                var resultSession = await _checker.CheckAndResultAsync(AuthorizationSessionValidatorStates.HasMatchingSession, new DataFilter
                {
                    Items = new Dictionary<string, object>
                {
                        { "userId", _userClaims.UserId },
                    { "servicesIds", dataAuthSession.ServicesIds },
                    { "authorizationType", dataTokenRequest.AuthorizationType }
                }
                });

                var session = GetMapper().Map<AuthorizationSessionResponseDso>(resultSession.Result);
                if (resultSession.Success == true)
                {
                    //var services = GetMapper().Map<List<ServiceResponseDso>>(session.AuthorizationSessionServices);
                    //var modelCore = services[0].ModelAi.ModelGateway;
                    return PreapareSessionToken(GetMapper().Map<List<ServiceResponseDso>>(session.AuthorizationSessionServices.Select(s => s.Service)), GetApiUrl(), dataTokenRequest, claims, null, session);
                }


                var resultServices = await _checker.CheckAndResultAsync(AuthorizationSessionValidatorStates.IsServicesAsignedToUser, new DataFilter
                {
                    Value = dataAuthSession.ServicesIds,
                    Items = new Dictionary<string, object> { { "userId", _userClaims.UserId } }
                });

                if (resultServices.Success == false)
                    throw new ProblemDetailsException(resultServices.Result ?? resultServices.Message!);

                // get model gateway
                var services = GetMapper().Map<List<ServiceResponseDso>>(resultServices.Result);
                //AuthorizationSessionResponseDso session = await GetOrCreateSession(servicesIds, dataTokenRequest.AuthorizationType, dataTokenRequest.Expires, modelAiId);
                var newSession = await PrepareCreateSession(dataAuthSession.ServicesIds, dataTokenRequest.AuthorizationType, dataTokenRequest.Expires);
                return PreapareSessionToken(services, GetApiUrl(), dataTokenRequest, claims, null, newSession);
            }
            catch
            {
                throw;
            }
        }

        public async Task<AuthorizationSessionResponseDso?> CreateForDashboard(DataAuthSession dataAuthSession)
        {
            var service = await _serviceShare.GetOneByAsync(new List<FilterCondition>
            {
                new FilterCondition(nameof(ServiceRequestBuildDto.AbsolutePath),ServiceType.Dash)
            });
            if (service == null) return null;
            dataAuthSession.ServicesIds = [service.Id];
            return await CreateGeneralSession(dataAuthSession);
        }

        public async Task<AuthorizationSessionResponseDso> CreateForAllServices(DataAuthSession dataAuthSession)
        {
            DataTokenRequest? dataTokenRequest = ValidatePlatform(dataAuthSession.Token);

            if (!await _checker.CheckAsync(SubscriptionValidatorStates.IsAllowedRequests, _userClaims.SubscriptionId))
            {
                throw new ProblemDetailsException("You have exhausted all allowed subscription requests.");
                //return ConditionResult.ToError("You have exhausted all allowed subscription requests.");
            }

            var services = await _serviceShare.GetListWithoutSome();
            dataAuthSession.ServicesIds = services.Select(s => s.Id).ToList();
            return await CreateGeneralSession(dataAuthSession);
        }

        public async Task<AuthorizationSessionResponseDso> CreateSecretSession(DataAuthSession dataAuthSession)
        {
            try
            {
                // check if platform token is validate 
                DataTokenRequest? dataTokenRequest = ValidatePlatform(dataAuthSession.Token);

                List<Claim> claims = [];


                // check if service is createspace and spaces are available 
                await CheckCreateSpace(dataAuthSession.ServicesIds);

                var resultSession = await _checker.CheckAndResultAsync(AuthorizationSessionValidatorStates.HasMatchingSession, new DataFilter
                {
                    Items = new Dictionary<string, object>
                {
                        {"userId",_userClaims.UserId },
                    { "ServicesIds", dataAuthSession.ServicesIds },
                    { "AuthorizationType", dataTokenRequest.AuthorizationType }
                }
                });

                var session = GetMapper().Map<AuthorizationSessionResponseDso>(resultSession.Result);
                if (resultSession.Success == true)
                {
                    //var services = GetMapper().Map<List<ServiceResponseDso>>(session.Services);
                    //var modelCore = services[0].ModelAi.ModelGateway;
                    return PreapareSessionToken(GetMapper().Map<List<ServiceResponseDso>>(session.AuthorizationSessionServices.Select(s => s.Service)), GetApiUrl(), dataTokenRequest, claims, null, session);
                }


                var resultServices = await _checker.CheckAndResultAsync(AuthorizationSessionValidatorStates.IsServicesAsignedToUser, new DataFilter
                {
                    Value = dataAuthSession.ServicesIds,
                    Items = new Dictionary<string, object> { { "userId", _userClaims.UserId } }
                });

                if (resultServices.Success == false)
                    throw new ProblemDetailsException(resultServices.Result ?? resultServices.Message);

                // get model gateway
                var services = GetMapper().Map<List<ServiceResponseDso>>(resultServices.Result);
                //AuthorizationSessionResponseDso session = await GetOrCreateSession(servicesIds, dataTokenRequest.AuthorizationType, dataTokenRequest.Expires, modelAiId);
                var newSession = await PrepareCreateSession(dataAuthSession.ServicesIds, dataTokenRequest.AuthorizationType, dataTokenRequest.Expires);
                return PreapareSessionToken(services, GetApiUrl(), dataTokenRequest, claims, null, newSession);
            }
            catch
            {
                throw;
            }
        }

        public async Task<AuthorizationSessionResponseDso> PrepareCreateSession(List<string> servicesIds, string type, DateTime? expire)
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

            //return await CreateAsync(newSession);
        }

        private async Task CheckCreateSpace(List<string> servicesIds)
        {
            if (await _checker.CheckAsync(ServiceValidatorStates.IsCreateSpaceIn, new DataFilter { Value = servicesIds }))
            {
                if (await _checker.CheckAndResultAsync(SubscriptionValidatorStates.IsAvailableSpaces, _userClaims.SubscriptionId)
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

        private AuthorizationSessionResponseDso PreapareSessionToken(List<ServiceResponseDso> services, string ApiUrl, DataTokenRequest dataTokenRequest, List<Claim> claims, Dictionary<string, object>? sessionData, AuthorizationSessionResponseDso newSession)
        {
            sessionData ??= new Dictionary<string, object>();

            sessionData["SessionId"] = newSession.Id;
            claims.AddRange([
                new Claim(ClaimTypes2.SessionToken, newSession.SessionToken),
                    new Claim(ClaimTypes2.ApiUrl, ApiUrl),
                    new Claim(ClaimTypes2.WebToken, dataTokenRequest.Token),
                    new Claim(ClaimTypes2.Data,JsonConvert.SerializeObject(sessionData))]);

            var modelCore = services[0].ModelAi.ModelGateway;
            var encrptedToken = _checker.Injector.TokenService.GenerateToken(claims, modelCore.Token, newSession.EndTime);

            var urlCore = $"{modelCore.Url}";
            foreach (var service in services)
            {
                if (service.AbsolutePath == ServiceType.Space)
                {
                    urlCore += $"/{service.AbsolutePath}";
                }
            }
            sessionData["Services"] = services.Select(s => new
            {
                s.Id,
                s.AbsolutePath,
                modelAi = s.ModelAi.AbsolutePath,
                UrlCore = $"{urlCore}/{s.AbsolutePath}"
            }).ToList();

            //string urlCore = $"{modelCore.Url}/{services.AbsolutePath}";
            return new AuthorizationSessionResponseDso()
            {
                SessionToken = encrptedToken,
                URLCore = $"{urlCore}/{services[0].AbsolutePath}"
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



        private AuthorizationSessionResponseDso MapToResponse(AuthorizationSessionRequestDso requestDso)
        {
            return GetMapper().Map<AuthorizationSessionResponseDso>(requestDso);
        }

        public override Task<int> CountAsync()
        {
            try
            {
                _logger.LogInformation("Counting AuthorizationSession entities...");
                return _share.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CountAsync for AuthorizationSession entities.");
                return Task.FromResult(0);
            }
        }

        public override async Task<AuthorizationSessionResponseDso> CreateAsync(AuthorizationSessionRequestDso entity)
        {
            try
            {
                _logger.LogInformation("Creating new AuthorizationSession entity...");
                var result = await _share.CreateAsync(entity);
                var output = GetMapper().Map<AuthorizationSessionResponseDso>(result);
                _logger.LogInformation("Created AuthorizationSession entity successfully.");
                return output;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating AuthorizationSession entity.");
                return null;
            }
        }

        public override Task DeleteAsync(string id)
        {
            try
            {
                _logger.LogInformation($"Deleting AuthorizationSession entity with ID: {id}...");
                return _share.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while deleting AuthorizationSession entity with ID: {id}.");
                return Task.CompletedTask;
            }
        }

        public override async Task<IEnumerable<AuthorizationSessionResponseDso>> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("Retrieving all AuthorizationSession entities...");
                var results = await _share.GetAllAsync();
                return GetMapper().Map<IEnumerable<AuthorizationSessionResponseDso>>(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAllAsync for AuthorizationSession entities.");
                return null;
            }
        }

        public override async Task<AuthorizationSessionResponseDso?> GetByIdAsync(string id)
        {
            try
            {
                _logger.LogInformation($"Retrieving AuthorizationSession entity with ID: {id}...");
                var result = await _share.GetByIdAsync(id);
                var item = GetMapper().Map<AuthorizationSessionResponseDso>(result);
                _logger.LogInformation("Retrieved AuthorizationSession entity successfully.");
                return item;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in GetByIdAsync for AuthorizationSession entity with ID: {id}.");
                return null;
            }
        }

        public override IQueryable<AuthorizationSessionResponseDso> GetQueryable()
        {
            try
            {
                _logger.LogInformation("Retrieving IQueryable<AuthorizationSessionResponseDso> for AuthorizationSession entities...");
                var queryable = _share.GetQueryable();
                var result = GetMapper().ProjectTo<AuthorizationSessionResponseDso>(queryable);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetQueryable for AuthorizationSession entities.");
                return null;
            }
        }

        public override async Task<AuthorizationSessionResponseDso> UpdateAsync(AuthorizationSessionRequestDso entity)
        {
            try
            {
                _logger.LogInformation("Updating AuthorizationSession entity...");
                var result = await _share.UpdateAsync(entity);
                return GetMapper().Map<AuthorizationSessionResponseDso>(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateAsync for AuthorizationSession entity.");
                return null;
            }
        }

        public override async Task<bool> ExistsAsync(object value, string name = "Id")
        {
            try
            {
                _logger.LogInformation("Checking if AuthorizationSession exists with {Key}: {Value}", name, value);
                var exists = await _share.ExistsAsync(value, name);
                if (!exists)
                {
                    _logger.LogWarning("AuthorizationSession not found with {Key}: {Value}", name, value);
                }

                return exists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while checking existence of AuthorizationSession with {Key}: {Value}", name, value);
                return false;
            }
        }

        public override async Task<PagedResponse<AuthorizationSessionResponseDso>> GetAllAsync(string[]? includes = null, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                _logger.LogInformation("Fetching all AuthorizationSessions with pagination: Page {PageNumber}, Size {PageSize}", pageNumber, pageSize);
                var results = (await _share.GetAllAsync(includes, pageNumber, pageSize));
                var items = GetMapper().Map<List<AuthorizationSessionResponseDso>>(results.Data);
                return new PagedResponse<AuthorizationSessionResponseDso>(items, results.PageNumber, results.PageSize, results.TotalPages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching all AuthorizationSessions.");
                return new PagedResponse<AuthorizationSessionResponseDso>(new List<AuthorizationSessionResponseDso>(), pageNumber, pageSize, 0);
            }
        }

        public override async Task<AuthorizationSessionResponseDso?> GetByIdAsync(object id)
        {
            try
            {
                _logger.LogInformation("Fetching AuthorizationSession by ID: {Id}", id);
                var result = await _share.GetByIdAsync(id);
                if (result == null)
                {
                    _logger.LogWarning("AuthorizationSession not found with ID: {Id}", id);
                    return null;
                }

                _logger.LogInformation("Retrieved AuthorizationSession successfully.");
                return GetMapper().Map<AuthorizationSessionResponseDso>(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while retrieving AuthorizationSession by ID: {Id}", id);
                return null;
            }
        }

        public override async Task DeleteAsync(object value, string key = "Id")
        {
            try
            {
                _logger.LogInformation("Deleting AuthorizationSession with {Key}: {Value}", key, value);
                await _share.DeleteAsync(value, key);
                _logger.LogInformation("AuthorizationSession with {Key}: {Value} deleted successfully.", key, value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting AuthorizationSession with {Key}: {Value}", key, value);
            }
        }

        public override async Task DeleteRange(List<AuthorizationSessionRequestDso> entities)
        {
            try
            {
                var builddtos = entities.OfType<AuthorizationSessionRequestShareDto>().ToList();
                _logger.LogInformation("Deleting {Count} AuthorizationSessions...", 201);
                await _share.DeleteRange(builddtos);
                _logger.LogInformation("{Count} AuthorizationSessions deleted successfully.", 202);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting multiple AuthorizationSessions.");
            }
        }

        public override async Task<PagedResponse<AuthorizationSessionResponseDso>> GetAllByAsync(List<FilterCondition> conditions, ParamOptions? options = null)
        {
            try
            {
                _logger.LogInformation("Retrieving all AuthorizationSession entities...");
                var results = await _share.GetAllAsync();
                var response = await _share.GetAllByAsync(conditions, options);
                return response.ToResponse(GetMapper().Map<IEnumerable<AuthorizationSessionResponseDso>>(response.Data));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAllAsync for AuthorizationSession entities.");
                return null;
            }
        }

        public override async Task<AuthorizationSessionResponseDso?> GetOneByAsync(List<FilterCondition> conditions, ParamOptions? options = null)
        {
            try
            {
                _logger.LogInformation("Retrieving AuthorizationSession entity...");
                return GetMapper().Map<AuthorizationSessionResponseDso>(await _share.GetOneByAsync(conditions, options));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetOneByAsync  for AuthorizationSession entity.");
                return null;
            }
        }
    }
}