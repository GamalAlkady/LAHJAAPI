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
    /// UserModelAi class property for BuilderRepository.
    /// </summary>
     //
    public class UserModelAiBuilderRepository : BaseBuilderRepository<UserModelAi, UserModelAiRequestBuildDto, UserModelAiResponseBuildDto>, IUserModelAiBuilderRepository<UserModelAiRequestBuildDto, UserModelAiResponseBuildDto>, ITBuilder
    {
        /// <summary>
        /// Constructor for UserModelAiBuilderRepository.
        /// </summary>
        public UserModelAiBuilderRepository(DataContext dbContext, IMapper mapper, ILoggerFactory logger) : base(dbContext, mapper, logger) // Initialize  constructor.
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