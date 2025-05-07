using AutoGenerator.Helper;
using AutoGenerator.Helper.Translation;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using V1.DyModels.Dso.Requests;
using V1.DyModels.VMs;
using V1.Services.Services;

namespace LAHJAAPI.V1.Controllers.Api
{
    [ApiExplorerSettings(GroupName = "User")]

    [Route("api/v1/user/[controller]")]
    [ApiController]
    public class CategoryModelController : ControllerBase
    {
        private readonly IUseCategoryModelService _categoryModelService;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public CategoryModelController(
            IUseCategoryModelService categoryModelService,
            IMapper mapper,
            ILoggerFactory logger)
        {
            _categoryModelService = categoryModelService;
            _mapper = mapper;
            _logger = logger.CreateLogger(typeof(CategoryModelController).FullName);
        }

        // Get all CategoryModels.
        [HttpGet(Name = "GetCategoryModels")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<CategoryModelOutputVM>>> GetAll(string? lg)
        {
            try
            {
                _logger.LogInformation("Fetching all CategoryModels...");
                var result = await _categoryModelService.GetAllAsync();

                if (string.IsNullOrWhiteSpace(lg))
                    return Ok(_mapper.Map<List<CategoryModelOutputVM>>(result));

                //TODO:error when send lg because there are items have no the same lg 
                return Ok(_mapper.Map<List<CategoryModelOutputVM>>(result, opt => opt.Items[HelperTranslation.KEYLG] = lg));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching all CategoryModels");
                return BadRequest(ex.Message);
                return StatusCode(500, "Internal Server Error");
            }
        }

        // Get a CategoryModel by ID.
        [HttpGet("{id}", Name = "GetCategoryModel")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CategoryModelOutputVM>> GetById(string id, string? lg)
        {
            try
            {
                _logger.LogInformation("Fetching CategoryModel with ID: {id}", id);
                var entity = await _categoryModelService.GetByIdAsync(id);

                if (entity == null)
                {
                    _logger.LogWarning("CategoryModel not found with ID: {id}", id);
                    return NotFound();
                }

                if (string.IsNullOrWhiteSpace(lg))
                    return Ok(_mapper.Map<CategoryModelOutputVM>(entity));

                //TODO: error when call by id catm_1b43de3b9ed44212a0b9086e5607a81f. I Think because the description is null
                var item = _mapper.Map<CategoryModelOutputVM>(entity, opts => opts.Items.Add(HelperTranslation.KEYLG, lg));
                return Ok(item);
            }
            catch (NullReferenceException ex)
            {
                _logger.LogError(ex, "Error while fetching CategoryModel with ID: {id}", id);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching CategoryModel with ID: {id}", id);
                return StatusCode(500, ex.Message);
            }
        }

        // Get a CategoryModel by Name.
        [AllowAnonymous]
        [HttpGet("ByName/{name}", Name = "GetCategoryModelByName")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CategoryModelOutputVM>> GetByName(string name, string? lg)
        {
            try
            {
                _logger.LogInformation("Fetching CategoryModel with name: {name}", name);
                var entity = await _categoryModelService.GetOneByAsync(
                [
                    new FilterCondition
                    {
                        PropertyName = nameof(CategoryModelRequestDso.Name),
                        Value = name,
                        Operator = FilterOperator.Contains,
                    }
                ]);

                if (entity == null)
                {
                    _logger.LogWarning("CategoryModel not found with name: {name}", name);
                    return NotFound();
                }

                if (string.IsNullOrWhiteSpace(lg))
                    return Ok(_mapper.Map<CategoryModelOutputVM>(entity));

                var item = _mapper.Map<CategoryModelOutputVM>(entity, opt => opt.Items.Add(HelperTranslation.KEYLG, lg));
                return Ok(item);
            }
            catch (NullReferenceException ex)
            {
                _logger.LogError(ex, "Error while fetching CategoryModel with name: {name}", name);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching CategoryModel with name: {name}", name);
                return StatusCode(500, "Internal Server Error");
            }
        }

        // Get CategoryModels by Language.
        [HttpGet("GetCategoryModelsByLanguage", Name = "GetCategoryModelsByLg")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<CategoryModelOutputVM>>> GetByLanguage(string lg)
        {
            if (string.IsNullOrWhiteSpace(lg))
            {
                _logger.LogWarning("Invalid language parameter received.");
                return BadRequest("Language parameter is required.");
            }

            try
            {
                _logger.LogInformation("Fetching CategoryModels with language: {lg}", lg);
                var result = await _categoryModelService.GetAllAsync();

                if (result == null)
                {
                    _logger.LogWarning("No CategoryModels found.");
                    return NotFound();
                }

                var items = _mapper.Map<IEnumerable<CategoryModelOutputVM>>(result, opt => opt.Items.Add(HelperTranslation.KEYLG, lg));
                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching CategoryModels with language: {lg}", lg);
                return StatusCode(500, ex.Message);
            }
        }

        // Create a new CategoryModel.
        [HttpPost(Name = "CreateCategoryModel")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CategoryModelOutputVM>> Create([FromBody] CategoryModelCreateVM model)
        {
            if (model == null)
            {
                _logger.LogWarning("CategoryModel data is null in Create.");
                return BadRequest("CategoryModel data is required.");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state in Create: {ModelState}", ModelState);
                return BadRequest(ModelState);
            }

            try
            {
                _logger.LogInformation("Creating new CategoryModel with data: {@model}", model);
                var item = _mapper.Map<CategoryModelRequestDso>(model);
                var createdEntity = await _categoryModelService.CreateAsync(item);
                var createdItem = _mapper.Map<CategoryModelOutputVM>(createdEntity);
                return Ok(createdItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating a new CategoryModel");
                return StatusCode(500, "Internal Server Error");
            }
        }

        // Create multiple CategoryModels.
        [HttpPost("createRange")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<CategoryModelOutputVM>>> CreateRange([FromBody] IEnumerable<CategoryModelCreateVM> models)
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
                _logger.LogInformation("Creating multiple CategoryModels.");
                var items = _mapper.Map<List<CategoryModelRequestDso>>(models);
                var createdEntities = await _categoryModelService.CreateRangeAsync(items);
                var createdItems = _mapper.Map<List<CategoryModelOutputVM>>(createdEntities);
                return Ok(createdItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating multiple CategoryModels");
                return StatusCode(500, "Internal Server Error");
            }
        }

        // Update an existing CategoryModel.
        [HttpPut("{id}", Name = "UpdateCategoryModel")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CategoryModelOutputVM>> Update(string id, [FromBody] CategoryModelUpdateVM model)
        {
            try
            {
                var item = await _categoryModelService.GetByIdAsync(id);
                if (item == null)
                {
                    _logger.LogWarning("CategoryModel not found for update with ID: {id}", id);
                    return NotFound($"CategoryModel not found with ID: {id}");
                }

                _logger.LogInformation("Updating CategoryModel with ID: {id}", id);
                var newItem = _mapper.Map<CategoryModelRequestDso>(model);
                newItem.Id = id; // Ensure ID is set
                var updatedEntity = await _categoryModelService.UpdateAsync(newItem);

                if (updatedEntity == null)
                {
                    _logger.LogWarning("CategoryModel not found for update with ID: {id}", id);
                    return NotFound();
                }

                var updatedItem = _mapper.Map<CategoryModelOutputVM>(updatedEntity);
                return Ok(updatedItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating CategoryModel with ID: {id}", id);
                return StatusCode(500, "Internal Server Error");
            }
        }

        // Delete a CategoryModel.
        [HttpDelete("{id}", Name = "DeleteCategoryModel")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                if (!await _categoryModelService.ExistsAsync(id))
                {
                    _logger.LogWarning("CategoryModel not found with ID: {id}", id);
                    return NotFound($"CategoryModel not found with ID: {id}");
                }

                _logger.LogInformation("Deleting CategoryModel with ID: {id}", id);
                await _categoryModelService.DeleteAsync(id);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting CategoryModel with ID: {id}", id);
                return StatusCode(500, "Internal Server Error");
            }
        }

        // Get count of CategoryModels.
        [HttpGet("CountCategoryModel")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<int>> Count()
        {
            try
            {
                _logger.LogInformation("Counting CategoryModels...");
                var count = await _categoryModelService.CountAsync();
                return Ok(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while counting CategoryModels");
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}