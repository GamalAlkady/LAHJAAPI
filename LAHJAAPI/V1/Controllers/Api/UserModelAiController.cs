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
    public class UserModelAiController : BaseBPRControllerLayer<UserModelAiRequestDso, UserModelAiResponseDso, UserModelAiCreateVM, UserModelAiOutputVM, UserModelAiUpdateVM, UserModelAiInfoVM, UserModelAiDeleteVM, UserModelAiFilterVM>
    {
        private readonly IUseUserModelAiService _service;
        public UserModelAiController(IMapper mapper, ILoggerFactory logger, IUseUserModelAiService bPR) : base(mapper, logger, bPR)
        {
            _service = bPR;
        }
    }
}