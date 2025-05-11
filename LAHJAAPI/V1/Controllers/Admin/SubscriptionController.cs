using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StripeGateway;
using V1.DyModels.VMs;
using V1.Services.Services;

namespace LAHJAAPI.V1.Controllers.Admin;

//[ApiExplorerSettings(GroupName = "Admin")]
[ServiceFilter(typeof(SubscriptionCheckFilter))]
[Route("api/v1/admin/[controller]")]
[ApiController]
public class SubscriptionController : ControllerBase
{
    private readonly IUseSubscriptionService _subscriptionService;
    private readonly IUseApplicationUserService _userService;
    private readonly IStripeCustomer _stripeCustomer;
    private readonly IStripeSubscription _stripeSubscription;
    private readonly IUsePlanService _planService;
    private readonly IUsePlanFeatureService _planFeatureService;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;
    public SubscriptionController(
        IUseSubscriptionService subscriptionService,
        IUseApplicationUserService userService,
        IStripeCustomer stripeCustomer,
        IStripeSubscription stripeSubscription,
        IUsePlanService planService,
        IUsePlanFeatureService planFeatureService,
        IMapper mapper,
        ILoggerFactory logger)
    {
        _subscriptionService = subscriptionService;
        _userService = userService;
        _stripeCustomer = stripeCustomer;
        _stripeSubscription = stripeSubscription;
        _planService = planService;
        _planFeatureService = planFeatureService;
        _mapper = mapper;
        _logger = logger.CreateLogger(typeof(SubscriptionController).FullName);
    }

    // Get all Subscriptions.
    [HttpGet(Name = "GetAllSubscriptions")]
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
    [HttpGet("{id}", Name = "GetOneSubscription")]
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



    [HttpPut("PauseCollection/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PauseCollection(string id, SubscriptionUpdateRequest subscriptionUpdate)
    {
        try
        {
            var result = await _stripeSubscription.UpdateAsync(id, new Stripe.SubscriptionUpdateOptions
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
            return BadRequest(HandleResult.Problem(ex));
        }
    }


    [HttpPut("ResumeCollection/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResumeCollection(string id)
    {
        try
        {
            var options = new Stripe.SubscriptionUpdateOptions();
            options.AddExtraParam("pause_collection", "");

            var result = await _stripeSubscription.UpdateAsync(id, options);
            //var item = await subscriptionRepository.GetByIdAsync(id);
            return Ok();
        }
        catch (Stripe.StripeException ex)
        {
            return BadRequest(HandleResult.Problem(ex));
        }
        catch (Exception ex)
        {
            return BadRequest(HandleResult.Problem(ex));
        }
    }



    [HttpDelete("cancel/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CancelSubscription(string id)
    {
        try
        {
            var result = await _stripeSubscription.CancelAsync(id);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(HandleResult.Problem(ex));
        }
    }

    [HttpPut("CancelAtEnd/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CancelAtEnd(string id)
    {
        try
        {
            var result = await _stripeSubscription.UpdateAsync(id, new Stripe.SubscriptionUpdateOptions
            {
                CancelAtPeriodEnd = true
            });
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(HandleResult.Problem(ex));
        }
    }

    [HttpPut("Renew/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Renew(string id)
    {
        try
        {
            var result = await _stripeSubscription.UpdateAsync(id, new Stripe.SubscriptionUpdateOptions
            {
                CancelAtPeriodEnd = false
            });
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(HandleResult.Problem(ex));
        }
    }

    [HttpPut("resume/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Resume(string id, SubscriptionResumeRequest subscriptionResume)
    {
        try
        {
            var result = await _stripeSubscription.ResumeAsync(id, new Stripe.SubscriptionResumeOptions
            {
                BillingCycleAnchor = Stripe.SubscriptionBillingCycleAnchor.Now, // إعادة ضبط تاريخ الفوترة
                ProrationBehavior = subscriptionResume.ProrationBehavior
            });
            //var item = await subscriptionRepository.GetByIdAsync(id);
            return Ok();
        }
        catch (Stripe.StripeException ex)
        {
            return BadRequest(HandleResult.Problem(ex));
        }
        catch (Exception ex)
        {
            return BadRequest(HandleResult.Problem(ex));
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


    // Delete a Subscription.
    [ApiExplorerSettings(IgnoreApi = true)]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string? id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            _logger.LogWarning("Invalid Subscription ID received in Delete.");
            return BadRequest("Invalid Subscription ID.");
        }

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
    [HttpGet("CountSubscription")]
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