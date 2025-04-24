using System.ComponentModel.DataAnnotations;

namespace LAHJAAPI.V1.DyModels.VM.Stripe.Invoice
{
    public class InvoiceCreate
    {
        [Required]
        public string CustomerId { get; set; }
        [Required]
        public string SubscriptionId { get; set; }
        public bool AutoAdvance { get; set; }

    }
}
