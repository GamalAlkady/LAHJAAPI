using AutoGenerator;
using AutoGenerator.Repositories.Base;
using AutoGenerator.Services.Base;
using V1.DyModels.Dso.Requests;
using V1.DyModels.Dso.Responses;

namespace V1.Services.Services
{
    public interface IUseSubscriptionService : ISubscriptionService<SubscriptionRequestDso, SubscriptionResponseDso>, IBaseService//يمكنك  التزويد بكل  دوال   طبقة Builder   ببوابات  الطبقة   هذه نفسها
    //, ISubscriptionBuilderRepository<SubscriptionRequestDso, SubscriptionResponseDso>
    , IBasePublicRepository<SubscriptionRequestDso, SubscriptionResponseDso>
    {
        Task<int> AllowedRequests();
        Task<int> AvailableSpace(string? subscriptionId = null);
        Task<SubscriptionResponseDso> GetCustomerSubscription(string subscriptionId);
        Task<PagedResponse<SubscriptionResponseDso>> GetCustomerSubscriptions();
        Task<int> GetNumberRequests(string? subsriptionId = null, bool refresh = true);
        Task<SpaceResponseDso> GetSpace(string spaceId, string? subscriptionId = null);
        Task<PagedResponse<SpaceResponseDso>> GetSpaces(string? subscriptionId = null);
        Task<SubscriptionResponseDso> GetUserSubscription(string? subscriptionId = null);
        Task<bool> IsActive(string? subscriptionId = null);
        Task<bool> IsCancelAtPeriodEnd(string? subscriptionId = null);
        Task<bool> IsCanceled(string? subscriptionId = null);
        Task<(bool IsNotSubscribed, object? Result)> IsNotSubscribe(string? subscriptionId = null);
        Task<bool> IsSpaceFound(string spaceId, string? subscriptionId = null);
        Task<bool> IsSubscribe(string? subscriptionId = null);
    }
}