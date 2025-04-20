using AutoGenerator;
using AutoGenerator.Helper;
using AutoGenerator.Helper.Translation;
using AutoGenerator.Services.Base;
using AutoMapper;
using StripeGateway;
using V1.DyModels.Dso.Requests;
using V1.DyModels.Dso.Responses;
using V1.DyModels.Dto.Share.Requests;
using V1.Repositories.Share;

namespace V1.Services.Services
{
    public class PlanService : BaseService<PlanRequestDso, PlanResponseDso>, IUsePlanService
    {
        private readonly IPlanShareRepository _share;
        private readonly IStripeProduct _stripeProduct;
        private readonly IStripePrice _stripePrice;

        public PlanService(
            IPlanShareRepository buildPlanShareRepository,
            IStripeProduct stripeProduct,
            IStripePrice stripePrice,
            IMapper mapper,
            ILoggerFactory logger) : base(mapper, logger)
        {
            _share = buildPlanShareRepository;
            _stripeProduct = stripeProduct;
            _stripePrice = stripePrice;
        }

        public override Task<int> CountAsync()
        {
            try
            {
                _logger.LogInformation("Counting Plan entities...");
                return _share.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CountAsync for Plan entities.");
                return Task.FromResult(0);
            }
        }

        public override async Task<PlanResponseDso> CreateAsync(PlanRequestDso entity)
        {
            try
            {
                _logger.LogInformation("Creating new Plan entity...");
                Stripe.Product? product = null;
                if (string.IsNullOrEmpty(entity.ProductId))
                {
                    if (entity.ProductName?.Value == null)
                    {
                        throw new ArgumentException("ProductName is required.");
                    }

                    var options = new Stripe.ProductCreateOptions
                    {
                        Name = HelperTranslation.CoverTranslationDataToText(entity.ProductName)
                    };

                    if (entity?.Description?.Value != null)
                    {
                        options.Description = HelperTranslation.CoverTranslationDataToText(entity.Description);
                    }

                    if (entity?.Images.Count > 0)
                    {
                        options.Images = entity.Images;
                    }

                    product = await _stripeProduct.CreateAsync(options);

                    entity.ProductId = product.Id;
                }
                else
                {
                    product = await _stripeProduct.GetByIdAsync(entity.ProductId);
                    entity.ProductName = HelperTranslation.ConvertToTranslationData(product.Name);
                }


                if (entity.Amount < 0)
                {
                    throw new ArgumentException("Amount must be greater than zero.");
                }
                var price = await _stripePrice!.CreateAsync(new()
                {
                    Currency = entity.Currency,
                    UnitAmount = Convert.ToInt64(entity.Amount) * 100,
                    Recurring = new Stripe.PriceRecurringOptions { Interval = entity.BillingPeriod },
                    Product = product.Id
                });
                entity.Id = price.Id;

                var result = await _share.CreateAsync(entity);
                var output = GetMapper().Map<PlanResponseDso>(result);
                _logger.LogInformation("Created Plan entity successfully.");
                return output;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating Plan entity.");
                throw;
            }
        }

        public async Task<PlanResponseDso> SetPlanAsync(PlanRequestDso entity)
        {
            try
            {
                _logger.LogInformation("Creating new  Plan entity with price...");
                Stripe.Product? product = null;

                product = await _stripeProduct.GetByIdAsync(entity.ProductId);
                entity.ProductId = product.Id;
                entity.ProductName = HelperTranslation.ConvertToTranslationData(product.Name);
                if (product.Description != null)
                    entity.Description = HelperTranslation.ConvertToTranslationData(product.Description);

                var price = await _stripePrice!.GetByIdAsync(entity.Id);
                entity.Id = price.Id;
                entity.Amount = Convert.ToDouble(price.UnitAmount) / 100;
                entity.BillingPeriod = price.Recurring.Interval;
                entity.Currency = price.Currency;
                entity.Active = price.Active;

                var result = await _share.CreateAsync(entity);
                var output = GetMapper().Map<PlanResponseDso>(result);
                _logger.LogInformation("Created Plan entity successfully.");
                return output;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating Plan entity.");
                throw;
            }
        }

        public override Task DeleteAsync(string id)
        {
            try
            {
                _logger.LogInformation($"Deleting Plan entity with ID: {id}...");
                return _share.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while deleting Plan entity with ID: {id}.");
                return Task.CompletedTask;
            }
        }

        public override async Task<IEnumerable<PlanResponseDso>> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("Retrieving all Plan entities...");
                var results = await _share.GetAllAsync();
                return GetMapper().Map<IEnumerable<PlanResponseDso>>(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAllAsync for Plan entities.");
                return null;
            }
        }

        public override async Task<PlanResponseDso?> GetByIdAsync(string id)
        {
            try
            {
                _logger.LogInformation($"Retrieving Plan entity with ID: {id}...");
                var result = await _share.GetByIdAsync(id);
                var item = GetMapper().Map<PlanResponseDso>(result);
                _logger.LogInformation("Retrieved Plan entity successfully.");
                return item;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in GetByIdAsync for Plan entity with ID: {id}.");
                return null;
            }
        }

        public override IQueryable<PlanResponseDso> GetQueryable()
        {
            try
            {
                _logger.LogInformation("Retrieving IQueryable<PlanResponseDso> for Plan entities...");
                var queryable = _share.GetQueryable();
                var result = GetMapper().ProjectTo<PlanResponseDso>(queryable);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetQueryable for Plan entities.");
                return null;
            }
        }

        public override async Task<PlanResponseDso> UpdateAsync(PlanRequestDso entity)
        {
            try
            {
                _logger.LogInformation("Updating Plan entity...");
                var result = await _share.UpdateAsync(entity);
                return GetMapper().Map<PlanResponseDso>(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateAsync for Plan entity.");
                return null;
            }
        }

        public override async Task<bool> ExistsAsync(object value, string name = "Id")
        {
            try
            {
                _logger.LogInformation("Checking if Plan exists with {Key}: {Value}", name, value);
                var exists = await _share.ExistsAsync(value, name);
                if (!exists)
                {
                    _logger.LogWarning("Plan not found with {Key}: {Value}", name, value);
                }

                return exists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while checking existence of Plan with {Key}: {Value}", name, value);
                return false;
            }
        }

        public override async Task<PagedResponse<PlanResponseDso>> GetAllAsync(string[]? includes = null, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                _logger.LogInformation("Fetching all Plans with pagination: Page {PageNumber}, Size {PageSize}", pageNumber, pageSize);
                var results = (await _share.GetAllAsync(includes, pageNumber, pageSize));
                var items = GetMapper().Map<List<PlanResponseDso>>(results.Data);
                return new PagedResponse<PlanResponseDso>(items, results.PageNumber, results.PageSize, results.TotalPages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching all Plans.");
                return new PagedResponse<PlanResponseDso>(new List<PlanResponseDso>(), pageNumber, pageSize, 0);
            }
        }

        public override async Task<PlanResponseDso?> GetByIdAsync(object id)
        {
            try
            {
                _logger.LogInformation("Fetching Plan by ID: {Id}", id);
                var result = await _share.GetByIdAsync(id);
                if (result == null)
                {
                    _logger.LogWarning("Plan not found with ID: {Id}", id);
                    return null;
                }

                _logger.LogInformation("Retrieved Plan successfully.");
                return GetMapper().Map<PlanResponseDso>(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while retrieving Plan by ID: {Id}", id);
                return null;
            }
        }

        public override async Task DeleteAsync(object value, string key = "Id")
        {
            try
            {
                _logger.LogInformation("Deleting Plan with {Key}: {Value}", key, value);
                await _share.DeleteAsync(value, key);
                _logger.LogInformation("Plan with {Key}: {Value} deleted successfully.", key, value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting Plan with {Key}: {Value}", key, value);
            }
        }

        public override async Task DeleteRange(List<PlanRequestDso> entities)
        {
            try
            {
                var builddtos = entities.OfType<PlanRequestShareDto>().ToList();
                _logger.LogInformation("Deleting {Count} Plans...", 201);
                await _share.DeleteRange(builddtos);
                _logger.LogInformation("{Count} Plans deleted successfully.", 202);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting multiple Plans.");
            }
        }

        public override async Task<PagedResponse<PlanResponseDso>> GetAllByAsync(List<FilterCondition> conditions, ParamOptions? options = null)
        {
            try
            {
                _logger.LogInformation("Retrieving all Plan entities...");
                var results = await _share.GetAllAsync();
                var response = await _share.GetAllByAsync(conditions, options);
                return response.ToResponse(GetMapper().Map<IEnumerable<PlanResponseDso>>(response.Data));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAllAsync for Plan entities.");
                return null;
            }
        }

        public override async Task<PlanResponseDso?> GetOneByAsync(List<FilterCondition> conditions, ParamOptions? options = null)
        {
            try
            {
                _logger.LogInformation("Retrieving Plan entity...");
                return GetMapper().Map<PlanResponseDso>(await _share.GetOneByAsync(conditions, options));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetOneByAsync  for Plan entity.");
                return null;
            }
        }
    }
}