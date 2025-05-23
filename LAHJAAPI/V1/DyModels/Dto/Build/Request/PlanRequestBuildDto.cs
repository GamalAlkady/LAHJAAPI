using AutoGenerator;
using AutoGenerator.Helper.Translation;

namespace V1.DyModels.Dto.Build.Requests
{
    public class PlanRequestBuildDto : ITBuildDto
    {
        /// <summary>
        /// Id property for DTO.
        /// </summary>
        public string Id { get; set; } = $"plan_{Guid.NewGuid():N}";
        /// <summary>
        /// ProductId property for DTO.
        /// </summary>
        public required String ProductId { get; set; }
        public TranslationData? ProductName { get; set; } = new();
        public TranslationData? Description { get; set; } = new();
        public List<String>? Images { get; set; }
        /// <summary>
        /// BillingPeriod property for DTO.
        /// </summary>
        public String? BillingPeriod { get; set; }
        /// <summary>
        /// Amount property for DTO.
        /// </summary>
        public Double Amount { get; set; }

        public string? Currency { get; set; } = "SAR";       // daily , weekly , monthly ,.. 

        /// <summary>
        /// Active property for DTO.
        /// </summary>
        public Boolean Active { get; set; } = true;
        /// <summary>
        /// UpdatedAt property for DTO.
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        /// <summary>
        /// CreatedAt property for DTO.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<SubscriptionRequestBuildDto>? Subscriptions { get; set; }
        public ICollection<PlanFeatureRequestBuildDto>? PlanFeatures { get; set; }
    }
}