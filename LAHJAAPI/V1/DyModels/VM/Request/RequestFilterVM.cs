using AutoGenerator;

namespace V1.DyModels.VMs
{
    /// <summary>
    /// Request  property for VM Filter.
    /// </summary>
    public class RequestFilterVM : ITVM
    {
        public string? ModelGateway { get; set; }
        public string? ModelAi { get; set; }
        public string? Service { get; set; }
        public string? Token { get; set; }
        public string? EventId { get; set; }
        public string? AllowedRequests { get; set; }
        public int NumberRequests { get; internal set; }
    }
}