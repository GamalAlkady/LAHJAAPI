using AutoGenerator.Helper.Translation;
using AutoMapper;
using StripeGateway;
using V1.BPR.Layers.Base;
using V1.DyModels.Dso.Requests;
using V1.DyModels.Dso.Responses;
using V1.DyModels.Dto.Share.Requests;
using V1.DyModels.Dto.Share.Responses;
using V1.Repositories.Share;

namespace V1.Services.Services
{
    public class PlanService : BaseBPRServiceLayer<PlanRequestDso, PlanResponseDso, PlanRequestShareDto, PlanResponseShareDto>, IUsePlanService
    {
        private readonly IPlanShareRepository _share;
        private readonly IStripeProduct _stripeProduct;
        private readonly IStripePrice _stripePrice;

        public PlanService(
            IPlanShareRepository buildPlanShareRepository,
            IStripeProduct stripeProduct,
            IStripePrice stripePrice,
            IMapper mapper,
            ILoggerFactory logger) : base(mapper, logger, buildPlanShareRepository)
        {
            _share = buildPlanShareRepository;
            _stripeProduct = stripeProduct;
            _stripePrice = stripePrice;
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
                if (string.IsNullOrWhiteSpace(entity.Id))
                {
                    var price = await _stripePrice!.CreateAsync(new()
                    {
                        Currency = entity.Currency,
                        UnitAmount = Convert.ToInt64(entity.Amount) * 100,
                        Recurring = new Stripe.PriceRecurringOptions { Interval = entity.BillingPeriod },
                        Product = product.Id
                    });
                    entity.Id = price.Id;
                }

                var result = await _share.CreateAsync(entity);
                var output = MapToResponse(result);
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
                var output = MapToResponse(result);
                _logger.LogInformation("Created Plan entity successfully.");
                return output;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating Plan entity.");
                throw;
            }
        }


    }
}