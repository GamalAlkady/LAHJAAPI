using AutoGenerator.Helper.Translation;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using V1.DyModels.Dso.Requests;
using V1.DyModels.VMs;
using V1.Services.Services;

namespace V1.Controllers.Api
{
    //[ApiExplorerSettings(GroupName = "V1")]
    [Route("api/V1/user/[controller]")]
    [ApiController]
    public class AuthorizationSessionServiceController : ControllerBase
    {
        private readonly IUseAuthorizationSessionServiceService _authorizationsessionserviceService;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        public AuthorizationSessionServiceController(IUseAuthorizationSessionServiceService authorizationsessionserviceService, IMapper mapper, ILoggerFactory logger)
        {
            _authorizationsessionserviceService = authorizationsessionserviceService;
            _mapper = mapper;
            _logger = logger.CreateLogger(typeof(AuthorizationSessionServiceController).FullName);
        }

        // Get all AuthorizationSessionServices.
        [HttpGet(Name = "GetAuthorizationSessionServices")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<AuthorizationSessionServiceOutputVM>>> GetAll()
        {
            try
            {
                _logger.LogInformation("Fetching all AuthorizationSessionServices...");
                var result = await _authorizationsessionserviceService.GetAllAsync();
                var items = _mapper.Map<List<AuthorizationSessionServiceOutputVM>>(result);
                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching all AuthorizationSessionServices");
                return StatusCode(500, "Internal Server Error");
            }
        }

        // Get a AuthorizationSessionService by ID.
        [HttpGet("{id}", Name = "GetAuthorizationSessionService")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AuthorizationSessionServiceInfoVM>> GetById(string? id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                _logger.LogWarning("Invalid AuthorizationSessionService ID received.");
                return BadRequest("Invalid AuthorizationSessionService ID.");
            }

            try
            {
                _logger.LogInformation("Fetching AuthorizationSessionService with ID: {id}", id);
                var entity = await _authorizationsessionserviceService.GetByIdAsync(id);
                if (entity == null)
                {
                    _logger.LogWarning("AuthorizationSessionService not found with ID: {id}", id);
                    return NotFound();
                }

                var item = _mapper.Map<AuthorizationSessionServiceInfoVM>(entity);
                return Ok(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching AuthorizationSessionService with ID: {id}", id);
                return StatusCode(500, "Internal Server Error");
            }
        }

        // // Get a AuthorizationSessionService by Lg.
        [HttpGet("GetAuthorizationSessionServiceByLanguage", Name = "GetAuthorizationSessionServiceByLg")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AuthorizationSessionServiceOutputVM>> GetAuthorizationSessionServiceByLg(AuthorizationSessionServiceFilterVM model)
        {
            var id = model.Id;
            if (string.IsNullOrWhiteSpace(id))
            {
                _logger.LogWarning("Invalid AuthorizationSessionService ID received.");
                return BadRequest("Invalid AuthorizationSessionService ID.");
            }

            try
            {
                _logger.LogInformation("Fetching AuthorizationSessionService with ID: {id}", id);
                var entity = await _authorizationsessionserviceService.GetByIdAsync(id);
                if (entity == null)
                {
                    _logger.LogWarning("AuthorizationSessionService not found with ID: {id}", id);
                    return NotFound();
                }

                var item = _mapper.Map<AuthorizationSessionServiceOutputVM>(entity, opt => opt.Items.Add(HelperTranslation.KEYLG, model.Lg));
                return Ok(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching AuthorizationSessionService with ID: {id}", id);
                return StatusCode(500, "Internal Server Error");
            }
        }

        // // Get a AuthorizationSessionServices by Lg.
        [HttpGet("GetAuthorizationSessionServicesByLanguage", Name = "GetAuthorizationSessionServicesByLg")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<AuthorizationSessionServiceOutputVM>>> GetAuthorizationSessionServicesByLg(string? lg)
        {
            if (string.IsNullOrWhiteSpace(lg))
            {
                _logger.LogWarning("Invalid AuthorizationSessionService lg received.");
                return BadRequest("Invalid AuthorizationSessionService lg null ");
            }

            try
            {
                var authorizationsessionservices = await _authorizationsessionserviceService.GetAllAsync();
                if (authorizationsessionservices == null)
                {
                    _logger.LogWarning("AuthorizationSessionServices not found  by  ");
                    return NotFound();
                }

                var items = _mapper.Map<IEnumerable<AuthorizationSessionServiceOutputVM>>(authorizationsessionservices, opt => opt.Items.Add(HelperTranslation.KEYLG, lg));
                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching AuthorizationSessionServices with Lg: {lg}", lg);
                return StatusCode(500, "Internal Server Error");
            }
        }

        // Create a new AuthorizationSessionService.
        [HttpPost(Name = "CreateAuthorizationSessionService")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AuthorizationSessionServiceOutputVM>> Create([FromBody] AuthorizationSessionServiceCreateVM model)
        {
            if (model == null)
            {
                _logger.LogWarning("AuthorizationSessionService data is null in Create.");
                return BadRequest("AuthorizationSessionService data is required.");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state in Create: {ModelState}", ModelState);
                return BadRequest(ModelState);
            }

            try
            {
                _logger.LogInformation("Creating new AuthorizationSessionService with data: {@model}", model);
                var item = _mapper.Map<AuthorizationSessionServiceRequestDso>(model);
                var createdEntity = await _authorizationsessionserviceService.CreateAsync(item);
                var createdItem = _mapper.Map<AuthorizationSessionServiceOutputVM>(createdEntity);
                return Ok(createdItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating a new AuthorizationSessionService");
                return StatusCode(500, "Internal Server Error");
            }
        }

        // Create multiple AuthorizationSessionServices.
        [HttpPost("createRange")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<AuthorizationSessionServiceOutputVM>>> CreateRange([FromBody] IEnumerable<AuthorizationSessionServiceCreateVM> models)
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
                _logger.LogInformation("Creating multiple AuthorizationSessionServices.");
                var items = _mapper.Map<List<AuthorizationSessionServiceRequestDso>>(models);
                var createdEntities = await _authorizationsessionserviceService.CreateRangeAsync(items);
                var createdItems = _mapper.Map<List<AuthorizationSessionServiceOutputVM>>(createdEntities);
                return Ok(createdItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating multiple AuthorizationSessionServices");
                return StatusCode(500, "Internal Server Error");
            }
        }

        // Update an existing AuthorizationSessionService.
        [HttpPut(Name = "UpdateAuthorizationSessionService")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AuthorizationSessionServiceOutputVM>> Update([FromBody] AuthorizationSessionServiceUpdateVM model)
        {
            if (model == null)
            {
                _logger.LogWarning("Invalid data in Update.");
                return BadRequest("Invalid data.");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state in Update: {ModelState}", ModelState);
                return BadRequest(ModelState);
            }

            try
            {
                _logger.LogInformation("Updating AuthorizationSessionService with ID: {id}", model?.Id);
                var item = _mapper.Map<AuthorizationSessionServiceRequestDso>(model);
                var updatedEntity = await _authorizationsessionserviceService.UpdateAsync(item);
                if (updatedEntity == null)
                {
                    _logger.LogWarning("AuthorizationSessionService not found for update with ID: {id}", model?.Id);
                    return NotFound();
                }

                var updatedItem = _mapper.Map<AuthorizationSessionServiceOutputVM>(updatedEntity);
                return Ok(updatedItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating AuthorizationSessionService with ID: {id}", model?.Id);
                return StatusCode(500, "Internal Server Error");
            }
        }

        // Delete a AuthorizationSessionService.
        [HttpDelete("{id}", Name = "DeleteAuthorizationSessionService")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(string? id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                _logger.LogWarning("Invalid AuthorizationSessionService ID received in Delete.");
                return BadRequest("Invalid AuthorizationSessionService ID.");
            }

            try
            {
                _logger.LogInformation("Deleting AuthorizationSessionService with ID: {id}", id);
                await _authorizationsessionserviceService.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting AuthorizationSessionService with ID: {id}", id);
                return StatusCode(500, "Internal Server Error");
            }
        }

        // Get count of AuthorizationSessionServices.
        [HttpGet("CountAuthorizationSessionService")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<int>> Count()
        {
            try
            {
                _logger.LogInformation("Counting AuthorizationSessionServices...");
                var count = await _authorizationsessionserviceService.CountAsync();
                return Ok(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while counting AuthorizationSessionServices");
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}