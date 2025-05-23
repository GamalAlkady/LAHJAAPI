using AutoMapper;
using V1.BPR.Layers.Base;
using V1.DyModels.Dso.Requests;
using V1.DyModels.Dso.Responses;
using V1.DyModels.Dto.Share.Requests;
using V1.DyModels.Dto.Share.Responses;
using V1.Repositories.Share;

namespace V1.Services.Services
{
    public class AdvertisementService : BaseBPRServiceLayer<AdvertisementRequestDso, AdvertisementResponseDso, AdvertisementRequestShareDto, AdvertisementResponseShareDto>, IUseAdvertisementService
    {
        private readonly IAdvertisementShareRepository _share;
        public AdvertisementService(IMapper mapper, ILoggerFactory logger, IAdvertisementShareRepository bpr) : base(mapper, logger, bpr)
        {
            _share = bpr;
        }
    }
}