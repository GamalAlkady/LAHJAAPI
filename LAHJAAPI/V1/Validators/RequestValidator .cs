using AutoGenerator.Conditions;
using LAHJAAPI.Models;
using LAHJAAPI.V1.Enums;
using LAHJAAPI.V1.Validators.Conditions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using V1.DyModels.Dso.Requests;
using V1.DyModels.Dso.Responses;
using V1.DyModels.VMs;

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
        IsFull,
        HasValidDate,
        HasServiceIdIfNeeded,
        HasSubscriptionIdIfNeeded,
        IsValid,
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
                 RequestValidatorStates.IsFull,
                 new LambdaCondition<RequestRequestDso>(
                     nameof(RequestValidatorStates.IsFull),
                     context => IsFull(context),
                     "Request is not"
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

            //_provider.Register(
            //    RequestValidatorStates.IsAllowed,
            //    new LambdaCondition<SubscriptionResponseDso>(
            //        nameof(RequestValidatorStates.IsAllowed),
            //        context => IsAllowedRequests(context),
            //        "Requests not allowed"
            //    )
            //);

            _provider.Register(
                   RequestValidatorStates.IsValidRequestId,
                   new LambdaCondition<RequestResponseDso>(
                       nameof(RequestValidatorStates.IsValidRequestId),
                       context => IsValidRequestId(context.Id),
                       "Request ID is required"
                   )
               );



            //_provider.Register(
            //        RequestValidatorStates.IsValid,
            //        new LambdaCondition<RequestFilterVM>(
            //            nameof(RequestValidatorStates.IsValid),
            //            context => IsValid(context),
            //            "Request ID is required"
            //        )
            //    );

        }


        //private bool IsValidCustomerId(string userId)
        //{

        //    return _checker.Check(RequestValidatorStates.IsUserId, userId);
        //}



        //bool IsAllowedRequests(SubscriptionResponseDso context)
        //{
        //    var requests = _checker.Injector.Context.Requests
        //        .Where(r => r.SubscriptionId == context.Id
        //        && r.Status == RequestStatus.Success.ToString()
        //        && r.CreatedAt >= context.CurrentPeriodStart && r.CreatedAt <= context.CurrentPeriodEnd)
        //        .ToList();

        //    return context.AllowedRequests > requests.Count;
        //}

        [RegisterConditionValidator(typeof(RequestValidatorStates), RequestValidatorStates.IsAllowed, "You have exhausted all allowed subscription requests.")]
        async Task<ConditionResult> IsAllowed(DataFilter<SubscriptionResponseDso, Request> data)
        {
            return await IsAllowedRequests(data);
        }
        async Task<ConditionResult> IsAllowedRequests(DataFilter<SubscriptionResponseDso, Request> data)
        {

            if (data.Value == null)
                return ConditionResult.ToError("Object share must have subscription data.");

            HashSet<string> status = [RequestStatus.Success.ToString(), RequestStatus.Processing.ToString()];

            var countRequests = await _injector.Context.Requests
                    .CountAsync(r => r.SubscriptionId == data.Value.Id
                    && status.Contains(r.Status)
                    && r.CreatedAt >= data.Value.CurrentPeriodStart && r.CreatedAt <= data.Value.CurrentPeriodEnd);

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
                Status = SubscriptionValidatorStates.IsAllowedRequests.ToInt()
            });
        }

        [RegisterConditionValidator(typeof(RequestValidatorStates), RequestValidatorStates.IsValid, "You have exhausted all allowed subscription requests.")]
        private async Task<ConditionResult> IsValid(DataFilter<SubscriptionResponseDso, Request> data)
        {
            if (data == null)
            {
                return ConditionResult.ToFailure(new ProblemDetails
                {
                    Title = "Not valid",
                    Detail = "Request is not valid",
                    Status = (int)RequestValidatorStates.IsValid
                }, "Request is not valid.");
            }

            if (data?.Items?.ContainsKey("serviceId") != true)
                return ConditionResult.ToError("Item must contains serviceId");

            var resultAllowed = await IsAllowedRequests(data);
            if (resultAllowed.Success == false)
            {
                return resultAllowed;

            }


            if (_checker.CheckAndResult(TokenValidatorStates.IsServiceIdFound, data.Items["serviceId"].ToString()) is { Success: false } resultService)
            {
                return resultService;
            }

            if (await _checker.CheckAndResultAsync(ServiceValidatorStates.IsIn, new DataFilter(data.Items["serviceId"].ToString())
            {
                Value = new List<string> { ServiceType.Space, ServiceType.Dash }
            }) is { Success: true } res)
            {
                return ConditionResult.ToFailure(new ProblemDetails
                {
                    Title = "Coudn't create request",
                    Detail = "You can't create request with service Createspace or Dashboard",
                    Status = RequestValidatorStates.IsAllowed.ToInt()
                }, "You can't create request with service Createspace or Dashboard");
            }


            if (await _checker.CheckAndResultAsync(SessionValidatorStates.IsActive, new DataFilter(data.Items["sessionId"].ToString())) is { Success: false } result)
            {
                return result;
            }

            if (!_checker.Check(SpaceValidatorStates.HasSubscriptionId, new DataFilter
            {
                Id = data.Items["spaceId"].ToString(),
                Value = data.Value.Id
            }))
            {
                return ConditionResult.ToFailure(new ProblemDetails
                {
                    Title = "Not found",
                    Detail = "This space is not included in your subscription.",
                    Status = (int)SpaceValidatorStates.IsFound
                });
            }

            return ConditionResult.ToSuccess(resultAllowed.Result);
        }




        private bool IsFull(RequestRequestDso? context)
        {

            var result1 = _checker.Check(
                SpaceValidatorStates.IsFound,
                context.SpaceId);

            var result2 = _checker.Check(
                TokenValidatorStates.IsServiceIdFound,
                context.ServiceId);

            if (result1 == true && result2 == true)
                return true;
            return false;

            //return _checker.Check(
            //    SpaceValidatorStates.IsSpaceId,
            //    context.SpaceId
            //) && _checker.Check(
            //    ServiceValidatorStates.IsServiceId,
            //    context.ServiceId
            //);


            //&& 

            //_checker.Check(
            //    RequestValidatorStates.IsAllowedRequest,
            //    context.ServiceId
            //);
        }





        private bool IsValidRequestId(string requestId)
        {
            if (requestId != "")
            {
                var result = _checker.Injector.Context.Set<Request>()
                    .Any(x => x.Id == requestId);

                return result;
            }
            else
            {
                return false;

            }



        }
    }
}