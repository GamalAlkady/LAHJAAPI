using AutoGenerator;
using AutoGenerator.Helper.Translation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace V1.DyModels.VMs
{
    /// <summary>
    /// Plan  property for VM Create.
    /// </summary>
    public class PlanCreateVM : ITVM
    {
        [DefaultValue("")]
        public String? ProductId { get; set; }
        public TranslationData? ProductName { get; set; }
        public TranslationData? Description { get; set; }
        [DefaultValue(null)]
        public List<String>? Images { get; set; } = new List<string>();

        [Required]
        [DefaultValue("month")]
        public String BillingPeriod { get; set; } = "month";

        [Required]
        public Double Amount { get; set; }

        [Required]
        [DefaultValue("SAR")]
        public string Currency { get; set; } = "SAR";
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