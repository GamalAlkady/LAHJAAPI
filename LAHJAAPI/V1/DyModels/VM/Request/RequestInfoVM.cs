using AutoGenerator;

namespace V1.DyModels.VMs
{
    /// <summary>
    /// Request  property for VM Info.
    /// </summary>
    public class RequestInfoVM : ITVM
    {

        public String? Id { get; set; }
        ///
        public String? Status { get; set; }
        ///
        public String? Question { get; set; }
        ///
        public String? Answer { get; set; }
        ///
        public String? ModelGateway { get; set; }
        ///
        public String? ModelAi { get; set; }
        ///
        public DateTime CreatedAt { get; set; }
        ///
        public DateTime UpdatedAt { get; set; }
        ///
        public String? UserId { get; set; }
        public ApplicationUserOutputVM? User { get; set; }
        ///
        public String? SubscriptionId { get; set; }
        public SubscriptionOutputVM? Subscription { get; set; }
        ///
        public String? ServiceId { get; set; }
        public ServiceOutputVM? Service { get; set; }
        ///
        public String? SpaceId { get; set; }
        public SpaceOutputVM? Space { get; set; }
        //
        public List<EventRequestOutputVM>? Events { get; set; }

    }
}