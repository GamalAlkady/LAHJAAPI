using AutoGenerator;
using AutoGenerator.Helper.Translation;

namespace V1.DyModels.VMs
{
    /// <summary>
    /// Advertisement  property for VM Update.
    /// </summary>
    public class AdvertisementUpdateVM : ITVM
    {
        public TranslationData? Title { get; set; }
        //
        public TranslationData? Description { get; set; }
        ///
        public String? Image { get; set; }
        ///
        public Boolean Active { get; set; }
        ///
        public String? Url { get; set; }
    }
}