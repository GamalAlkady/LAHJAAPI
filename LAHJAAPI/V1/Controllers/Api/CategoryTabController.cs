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
    public class CategoryTabController : BaseBPRControllerLayer<CategoryTabRequestDso, CategoryTabResponseDso, CategoryTabCreateVM, CategoryTabOutputVM, CategoryTabUpdateVM, CategoryTabInfoVM, CategoryTabDeleteVM, CategoryTabFilterVM>
    {
        private readonly IUseCategoryTabService _service;
        public CategoryTabController(IMapper mapper, ILoggerFactory logger, IUseCategoryTabService bPR) : base(mapper, logger, bPR)
        {
            _service = bPR;
        }
    }
}