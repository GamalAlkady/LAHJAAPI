using AutoMapper;
using V1.BPR.Layers.Base;
using V1.DyModels.Dso.Requests;
using V1.DyModels.Dso.Responses;
using V1.DyModels.Dto.Share.Requests;
using V1.DyModels.Dto.Share.Responses;
using V1.Repositories.Share;

namespace V1.Services.Services
{
    public class AdvertisementTabService : BaseBPRServiceLayer<AdvertisementTabRequestDso, AdvertisementTabResponseDso, AdvertisementTabRequestShareDto, AdvertisementTabResponseShareDto>, IUseAdvertisementTabService
    {
        private readonly IAdvertisementTabShareRepository _share;
        public AdvertisementTabService(IMapper mapper, ILoggerFactory logger, IAdvertisementTabShareRepository bpr) : base(mapper, logger, bpr)
        {
            _share = bpr;
        }
    }
}