using AutoGenerator;
using AutoGenerator.Services.Base;
using V1.BPR.Layers.Base;
using V1.DyModels.Dso.Requests;
using V1.DyModels.Dso.Responses;

namespace V1.Services.Services
{
    public interface IUseSubscriptionService : ISubscriptionService<SubscriptionRequestDso, SubscriptionResponseDso>, IBaseService, IBaseBPRServiceLayer<SubscriptionRequestDso, SubscriptionResponseDso>

    {
        //Task<int> AvailableSpace(string? subscriptionId = null);
        //Task<SubscriptionResponseDso> GetSubscriptionByCustomer(string subscriptionId, string? customerId = null);
        Task<PagedResponse<SubscriptionResponseDso>> GetSubscriptionsByCustomer(string? customerId = null);
        Task<int> GetNumberRequests(bool refresh = true, string? subscriptionId = null);
        Task<SpaceResponseDso> GetSpace(string spaceId, string? subscriptionId = null);
        Task<PagedResponse<SpaceResponseDso>> GetSpaces(string? subscriptionId = null);
        Task<SubscriptionResponseDso> GetUserSubscription(string? subscriptionId = null);
        Task<bool> IsActive(string? subscriptionId = null);
        Task<bool> IsCancelAtPeriodEnd(string? subscriptionId = null);
        Task<bool> IsCanceled(string? subscriptionId = null);
        //Task<(bool IsNotSubscribed, object? Result)> IsNotSubscribe(string? subscriptionId = null);
        //Task<bool> IsSpaceFound(string spaceId, string? subscriptionId = null);
        Task<bool> IsSubscribe(string? subscriptionId = null);
        Task<string?> AllowedRequests(bool refresh = true, string? subscriptionId = null);
    }
}