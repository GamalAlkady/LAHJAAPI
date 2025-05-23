using AutoGenerator.Services.Base;
using V1.BPR.Layers.Base;
using V1.DyModels.Dso.Requests;
using V1.DyModels.Dso.Responses;

namespace V1.Services.Services
{
    public interface IUseServiceMethodService : IServiceMethodService<ServiceMethodRequestDso, ServiceMethodResponseDso>, IBaseService, IBaseBPRServiceLayer<ServiceMethodRequestDso, ServiceMethodResponseDso>
    {
    }
}