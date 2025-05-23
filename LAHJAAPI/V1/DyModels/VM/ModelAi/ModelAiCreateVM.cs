using AutoGenerator;
using AutoGenerator.Helper.Translation;
using System.ComponentModel.DataAnnotations;

namespace V1.DyModels.VMs
{
    /// <summary>
    /// ModelAi  property for VM Create.
    /// </summary>
    public class ModelAiCreateVM : ITVM
    {
        [Required]
        public TranslationData Name { get; set; }
        ///
        //public String? Token { get; set; }
        ///
        [Required]
        public String? AbsolutePath { get; set; }
        //
        public TranslationData? Category { get; set; }
        //
        public string? Language { get; set; }
        //
        public bool IsStandard { get; set; }
        //
        public string? Gender { get; set; }
        //
        public TranslationData? Dialect { get; set; }
        ///
        public String? Type { get; set; }

        [Required]
        public String ModelGatewayId { get; set; }
    }
}