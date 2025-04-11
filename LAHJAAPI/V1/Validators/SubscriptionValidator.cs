using AutoGenerator.Conditions;
using LAHJAAPI.Models;
using LAHJAAPI.V1.Validators.Conditions;
using Microsoft.AspNetCore.Mvc;
using V1.DyModels.Dso.ResponseFilters;

namespace LAHJAAPI.V1.Validators;

public enum SubscriptionValidatorStates
{
    IsSubscribe = 10000,
    IsActive = 10001,
    IsAllowedRequests = 10002,
    IsNotSubscribe = 6000,
    IsCancelAtPeriodEnd = 6001,
    IsCanceled = 6002,
    IsNotAllowed = 6003,
    IsSubscriptionId,
    IsCustomerId,
    HasStatus,
    IsValidStartDate,
    IsValidPeriodDates,
    IsIsAllowedSpaces,
    IsFull,
    IsValid
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
                nameof(SubscriptionValidatorStates.IsActive),
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
            SubscriptionValidatorStates.IsAllowedRequests,
            new LambdaCondition<SubscriptionResponseFilterDso>(
                nameof(SubscriptionValidatorStates.IsAllowedRequests),
                context => IsAllowedRequests(context),
                "You have exhausted all allowed subscription requests."
            )
        );

        _provider.Register(
              SubscriptionValidatorStates.IsSubscriptionId,
              new LambdaCondition<string>(
                  nameof(SubscriptionValidatorStates.IsSubscriptionId),
                  context => isSubscriptionId(context),
                  "Customer Id is required"
              )
          );


        _provider.Register(
            SubscriptionValidatorStates.IsCustomerId,
            new LambdaCondition<string>(
                nameof(SubscriptionValidatorStates.IsCustomerId),
                context => isCustomerId(context),
                "Customer Id is required"
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

        _provider.Register(
    SubscriptionValidatorStates.IsIsAllowedSpaces,
    new LambdaCondition<int>(
        nameof(SubscriptionValidatorStates.IsIsAllowedSpaces),
        context => isIsAllowedSpaces(context),
        "Space is not active"
    )
);
    }

    //async Task GetSubscription(SubscriptionResponseFilterDso context)
    //{
    //    Subscription = _dataContext.Subscriptions.FirstOrDefault(s => s.UserId == context.UserId);
    //    //?? throw new ArgumentNullException("No subscription found");
    //}

    bool isIsAllowedSpaces(int count)
    {


        var result = _checker.Injector.Context.Set<Subscription>()
        .Any(sub => sub.AllowedSpaces < count);

        return result;


    }

    bool IsAllowedSpaces(params object[] args)
    {
        if (args.Length != 2 || args[0] is not int count || args[1] is not string subId)
            return false;

        var result = _checker.Injector.Context.Set<Subscription>()
            .FirstOrDefault(x => x.Id == subId);

        return result?.AllowedSpaces >= count;
    }

    private bool isSubscriptionId(string subId)
    {
        var result = _checker.Injector.Context.Set<Subscription>()
         .Any(user => user.Id == subId);
        return result;
    }

    bool isCustomerId(string userId)
    {
        return _checker.Check(ApplicationUserValidatorStates.HasCustomerId, userId);

    }

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