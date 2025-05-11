using AutoGenerator;

namespace LAHJAAPI.Models
{
    public class AuthorizationSessionService : ITModel
    {
        public string AuthorizationSessionId { get; set; }
        public AuthorizationSession AuthorizationSession { get; set; }

        public string ServiceId { get; set; }
        public Service Service { get; set; }
    }
}
