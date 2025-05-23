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
    public class ServiceMethodController : BaseBPRControllerLayer<ServiceMethodRequestDso, ServiceMethodResponseDso, ServiceMethodCreateVM, ServiceMethodOutputVM, ServiceMethodUpdateVM, ServiceMethodInfoVM, ServiceMethodDeleteVM, ServiceMethodFilterVM>
    {
        private readonly IUseServiceMethodService _service;
        public ServiceMethodController(IMapper mapper, ILoggerFactory logger, IUseServiceMethodService bPR) : base(mapper, logger, bPR)
        {
            _service = bPR;
        }
    }
}