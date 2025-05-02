using AutoGenerator;
using AutoGenerator.Repositories.Base;
using AutoGenerator.Services.Base;
using V1.DyModels.Dso.Requests;
using V1.DyModels.Dso.Responses;
using V1.DyModels.VMs;

namespace V1.Services.Services
{
    public interface IUseModelAiService : IModelAiService<ModelAiRequestDso, ModelAiResponseDso>, IBaseService//يمكنك  التزويد بكل  دوال   طبقة Builder   ببوابات  الطبقة   هذه نفسها
    //, IModelAiBuilderRepository<ModelAiRequestDso, ModelAiResponseDso>
    , IBasePublicRepository<ModelAiRequestDso, ModelAiResponseDso>
    {
        Task<PagedResponse<ModelAiResponseDso>> FilterMaodelAi(ModelAiFilterVM searchModel);
    }
}