using AutoGenerator.Conditions;
using LAHJAAPI.Models;
using LAHJAAPI.V1.Enums;
using LAHJAAPI.V1.Validators.Conditions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LAHJAAPI.V1.Validators;

public enum SubscriptionValidatorStates
{
    IsSubscribe = 10000,
    IsActive = 10001,
    IsAllowedRequests = 10002,
    IsNotSubscribe = 6000,
    IsCancelAtPeriodEnd = 6001,
    IsCanceled = 6002,
    IsNotAllowedRequests = 6003,
    IsSubscriptionId,
    IsCustomerId,
    HasStatus,
    IsValidStartDate,
    IsValidPeriodDates,
    IsAvailableSpaces,
    IsFull,
    IsValid,
    IsActiveAndResult,
    FindSubscription,
}


public class SubscriptionValidator : ValidatorContext<Subscription, SubscriptionValidatorStates>
{
    private readonly IConditionChecker _checker;
    private Subscription? Subscription { get; set; }
    public SubscriptionValidator(IConditionChecker checker) : base(checker)
    {
        _checker = checker;
        //_dataContext = checker.Injector.DataContext;
        //checker.Injector.DataContext = this;
    }


    protected override void InitializeConditions()
    {
    }

    async Task<Subscription> GetSubscription()
    {
        Subscription ??= await _checker.Injector.Context.Set<Subscription>()
            .Where(s => s.UserId == _checker.Injector.UserClaims.UserId)
            .FirstOrDefaultAsync();
        return Subscription;
    }

    bool IsAllowedSpaces(int count)
    {
        var result = _checker.Injector.Context.Set<Subscription>()
            .Where(x => x.UserId == _checker.Injector.UserClaims.UserId)
             .Select(s => new
             {
                 s.AllowedSpaces,
                 SpaceCount = s.Spaces.Count()
             }).FirstOrDefault();


        return result.AllowedSpaces >= result.SpaceCount;
    }

    //TODO: Check spaces from plan features by build validator
    [RegisterConditionValidator(typeof(SubscriptionValidatorStates), SubscriptionValidatorStates.IsAvailableSpaces, "Spaces are not avaliable")]
    async Task<ConditionResult> IsSpacesAvailableAsync(DataFilter<string, Subscription> data)
    {
        data.Share ??= await GetModel(null);
        var countSpaces = await _checker.Injector.Context.Spaces.CountAsync(x => x.SubscriptionId == data.Share.Id);
        data.Value = countSpaces.ToString();
        if ((data.Share?.AllowedSpaces > countSpaces))
            return ConditionResult.ToSuccess(data.Share, "Not Available");
        return ConditionResult.ToFailure(new ProblemDetails
        {
            Title = "Coudn't create space",
            Detail = "You have exhausted all allowed subscription spaces.",
            Status = SubscriptionValidatorStates.IsAvailableSpaces.ToInt()
        });
    }


    [RegisterConditionValidator(typeof(SubscriptionValidatorStates), SubscriptionValidatorStates.IsActive, "Subscription not active", IsCachability = true)]
    private async Task<ConditionResult> IsActive(DataFilter<string, Subscription> data)
    {
        //GetSubscription(context);
        if (data.Share == null) return ConditionResult.ToFailure(null, "Subscription is null.Please provide correct id or share");
        if (data.Share.Status!.Equals("active", StringComparison.OrdinalIgnoreCase))
            return ConditionResult.ToSuccess(data.Share, "Subscription is not active");
        return ConditionResult.ToFailure(new ProblemDetails
        {
            Title = "Subscription is not active",
            Detail = "You are not subscription",
            Status = (int)SubscriptionValidatorStates.IsNotSubscribe
        }, "Subscription is null.Please provide id or share");
    }

    [RegisterConditionValidator(typeof(SubscriptionValidatorStates), SubscriptionValidatorStates.IsCanceled, "Subscription is canceled")]
    private Task<ConditionResult> IsCanceled(DataFilter<string, Subscription> data)
    {
        if (data.Share == null) return ConditionResult.ToFailureAsync(null, "Subscription is null.Please provide correct id or share");
        return Task.FromResult(new ConditionResult(Subscription.Status!.Equals("canceled", StringComparison.OrdinalIgnoreCase), Subscription, "Subscription is canceled"));
    }

    [RegisterConditionValidator(typeof(SubscriptionValidatorStates), SubscriptionValidatorStates.IsCancelAtPeriodEnd, "Subscription is will cancel at period end")]
    private Task<ConditionResult> IsCancelAtPeriodEnd(DataFilter<string, Subscription> data)
    {
        if (data.Share == null) return ConditionResult.ToFailureAsync(null, "Subscription is null.Please provide correct id or share");
        return Task.FromResult(new ConditionResult(data.Share.CancelAtPeriodEnd, new ProblemDetails
        {
            Title = "Subscription will  cancel at period end",
            Detail = "Subscription is canceled. Do you want to renew it?",
            Status = (int)SubscriptionValidatorStates.IsCancelAtPeriodEnd
        }, "Subscription is will cancel at period end"));
    }
    [RegisterConditionValidator(typeof(SubscriptionValidatorStates), SubscriptionValidatorStates.IsAllowedRequests, "You have exhausted all allowed subscription requests.")]
    async Task<ConditionResult> IsAllowedRequests(DataFilter<string, Subscription> data)
    {
        if (data.Share == null) return ConditionResult.ToFailure(null, "Subscription is null.Please provide correct id or share");
        var requests = await _checker.Injector.Context.Requests
            .CountAsync(r => r.SubscriptionId == data.Share.Id
            && r.Status == RequestStatus.Success.ToString()
            && r.CreatedAt >= data.Share.CurrentPeriodStart && r.CreatedAt <= data.Share.CurrentPeriodEnd);

        return new ConditionResult(requests < data.Share.AllowedRequests, data.Share, "You have exhausted all allowed subscription requests.");
    }

    [RegisterConditionValidator(typeof(SubscriptionValidatorStates), SubscriptionValidatorStates.IsSubscribe, "You are not subscriped.")]
    async Task<ConditionResult> IsSubscribeAsync(DataFilter<string, Subscription> data)
    {
        var checks = new Func<DataFilter<string, Subscription>, Task<ConditionResult>>[]
    {
        IsCancelAtPeriodEnd,
        IsCanceled,
    };

        foreach (var check in checks)
        {
            var result = await check(data);
            if (result.Success == true)
            {
                result.Success = false;
                return result;
            }
        }

        return await IsActive(data);
    }

    [RegisterConditionValidator(typeof(SubscriptionValidatorStates), SubscriptionValidatorStates.IsNotSubscribe, "You are not subscriped.")]
    async Task<ConditionResult> IsNotSubscribeAsync(DataFilter<string, Subscription> data)
    {
        var result = await IsSubscribeAsync(data);
        result.Success = !result.Success;
        return result;
    }

    protected override async Task<Subscription?> GetModel(string? id)
    {
        var userId = _injector.UserClaims.UserId;

        //if (_subscription is { Id: var id1, UserId: var userId } && (id1 == id || userId == id))
        if (Subscription != null && (Subscription.Id == id || Subscription.UserId == userId))
            return Subscription;

        if (id is null || id.Equals("userId", StringComparison.CurrentCultureIgnoreCase))
            return await GetSubscription();

        Subscription = await base.GetModel(id);
        return Subscription;
    }
}