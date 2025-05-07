using AutoGenerator;

namespace V1.DyModels.Dto.Build.Requests
{
    public class InvoiceRequestBuildDto : ITBuildDto
    {
        /// <summary>
        /// Id property for DTO.
        /// </summary>
        public String? Id { get; set; } = $"inv_{Guid.NewGuid():N}";
        /// <summary>
        /// CustomerId property for DTO.
        /// </summary>
        public String? CustomerId { get; set; }
        /// <summary>
        /// Status property for DTO.
        /// </summary>
        public String? Status { get; set; }
        /// <summary>
        /// Url property for DTO.
        /// </summary>
        public String? Url { get; set; }
        /// <summary>
        /// InvoiceDate property for DTO.
        /// </summary>
        public Nullable<DateTime> InvoiceDate { get; set; }
    }
}