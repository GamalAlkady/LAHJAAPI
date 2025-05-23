using AutoGenerator;
using AutoGenerator.Helper;
using AutoGenerator.Helper.Translation;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Quartz.Util;
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
    public class ServiceController : BaseBPRControllerLayer<ServiceRequestDso, ServiceResponseDso, ServiceCreateVM, ServiceOutputVM, ServiceUpdateVM, ServiceInfoVM, ServiceDeleteVM, ServiceFilterVM>
    {
        private readonly IUseServiceService _serviceService;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        public ServiceController(IUseServiceService serviceService, IMapper mapper, ILoggerFactory logger) : base(mapper, logger, serviceService)
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
        public override async Task<ActionResult<IEnumerable<ServiceOutputVM>>> GetAllAsync()
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

        //// Get a Service by ID.
        //[HttpGet("{id}", Name = "GetService")]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        //public async Task<ActionResult<ServiceOutputVM>> GetById(string id)
        //{
        //    try
        //    {
        //        _logger.LogInformation("Fetching Service with ID: {id}", id);
        //        var entity = await _serviceService.GetByIdAsync(id);
        //        if (entity == null)
        //        {
        //            _logger.LogWarning("Service not found with ID: {id}", id);
        //            return NotFound();
        //        }

        //        var item = _mapper.Map<ServiceOutputVM>(entity);
        //        return Ok(item);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error while fetching Service with ID: {id}", id);
        //        return StatusCode(500, "Internal Server Error");
        //    }
        //}


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

                if (service == null) return NotFound(HandleResult.Text("Service not found."));
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

    }
}