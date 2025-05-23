using AutoGenerator;
using AutoGenerator.Services.Base;
using V1.BPR.Layers.Base;
using V1.DyModels.Dso.Requests;
using V1.DyModels.Dso.Responses;
using V1.DyModels.VMs;

namespace V1.Services.Services
{
    public interface IUseModelAiService : IModelAiService<ModelAiRequestDso, ModelAiResponseDso>, IBaseService
        , IBaseBPRServiceLayer<ModelAiRequestDso, ModelAiResponseDso>
    {
        Task<PagedResponse<ModelAiResponseDso>> FilterMaodelAi(ModelAiFilterVM searchModel);
    }
}