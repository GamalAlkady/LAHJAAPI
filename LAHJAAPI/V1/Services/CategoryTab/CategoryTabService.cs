using AutoMapper;
using V1.BPR.Layers.Base;
using V1.DyModels.Dso.Requests;
using V1.DyModels.Dso.Responses;
using V1.DyModels.Dto.Share.Requests;
using V1.DyModels.Dto.Share.Responses;
using V1.Repositories.Share;

namespace V1.Services.Services
{
    public class CategoryTabService : BaseBPRServiceLayer<CategoryTabRequestDso, CategoryTabResponseDso, CategoryTabRequestShareDto, CategoryTabResponseShareDto>, IUseCategoryTabService
    {
        private readonly ICategoryTabShareRepository _share;
        public CategoryTabService(IMapper mapper, ILoggerFactory logger, ICategoryTabShareRepository bpr) : base(mapper, logger, bpr)
        {
            _share = bpr;
        }
    }
}