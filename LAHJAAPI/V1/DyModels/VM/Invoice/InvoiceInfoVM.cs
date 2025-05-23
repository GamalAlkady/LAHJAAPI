using AutoGenerator;

namespace V1.DyModels.VMs
{
    /// <summary>
    /// Invoice  property for VM Info.
    /// </summary>
    public class InvoiceInfoVM : ITVM
    {
        public String? Id { get; set; }
        ///
        public String? CustomerId { get; set; }
        ///
        public String? Status { get; set; }
        ///
        public String? Url { get; set; }
        ///
        public Nullable<DateTime> InvoiceDate { get; set; }
    }
}