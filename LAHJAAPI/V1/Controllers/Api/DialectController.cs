using AutoGenerator.Helper;
using AutoGenerator.Helper.Translation;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quartz.Util;
using V1.DyModels.Dso.Requests;
using V1.DyModels.Dso.Responses;
using V1.DyModels.VMs;
using V1.Services.Services;

namespace LAHJAAPI.V1.Controllers.Api
{
    //[ApiExplorerSettings(GroupName = "User")]
    [Route("api/v1/user/[controller]")]
    [ApiController]
    public class DialectController : ControllerBase
    {
        private readonly IUseDialectService _dialectService;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public DialectController(
            IUseDialectService dialectService,
            IMapper mapper,
            ILoggerFactory logger)
        {
            _dialectService = dialectService;
            _mapper = mapper;
            _logger = logger.CreateLogger(typeof(DialectController).FullName);
        }

        // Get all Dialects.
        [HttpGet(Name = "GetDialects")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<DialectOutputVM>>> GetAll(string? lg)
        {
            try
            {
                _logger.LogInformation("Fetching all Dialects...");
                var result = await _dialectService.GetAllAsync();

                if (lg.IsNullOrWhiteSpace())
                    return Ok(_mapper.Map<List<DialectOutputVM>>(result));

                return Ok(_mapper.Map<List<DialectOutputVM>>(result, opt => opt.Items[HelperTranslation.KEYLG] = lg));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching all Dialects");
                return StatusCode(500, "Internal Server Error");
            }
        }

        // Get a Dialect by ID.
        [HttpGet("{id}", Name = "GetDialect")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DialectOutputVM>> GetById(string id, string? lg)
        {
            try
            {
                _logger.LogInformation("Fetching Dialect with ID: {id}", id);
                var entity = await _dialectService.GetByIdAsync(id);

                if (entity == null)
                {
                    _logger.LogWarning("Dialect not found with ID: {id}", id);
                    return NotFound();
                }

                if (lg.IsNullOrWhiteSpace())
                    return Ok(_mapper.Map<DialectOutputVM>(entity));

                var item = _mapper.Map<DialectOutputVM>(entity, opts => opts.Items.Add(HelperTranslation.KEYLG, lg));
                return Ok(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching Dialect with ID: {id}", id);
                return StatusCode(500, "Internal Server Error");
            }
        }

        // Get Dialect by Language ID.
        [AllowAnonymous]
        [HttpGet("ByLanguage/{langId}", Name = "GetDialectByLanguage")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DialectOutputVM>> GetByLanguageId(string langId, string? lg)
        {
            try
            {
                _logger.LogInformation("Fetching Dialect with Language ID: {langId}", langId);
                var entity = await _dialectService.GetOneByAsync([new FilterCondition(nameof(DialectResponseDso.LanguageId), langId)]);

                if (entity == null)
                {
                    _logger.LogWarning("Dialect not found with Language ID: {langId}", langId);
                    return NotFound();
                }

                if (lg.IsNullOrWhiteSpace())
                    return Ok(_mapper.Map<DialectOutputVM>(entity));

                var item = _mapper.Map<DialectOutputVM>(entity, opt => opt.Items.Add(HelperTranslation.KEYLG, lg));
                return Ok(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching Dialect with Language ID: {langId}", langId);
                return StatusCode(500, "Internal Server Error");
            }
        }

        // Get Dialects by Language ID.
        [AllowAnonymous]
        [HttpGet("ByLanguage/All/{langId}", Name = "GetDialectsByLanguage")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<DialectOutputVM>>> GetDialectsByLanguage(string langId, string? lg)
        {
            try
            {
                _logger.LogInformation("Fetching Dialects with Language ID: {langId}", langId);
                var result = await _dialectService.GetAllByAsync([new FilterCondition(nameof(DialectResponseDso.LanguageId), langId)]);

                if (result.TotalRecords == 0)
                {
                    _logger.LogWarning("Dialects not found with Language ID: {langId}", langId);
                    return NotFound();
                }

                if (lg.IsNullOrWhiteSpace())
                    return Ok(_mapper.Map<List<DialectOutputVM>>(result.Data));

                var items = _mapper.Map<List<DialectOutputVM>>(result.Data, opt => opt.Items.Add(HelperTranslation.KEYLG, lg));
                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching Dialects with Language ID: {langId}", langId);
                return StatusCode(500, "Internal Server Error");
            }
        }

        // Create a new Dialect.
        [HttpPost(Name = "CreateDialect")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DialectOutputVM>> Create([FromBody] DialectCreateVM model)
        {
            if (model == null)
            {
                _logger.LogWarning("Dialect data is null in Create.");
                return BadRequest("Dialect data is required.");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state in Create: {ModelState}", ModelState);
                return BadRequest(ModelState);
            }

            try
            {
                _logger.LogInformation("Creating new Dialect with data: {@model}", model);
                var item = _mapper.Map<DialectRequestDso>(model);
                var createdEntity = await _dialectService.CreateAsync(item);
                if (createdEntity is null)
                {
                    _logger.LogError("Could not save Dialect with data: {model}", model);
                    return BadRequest("Could not save Dialect");
                }
                var createdItem = _mapper.Map<DialectOutputVM>(createdEntity);
                return Ok(createdItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating a new Dialect");
                return StatusCode(500, "Internal Server Error");
            }
        }

        // Create multiple Dialects.
        [HttpPost("createRange")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<DialectOutputVM>>> CreateRange([FromBody] IEnumerable<DialectCreateVM> models)
        {
            if (models == null)
            {
                _logger.LogWarning("Data is null in CreateRange.");
                return BadRequest("Data is required.");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state in CreateRange: {ModelState}", ModelState);
                return BadRequest(ModelState);
            }

            try
            {
                _logger.LogInformation("Creating multiple Dialects.");
                var items = _mapper.Map<List<DialectRequestDso>>(models);
                var createdEntities = await _dialectService.CreateRangeAsync(items);
                var createdItems = _mapper.Map<List<DialectOutputVM>>(createdEntities);
                return Ok(createdItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating multiple Dialects");
                return StatusCode(500, "Internal Server Error");
            }
        }

        // Update an existing Dialect.
        [HttpPut("{id}", Name = "UpdateDialect")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DialectOutputVM>> Update(string id, [FromBody] DialectUpdateVM model)
        {
            try
            {
                var item = await _dialectService.GetByIdAsync(id);
                if (item == null)
                {
                    _logger.LogWarning("Dialect not found for update with ID: {id}", id);
                    return NotFound($"Dialect not found with ID: {id}");
                }

                _logger.LogInformation("Updating Dialect with ID: {id}", id);
                var newItem = _mapper.Map<DialectRequestDso>(model);
                newItem.Id = id; // Ensure ID is set
                newItem.LanguageId = item.LanguageId; // Preserve LanguageId
                var updatedEntity = await _dialectService.UpdateAsync(newItem);

                if (updatedEntity == null)
                {
                    _logger.LogWarning("Dialect not found for update with ID: {id}", id);
                    return NotFound();
                }

                var updatedItem = _mapper.Map<DialectOutputVM>(updatedEntity);
                return Ok(updatedItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating Dialect with ID: {id}", id);
                return StatusCode(500, "Internal Server Error");
            }
        }

        // Delete a Dialect.
        [HttpDelete("{id}", Name = "DeleteDialect")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                if (!await _dialectService.ExistsAsync(id))
                {
                    _logger.LogWarning("Dialect not found with ID: {id}", id);
                    return NotFound($"Dialect not found with ID: {id}");
                }

                _logger.LogInformation("Deleting Dialect with ID: {id}", id);
                await _dialectService.DeleteAsync(id);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting Dialect with ID: {id}", id);
                return StatusCode(500, "Internal Server Error");
            }
        }

        // Get count of Dialects.
        [HttpGet("CountDialect")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<int>> Count()
        {
            try
            {
                _logger.LogInformation("Counting Dialects...");
                var count = await _dialectService.CountAsync();
                return Ok(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while counting Dialects");
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}