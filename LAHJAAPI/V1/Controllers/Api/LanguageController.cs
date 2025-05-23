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
    public class LanguageController : BaseBPRControllerLayer<LanguageRequestDso, LanguageResponseDso, LanguageCreateVM, LanguageOutputVM, LanguageUpdateVM, LanguageInfoVM, LanguageDeleteVM, LanguageFilterVM>
    {
        private readonly IUseLanguageService _service;
        public LanguageController(IMapper mapper, ILoggerFactory logger, IUseLanguageService bPR) : base(mapper, logger, bPR)
        {
            _service = bPR;
        }
    }
}