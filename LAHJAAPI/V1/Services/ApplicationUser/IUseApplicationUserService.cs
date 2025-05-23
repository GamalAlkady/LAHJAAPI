using AutoGenerator.Services.Base;
using V1.BPR.Layers.Base;
using V1.DyModels.Dso.Requests;
using V1.DyModels.Dso.Responses;

namespace V1.Services.Services
{
    public interface IUseApplicationUserService : IApplicationUserService<ApplicationUserRequestDso, ApplicationUserResponseDso>, IBaseService, IBaseBPRServiceLayer<ApplicationUserRequestDso, ApplicationUserResponseDso>

    {
        Task<IEnumerable<ServiceResponseDso>?> GetServices(string? userId = null);
        Task<IEnumerable<ModelAiResponseDso>?> GetModels(string? userId = null);
        Task<ApplicationUserResponseDso> GetUser();
        Task<ApplicationUserResponseDso> GetUserWithSubscription();
        Task<UserModelAiResponseDso?> AssignModelAi(string modelAiId, string? userId = null);
        Task<UserServiceResponseDso?> AssignService(string serviceId, string? userId = null);
    }
}