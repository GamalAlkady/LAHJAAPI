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
    //[ApiExplorerSettings(GroupName = "User")]
    [Route("api/v1/user/[controller]")]
    [ApiController]
    public class CategoryTabController : ControllerBase
    {
        private readonly IUseCategoryTabService _categoryTabService;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public CategoryTabController(
            IUseCategoryTabService categoryTabService,
            IMapper mapper,
            ILoggerFactory logger)
        {
            _categoryTabService = categoryTabService;
            _mapper = mapper;
            _logger = logger.CreateLogger(typeof(CategoryTabController).FullName);
        }

        // Get all CategoryTabs
        [HttpGet(Name = "GetCategoryTabs")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<CategoryTabOutputVM>>> GetAll(string? lg)
        {
            try
            {
                _logger.LogInformation("Fetching all CategoryTabs...");
                var result = await _categoryTabService.GetAllAsync();

                if (lg.IsNullOrWhiteSpace())
                    return Ok(_mapper.Map<List<CategoryTabOutputVM>>(result));

                return Ok(_mapper.Map<List<CategoryTabOutputVM>>(result, opt => opt.Items[HelperTranslation.KEYLG] = lg));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching all CategoryTabs");
                return StatusCode(500, "Internal Server Error");
            }
        }

        // Get active CategoryTabs
        [HttpGet("Active", Name = "GetActiveCategoryTabs")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<CategoryTabOutputVM>>> GetActive(string? lg)
        {
            try
            {
                _logger.LogInformation("Fetching active CategoryTabs...");
                var result = await _categoryTabService.GetAllByAsync([new FilterCondition("active", true)]);

                if (lg.IsNullOrWhiteSpace())
                    return Ok(_mapper.Map<List<CategoryTabOutputVM>>(result.Data));

                return Ok(_mapper.Map<List<CategoryTabOutputVM>>(result.Data, opt => opt.Items[HelperTranslation.KEYLG] = lg));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching active CategoryTabs");
                return StatusCode(500, "Internal Server Error");
            }
        }

        // Get CategoryTab by ID
        [HttpGet("{id}", Name = "GetCategoryTab")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CategoryTabOutputVM>> GetById(string id, string? lg)
        {
            try
            {
                _logger.LogInformation("Fetching CategoryTab with ID: {id}", id);
                var entity = await _categoryTabService.GetByIdAsync(id);

                //if (entity == null)
                //{
                //    _logger.LogWarning("CategoryTab not found with ID: {id}", id);
                //    return NotFound();
                //}

                if (lg.IsNullOrWhiteSpace())
                    return Ok(_mapper.Map<CategoryTabOutputVM>(entity));

                var item = _mapper.Map<CategoryTabOutputVM>(entity, opts => opts.Items.Add(HelperTranslation.KEYLG, lg));
                return Ok(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching CategoryTab with ID: {id}", id);
                return StatusCode(500, "Internal Server Error");
            }
        }

        // Get CategoryTab by language filter
        [HttpGet("ByLanguage", Name = "GetCategoryTabByLanguage")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CategoryTabOutputVM>> GetByLanguage([FromQuery] CategoryTabFilterVM model)
        {
            try
            {
                _logger.LogInformation("Fetching CategoryTab with ID: {id} and language: {lg}", model.Id, model.Lg);
                var entity = await _categoryTabService.GetByIdAsync(model.Id);

                if (entity == null)
                {
                    _logger.LogWarning("CategoryTab not found with ID: {id}", model.Id);
                    return NotFound();
                }

                var item = _mapper.Map<CategoryTabOutputVM>(entity, opt => opt.Items.Add(HelperTranslation.KEYLG, model.Lg));
                return Ok(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching CategoryTab with ID: {id}", model.Id);
                return StatusCode(500, "Internal Server Error");
            }
        }

        // Get CategoryTabs by language
        [HttpGet("ByLanguage/All", Name = "GetCategoryTabsByLanguage")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<CategoryTabOutputVM>>> GetByLanguage(string lg)
        {
            try
            {
                _logger.LogInformation("Fetching CategoryTabs with language: {lg}", lg);
                var result = await _categoryTabService.GetAllAsync();

                if (result == null || !result.Any())
                {
                    _logger.LogWarning("No CategoryTabs found");
                    return NotFound();
                }

                var items = _mapper.Map<List<CategoryTabOutputVM>>(result, opt => opt.Items.Add(HelperTranslation.KEYLG, lg));
                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching CategoryTabs with language: {lg}", lg);
                return StatusCode(500, "Internal Server Error");
            }
        }

        // Create new CategoryTab
        [HttpPost(Name = "CreateCategoryTab")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CategoryTabOutputVM>> Create([FromBody] CategoryTabCreateVM model)
        {
            if (model == null)
            {
                _logger.LogWarning("CategoryTab data is null in Create");
                return BadRequest("CategoryTab data is required");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state in Create: {ModelState}", ModelState);
                return BadRequest(ModelState);
            }

            try
            {
                _logger.LogInformation("Creating new CategoryTab with data: {@model}", model);
                var item = _mapper.Map<CategoryTabRequestDso>(model);
                var createdEntity = await _categoryTabService.CreateAsync(item);
                var createdItem = _mapper.Map<CategoryTabOutputVM>(createdEntity);
                return Ok(createdItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating new CategoryTab");
                return StatusCode(500, "Internal Server Error");
            }
        }

        // Create multiple CategoryTabs
        [HttpPost("createRange")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<CategoryTabOutputVM>>> CreateRange([FromBody] IEnumerable<CategoryTabCreateVM> models)
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
                _logger.LogInformation("Creating {count} CategoryTabs", models.Count());
                var items = _mapper.Map<List<CategoryTabRequestDso>>(models);
                var createdEntities = await _categoryTabService.CreateRangeAsync(items);
                var createdItems = _mapper.Map<List<CategoryTabOutputVM>>(createdEntities);
                return Ok(createdItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating multiple CategoryTabs");
                return StatusCode(500, "Internal Server Error");
            }
        }

        // Update CategoryTab
        [HttpPut("{id}", Name = "UpdateCategoryTab")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CategoryTabOutputVM>> Update(string id, [FromBody] CategoryTabUpdateVM model)
        {
            try
            {
                _logger.LogInformation("Updating CategoryTab with ID: {id}", id);
                var existingItem = await _categoryTabService.GetByIdAsync(id);

                if (existingItem == null)
                {
                    _logger.LogWarning("CategoryTab not found with ID: {id}", id);
                    return NotFound();
                }

                var item = _mapper.Map<CategoryTabRequestDso>(model);
                item.Id = id;
                var updatedEntity = await _categoryTabService.UpdateAsync(item);

                if (updatedEntity == null)
                {
                    _logger.LogError("Failed to update CategoryTab with ID: {id}", id);
                    return BadRequest($"Failed to update CategoryTap with ID: {id}");
                }

                var updatedItem = _mapper.Map<CategoryTabOutputVM>(updatedEntity);
                return Ok(updatedItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating CategoryTab with ID: {id}", id);
                return StatusCode(500, "Internal Server Error");
            }
        }

        // Delete CategoryTab
        [HttpDelete("{id}", Name = "DeleteCategoryTab")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                _logger.LogInformation("Deleting CategoryTab with ID: {id}", id);

                if (!await _categoryTabService.ExistsAsync(id))
                {
                    _logger.LogWarning("CategoryTab not found with ID: {id}", id);
                    return NotFound();
                }

                await _categoryTabService.DeleteAsync(id);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting CategoryTab with ID: {id}", id);
                return StatusCode(500, "Internal Server Error");
            }
        }

        // Get CategoryTabs count
        [HttpGet("Count", Name = "CountCategoryTabs")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<int>> Count()
        {
            try
            {
                _logger.LogInformation("Counting CategoryTabs...");
                var count = await _categoryTabService.CountAsync();
                return Ok(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while counting CategoryTabs");
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}