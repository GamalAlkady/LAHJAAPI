using AutoGenerator;

namespace V1.DyModels.VMs
{
    /// <summary>
    /// ApplicationUser  property for VM Filter.
    /// </summary>
    public class ApplicationUserFilterVM : ITVM
    {
        ///
        public string? Id { get; set; }
        ///
        public string? Lg { get; set; }
        public string? Email { get; internal set; }
        public string SubscriptionId { get; internal set; }
        public string SubscriptionStatus { get; internal set; }
        public string SubscriptionPlanName { get; internal set; }
        public string? SubscriptionPlanId { get; internal set; }
        public int Days { get; internal set; }
    }
}