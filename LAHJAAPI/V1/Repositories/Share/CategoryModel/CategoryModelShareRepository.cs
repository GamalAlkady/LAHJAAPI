using AutoMapper;
using LAHJAAPI.Data;
using LAHJAAPI.Models;
using V1.Repositories.Base;
using AutoGenerator.Repositories.Builder;
using V1.DyModels.Dto.Build.Requests;
using V1.DyModels.Dto.Build.Responses;
using AutoGenerator;
using AutoGenerator.Repositories.Base;
using AutoGenerator;
using V1.Repositories.Builder;
using AutoGenerator.Repositories.Share;
using System.Linq.Expressions;
using AutoGenerator.Repositories.Base;
using AutoGenerator.Helper;
using V1.DyModels.Dto.Share.Requests;
using V1.DyModels.Dto.Share.Responses;
using System;
using V1.BPR.Layers.Base;

namespace V1.Repositories.Share
{
    /// <summary>
    /// CategoryModel class for ShareRepository.
    /// </summary>
    public class CategoryModelShareRepository : BaseBPRShareLayer<CategoryModelRequestShareDto, CategoryModelResponseShareDto, CategoryModelRequestBuildDto, CategoryModelResponseBuildDto>, ICategoryModelShareRepository
    {
        // Declare the builder repository.
        private readonly CategoryModelBuilderRepository _builder;
        public CategoryModelShareRepository(IMapper mapper, ILoggerFactory logger, CategoryModelBuilderRepository bpr) : base(mapper, logger, bpr)
        {
            _builder = bpr;
        }
    }
}