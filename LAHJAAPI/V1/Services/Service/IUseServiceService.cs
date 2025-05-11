using AutoGenerator.Repositories.Base;
using AutoGenerator.Services.Base;
using V1.DyModels.Dso.Requests;
using V1.DyModels.Dso.Responses;

namespace V1.Services.Services
{
    public interface IUseServiceService : IServiceService<ServiceRequestDso, ServiceResponseDso>, IBaseService//يمكنك  التزويد بكل  دوال   طبقة Builder   ببوابات  الطبقة   هذه نفسها
    //, IServiceBuilderRepository<ServiceRequestDso, ServiceResponseDso>
    , IBasePublicRepository<ServiceRequestDso, ServiceResponseDso>
    {
        Task<ServiceResponseDso> GetByAbsolutePath(string absolutePath);
        Task<ServiceResponseDso> GetByName(string name);
        Task<List<ServiceResponseDso>> GetListWithoutSome(List<string>? servicesId = null, string? modelId = null);
        Task<List<ServiceResponseDso>> GetUserServices(string userId);
    }
}