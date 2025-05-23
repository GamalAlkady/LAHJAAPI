using AutoMapper;
using V1.BPR.Layers.Base;
using V1.DyModels.Dso.Requests;
using V1.DyModels.Dso.Responses;
using V1.DyModels.Dto.Share.Requests;
using V1.DyModels.Dto.Share.Responses;
using V1.Repositories.Share;

namespace V1.Services.Services
{
    public class AuthorizationSessionServiceService : BaseBPRServiceLayer<AuthorizationSessionServiceRequestDso, AuthorizationSessionServiceResponseDso, AuthorizationSessionServiceRequestShareDto, AuthorizationSessionServiceResponseShareDto>, IUseAuthorizationSessionServiceService
    {
        private readonly IAuthorizationSessionServiceShareRepository _share;
        public AuthorizationSessionServiceService(IMapper mapper, ILoggerFactory logger, IAuthorizationSessionServiceShareRepository bpr) : base(mapper, logger, bpr)
        {
            _share = bpr;
        }
    }
}