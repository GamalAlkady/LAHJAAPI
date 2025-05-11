using AutoGenerator.Helper;
using AutoGenerator.Helper.Translation;
using AutoMapper;
using LAHJAAPI.Exceptions;
using LAHJAAPI.Utilities;
using LAHJAAPI.V1.Validators;
using LAHJAAPI.V1.Validators.Conditions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using V1.DyModels.Dso.Requests;
using V1.DyModels.VMs;
using V1.Services.Services;

namespace LAHJAAPI.V1.Controllers.Api
{
    ////[ApiExplorerSettings(GroupName = "User")]
    [Route("api/v1/user/[controller]")]
    [ApiController]
    public class AuthorizationSessionController : ControllerBase
    {
        private readonly IUseAuthorizationSessionService _authorizationsessionService;
        private readonly IConditionChecker _checker;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        public AuthorizationSessionController(IUseAuthorizationSessionService authorizationsessionService,
            IConditionChecker checker,
            IMapper mapper, ILoggerFactory logger)
        {
            _authorizationsessionService = authorizationsessionService;
            _checker = checker;
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
                return StatusCode(500, ex.Message);
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
                return StatusCode(500, ex.Message);
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
                return StatusCode(500, ex.Message);
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

                var token = _checker.Injector.TokenService.GenerateToken([
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
                return BadRequest(HandleResult.Problem(ex));
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

                var session = await _authorizationsessionService.CreateSecretSession(
                    new DataAuthSession
                    {
                        Token = model.Token,
                        SpaceId = model.SpaceId,
                        ServicesIds = [model.ServiceId]
                    });
                return Ok(_mapper.Map<AuthorizationSessionOutputVM>(session));
            }
            catch (ProblemDetailsException ex)
            {
                _logger.LogError(ex, "Error while creating a new AuthorizationSession");
                return BadRequest(HandleResult.Problem(ex.Problem));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating a new AuthorizationSession");
                //return StatusCode(500, "Internal Server Error");
                return BadRequest(HandleResult.Problem(ex));
            }
        }


        [EndpointSummary("Create session for dashboard")]
        [HttpPost("CreateForDashboard")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AuthorizationSessionInfoVM>> CreateForDashboard(CreateAuthorizationForDashboard model)
        {
            try
            {
                _logger.LogInformation("Creating new AuthorizationSession for dashboard with data: {@model}", model);
                var session = await _authorizationsessionService.CreateForDashboard(new DataAuthSession { Token = model.Token });

                return Ok(_mapper.Map<AuthorizationSessionOutputVM>(session));
            }
            catch (ProblemDetailsException ex)
            {
                _logger.LogError(ex, "Error while creating a new AuthorizationSession for dashboard");
                return BadRequest(HandleResult.Problem(ex.Problem));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating a new AuthorizationSession for dashboard");
                return BadRequest(HandleResult.Problem(ex));
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


                var session = await _authorizationsessionService.CreateSecretSession(new DataAuthSession
                {
                    Token = model.Token,
                    ServicesIds = model.ServicesIds,
                    SpaceId = model.SpaceId
                });
                return Ok(_mapper.Map<AuthorizationSessionOutputVM>(session));
            }
            catch (ProblemDetailsException ex)
            {
                _logger.LogError(ex, "Error while creating a new AuthorizationSession for list services");
                return BadRequest(HandleResult.Problem(ex.Problem));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating a new AuthorizationSession for list services");
                return BadRequest(HandleResult.Problem(ex));
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

                var session = await _authorizationsessionService.CreateForAllServices(new DataAuthSession
                {
                    Token = model.Token,
                    SpaceId = model.SpaceId
                });

                return Ok(_mapper.Map<AuthorizationSessionOutputVM>(session));
            }
            catch (ProblemDetailsException ex)
            {
                _logger.LogError(ex, "Error while creating a new AuthorizationSession for all services");
                return BadRequest(HandleResult.Problem(ex.Problem));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating a new AuthorizationSession for all services");
                return BadRequest(HandleResult.Problem(ex));
            }
        }


        [ServiceFilter(typeof(SubscriptionCheckFilter))]
        [EndpointSummary("Create authorization session for list services")]
        [HttpPost("CreateGeneralSession")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AuthorizationSessionInfoVM>> CreateGeneralSession(CreateAuthorizationForListServices model)
        {
            try
            {
                var session = await _authorizationsessionService.CreateGeneralSession(new DataAuthSession
                {
                    Token = model.Token,
                    ServicesIds = model.ServicesIds,
                });


                return Ok(_mapper.Map<AuthorizationSessionInfoVM>(session));

            }
            catch (ProblemDetailsException ex)
            {
                _logger.LogError(ex, "Error while creating a new AuthorizationSession for list services");
                return BadRequest(HandleResult.Problem(ex.Problem));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating a new AuthorizationSession for list services");
                return BadRequest(HandleResult.Problem(ex));
            }
        }


        // Delete a AuthorizationSession.
        [HttpDelete("{id}", Name = "DeleteAuthorizationSession")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(string id)
        {

            try
            {
                _logger.LogInformation("Deleting AuthorizationSession with ID: {id}", id);
                await _authorizationsessionService.DeleteAsync(id);
                return Ok(HandleResult.Text("Deleted successfully."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting AuthorizationSession with ID: {id}", id);
                return BadRequest(HandleResult.Problem(ex));
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
                return BadRequest(HandleResult.Problem(ex));
            }
        }


        [AllowAnonymous]
        [HttpPost("SimulationPlatForm", Name = "SimulationPlatForm")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public ActionResult<string> SimulationPlatForm(EncryptTokenRequest encryptToken)
        {
            try
            {
                _logger.LogInformation("Creating platform token with data: {@encryptToken}", encryptToken);

                var encrptedToken = _authorizationsessionService.SimulationPlatForm(encryptToken);
                return Ok(encrptedToken);
            }
            catch (ProblemDetailsException ex)
            {
                _logger.LogError(ex, "Error when create platform token.");
                return BadRequest(HandleResult.Problem(ex.Problem));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when create platform token.");
                return BadRequest(HandleResult.Problem(ex));
            }
        }

        [AllowAnonymous]
        [HttpGet("SimulationCore", Name = "SimulationCore")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public ActionResult<string> SimulationCore(string encrptedToken, string coreToken)
        {
            try
            {
                _logger.LogInformation("Creating core token with data: {@encrptedToken}", encrptedToken);
                var token = _authorizationsessionService.SimulationCore(encrptedToken, coreToken);

                return Ok(token);
            }
            catch (ProblemDetailsException ex)
            {
                _logger.LogError(ex, "Error when create core token.");
                return BadRequest(HandleResult.Problem(ex.Problem));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when create core token.");
                return BadRequest(HandleResult.Problem(ex));
            }
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

                return BadRequest(HandleResult.Problem(ex));
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
                return BadRequest(HandleResult.Problem(ex));
            }
        }

    }
}