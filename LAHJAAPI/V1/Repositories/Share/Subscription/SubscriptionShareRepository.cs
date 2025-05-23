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
    /// Subscription class for ShareRepository.
    /// </summary>
    public class SubscriptionShareRepository : BaseBPRShareLayer<SubscriptionRequestShareDto, SubscriptionResponseShareDto, SubscriptionRequestBuildDto, SubscriptionResponseBuildDto>, ISubscriptionShareRepository
    {
        // Declare the builder repository.
        private readonly SubscriptionBuilderRepository _builder;
        public SubscriptionShareRepository(IMapper mapper, ILoggerFactory logger, SubscriptionBuilderRepository bpr) : base(mapper, logger, bpr)
        {
            _builder = bpr;
        }
    }
}