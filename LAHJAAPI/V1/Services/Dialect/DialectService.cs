using AutoGenerator;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using AutoGenerator.Services.Base;
using V1.DyModels.Dso.Requests;
using V1.DyModels.Dso.Responses;
using LAHJAAPI.Models;
using V1.DyModels.Dto.Share.Requests;
using V1.DyModels.Dto.Share.Responses;
using V1.Repositories.Share;
using System.Linq.Expressions;
using V1.Repositories.Builder;
using AutoGenerator.Repositories.Base;
using AutoGenerator.Helper;
using System;
using V1.BPR.Layers.Base;

namespace V1.Services.Services
{
    public class DialectService : BaseBPRServiceLayer<DialectRequestDso, DialectResponseDso, DialectRequestShareDto, DialectResponseShareDto>, IUseDialectService
    {
        private readonly IDialectShareRepository _share;
        public DialectService(IMapper mapper, ILoggerFactory logger, IDialectShareRepository bpr) : base(mapper, logger, bpr)
        {
            _share = bpr;
        }
    }
}