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
    /// TypeModel class property for BuilderRepository.
    /// </summary>
     //
    public class TypeModelBuilderRepository : BaseBuilderRepository<TypeModel, TypeModelRequestBuildDto, TypeModelResponseBuildDto>, ITypeModelBuilderRepository<TypeModelRequestBuildDto, TypeModelResponseBuildDto>, ITBuilder
    {
        /// <summary>
        /// Constructor for TypeModelBuilderRepository.
        /// </summary>
        public TypeModelBuilderRepository(DataContext dbContext, IMapper mapper, ILoggerFactory logger) : base(dbContext, mapper, logger) // Initialize  constructor.
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