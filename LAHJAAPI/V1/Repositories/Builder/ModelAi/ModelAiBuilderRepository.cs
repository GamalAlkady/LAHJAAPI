using AutoMapper;
using LAHJAAPI.Data;
using LAHJAAPI.Models;
using V1.Repositories.Base;
using AutoGenerator.Repositories.Builder;
using V1.DyModels.Dto.Build.Requests;
using V1.DyModels.Dto.Build.Responses;
using AutoGenerator;
using AutoGenerator.Repositories.Base;
using System;
using V1.BPR.Layers.Base;

namespace V1.Repositories.Builder
{
    /// <summary>
    /// ModelAi class property for BuilderRepository.
    /// </summary>
     //
    public class ModelAiBuilderRepository : BaseBuilderRepository<ModelAi, ModelAiRequestBuildDto, ModelAiResponseBuildDto>, IModelAiBuilderRepository<ModelAiRequestBuildDto, ModelAiResponseBuildDto>, ITBuilder
    {
        /// <summary>
        /// Constructor for ModelAiBuilderRepository.
        /// </summary>
        public ModelAiBuilderRepository(DataContext dbContext, IMapper mapper, ILoggerFactory logger) : base(dbContext, mapper, logger) // Initialize  constructor.
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