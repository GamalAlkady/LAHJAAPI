using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using V1.BPR.Layers.Base;
using V1.DyModels.Dso.Requests;
using V1.DyModels.Dso.Responses;
using V1.DyModels.VMs;
using V1.Services.Services;

namespace V1.Controllers.Api
{
    //[ApiExplorerSettings(GroupName = "V1")]
    [Route("api/V1/user/[controller]")]
    [ApiController]
    public class EventRequestController : BaseBPRControllerLayer<EventRequestRequestDso, EventRequestResponseDso, EventRequestCreateVM, EventRequestOutputVM, EventRequestUpdateVM, EventRequestInfoVM, EventRequestDeleteVM, EventRequestFilterVM>
    {
        private readonly IUseEventRequestService _service;
        public EventRequestController(IMapper mapper, ILoggerFactory logger, IUseEventRequestService bPR) : base(mapper, logger, bPR)
        {
            _service = bPR;
        }

        [ServiceFilter(typeof(SubscriptionCheckFilter))]
        [EndpointSummary("Create Event")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public override async Task<ActionResult<EventRequestOutputVM>> CreateAsync([FromBody] EventRequestCreateVM model)
        {
            var eventRequest = await _service.CreateAsync(new EventRequestRequestDso
            {
                Id = model.EventId,
                Status = model.Status.GetDisplayName(),
                Details = model.Details,
            });

            return Ok(_mapper.Map<EventRequestOutputVM>(eventRequest));
        }
    }
}