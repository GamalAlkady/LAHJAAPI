using AutoMapper;
using LAHJAAPI.V1.Validators.Conditions;
using Microsoft.AspNetCore.Mvc;
using V1.BPR.Layers.Base;
using V1.DyModels.Dso.Requests;
using V1.DyModels.Dso.Responses;
using V1.DyModels.VMs;
using V1.Services.Services;

namespace LAHJAAPI.V1.Controllers.Api
{
    //[ApiExplorerSettings(GroupName = "User")]
    [Route("api/v1/user/[controller]")]
    [ApiController]
    public class ApplicationUserController : BaseBPRControllerLayer<ApplicationUserRequestDso, ApplicationUserResponseDso, ApplicationUserCreateVM, ApplicationUserOutputVM, ApplicationUserUpdateVM, ApplicationUserInfoVM, ApplicationUserDeleteVM, ApplicationUserFilterVM>
    {
        private readonly IUseApplicationUserService _applicationuserService;
        private readonly IConditionChecker _checker;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        public ApplicationUserController(
            IUseApplicationUserService applicationuserService,
            IConditionChecker checker,
            IMapper mapper,
            ILoggerFactory logger) : base(mapper, logger, applicationuserService)
        {
            _applicationuserService = applicationuserService;
            _checker = checker;
            _mapper = mapper;
            _logger = logger.CreateLogger(typeof(ApplicationUserController).FullName);
            //_logger.LogWarning("User:" + User);
        }

        // Get all ApplicationUsers.
        //[HttpGet(Name = "GetApplicationUsers")]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        //public async Task<ActionResult<IEnumerable<ApplicationUserOutputVM>>> GetAll()
        //{
        //    try
        //    {
        //        _logger.LogInformation("Fetching all ApplicationUsers...");
        //        var result = await _applicationuserService.GetAllAsync();
        //        var items = _mapper.Map<List<ApplicationUserOutputVM>>(result);
        //        return Ok(items);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error while fetching all ApplicationUsers");
        //        return StatusCode(500, "Internal Server Error");
        //    }
        //}

        //// Get a ApplicationUser by ID.
        //[HttpGet("{id}", Name = "GetApplicationUser")]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        //public async Task<ActionResult<ApplicationUserOutputVM>> GetById(string id, string? lg)
        //{

        //    try
        //    {
        //        _logger.LogInformation("Fetching ApplicationUser with ID: {id}", id);
        //        var entity = await _applicationuserService.GetByIdAsync(id);
        //        if (entity == null)
        //        {
        //            _logger.LogWarning("ApplicationUser not found with ID: {id}", id);
        //            return NotFound();
        //        }
        //        if (string.IsNullOrWhiteSpace(lg))
        //            return Ok(_mapper.Map<ApplicationUserOutputVM>(entity));

        //        return Ok(_mapper.Map<ApplicationUserOutputVM>(entity, opts => opts.Items[HelperTranslation.KEYLG] = lg));
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error while fetching ApplicationUser with ID: {id}", id);
        //        return StatusCode(500, "Internal Server Error");
        //    }
        //}



        [HttpPost("AssignService", Name = "AssignService")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<string>> AssignService(AssignServiceRequestVM assignService)
        {
            try
            {
                _logger.LogInformation("Assigning service to user with ID: {id}", assignService.ServiceId);
                await _applicationuserService.AssignService(assignService.ServiceId);
                _logger.LogInformation("Service successfully assigned to user with ID: {id}", assignService.ServiceId);
                return Ok(HandleResult.Text("Service successfully assigned."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while assigning service to user with ID: {id}", assignService.ServiceId);
                return BadRequest(HandleResult.Problem(ex));
            }
        }

        [HttpPost("AssignModelAi", Name = "AssignModelAi")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<string>> AssignModelAi(AssignModelRequestVM requestVM)
        {
            try
            {
                _logger.LogInformation("Assigning Model AI to user with ID: {id}", requestVM.ModelAiId);
                await _applicationuserService.AssignModelAi(requestVM.ModelAiId);
                _logger.LogInformation("Model AI successfully assigned to user with ID: {id}", requestVM.ModelAiId);
                return Ok(HandleResult.Text("Model AI successfully assigned."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while assigning Model AI to user with ID: {id}", requestVM.ModelAiId);
                return BadRequest(HandleResult.Problem(ex));
            }
        }

    }
}