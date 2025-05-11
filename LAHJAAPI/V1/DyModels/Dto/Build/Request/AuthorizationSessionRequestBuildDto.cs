using AutoGenerator;

namespace V1.DyModels.Dto.Build.Requests
{
    public class AuthorizationSessionRequestBuildDto : ITBuildDto
    {
        /// <summary>
        /// Id property for DTO.
        /// </summary>
        public string Id { get; set; } = $"sess_{Guid.NewGuid():N}";
        /// <summary>
        /// SessionToken property for DTO.
        /// </summary>
        public required String SessionToken { get; set; }
        /// <summary>
        /// UserToken property for DTO.
        /// </summary>
        public String? UserToken { get; set; }
        /// <summary>
        /// AuthorizationType property for DTO.
        /// </summary>
        public String? AuthorizationType { get; set; }
        /// <summary>
        /// StartTime property for DTO.
        /// </summary>
        public DateTime StartTime { get; set; } = DateTime.UtcNow;
        /// <summary>
        /// EndTime property for DTO.
        /// </summary>
        public Nullable<DateTime> EndTime { get; set; }
        /// <summary>
        /// IsActive property for DTO.
        /// </summary>
        public Boolean IsActive { get; set; } = true;
        /// <summary>
        /// UserId property for DTO.
        /// </summary>
        public String? UserId { get; set; }
        public ApplicationUserRequestBuildDto? User { get; set; }
        /// <summary>
        /// IpAddress property for DTO.
        /// </summary>
        public String? IpAddress { get; set; }
        /// <summary>
        /// DeviceInfo property for DTO.
        /// </summary>
        public String? DeviceInfo { get; set; }
        /// <summary>
        /// ServicesIds property for DTO.
        /// </summary>
        public String? ServicesIds { get; set; }

        public ICollection<ServiceRequestBuildDto>? Services { get; set; } = [];
    }
}