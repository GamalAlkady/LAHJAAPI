using APILAHJA.Utilities;
using AutoMapper;
using LAHJAAPI.Attributes;
using Microsoft.AspNetCore.Mvc;
using StripeGateway;
using V1.DyModels.Dso.Requests;
using V1.DyModels.VMs;
using V1.Services.Services;

namespace LAHJAAPI.V1.Controllers.Api
{
    [ApiExplorerSettings(GroupName = "User")]
    [ServiceFilter(typeof(SubscriptionCheckFilter))]
    [Route("api/v1/user/[controller]")]
    [ApiController]
    public class SubscriptionController : ControllerBase
    {
        private readonly IUseSubscriptionService _subscriptionService;
        private readonly IUseApplicationUserService _userService;
        private readonly IStripeCustomer _stripeCustomer;
        private readonly IStripeSubscription _stripeSubscription;
        private readonly IUsePlanFeatureService _planFeatureService;
        private readonly IUserClaimsHelper _userClaims;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        public SubscriptionController(
            IUseSubscriptionService subscriptionService,
            IUseApplicationUserService userService,
            IStripeCustomer stripeCustomer,
            IStripeSubscription stripeSubscription,
            IUsePlanService planService,
            IUsePlanFeatureService planFeatureService,
            IUserClaimsHelper userClaims,
            IMapper mapper,
            ILoggerFactory logger)
        {
            _subscriptionService = subscriptionService;
            _userService = userService;
            _stripeCustomer = stripeCustomer;
            _stripeSubscription = stripeSubscription;
            _planFeatureService = planFeatureService;
            _userClaims = userClaims;
            _mapper = mapper;
            _logger = logger.CreateLogger(typeof(SubscriptionController).FullName);
        }

        // Get all Subscriptions.
        [SkipSubscriptionCheck]
        [HttpGet(Name = "GetSubscriptions")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<SubscriptionOutputVM>>> GetAll()
        {
            try
            {
                _logger.LogInformation("Fetching all Subscriptions...");
                var result = await _subscriptionService.GetAllAsync();
                //var subscriptionRequest = _mapper.Map<List<SubscriptionRequestDso>>(result);

                var items = _mapper.Map<List<SubscriptionOutputVM>>(result);
                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching all Subscriptions");
                return StatusCode(500, "Internal Server Error");
            }
        }

        // Get a Subscription by ID.
        [SkipSubscriptionCheck]
        [HttpGet("{id}", Name = "GetSubscription")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SubscriptionOutputVM>> GetById(string? id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                _logger.LogWarning("Invalid Subscription ID received.");
                return BadRequest("Invalid Subscription ID.");
            }

            try
            {
                _logger.LogInformation("Fetching Subscription with ID: {id}", id);
                var entity = await _subscriptionService.GetByIdAsync(id);
                if (entity == null)
                {
                    _logger.LogWarning("Subscription not found with ID: {id}", id);
                    return NotFound();
                }

                var item = _mapper.Map<SubscriptionOutputVM>(entity);
                return Ok(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching Subscription with ID: {id}", id);
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("My", Name = "GetMySubscription")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [SkipSubscriptionCheck]
        public async Task<ActionResult<SubscriptionOutputVM>> GetMy()
        {

            try
            {
                _logger.LogInformation("Fetching My Subscription.");
                var entity = await _subscriptionService.GetUserSubscription();
                if (entity == null)
                {
                    _logger.LogWarning("Subscription not found.");
                    return NotFound();
                }

                var item = _mapper.Map<SubscriptionOutputVM>(entity);
                return Ok(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching my Subscription.");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("PauseCollection", Name = "PauseCollection")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PauseCollection(SubscriptionUpdateRequest subscriptionUpdate)
        {
            try
            {

                var result = await _stripeSubscription.UpdateAsync(_userClaims.SubscriptionId, new Stripe.SubscriptionUpdateOptions
                {
                    PauseCollection = new Stripe.SubscriptionPauseCollectionOptions()
                    {
                        Behavior = subscriptionUpdate.PauseCollectionBehavior.ToString(),
                        ResumesAt = subscriptionUpdate.ResumesAt
                    }
                });

                //if (result.CancelAtPeriodEnd)
                return Ok();
                //return BadRequest(new ProblemDetails { Detail = "Faild cancel subscription" });
            }
            catch (Exception ex)
            {
                return BadRequest(HandelResult.Problem(ex));
            }
        }


        [HttpPut("ResumeCollection", Name = "ResumeCollection")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ResumeCollection()
        {
            try
            {
                var options = new Stripe.SubscriptionUpdateOptions();
                options.AddExtraParam("pause_collection", "");

                var result = await _stripeSubscription.UpdateAsync(_userClaims.SubscriptionId, options);
                //var item = await subscriptionRepository.GetByIdAsync(id);
                return Ok();
            }
            catch (Stripe.StripeException ex)
            {
                return BadRequest(HandelResult.Problem(ex));
            }
            catch (Exception ex)
            {
                return BadRequest(HandelResult.Problem(ex));
            }
        }



        [HttpDelete("Cancel", Name = "CancelSubscription")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CancelSubscription()
        {
            try
            {
                var result = await _stripeSubscription.CancelAsync(_userClaims.SubscriptionId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(HandelResult.Problem(ex));
            }
        }

        [HttpPut("CancelAtEnd", Name = "CancelAtEnd")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CancelAtEnd()
        {
            try
            {
                _logger.LogInformation("Canceling subscription at the end of the period for Subscription ID: {id}", _userClaims.SubscriptionId);
                var result = await _stripeSubscription.UpdateAsync(_userClaims.SubscriptionId, new Stripe.SubscriptionUpdateOptions
                {
                    CancelAtPeriodEnd = true
                });
                return Ok(HandelResult.Text("Subscription canceled successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(HandelResult.Problem(ex));
            }
        }

        [SkipSubscriptionCheck]
        [HttpPut("Renew", Name = "RenewSubscription")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Renew()
        {
            try
            {
                var result = await _stripeSubscription.UpdateAsync(_userClaims.SubscriptionId, new Stripe.SubscriptionUpdateOptions
                {
                    CancelAtPeriodEnd = false
                });
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(HandelResult.Problem(ex));
            }
        }

        [SkipSubscriptionCheck]
        [HttpPut("Resume", Name = "ResumeSubscription")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Resume(SubscriptionResumeRequest subscriptionResume)
        {
            try
            {
                var result = await _stripeSubscription.ResumeAsync(_userClaims.SubscriptionId, new Stripe.SubscriptionResumeOptions
                {
                    BillingCycleAnchor = Stripe.SubscriptionBillingCycleAnchor.Now, // إعادة ضبط تاريخ الفوترة
                    ProrationBehavior = subscriptionResume.ProrationBehavior
                });
                //var item = await subscriptionRepository.GetByIdAsync(id);
                return Ok();
            }
            catch (Stripe.StripeException ex)
            {
                return BadRequest(HandelResult.Problem(ex));
            }
            catch (Exception ex)
            {
                return BadRequest(HandelResult.Problem(ex));
            }
        }



        //[ApiExplorerSettings(IgnoreApi = true)]
        //[HttpDelete("DeleteByStatus/{status}", Name = "DeleteByStatus")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        //public async Task<IActionResult> DeleteByStatus(string status)
        //{
        //    try
        //    {
        //        await _subscriptionService.DeleteAllAsync(s => s.Status == status);
        //        return Ok();
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(HandelErrors.Problem(ex));
        //    }
        //}


        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPut(Name = "UpdateSubscription")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SubscriptionOutputVM>> Update([FromBody] SubscriptionUpdateVM model)
        {

            try
            {
                _logger.LogInformation("Updating Subscription with ID: {id}", model?.Id);
                var item = _mapper.Map<SubscriptionRequestDso>(model);
                var updatedEntity = await _subscriptionService.UpdateAsync(item);
                if (updatedEntity == null)
                {
                    _logger.LogWarning("Subscription not found for update with ID: {id}", model?.Id);
                    return NotFound();
                }

                var updatedItem = _mapper.Map<SubscriptionOutputVM>(updatedEntity);
                return Ok(updatedItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating Subscription with ID: {id}", model?.Id);
                return StatusCode(500, "Internal Server Error");
            }
        }

        // Delete a Subscription.
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpDelete(Name = "DeleteSubscription")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete()
        {
            string id = _userClaims.SubscriptionId!;

            try
            {
                _logger.LogInformation("Deleting Subscription with ID: {id}", id);
                await _subscriptionService.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting Subscription with ID: {id}", id);
                return StatusCode(500, "Internal Server Error");
            }
        }

        // Get count of Subscriptions.
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("Count", Name = "CountSubscription")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<int>> Count()
        {
            try
            {
                _logger.LogInformation("Counting Subscriptions...");
                var count = await _subscriptionService.CountAsync();
                return Ok(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while counting Subscriptions");
                return StatusCode(500, "Internal Server Error");
            }
        }

    }
}