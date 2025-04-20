using System.ComponentModel.DataAnnotations;

namespace LAHJAAPI.Stripe.Checkout
{
    public class CheckoutOptions
    {
        [Required]
        public string PlanId { get; set; }

        public string SuccessUrl { get; set; }
        public string CancelUrl { get; set; }

    }
}
