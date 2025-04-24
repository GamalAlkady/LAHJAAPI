using System.ComponentModel.DataAnnotations;

namespace LAHJAAPI.V1.DyModels.VM.Stripe.Customer
{
    public class SessionCreate
    {
        [Required] public string ReturnUrl { get; set; }
    }
}
