using APILAHJA.Utilities;
using AutoGenerator;
using AutoGenerator.Helper;
using AutoGenerator.Services.Base;
using AutoMapper;
using LAHJAAPI.V1.Enums;
using LAHJAAPI.V1.Validators;
using LAHJAAPI.V1.Validators.Conditions;
using V1.DyModels.Dso.Requests;
using V1.DyModels.Dso.ResponseFilters;
using V1.DyModels.Dso.Responses;
using V1.DyModels.Dto.Share.Requests;
using V1.Repositories.Share;

namespace V1.Services.Services
{
    public class SubscriptionService : BaseService<SubscriptionRequestDso, SubscriptionResponseDso>, IUseSubscriptionService
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
            ILoggerFactory logger) : base(mapper, logger)
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
                Subscription = GetMapper().Map<SubscriptionResponseDso>(subscription);
                return Subscription;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating Subscription entity.");
                throw;
            }
        }
        public async Task<SubscriptionResponseDso> GetCustomerSubscription(string subscriptionId)
        {
            var customerId = _userClaims.CustomerId ?? (await GetUserSubscription()).CustomerId
                ?? throw new ArgumentException("Customer id is null.");

            var customer = await GetOneByAsync([
                new FilterCondition { PropertyName = "CustomerId",Value = customerId},
                new FilterCondition { PropertyName = "Id",Value = subscriptionId},
            ]);
            return GetMapper().Map<SubscriptionResponseDso>(customer);
        }

        public async Task<PagedResponse<SubscriptionResponseDso>> GetCustomerSubscriptions()
        {
            var customerId = _userClaims.CustomerId ?? (await GetUserSubscription()).CustomerId
                ?? throw new ArgumentException("Customer id is null.");

            var response = await GetAllByAsync([
                new FilterCondition { PropertyName = "CustomerId",Value = customerId}
            ]);
            if (response.TotalRecords == 0)
                return response.ToResponse(new List<SubscriptionResponseDso>());
            return response.ToResponse(GetMapper().Map<IEnumerable<SubscriptionResponseDso>>(response.Data));
        }

        public async Task<int> GetNumberRequests(string? subsriptionId = null, bool refresh = true)
        {
            if (NumberRequests != 0 && !refresh) return NumberRequests;
            await GetUserSubscription(subsriptionId);
            //NumberRequests = await _requestRepository.GetCount(Subscription.Id, null, Subscription.CurrentPeriodStart, Subscription.CurrentPeriodEnd, RequestStatus.Success.ToString());

            var response = await _requestRepository.GetAllByAsync([
             new FilterCondition("SubscriptionId", Subscription.Id),
                new FilterCondition("Status", new[]{RequestStatus.Processing.ToString(),RequestStatus.Success.ToString() },FilterOperator.In),
                new FilterCondition("UpdatedAt", Subscription.CurrentPeriodStart,FilterOperator.GreaterThanOrEqual),
                new FilterCondition("UpdatedAt", Subscription.CurrentPeriodEnd,FilterOperator.LessThanOrEqual)
                 ], new ParamOptions(1, Subscription.AllowedRequests));

            NumberRequests = response.TotalRecords;
            return NumberRequests;
        }

        public async Task<int> AllowedRequests()
        {
            NumberRequests = await GetNumberRequests();
            return Subscription.AllowedRequests - NumberRequests;
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
            return response.ToResponse(GetMapper().Map<IEnumerable<SpaceResponseDso>>(response.Data));
        }

        public async Task<SpaceResponseDso> GetSpace(string spaceId, string? subscriptionId = null)
        {
            subscriptionId ??= (await GetUserSubscription()).Id;
            var space = await _spaceRepository.GetOneByAsync([
                    new FilterCondition("Id",spaceId),
                new FilterCondition("SubscriptionId",subscriptionId),
                ]);
            return GetMapper().Map<SpaceResponseDso>(space);
        }

        public async Task<bool> IsSpaceFound(string spaceId, string? subscriptionId = null)
        {

            var space = await GetSpace(spaceId, subscriptionId);
            return space is not null;
        }

        public async Task<int> AvailableSpace(string? subscriptionId = null)
        {
            await GetUserSubscription(subscriptionId);
            var pagedResponse = await GetSpaces(subscriptionId);

            return Subscription.AllowedSpaces - pagedResponse.TotalRecords;
        }


        public async Task<bool> IsActive(string? subscriptionId = null)
        {
            await GetUserSubscription(subscriptionId);

            return _checker.Check(SubscriptionValidatorStates.IsActive, new SubscriptionResponseFilterDso
            {
                Status = Subscription.Status,
            });
        }

        public async Task<bool> IsCanceled(string? subscriptionId = null)
        {
            await GetUserSubscription(subscriptionId);
            return _checker.Check(SubscriptionValidatorStates.IsCanceled, new SubscriptionResponseFilterDso
            {
                Status = Subscription.Status,
            });
        }

        public async Task<bool> IsCancelAtPeriodEnd(string? subscriptionId = null)
        {
            await GetUserSubscription(subscriptionId);
            return _checker.Check(SubscriptionValidatorStates.IsCancelAtPeriodEnd, new SubscriptionResponseFilterDso
            {
                CancelAtPeriodEnd = Subscription.CancelAtPeriodEnd,
            });
        }


        public async Task<(bool IsNotSubscribed, object? Result)> IsNotSubscribe(string? subscriptionId = null)
        {
            await GetUserSubscription(subscriptionId);
            var result = _checker.CheckAndResult(SubscriptionValidatorStates.IsNotSubscribe,
                new SubscriptionResponseFilterDso
                {
                    Status = Subscription.Status,
                    CancelAtPeriodEnd = Subscription.CancelAtPeriodEnd,
                });
            return (result.Result is not null, result.Result);
        }

        public async Task<bool> IsSubscribe(string? subscriptionId = null)
        {
            await GetUserSubscription(subscriptionId);
            return _checker.Check(SubscriptionValidatorStates.IsSubscribe,
                new SubscriptionResponseFilterDso
                {
                    Status = Subscription.Status,
                    CancelAtPeriodEnd = Subscription.CancelAtPeriodEnd,
                });
        }

        #endregion

        public async override Task<int> CountAsync()
        {
            try
            {
                _logger.LogInformation("Counting Subscription entities...");
                return await _share.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CountAsync for Subscription entities.");
                return await Task.FromResult(0);
            }
        }

        public override async Task<SubscriptionResponseDso> CreateAsync(SubscriptionRequestDso entity)
        {
            try
            {
                _logger.LogInformation("Creating new Subscription entity...");
                var result = await _share.CreateAsync(entity);
                var output = GetMapper().Map<SubscriptionResponseDso>(result);
                _logger.LogInformation("Created Subscription entity successfully.");
                return output;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating Subscription entity.");
                return null;
            }
        }

        public override Task DeleteAsync(string id)
        {
            try
            {
                _logger.LogInformation($"Deleting Subscription entity with ID: {id}...");
                return _share.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while deleting Subscription entity with ID: {id}.");
                return Task.CompletedTask;
            }
        }

        public override async Task<IEnumerable<SubscriptionResponseDso>> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("Retrieving all Subscription entities...");
                var results = await _share.GetAllAsync();
                return GetMapper().Map<IEnumerable<SubscriptionResponseDso>>(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAllAsync for Subscription entities.");
                return null;
            }
        }

        public override async Task<SubscriptionResponseDso?> GetByIdAsync(string id)
        {
            try
            {
                _logger.LogInformation($"Retrieving Subscription entity with ID: {id}...");
                var result = await _share.GetByIdAsync(id);
                var item = GetMapper().Map<SubscriptionResponseDso>(result);
                _logger.LogInformation("Retrieved Subscription entity successfully.");
                return item;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in GetByIdAsync for Subscription entity with ID: {id}.");
                return null;
            }
        }

        public override IQueryable<SubscriptionResponseDso> GetQueryable()
        {
            try
            {
                _logger.LogInformation("Retrieving IQueryable<SubscriptionResponseDso> for Subscription entities...");
                var queryable = _share.GetQueryable();
                var result = GetMapper().ProjectTo<SubscriptionResponseDso>(queryable);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetQueryable for Subscription entities.");
                return null;
            }
        }

        public override async Task<SubscriptionResponseDso> UpdateAsync(SubscriptionRequestDso entity)
        {
            try
            {
                _logger.LogInformation("Updating Subscription entity...");
                var result = await _share.UpdateAsync(entity);
                return GetMapper().Map<SubscriptionResponseDso>(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateAsync for Subscription entity.");
                return null;
            }
        }

        public override async Task<bool> ExistsAsync(object value, string name = "Id")
        {
            try
            {
                _logger.LogInformation("Checking if Subscription exists with {Key}: {Value}", name, value);
                var exists = await _share.ExistsAsync(value, name);
                if (!exists)
                {
                    _logger.LogWarning("Subscription not found with {Key}: {Value}", name, value);
                }

                return exists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while checking existence of Subscription with {Key}: {Value}", name, value);
                return false;
            }
        }

        public override async Task<PagedResponse<SubscriptionResponseDso>> GetAllAsync(string[]? includes = null, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                _logger.LogInformation("Fetching all Subscriptions with pagination: Page {PageNumber}, Size {PageSize}", pageNumber, pageSize);
                var results = (await _share.GetAllAsync(includes, pageNumber, pageSize));
                var items = GetMapper().Map<List<SubscriptionResponseDso>>(results.Data);
                return new PagedResponse<SubscriptionResponseDso>(items, results.PageNumber, results.PageSize, results.TotalPages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching all Subscriptions.");
                return new PagedResponse<SubscriptionResponseDso>(new List<SubscriptionResponseDso>(), pageNumber, pageSize, 0);
            }
        }

        public override async Task<SubscriptionResponseDso?> GetByIdAsync(object id)
        {
            try
            {
                _logger.LogInformation("Fetching Subscription by ID: {Id}", id);
                var result = await _share.GetByIdAsync(id);
                if (result == null)
                {
                    _logger.LogWarning("Subscription not found with ID: {Id}", id);
                    return null;
                }

                _logger.LogInformation("Retrieved Subscription successfully.");
                return GetMapper().Map<SubscriptionResponseDso>(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while retrieving Subscription by ID: {Id}", id);
                return null;
            }
        }

        public override async Task DeleteAsync(object value, string key = "Id")
        {
            try
            {
                _logger.LogInformation("Deleting Subscription with {Key}: {Value}", key, value);
                await _share.DeleteAsync(value, key);
                _logger.LogInformation("Subscription with {Key}: {Value} deleted successfully.", key, value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting Subscription with {Key}: {Value}", key, value);
            }
        }

        public override async Task DeleteRange(List<SubscriptionRequestDso> entities)
        {
            try
            {
                var builddtos = entities.OfType<SubscriptionRequestShareDto>().ToList();
                _logger.LogInformation("Deleting {Count} Subscriptions...", 201);
                await _share.DeleteRange(builddtos);
                _logger.LogInformation("{Count} Subscriptions deleted successfully.", 202);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting multiple Subscriptions.");
            }
        }

        public override async Task<PagedResponse<SubscriptionResponseDso>> GetAllByAsync(List<FilterCondition> conditions, ParamOptions? options = null)
        {
            try
            {
                _logger.LogInformation("Retrieving all Subscription entities...");
                var response = await _share.GetAllByAsync(conditions, options);
                return response.ToResponse(GetMapper().Map<IEnumerable<SubscriptionResponseDso>>(response.Data));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAllAsync for Subscription entities.");
                return null;
            }
        }

        public override async Task<SubscriptionResponseDso?> GetOneByAsync(List<FilterCondition> conditions, ParamOptions? options = null)
        {
            try
            {
                _logger.LogInformation("Retrieving Subscription entity...");
                return GetMapper().Map<SubscriptionResponseDso>(await _share.GetOneByAsync(conditions, options));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetOneByAsync  for Subscription entity.");
                return null;
            }
        }
    }
}