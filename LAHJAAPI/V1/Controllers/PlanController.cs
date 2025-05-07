using AutoGenerator;
using AutoGenerator.Helper;
using AutoGenerator.Helper.Translation;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quartz.Util;
using V1.DyModels.VMs;
using V1.Services.Services;

namespace LAHJAAPI.V1.Controllers
{
    //[ApiExplorerSettings(GroupName = "V1")]
    [AllowAnonymous]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class PlanController : ControllerBase
    {
        private readonly IUsePlanService _planService;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public PlanController(
            IUsePlanService planService,
            IMapper mapper,
            ILoggerFactory logger)
        {
            _planService = planService;
            _mapper = mapper;
            _logger = logger.CreateLogger(typeof(PlanController).FullName);
        }

        // Get all Plans
        [HttpGet(Name = "GetAllPlans")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PagedResponse<IEnumerable<PlanOutputVM>>>> GetAll(string? lg)
        {
            try
            {
                _logger.LogInformation("Fetching all Plans...");
                var result = await _planService.GetAllByAsync([new FilterCondition("Active", true)], new ParamOptions(["PlanFeatures"]));
                //return Ok(result);
                if (lg.IsNullOrWhiteSpace())
                    return Ok(_mapper.Map<List<PlanOutputVM>>(result.Data));

                return Ok(_mapper.Map<List<PlanOutputVM>>(result.Data, opt => opt.Items[HelperTranslation.KEYLG] = lg));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching all Plans");
                return StatusCode(500, HandelResult.Problem(ex));
            }
        }


        // Get Plan by ID
        [HttpGet("{id}", Name = "GetPlanById")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PlanOutputVM>> GetById(string id, string? lg = "en")
        {
            try
            {
                _logger.LogInformation("Fetching Plan with ID: {id}", id);
                var entity = await _planService.GetByIdAsync(id);

                if (entity == null)
                {
                    _logger.LogWarning("Plan not found with ID: {id}", id);
                    return NotFound();
                }

                if (lg.IsNullOrWhiteSpace())
                    return Ok(_mapper.Map<PlanOutputVM>(entity));

                var item = _mapper.Map<PlanOutputVM>(entity, opts => opts.Items.Add(HelperTranslation.KEYLG, lg));
                return Ok(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching Plan with ID: {id}", id);
                return StatusCode(500, HandelResult.Problem(ex));
            }
        }

        // Get Plans count
        [HttpGet("Count", Name = "CountAllPlans")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<int>> Count()
        {
            try
            {
                _logger.LogInformation("Counting Plans...");
                var count = await _planService.CountAsync();
                return Ok(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while counting Plans");
                return StatusCode(500, HandelResult.Problem(ex));
            }
        }
    }
}