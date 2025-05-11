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
    public class FAQItemController : ControllerBase
    {
        private readonly IUseFAQItemService _faqItemService;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public FAQItemController(
            IUseFAQItemService faqItemService,
            IMapper mapper,
            ILoggerFactory logger)
        {
            _faqItemService = faqItemService;
            _mapper = mapper;
            _logger = logger.CreateLogger(typeof(FAQItemController).FullName);
        }

        // Get all FAQItems
        [HttpGet(Name = "GetFAQItems")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<FAQItemOutputVM>>> GetAll(string? lg)
        {
            try
            {
                _logger.LogInformation("Fetching all FAQItems...");
                var result = await _faqItemService.GetAllAsync();

                if (lg.IsNullOrWhiteSpace())
                    return Ok(_mapper.Map<List<FAQItemOutputVM>>(result));

                return Ok(_mapper.Map<List<FAQItemOutputVM>>(result, opt => opt.Items[HelperTranslation.KEYLG] = lg));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching all FAQItems");
                return StatusCode(500, "Internal Server Error");
            }
        }



        // Get FAQItem by ID
        [HttpGet("{id}", Name = "GetFAQItem")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<FAQItemOutputVM>> GetById(string id, string? lg)
        {
            try
            {
                _logger.LogInformation("Fetching FAQItem with ID: {id}", id);
                var entity = await _faqItemService.GetByIdAsync(id);

                if (entity == null)
                {
                    _logger.LogWarning("FAQItem not found with ID: {id}", id);
                    return NotFound();
                }

                if (lg.IsNullOrWhiteSpace())
                    return Ok(_mapper.Map<FAQItemOutputVM>(entity));

                var item = _mapper.Map<FAQItemOutputVM>(entity, opts => opts.Items.Add(HelperTranslation.KEYLG, lg));
                return Ok(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching FAQItem with ID: {id}", id);
                return StatusCode(500, "Internal Server Error");
            }
        }

        // Get FAQItem by language filter
        [HttpGet("ByLanguage", Name = "GetFAQItemByLg")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<FAQItemOutputVM>> GetByLanguage([FromQuery] FAQItemFilterVM model)
        {
            try
            {
                _logger.LogInformation("Fetching FAQItem with ID: {id} and language: {lg}", model.Id, model.Lg);
                var entity = await _faqItemService.GetByIdAsync(model.Id);

                if (entity == null)
                {
                    _logger.LogWarning("FAQItem not found with ID: {id}", model.Id);
                    return NotFound();
                }

                var item = _mapper.Map<FAQItemOutputVM>(entity, opt => opt.Items.Add(HelperTranslation.KEYLG, model.Lg));
                return Ok(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching FAQItem with ID: {id}", model.Id);
                return StatusCode(500, "Internal Server Error");
            }
        }

        // Get FAQItems by language
        [HttpGet("ByLanguage/All", Name = "GetFAQItemsByLg")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<FAQItemOutputVM>>> GetByLanguage(string lg)
        {
            try
            {
                _logger.LogInformation("Fetching FAQItems with language: {lg}", lg);
                var result = await _faqItemService.GetAllAsync();

                if (result == null || !result.Any())
                {
                    _logger.LogWarning("No FAQItems found");
                    return NotFound();
                }

                var items = _mapper.Map<List<FAQItemOutputVM>>(result, opt => opt.Items.Add(HelperTranslation.KEYLG, lg));
                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching FAQItems with language: {lg}", lg);
                return StatusCode(500, "Internal Server Error");
            }
        }

        // Create new FAQItem
        [HttpPost(Name = "CreateFAQItem")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<FAQItemOutputVM>> Create([FromBody] FAQItemCreateVM model)
        {
            if (model == null)
            {
                _logger.LogWarning("FAQItem data is null in Create");
                return BadRequest("FAQItem data is required");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state in Create: {ModelState}", ModelState);
                return BadRequest(ModelState);
            }

            try
            {
                _logger.LogInformation("Creating new FAQItem with data: {@model}", model);
                var item = _mapper.Map<FAQItemRequestDso>(model);
                var createdEntity = await _faqItemService.CreateAsync(item);
                var createdItem = _mapper.Map<FAQItemOutputVM>(createdEntity);
                return Ok(createdItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating new FAQItem");
                return StatusCode(500, "Internal Server Error");
            }
        }

        // Create multiple FAQItems
        [HttpPost("createRange")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<FAQItemOutputVM>>> CreateRange([FromBody] IEnumerable<FAQItemCreateVM> models)
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
                _logger.LogInformation("Creating {count} FAQItems", models.Count());
                var items = _mapper.Map<List<FAQItemRequestDso>>(models);
                var createdEntities = await _faqItemService.CreateRangeAsync(items);
                var createdItems = _mapper.Map<List<FAQItemOutputVM>>(createdEntities);
                return Ok(createdItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating multiple FAQItems");
                return StatusCode(500, "Internal Server Error");
            }
        }

        // Update FAQItem
        [HttpPut("{id}", Name = "UpdateFAQItem")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<FAQItemOutputVM>> Update(string id, [FromBody] FAQItemUpdateVM model)
        {
            try
            {
                _logger.LogInformation("Updating FAQItem with ID: {id}", id);
                var existingItem = await _faqItemService.GetByIdAsync(id);

                if (existingItem == null)
                {
                    _logger.LogWarning("FAQItem not found with ID: {id}", id);
                    return NotFound();
                }

                var item = _mapper.Map<FAQItemRequestDso>(model);
                item.Id = id;
                var updatedEntity = await _faqItemService.UpdateAsync(item);

                if (updatedEntity == null)
                {
                    _logger.LogWarning("Failed to update FAQItem with ID: {id}", id);
                    return NotFound();
                }

                var updatedItem = _mapper.Map<FAQItemOutputVM>(updatedEntity);
                return Ok(updatedItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating FAQItem with ID: {id}", id);
                return StatusCode(500, "Internal Server Error");
            }
        }

        // Delete FAQItem
        [HttpDelete("{id}", Name = "DeleteFAQItem")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                _logger.LogInformation("Deleting FAQItem with ID: {id}", id);

                if (!await _faqItemService.ExistsAsync(id))
                {
                    _logger.LogWarning("FAQItem not found with ID: {id}", id);
                    return NotFound();
                }

                await _faqItemService.DeleteAsync(id);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting FAQItem with ID: {id}", id);
                return StatusCode(500, "Internal Server Error");
            }
        }

        // Get FAQItems count
        [HttpGet("Count", Name = "CountFAQItems")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<int>> Count()
        {
            try
            {
                _logger.LogInformation("Counting FAQItems...");
                var count = await _faqItemService.CountAsync();
                return Ok(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while counting FAQItems");
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}