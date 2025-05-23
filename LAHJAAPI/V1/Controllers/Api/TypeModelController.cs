using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using V1.BPR.Layers.Base;
using V1.DyModels.Dso.Requests;
using V1.DyModels.Dso.Responses;
using V1.DyModels.VMs;
using V1.Services.Services;

namespace V1.Controllers.Api
{
    //[ApiExplorerSettings(GroupName = "V1")]
    [Route("api/V1/user/[controller]")]
    [ApiController]
    public class TypeModelController : BaseBPRControllerLayer<TypeModelRequestDso, TypeModelResponseDso, TypeModelCreateVM, TypeModelOutputVM, TypeModelUpdateVM, TypeModelInfoVM, TypeModelDeleteVM, TypeModelFilterVM>
    {
        private readonly IUseTypeModelService _service;
        public TypeModelController(IMapper mapper, ILoggerFactory logger, IUseTypeModelService bPR) : base(mapper, logger, bPR)
        {
            _service = bPR;
        }
    }
}