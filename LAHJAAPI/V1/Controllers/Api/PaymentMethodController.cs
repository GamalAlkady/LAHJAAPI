using APILAHJA.Utilities;
using LAHJAAPI.V1.DyModels.VM.Stripe.Payment;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using StripeGateway;

namespace LAHJAAPI.V1.Controllers.Api
{
    [ApiExplorerSettings(GroupName = "User")]
    [Route("api/v1/user/[controller]")]
    [ApiController]
    public class PaymentMethodController(
        IStripeSetupIntent stripeSetupIntent,
        IStripeCustomer stripeCustomer,
        IStripePaymentMethod stripePaymentMethod,
        IUserClaimsHelper userClaims
        ) : Controller
    {


        [HttpGet("GetMethods")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<PaymentMethodResponse>>> GetMethods()
        {
            try
            {
                var items = await stripePaymentMethod.GetListByCustomerAsync(userClaims.CustomerId);

                return Ok(items);
            }
            catch (Exception ex)
            {
                return BadRequest(new ProblemDetails { Detail = ex.Message });
            }
        }


        [HttpPut("UpdateBillingInformation")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CustomerResponse>> UpdateBillingInformation(BillingInformationRequest billing)
        {
            try
            {
                var item = await stripeCustomer.UpdateAsync(userClaims.CustomerId, new CustomerUpdateOptions
                {
                    Name = billing.Name,
                    Email = billing.Email,
                    Address = new AddressOptions
                    {
                        City = billing.City,
                        Country = billing.Country,
                        Line1 = billing.Line1,
                        Line2 = billing.Line2,
                        PostalCode = billing.PostalCode,
                        State = billing.State
                    }
                });

                return Ok(item);
            }
            catch (Exception ex)
            {
                return BadRequest(new ProblemDetails { Detail = ex.Message });
            }
        }

        //[HttpPut("AllowRedisplay/{id}")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //public async Task<ActionResult<PaymentMethodResponse>> AllowRedisplay(string id)
        //{
        //    try
        //    {
        //        var item = await stripePaymentMethod.UpdateAsync(id, new PaymentMethodUpdateOptions { AllowRedisplay = "always" });

        //        return Ok(item);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new ProblemDetails { Detail = ex.Message });
        //    }
        //}



        [HttpPut("MakePaymentMethodDefault/{paymentMethodId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> MakePaymentMethodDefault(string paymentMethodId)
        {
            try
            {
                var items = await stripeCustomer.UpdateAsync(userClaims.CustomerId, new CustomerUpdateOptions
                {
                    InvoiceSettings = new CustomerInvoiceSettingsOptions
                    {
                        DefaultPaymentMethod = paymentMethodId // معرف PaymentMethod الذي تم حفظه
                    }
                });

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new ProblemDetails { Detail = ex.Message });
            }
        }

        [HttpDelete("DeleteMethod/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteMethod(string id)
        {
            try
            {
                var items = await stripePaymentMethod.DeleteAsync(id);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new ProblemDetails { Detail = ex.Message });
            }
        }

        [HttpGet("GetSetupIntents")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> GetSetupIntents()
        {
            try
            {
                var items = await stripeSetupIntent.GetListAsync(new SetupIntentListOptions { Customer = userClaims.CustomerId });

                return Ok(items);
            }
            catch (Exception ex)
            {
                return BadRequest(new ProblemDetails { Detail = ex.Message });
            }
        }

        [HttpDelete("cancel/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Cancel(string id)
        {
            try
            {
                var items = await stripeSetupIntent.CancelAsync(id);

                return Ok(items);
            }
            catch (Exception ex)
            {
                return BadRequest(new ProblemDetails { Detail = ex.Message });
            }
        }

        [HttpPut("Confirm/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> ConfirmAsync(string id)
        {
            try
            {
                var item = await stripeSetupIntent.ConfirmAsync(id, new SetupIntentConfirmOptions { });

                return Ok(item);
            }
            catch (Exception ex)
            {
                return BadRequest(new ProblemDetails { Detail = ex.Message });
            }
        }

        [HttpPost("CreatePaymentMethod")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResponseClientSecret>> CreatePaymentMethod([FromBody] PaymentMethodsRequest request)
        {
            try
            {
                var setupIntents = await stripeSetupIntent.GetListAsync(new SetupIntentListOptions { Customer = userClaims.CustomerId });
                var setupIntent = setupIntents.FirstOrDefault();
                if (setupIntent == null || setupIntent.Status != "requires_payment_method")
                {
                    setupIntent = await stripeSetupIntent.CreateAsync(new SetupIntentCreateOptions
                    {
                        Customer = userClaims.CustomerId,
                        PaymentMethodTypes = request.PaymentMethodTypes,
                    });
                }

                return Ok(new ResponseClientSecret { ClientSecret = setupIntent.ClientSecret });
            }
            catch (Exception ex)
            {
                return BadRequest(new ProblemDetails { Detail = ex.Message });
            }
        }



    }
}