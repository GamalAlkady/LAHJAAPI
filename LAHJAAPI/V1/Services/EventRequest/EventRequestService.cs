using AutoMapper;
using LAHJAAPI.Exceptions;
using LAHJAAPI.V1.Enums;
using V1.BPR.Layers.Base;
using V1.DyModels.Dso.Requests;
using V1.DyModels.Dso.Responses;
using V1.DyModels.Dto.Share.Requests;
using V1.DyModels.Dto.Share.Responses;
using V1.Repositories.Share;

namespace V1.Services.Services
{
    public class EventRequestService : BaseBPRServiceLayer<EventRequestRequestDso, EventRequestResponseDso, EventRequestRequestShareDto, EventRequestResponseShareDto>, IUseEventRequestService
    {
        private readonly IUseRequestService _requestService;
        private readonly IEventRequestShareRepository _share;
        public EventRequestService(
            IMapper mapper,
            ILoggerFactory logger,
            IUseRequestService requestService,
            IEventRequestShareRepository bpr) : base(mapper, logger, bpr)
        {
            _requestService = requestService;
            _share = bpr;
        }


        public override async Task<EventRequestResponseDso> CreateAsync(EventRequestRequestDso requestDso)
        {

            var eventRequest = await _share.GetByIdAsync(requestDso.Id);
            if (eventRequest == null) throw new ProblemDetailsException(HandleResult.Problem("Event request not found", "EventId not found."));

            if (eventRequest.Status != RequestStatus.Created.GetDisplayName() && eventRequest.Status != RequestStatus.Retry.GetDisplayName())
                throw new ProblemDetailsException(HandleResult.Problem("Event request not acceptable.", "Only event with status Success or Retry are acceptable."));

            var requestId = eventRequest.RequestId;
            var request = await _requestService.GetByIdAsync(requestId);

            if (request.Status == RequestStatus.Success.ToString())
                throw new ProblemDetailsException(HandleResult.Problem("Create Event", "This request has completed."));

            request.Status = requestDso.Status.ToString();
            request.UpdatedAt = DateTime.UtcNow;

            var newEventRequest = new EventRequestRequestDso
            {
                Status = RequestStatus.Success.GetDisplayName(),
                RequestId = requestId,
            };

            if (requestDso.Status == RequestStatus.Success.GetDisplayName())
            {
                request.Answer = requestDso.Details;
                newEventRequest.Details = $"Request has been completed for {request.ModelGateway}.";
            }
            else newEventRequest.Details = requestDso.Details;

            //await _requestService.ExecuteTransactionAsync(async () =>
            //{
            //await _eventRequestService.CreateAsync(newEventRequest);
            await _share.CreateAsync(newEventRequest);
            //var requestVm = _mapper.Map<RequestOutputVM>(request);
            var requestRequest = _mapper.Map<RequestRequestDso>(request);
            //requestRequest.Events.Add(newEventRequest);
            await _requestService.UpdateAsync(requestRequest);
            return MapToResponse(newEventRequest);

            //}
            //catch (ProblemDetailsException ex)
            //{
            //    _logger.LogError(ex, "Error while creating a new Request");
            //    return BadRequest(HandleResult.Problem(ex.Problem));
            //}

            //catch (Exception ex)
            //{
            //    _logger.LogError(ex, "Error while creating a new Request");
            //    return BadRequest(HandleResult.Problem(ex));
            //}
        }
    }
}