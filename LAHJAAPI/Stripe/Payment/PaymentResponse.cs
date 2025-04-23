namespace LAHJAAPI.Stripe.Payment
{
    public class ResponseClientSecret
    {
        public string? ClientSecret { get; set; }
    }

    public class CustomerSessionResponse
    {
        public required string CustomerSessionClientSecret { get; set; }
    }


    public class SessionResponse
    {
        public required string Status { get; set; }
        public required string CustomerEmail { get; set; }
    }
}
