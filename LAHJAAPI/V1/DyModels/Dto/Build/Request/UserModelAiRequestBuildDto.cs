using AutoGenerator;

namespace V1.DyModels.Dto.Build.Requests
{
    public class UserModelAiRequestBuildDto : ITBuildDto
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
        /// ModelAiId property for DTO.
        /// </summary>
        public required String ModelAiId { get; set; }
        public ModelAiRequestBuildDto? ModelAi { get; set; }
    }
}