using AutoGenerator;

namespace V1.DyModels.Dto.Build.Requests
{
    public class ModelGatewayRequestBuildDto : ITBuildDto
    {
        /// <summary>
        /// Id property for DTO.
        /// </summary>
        public string Id { get; set; } = $"modg_{Guid.NewGuid():N}";
        /// <summary>
        /// Name property for DTO.
        /// </summary>
        public required String Name { get; set; }
        /// <summary>
        /// Url property for DTO.
        /// </summary>
        public required String Url { get; set; }
        /// <summary>
        /// Token property for DTO.
        /// </summary>
        public required String Token { get; set; }
        /// <summary>
        /// IsDefault property for DTO.
        /// </summary>
        public Boolean IsDefault { get; set; }
    }
}