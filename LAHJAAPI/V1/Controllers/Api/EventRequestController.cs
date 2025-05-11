using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using V1.DyModels.VMs;
using V1.Services.Services;

namespace LAHJAAPI.V1.Controllers.Api
{
    //[ApiExplorerSettings(GroupName = "User")]
    [Route("api/v1/user/[controller]")]
    [ApiController]
    public class EventRequestController : ControllerBase
    {
        private readonly IUseEventRequestService _eventrequestService;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        public EventRequestController(IUseEventRequestService eventrequestService, IMapper mapper, ILoggerFactory logger)
        {
            _eventrequestService = eventrequestService;
            _mapper = mapper;
            _logger = logger.CreateLogger(typeof(EventRequestController).FullName);
        }

        // Get all EventRequests.
        [HttpGet(Name = "GetEventRequests")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<EventRequestOutputVM>>> GetAll()
        {
            try
            {
                _logger.LogInformation("Fetching all EventRequests...");
                var result = await _eventrequestService.GetAllAsync();
                var items = _mapper.Map<List<EventRequestOutputVM>>(result);
                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching all EventRequests");
                return StatusCode(500, "Internal Server Error");
            }
        }

        // Get a EventRequest by ID.
        [HttpGet("{id}", Name = "GetEventRequest")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EventRequestOutputVM>> GetById(string id)
        {

            try
            {
                _logger.LogInformation("Fetching EventRequest with ID: {id}", id);
                var entity = await _eventrequestService.GetByIdAsync(id);
                if (entity == null)
                {
                    _logger.LogWarning("EventRequest not found with ID: {id}", id);
                    return NotFound();
                }

                var item = _mapper.Map<EventRequestOutputVM>(entity);
                return Ok(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching EventRequest with ID: {id}", id);
                return StatusCode(500, "Internal Server Error");
            }
        }


        // Delete a EventRequest.
        [HttpDelete("{id}", Name = "DeleteEventRequest")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                _logger.LogWarning("Invalid EventRequest ID received in Delete.");
                return BadRequest("Invalid EventRequest ID.");
            }

            try
            {
                if (!await _eventrequestService.ExistsAsync(id))
                {
                    _logger.LogWarning("EventRequest not found with ID: {id}", id);
                    return NotFound(HandleResult.NotFound($"EventRequest not found with ID: {id}"));
                }
                _logger.LogInformation("Deleting EventRequest with ID: {id}", id);
                await _eventrequestService.DeleteAsync(id);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting EventRequest with ID: {id}", id);
                return StatusCode(500, "Internal Server Error");
            }
        }

        // Get count of EventRequests.
        [HttpGet("CountEventRequest")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<int>> Count()
        {
            try
            {
                _logger.LogInformation("Counting EventRequests...");
                var count = await _eventrequestService.CountAsync();
                return Ok(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while counting EventRequests");
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}