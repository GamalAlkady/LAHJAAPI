using AutoGenerator;
using System.ComponentModel.DataAnnotations;

namespace LAHJAAPI.Models
{
    public class ModelGateway : ITModel
    {
        [Key]
        public string Id { get; set; } = $"modg_{Guid.NewGuid():N}";
        public required string Name { get; set; }
        public required string Url { get; set; }
        public required string Token { get; set; }
        public bool IsDefault { get; set; }

        public ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
        public ICollection<ModelAi>? ModelAis { get; set; }
    }
}
