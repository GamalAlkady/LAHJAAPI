using AutoGenerator;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LAHJAAPI.Models
{
    [Index(nameof(Key), IsUnique = true)] // Moved the Index attribute to the class level
    public class PlanFeature : ITModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string? Key { get; set; }
        public string? Value { get; set; }

        [Required]
        [ToTranslation]
        public string? Name { get; set; }
        [ToTranslation]
        [Required] public string? Description { get; set; }
        public string? PlanId { get; set; }

        public Plan? Plan { get; set; }
    }
}
