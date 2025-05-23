using AutoGenerator;
using AutoGenerator.Repositories.Builder;
using AutoMapper;
using LAHJAAPI.Data;
using V1.BPR.Layers.Base;

namespace V1.Repositories.Base
{
    /// <summary>
    /// BaseRepository class for ShareRepository.
    /// </summary>
    public abstract class BaseBuilderRepository<TModel, TBuildRequestDto, TBuildResponseDto> : BaseBPRLayer<TBuildRequestDto, TBuildResponseDto, TModel, TModel, ITBase, ITModel>, IBPRLayer<TBuildRequestDto, TBuildResponseDto>, ITBuildRepository, IBaseBuilderRepository<TBuildRequestDto, TBuildResponseDto> where TModel : class where TBuildRequestDto : class where TBuildResponseDto : class
    {
        private readonly BaseBPRRepository<TModel> _baseRepository;
        protected BaseBuilderRepository(DataContext dbContext, IMapper mapper, ILoggerFactory logger) :
            this(dbContext, mapper, logger, new BaseBPRRepository<TModel>(dbContext, logger))
        {
        }

        protected BaseBuilderRepository(DataContext dbContext, IMapper mapper, ILoggerFactory logger, BaseBPRRepository<TModel> bpr) : base(mapper, logger, bpr)
        {
            _baseRepository = bpr;
        }

        public async Task<bool> ExecuteTransactionAsync(Func<Task<bool>> operation)
        {
            return await _baseRepository.ExecuteTransactionAsync(operation);
        }

        public async Task<IEnumerable<TBuildResponseDto>> GetAllAsync(string propertyName, object value, string[]? includes = null)
        {
            return Map<TModel, TBuildResponseDto>(await _baseRepository.GetAllAsync(propertyName, value, includes));
        }
    }
}