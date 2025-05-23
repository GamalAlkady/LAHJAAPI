using AutoMapper;
using V1.BPR.Layers.Base;
using V1.DyModels.Dto.Build.Requests;
using V1.DyModels.Dto.Build.Responses;
using V1.DyModels.Dto.Share.Requests;
using V1.DyModels.Dto.Share.Responses;
using V1.Repositories.Builder;

namespace V1.Repositories.Share
{
    /// <summary>
    /// ApplicationUser class for ShareRepository.
    /// </summary>
    public class ApplicationUserShareRepository : BaseBPRShareLayer<ApplicationUserRequestShareDto, ApplicationUserResponseShareDto, ApplicationUserRequestBuildDto, ApplicationUserResponseBuildDto>, IApplicationUserShareRepository
    {
        // Declare the builder repository.
        private readonly ApplicationUserBuilderRepository _builder;
        public ApplicationUserShareRepository(IMapper mapper, ILoggerFactory logger, ApplicationUserBuilderRepository bpr) : base(mapper, logger, bpr)
        {
            _builder = bpr;
        }
    }
}