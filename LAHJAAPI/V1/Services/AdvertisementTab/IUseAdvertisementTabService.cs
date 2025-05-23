using AutoGenerator.Services.Base;
using V1.BPR.Layers.Base;
using V1.DyModels.Dso.Requests;
using V1.DyModels.Dso.Responses;

namespace V1.Services.Services
{
    public interface IUseAdvertisementTabService : IAdvertisementTabService<AdvertisementTabRequestDso, AdvertisementTabResponseDso>, IBaseService, IBaseBPRServiceLayer<AdvertisementTabRequestDso, AdvertisementTabResponseDso>
    {
    }
}