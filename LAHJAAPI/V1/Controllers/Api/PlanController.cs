using AutoGenerator.Helper;
using AutoGenerator.Helper.Translation;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Quartz.Util;
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
    public class PlanController : BaseBPRControllerLayer<PlanRequestDso, PlanResponseDso, PlanCreateVM, PlanOutputVM, PlanUpdateVM, PlanInfoVM, PlanDeleteVM, PlanFilterVM>
    {
        private readonly IUsePlanService _planService;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public PlanController(
            IUsePlanService planService,
            IMapper mapper,
            ILoggerFactory logger) : base(mapper, logger, planService)
        {
            _planService = planService;
            _mapper = mapper;
            _logger = logger.CreateLogger(typeof(PlanController).FullName);
        }



        // Get active Plans
        [HttpGet("ActivePlans", Name = "GetActivePlans")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<PlanOutputVM>>> GetActive(string? lg = "en")
        {
            try
            {
                _logger.LogInformation("Fetching active Plans...");
                var result = await _planService.GetAllByAsync([new FilterCondition("active", true)]);

                if (lg.IsNullOrWhiteSpace())
                    return Ok(_mapper.Map<List<PlanOutputVM>>(result.Data));

                return Ok(_mapper.Map<List<PlanOutputVM>>(result.Data, opt => opt.Items[HelperTranslation.KEYLG] = lg));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching active Plans");
                return StatusCode(500, HandleResult.Problem(ex));
            }
        }




        // Set new Plan
        [HttpPost("SetPlan", Name = "CreateNewPlan")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PlanOutputVM>> SetPlan([FromBody] PlanSetVM model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state in SetPlan: {ModelState}", ModelState);
                return BadRequest(ModelState);
            }

            try
            {
                _logger.LogInformation("Setting new Plan with data: {@model}", model);

                var createdEntity = await _planService.SetPlanAsync(new PlanRequest2Dso { Id = model.Id });
                var createdItem = _mapper.Map<PlanOutputVM>(createdEntity);
                return Ok(createdItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while setting new Plan");
                return StatusCode(500, HandleResult.Problem(ex));
            }
        }



    }
}