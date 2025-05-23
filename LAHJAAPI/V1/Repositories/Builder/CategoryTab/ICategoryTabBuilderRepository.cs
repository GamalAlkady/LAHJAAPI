using V1.BPR.Layers.Base;

namespace V1.Repositories.Builder
{
    /// <summary>
    /// CategoryTab interface property for BuilderRepository.
    /// </summary>
    public interface ICategoryTabBuilderRepository<TBuildRequestDto, TBuildResponseDto> : IBPRLayer<TBuildRequestDto, TBuildResponseDto> //
 where TBuildRequestDto : class //
 where TBuildResponseDto : class //
    {
        // Define methods or properties specific to the builder interface.
    }
}