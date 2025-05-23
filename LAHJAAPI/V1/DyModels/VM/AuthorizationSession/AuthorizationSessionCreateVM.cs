using AutoGenerator;
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


        [Required(ErrorMessage = "The ServicesIds field is required.")]
        [MinLength(1, ErrorMessage = "The ServicesIds field must have at least 1 item.")]
        public List<string> ServicesIds { get; set; }

        public string? SpaceId { get; set; }
    }

    public class GeneralAuthSessionCreateVM
    {
        [Required]
        public string Token { get; set; }

        [Required(ErrorMessage = "The ServicesIds field is required.")]
        public List<string> ServicesIds { get; set; }
    }

    public class CreateAuthorizationForListServices
    {
        [Required]
        public string Token { get; set; }

        [Required(ErrorMessage = "The ServicesIds field is required.")]
        public List<string> ServicesIds { get; set; }
        public string? SpaceId { get; set; }

    }

    public class CreateAuthorizationForServices
    {
        [Required]
        public string Token { get; set; }

        [Required]
        public string SpaceId { get; set; }

        public List<string>? Except { get; set; }
    }

    public class DataAuthSession
    {
        public required string Token { get; set; }

        public string? SpaceId { get; set; }
        public List<string> ServicesIds { get; internal set; } = [];

        public List<string>? Except { get; set; }

    }
}