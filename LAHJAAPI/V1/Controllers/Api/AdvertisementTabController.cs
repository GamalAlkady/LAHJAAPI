using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using V1.BPR.Layers.Base;
using V1.DyModels.Dso.Requests;
using V1.DyModels.Dso.Responses;
using V1.DyModels.VMs;
using V1.Services.Services;

namespace V1.Controllers.Api
{
    ////[ApiExplorerSettings(GroupName = "User")]
    [Route("api/v1/user/[controller]")]
    [ApiController]
    public class AdvertisementTabController : BaseBPRControllerLayer<AdvertisementTabRequestDso, AdvertisementTabResponseDso, AdvertisementTabCreateVM, AdvertisementTabOutputVM, AdvertisementTabUpdateVM, AdvertisementTabInfoVM, AdvertisementTabDeleteVM, AdvertisementTabFilterVM>
    {
        private readonly IUseAdvertisementTabService _service;
        public AdvertisementTabController(IMapper mapper, ILoggerFactory logger, IUseAdvertisementTabService bPR) : base(mapper, logger, bPR)
        {
            _service = bPR;
        }
    }
}