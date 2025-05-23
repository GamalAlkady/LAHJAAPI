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
    /// AdvertisementTab class for ShareRepository.
    /// </summary>
    public class AdvertisementTabShareRepository : BaseBPRShareLayer<AdvertisementTabRequestShareDto, AdvertisementTabResponseShareDto, AdvertisementTabRequestBuildDto, AdvertisementTabResponseBuildDto>, IAdvertisementTabShareRepository
    {
        // Declare the builder repository.
        private readonly AdvertisementTabBuilderRepository _builder;
        public AdvertisementTabShareRepository(IMapper mapper, ILoggerFactory logger, AdvertisementTabBuilderRepository bpr) : base(mapper, logger, bpr)
        {
            _builder = bpr;
        }
    }
}