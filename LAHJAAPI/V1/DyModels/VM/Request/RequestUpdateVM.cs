using AutoGenerator;

namespace V1.DyModels.VMs
{
    /// <summary>
    /// Request  property for VM Update.
    /// </summary>
    public class RequestUpdateVM : ITVM
    {
        public String? Question { get; set; }

        public String? ServiceId { get; set; }
        ///
        public String? SpaceId { get; set; }
    }
}