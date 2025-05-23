using AutoMapper;
using V1.BPR.Layers.Base;
using V1.DyModels.Dso.Requests;
using V1.DyModels.Dso.Responses;
using V1.DyModels.Dto.Share.Requests;
using V1.DyModels.Dto.Share.Responses;
using V1.Repositories.Share;

namespace V1.Services.Services
{
    public class UserModelAiService : BaseBPRServiceLayer<UserModelAiRequestDso, UserModelAiResponseDso, UserModelAiRequestShareDto, UserModelAiResponseShareDto>, IUseUserModelAiService
    {
        private readonly IUserModelAiShareRepository _share;
        public UserModelAiService(IMapper mapper, ILoggerFactory logger, IUserModelAiShareRepository bpr) : base(mapper, logger, bpr)
        {
            _share = bpr;
        }
    }
}