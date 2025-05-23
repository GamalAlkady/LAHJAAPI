using AutoGenerator;

namespace V1.DyModels.VMs
{
    /// <summary>
    /// Advertisement  property for VM Info.
    /// </summary>
    public class AdvertisementInfoVM : ITVM
    {
        ///
        public String? Id { get; set; }
        //
        public string? Title { get; set; }
        //
        public string? Description { get; set; }
        ///
        public String? Image { get; set; }
        ///
        public Boolean Active { get; set; }
        ///
        public String? Url { get; set; }
    }
}