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
    /// ServiceMethod class for ShareRepository.
    /// </summary>
    public class ServiceMethodShareRepository : BaseBPRShareLayer<ServiceMethodRequestShareDto, ServiceMethodResponseShareDto, ServiceMethodRequestBuildDto, ServiceMethodResponseBuildDto>, IServiceMethodShareRepository
    {
        // Declare the builder repository.
        private readonly ServiceMethodBuilderRepository _builder;
        public ServiceMethodShareRepository(IMapper mapper, ILoggerFactory logger, ServiceMethodBuilderRepository bpr) : base(mapper, logger, bpr)
        {
            _builder = bpr;
        }
    }
}