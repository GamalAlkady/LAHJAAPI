﻿//using AutoGenerator.Conditions;
//using AutoMapper;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Logging;
//using StripeGateway;

//namespace SubPayService
//{
//    [Route("api/v1/user/[controller]")]
//    [ApiController]
//    public class CheckoutController(
//        IStripeSubscription stripeSubscription,
//        IStripeCheckout stripeCheckout,
//        IStripeCustomer stripeCustomer,
//        ILogger<CheckoutController> logger,
//        IConditionChecker checker,
//        IMapper mapper
//        ) : Controller
//    {


//        public async Task<ActionResult<CheckoutResponse>> CreateCheckout(CheckoutOptions checkoutOptions)
//        {
//            try
//            {
//                var user = await userService.GetUserWithSubscription();
//                if (checker.Check(SubscriptionValidatorStates.IsSubscribe, new DataFilter("userId")))
//                {
//                    return Conflict(new ProblemDetails { Detail = "You already have subscription" });
//                }

//                await CreateCustomer(user);

//                var plan = await planService.GetByIdAsync(checkoutOptions.PlanId);
//                if (plan is null) return NotFound(new ProblemDetails { Title = "NOT FOUND", Detail = "Plan not found" });


//                if (plan.Amount == 0)
//                {
//                    // Create a free subscription
//                    return await CreateFreeSubscription(plan.Id, user.CustomerId);
//                }

//                var options = new SessionCreateOptions
//                {
//                    Customer = user.CustomerId,
//                    SuccessUrl = $"{checkoutOptions.SuccessUrl}?session_id={{CHECKOUT_SESSION_ID}}",
//                    CancelUrl = $"{checkoutOptions.CancelUrl}",
//                    Mode = "subscription",
//                    //Locale = "ar",
//                    Expand = new List<string> { "customer" },
//                    LineItems = new List<SessionLineItemOptions>
//                {
//                        new SessionLineItemOptions
//                        {
//                        Price = checkoutOptions.PlanId,
//                        Quantity = 1,
//                    },
//                },
//                    // AutomaticTax = new SessionAutomaticTaxOptions { Enabled = true },
//                };

//                //options.Customer = user.CustomerId;
//                //else options.CustomerEmail = user.Email;

//                var session = await stripeCheckout.CreateCheckoutSession(options);
//                return Ok(new { session.Url });
//            }
//            catch (Exception ex)
//            {
//                return BadRequest(new ProblemDetails { Detail = ex.Message });
//            }
//        }

//        [HttpPost("CreateWebCheckout", Name = "CreateWebCheckout")]
//        [ProducesResponseType(StatusCodes.Status200OK)]
//        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
//        [ProducesResponseType(StatusCodes.Status400BadRequest)]
//        public async Task<ActionResult<ResponseClientSecret>> CreateWebCheckout(CheckoutWebOptions checkoutOptions)
//        {
//            try
//            {
//                var user = await userService.GetUserWithSubscription();
//                //if (checker.Check(SubscriptionValidatorStates.IsSubscribe, new SubscriptionResponseFilterDso()))
//                //{
//                //    return Conflict(new ProblemDetails { Detail = "You already have subscription" });
//                //}

//                await CreateCustomer(user);

//                var plan = await planService.GetByIdAsync(checkoutOptions.PlanId);
//                if (plan is null) return NotFound(new ProblemDetails { Title = "NOT FOUND", Detail = "Plan not found" });


//                if (plan.Amount == 0)
//                {
//                    // Create a free subscription
//                    return await CreateFreeSubscription(plan.Id, user.CustomerId);
//                }

//                var options = new SessionCreateOptions
//                {
//                    Customer = user.CustomerId,
//                    //SuccessUrl = $"{checkoutOptions.SuccessUrl}?session_id={{CHECKOUT_SESSION_ID}}",
//                    //CancelUrl = $"{checkoutOptions.CancelUrl}",
//                    Mode = "subscription",
//                    //Locale = "ar",
//                    BillingAddressCollection = "auto",
//                    ShippingAddressCollection = new SessionShippingAddressCollectionOptions
//                    {
//                        //      AllowedCountries = new List<string>
//                        //{
//                        //  "US",
//                        //  "CA",
//                        //},
//                    },
//                    Expand = new List<string> { "customer" },
//                    LineItems = new List<SessionLineItemOptions>
//                {
//                        new SessionLineItemOptions
//                        {
//                        Price = checkoutOptions.PlanId,
//                        Quantity = 1,
//                    },
//                },
//                    UiMode = "custom",
//                    //UiMode = "embedded",
//                    ReturnUrl = checkoutOptions.ReturnUrl + "?session_id={CHECKOUT_SESSION_ID}",
//                };

//                //options.Customer = user.CustomerId;
//                //else options.CustomerEmail = user.Email;

//                var session = await stripeCheckout.CreateCheckoutSession(options);
//                return Ok(new ResponseClientSecret { ClientSecret = session.ClientSecret });
//            }
//            catch (Exception ex)
//            {
//                return BadRequest(new ProblemDetails { Detail = ex.Message });
//            }
//        }


//        private async Task<ActionResult> CreateFreeSubscription(string planId, string customerId)
//        {
//            logger.LogInformation("Creating free subscription for plan ID: {planId}", planId);
//            var sub = await stripeSubscription.CreateAsync(new SubscriptionCreateOptions()
//            {
//                Customer = customerId,
//                Items = new List<SubscriptionItemOptions>{
//                        new SubscriptionItemOptions
//                        {
//                            Price = planId,
//                        },
//                    },
//                TrialPeriodDays = 0, // بدون فترة تجريبية
//                PaymentBehavior = "default_incomplete", // يتم تجاهل الدفع لأنه مجاني

//            });
//            if (sub != null)
//            {
//                logger.LogInformation("Successfully created free subscription with ID: {subscriptionId}", sub.Id);
//                return Ok();
//                //return Ok(new { Message = "You have successfully subscribed to the free plan." });
//            }

//            logger.LogError("Failed to create free subscription for plan ID: {planId}", planId);
//            return BadRequest(new ProblemDetails { Detail = "con not subscribe for free plan" });
//        }
//        private async Task CreateCustomer(ApplicationUserResponseDso user)
//        {
//            try
//            {
//                if (user.CustomerId != null) return;
//                logger.LogInformation("Creating customer for user: {@user}", user);
//                var customers = await stripeCustomer.GetCustomersByEmail(user.Email);
//                var customer = customers.FirstOrDefault();
//                if (customer == null)
//                {
//                    customer = await stripeCustomer.CreateAsync(new CustomerCreateOptions()
//                    {
//                        Name = user.DisplayName,
//                        Email = user.Email
//                    });

//                    user.CustomerId = customer.Id;
//                    //await userManager.AddClaimAsync(user, new Claim(ClaimTypes2.CustomerId, customer.Id));
//                }
//                else
//                {
//                    user.CustomerId = customer.Id;
//                }
//                //var userVM = mapper.Map<ApplicationUserInfoVM>(user);
//                var userRequest = mapper.Map<ApplicationUserRequestDso>(user);
//                logger.LogInformation("Updating user with new customer ID: {customerId}", user.CustomerId);
//                await userService.UpdateAsync(userRequest);
//            }
//            catch (Exception ex)
//            {
//                logger.LogError(ex, "Error while creating customer for user: {@user}", user);
//                throw;
//            }
//        }


//        [HttpPost("manage")]
//        [ProducesResponseType(StatusCodes.Status200OK)]
//        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
//        [ProducesResponseType(StatusCodes.Status400BadRequest)]
//        public async Task<ActionResult<CheckoutResponse>> ManageSubscription(SessionCreate sessionCreate)
//        {
//            try
//            {
//                var user = await userService.GetUser();
//                //if(user.CustomerId==null) return 
//                var session = await stripeCustomer.CustomerPortal(new Stripe.BillingPortal.SessionCreateOptions
//                {
//                    Customer = user.CustomerId,
//                    ReturnUrl = sessionCreate.ReturnUrl,
//                    //Locale = "ar"
//                });
//                return Ok(new { session.Url });
//            }
//            catch (Exception ex)
//            {
//                return BadRequest(new ProblemDetails { Detail = ex.Message });
//            }
//        }



//        [HttpPost("CreateCustomerSession")]
//        [ProducesResponseType(StatusCodes.Status200OK)]
//        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
//        [ProducesResponseType(StatusCodes.Status400BadRequest)]
//        public async Task<ActionResult<ResponseClientSecret>> CreateCustomerSession([FromBody] PaymentMethodsRequest request)
//        {
//            try
//            {
//                var user = await userService.GetUser();
//                var customerSessionOptions = new CustomerSessionCreateOptions
//                {
//                    Customer = user.CustomerId,
//                    Components = new CustomerSessionComponentsOptions(),
//                };
//                customerSessionOptions.AddExtraParam("components[payment_element][enabled]", true);
//                customerSessionOptions.AddExtraParam(
//                    "components[payment_element][features][payment_method_redisplay]",
//                    "enabled");
//                customerSessionOptions.AddExtraParam(
//                    "components[payment_element][features][payment_method_save]",
//                    "enabled");
//                customerSessionOptions.AddExtraParam(
//                    "components[payment_element][features][payment_method_save_usage]",
//                    "on_session");
//                customerSessionOptions.AddExtraParam(
//                    "components[payment_element][features][payment_method_remove]",
//                    "enabled");

//                var customerSession = await stripeCustomer.CreateCustomerSession(customerSessionOptions);
//                return Ok(new CustomerSessionResponse
//                {
//                    CustomerSessionClientSecret = customerSession.ClientSecret,
//                });
//            }
//            catch (Exception ex)
//            {
//                return BadRequest(new ProblemDetails { Detail = ex.Message });
//            }
//        }


//        [HttpGet("session-status/{session_id}")]
//        [ProducesResponseType(StatusCodes.Status200OK)]
//        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
//        [ProducesResponseType(StatusCodes.Status400BadRequest)]
//        public ActionResult<SessionResponse> SessionStatus(string session_id)
//        {
//            try
//            {
//                var sessionService = new SessionService();
//                Session session = sessionService.Get(session_id);

//                return Ok(new SessionResponse { Status = session.Status, CustomerEmail = session.CustomerDetails.Email });
//            }
//            catch (Exception ex)
//            {
//                return BadRequest(new ProblemDetails { Detail = ex.Message });
//            }
//        }
//    }

//}