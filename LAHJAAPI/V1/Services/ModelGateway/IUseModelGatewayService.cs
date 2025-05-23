using AutoGenerator.Services.Base;
using V1.BPR.Layers.Base;
using V1.DyModels.Dso.Requests;
using V1.DyModels.Dso.Responses;

namespace V1.Services.Services
{
    public interface IUseModelGatewayService : IModelGatewayService<ModelGatewayRequestDso, ModelGatewayResponseDso>, IBaseService, IBaseBPRServiceLayer<ModelGatewayRequestDso, ModelGatewayResponseDso>
    {
    }
}