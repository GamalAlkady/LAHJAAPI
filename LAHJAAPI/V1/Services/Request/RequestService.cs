using APILAHJA.Utilities;
using AutoGenerator.Helper;
using AutoMapper;
using LAHJAAPI.Exceptions;
using LAHJAAPI.V1.Enums;
using LAHJAAPI.V1.Validators;
using LAHJAAPI.V1.Validators.Conditions;
using V1.BPR.Layers.Base;
using V1.DyModels.Dso.Requests;
using V1.DyModels.Dso.Responses;
using V1.DyModels.Dto.Share.Requests;
using V1.DyModels.Dto.Share.Responses;
using V1.DyModels.VMs;
using V1.Repositories.Share;
using WasmAI.ConditionChecker.Base;

namespace V1.Services.Services
{
    public class RequestService : BaseBPRServiceLayer<RequestRequestDso, RequestResponseDso, RequestRequestShareDto, RequestResponseShareDto>, IUseRequestService
    {

        private readonly IConditionChecker _checker;
        private readonly IUseServiceService _serviceService;
        private readonly IUserClaimsHelper _userClaims;
        public RequestService(
            IMapper mapper,
            ILoggerFactory logger,
            IRequestShareRepository bpr,
            IConditionChecker checker,
            IUseServiceService serviceService,
            IUserClaimsHelper userClaims) : base(mapper, logger, bpr)
        {

            _checker = checker;
            _serviceService = serviceService;
            _userClaims = userClaims;
        }

        public override async Task<RequestResponseDso> CreateAsync(RequestRequestDso entity)
        {
            try
            {
                if (_userClaims.ServicesIds == null || !_userClaims.ServicesIds.Any(x => x == entity.ServiceId))
                {
                    throw new Exception("No service found in this token");

                }

                var requestFilter = new DataFilter
                {
                    Id = _userClaims.SubscriptionId, // Check if subscription id in token
                    Items = new Dictionary<string, object> {
                        {"serviceId", entity.ServiceId }, // Check if service id in token
                        {"spaceId", entity.SpaceId }, // Check if space is found
                        {"sessionId", _userClaims.SessionId }, // Check if space is found
                    },
                };

                var result = await _checker.CheckAndResultAsync(SubscriptionValidatorStates.IsFullAllowedRequests, requestFilter);
                if (result.Success == false)
                {
                    throw new ProblemDetailsException(result);
                }

                var subscriptionFilter = (SubscriptionFilterVM)result.Result!;


                var service = await _serviceService.GetOneByAsync(
                [new FilterCondition("Id", entity.ServiceId)],
                new ParamOptions(["ModelAi.ModelGateway"])) ?? throw new Exception("This service not found.");

                var modelAi = service.ModelAi;
                var modelGateway = modelAi.ModelGateway;

                entity.Status = RequestStatus.Processing.ToString();
                entity.ModelGateway = modelGateway.Url;
                entity.ModelAi = modelAi.AbsolutePath;
                entity.UserId = _userClaims.UserId;
                entity.SubscriptionId = subscriptionFilter.Id;

                var coreUrl = $" {entity.ModelGateway}/{service.AbsolutePath}";
                var eventRequest = new EventRequestRequestDso()
                {
                    Status = RequestStatus.Created.GetDisplayName(),
                    RequestId = entity.Id,
                    Details = $"Request has been created for {coreUrl}."
                };
                entity.Events.Add(eventRequest);

                _logger.LogInformation("Creating new Request with data: {@model}", entity);
                var request = await base.CreateAsync(entity);


                request.EventId = eventRequest.Id;
                request.NumberRequests = subscriptionFilter.NumberRequests + 1;
                request.AllowedRequests = subscriptionFilter.AllowedRequests;

                return request;
            }
            catch (ProblemDetailsException)
            {
                throw;
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}