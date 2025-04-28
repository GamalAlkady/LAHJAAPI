using AutoGenerator;
using AutoGenerator.Helper.Translation;
using System.ComponentModel.DataAnnotations;

namespace V1.DyModels.VMs
{
    /// <summary>
    /// Dialect  property for VM Update.
    /// </summary>
    public class DialectUpdateVM : ITVM
    {

        [Required]
        public TranslationData? Name { get; set; }
        //
        public TranslationData? Description { get; set; }
    }
}