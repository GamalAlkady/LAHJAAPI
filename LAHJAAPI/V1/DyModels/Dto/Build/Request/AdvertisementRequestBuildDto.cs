using AutoGenerator;
using AutoGenerator.Helper.Translation;
using LAHJAAPI.V1.DyModels.Dto;

namespace V1.DyModels.Dto.Build.Requests
{
    public class AdvertisementRequestBuildDto : ITBuildDto, IHaveId<string>
    {
        /// <summary>
        /// Id property for DTO.
        /// </summary>
        public String Id { get; set; } = $"adv_{Guid.NewGuid():N}";
        public TranslationData? Title { get; set; } = new();
        public TranslationData? Description { get; set; } = new();
        /// <summary>
        /// Image property for DTO.
        /// </summary>
        public String? Image { get; set; }
        /// <summary>
        /// Active property for DTO.
        /// </summary>
        public Boolean Active { get; set; }
        /// <summary>
        /// Url property for DTO.
        /// </summary>
        public String? Url { get; set; }
        //  public ICollection<AdvertisementTabRequestBuildDto>? AdvertisementTabs { get; set; }
    }
}