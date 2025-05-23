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
    public class UserServiceController : BaseBPRControllerLayer<UserServiceRequestDso, UserServiceResponseDso, UserServiceCreateVM, UserServiceOutputVM, UserServiceUpdateVM, UserServiceInfoVM, UserServiceDeleteVM, UserServiceFilterVM>
    {
        private readonly IUseUserServiceService _service;
        public UserServiceController(IMapper mapper, ILoggerFactory logger, IUseUserServiceService bPR) : base(mapper, logger, bPR)
        {
            _service = bPR;
        }
    }
}