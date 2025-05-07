using AutoGenerator;
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
    [ApiExplorerSettings(GroupName = "User")]
    [Route("api/v1/user/[controller]")]
    [ApiController]
    public class ModelAiController : ControllerBase
    {
        private readonly IUseModelAiService _modelAiService;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public ModelAiController(
            IUseModelAiService modelAiService,
            IMapper mapper,
            ILoggerFactory logger)
        {
            _modelAiService = modelAiService;
            _mapper = mapper;
            _logger = logger.CreateLogger(typeof(ModelAiController).FullName);
        }

        // Get all ModelAis
        [HttpGet(Name = "GetModelAis")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<ModelAiOutputVM>>> GetAll(string? lg)
        {
            try
            {
                _logger.LogInformation("Fetching all ModelAis...");
                var result = await _modelAiService.GetAllAsync();

                if (lg.IsNullOrWhiteSpace())
                    return Ok(_mapper.Map<List<ModelAiOutputVM>>(result));

                return Ok(_mapper.Map<List<ModelAiOutputVM>>(result, opt => opt.Items[HelperTranslation.KEYLG] = lg));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching all ModelAis");
                return StatusCode(500, HandelResult.Problem(ex));
            }
        }


        // Get ModelAi by ID
        [HttpGet("{id}", Name = "GetModelAi")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ModelAiOutputVM>> GetById(string id, string? lg)
        {
            try
            {
                _logger.LogInformation("Fetching ModelAi with ID: {id}", id);
                var entity = await _modelAiService.GetByIdAsync(id);

                if (entity == null)
                {
                    _logger.LogWarning("ModelAi not found with ID: {id}", id);
                    return NotFound();
                }

                if (lg.IsNullOrWhiteSpace())
                    return Ok(_mapper.Map<ModelAiOutputVM>(entity));

                var item = _mapper.Map<ModelAiOutputVM>(entity, opts => opts.Items.Add(HelperTranslation.KEYLG, lg));
                return Ok(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching ModelAi with ID: {id}", id);
                return StatusCode(500, HandelResult.Problem(ex));
            }
        }

        [EndpointSummary("Get Models By Type")]
        [HttpGet("ByType/{type}", Name = "GetModelsByType")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ModelAiOutputVM>>> GetModelsByType(string type)
        {
            var items = await _modelAiService.GetAllByAsync([new FilterCondition("Type", type)]);
            var result = _mapper.Map<IEnumerable<ModelAiOutputVM>>(items);
            return Ok(result);
        }

        [EndpointSummary("Get Categories By Type")]
        [HttpGet("CategoriesByType/{type}", Name = "GetCategoriesByType")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<string>>> GetCategoriesByType(string type)
        {
            var items = await _modelAiService.GetAllByAsync([new FilterCondition("Type", type)]);
            if (items.TotalRecords == 0) return NoContent();
            var result = items.Data.Select(s => s.Category);
            return Ok(result);
        }
        [EndpointSummary("Get Languages By Type And Category")]
        [HttpGet("LanguagesBy", Name = "GetLanguagesBy")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<string>>> GetLanguagesBy(string type, string category)
        {
            var items = await _modelAiService.GetAllByAsync([
                new FilterCondition("Type", type),
                new FilterCondition("Category", type),
            ]);

            if (items.TotalRecords == 0) return NoContent();
            var result = items.Data.Select(s => s.Language).Distinct();
            return Ok(result);
        }

        [EndpointSummary("Get Models By Category")]
        [HttpGet("category/{category}", Name = "GetModelsByCategory")]

        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ModelAiOutputVM>>> GetByCategoryAsync(string category)
        {
            var items = await _modelAiService.GetAllByAsync([new FilterCondition("Category", category)]);
            if (items.TotalRecords == 0) return NoContent();
            var result = _mapper.Map<IEnumerable<ModelAiOutputVM>>(items);
            return Ok(result);
        }


        [EndpointSummary("Filter models")]
        [HttpPost("GetFilterModel", Name = "FilterMaodelAi")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResponse<ModelAiOutputVM>>> GetFilterModel([FromBody] ModelAiFilterVM serchModelAI)
        {

            var response = await _modelAiService.FilterMaodelAi(serchModelAI);
            var result = response.ToResponse(_mapper.Map<IEnumerable<ModelAiOutputVM>>(response.Data));

            return Ok(result);
        }

        // Get ModelAis by language
        [HttpGet("ByLanguage/All", Name = "GetModelAisByLanguage")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<ModelAiOutputVM>>> GetByLanguage(string lg)
        {
            try
            {
                _logger.LogInformation("Fetching ModelAis with language: {lg}", lg);
                var result = await _modelAiService.GetAllAsync();

                if (result == null || !result.Any())
                {
                    _logger.LogWarning("No ModelAis found");
                    return NotFound();
                }

                var items = _mapper.Map<List<ModelAiOutputVM>>(result, opt => opt.Items.Add(HelperTranslation.KEYLG, lg));
                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching ModelAis with language: {lg}", lg);
                return StatusCode(500, HandelResult.Problem(ex));
            }
        }

        // Create new ModelAi
        [HttpPost(Name = "CreateModelAi")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ModelAiOutputVM>> Create([FromBody] ModelAiCreateVM model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state in Create: {ModelState}", ModelState);
                return BadRequest(ModelState);
            }

            try
            {
                _logger.LogInformation("Creating new ModelAi with data: {@model}", model);
                var item = _mapper.Map<ModelAiRequestDso>(model);
                var createdEntity = await _modelAiService.CreateAsync(item);
                var createdItem = _mapper.Map<ModelAiOutputVM>(createdEntity);
                return Ok(createdItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating new ModelAi");
                return StatusCode(500, HandelResult.Problem(ex));
            }
        }

        // Create multiple ModelAis
        [HttpPost("createRange")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<ModelAiOutputVM>>> CreateRange([FromBody] IEnumerable<ModelAiCreateVM> models)
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
                _logger.LogInformation("Creating {count} ModelAis", models.Count());
                var items = _mapper.Map<List<ModelAiRequestDso>>(models);
                var createdEntities = await _modelAiService.CreateRangeAsync(items);
                var createdItems = _mapper.Map<List<ModelAiOutputVM>>(createdEntities);
                return Ok(createdItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating multiple ModelAis");
                return StatusCode(500, HandelResult.Problem(ex));
            }
        }

        // Update ModelAi
        [HttpPut("{id}", Name = "UpdateModelAi")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ModelAiOutputVM>> Update(string id, [FromBody] ModelAiUpdateVM model)
        {
            try
            {
                _logger.LogInformation("Updating ModelAi with ID: {id}", id);
                var existingItem = await _modelAiService.GetByIdAsync(id);

                if (existingItem == null)
                {
                    _logger.LogWarning("ModelAi not found with ID: {id}", id);
                    return NotFound();
                }

                var item = _mapper.Map<ModelAiRequestDso>(model);
                item.Id = id;
                item.ModelGatewayId = existingItem.ModelGatewayId;
                var updatedEntity = await _modelAiService.UpdateAsync(item);

                if (updatedEntity == null)
                {
                    _logger.LogWarning("Failed to update ModelAi with ID: {id}", id);
                    return NotFound();
                }

                var updatedItem = _mapper.Map<ModelAiOutputVM>(updatedEntity);
                return Ok(updatedItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating ModelAi with ID: {id}", id);
                return StatusCode(500, HandelResult.Problem(ex));
            }
        }

        // Delete ModelAi
        [HttpDelete("{id}", Name = "DeleteModelAi")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                _logger.LogInformation("Deleting ModelAi with ID: {id}", id);

                if (!await _modelAiService.ExistsAsync(id))
                {
                    _logger.LogWarning("ModelAi not found with ID: {id}", id);
                    return NotFound();
                }

                await _modelAiService.DeleteAsync(id);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting ModelAi with ID: {id}", id);
                return StatusCode(500, HandelResult.Problem(ex));
            }
        }

        // Get ModelAis count
        [HttpGet("Count", Name = "CountModelAis")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<int>> Count()
        {
            try
            {
                _logger.LogInformation("Counting ModelAis...");
                var count = await _modelAiService.CountAsync();
                return Ok(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while counting ModelAis");
                return StatusCode(500, HandelResult.Problem(ex));
            }
        }
    }
}