using AutoGenerator.Services.Base;
using V1.BPR.Layers.Base;
using V1.DyModels.Dso.Requests;
using V1.DyModels.Dso.Responses;

namespace V1.Services.Services
{
    public interface IUseUserModelAiService : IUserModelAiService<UserModelAiRequestDso, UserModelAiResponseDso>, IBaseService, IBaseBPRServiceLayer<UserModelAiRequestDso, UserModelAiResponseDso>
    {
    }
}