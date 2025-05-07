using AutoGenerator;
using System.ComponentModel.DataAnnotations;

namespace LAHJAAPI.Models
{
    [Microsoft.EntityFrameworkCore.Index(nameof(Key), IsUnique = false)] // Moved the Index attribute to the class level
    public class PlanFeature : ITModel
    {
        [Key]
        public required string Key { get; set; }
        public string? Value { get; set; }

        [Required]
        [ToTranslation]
        public string? Name { get; set; }
        [ToTranslation]
        [Required] public string? Description { get; set; }
        public string PlanId { get; set; }

        public Plan? Plan { get; set; }
    }
}
