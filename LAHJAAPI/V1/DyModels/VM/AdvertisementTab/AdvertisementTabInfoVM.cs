using AutoGenerator;

namespace V1.DyModels.VMs
{
    /// <summary>
    /// AdvertisementTab  property for VM Info.
    /// </summary>
    public class AdvertisementTabInfoVM : ITVM
    {
        public String? Id { get; set; }
        ///
        public String? AdvertisementId { get; set; }
        //
        public string? Title { get; set; }
        //
        public string? Description { get; set; }
        ///
        public String? ImageAlt { get; set; }
    }
}