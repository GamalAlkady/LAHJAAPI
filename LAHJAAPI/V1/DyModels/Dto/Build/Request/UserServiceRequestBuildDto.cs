using AutoGenerator;

namespace V1.DyModels.Dto.Build.Requests
{
    public class UserServiceRequestBuildDto : ITBuildDto
    {
        /// <summary>
        /// CreatedAt property for DTO.
        /// </summary>
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
        /// <summary>
        /// UserId property for DTO.
        /// </summary>
        public required String UserId { get; set; }
        public ApplicationUserRequestBuildDto? User { get; set; }
        /// <summary>
        /// ServiceId property for DTO.
        /// </summary>
        public required String ServiceId { get; set; }
        public ServiceRequestBuildDto? Service { get; set; }
    }
}