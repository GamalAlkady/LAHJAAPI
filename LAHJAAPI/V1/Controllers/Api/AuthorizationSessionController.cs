using APILAHJA.Utilities;
using AutoGenerator.Conditions;
using AutoGenerator.Helper;
using AutoGenerator.Helper.Translation;
using AutoMapper;
using LAHJAAPI.Services2;
using LAHJAAPI.Utilities;
using LAHJAAPI.V1.Validators;
using LAHJAAPI.V1.Validators.Conditions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using V1.DyModels.Dso.Requests;
using V1.DyModels.Dso.Responses;
using V1.DyModels.VMs;
using V1.Services.Services;

namespace LAHJAAPI.V1.Controllers.Api
{
    [Route("api/v1/user/[controller]")]
    [ApiController]
    public class AuthorizationSessionController : ControllerBase
    {
        private readonly IUseAuthorizationSessionService _authorizationsessionService;
        private readonly LinkGenerator _linkGenerator;
        private readonly TokenService _tokenService;
        private readonly IUseServiceService _serviceService;
        private readonly IUserClaimsHelper _userClaims;
        private readonly IConditionChecker _checker;
        private readonly IOptions<AppSettings> _appSettings;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        public AuthorizationSessionController(IUseAuthorizationSessionService authorizationsessionService,
            LinkGenerator linkGenerator,
            TokenService tokenService,
            IUseModelGatewayService modelGatewayService,
            IUseServiceService serviceService,
            IUseSubscriptionService subscriptionService,
            IUseModelAiService modelAiRepository,
            IUserClaimsHelper userClaims,
            IConditionChecker checker,
            IOptions<AppSettings> appSettings,
            IMapper mapper, ILoggerFactory logger)
        {
            _linkGenerator = linkGenerator;
            _tokenService = tokenService;
            _authorizationsessionService = authorizationsessionService;
            _serviceService = serviceService;
            _userClaims = userClaims;
            _checker = checker;
            _appSettings = appSettings;
            _mapper = mapper;
            _logger = logger.CreateLogger(typeof(AuthorizationSessionController).FullName);
        }

        // Get all AuthorizationSessions.
        [HttpGet(Name = "GetAuthorizationSessions")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<AuthorizationSessionOutputVM>>> GetAll()
        {
            try
            {
                _logger.LogInformation("Fetching all AuthorizationSessions...");
                var result = await _authorizationsessionService.GetAllAsync();
                var items = _mapper.Map<List<AuthorizationSessionOutputVM>>(result);
                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching all AuthorizationSessions");
                return StatusCode(500, "Internal Server Error");
            }
        }

        // Get a AuthorizationSession by ID.
        [HttpGet("{id}", Name = "GetAuthorizationSession")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AuthorizationSessionOutputVM>> GetById(string id, string? lg)
        {

            try
            {
                _logger.LogInformation("Fetching AuthorizationSession with ID: {id}", id);
                var entity = await _authorizationsessionService.GetByIdAsync(id);
                if (entity == null)
                {
                    _logger.LogWarning("AuthorizationSession not found with ID: {id}", id);
                    return NotFound();
                }

                if (string.IsNullOrWhiteSpace(lg)) return Ok(_mapper.Map<AuthorizationSessionOutputVM>(entity));
                return Ok(_mapper.Map<AuthorizationSessionOutputVM>(entity, opts => opts.Items[HelperTranslation.KEYLG] = lg));

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching AuthorizationSession with ID: {id}", id);
                return StatusCode(500, "Internal Server Error");
            }
        }


        // // Get a AuthorizationSessions by Lg.
        [HttpGet("GetAuthorizationSessionsByLanguage", Name = "GetAuthorizationSessionsByLg")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<AuthorizationSessionOutputVM>>> GetAuthorizationSessionsByLg(string? lg)
        {
            if (string.IsNullOrWhiteSpace(lg))
            {
                _logger.LogWarning("Invalid AuthorizationSession lg received.");
                return BadRequest("Invalid AuthorizationSession lg null ");
            }

            try
            {
                var authorizationsessions = await _authorizationsessionService.GetAllAsync();
                if (authorizationsessions == null)
                {
                    _logger.LogWarning("AuthorizationSessions not found  by  ");
                    return NotFound();
                }

                var items = _mapper.Map<IEnumerable<AuthorizationSessionOutputVM>>(authorizationsessions, opt => opt.Items.Add(HelperTranslation.KEYLG, lg));
                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching AuthorizationSessions with Lg: {lg}", lg);
                return StatusCode(500, "Internal Server Error");
            }
        }


        [AllowAnonymous]
        [EndpointSummary("Validate Authorization Session")]
        [HttpPost("validate", Name = "AuthorizationSession")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AuthorizationSessionCoreResponse>> Validate(ValidateTokenRequest validateToken)
        {
            try
            {
                _logger.LogInformation("Validating AuthorizationSession with data: {@validateToken}", validateToken);
                //TODO: When create token for service createspace make it temporary
                var result = _checker.CheckAndResult(TokenValidatorStates.ValidateCoreToken, validateToken.Token);
                if (result.Success == false) return BadRequest(result.Message);

                var item = await _authorizationsessionService.GetOneByAsync([new FilterCondition("SessionToken", result.Result)]);
                if (item == null) return BadRequest("Not found session by token");

                var token = _tokenService.GenerateToken([
                    new Claim(ClaimTypes2.ServicesIds, item.ServicesIds),
                    new Claim(JwtRegisteredClaimNames.Sub, item.UserId),
                    new Claim(ClaimTypes2.SessionId, item.Id)]);

                item.UserToken = token;
                await _authorizationsessionService.UpdateAsync(_mapper.Map<AuthorizationSessionRequestDso>(item));
                return Ok(new AuthorizationSessionCoreResponse { Token = item.UserToken });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while validating AuthorizationSession");
                return BadRequest(HandelErrors.Problem(ex));
            }
        }


        // Create a new AuthorizationSession.
        [ServiceFilter(typeof(SubscriptionCheckFilter))]
        [HttpPost(Name = "CreateAuthorizationSession")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AuthorizationSessionOutputVM>> Create([FromBody] AuthorizationSessionCreateVM model)
        {

            try
            {
                _logger.LogInformation("Creating new AuthorizationSession with data: {@model}", model);
                var result = await _checker.CheckAndResultAsync(ServiceValidatorStates.IsCreateSpace, new DataFilter(model.ServiceId));
                if (result.Result == null) return NotFound(result.Result ?? result.Message);

                var service = _mapper.Map<ServiceResponseDso>(result.Result);
                bool isNeedSpace = true;
                if (result.Success == true)
                {
                    isNeedSpace = false;
                    if (await _checker.CheckAndResultAsync(SubscriptionValidatorStates.IsAvailableSpaces) is { Success: false } result2)
                        return BadRequest(result2.Result ?? result2.Message);
                }

                var response = await PrepareCreateSession([service], model.Token, [model.ServiceId], isNeedSpace);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating a new AuthorizationSession");
                //return StatusCode(500, "Internal Server Error");
                return BadRequest(HandelErrors.Problem(ex));
            }
        }


        [EndpointSummary("Create session for dashboard")]
        [HttpPost("CreateForDashboard")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AuthorizationSessionOutputVM>> CreateForDashboard(CreateAuthorizationForDashboard model)
        {
            try
            {
                _logger.LogInformation("Creating new AuthorizationSession for dashboard with data: {@model}", model);
                var service = await _serviceService.GetByAbsolutePath("dashboard");
                if (service == null) return NotFound(HandelErrors.Problem("Create session", "No service found for dahsboard."));
                var response = await PrepareCreateSession([service], model.Token, [service.Id], false);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating a new AuthorizationSession for dashboard");
                return BadRequest(HandelErrors.Problem(ex));
            }
        }


        [ServiceFilter(typeof(SubscriptionCheckFilter))]
        [EndpointSummary("Create authorization session for list services")]
        [HttpPost("CreateForListServices")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AuthorizationSessionOutputVM>> CreateForListServices(CreateAuthorizationForListServices model)
        {
            try
            {
                _logger.LogInformation("Creating new AuthorizationSession for list services with data: {@model}", model);
                var services = await _serviceService.GetListWithoutSome(model.ServicesIds);
                if (services.Count == 0) return NotFound(HandelErrors.Problem("Create session", "Services ids that you send not aceptable."));
                var servicesIds = services.Select(s => s.Id).ToList();

                var response = await PrepareCreateSession(services, model.Token, model.ServicesIds);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating a new AuthorizationSession for list services");
                return BadRequest(HandelErrors.Problem(ex));
            }
        }


        [ServiceFilter(typeof(SubscriptionCheckFilter))]
        [EndpointSummary("Create authorization session for all services")]
        [HttpPost("CreateForAllServices")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AuthorizationSessionOutputVM>> CreateForAllServices(CreateAuthorizationForServices model)
        {
            try
            {
                _logger.LogInformation("Creating new AuthorizationSession for all services with data: {@model}", model);

                var services = await _serviceService.GetListWithoutSome(modelId: model.ModelAiId);
                if (services.Count == 0) return NotFound(HandelErrors.Problem("Create session", "Services ids that you send not aceptable.", null, 404));
                var servicesIds = services.Select(s => s.Id).ToList();

                var response = await PrepareCreateSession(services, model.Token, servicesIds);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating a new AuthorizationSession for all services");
                return BadRequest(HandelErrors.Problem(ex));
            }
        }

        // Delete a AuthorizationSession.
        [HttpDelete("{id}", Name = "DeleteAuthorizationSession")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(string? id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                _logger.LogWarning("Invalid AuthorizationSession ID received in Delete.");
                return BadRequest("Invalid AuthorizationSession ID.");
            }

            try
            {
                _logger.LogInformation("Deleting AuthorizationSession with ID: {id}", id);
                await _authorizationsessionService.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting AuthorizationSession with ID: {id}", id);
                return StatusCode(500, "Internal Server Error");
            }
        }

        // Get count of AuthorizationSessions.
        [HttpGet("Count", Name = "CountAuthorizationSession")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<int>> Count()
        {
            try
            {
                _logger.LogInformation("Counting AuthorizationSessions...");
                var count = await _authorizationsessionService.CountAsync();
                return Ok(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while counting AuthorizationSessions");
                return StatusCode(500, "Internal Server Error");
            }
        }


        [AllowAnonymous]
        [HttpPost("SimulationPlatForm", Name = "SimulationPlatForm")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<string>> SimulationPlatForm(EncryptTokenRequest encryptToken)
        {
            try
            {
                _logger.LogInformation("Creating platform token with data: {@encryptToken}", encryptToken);
                var webToken = _appSettings.Value.Jwt.WebSecret;
                List<Claim> claims = [new Claim(ClaimTypes2.Data, JsonSerializer.Serialize(encryptToken))];
                //if (encryptToken.Expires != null) claims.Add(new Claim("Expires", encryptToken!.Expires!.ToString()!));
                var encrptedToken = _tokenService.GenerateToken(claims, webToken!, encryptToken.Expires);
                return Ok(encrptedToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when create platform token.");
                return BadRequest(ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpGet("SimulationCore", Name = "SimulationCore")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<string>> SimulationCore(string encrptedToken, string coreToken)
        {
            try
            {
                _logger.LogInformation("Creating core token with data: {@encrptedToken}", encrptedToken);
                var decrptedToken = _tokenService.ValidateToken(encrptedToken, coreToken);
                if (decrptedToken.IsFailed) return Unauthorized(decrptedToken.Errors);
                var claims = decrptedToken.ValueOrDefault;
                var sessionToken = claims.FindFirstValue(ClaimTypes2.SessionToken);
                var data = claims.FindFirstValue(ClaimTypes2.Data);
                var webToken = claims.FindFirstValue(ClaimTypes2.WebToken);

                var token = _tokenService.GenerateToken([new Claim(ClaimTypes2.SessionToken, sessionToken)], webToken);

                return Ok(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when create core token.");
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("ValidateWebTokenAsync")]
        public async Task<ActionResult> ValidateWebTokenAsync(string token)
        {
            var result = _checker.CheckAndResult(TokenValidatorStates.ValidatePlatformToken, token);
            //if (result.Success == false) return BadRequest(result.Message);

            return Ok(result);
        }

        [HttpGet("ValidateCreateToken")]
        public ActionResult ValidateCreateToken(string token, string coreToken)
        {
            var decrptedToken = _tokenService.ValidateToken(token, coreToken);

            return Ok(new { decrptedToken });
        }

        [HttpGet("ValidateCoreToken")]
        public ActionResult ValidateCoreToken(string token, string coreToken)
        {
            var decrptedToken = _tokenService.ValidateToken(token, coreToken);

            return Ok(new { decrptedToken });
        }

        [EndpointSummary("Pause AuthorizationSession")]
        [HttpPut("pause/{id}", Name = "PauseAuthorizationSession")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Pause(string id)
        {
            try
            {
                var session = await _authorizationsessionService.GetByIdAsync(id);
                session.IsActive = false;

                await _authorizationsessionService.UpdateAsync(_mapper.Map<AuthorizationSessionRequestDso>(session));
                return Ok();
            }
            catch (Exception ex)
            {

                return BadRequest(HandelErrors.Problem(ex));
            }
        }

        //private AuthorizationSessionResponseDso MapToResponse()
        //{

        //}
        [EndpointSummary("Resume AuthorizationSession")]
        [HttpPut("resume/{id}", Name = "ResumeAuthorizationSession")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Resume(string id)
        {
            try
            {
                var session = await _authorizationsessionService.GetByIdAsync(id);
                session.IsActive = true;
                await _authorizationsessionService.UpdateAsync(_mapper.Map<AuthorizationSessionRequestDso>(session));
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(HandelErrors.Problem(ex));
            }
        }

        private async Task<AuthorizationSessionInfoVM> PrepareCreateSession(List<ServiceResponseDso> services, string token, List<string> servicesIds, bool isSpaceRequired = true)
        {
            try
            {
                List<Claim> claims = [];
                var sessionData = new SessionData
                {
                    Services = services.Select(s => new { s.Id, s.AbsolutePath }).ToList(),
                    //SubscriptionId = (await _subscriptionService.GetUserSubscription()).Id
                };

                // get platform to validate token
                var resultToken = _checker.CheckAndResult(TokenValidatorStates.ValidatePlatformToken, token);
                if (resultToken.Success == false) throw new Exception(resultToken.Message);
                var dataTokenRequest = (DataTokenRequest?)resultToken.Result;

                if (isSpaceRequired)
                {
                    var resultSpace = await _checker.CheckAndResultAsync(SpaceValidatorStates.IsValid, new DataFilter(dataTokenRequest.SpaceId));

                    if (resultSpace.Success == false) throw new Exception(resultSpace.Message);

                    var space = (SpaceResponseDso)resultSpace.Result!;
                    sessionData.Space = new { space.Id, space.Name };
                }

                var modelAiId = services[0].ModelAiId;
                var result = await _checker.CheckAndResultAsync(ModelValidatorStates.HasService, modelAiId);
                if (result.Success == false) throw new Exception(result.Message);


                //var modelAi = await _modelAiRepository.GetOneByAsync([new FilterCondition("Id", services[0].ModelAiId)], new ParamOptions(["ModelGateway"]));
                var modelAi = _mapper.Map<ModelAiResponseDso>(result.Result);

                var modelCore = modelAi.ModelGateway;
                //if (modelCore == null) throw new Exception("This model ai not belong to model gateway.");


                AuthorizationSessionResponseDso session = await GetOrCreateSession(servicesIds, dataTokenRequest.AuthorizationType, dataTokenRequest.Expires, modelAiId);
                sessionData.SessionId = session.Id;
                claims.AddRange([new Claim(ClaimTypes2.SessionToken, session.SessionToken),
                    new Claim(ClaimTypes2.ApiUrl, GetApiUrl()!),
                    new Claim(ClaimTypes2.WebToken, dataTokenRequest.Token),
                    new Claim(ClaimTypes2.Data,JsonSerializer.Serialize(sessionData))]);

                var encrptedToken = _tokenService.GenerateToken(claims, modelCore.Token, session.EndTime);

                string urlCore = $"{modelCore.Url}";
                if (services.Count == 1) urlCore += $"/{services[0].AbsolutePath}";
                //string urlCore = $"{modelCore.Url}/{services.AbsolutePath}";
                return new AuthorizationSessionInfoVM()
                {
                    SessionToken = encrptedToken,
                    URLCore = urlCore
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task<AuthorizationSessionResponseDso> GetOrCreateSession(List<string> servicesIds, string type, DateTime? expire, string modelAiId)
        {
            var resultSession = await _authorizationsessionService.GetSessionByServices(_userClaims.UserId, servicesIds, type);
            var session = resultSession.ValueOrDefault;
            //DateTime endTime = DateTime.UtcNow.AddDays(30);
            if (resultSession.IsFailed || !_checker.Check(TokenValidatorStates.CheckSessionToken, session.SessionToken))
            {
                expire ??= DateTime.UtcNow.AddDays(30);
                string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

                var newSession = new AuthorizationSessionRequestDso
                {
                    UserId = _userClaims.UserId,
                    EndTime = expire,
                    SessionToken = _tokenService.GenerateTemporary(expires: expire),
                    AuthorizationType = type,
                    IpAddress = ipAddress,
                    DeviceInfo = "",
                    ServicesIds = JsonSerializer.Serialize(servicesIds),
                };
                session = await _authorizationsessionService.CreateAsync(newSession);
            }
            else if (!session.IsActive)
            {
                _logger.LogError("Session is not active for Id:{id}", session.Id);
                throw new Exception($"Your current session has been suspended for ({session.Id}).", new Exception("Contact with us for more information."));
            }

            return session;
        }

        private string? GetApiUrl()
        {
            return _linkGenerator.GetUriByAction(
                action: nameof(Validate),
                controller: "AuthorizationSession",
                values: null,
                scheme: Request.Scheme,
                host: Request.Host);
        }

        private DataTokenRequest ValidateWebToken(string token)
        {
            try
            {
                //var modelGateway = await _modelGatewayRepository.GetWebAsync();
                var webToken = _appSettings.Value.Jwt.WebSecret;
                var result = _tokenService.ValidateToken(token, webToken);
                if (result.IsFailed) throw new Exception(result.Errors.FirstOrDefault().Message);

                var claims = result.Value;
                var dataTokenRequest = JsonSerializer.Deserialize<DataTokenRequest>(claims.FindFirstValue("Data"));
                dataTokenRequest!.Token = webToken;
                return dataTokenRequest;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        private class SessionData
        {
            public string? SessionId { get; set; }
            public string? SubscriptionId { get; set; }
            public object? Services { get; set; }
            public object? Space { get; set; }
        }
    }
}