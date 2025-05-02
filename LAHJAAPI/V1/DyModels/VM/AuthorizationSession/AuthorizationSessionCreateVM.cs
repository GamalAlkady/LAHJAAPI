using AutoGenerator;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace V1.DyModels.VMs
{
    /// <summary>
    /// AuthorizationSession  property for VM Create.
    /// </summary>
    public class AuthorizationSessionCreateVM : ITVM
    {
        [Required]
        public string Token { get; set; }

        [Required]
        public string ServiceId { get; set; }
    }

    public class CreateAuthorizationForDashboard
    {
        [Required]
        public string Token { get; set; }
    }

    public class CreateAuthorizationForListServices
    {
        [Required]
        public string Token { get; set; }

        [Required(ErrorMessage = "The ServicesIds field is required.")]
        public List<string> ServicesIds { get; set; }

    }

    public class CreateAuthorizationForServices
    {
        [Required]
        public string? Token { get; set; }

        [DefaultValue("")]
        public string? ModelAiId { get; set; }

    }
}