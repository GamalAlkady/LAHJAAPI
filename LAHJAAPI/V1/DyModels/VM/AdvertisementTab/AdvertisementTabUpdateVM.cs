using AutoGenerator;
using AutoGenerator.Helper.Translation;
using System.ComponentModel.DataAnnotations;

namespace V1.DyModels.VMs
{
    /// <summary>
    /// AdvertisementTab  property for VM Update.
    /// </summary>
    public class AdvertisementTabUpdateVM : ITVM
    {
        [Required]
        public TranslationData Title { get; set; }

        public TranslationData? Description { get; set; }
        ///
        public String? ImageAlt { get; set; }
    }
}