using AutoGenerator.Conditions;
using LAHJAAPI.Models;
using LAHJAAPI.V1.Validators.Conditions;
using V1.DyModels.Dso.Requests;
using V1.DyModels.Dso.Responses;

namespace LAHJAAPI.V1.Validators
{



    //public enum ApplicationUserValidatorStates
    //{
    //    IsActive,
    //    IsFull,
    //    HasCustomerId,
    //    HasFirstName,
    //    HasLastName,
    //    HasDisplayName,
    //    HasProfileUrl,
    //    HasImageUrl,
    //    HasLastLoginIp,
    //    HasCreatedAt,
    //    HasUpdatedAt,
    //    IsNotArchived,
    //    HasArchivedDate,
    //    HasLastLoginDate


    //}
    public enum RequestValidatorStates
    {
        HasValidStatus,

        HasQuestion,
        IsValidRequestId,

        IsAllowedRequest,
        IsServiceId,
        IsSpaceId,
        IsUserId,
        IsFull,
        HasValidDate,
        HasServiceIdIfNeeded,
        HasSubscriptionIdIfNeeded,
    }

    public class RequestValidator : BaseValidator<RequestRequestDso, RequestValidatorStates>, ITValidator
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

            //_provider.Register(
            //    RequestValidatorStates.IsUserId,
            //    new LambdaCondition<RequestRequestDso>(
            //        nameof(RequestValidatorStates.IsUserId),
            //        context => !string.IsNullOrWhiteSpace(context.UserId),
            //        "User ID is required"
            //    )
            //);


            //_provider.Register(
            //     RequestValidatorStates.IsUserId,
            //     new LambdaCondition<string>(
            //         nameof(RequestValidatorStates.IsUserId),
            //         context => !string.IsNullOrWhiteSpace(context),
            //         "User ID is required"
            //     )
            // );



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
            //    RequestValidatorStates.HasServiceIdIfNeeded,
            //    new LambdaCondition<RequestRequestDso>(
            //        nameof(RequestValidatorStates.HasServiceIdIfNeeded),
            //        context => !string.IsNullOrWhiteSpace(context.ServiceId),
            //        "Service ID is required if not global"
            //    )
            //);

            //_provider.Register(
            //    RequestValidatorStates.HasSubscriptionIdIfNeeded,
            //    new LambdaCondition<RequestRequestDso>(
            //        nameof(RequestValidatorStates.HasSubscriptionIdIfNeeded),
            //        context => !string.IsNullOrWhiteSpace(context.SubscriptionId),
            //        "Subscription ID is required if not global"
            //    )
            //    );

            _provider.Register(
                RequestValidatorStates.IsAllowedRequest,
                new LambdaCondition<RequestRequestDso>(
                    nameof(RequestValidatorStates.IsAllowedRequest),
                    context => IsValidAllowedRequest(context.ServiceId),
                    "Service ID is required"
                )
            );

            _provider.Register(
                   RequestValidatorStates.IsValidRequestId,
                   new LambdaCondition<RequestResponseDso>(
                       nameof(RequestValidatorStates.IsValidRequestId),
                       context => IsValidRequestId(context.Id),
                       "Request ID is required"
                   )
               );

            _provider.Register(
                    RequestValidatorStates.IsValidRequestId,
                    new LambdaCondition<string>(
                        nameof(RequestValidatorStates.IsValidRequestId),
                        context => IsValidRequestId(context),
                        "Request ID is required"
                    )
                );

        }


        //private bool IsValidCustomerId(string userId)
        //{

        //    return _checker.Check(RequestValidatorStates.IsUserId, userId);
        //}



        private bool IsValidAllowedRequest(string servid)
        {
            return true;
        }

        private bool IsValidSpace(string spacId)
        {
            return true;
        }




        private bool IsFull(RequestRequestDso? context)
        {

            var result1 = _checker.Check(
                SpaceValidatorStates.IsSpaceId,
                context.SpaceId);

            var result2 = _checker.Check(
                ServiceValidatorStates.IsServiceId,
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