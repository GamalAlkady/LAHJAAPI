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
    public class PlanFeatureController : BaseBPRControllerLayer<PlanFeatureRequestDso, PlanFeatureResponseDso, PlanFeatureCreateVM, PlanFeatureOutputVM, PlanFeatureUpdateVM, PlanFeatureInfoVM, PlanFeatureDeleteVM, PlanFeatureFilterVM>
    {
        private readonly IUsePlanFeatureService _service;
        public PlanFeatureController(IMapper mapper, ILoggerFactory logger, IUsePlanFeatureService bPR) : base(mapper, logger, bPR)
        {
            _service = bPR;
        }
    }
}