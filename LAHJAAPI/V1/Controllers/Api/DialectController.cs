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
    public class DialectController : BaseBPRControllerLayer<DialectRequestDso, DialectResponseDso, DialectCreateVM, DialectOutputVM, DialectUpdateVM, DialectInfoVM, DialectDeleteVM, DialectFilterVM>
    {
        private readonly IUseDialectService _service;
        public DialectController(IMapper mapper, ILoggerFactory logger, IUseDialectService bPR) : base(mapper, logger, bPR)
        {
            _service = bPR;
        }
    }
}