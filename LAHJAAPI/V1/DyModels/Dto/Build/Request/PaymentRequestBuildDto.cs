using AutoGenerator;

namespace V1.DyModels.Dto.Build.Requests
{
    public class PaymentRequestBuildDto : ITBuildDto
    {
        /// <summary>
        /// Id property for DTO.
        /// </summary>
        public String? Id { get; set; } = $"pay_{Guid.NewGuid():N}";
        /// <summary>
        /// CustomerId property for DTO.
        /// </summary>
        public String? CustomerId { get; set; }
        /// <summary>
        /// InvoiceId property for DTO.
        /// </summary>
        public String? InvoiceId { get; set; }
        /// <summary>
        /// Status property for DTO.
        /// </summary>
        public String? Status { get; set; }
        /// <summary>
        /// Amount property for DTO.
        /// </summary>
        public String? Amount { get; set; }
        /// <summary>
        /// Currency property for DTO.
        /// </summary>
        public String? Currency { get; set; }
        /// <summary>
        /// Date property for DTO.
        /// </summary>
        public DateOnly Date { get; set; }
        public InvoiceRequestBuildDto? Invoice { get; set; }
    }
}