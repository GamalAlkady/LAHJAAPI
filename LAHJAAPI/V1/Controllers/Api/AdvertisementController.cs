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
    public class AdvertisementController : ControllerBase
    {
        private readonly IUseAdvertisementService _advertisementService;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        public AdvertisementController(IUseAdvertisementService advertisementService, IMapper mapper, ILoggerFactory logger)
        {
            _advertisementService = advertisementService;
            _mapper = mapper;
            _logger = logger.CreateLogger(typeof(AdvertisementController).FullName);
        }

        // Get all Advertisements.
        [AllowAnonymous]
        [HttpGet(Name = "GetAdvertisements")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<AdvertisementOutputVM>>> GetAll(string? lg)
        {
            try
            {
                _logger.LogInformation("Fetching all Advertisements...");
                var result = await _advertisementService.GetAllAsync();
                //TODO: error when mapping multi entities while key lg = ar 
                if (string.IsNullOrWhiteSpace(lg))
                    return Ok(_mapper.Map<List<AdvertisementOutputVM>>(result));
                return Ok(_mapper.Map<List<AdvertisementOutputVM>>(result, opt => opt.Items[HelperTranslation.KEYLG] = lg));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching all Advertisements");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [AllowAnonymous]
        [HttpGet("ActiveAdvertisements", Name = "GetActiveAdvertisements")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<AdvertisementOutputVM>>> GetActiveAdvertisements(string? lg)
        {
            try
            {
                _logger.LogInformation("Fetching all active Advertisements...");
                var result = await _advertisementService.GetAllByAsync([new FilterCondition("active", true)]);
                if (string.IsNullOrWhiteSpace(lg))
                    return Ok(_mapper.Map<List<AdvertisementOutputVM>>(result.Data));
                return Ok(_mapper.Map<List<AdvertisementOutputVM>>(result.Data, opt => opt.Items[HelperTranslation.KEYLG] = lg));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching all active Advertisements");
                return StatusCode(500, "Internal Server Error");
            }
        }

        // Get a Advertisement by ID.
        [HttpGet("{id}", Name = "GetAdvertisement")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AdvertisementOutputVM>> GetById(string id, string? lg)
        {
            try
            {
                _logger.LogInformation("Fetching Advertisement with ID: {id}", id);
                var entity = await _advertisementService.GetByIdAsync(id);
                if (entity == null)
                {
                    _logger.LogWarning("Advertisement not found with ID: {id}", id);
                    return NotFound();
                }
                if (string.IsNullOrWhiteSpace(lg))
                    return Ok(_mapper.Map<AdvertisementOutputVM>(entity));
                var item = _mapper.Map<AdvertisementOutputVM>(entity, opts => opts.Items.Add(HelperTranslation.KEYLG, lg));
                return Ok(item);
            }
            catch (NullReferenceException ex)
            {
                _logger.LogError(ex, "Error while fetching Advertisement with ID: {id}", id);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching Advertisement with ID: {id}", id);
                return BadRequest(ex.Message);
                return StatusCode(500, "Internal Server Error");
            }
        }



        // // Get a Advertisements by Lg.
        [HttpGet("GetAdvertisementsByLanguage", Name = "GetAdvertisementsByLg")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<AdvertisementOutputVM>>> GetAdvertisementsByLg(string lg)
        {
            if (string.IsNullOrWhiteSpace(lg))
            {
                _logger.LogWarning("Invalid Advertisement lg received.");
                return BadRequest("Invalid Advertisement lg null ");
            }

            try
            {
                var advertisements = await _advertisementService.GetAllAsync();
                if (advertisements == null)
                {
                    _logger.LogWarning("No Advertisements found for lg: {lg}", lg);
                    return NotFound();
                }

                //TODO: error when mapping multi entities while key lg = ar 
                var items = _mapper.Map<IEnumerable<AdvertisementOutputVM>>(advertisements, opt => opt.Items.Add(HelperTranslation.KEYLG, lg));
                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching Advertisements with Lg: {lg}", lg);
                return BadRequest(ex.Message);
                return StatusCode(500, "Internal Server Error");
            }
        }

        // Create a new Advertisement.
        [HttpPost(Name = "CreateAdvertisement")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AdvertisementOutputVM>> Create([FromBody] AdvertisementCreateVM model)
        {
            if (model == null)
            {
                _logger.LogWarning("Advertisement data is null in Create.");
                return BadRequest("Advertisement data is required.");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state in Create: {ModelState}", ModelState);
                return BadRequest(ModelState);
            }

            try
            {
                _logger.LogInformation("Creating new Advertisement with data: {@model}", model);
                var item = _mapper.Map<AdvertisementRequestDso>(model);
                var createdEntity = await _advertisementService.CreateAsync(item);
                var createdItem = _mapper.Map<AdvertisementOutputVM>(createdEntity);
                return Ok(createdItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating a new Advertisement");
                return StatusCode(500, ex.Message);
            }
        }

        // Create multiple Advertisements.
        [HttpPost("createRange")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<AdvertisementOutputVM>>> CreateRange([FromBody] IEnumerable<AdvertisementCreateVM> models)
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
                _logger.LogInformation("Creating multiple Advertisements.");
                var items = _mapper.Map<List<AdvertisementRequestDso>>(models);
                var createdEntities = await _advertisementService.CreateRangeAsync(items);
                var createdItems = _mapper.Map<List<AdvertisementOutputVM>>(createdEntities);
                return Ok(createdItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating multiple Advertisements");
                return StatusCode(500, "Internal Server Error");
            }
        }

        // Update an existing Advertisement.
        [HttpPut("{id}", Name = "UpdateAdvertisement")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AdvertisementOutputVM>> Update(string id, [FromBody] AdvertisementUpdateVM model)
        {
            try
            {
                var item = await _advertisementService.GetByIdAsync(id);
                if (item == null)
                {
                    _logger.LogWarning("Advertisement not found for update with ID: {id}", id);
                    return NotFound(string.Format("Advertisement not found for update with ID: {id}", id));
                }
                _logger.LogInformation("Updating Advertisement with ID: {id}", id);
                var newItem = _mapper.Map<AdvertisementRequestDso>(model);
                newItem.Id = id; // Ensure the ID is set for the update
                var updatedEntity = await _advertisementService.UpdateAsync(newItem);
                if (updatedEntity == null)
                {
                    _logger.LogWarning("Advertisement not found for update with ID: {id}", id);
                    return NotFound();
                }

                var updatedItem = _mapper.Map<AdvertisementOutputVM>(updatedEntity);
                return Ok(updatedItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating Advertisement with ID: {id}", id);
                return BadRequest(ex.Message);
                return StatusCode(500, "Internal Server Error");
            }
        }

        // Delete a Advertisement.
        [HttpDelete("{id}", Name = "DeleteAdvertisement")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                if (!await _advertisementService.ExistsAsync(id))
                {
                    _logger.LogWarning("Advertisement not found with ID: {id}", id);
                    return NotFound(HandelResult.NotFound($"Advertisement not found with ID: {id}"));
                }
                _logger.LogInformation("Deleting Advertisement with ID: {id}", id);
                await _advertisementService.DeleteAsync(id);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting Advertisement with ID: {id}", id);
                return StatusCode(500, "Internal Server Error");
            }
        }

        // Get count of Advertisements.
        [HttpGet("CountAdvertisement")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<int>> Count()
        {
            try
            {
                _logger.LogInformation("Counting Advertisements...");
                var count = await _advertisementService.CountAsync();
                return Ok(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while counting Advertisements");
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}