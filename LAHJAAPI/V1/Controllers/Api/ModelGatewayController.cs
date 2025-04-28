using AutoGenerator.Helper;
using AutoGenerator.Helper.Translation;
using AutoMapper;
using LAHJAAPI.Services2;
using Microsoft.AspNetCore.Mvc;
using V1.DyModels.Dso.Requests;
using V1.DyModels.VMs;
using V1.Services.Services;

namespace LAHJAAPI.V1.Controllers.Api
{
    //[ApiExplorerSettings(GroupName = "V1")]
    [Route("api/v1/user/[controller]")]
    [ApiController]
    public class ModelGatewayController : ControllerBase
    {
        private readonly IUseModelGatewayService _modelgatewayService;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        public ModelGatewayController(IUseModelGatewayService modelgatewayService, IMapper mapper, ILoggerFactory logger)
        {
            _modelgatewayService = modelgatewayService;
            _mapper = mapper;
            _logger = logger.CreateLogger(typeof(ModelGatewayController).FullName);
        }

        // Get all ModelGateways.
        [HttpGet(Name = "GetModelGateways")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<ModelGatewayOutputVM>>> GetAll()
        {
            try
            {
                _logger.LogInformation("Fetching all ModelGateways...");
                var result = await _modelgatewayService.GetAllAsync();
                var items = _mapper.Map<List<ModelGatewayOutputVM>>(result);
                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching all ModelGateways");
                return StatusCode(500, "Internal Server Error");
            }
        }

        // Get a ModelGateway by ID.
        [HttpGet("{id}", Name = "GetModelGateway")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ModelGatewayOutputVM>> GetById(string? id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                _logger.LogWarning("Invalid ModelGateway ID received.");
                return BadRequest("Invalid ModelGateway ID.");
            }

            try
            {
                _logger.LogInformation("Fetching ModelGateway with ID: {id}", id);
                var entity = await _modelgatewayService.GetByIdAsync(id);
                if (entity == null)
                {
                    _logger.LogWarning("ModelGateway not found with ID: {id}", id);
                    return NotFound();
                }


                var item = _mapper.Map<ModelGatewayOutputVM>(entity);
                return Ok(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching ModelGateway with ID: {id}", id);
                return StatusCode(500, "Internal Server Error");
            }
        }

        // // Get a ModelGateway by Lg.
        [HttpGet("GetModelGatewayByLanguage", Name = "GetModelGatewayByLg")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ModelGatewayOutputVM>> GetModelGatewayByLg(ModelGatewayFilterVM model)
        {
            var id = model.Id;
            if (string.IsNullOrWhiteSpace(id))
            {
                _logger.LogWarning("Invalid ModelGateway ID received.");
                return BadRequest("Invalid ModelGateway ID.");
            }

            try
            {
                _logger.LogInformation("Fetching ModelGateway with ID: {id}", id);
                var entity = await _modelgatewayService.GetByIdAsync(id);
                if (entity == null)
                {
                    _logger.LogWarning("ModelGateway not found with ID: {id}", id);
                    return NotFound();
                }

                var item = _mapper.Map<ModelGatewayOutputVM>(entity, opt => opt.Items.Add(HelperTranslation.KEYLG, model.Lg));
                return Ok(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching ModelGateway with ID: {id}", id);
                return StatusCode(500, "Internal Server Error");
            }
        }

        // // Get a ModelGateways by Lg.
        [HttpGet("GetModelGatewaysByLanguage", Name = "GetModelGatewaysByLg")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<ModelGatewayOutputVM>>> GetModelGatewaysByLg(string? lg)
        {
            if (string.IsNullOrWhiteSpace(lg))
            {
                _logger.LogWarning("Invalid ModelGateway lg received.");
                return BadRequest("Invalid ModelGateway lg null ");
            }

            try
            {
                var modelgateways = await _modelgatewayService.GetAllAsync();
                if (modelgateways == null)
                {
                    _logger.LogWarning("ModelGateways not found  by  ");
                    return NotFound();
                }

                var items = _mapper.Map<IEnumerable<ModelGatewayOutputVM>>(modelgateways, opt => opt.Items.Add(HelperTranslation.KEYLG, lg));
                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching ModelGateways with Lg: {lg}", lg);
                return StatusCode(500, "Internal Server Error");
            }
        }

        // Create a new ModelGateway.
        [HttpPost(Name = "CreateModelGateway")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ModelGatewayOutputVM>> Create([FromBody] ModelGatewayCreateVM model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state in Create: {ModelState}", ModelState);
                return BadRequest(ModelState);
            }

            try
            {
                _logger.LogInformation("Creating new ModelGateway with data: {@model}", model);
                var item = _mapper.Map<ModelGatewayRequestDso>(model);
                item.Token = TokenService.GenerateSecureToken();
                var createdEntity = await _modelgatewayService.CreateAsync(item);
                var createdItem = _mapper.Map<ModelGatewayOutputVM>(createdEntity);
                return Ok(createdItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating a new ModelGateway");
                return StatusCode(500, "Internal Server Error");
            }
        }

        // Create multiple ModelGateways.
        [HttpPost("createRange")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<ModelGatewayOutputVM>>> CreateRange([FromBody] IEnumerable<ModelGatewayCreateVM> models)
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
                _logger.LogInformation("Creating multiple ModelGateways.");
                var items = _mapper.Map<List<ModelGatewayRequestDso>>(models);
                var createdEntities = await _modelgatewayService.CreateRangeAsync(items);
                var createdItems = _mapper.Map<List<ModelGatewayOutputVM>>(createdEntities);
                return Ok(createdItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating multiple ModelGateways");
                return StatusCode(500, "Internal Server Error");
            }
        }

        // Update an existing ModelGateway.
        [HttpPut("{id}", Name = "UpdateModelGateway")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ModelGatewayOutputVM>> Update(string id, [FromBody] ModelGatewayUpdateVM model)
        {
            try
            {
                _logger.LogInformation("Updating ModelGateway with ID: {id}", id);
                var modelGateway = await _modelgatewayService.GetByIdAsync(id);
                if (modelGateway == null)
                {
                    return NotFound(HandelErrors.NotFound("Record not found make sure that id is correct."));
                }

                var item = _mapper.Map<ModelGatewayRequestDso>(modelGateway);
                item.Id = id;

                var updatedEntity = await _modelgatewayService.UpdateAsync(item);
                if (updatedEntity == null)
                {
                    _logger.LogWarning("ModelGateway not found for update with ID: {id}", id);
                    return NotFound();
                }

                var updatedItem = _mapper.Map<ModelGatewayOutputVM>(updatedEntity);
                return Ok(updatedItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating ModelGateway with ID: {id}", id);
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPut("default/{id}", Name = "ChangeDefaultModelGateway")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> MakeDefault(string id)
        {
            try
            {
                _logger.LogInformation("Making ModelGateway with ID: {id} default.", id);
                var modelGateway = await _modelgatewayService.GetByIdAsync(id);
                if (modelGateway == null)
                {
                    return NotFound(HandelErrors.NotFound("Record not found make sure that id is correct."));
                }

                if (!modelGateway.IsDefault)
                {
                    var defaultModel = await _modelgatewayService.GetOneByAsync([new FilterCondition("IsDefault", true)]);
                    defaultModel.IsDefault = false;
                    await _modelgatewayService.UpdateAsync(_mapper.Map<ModelGatewayRequestDso>(defaultModel));

                    modelGateway.IsDefault = true;
                    await _modelgatewayService.UpdateAsync(_mapper.Map<ModelGatewayRequestDso>(modelGateway));
                }
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while making ModelGateway with ID: {id} default", id);
                return StatusCode(500, "Internal Server Error");
            }
        }

        // Delete a ModelGateway.
        [HttpDelete("{id}", Name = "DeleteModelGateway")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                _logger.LogWarning("Invalid ModelGateway ID received in Delete.");
                return BadRequest("Invalid ModelGateway ID.");
            }

            try
            {
                if (!await _modelgatewayService.ExistsAsync(id))
                {
                    _logger.LogWarning("ModelGateway not found with ID: {id}", id);
                    return NotFound();
                }
                _logger.LogInformation("Deleting ModelGateway with ID: {id}", id);
                await _modelgatewayService.DeleteAsync(id);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting ModelGateway with ID: {id}", id);
                return StatusCode(500, "Internal Server Error");
            }
        }

        // Get count of ModelGateways.
        [HttpGet("CountModelGateway")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<int>> Count()
        {
            try
            {
                _logger.LogInformation("Counting ModelGateways...");
                var count = await _modelgatewayService.CountAsync();
                return Ok(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while counting ModelGateways");
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}