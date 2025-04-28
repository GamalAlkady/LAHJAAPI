using AutoGenerator;

namespace V1.DyModels.VMs
{
    /// <summary>
    /// Advertisement  property for VM Output.
    /// </summary>
    public class AdvertisementOutputVM : ITVM
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