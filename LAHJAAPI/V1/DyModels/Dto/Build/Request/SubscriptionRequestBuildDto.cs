using AutoGenerator;

namespace V1.DyModels.Dto.Build.Requests
{
    public class SubscriptionRequestBuildDto : ITBuildDto
    {
        /// <summary>
        /// Id property for DTO.
        /// </summary>
        public String? Id { get; set; }
        /// <summary>
        /// CustomerId property for DTO.
        /// </summary>
        public String? CustomerId { get; set; }
        /// <summary>
        /// StartDate property for DTO.
        /// </summary>
        public DateTime StartDate { get; set; }
        /// <summary>
        /// CurrentPeriodStart property for DTO.
        /// </summary>
        public DateTime CurrentPeriodStart { get; set; }
        /// <summary>
        /// CurrentPeriodEnd property for DTO.
        /// </summary>
        public DateTime CurrentPeriodEnd { get; set; }

        public bool IsFree { get; set; } = false;
        public required string Status { get; set; }
        public string? BillingPeriod { get; set; }
        /// <summary>
        /// CancelAtPeriodEnd property for DTO.
        /// </summary>
        public Boolean CancelAtPeriodEnd { get; set; }
        /// <summary>
        /// NumberRequests property for DTO.
        /// </summary>
        public Int32 NumberRequests { get; set; }
        /// <summary>
        /// AllowedRequests property for DTO.
        /// </summary>
        public Int32 AllowedRequests { get; set; }
        /// <summary>
        /// AllowedSpaces property for DTO.
        /// </summary>
        public Int32 AllowedSpaces { get; set; }
        /// <summary>
        /// CancelAt property for DTO.
        /// </summary>
        public Nullable<DateTime> CancelAt { get; set; }
        /// <summary>
        /// CanceledAt property for DTO.
        /// </summary>
        public Nullable<DateTime> CanceledAt { get; set; }
        /// <summary>
        /// PlanId property for DTO.
        /// </summary>
        public required String PlanId { get; set; }
        //public PlanRequestBuildDto? Plan { get; set; }
        /// <summary>
        /// UserId property for DTO.
        /// </summary>
        //public String? UserId { get; set; }
        public ApplicationUserRequestBuildDto? User { get; set; }
        public ICollection<RequestRequestBuildDto>? Requests { get; set; }
        public ICollection<SpaceRequestBuildDto>? Spaces { get; set; }
    }
}