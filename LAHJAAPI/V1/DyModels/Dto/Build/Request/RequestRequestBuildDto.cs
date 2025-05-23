using AutoGenerator;

namespace V1.DyModels.Dto.Build.Requests
{
    public class RequestRequestBuildDto : ITBuildDto
    {
        /// <summary>
        /// Id property for DTO.
        /// </summary>
        public string Id { get; set; } = $"req_{Guid.NewGuid():N}";
        /// <summary>
        /// Status property for DTO.
        /// </summary>
        public String? Status { get; set; }
        /// <summary>
        /// Question property for DTO.
        /// </summary>
        public String? Question { get; set; }
        /// <summary>
        /// Answer property for DTO.
        /// </summary>
        public String? Answer { get; set; }
        /// <summary>
        /// ModelGateway property for DTO.
        /// </summary>
        public String? ModelGateway { get; set; }
        /// <summary>
        /// ModelAi property for DTO.
        /// </summary>
        public String? ModelAi { get; set; }
        /// <summary>
        /// CreatedAt property for DTO.
        /// </summary>

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        /// <summary>
        /// UserId property for DTO.
        /// </summary>
        public String? UserId { get; set; }
        public ApplicationUserRequestBuildDto? User { get; set; }
        /// <summary>
        /// SubscriptionId property for DTO.
        /// </summary>
        public String? SubscriptionId { get; set; }
        public SubscriptionRequestBuildDto? Subscription { get; set; }
        /// <summary>
        /// ServiceId property for DTO.
        /// </summary>
        public String? ServiceId { get; set; }
        public ServiceRequestBuildDto? Service { get; set; }
        /// <summary>
        /// SpaceId property for DTO.
        /// </summary>
        public String? SpaceId { get; set; }
        public SpaceRequestBuildDto? Space { get; set; }
        public ICollection<EventRequestRequestBuildDto>? Events { get; set; } = [];
    }
}