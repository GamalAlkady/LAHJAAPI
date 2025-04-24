using AutoMapper;
using LAHJAAPI.V1.DyModels.VM.Stripe.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Stripe;
using StripeGateway;

namespace LAHJAAPI.V1.Controllers.Api
{
    [AllowAnonymous]
    [Route("api/v1/user/[controller]")]
    [ApiController]
    public class ProductController(IStripeProduct stripeProduct, IMemoryCache memoryCache, IMapper mapper) : Controller
    {

        [HttpGet(Name = "GetProducts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<ProductResponse>>> GetAll(string? startingAfter, string? endingBefore, long? limit)
        {
            var options = new ProductListOptions
            {
                Active = true,
                StartingAfter = startingAfter,
                EndingBefore = endingBefore,
                Limit = limit
            };

            var listProducts = await stripeProduct.GetAllAsync(options);
            StripeList<ProductResponse> responses = new StripeList<ProductResponse>()
            {
                HasMore = listProducts.HasMore,
                Object = listProducts.Object,
                Url = listProducts.Url,
            };
            responses.Data = mapper.Map<List<ProductResponse>>(listProducts);

            return Ok(responses);
        }

        [HttpGet("{id}", Name = "GetProduct")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProductResponse>> GetOne(string id)
        {
            var product = await stripeProduct.GetByIdAsync(id);
            ProductResponse response = mapper.Map<ProductResponse>(product);
            return Ok(response);
        }

        [HttpPost(Name = "CreateProduct")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ProductResponse>> Create(ProductCreate productCreate)
        {

            var options = mapper.Map<ProductCreate, ProductCreateOptions>(productCreate);
            var product = await stripeProduct.CreateAsync(options);
            ProductResponse response = mapper.Map<ProductResponse>(product);
            return Ok(response);
        }

        [HttpPut("{id}", Name = "UpdateProduct")]
        public async Task<ActionResult<ProductResponse>> Update(string id, ProductUpdate productUpdate)
        {
            var options = new ProductUpdateOptions
            {
                Name = productUpdate.Name,
                Active = productUpdate.Active,
                Description = productUpdate.Description,
                Shippable = productUpdate.Shippable,

                //Metadata = new Dictionary<string, string> { { "order_id", "6735" } },
            };
            var product = await stripeProduct.UpdateAsync(id, options);
            var response = mapper.Map<ProductResponse>(product);
            return Ok(response);
        }

        //[HttpPut("archive/{id}")]
        //public async Task<ActionResult<ProductResponse>> Archive(string id, ProductUpdate productUpdate)
        //{
        //    var result = await productService.UpdateAsync(id, productUpdate);
        //    return Ok(result);
        //}

        [HttpDelete("{id}", Name = "DeleteProduct")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var response = await stripeProduct.DeleteAsync(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new ProblemDetails
                {
                    Detail = ex.Message
                });
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<StripeSearchResult<ProductResponse>>> Search(string query, int limit = 10, string? page = null)
        {
            var product = await stripeProduct.SearchAsync(new ProductSearchOptions
            {
                Query = query,
                Limit = limit,
                Page = page
            });

            var response = mapper.Map<StripeSearchResult<ProductResponse>>(product);
            return Ok(response);
        }

    }
}