using LAHJAAPI.Models;
using LAHJAAPI.V1.Enums;
using LAHJAAPI.V1.Validators.Conditions;
using Microsoft.AspNetCore.Mvc;
using V1.DyModels.Dso.Responses;
using V1.DyModels.VMs;
using WasmAI.ConditionChecker.Base;

namespace LAHJAAPI.V1.Validators
{
    public enum RequestValidatorStates
    {
        IsAllowed = 6300,
        HasValidStatus,
        HasQuestion,
        IsValidRequestId,
        IsServiceId,
        IsSpaceId,
        IsUserId,
        HasValidDate,
        HasServiceIdIfNeeded,
        HasSubscriptionIdIfNeeded,
        //IsValidForCreate,
    }

    public class RequestValidator : ValidatorContext<Request, RequestValidatorStates>
    {
        private readonly IConditionChecker _checker;

        public RequestValidator(IConditionChecker checker) : base(checker)
        {
            _checker = checker;
        }


        protected override void InitializeConditions()
        {


            _provider.Register(
                RequestValidatorStates.HasValidStatus,
                new LambdaCondition<RequestResponseDso>(
                    nameof(RequestValidatorStates.HasValidStatus),
                    context => !string.IsNullOrWhiteSpace(context.Status),
                    "Status is required"
                )
            );

            _provider.Register(
                RequestValidatorStates.HasQuestion,
                new LambdaCondition<RequestResponseDso>(
                    nameof(RequestValidatorStates.HasQuestion),
                    context => !string.IsNullOrWhiteSpace(context.Question),
                    "Question is required"
                )
            );


            _provider.Register(
                    RequestValidatorStates.HasValidDate,
                    new LambdaCondition<RequestResponseDso>(
                        nameof(RequestValidatorStates.HasValidDate),
                        context => context.CreatedAt <= context.UpdatedAt,
                        "CreatedAt must be less than or equal to UpdatedAt"
                    )
                );

        }


        [RegisterConditionValidator(typeof(RequestValidatorStates), RequestValidatorStates.IsAllowed, "You have exhausted all allowed subscription requests.")]
        async Task<ConditionResult> IsAllowed(DataFilter<Subscription, Request> data)
        {

            if (data.Value == null)
                return ConditionResult.ToError("Object share must have subscription data.");

            HashSet<string> status = [RequestStatus.Success.ToString(), RequestStatus.Processing.ToString()];

            var countRequests = await QueryCountAsync<Request>(db =>
                   db.Where(r =>
                       r.SubscriptionId == data.Value.Id &&
                       status.Contains(r.Status) &&
                       r.CreatedAt >= data.Value.CurrentPeriodStart &&
                       r.CreatedAt <= data.Value.CurrentPeriodEnd
                   )
               );



            if ((await _checker.CheckAndResultAsync(PlanValidatorStates.HasAllowedRequests, new DataFilter
            {
                Id = data.Value.PlanId,
                Value = countRequests
            })).Result is PlanFeature result)
            {
                return ConditionResult.ToSuccess(new RequestInfoVM
                {
                    AllowedRequests = result.Value,
                    NumberRequests = countRequests
                }); ;
            }
            return ConditionResult.ToFailure(new ProblemDetails
            {
                Title = "Requests not allowed",
                Detail = "You have exhausted all allowed subscription requests.",
                Status = SubscriptionValidatorStates.IsAllowedRequestsForCreate.ToInt()
            });
        }


    }
}