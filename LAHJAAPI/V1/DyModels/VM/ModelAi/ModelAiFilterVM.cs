using AutoGenerator;

namespace V1.DyModels.VMs
{
    /// <summary>
    /// ModelAi  property for VM Filter.
    /// </summary>
    public class ModelAiFilterVM : ITVM
    {
        public string? Name { get; set; }

        public string? Category { get; set; }
        public string? Language { get; set; }
        public bool? IsStandard { get; set; }
        public string? Gender { get; set; }
        public string? Dialect { get; set; }
        public string? Type { get; set; }
    }
}