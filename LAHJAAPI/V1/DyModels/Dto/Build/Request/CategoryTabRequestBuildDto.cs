using AutoGenerator;
using AutoGenerator.Helper.Translation;

namespace V1.DyModels.Dto.Build.Requests
{
    public class CategoryTabRequestBuildDto : ITBuildDto
    {
        /// <summary>
        /// Id property for DTO.
        /// </summary>
        public String? Id { get; set; } = $"catm_tab{Guid.NewGuid():N}";
        public required TranslationData Name { get; set; } = new();
        public TranslationData? Description { get; set; } = new();
        /// <summary>
        /// Active property for DTO.
        /// </summary>
        public Boolean Active { get; set; }
        /// <summary>
        /// Image property for DTO.
        /// </summary>
        public String? Image { get; set; }
        /// <summary>
        /// UrlUsed property for DTO.
        /// </summary>
        public String? UrlUsed { get; set; }
        /// <summary>
        /// CountFalvet property for DTO.
        /// </summary>
        public Int32 CountFalvet { get; set; }
        /// <summary>
        /// Rateing property for DTO.
        /// </summary>
        public Int32 Rateing { get; set; }
    }
}