using AutoBuilderApiCore.V1.Validators;
using AutoGenerator.Conditions;
using Microsoft.AspNetCore.Mvc;
using V1.DyModels.Dso.ResponseFilters;

namespace V1.Validators
{
    public enum SubscriptionValidatorStates
    {
        IsSubscribe = 10000,
        IsActive = 10001,
        IsAllowed = 10002,
        IsNotSubscribe = 6000,
        IsCancelAtPeriodEnd = 6001,
        IsCanceled = 6002,
        IsNotAllowed = 6003,
    }


    public class SubscriptionValidator : BaseValidator<SubscriptionResponseFilterDso, SubscriptionValidatorStates>, ITValidator
    {
        private readonly IConditionChecker _checker;
        public SubscriptionValidator(IConditionChecker checker) : base(checker)
        {
            _checker = checker;
            //_dataContext = checker.Injector.DataContext;
            //checker.Injector.DataContext = this;
        }


        protected override void InitializeConditions()
        {
            _provider.Register(
                SubscriptionValidatorStates.IsActive,
                new LambdaCondition<SubscriptionResponseFilterDso>(
                    nameof(SubscriptionValidatorStates.IsActive),
                    context => IsActive(context),
                    "Subscription is not active"
                )
            );

            _provider.Register(
                SubscriptionValidatorStates.IsCanceled,
                new LambdaCondition<SubscriptionResponseFilterDso>(
                    nameof(SubscriptionValidatorStates.IsCanceled),
                    context => context.Status!.Equals(SubscriptionValidatorStates.IsCanceled.ToString(), StringComparison.OrdinalIgnoreCase),
                    "Your subscription has canceled"
                )
            );

            _provider.Register(
                SubscriptionValidatorStates.IsCancelAtPeriodEnd,
                new LambdaCondition<SubscriptionResponseFilterDso>(
                    nameof(SpaceValidatorStates.IsActive),
                    context => context.CancelAtPeriodEnd,
                    "Subscription is canceled. Do you want to renew it?"
                )
            );


            _provider.Register(
                SubscriptionValidatorStates.IsSubscribe,
                new LambdaCondition<SubscriptionResponseFilterDso>(
                    nameof(SubscriptionValidatorStates.IsSubscribe),
                    context => IsSubscribe(context),
                    "You are not subscription"
                )
            );

            _provider.Register(
                SubscriptionValidatorStates.IsNotSubscribe,
                new LambdaCondition<SubscriptionResponseFilterDso>(
                    nameof(SubscriptionValidatorStates.IsNotSubscribe),
                    context => IsNotSubscribe(context),
                    "You are not subscription"
                )
            );

            _provider.Register(
                SubscriptionValidatorStates.IsAllowed,
                new LambdaCondition<SubscriptionResponseFilterDso>(
                    nameof(SubscriptionValidatorStates.IsAllowed),
                    context => IsAllowedRequests(context),
                    "You have exhausted all allowed subscription requests."
                )
            );


            _provider.Register(
                SubscriptionValidatorStates.IsNotAllowed,
                new LambdaCondition<SubscriptionResponseFilterDso>(
                    nameof(SubscriptionValidatorStates.IsNotAllowed),
                    context => IsNotAllowedRequests(context),
                    "You have exhausted all allowed subscription requests."
                )
            );
        }

        //async Task GetSubscription(SubscriptionResponseFilterDso context)
        //{
        //    Subscription = _dataContext.Subscriptions.FirstOrDefault(s => s.UserId == context.UserId);
        //    //?? throw new ArgumentNullException("No subscription found");
        //}

        bool IsActive(SubscriptionResponseFilterDso context)
        {
            //await GetSubscription(context);
            return context.Status!.Equals("active", StringComparison.OrdinalIgnoreCase);
        }

        bool IsCanceled(SubscriptionResponseFilterDso context)
        {
            //await GetSubscription(context);
            return context.Status!.Equals("canceled", StringComparison.OrdinalIgnoreCase);
        }

        bool IsCancelAtPeriodEnd(SubscriptionResponseFilterDso context)
        {
            //await GetSubscription(context);
            return context.CancelAtPeriodEnd;
        }

        bool IsAllowedRequests(SubscriptionResponseFilterDso context)
        {
            //await GetSubscription(context);
            return context.AllowedRequests >= context.CountRequests;
        }

        bool IsNotAllowedRequests(SubscriptionResponseFilterDso context)
        {
            //await GetSubscription(context);
            return !IsAllowedRequests(context);
        }

        bool IsSubscribe(SubscriptionResponseFilterDso context)
        {
            return
                !IsCancelAtPeriodEnd(context) ||
                !IsCanceled(context) ||
                IsActive(context);
        }

        ProblemDetails? IsNotSubscribe(SubscriptionResponseFilterDso context)
        {
            if (IsCanceled(context))
            {
                return new ProblemDetails
                {
                    Title = "Subscription has canceled",
                    Detail = "Your subscription has canceled",
                    Status = (int)SubscriptionValidatorStates.IsCanceled,
                    //Type = "https://example.com/canceled"
                };
            }
            else if (IsCancelAtPeriodEnd(context))
            {
                return new ProblemDetails
                {
                    Title = "Subscription will cancel cancel at periodend",
                    Detail = "Subscription is canceled. Do you want to renew it?",
                    Status = (int)SubscriptionValidatorStates.IsCancelAtPeriodEnd
                };
            }
            else if (!IsActive(context))
            {
                return new ProblemDetails
                {
                    Title = "Subscription is not active",
                    Detail = "You are not subscription",
                    Status = (int)SubscriptionValidatorStates.IsNotSubscribe
                };
            }
            return null;
        }


        //async Task<bool> IsNotSubscribe(SubscriptionResponseFilterDso context)
        //{
        //    if (await IsCanceled(context))
        //    {
        //        return true;
        //    }
        //    else if (await IsCancelAtPeriodEnd(context))
        //    {
        //        return true;
        //    }
        //    else if (!await IsActive(context))
        //    {
        //        return true;
        //    }
        //    return false;
        //}


    }
}