using APILAHJA.Utilities;
using LAHJAAPI.Attributes;
using LAHJAAPI.V1.Validators;
using LAHJAAPI.V1.Validators.Conditions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class SubscriptionCheckFilter(IConditionChecker checker, IUserClaimsHelper userClaims) : IAsyncActionFilter
{

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        try
        {
            var skip = context.ActionDescriptor.EndpointMetadata
         .OfType<SkipSubscriptionCheckAttribute>()
         .Any();
            if (!skip)
            {
                var result = await checker.CheckAndResultAsync(SubscriptionValidatorStates.IsNotSubscribe, userClaims.SubscriptionId);
                if (result.Success == true)
                {
                    context.Result = new ObjectResult(result.Result ?? result.Message)
                    { StatusCode = StatusCodes.Status402PaymentRequired };
                    return;
                }
            }
        }
        catch (Exception ex)
        {
            //if(ex.GetType() == typeof(ArgumentNullException)) 
            context.Result = new ObjectResult(new ProblemDetails
            {
                Title = "Check subscription",
                Detail = "You do not have a subscription. Please subscribe if you want use this service.",
                Status = (int)SubscriptionValidatorStates.IsNotSubscribe
            })
            { StatusCode = StatusCodes.Status402PaymentRequired };
            return;
        }
        await next();
    }


}

