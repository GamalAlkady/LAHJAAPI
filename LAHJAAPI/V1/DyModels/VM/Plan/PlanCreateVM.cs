using AutoGenerator;
using AutoGenerator.Helper.Translation;
using System.ComponentModel.DataAnnotations;

namespace V1.DyModels.VMs
{
    /// <summary>
    /// Plan  property for VM Create.
    /// </summary>
    public class PlanCreateVM : ITVM
    {
        public String? ProductId { get; set; }
        public TranslationData? ProductName { get; set; }
        public TranslationData? Description { get; set; }
        public List<String>? Images { get; set; }

        [Required]
        public String BillingPeriod { get; set; } = "month";

        [Required]
        public Double Amount { get; set; }

        [Required]
        public string Currency { get; set; } = "USD";
        public Boolean Active { get; set; } = true;
    }

    public class PlanSetVM : ITVM
    {
        [Required]
        public String? ProductId { get; set; }

        [Required]
        public String Id { get; set; }
    }
}