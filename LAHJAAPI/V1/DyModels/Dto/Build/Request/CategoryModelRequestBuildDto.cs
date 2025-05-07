using AutoGenerator;
using AutoGenerator.Helper.Translation;

namespace V1.DyModels.Dto.Build.Requests
{
    public class CategoryModelRequestBuildDto : ITBuildDto
    {
        /// <summary>
        /// Id property for DTO.
        /// </summary>
        public String Id { get; set; } = $"catm_{Guid.NewGuid():N}";


        public required TranslationData Name { get; set; } = new();
        public TranslationData? Description { get; set; } = new();
    }
}