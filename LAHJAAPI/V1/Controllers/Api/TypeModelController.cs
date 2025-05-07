using AutoGenerator.Helper;
using AutoGenerator.Helper.Translation;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
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
    public class TypeModelController : ControllerBase
    {
        private readonly IUseTypeModelService _typeModelService;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public TypeModelController(
            IUseTypeModelService typeModelService,
            IMapper mapper,
            ILoggerFactory logger)
        {
            _typeModelService = typeModelService;
            _mapper = mapper;
            _logger = logger.CreateLogger(typeof(TypeModelController).FullName);
        }

        // Get all TypeModels
        [HttpGet(Name = "GetTypeModels")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<TypeModelOutputVM>>> GetAll(string? lg = "en")
        {
            try
            {
                _logger.LogInformation("Fetching all TypeModels...");
                var result = await _typeModelService.GetAllAsync();

                if (lg.IsNullOrWhiteSpace())
                    return Ok(_mapper.Map<List<TypeModelOutputVM>>(result));

                return Ok(_mapper.Map<List<TypeModelOutputVM>>(result, opt => opt.Items[HelperTranslation.KEYLG] = lg));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching all TypeModels");
                return StatusCode(500, HandelResult.Problem(ex));
            }
        }

        // Get TypeModel by ID
        [HttpGet("{id}", Name = "GetTypeModel")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TypeModelOutputVM>> GetById(string id, string? lg = "en")
        {
            try
            {
                _logger.LogInformation("Fetching TypeModel with ID: {id}", id);
                var entity = await _typeModelService.GetByIdAsync(id);

                if (entity == null)
                {
                    _logger.LogWarning("TypeModel not found with ID: {id}", id);
                    return NotFound();
                }

                if (lg.IsNullOrWhiteSpace())
                    return Ok(_mapper.Map<TypeModelOutputVM>(entity));

                var item = _mapper.Map<TypeModelOutputVM>(entity, opts => opts.Items.Add(HelperTranslation.KEYLG, lg));
                return Ok(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching TypeModel with ID: {id}", id);
                return StatusCode(500, HandelResult.Problem(ex));
            }
        }

        // Get TypeModel by Name
        [AllowAnonymous]
        [HttpGet("ByName/{name}", Name = "GetTypeModelByName")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TypeModelOutputVM>> GetByName(string name, string? lg = null)
        {
            try
            {
                _logger.LogInformation("Fetching TypeModel with name: {name}", name);
                var entity = await _typeModelService.GetOneByAsync(
                [
                    new FilterCondition
                    {
                        PropertyName = "Name",
                        Value = name,
                        Operator = FilterOperator.Contains
                    }
                ]);

                if (entity == null)
                {
                    _logger.LogWarning("TypeModel not found with name: {name}", name);
                    return NotFound();
                }

                if (lg.IsNullOrWhiteSpace())
                    return Ok(_mapper.Map<TypeModelOutputVM>(entity));

                var item = _mapper.Map<TypeModelOutputVM>(entity, opt => opt.Items.Add(HelperTranslation.KEYLG, lg));
                return Ok(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching TypeModel with name: {name}", name);
                return StatusCode(500, HandelResult.Problem(ex));
            }
        }

        // Get active TypeModels
        [HttpGet("Active", Name = "GetActiveTypeModels")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<TypeModelOutputVM>>> GetActive(string? lg = null)
        {
            try
            {
                _logger.LogInformation("Fetching active TypeModels...");
                var result = await _typeModelService.GetAllByAsync(
                [
                    new FilterCondition
                    {
                        PropertyName = "Active",
                        Value = true
                    }
                ]);

                if (result.TotalRecords == 0)
                {
                    _logger.LogWarning("No active TypeModels found");
                    return NotFound();
                }

                if (lg.IsNullOrWhiteSpace())
                    return Ok(_mapper.Map<List<TypeModelOutputVM>>(result.Data));

                var items = _mapper.Map<List<TypeModelOutputVM>>(result.Data, opt => opt.Items.Add(HelperTranslation.KEYLG, lg));
                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching active TypeModels");
                return StatusCode(500, HandelResult.Problem(ex));
            }
        }

        // Get TypeModel by language filter
        [HttpGet("ByLanguage", Name = "GetTypeModelByLanguage")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TypeModelOutputVM>> GetByLanguage([FromQuery] TypeModelFilterVM model)
        {
            try
            {
                _logger.LogInformation("Fetching TypeModel with ID: {id} and language: {lg}", model.Id, model.Lg);
                var entity = await _typeModelService.GetByIdAsync(model.Id);

                if (entity == null)
                {
                    _logger.LogWarning("TypeModel not found with ID: {id}", model.Id);
                    return NotFound();
                }

                var item = _mapper.Map<TypeModelOutputVM>(entity, opt => opt.Items.Add(HelperTranslation.KEYLG, model.Lg));
                return Ok(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching TypeModel with ID: {id}", model.Id);
                return StatusCode(500, HandelResult.Problem(ex));
            }
        }

        // Get TypeModels by language
        [HttpGet("ByLanguage/All", Name = "GetTypeModelsByLanguage")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<TypeModelOutputVM>>> GetByLanguage(string lg = "en")
        {
            try
            {
                _logger.LogInformation("Fetching TypeModels with language: {lg}", lg);
                var result = await _typeModelService.GetAllAsync();

                if (result == null || !result.Any())
                {
                    _logger.LogWarning("No TypeModels found");
                    return NotFound();
                }

                var items = _mapper.Map<List<TypeModelOutputVM>>(result, opt => opt.Items.Add(HelperTranslation.KEYLG, lg));
                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching TypeModels with language: {lg}", lg);
                return StatusCode(500, HandelResult.Problem(ex));
            }
        }

        // Create new TypeModel
        [HttpPost(Name = "CreateTypeModel")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<TypeModelOutputVM>> Create([FromBody] TypeModelCreateVM model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state in Create: {ModelState}", ModelState);
                return BadRequest(ModelState);
            }

            try
            {
                _logger.LogInformation("Creating new TypeModel with data: {@model}", model);
                var item = _mapper.Map<TypeModelRequestDso>(model);
                var createdEntity = await _typeModelService.CreateAsync(item);
                var createdItem = _mapper.Map<TypeModelOutputVM>(createdEntity);
                return Ok(createdItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating new TypeModel");
                return StatusCode(500, HandelResult.Problem(ex));
            }
        }

        // Create multiple TypeModels
        [HttpPost("createRange")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<TypeModelOutputVM>>> CreateRange([FromBody] IEnumerable<TypeModelCreateVM> models)
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
                _logger.LogInformation("Creating {count} TypeModels", models.Count());
                var items = _mapper.Map<List<TypeModelRequestDso>>(models);
                var createdEntities = await _typeModelService.CreateRangeAsync(items);
                var createdItems = _mapper.Map<List<TypeModelOutputVM>>(createdEntities);
                return Ok(createdItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating multiple TypeModels");
                return StatusCode(500, HandelResult.Problem(ex));
            }
        }

        // Update TypeModel
        [HttpPut("{id}", Name = "UpdateTypeModel")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TypeModelOutputVM>> Update(string id, [FromBody] TypeModelUpdateVM model)
        {
            try
            {
                _logger.LogInformation("Updating TypeModel with ID: {id}", id);
                var existingItem = await _typeModelService.GetByIdAsync(id);

                if (existingItem == null)
                {
                    _logger.LogWarning("TypeModel not found with ID: {id}", id);
                    return NotFound();
                }

                var item = _mapper.Map<TypeModelRequestDso>(model);
                item.Id = id;
                var updatedEntity = await _typeModelService.UpdateAsync(item);

                if (updatedEntity == null)
                {
                    _logger.LogWarning("Failed to update TypeModel with ID: {id}", id);
                    return NotFound();
                }

                var updatedItem = _mapper.Map<TypeModelOutputVM>(updatedEntity);
                return Ok(updatedItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating TypeModel with ID: {id}", id);
                return StatusCode(500, HandelResult.Problem(ex));
            }
        }

        // Delete TypeModel
        [HttpDelete("{id}", Name = "DeleteTypeModel")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                _logger.LogInformation("Deleting TypeModel with ID: {id}", id);

                if (!await _typeModelService.ExistsAsync(id))
                {
                    _logger.LogWarning("TypeModel not found with ID: {id}", id);
                    return NotFound();
                }

                await _typeModelService.DeleteAsync(id);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting TypeModel with ID: {id}", id);
                return StatusCode(500, HandelResult.Problem(ex));
            }
        }

        // Get TypeModels count
        [HttpGet("Count", Name = "CountTypeModels")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<int>> Count()
        {
            try
            {
                _logger.LogInformation("Counting TypeModels...");
                var count = await _typeModelService.CountAsync();
                return Ok(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while counting TypeModels");
                return StatusCode(500, HandelResult.Problem(ex));
            }
        }
    }
}