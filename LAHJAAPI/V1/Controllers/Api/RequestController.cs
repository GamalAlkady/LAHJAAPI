using AutoMapper;
using LAHJAAPI.Exceptions;
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
    public class RequestController : BaseBPRControllerLayer<RequestRequestDso, RequestResponseDso, RequestCreateVM, RequestOutputVM, RequestInfoVM, RequestDeleteVM>
    {

        private readonly ILogger _logger;
        private readonly IUseRequestService _requestService;

        public RequestController(
            IUseRequestService requestService,
            IMapper mapper, ILoggerFactory logger) : base(mapper, logger, requestService)
        {
            _logger = logger.CreateLogger(typeof(RequestController).FullName);
            _requestService = requestService;
        }


        // Create a new Request.
        [ServiceFilter(typeof(SubscriptionCheckFilter))]
        public override async Task<ActionResult<RequestOutputVM>> CreateAsync([FromBody] RequestCreateVM model)
        {
            try
            {
                return await base.CreateAsync(model);
            }
            catch (ProblemDetailsException ex)
            {
                _logger.LogError(ex, "Error when create request");
                return BadRequest(HandleResult.Problem(ex.Problem));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when create request");
                return BadRequest(HandleResult.Error(ex.Message));
            }
        }


        // Create a new Request.
        //[HttpPost(Name = "CreateRequest")]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[ProducesResponseType(StatusCodes.Status402PaymentRequired)]
        //public async Task<ActionResult<RequestFilterVM>> Create([FromBody] RequestCreateVM model)
        //{
        //    try
        //    {
        //        if (_userClaims.ServicesIds == null || !_userClaims.ServicesIds.Any(x => x == model.ServiceId))
        //        {
        //            return StatusCode(StatusCodes.Status402PaymentRequired, HandleResult.NotFound("No service found in this token"));

        //        }
        //        var requestFilter = new DataFilter
        //        {
        //            Id = _userClaims.SubscriptionId, // Check if subscription id in token
        //            Items = new Dictionary<string, object> {
        //                {"serviceId", model.ServiceId }, // Check if service id in token
        //                {"spaceId", model.SpaceId }, // Check if space is found
        //                {"sessionId", _userClaims.SessionId }, // Check if space is found
        //            },
        //        };
        //        var result = await _checker.CheckAndResultAsync(SubscriptionValidatorStates.IsAllowedRequestsForCreate, requestFilter);
        //        if (result.Success == false)
        //        {
        //            if (result.Result is ProblemDetails problem)
        //                return StatusCode(StatusCodes.Status402PaymentRequired, problem);
        //            return StatusCode(StatusCodes.Status402PaymentRequired, result.Result ?? result.Message);
        //        }
        //        var subscriptionFilter = (SubscriptionFilterVM)result.Result!;

        //        var service = await _serviceService.GetOneByAsync(
        //        [new FilterCondition("Id", model.ServiceId)],
        //        new ParamOptions(["ModelAi.ModelGateway"]));

        //        if (service == null)
        //            return NotFound(HandleResult.Problem("Create request", "This service not found."));
        //        var modelAi = service.ModelAi;
        //        var modelGateway = modelAi.ModelGateway;

        //        RequestRequestDso request = new()
        //        {
        //            Status = RequestStatus.Processing.ToString(),
        //            Question = model.Question,
        //            ModelGateway = modelGateway.Url,
        //            ModelAi = modelAi.AbsolutePath,
        //            UserId = _userClaims.UserId,
        //            ServiceId = service.Id,
        //            SpaceId = model.SpaceId,
        //            SubscriptionId = subscriptionFilter.Id
        //        };


        //        var coreUrl = $" {request.ModelGateway}/{service.AbsolutePath}";
        //        var eventRequest = new EventRequestRequestDso()
        //        {
        //            Status = RequestStatus.Created.GetDisplayName(),
        //            RequestId = request.Id,
        //            Details = $"Request has been created for {coreUrl}."
        //        };
        //        request.Events.Add(eventRequest);

        //        _logger.LogInformation("Creating new Request with data: {@model}", request);
        //        await _requestService.CreateAsync(request);

        //        return Ok(new RequestFilterVM
        //        {
        //            ModelGateway = request.ModelGateway,
        //            ModelAi = request.ModelAi,
        //            Service = service.AbsolutePath,
        //            Token = service.Token,
        //            EventId = eventRequest.Id ?? request.Id,
        //            AllowedRequests = subscriptionFilter.AllowedRequests,
        //            NumberRequests = subscriptionFilter.NumberRequests,
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error while creating a new Request");
        //        return StatusCode(500, "Internal Server Error");
        //    }
        //}


        //[ServiceFilter(typeof(SubscriptionCheckFilter))]
        //[EndpointSummary("Create Event")]
        //[HttpPost("CreateEvent")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //public async Task<ActionResult<EventRequestOutputVM>> CreateEvent(EventRequestCreateVM eventRequestCreate)
        //{
        //    //var requestId = "";
        //    try
        //    {
        //        //var subscription = await subscriptionRepository.GetSubscription();

        //        var eventRequest = await _eventRequestService.GetByIdAsync(eventRequestCreate.EventId);
        //        if (eventRequest == null) return NotFound(HandleResult.Problem("Event request not found", "EventId not found."));

        //        if (eventRequest.Status != RequestStatus.Created.GetDisplayName() || eventRequest.Status != RequestStatus.Retry.GetDisplayName())
        //            return BadRequest(HandleResult.Problem("Event request not acceptable.", "Only event with status Success or Retry are acceptable."));

        //        var requestId = eventRequest.RequestId;
        //        var request = await _requestService.GetByIdAsync(requestId);

        //        if (request.Status == RequestStatus.Success.ToString())
        //            return BadRequest(HandleResult.Problem("Create Event", "This request has completed."));

        //        request.Status = eventRequestCreate.Status.ToString();
        //        request.UpdatedAt = DateTime.UtcNow;

        //        var newEventRequest = new EventRequestRequestDso
        //        {
        //            Status = RequestStatus.Success.GetDisplayName(),
        //            RequestId = requestId,
        //        };

        //        if ((int)eventRequestCreate.Status == (int)RequestStatus.Success)
        //        {
        //            request.Answer = eventRequestCreate.Details;
        //            newEventRequest.Details = $"Request has been completed for {request.ModelGateway}.";
        //        }
        //        else newEventRequest.Details = eventRequestCreate.Details;

        //        //await _requestService.ExecuteTransactionAsync(async () =>
        //        //{
        //        //await _eventRequestService.CreateAsync(newEventRequest);
        //        await _eventRequestService.CreateAsync(newEventRequest);
        //        var requestVm = _mapper.Map<RequestOutputVM>(request);
        //        var requestRequest = _mapper.Map<RequestRequestDso>(requestVm);
        //        //requestRequest.Events.Add(newEventRequest);
        //        await _requestService.UpdateAsync(requestRequest);
        //        //    return true;

        //        //});
        //        return Ok(_mapper.Map<EventRequestOutputVM>(newEventRequest));

        //        //return BadRequest(HandelErrors.Problem("Create Event", "Transaction failed."));
        //    }
        //    catch (ProblemDetailsException ex)
        //    {
        //        _logger.LogError(ex, "Error while creating a new Request");
        //        return BadRequest(HandleResult.Problem(ex.Problem));
        //    }

        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error while creating a new Request");
        //        return BadRequest(HandleResult.Problem(ex));
        //    }
        //}

    }
}