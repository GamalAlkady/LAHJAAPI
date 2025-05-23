using AutoGenerator;

namespace V1.DyModels.VMs
{
    /// <summary>
    /// Dialect  property for VM Info.
    /// </summary>
    public class DialectInfoVM : ITVM
    {
        public String? Id { get; set; }
        //
        public string? Name { get; set; }
        //
        public string? Description { get; set; }
        ///
        public String? LanguageId { get; set; }
        public LanguageOutputVM? Language { get; set; }
    }
}