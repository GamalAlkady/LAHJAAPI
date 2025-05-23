using AutoGenerator.Services.Base;
using V1.BPR.Layers.Base;
using V1.DyModels.Dso.Requests;
using V1.DyModels.Dso.Responses;

namespace V1.Services.Services
{
    public interface IUseSpaceService : ISpaceService<SpaceRequestDso, SpaceResponseDso>, IBaseService, IBaseBPRServiceLayer<SpaceRequestDso, SpaceResponseDso>

    {
        Task<SpaceResponseDso> GetSpaceBySubscriptionId(string subscriptionId, string spaceId);
        Task<IEnumerable<SpaceResponseDso>> GetSpacesBySubscriptionId(string subscriptionId);
    }
}