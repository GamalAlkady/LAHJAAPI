using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LAHJAAPI.V1.DyModels.VM.Stripe.Checkout
{
    public class CheckoutOptions
    {
        [DefaultValue("price_1Qn3yrKMQ7LabgRT4wwrFO8N")]
        [Required]
        public string PlanId { get; set; }

        [DefaultValue("https://lahja.runasp.net/")]
        public string SuccessUrl { get; set; }

        [DefaultValue("https://lahja.runasp.net/")]
        public string CancelUrl { get; set; }

    }

    public class CheckoutWebOptions
    {
        [Required]
        public string PlanId { get; set; }

        [Required]
        public string ReturnUrl { get; set; }

    }
}
