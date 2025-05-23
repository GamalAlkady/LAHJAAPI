using AutoGenerator.Repositories.Share;
using V1.BPR.Layers.Base;
using V1.DyModels.Dto.Share.Requests;
using V1.DyModels.Dto.Share.Responses;

namespace V1.Repositories.Share
{
    /// <summary>
    /// ApplicationUser interface for RepositoriesRepository.
    /// </summary>
    public interface IApplicationUserShareRepository : IBaseShareRepository<ApplicationUserRequestShareDto, ApplicationUserResponseShareDto> //
    , IBaseBPRShareLayer<ApplicationUserRequestShareDto, ApplicationUserResponseShareDto>
    //  يمكنك  التزويد بكل  دوال   طبقة Builder   ببوابات  الطبقة   هذه نفسها      
    //,IApplicationUserBuilderRepository<ApplicationUserRequestShareDto, ApplicationUserResponseShareDto>
    {
        // Define methods or properties specific to the share repository interface.
    }
}