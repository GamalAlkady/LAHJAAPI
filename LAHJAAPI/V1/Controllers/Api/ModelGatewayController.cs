using AutoGenerator.Helper;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using V1.BPR.Layers.Base;
using V1.DyModels.Dso.Requests;
using V1.DyModels.Dso.Responses;
using V1.DyModels.VMs;
using V1.Services.Services;

namespace LAHJAAPI.V1.Controllers.Api
{
    //[ApiExplorerSettings(GroupName = "User")]
    [Route("api/v1/user/[controller]")]
    [ApiController]
    public class ModelGatewayController : BaseBPRControllerLayer<ModelGatewayRequestDso, ModelGatewayResponseDso, ModelGatewayCreateVM, ModelGatewayOutputVM, ModelGatewayUpdateVM, ModelGatewayInfoVM, ModelGatewayDeleteVM>
    {
        private readonly IUseModelGatewayService _modelgatewayService;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        public ModelGatewayController(
            IUseModelGatewayService modelgatewayService,
            IMapper mapper,
            ILoggerFactory logger) : base(mapper, logger, modelgatewayService)
        {
            _modelgatewayService = modelgatewayService;
            _mapper = mapper;
            _logger = logger.CreateLogger(typeof(ModelGatewayController).FullName);
        }



        // Update an existing ModelGateway.
        [HttpPut("update/{id}", Name = "UpdateModelGateway")]
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
                    return NotFound(HandleResult.NotFound("Record not found make sure that id is correct."));
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
                    return NotFound(HandleResult.NotFound("Record not found make sure that id is correct."));
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

    }
}