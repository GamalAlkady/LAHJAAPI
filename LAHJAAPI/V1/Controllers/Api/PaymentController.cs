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
    public class PaymentController : BaseBPRControllerLayer<PaymentRequestDso, PaymentResponseDso, PaymentCreateVM, PaymentOutputVM, PaymentUpdateVM, PaymentInfoVM, PaymentDeleteVM, PaymentFilterVM>
    {
        private readonly IUsePaymentService _service;
        public PaymentController(IMapper mapper, ILoggerFactory logger, IUsePaymentService bPR) : base(mapper, logger, bPR)
        {
            _service = bPR;
        }
    }
}