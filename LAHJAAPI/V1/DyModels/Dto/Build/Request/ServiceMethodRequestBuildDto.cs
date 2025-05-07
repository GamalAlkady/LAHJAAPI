using AutoGenerator;

namespace V1.DyModels.Dto.Build.Requests
{
    public class ServiceMethodRequestBuildDto : ITBuildDto
    {
        /// <summary>
        /// Id property for DTO.
        /// </summary>
        public required string Id { get; set; } = $"servm_{Guid.NewGuid():N}";
        /// <summary>
        /// Method property for DTO.
        /// </summary>
        public String? Method { get; set; }
        /// <summary>
        /// InputParameters property for DTO.
        /// </summary>
        public String? InputParameters { get; set; }
        /// <summary>
        /// OutputParameters property for DTO.
        /// </summary>
        public String? OutputParameters { get; set; }
        /// <summary>
        /// ServiceId property for DTO.
        /// </summary>
        public String? ServiceId { get; set; }
        public ServiceRequestBuildDto? Service { get; set; }
    }
}