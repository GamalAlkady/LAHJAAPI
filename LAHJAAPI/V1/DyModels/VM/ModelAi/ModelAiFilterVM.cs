using AutoGenerator;
using System.ComponentModel;

namespace V1.DyModels.VMs
{
    /// <summary>
    /// ModelAi  property for VM Filter.
    /// </summary>
    public class ModelAiFilterVM : ITVM
    {
        [DefaultValue(null)]
        public string? Name { get; set; }

        [DefaultValue(null)]
        public string? Category { get; set; }
        [DefaultValue(null)]
        public string? Language { get; set; }
        public bool? IsStandard { get; set; }
        [DefaultValue(null)]
        public string? Gender { get; set; }
        [DefaultValue(null)]
        public string? Dialect { get; set; }
        [DefaultValue(null)]
        public string? Type { get; set; }
    }
}