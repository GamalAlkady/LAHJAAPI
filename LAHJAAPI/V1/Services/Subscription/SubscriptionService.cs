using APILAHJA.Utilities;
using AutoGenerator;
using AutoGenerator.Helper;
using AutoMapper;
using LAHJAAPI.Exceptions;
using LAHJAAPI.V1.Validators;
using LAHJAAPI.V1.Validators.Conditions;
using V1.BPR.Layers.Base;
using V1.DyModels.Dso.Requests;
using V1.DyModels.Dso.Responses;
using V1.DyModels.Dto.Share.Requests;
using V1.DyModels.Dto.Share.Responses;
using V1.DyModels.VMs;
using V1.Repositories.Share;

namespace V1.Services.Services
{
    public class SubscriptionService : BaseBPRServiceLayer<SubscriptionRequestDso, SubscriptionResponseDso, SubscriptionRequestShareDto, SubscriptionResponseShareDto>, IUseSubscriptionService
    {
        private readonly ISubscriptionShareRepository _share;
        private readonly ISpaceShareRepository _spaceRepository;
        private readonly IRequestShareRepository _requestRepository;
        private readonly IUserClaimsHelper _userClaims;
        private readonly IConditionChecker _checker;

        public SubscriptionService(
            ISubscriptionShareRepository buildSubscriptionShareRepository,
            ISpaceShareRepository spaceRepository,
            IRequestShareRepository requestRepository,
            IUserClaimsHelper userClaims,
            IMapper mapper,
            IConditionChecker checker,
            ILoggerFactory logger) : base(mapper, logger, buildSubscriptionShareRepository)
        {
            _share = buildSubscriptionShareRepository;
            _spaceRepository = spaceRepository;
            _requestRepository = requestRepository;
            _userClaims = userClaims;
            _checker = checker;
        }

        #region My Functions
        private SubscriptionResponseDso Subscription { get; set; }
        private int NumberRequests { get; set; }

        public async Task<SubscriptionResponseDso> GetUserSubscription(string? subscriptionId = null)
        {
            try
            {
                subscriptionId ??= _userClaims.SubscriptionId;
                //userId ??= _userClaims.UserId;
                if (Subscription != null && Subscription.Id == subscriptionId) return Subscription;

                _logger.LogInformation($"Retrieving user Subscription entity.");
                //var filter = new List<FilterCondition> { new FilterCondition { PropertyName = "UserId", Value = _userClaims.UserId } };
                var filter = new List<FilterCondition>();
                if (!string.IsNullOrEmpty(subscriptionId))
                {
                    filter.Add(new FilterCondition { PropertyName = "Id", Value = subscriptionId });
                }
                //else
                //{
                //    filter.Add(new FilterCondition { PropertyName = "UserId", Value = userId });
                //}
                var subscription = await _share.GetOneByAsync(filter)
                                       ?? throw new ArgumentNullException("You have no subscription");

                _logger.LogInformation("Retrieved user subscription entity successfully.");
                Subscription = MapToResponse(subscription);
                return Subscription;
            }
            catch (ArgumentNullException) { return null; }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating Subscription entity.");
                throw;
            }
        }
        //public async Task<SubscriptionResponseDso> GetSubscriptionByCustomer(string subscriptionId, string? customerId = null)
        //{
        //    customerId ??= _userClaims.CustomerId ?? (await GetUserSubscription()).CustomerId
        //       ?? throw new ArgumentException("Customer id is null.");

        //    var subscription = await GetOneByAsync([
        //        new FilterCondition { PropertyName = "CustomerId",Value = customerId},
        //        new FilterCondition { PropertyName = "Id",Value = subscriptionId},
        //    ]);
        //    return MapToResponse(subscription);
        //}

        public async Task<PagedResponse<SubscriptionResponseDso>> GetSubscriptionsByCustomer(string? customerId = null)
        {
            customerId ??= _userClaims.CustomerId ?? (await GetUserSubscription()).CustomerId
               ?? throw new ArgumentException("Customer id is null.");

            var response = await GetAllByAsync([
                new FilterCondition { PropertyName = "CustomerId",Value = customerId}
            ]);
            if (response.TotalRecords == 0)
                return response.ToResponse(new List<SubscriptionResponseDso>());
            return response.ToResponse(MapToResponses(response.Data));
        }
        SubscriptionFilterVM _subscriptionFilterVM;

        public async Task<int> GetNumberRequests(bool refresh = true, string? subscriptionId = null)
        {
            subscriptionId ??= _userClaims.SubscriptionId;
            if (_subscriptionFilterVM != null && refresh) return _subscriptionFilterVM.NumberRequests;
            var result = await _checker.CheckAndResultAsync(SubscriptionValidatorStates.IsAllowedRequests, subscriptionId);
            if (result.Result is SubscriptionFilterVM subscriptionFilter)
            {
                _subscriptionFilterVM = subscriptionFilter;
                NumberRequests = subscriptionFilter.NumberRequests;
                return NumberRequests;
            }

            throw new ProblemDetailsException(result);

            //if (NumberRequests != 0 && !refresh) return NumberRequests;
            //await GetUserSubscription(subsriptionId);
            ////NumberRequests = await _requestRepository.GetCount(Subscription.Id, null, Subscription.CurrentPeriodStart, Subscription.CurrentPeriodEnd, RequestStatus.Success.ToString());

            //var response = await _requestRepository.GetAllByAsync([
            // new FilterCondition("SubscriptionId", Subscription.Id),
            //    new FilterCondition("Status", new[]{RequestStatus.Processing.ToString(),RequestStatus.Success.ToString() },FilterOperator.In),
            //    new FilterCondition("UpdatedAt", Subscription.CurrentPeriodStart,FilterOperator.GreaterThanOrEqual),
            //    new FilterCondition("UpdatedAt", Subscription.CurrentPeriodEnd,FilterOperator.LessThanOrEqual)
            //     ], new ParamOptions(1, Subscription.AllowedRequests));

            //NumberRequests = response.TotalRecords;
            //return NumberRequests;
        }

        public async Task<string?> AllowedRequests(bool refresh = true, string? subscriptionId = null)
        {
            if (_subscriptionFilterVM == null || refresh)
                await GetNumberRequests(refresh, subscriptionId);
            return _subscriptionFilterVM!.AllowedRequests;
        }

        //public async Task<bool> IsFree(string? userId = null)
        //{
        //    userId ??= _userClaims.UserId;
        //    await GetUserSubscription();
        //    var plan = await _spaceShareRepository.GetByIdAsync(Subscription.PlanId!, "en");
        //    return plan.Amount == 0;
        //}

        public async Task<PagedResponse<SpaceResponseDso>> GetSpaces(string? subscriptionId = null)
        {
            subscriptionId ??= _userClaims.SubscriptionId ?? (await GetUserSubscription()).Id;
            var response = await _spaceRepository.GetAllByAsync(
                [new FilterCondition("SubscriptionId", subscriptionId)], new ParamOptions { PageSize = 100 });
            return response.ToResponse(Map<object, SpaceResponseDso>(response.Data));
        }

        public async Task<SpaceResponseDso> GetSpace(string spaceId, string? subscriptionId = null)
        {
            subscriptionId ??= (await GetUserSubscription()).Id;
            var space = await _spaceRepository.GetOneByAsync([
                    new FilterCondition("Id",spaceId),
                new FilterCondition("SubscriptionId",subscriptionId),
                ]);
            return Map<SpaceResponseShareDto, SpaceResponseDso>(space);
        }

        //public async Task<bool> IsSpaceFound(string spaceId, string? subscriptionId = null)
        //{
        //    var space = await GetSpace(spaceId, subscriptionId);
        //    return space is not null;
        //}

        //public async Task<int> AvailableSpace(string? subscriptionId = null)
        //{
        //    await GetUserSubscription(subscriptionId);
        //    var pagedResponse = await GetSpaces(subscriptionId);

        //    return Subscription.AllowedSpaces - pagedResponse.TotalRecords;
        //}


        public async Task<bool> IsActive(string? subscriptionId = null)
        {
            subscriptionId ??= _userClaims.SubscriptionId;
            return await _checker.CheckAsync(SubscriptionValidatorStates.IsActive, subscriptionId);
        }

        public async Task<bool> IsCanceled(string? subscriptionId = null)
        {
            subscriptionId ??= _userClaims.SubscriptionId;
            return await _checker.CheckAsync(SubscriptionValidatorStates.IsCanceled, subscriptionId);
        }

        public async Task<bool> IsCancelAtPeriodEnd(string? subscriptionId = null)
        {
            subscriptionId ??= _userClaims.SubscriptionId;
            return await _checker.CheckAsync(SubscriptionValidatorStates.IsCancelAtPeriodEnd, subscriptionId);
        }
        public async Task<bool> IsSubscribe(string? subscriptionId = null)
        {
            subscriptionId ??= _userClaims.SubscriptionId;
            //await GetUserSubscription(subscriptionId);
            return await _checker.CheckAsync(SubscriptionValidatorStates.IsSubscribe, subscriptionId);
        }

        #endregion
    }
}