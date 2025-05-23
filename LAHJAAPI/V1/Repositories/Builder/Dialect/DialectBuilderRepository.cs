using AutoGenerator;
using AutoMapper;
using LAHJAAPI.Data;
using LAHJAAPI.Models;
using V1.DyModels.Dto.Build.Requests;
using V1.DyModels.Dto.Build.Responses;
using V1.Repositories.Base;

namespace V1.Repositories.Builder
{
    /// <summary>
    /// Dialect class property for BuilderRepository.
    /// </summary>
     //
    public class DialectBuilderRepository : BaseBuilderRepository<Dialect, DialectRequestBuildDto, DialectResponseBuildDto>, IDialectBuilderRepository<DialectRequestBuildDto, DialectResponseBuildDto>, ITBuilder
    {
        /// <summary>
        /// Constructor for DialectBuilderRepository.
        /// </summary>
        public DialectBuilderRepository(DataContext dbContext, IMapper mapper, ILoggerFactory logger) : base(dbContext, mapper, logger) // Initialize  constructor.
        {
            // Initialize necessary fields or call base constructor.
            ///
            /// 

            /// 
        }
        //
        // Add additional methods or properties as needed.
    }
}