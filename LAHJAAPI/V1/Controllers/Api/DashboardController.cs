using LAHJAAPI.V1.Enums;
using LAHJAAPI.V1.Services.Statistics;
using Microsoft.AspNetCore.Mvc;
using V1.DyModels.VM;


namespace Api.Controllers
{
    //[ApiExplorerSettings(GroupName = "User")]
    [ServiceFilter(typeof(SubscriptionCheckFilter))]
    [ApiController]
    [Route("api/v1/user/[controller]")]
    //[OutputCache(PolicyName = "CustomPolicy", Tags = new[] { "requests" })]
    public class DashboardController(
        IStatisticsService statisticsService,
        ILogger<DashboardController> logger
        ) : ControllerBase
    {
        [EndpointSummary("Service Usage Data")]
        [HttpGet("ServiceUsageData")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<UsedRequestsVm>>> ServiceUsageDataAsync()
        {
            var items = await statisticsService.GetServiceUsageDataAsync();
            return Ok(items);
        }

        [EndpointSummary("Service Users Count")]
        [HttpGet("ServiceUsersCount")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<ServiceUsersCount>>> ServiceUsersCount()
        {
            var items = await statisticsService.ServiceUsersCount();
            return Ok(items);
        }

        [EndpointSummary("Service Usage And Remaining")]
        [HttpGet("ServiceUsageAndRemaining")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<UsedRequestsVm>>> GetServiceUsageAndRemaining()
        {
            var items = await statisticsService.GetServiceUsageAndRemaining();
            return Ok(items);
        }

        [EndpointSummary("Usage And Remaining Requests")]
        [HttpGet("UsageAndRemainingRequests")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UsedRequestsVm>> UsageAndRemainingRequests()
        {
            var items = await statisticsService.GetUsageAndRemainingRequests();
            return Ok(items);
        }

        [HttpGet("GetRequests")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<RequestData>>> GetRequests(FilterBy filterBy, DateTime? startDate, DateTime? endDate, RequestType requestType = RequestType.All)
        {
            var items = await statisticsService.GetRequests(filterBy, requestType, startDate, endDate);
            return Ok(items);
        }

        [HttpGet("GetRequestsByDatetime")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<RequestData>>> GetRequestsByDatetime(FilterBy filterBy, DateTime? startDate, DateTime? endDate, RequestType requestType = RequestType.All, DateTimeFilter groupBy = DateTimeFilter.Day)
        {
            var items = await statisticsService.GetRequestsByDatetime(filterBy, requestType, startDate, endDate, groupBy);
            return Ok(items);
        }


        [HttpGet("GetRequestsByStatus")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<ServiceDataTod>>> GetRequestsByStatus(FilterBy filterBy, DateTime? startDate, DateTime? endDate, RequestType requestType = RequestType.All)
        {
            var items = await statisticsService.GetRequestsByStatus(filterBy, requestType, startDate, endDate);
            return Ok(items);
        }

        [HttpGet("ModelAiServiceRequests")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<ModelAiServiceData>>> ModelAiServiceRequests(string? lg = null)
        {
            var items = await statisticsService.GetModelAiServicesRequests(lg);
            return Ok(items);
        }

    }
}
