using AutoGenerator.Helper;
using AutoGenerator.Helper.Translation;
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
    public class ModelAiController : BaseBPRControllerLayer<ModelAiRequestDso, ModelAiResponseDso, ModelAiCreateVM, ModelAiOutputVM, ModelAiUpdateVM, ModelAiInfoVM, ModelAiDeleteVM, ModelAiFilterVM>
    {
        private readonly IUseModelAiService _modelAiService;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public ModelAiController(
            IUseModelAiService modelAiService,
            IMapper mapper,
            ILoggerFactory logger) : base(mapper, logger, modelAiService)
        {
            _modelAiService = modelAiService;
            _mapper = mapper;
            _logger = logger.CreateLogger(typeof(ModelAiController).FullName);
        }

        //// Get all ModelAis
        //[HttpGet(Name = "GetModelAis")]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        //public async Task<ActionResult<IEnumerable<ModelAiOutputVM>>> GetAll(string? lg)
        //{
        //    try
        //    {
        //        _logger.LogInformation("Fetching all ModelAis...");
        //        var result = await _modelAiService.GetAllAsync();

        //        if (lg.IsNullOrWhiteSpace())
        //            return Ok(_mapper.Map<List<ModelAiOutputVM>>(result));

        //        return Ok(_mapper.Map<List<ModelAiOutputVM>>(result, opt => opt.Items[HelperTranslation.KEYLG] = lg));
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error while fetching all ModelAis");
        //        return StatusCode(500, HandleResult.Problem(ex));
        //    }
        //}


        //// Get ModelAi by ID
        //[HttpGet("{id}", Name = "GetModelAi")]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        //public async Task<ActionResult<ModelAiOutputVM>> GetById(string id, string? lg)
        //{
        //    try
        //    {
        //        _logger.LogInformation("Fetching ModelAi with ID: {id}", id);
        //        var entity = await _modelAiService.GetByIdAsync(id);

        //        if (entity == null)
        //        {
        //            _logger.LogWarning("ModelAi not found with ID: {id}", id);
        //            return NotFound();
        //        }

        //        if (lg.IsNullOrWhiteSpace())
        //            return Ok(_mapper.Map<ModelAiOutputVM>(entity));

        //        var item = _mapper.Map<ModelAiOutputVM>(entity, opts => opts.Items.Add(HelperTranslation.KEYLG, lg));
        //        return Ok(item);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error while fetching ModelAi with ID: {id}", id);
        //        return StatusCode(500, HandleResult.Problem(ex));
        //    }
        //}

        [EndpointSummary("Get Models By Type")]
        [HttpGet("ByType/{type}", Name = "GetModelsByType")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ModelAiOutputVM>>> GetModelsByType(string type)
        {
            var items = await _modelAiService.GetAllByAsync([new FilterCondition("Type", type)]);
            var result = _mapper.Map<IEnumerable<ModelAiOutputVM>>(items);
            return Ok(result);
        }

        [EndpointSummary("Get Categories By Type")]
        [HttpGet("CategoriesByType/{type}", Name = "GetCategoriesByType")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<string>>> GetCategoriesByType(string type)
        {
            var items = await _modelAiService.GetAllByAsync([new FilterCondition("Type", type)]);
            if (items.TotalRecords == 0) return NoContent();
            var result = items.Data.Select(s => s.Category);
            return Ok(result);
        }
        [EndpointSummary("Get Languages By Type And Category")]
        [HttpGet("LanguagesBy", Name = "GetLanguagesBy")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<string>>> GetLanguagesBy(string type, string category)
        {
            var items = await _modelAiService.GetAllByAsync([
                new FilterCondition("Type", type),
                new FilterCondition("Category", type),
            ]);

            if (items.TotalRecords == 0) return NoContent();
            var result = items.Data.Select(s => s.Language).Distinct();
            return Ok(result);
        }


        [EndpointSummary("GetAllByModelGateway")]
        [HttpGet("GetAllByModelGateway", Name = "GetAllByModelGateway")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<string>>> GetAllByModelGateway(string modelGatewayId, string? lg = "en")
        {
            var items = await _modelAiService.GetAllAsync(nameof(ModelAiCreateVM.ModelGatewayId), modelGatewayId);
            var result = _mapper.Map<IEnumerable<ModelAiOutputVM>>(items, opts => opts.Items[HelperTranslation.KEYLG] = lg);
            return Ok(result);
        }

        [EndpointSummary("Get Models By Category")]
        [HttpGet("category/{category}", Name = "GetModelsByCategory")]

        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ModelAiOutputVM>>> GetByCategoryAsync(string category, string? lg = "en")
        {
            var items = await _modelAiService.GetAllAsync("Category", category);
            var result = _mapper.Map<IEnumerable<ModelAiOutputVM>>(items, opts => opts.Items[HelperTranslation.KEYLG] = lg);
            return Ok(result);
        }


        [EndpointSummary("Filter models")]
        [HttpPost("GetFilterModel", Name = "FilterMaodelAi")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ModelAiOutputVM>> GetFilterModel([FromBody] ModelAiFilterVM serchModelAI, string lg = "en")
        {

            var response = await _modelAiService.FilterMaodelAi(serchModelAI);
            var result = response.ToResponse(_mapper.Map<IEnumerable<ModelAiOutputVM>>(response.Data, opts => opts.Items[HelperTranslation.KEYLG] = lg));

            return Ok(result.Data);
        }



        // Update ModelAi
        [HttpPut("update/{id}", Name = "UpdateModelAi")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ModelAiOutputVM>> Update(string id, [FromBody] ModelAiUpdateVM model)
        {
            try
            {
                _logger.LogInformation("Updating ModelAi with ID: {id}", id);
                var existingItem = await _modelAiService.GetByIdAsync(id);

                if (existingItem == null)
                {
                    _logger.LogWarning("ModelAi not found with ID: {id}", id);
                    return NotFound();
                }

                var item = _mapper.Map<ModelAiRequestDso>(model);
                item.Id = id;
                item.ModelGatewayId = existingItem.ModelGatewayId;
                var updatedEntity = await _modelAiService.UpdateAsync(item);

                if (updatedEntity == null)
                {
                    _logger.LogWarning("Failed to update ModelAi with ID: {id}", id);
                    return NotFound();
                }

                var updatedItem = _mapper.Map<ModelAiOutputVM>(updatedEntity);
                return Ok(updatedItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating ModelAi with ID: {id}", id);
                return StatusCode(500, HandleResult.Problem(ex));
            }
        }


    }
}