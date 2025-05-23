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
    public class AdvertisementController : BaseBPRControllerLayer<AdvertisementRequestDso, AdvertisementResponseDso, AdvertisementCreateVM, AdvertisementOutputVM, AdvertisementUpdateVM, AdvertisementInfoVM, AdvertisementDeleteVM, AdvertisementFilterVM>
    {
        private readonly IUseAdvertisementService _service;
        public AdvertisementController(IMapper mapper, ILoggerFactory logger, IUseAdvertisementService bPR) : base(mapper, logger, bPR)
        {
            _service = bPR;
        }


        public override Task<ActionResult<AdvertisementInfoVM>> GetByIdAsync(string id)
        {
            return base.GetByIdAsync(id);
        }
    }
}