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
    /// Space class for ShareRepository.
    /// </summary>
    public class SpaceShareRepository : BaseBPRShareLayer<SpaceRequestShareDto, SpaceResponseShareDto, SpaceRequestBuildDto, SpaceResponseBuildDto>, ISpaceShareRepository
    {
        // Declare the builder repository.
        private readonly SpaceBuilderRepository _builder;
        public SpaceShareRepository(IMapper mapper, ILoggerFactory logger, SpaceBuilderRepository bpr) : base(mapper, logger, bpr)
        {
            _builder = bpr;
        }
    }
}