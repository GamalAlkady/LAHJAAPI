using AutoGenerator;
using AutoGenerator.Helper.Translation;

namespace V1.DyModels.Dto.Build.Requests
{
    public class PlanRequestBuildDto : ITBuildDto
    {
        /// <summary>
        /// Id property for DTO.
        /// </summary>
        public String? Id { get; set; }
        /// <summary>
        /// ProductId property for DTO.
        /// </summary>
        public String? ProductId { get; set; }
        public TranslationData? ProductName { get; set; } = new();
        public TranslationData? Description { get; set; } = new();
        public List<String> Images { get; set; } = [];
        /// <summary>
        /// BillingPeriod property for DTO.
        /// </summary>
        public String? BillingPeriod { get; set; }
        /// <summary>
        /// Amount property for DTO.
        /// </summary>
        public string? Currency { get; internal set; }
        public Double Amount { get; set; }
        /// <summary>
        /// Active property for DTO.
        /// </summary>
        public Boolean Active { get; set; }

        public ICollection<SubscriptionRequestBuildDto>? Subscriptions { get; set; }
        public ICollection<PlanFeatureRequestBuildDto>? PlanFeatures { get; set; }
    }
}