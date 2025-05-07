using AutoGenerator;

namespace V1.DyModels.Dto.Build.Requests
{
    public class SettingRequestBuildDto : ITBuildDto
    {
        /// <summary>
        /// Name property for DTO.
        /// </summary>
        public required String Name { get; set; }
        /// <summary>
        /// Value property for DTO.
        /// </summary>
        public String? Value { get; set; }
    }
}