using AutoGenerator;
using V1.DyModels.Dso.Responses;

namespace V1.DyModels.VMs
{
    /// <summary>
    /// Request  property for VM Filter.
    /// </summary>
    public class RequestFilterVM : ITVM
    {
        ///
        public string? Id { get; set; }

        public SubscriptionResponseDso? Subscription { get; set; }

        public string? ServiceId { get; set; }

        public AuthorizationSessionFilterVM? AuthorizationSession { get; set; }

        public SpaceFilterVM? Space { get; set; }
    }
}