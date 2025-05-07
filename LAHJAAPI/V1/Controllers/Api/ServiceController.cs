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
    public class ServiceController : ControllerBase
    {
        private readonly IUseServiceService _serviceService;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        public ServiceController(IUseServiceService serviceService, IMapper mapper, ILoggerFactory logger)
        {
            _serviceService = serviceService;
            _mapper = mapper;
            _logger = logger.CreateLogger(typeof(ServiceController).FullName);
        }

        // Get all Services.
        [HttpGet(Name = "GetServices")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<ServiceOutputVM>>> GetAll()
        {
            try
            {
                _logger.LogInformation("Fetching all Services...");
                var result = await _serviceService.GetAllAsync();
                var items = _mapper.Map<List<ServiceOutputVM>>(result);
                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching all Services");
                return StatusCode(500, "Internal Server Error");
            }
        }

        // Get a Service by ID.
        [HttpGet("{id}", Name = "GetService")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ServiceOutputVM>> GetById(string? id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                _logger.LogWarning("Invalid Service ID received.");
                return BadRequest("Invalid Service ID.");
            }

            try
            {
                _logger.LogInformation("Fetching Service with ID: {id}", id);
                var entity = await _serviceService.GetByIdAsync(id);
                if (entity == null)
                {
                    _logger.LogWarning("Service not found with ID: {id}", id);
                    return NotFound();
                }

                var item = _mapper.Map<ServiceOutputVM>(entity);
                return Ok(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching Service with ID: {id}", id);
                return StatusCode(500, "Internal Server Error");
            }
        }

        // // Get a Service by Lg.
        [HttpGet("GetServiceByLanguage", Name = "GetServiceByLg")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ServiceOutputVM>> GetServiceByLg(ServiceFilterVM model)
        {
            var id = model.Id;
            if (string.IsNullOrWhiteSpace(id))
            {
                _logger.LogWarning("Invalid Service ID received.");
                return BadRequest("Invalid Service ID.");
            }

            try
            {
                _logger.LogInformation("Fetching Service with ID: {id}", id);
                var entity = await _serviceService.GetByIdAsync(id);
                if (entity == null)
                {
                    _logger.LogWarning("Service not found with ID: {id}", id);
                    return NotFound();
                }

                var item = _mapper.Map<ServiceOutputVM>(entity, opt => opt.Items.Add(HelperTranslation.KEYLG, model.Lg));
                return Ok(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching Service with ID: {id}", id);
                return StatusCode(500, "Internal Server Error");
            }
        }

        // // Get a Services by Lg.
        [HttpGet("GetServicesByLanguage", Name = "GetServicesByLg")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<ServiceOutputVM>>> GetServicesByLg(string? lg)
        {
            if (string.IsNullOrWhiteSpace(lg))
            {
                _logger.LogWarning("Invalid Service lg received.");
                return BadRequest("Invalid Service lg null ");
            }

            try
            {
                var services = await _serviceService.GetAllAsync();
                if (services == null)
                {
                    _logger.LogWarning("Services not found  by  ");
                    return NotFound();
                }

                var items = _mapper.Map<IEnumerable<ServiceOutputVM>>(services, opt => opt.Items.Add(HelperTranslation.KEYLG, lg));
                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching Services with Lg: {lg}", lg);
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("GetServicesByModelAi", Name = "GetServicesByModelAi")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PagedResponse<ServiceOutputVM>>> GetServicesByModelAiId(string modelAiId, string? lg = null)
        {

            try
            {
                var response = await _serviceService.GetAllByAsync([new FilterCondition(nameof(ServiceRequestDso.ModelAiId), modelAiId)]);

                if (response.TotalRecords == 0)
                {
                    _logger.LogWarning("Services not found  by  modelAiId: {modelAiId}", modelAiId);
                    return NotFound($"Services not found  by  modelAiId: {modelAiId}");
                }
                if (lg.IsNullOrWhiteSpace())
                {
                    return Ok(response.ToResponse(_mapper.Map<IEnumerable<ServiceOutputVM>>(response.Data)));
                }
                var response1 = response.ToResponse(_mapper.Map<IEnumerable<ServiceOutputVM>>(response.Data, opt => opt.Items.Add(HelperTranslation.KEYLG, lg)));
                return Ok(response1);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching Services with Lg: {lg}", lg);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("GetModelAiByService", Name = "GetModelAiByService")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ModelAiOutputVM>> GetModelAiByService(string serviceId, string? lg = null)
        {

            try
            {
                var service = await _serviceService.GetOneByAsync(
                    [new FilterCondition(nameof(ServiceRequestDso.Id), serviceId)], new ParamOptions([nameof(ServiceRequestDso.ModelAi)]));

                if (service == null) return NotFound(HandelResult.Text("Service not found."));
                if (lg.IsNullOrWhiteSpace())
                {
                    return Ok(_mapper.Map<ModelAiOutputVM>(service.ModelAi));
                }

                return Ok(_mapper.Map<ModelAiOutputVM>(service.ModelAi, opt => opt.Items.Add(HelperTranslation.KEYLG, lg)));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching Services with Lg: {lg}", lg);
                return StatusCode(500, ex.Message);
            }
        }


        // Create a new Service.
        [HttpPost(Name = "CreateService")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ServiceOutputVM>> Create([FromBody] ServiceCreateVM model)
        {

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state in Create: {ModelState}", ModelState);
                return BadRequest(ModelState);
            }

            try
            {
                _logger.LogInformation("Creating new Service with data: {@model}", model);
                var item = _mapper.Map<ServiceRequestDso>(model);
                var createdEntity = await _serviceService.CreateAsync(item);
                var createdItem = _mapper.Map<ServiceOutputVM>(createdEntity);
                return Ok(createdItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating a new Service");
                return StatusCode(500, "Internal Server Error");
            }
        }

        // Create multiple Services.
        [HttpPost("createRange")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<ServiceOutputVM>>> CreateRange([FromBody] IEnumerable<ServiceCreateVM> models)
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
                _logger.LogInformation("Creating multiple Services.");
                var items = _mapper.Map<List<ServiceRequestDso>>(models);
                var createdEntities = await _serviceService.CreateRangeAsync(items);
                var createdItems = _mapper.Map<List<ServiceOutputVM>>(createdEntities);
                return Ok(createdItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating multiple Services");
                return StatusCode(500, "Internal Server Error");
            }
        }

        // Update an existing Service.
        [HttpPut("{id}", Name = "UpdateService")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ServiceOutputVM>> Update(string id, [FromBody] ServiceUpdateVM model)
        {

            try
            {
                var existingItem = await _serviceService.GetByIdAsync(id);
                if (existingItem == null)
                {
                    _logger.LogWarning("Service not found with ID: {id}", id);
                    return NotFound(string.Format("Service not found with ID: {id}", id));
                }
                _logger.LogInformation("Updating Service with ID: {id}", id);
                var item = _mapper.Map<ServiceRequestDso>(model);
                item.Id = id; // Ensure the ID is set for the update operation
                item.ModelAiId = existingItem.ModelAiId; // Preserve the ModelAiId if needed
                item.Token = existingItem.Token; // Preserve the Token if needed

                var updatedEntity = await _serviceService.UpdateAsync(item);

                var updatedItem = _mapper.Map<ServiceOutputVM>(updatedEntity);
                return Ok(updatedItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating Service with ID: {id}", id);
                return StatusCode(500, HandelResult.Problem(ex));
            }
        }

        // Delete a Service.
        [HttpDelete("{id}", Name = "DeleteService")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(string? id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                _logger.LogWarning("Invalid Service ID received in Delete.");
                return BadRequest("Invalid Service ID.");
            }

            try
            {
                if (!await _serviceService.ExistsAsync(id))
                {
                    _logger.LogWarning("Service not found with ID: {id}", id);
                    return NotFound();
                }
                _logger.LogInformation("Deleting Service with ID: {id}", id);
                await _serviceService.DeleteAsync(id);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting Service with ID: {id}", id);
                return StatusCode(500, "Internal Server Error");
            }
        }

        // Get count of Services.
        [HttpGet("CountService")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<int>> Count()
        {
            try
            {
                _logger.LogInformation("Counting Services...");
                var count = await _serviceService.CountAsync();
                return Ok(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while counting Services");
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}