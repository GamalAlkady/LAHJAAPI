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
    public class InvoiceController : BaseBPRControllerLayer<InvoiceRequestDso, InvoiceResponseDso, InvoiceCreateVM, InvoiceOutputVM, InvoiceUpdateVM, InvoiceInfoVM, InvoiceDeleteVM, InvoiceFilterVM>
    {
        private readonly IUseInvoiceService _service;
        public InvoiceController(IMapper mapper, ILoggerFactory logger, IUseInvoiceService bPR) : base(mapper, logger, bPR)
        {
            _service = bPR;
        }
    }
}