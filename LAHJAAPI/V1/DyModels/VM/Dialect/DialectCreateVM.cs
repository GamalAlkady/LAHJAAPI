using AutoGenerator;
using AutoGenerator.Helper.Translation;
using System.ComponentModel.DataAnnotations;

namespace V1.DyModels.VMs
{
    /// <summary>
    /// Dialect  property for VM Create.
    /// </summary>
    public class DialectCreateVM : ITVM
    {
        [Required]
        public TranslationData Name { get; set; }

        public TranslationData? Description { get; set; }

        [Required]
        public String? LanguageId { get; set; }
        //public LanguageCreateVM? Language { get; set; }
    }
}