using AutoGenerator;

namespace V1.DyModels.VMs
{
    /// <summary>
    /// FAQItem  property for VM Info.
    /// </summary>
    public class FAQItemInfoVM : ITVM
    {
        public String? Id { get; set; }
        //
        public string? Question { get; set; }
        //
        public string? Answer { get; set; }
        //
        public string? Tag { get; set; }
    }
}