using System.ComponentModel.DataAnnotations;

namespace LAHJAAPI.V1.DyModels.VM.Stripe.Checkout
{
    public class CheckoutOptions
    {
        [Required]
        public string PlanId { get; set; }

        public string SuccessUrl { get; set; }
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
