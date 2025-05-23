using AutoGenerator.Services.Base;
using V1.BPR.Layers.Base;
using V1.DyModels.Dso.Requests;
using V1.DyModels.Dso.Responses;

namespace V1.Services.Services
{
    public interface IUseServiceService : IServiceService<ServiceRequestDso, ServiceResponseDso>, IBaseService, IBaseBPRServiceLayer<ServiceRequestDso, ServiceResponseDso>
    {
        Task<ServiceResponseDso> GetByAbsolutePath(string absolutePath);
        new Task<ServiceResponseDso> GetByName(string name);
        Task<IEnumerable<ServiceResponseDso>> GetListWithoutSome(List<string>? servicesId = null, string? modelId = null);
        Task<IEnumerable<ServiceResponseDso>> GetUserServices(string userId);
    }
}