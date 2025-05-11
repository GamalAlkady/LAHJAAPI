using APILAHJA.Utilities;
using AutoGenerator;
using AutoGenerator.Helper;
using AutoMapper;
using LAHJAAPI.V1.Validators;
using LAHJAAPI.V1.Validators.Conditions;
using Microsoft.AspNetCore.Mvc;
using V1.DyModels.Dso.Requests;
using V1.DyModels.VMs;
using V1.Services.Services;

namespace LAHJAAPI.V1.Controllers.Api
{

    //[ApiExplorerSettings(GroupName = "User")]
    [Route("api/v1/user/[controller]")]
    [ApiController]
    public class ProfileController(
        IUseApplicationUserService userService,
        IUseSubscriptionService subscriptionService,
        IUseRequestService requestService,
        IConditionChecker checker,
        IMapper mapper,
        ILogger<ProfileController> logger,
        IUserClaimsHelper userClaims
        ) : Controller
    {
        [HttpGet("user", Name = "GetUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApplicationUserOutputVM>> GetUser()
        {
            var user = await userService.GetOneByAsync([new() { PropertyName = "Id", Value = userClaims.UserId }],
                new ParamOptions() { Includes = ["Subscription"] });
            var response = mapper.Map<ApplicationUserOutputVM>(user);
            return Ok(response);
        }

        [HttpPut("Update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> Update([FromBody] ApplicationUserUpdateVM model)
        {
            try
            {
                logger.LogInformation("Updating User");
                var user = mapper.Map<ApplicationUserRequestDso>(model);

                await userService.UpdateAsync(user);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(HandleResult.Problem(ex));
            }
        }

        //TDOO: inject one service for every controller
        [HttpGet("subscriptions", Name = "GetUserSubscriptions")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<PagedResponse<IEnumerable<SubscriptionOutputVM>>>> GetUserSubscriptions()
        {
            try
            {
                var user = await userService.GetByIdAsync(userClaims.UserId);

                if (user.CustomerId == null)
                    return NotFound(HandleResult.NotFound("You are not stripe customer yet!", "user", "No customer id found"));


                var response = await subscriptionService.GetAllByAsync([
                    new () { PropertyName="CustomerId",Value=user.CustomerId},
                    //     new (){
                    //    Logic=FilterLogic.And,
                    //    PropertyName = "Status", Value="Active"
                    //}
                    ], new ParamOptions() { PageSize = 100 });

                var response2 = response.ToResponse(mapper.Map<IEnumerable<SubscriptionOutputVM>>(response.Data));
                return Ok(response2);
            }
            catch (Exception ex)
            {
                return BadRequest(HandleResult.Problem(ex));
            }
        }

        [HttpGet("modelAis", Name = "ModelAis")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<ModelAiOutputVM>>> ModelAis()
        {
            var response = await userService.GetModels();
            var data = mapper.Map<IEnumerable<ModelAiOutputVM>>(response);

            return Ok(data);
        }

        [HttpGet("services", Name = "Services")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<ServiceOutputVM>>> Services()
        {
            try
            {

                var services = await userService.GetServices();
                if (services == null) return NoContent();

                var result = mapper.Map<List<ServiceOutputVM>>(services);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(HandleResult.Problem(ex));
            }
        }
        //TODO: Cause looping
        //[HttpGet("services-modelAi/{modelAiId}", Name = "ServicesModelAi")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //public async Task<ActionResult<IEnumerable<ModelAiOutputVM>>> ServicesModelAi(string modelAiId)
        //{
        //    var services = await serviceRepository.GetAllAsync(s => s.ModelAiId == modelAiId);
        //    var response = mapper.Map<List<ServiceResponse>>(services);

        //    return Ok(response);
        //}


        [HttpGet("spaces-subscription", Name = "SpacesSubscription")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<SpaceOutputVM>>> SpacesSubscription(string? subscriptionId)
        {
            if (string.IsNullOrWhiteSpace(subscriptionId))
            {
                subscriptionId = userClaims.SubscriptionId;
                //var items = await subscriptionService.GetSpaces(subscriptionId!);
                //var res = mapper.Map<IEnumerable<SpaceOutputVM>>(items);
                //return Ok(res);
            }
            else
            {

                if (!await checker.CheckAsync(SubscriptionValidatorStates.IsBelongToUser, subscriptionId))
                    return BadRequest(HandleResult.Problem("Subscription not belong to you!", $"Subscription with Id: {subscriptionId} not belong to you!", "Subscription"));
            }
            //var subscription = await subscriptionService.GetCustomerSubscription(subscriptionId);

            //if (subscription == null)
            //    return BadRequest(HandelResult.NotFound("Incorrect subscription id or not belong to you.", "Subscription", "No subscription found"));

            var responseDso = await subscriptionService.GetSpaces(subscriptionId);
            var response = responseDso.ToResponse(mapper.Map<IEnumerable<SpaceOutputVM>>(responseDso.Data));
            return Ok(response);
        }

        [HttpGet("space-subscription", Name = "SpaceSubscription")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<SpaceOutputVM>> SpaceSubscription(string subscriptionId, string spaceId)
        {
            //var subscription = await subscriptionService.GetUserSubscription();
            //if (subscription == null) return BadRequest(HandelErrors.NotFound("Subscription id incorrect or not belong to you.", "spaces subscription"));

            var item = await subscriptionService.GetSpace(spaceId, subscriptionId);
            var response = mapper.Map<SpaceOutputVM>(item);

            return Ok(response);
        }

        [HttpGet("requests-subscription", Name = "RequestsSubscription")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<PagedResponse<RequestOutputVM>>> RequestsSubscription(string subscriptionId)
        {
            var response = await requestService.GetAllByAsync([new FilterCondition("SubscriptionId", subscriptionId)], new ParamOptions(["Events"]));
            var result = response.ToResponse(mapper.Map<IEnumerable<RequestOutputVM>>(response.Data));

            return Ok(result);
        }

        [HttpGet("requests-services", Name = "RequestsService")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<PagedResponse<RequestOutputVM>>> RequestsService(string serviceId)
        {
            var response = await requestService.GetAllByAsync([new FilterCondition("ServiceId", serviceId)], new ParamOptions(["Events"]));
            var result = response.ToResponse(mapper.Map<IEnumerable<RequestOutputVM>>(response.Data));

            return Ok(result);
        }
    }
}