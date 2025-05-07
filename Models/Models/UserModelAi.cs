using AutoGenerator;

namespace LAHJAAPI.Models
{
    public class UserModelAi : ITModel
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public required string UserId { get; set; }
        public ApplicationUser? User { get; set; }

        public required string ModelAiId { get; set; }
        public ModelAi? ModelAi { get; set; }

    }
}
