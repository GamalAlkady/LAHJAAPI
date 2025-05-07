using AutoGenerator.Repositories.Base;
using AutoGenerator.Services.Base;
using V1.DyModels.Dso.Requests;
using V1.DyModels.Dso.Responses;

namespace V1.Services.Services
{
    public interface IUseApplicationUserService : IApplicationUserService<ApplicationUserRequestDso, ApplicationUserResponseDso>, IBaseService//يمكنك  التزويد بكل  دوال   طبقة Builder   ببوابات  الطبقة   هذه نفسها
    //, IApplicationUserBuilderRepository<ApplicationUserRequestDso, ApplicationUserResponseDso>
    , IBasePublicRepository<ApplicationUserRequestDso, ApplicationUserResponseDso>
    {
        Task<IEnumerable<ServiceResponseDso>?> GetServices(string? userId = null);
        Task<IEnumerable<ModelAiResponseDso>?> GetModels(string? userId = null);
        Task<ApplicationUserResponseDso> GetUser();
        Task<ApplicationUserResponseDso> GetUserWithSubscription();
    }
}