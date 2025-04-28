using AutoGenerator.Helper;
using AutoGenerator.Helper.Translation;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Quartz.Util;
using V1.DyModels.Dso.Requests;
using V1.DyModels.VMs;
using V1.Services.Services;

namespace LAHJAAPI.V1.Controllers.Api
{
    //[ApiExplorerSettings(GroupName = "V1")]
    [Route("api/v1/user/[controller]")]
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
        [HttpGet(Name = "GetPlans")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<PlanOutputVM>>> GetAll(string? lg)
        {
            try
            {
                _logger.LogInformation("Fetching all Plans...");
                var result = await _planService.GetAllAsync();

                if (lg.IsNullOrWhiteSpace())
                    return Ok(_mapper.Map<List<PlanOutputVM>>(result));

                return Ok(_mapper.Map<List<PlanOutputVM>>(result, opt => opt.Items[HelperTranslation.KEYLG] = lg));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching all Plans");
                return StatusCode(500, HandelErrors.Problem(ex));
            }
        }

        // Get active Plans
        [HttpGet("Active", Name = "GetActivePlans")]
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
                return StatusCode(500, HandelErrors.Problem(ex));
            }
        }

        // Get Plan by ID
        [HttpGet("{id}", Name = "GetPlan")]
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
                return StatusCode(500, HandelErrors.Problem(ex));
            }
        }

        // Get Plan by language filter
        [HttpGet("ByLanguage", Name = "GetPlanByLanguage")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PlanOutputVM>> GetByLanguage([FromQuery] PlanFilterVM model)
        {
            try
            {
                _logger.LogInformation("Fetching Plan with ID: {id} and language: {lg}", model.Id, model.Lg);
                var entity = await _planService.GetByIdAsync(model.Id);

                if (entity == null)
                {
                    _logger.LogWarning("Plan not found with ID: {id}", model.Id);
                    return NotFound();
                }

                var item = _mapper.Map<PlanOutputVM>(entity, opt => opt.Items.Add(HelperTranslation.KEYLG, model.Lg));
                return Ok(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching Plan with ID: {id}", model.Id);
                return StatusCode(500, HandelErrors.Problem(ex));
            }
        }

        // Get Plans by language
        [HttpGet("ByLanguage/All", Name = "GetPlansByLanguage")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<PlanOutputVM>>> GetByLanguage(string lg = "en")
        {
            try
            {
                _logger.LogInformation("Fetching Plans with language: {lg}", lg);
                var result = await _planService.GetAllAsync();

                if (result == null || !result.Any())
                {
                    _logger.LogWarning("No Plans found");
                    return NotFound();
                }

                var items = _mapper.Map<List<PlanOutputVM>>(result, opt => opt.Items.Add(HelperTranslation.KEYLG, lg));
                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching Plans with language: {lg}", lg);
                return StatusCode(500, HandelErrors.Problem(ex));
            }
        }

        // Create new Plan
        [HttpPost(Name = "CreatePlan")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PlanOutputVM>> Create([FromBody] PlanCreateVM model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state in Create: {ModelState}", ModelState);
                return BadRequest(ModelState);
            }

            try
            {
                _logger.LogInformation("Creating new Plan with data: {@model}", model);
                var item = _mapper.Map<PlanRequestDso>(model);
                var createdEntity = await _planService.CreateAsync(item);
                var createdItem = _mapper.Map<PlanOutputVM>(createdEntity);
                return Ok(createdItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating new Plan");
                return StatusCode(500, HandelErrors.Problem(ex));
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
                var item = new PlanRequestDso
                {
                    Id = model.Id,
                    ProductId = model.ProductId
                };
                var createdEntity = await _planService.SetPlanAsync(item);
                var createdItem = _mapper.Map<PlanOutputVM>(createdEntity);
                return Ok(createdItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while setting new Plan");
                return StatusCode(500, HandelErrors.Problem(ex));
            }
        }

        // Create multiple Plans
        [HttpPost("createRange")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<PlanOutputVM>>> CreateRange([FromBody] IEnumerable<PlanCreateVM> models)
        {
            if (models == null || !models.Any())
            {
                _logger.LogWarning("Empty data in CreateRange");
                return BadRequest("Data is required");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state in CreateRange: {ModelState}", ModelState);
                return BadRequest(ModelState);
            }

            try
            {
                _logger.LogInformation("Creating {count} Plans", models.Count());
                var items = _mapper.Map<List<PlanRequestDso>>(models);
                var createdEntities = await _planService.CreateRangeAsync(items);
                var createdItems = _mapper.Map<List<PlanOutputVM>>(createdEntities);
                return Ok(createdItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating multiple Plans");
                return StatusCode(500, HandelErrors.Problem(ex));
            }
        }

        // Update Plan
        [HttpPut("{id}", Name = "UpdatePlan")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PlanOutputVM>> Update(string id, [FromBody] PlanUpdateVM model)
        {
            try
            {
                _logger.LogInformation("Updating Plan with ID: {id}", id);
                var existingItem = await _planService.GetByIdAsync(id);

                if (existingItem == null)
                {
                    _logger.LogWarning("Plan not found with ID: {id}", id);
                    return NotFound();
                }

                var item = _mapper.Map<PlanRequestDso>(model);
                item.Id = id;
                var updatedEntity = await _planService.UpdateAsync(item);

                if (updatedEntity == null)
                {
                    _logger.LogWarning("Failed to update Plan with ID: {id}", id);
                    return NotFound();
                }

                var updatedItem = _mapper.Map<PlanOutputVM>(updatedEntity);
                return Ok(updatedItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating Plan with ID: {id}", id);
                return StatusCode(500, HandelErrors.Problem(ex));
            }
        }

        // Delete Plan
        [HttpDelete("{id}", Name = "DeletePlan")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                _logger.LogInformation("Deleting Plan with ID: {id}", id);

                if (!await _planService.ExistsAsync(id))
                {
                    _logger.LogWarning("Plan not found with ID: {id}", id);
                    return NotFound();
                }

                await _planService.DeleteAsync(id);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting Plan with ID: {id}", id);
                return StatusCode(500, HandelErrors.Problem(ex));
            }
        }

        // Get Plans count
        [HttpGet("Count", Name = "CountPlans")]
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
                return StatusCode(500, HandelErrors.Problem(ex));
            }
        }
    }
}