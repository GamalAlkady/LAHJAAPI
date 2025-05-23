using AutoGenerator;

namespace V1.DyModels.Dto.Build.Requests
{
    public class SpaceRequestBuildDto : ITBuildDto
    {
        /// <summary>
        /// Id property for DTO.
        /// </summary>
        public string Id { get; set; } = $"space_{Guid.NewGuid():N}";
        /// <summary>
        /// Name property for DTO.
        /// </summary>
        public String? Name { get; set; }
        /// <summary>
        /// Description property for DTO.
        /// </summary>
        public String? Description { get; set; }
        /// <summary>
        /// Ram property for DTO.
        /// </summary>
        public Nullable<Int32> Ram { get; set; }
        /// <summary>
        /// CpuCores property for DTO.
        /// </summary>
        public Nullable<Int32> CpuCores { get; set; }
        /// <summary>
        /// DiskSpace property for DTO.
        /// </summary>
        public Nullable<Single> DiskSpace { get; set; }
        /// <summary>
        /// IsGpu property for DTO.
        /// </summary>
        public Nullable<Boolean> IsGpu { get; set; }
        /// <summary>
        /// IsGlobal property for DTO.
        /// </summary>
        public Nullable<Boolean> IsGlobal { get; set; }
        /// <summary>
        /// Bandwidth property for DTO.
        /// </summary>
        public Nullable<Single> Bandwidth { get; set; }
        /// <summary>
        /// Token property for DTO.
        /// </summary>
        public String? Token { get; set; }
        /// <summary>
        /// SubscriptionId property for DTO.
        /// </summary>
        public String? SubscriptionId { get; set; }
        //public SubscriptionRequestBuildDto? Subscription { get; set; }
        //public ICollection<RequestRequestBuildDto>? Requests { get; set; }
    }
}