using LAHJAAPI.Models;
using LAHJAAPI.V1.Validators.Conditions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using V1.DyModels.VMs;
using WasmAI.ConditionChecker.Base;


namespace LAHJAAPI.V1.Validators;

public enum SubscriptionValidatorStates
{
    IsSubscribe = 10000,
    IsActive = 10001,
    IsFullAllowedRequests = 10002,
    IsBelongToUser = 10003,
    IsNotSubscribe,
    IsCancelAtPeriodEnd,
    IsCanceled,
    IsSubscriptionId,
    HasStatus,
    HasAllowedSpaces,
    IsExpireAfter,
    IsAllowedRequests,
    HasPlanId,
    HasCanceledAt,
    HasCancelAt,
    HasBillingPeriod,
    IsFree,
    HasCustomerId,
    HasId,
}


public class SubscriptionValidator : ValidatorContext<Subscription, SubscriptionValidatorStates>
{
    private Subscription? Subscription { get; set; }
    public SubscriptionValidator(IConditionChecker checker) : base(checker)
    {
    }


    protected override void InitializeConditions()
    {
    }

    [RegisterConditionValidator(typeof(SubscriptionValidatorStates), SubscriptionValidatorStates.HasId, "Subscription ID is required.")]
    private Task<ConditionResult> ValidateId(DataFilter<string, Subscription> f)
    {
        bool valid = !string.IsNullOrWhiteSpace(f.Share?.Id);
        return valid ? ConditionResult.ToSuccessAsync(f.Share) : ConditionResult.ToFailureAsync("Subscription ID is required.");
    }

    [RegisterConditionValidator(typeof(SubscriptionValidatorStates), SubscriptionValidatorStates.HasCustomerId, "Customer ID is required.")]
    private Task<ConditionResult> ValidateCustomerId(DataFilter<string, Subscription> f)
    {
        bool valid = !string.IsNullOrWhiteSpace(f.Share?.CustomerId) && f.Share.CustomerId == f.Value;
        return valid ? ConditionResult.ToSuccessAsync(f.Share) : ConditionResult.ToFailureAsync("Customer ID is required.");
    }




    [RegisterConditionValidator(typeof(SubscriptionValidatorStates), SubscriptionValidatorStates.IsFree, "Subscription type check failed.")]
    private Task<ConditionResult> ValidateIsFree(DataFilter<bool, Subscription> f)
    {
        bool valid = f.Share?.IsFree == true;
        return Task.FromResult(valid ? ConditionResult.ToSuccess(f.Share?.IsFree) : ConditionResult.ToFailure("Subscription is not free."));
    }

    [RegisterConditionValidator(typeof(SubscriptionValidatorStates), SubscriptionValidatorStates.HasStatus, "Status is required.")]
    private Task<ConditionResult> ValidateStatus(DataFilter<string, Subscription> f)
    {
        bool valid = !string.IsNullOrWhiteSpace(f.Share?.Status);
        return valid ? ConditionResult.ToSuccessAsync(f.Share?.Status) : ConditionResult.ToFailureAsync("Status is required.");
    }

    [RegisterConditionValidator(typeof(SubscriptionValidatorStates), SubscriptionValidatorStates.HasBillingPeriod, "Billing Period format is invalid.")]
    private Task<ConditionResult> ValidateBillingPeriod(DataFilter<string, Subscription> f)
    {
        var period = f.Share?.BillingPeriod;
        bool valid = period == null || !string.IsNullOrWhiteSpace(period);
        return Task.FromResult(valid ? ConditionResult.ToSuccess(period) : ConditionResult.ToFailure("Billing Period cannot be empty if provided."));
    }

    [RegisterConditionValidator(typeof(SubscriptionValidatorStates), SubscriptionValidatorStates.IsCancelAtPeriodEnd, "Cancel At Period End flag check failed.")]
    private Task<ConditionResult> ValidateIsCancelAtPeriodEnd(DataFilter<bool, Subscription> f)
    {
        bool valid = f.Share?.CancelAtPeriodEnd == true;
        return Task.FromResult(valid ? ConditionResult.ToSuccess(f.Share?.CancelAtPeriodEnd) : ConditionResult.ToFailure("Subscription is not set to cancel at period end."));
    }


    [RegisterConditionValidator(typeof(SubscriptionValidatorStates), SubscriptionValidatorStates.HasCancelAt, "Cancel At Date is required.")]
    private Task<ConditionResult> ValidateCancelAt(DataFilter<DateTime?, Subscription> f)
    {
        bool valid = f.Share?.CancelAt != null;
        return Task.FromResult(valid ? ConditionResult.ToSuccess(f.Share?.CancelAt) : ConditionResult.ToFailure("Cancel At Date is required."));
    }

    [RegisterConditionValidator(typeof(SubscriptionValidatorStates), SubscriptionValidatorStates.HasCanceledAt, "Canceled At Date is required.")]
    private Task<ConditionResult> ValidateCanceledAt(DataFilter<DateTime?, Subscription> f)
    {
        bool valid = f.Share?.CanceledAt != null;
        return Task.FromResult(valid ? ConditionResult.ToSuccess(f.Share?.CanceledAt) : ConditionResult.ToFailure("Canceled At Date is required."));
    }

    [RegisterConditionValidator(typeof(SubscriptionValidatorStates), SubscriptionValidatorStates.HasPlanId, "Plan ID is required.")]
    private Task<ConditionResult> ValidatePlanId(DataFilter<string, Subscription> f)
    {
        bool valid = !string.IsNullOrWhiteSpace(f.Share?.PlanId);
        return valid ? ConditionResult.ToSuccessAsync(f.Share?.PlanId) : ConditionResult.ToFailureAsync("Plan ID is required.");
    }

    [RegisterConditionValidator(typeof(SubscriptionValidatorStates), SubscriptionValidatorStates.IsBelongToUser, "Subscription is not belong to user")]
    async Task<ConditionResult> IsBelongToUserAsync(DataFilter<string, Subscription> data)
    {
        if (data.Items == null || data.Items.TryGetValue("customerId", out object? customerId) == false)
            return ConditionResult.ToError("Items must contains customerId");
        if (data.Share == null) return ConditionResult.ToError("You don't have subscription.");
        if (data.Share.CustomerId != customerId.ToString())
            return ConditionResult.ToFailure(new ProblemDetails
            {
                Title = "Subscription not belong to you",
                Detail = "Subscription not belong to you",
                Status = (int)SubscriptionValidatorStates.IsBelongToUser
            }, "Subscription is not belong to user");

        return ConditionResult.ToSuccess(data.Share, "Subscription is belong to user");
    }

    [RegisterConditionValidator(typeof(SubscriptionValidatorStates), SubscriptionValidatorStates.IsExpireAfter, "Subscription is will expire")]
    async Task<ConditionResult> IsExpireAfterAsync(DataFilter<object, Subscription> data)
    {
        try
        {
            if (data.Value is not (int or List<int>))
                return ConditionResult.ToFailure(null, "Value must array or integer");

            var users = await QueryListAsync<ApplicationUser>(dbSet => dbSet
                   .Include(u => u.Subscription) // يفضل استخدام Include اللامدا بدلاً من الاسم النصي لسلامة النوع (type safety)
                   .Where(u =>
                       u.Subscription != null
                       && u.Subscription.Status == Stripe.SubscriptionStatuses.Active
                       && u.Subscription.CancelAtPeriodEnd == false
                   )
               );
            //&& u.Subscription.Plan.BillingPeriod == null
            List<ApplicationUserFilterVM> userFilters = new();
            int? valInt = null;
            List<int>? valList = null;
            if (data.Value is int)
            {
                valInt = Convert.ToInt32(data.Value);
                if (valInt <= 0)
                    return ConditionResult.ToFailure(null, "Value must be greater than 0");
            }
            else if (data.Value is List<int>)
            {
                valList = (List<int>)data.Value;
                if (valList.Count == 0)
                    return ConditionResult.ToFailure(null, "Value must has one element at least.");
            }

            foreach (var user in users)
            {
                var endDate = user.Subscription.CurrentPeriodEnd;
                var daysLeft = (endDate - DateTime.UtcNow).TotalDays;
                var val = Convert.ToInt32(Math.Floor(daysLeft));

                if ((valInt != null && valInt == val) || (valList != null && valList.Contains(val)))
                {
                    userFilters.Add(new ApplicationUserFilterVM
                    {
                        Id = user.Id,
                        Email = user.Email,
                        SubscriptionId = user.Subscription.Id,
                        SubscriptionStatus = user.Subscription.Status,
                        //SubscriptionPlanId = user.Subscription.PlanId,
                        //SubscriptionPlanName = user.Subscription.Plan.ProductName,
                        Days = val
                    });
                }
            }

            if (userFilters.Count > 0)
            {
                return ConditionResult.ToSuccess(userFilters, "Subscription is will expire");
            }


            return ConditionResult.ToFailure(new ProblemDetails
            {
                Title = "Subscription is will not expire",
                Detail = "Subscription is will not expire",
                Status = (int)SubscriptionValidatorStates.IsExpireAfter
            }, "Subscription is will expire");
        }
        catch (Exception ex)
        {
            return ConditionResult.ToError(ex.Message);
        }
    }


    [RegisterConditionValidator(typeof(SubscriptionValidatorStates), SubscriptionValidatorStates.HasAllowedSpaces, "Spaces are not avaliable")]
    async Task<ConditionResult> ValidateAllowedSpaces(DataFilter<string, Subscription> data)
    {
        try
        {
            if (data.Share == null) return ConditionResult.ToFailure(null, "You don't have subscription.");

            if (await _checker.CheckAsync(SpaceValidatorStates.IsAvailable, new DataFilter
            {
                Items = new Dictionary<string, object> {
                 { "subscriptionId", data.Share?.Id! },
                { "planId", data.Share?.PlanId! }
            },
            }))
            {
                return ConditionResult.ToSuccess(data.Share, "Not Available");
            }
            return ConditionResult.ToFailure(new ProblemDetails
            {
                Title = "Coudn't create space",
                Detail = "You have exhausted all allowed subscription spaces.",
                Status = SubscriptionValidatorStates.HasAllowedSpaces.ToInt()
            });
        }
        catch { throw; }
    }


    [RegisterConditionValidator(typeof(SubscriptionValidatorStates), SubscriptionValidatorStates.IsActive, "Subscription not active", IsCachability = true)]
    private async Task<ConditionResult> IsActive(DataFilter<string, Subscription> data)
    {
        //data.Share ??= await GetModel(null);
        if (data.Share == null) return ConditionResult.ToError("You have no subscription.");

        if (data.Share.Status!.Equals("active", StringComparison.OrdinalIgnoreCase))
            return ConditionResult.ToSuccess(data.Share, "Subscription is not active");
        return ConditionResult.ToFailure(new ProblemDetails
        {
            Title = "Subscription is not active",
            Detail = "You are not subscription",
            Status = (int)SubscriptionValidatorStates.IsActive
        }, "Subscription is null.Please provide id or share");
    }

    [RegisterConditionValidator(typeof(SubscriptionValidatorStates), SubscriptionValidatorStates.IsCanceled, "Subscription is canceled")]
    private async Task<ConditionResult> IsCanceled(DataFilter<string, Subscription> data)
    {
        data.Share ??= await GetModel(null);
        if (data.Share == null) return ConditionResult.ToError("You have no subscription.");
        if (data.Share.Status!.Equals("canceled", StringComparison.OrdinalIgnoreCase))
            return ConditionResult.ToSuccess(new ProblemDetails
            {
                Title = "Subscription is canceled.",
                Detail = $"Subscription has canceled at {data.Share.CanceledAt}",
                Status = (int)SubscriptionValidatorStates.IsCanceled
            }, $"Subscription has canceled at {data.Share.CanceledAt}");
        return ConditionResult.ToFailure(data.Share, "Subscription is not canceled.");
    }

    [RegisterConditionValidator(typeof(SubscriptionValidatorStates), SubscriptionValidatorStates.IsCancelAtPeriodEnd, "Subscription is will cancel at period end")]
    private async Task<ConditionResult> IsCancelAtPeriodEnd(DataFilter<string, Subscription> data)
    {
        data.Share ??= await GetModel(null);
        if (data.Share == null) return ConditionResult.ToError("You have no subscription.");

        if (data.Share.CancelAtPeriodEnd)
            return ConditionResult.ToSuccess(new ProblemDetails
            {
                Title = "Subscription has canceled at " + data.Share.CanceledAt,
                Detail = "Subscription is canceled. Use endpoint renew if you want to renew it.",
                Status = (int)SubscriptionValidatorStates.IsCancelAtPeriodEnd
            }, "Subscription is will cancel at period end");

        return ConditionResult.ToFailure(data.Share, "Subscription is will not canceled at end.");
    }

    [RegisterConditionValidator(typeof(SubscriptionValidatorStates), SubscriptionValidatorStates.IsFullAllowedRequests, "You have exhausted all allowed subscription requests.")]
    private async Task<ConditionResult> ValidateAllowedRequests(DataFilter<string, Subscription> data)
    {
        if (data.Items == null || !data.Items.TryGetValue("serviceId", out object? serviceId))
            return ConditionResult.ToError("Items must contains serviceId");

        if (!data.Items.TryGetValue("sessionId", out object? sessionId))
            return ConditionResult.ToError("Items must contains sessionId");

        if (!data.Items.TryGetValue("spaceId", out object? spaceId))
            return ConditionResult.ToError("Items must contains spaceId");

        // Check if the service is Createspace or dashbaord
        if (await _checker.CheckAndResultAsync(ServiceValidatorStates.IsInAbsolutePath,
            new DataFilter(serviceId.ToString())
            {
                Value = new List<string> { ServiceType.CreateSpace, ServiceType.Dashboard }
            }) is { Success: false } res3)
        {
            return ConditionResult.ToFailure(new ProblemDetails
            {
                Title = "Coudn't create request",
                Detail = $"You can't create request with service {res3.Result}",
                Status = RequestValidatorStates.IsAllowed.ToInt()
            }, $"You can't create request with service {res3.Result}");
        }

        // Check if the session is active
        if (await _checker.CheckAndResultAsync(AuthorizationSessionValidatorStates.IsActive, new DataFilter(sessionId.ToString())) is { Success: false } result)
        {
            return result;
        }

        if (await _checker.CheckAndResultAsync(SpaceValidatorStates.HasSubscriptionId, new DataFilter
        {
            Id = spaceId.ToString(),
            Value = data.Id
        }) is { Success: false } res2)
        {
            return res2;
        }

        return await IsAllowedRequests(data);
    }


    [RegisterConditionValidator(typeof(SubscriptionValidatorStates), SubscriptionValidatorStates.IsAllowedRequests, "You have exhausted all allowed subscription requests.")]
    private async Task<ConditionResult> IsAllowedRequests(DataFilter<string, Subscription> filter)
    {
        if (filter.Share == null) return ConditionResult.ToError("You don't have subscription!");
        var result = await _checker.CheckAndResultAsync(RequestValidatorStates.IsAllowed, new DataFilter
        {
            Value = filter.Share
        });

        var request = (RequestFilterVM)result.Result!;

        if (result.Success == false)
        {
            return ConditionResult.ToFailure(new SubscriptionFilterVM
            {
                Id = filter.Id,
                AllowedRequests = request.AllowedRequests,
                NumberRequests = request.NumberRequests,
            }, result.Message);
        }

        return ConditionResult.ToSuccess(new SubscriptionFilterVM
        {
            Id = filter.Id,
            AllowedRequests = request.AllowedRequests,
            NumberRequests = request.NumberRequests,
        }, "You have allowed requests.");
    }


    [RegisterConditionValidator(typeof(SubscriptionValidatorStates), SubscriptionValidatorStates.IsSubscribe, "You are not subscriped.")]
    async Task<ConditionResult> IsSubscribeAsync(DataFilter<string, Subscription> data)
    {
        if (data.Share == null) return ConditionResult.ToError("You have no subscription.");
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
        try
        {
            if (id is null) return null;
            //if (_subscription is { Id: var id1, UserId: var userId } && (id1 == id || userId == id))
            if (Subscription != null && Subscription.Id == id)
                return Subscription;

            Subscription = await base.GetModel(id);
            return Subscription;
        }
        catch (Exception)
        {
            throw;
        }
    }
}