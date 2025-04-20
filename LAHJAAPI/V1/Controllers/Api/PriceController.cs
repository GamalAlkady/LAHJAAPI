using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Stripe;
using StripeGateway;

namespace ASG.Api2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PriceController(IStripePrice stripePrice, IMapper mapper, IMemoryCache memoryCache) : Controller
    {
        [HttpGet(Name = "GetPrices")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<PriceResponse>>> GetAll(string? productId = null, bool? active = null)
        {
            try
            {
                var options = new PriceListOptions { Product = productId, Active = active };

                var listPrices = await stripePrice.GetAllAsync(options);
                StripeList<PriceResponse> responses = new()
                {
                    HasMore = listPrices.HasMore,
                    Object = listPrices.Object,
                    Url = listPrices.Url,
                };
                responses.Data = mapper.Map<List<PriceResponse>>(listPrices);
                return Ok(responses);
            }
            catch (Exception e)
            {
                return BadRequest(new ProblemDetails { Detail = e.Message });
            }
        }

        [HttpGet("{id}", Name = "GetPrice")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PriceResponse>> GetById(string id)
        {
            try
            {
                Func<ICacheEntry, Task<PriceResponse>> factory = async entry =>
                {
                    entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

                    var price = await stripePrice.GetByIdAsync(id);
                    var response = mapper.Map<PriceResponse>(price);
                    return response;

                };
                var price = await memoryCache.GetOrCreateAsync($"price-{id}", factory);
                return Ok(price);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        [HttpPost(Name = "CreatePrice")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PriceResponse>> Create(PriceCreate priceCreate)
        {
            var price = await stripePrice.CreateAsync(new PriceCreateOptions
            {
                UnitAmount = priceCreate.UnitAmount,
                Currency = priceCreate.Currency,
                Recurring = new PriceRecurringOptions
                {
                    Interval = priceCreate.Interval
                },
                Product = priceCreate.ProductId

            });
            return Ok(mapper.Map<PriceResponse>(price));
        }


        [HttpPut("{id}", Name = "UpdatePrice")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PriceResponse>> Update(string id, PriceUpdate priceUpdate)
        {
            try
            {
                //if(ModelState.IsValid) {
                var price = await stripePrice.UpdateAsync(id, new PriceUpdateOptions { Active = priceUpdate.Active, LookupKey = priceUpdate.LookupKey });
                var item = mapper.Map<PriceResponse>(price);
                return Ok(item);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> SearchAsync(PriceSearchOptions options, CancellationToken cancellationToken = default)
        {
            var response = await stripePrice.SearchAsync(options, cancellationToken);

            return Ok(response);
        }
    }
}