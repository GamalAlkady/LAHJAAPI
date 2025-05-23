using AutoGenerator.Helper;
using AutoMapper;
using LAHJAAPI.Exceptions;
using LAHJAAPI.Utilities;
using LAHJAAPI.V1.Validators;
using LAHJAAPI.V1.Validators.Conditions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using V1.BPR.Layers.Base;
using V1.DyModels.Dso.Requests;
using V1.DyModels.Dso.Responses;
using V1.DyModels.VMs;
using V1.Services.Services;

namespace LAHJAAPI.V1.Controllers.Api
{
    ////[ApiExplorerSettings(GroupName = "User")]
    [Route("api/v1/user/[controller]")]
    [ApiController]
    public class AuthorizationSessionController : BaseBPRControllerLayer<AuthorizationSessionRequestDso, AuthorizationSessionResponseDso, AuthorizationSessionCreateVM, AuthorizationSessionOutputVM, AuthorizationSessionUpdateVM, AuthorizationSessionInfoVM, AuthorizationSessionDeleteVM, AuthorizationSessionFilterVM>
    {
        private readonly IUseAuthorizationSessionService _authorizationsessionService;
        private readonly IConditionChecker _checker;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        public AuthorizationSessionController(IUseAuthorizationSessionService authorizationsessionService,
            IConditionChecker checker,
            IMapper mapper, ILoggerFactory logger) : base(mapper, logger, authorizationsessionService)
        {
            _authorizationsessionService = authorizationsessionService;
            _checker = checker;
            _mapper = mapper;
            _logger = logger.CreateLogger(typeof(AuthorizationSessionController).FullName);
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

        //TODO: endpoint for service createspace
        //TODO: endpoint for service dashboard
        // Create a new AuthorizationSession.
        [ServiceFilter(typeof(SubscriptionCheckFilter))]
        [HttpPost(Name = "CreateAuthorizationSession")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public override async Task<ActionResult<AuthorizationSessionOutputVM>> CreateAsync([FromBody] AuthorizationSessionCreateVM model)
        {
            try
            {
                _logger.LogInformation("Creating new AuthorizationSession with data: {@model}", model);

                var session = await _authorizationsessionService.CreateSession(
                    new DataAuthSession
                    {
                        Token = model.Token,
                        SpaceId = model.SpaceId,
                        ServicesIds = model.ServicesIds
                    });
                var vm = _mapper.Map<AuthorizationSessionOutputVM>(session);
                vm.StartTime = null;
                vm.IsActive = null;
                return Ok(vm);
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


        //[ServiceFilter(typeof(SubscriptionCheckFilter))]
        //[EndpointSummary("Create authorization session for list services")]
        //[HttpPost("CreateForListServices", Name = "CreateForListServices")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //public async Task<ActionResult<AuthorizationSessionOutputVM>> CreateForListServices(CreateAuthorizationForListServices model)
        //{
        //    try
        //    {
        //        _logger.LogInformation("Creating new AuthorizationSession for list services with data: {@model}", model);

        //        var session = await _authorizationsessionService.CreateSession(new DataAuthSession
        //        {
        //            Token = model.Token,
        //            ServicesIds = model.ServicesIds,
        //            SpaceId = model.SpaceId
        //        });
        //        var vm = _mapper.Map<AuthorizationSessionOutputVM>(session);
        //        vm.StartTime = null;
        //        vm.IsActive = null;
        //        return Ok(vm);
        //    }
        //    catch (ProblemDetailsException ex)
        //    {
        //        _logger.LogError(ex, "Error while creating a new AuthorizationSession for list services");
        //        return BadRequest(HandleResult.Problem(ex.Problem));
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error while creating a new AuthorizationSession for list services");
        //        return BadRequest(HandleResult.Problem(ex));
        //    }
        //}


        [ServiceFilter(typeof(SubscriptionCheckFilter))]
        [EndpointSummary("Create authorization session for all services")]
        [HttpPost("CreateForMyAsignedServices", Name = "CreateForMyAsignedServices")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AuthorizationSessionOutputVM>> CreateForMyAsignedServices(CreateAuthorizationForServices model)
        {
            try
            {
                _logger.LogInformation("Creating new AuthorizationSession for all services with data: {@model}", model);

                var session = await _authorizationsessionService.CreateForMyAsignedServices(new DataAuthSession
                {
                    Token = model.Token,
                    SpaceId = model.SpaceId,
                    Except = model.Except,
                });

                var vm = _mapper.Map<AuthorizationSessionOutputVM>(session);
                vm.StartTime = null;
                vm.IsActive = null;
                return Ok(vm);
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
        public async Task<ActionResult<AuthorizationSessionOutputVM>> CreateGeneralSession(GeneralAuthSessionCreateVM model)
        {
            try
            {
                var session = await _authorizationsessionService.PrepareSession(new DataAuthSession
                {
                    Token = model.Token,
                    ServicesIds = model.ServicesIds,
                });

                return Ok(new AuthorizationSessionOutputVM { SessionToken = session.SessionToken, URLCore = session.URLCore });
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