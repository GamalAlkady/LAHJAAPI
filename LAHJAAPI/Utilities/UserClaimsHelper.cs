using LAHJAAPI.Utilities;
using System.Security.Claims;
using System.Text.Json;

namespace APILAHJA.Utilities;

public interface IUserClaimsHelper
{
    ClaimsPrincipal? User { get; }
    string UserId { get; }
    string? SubscriptionId { get; }
    string? CustomerId { get; }
    string? UserRole { get; }
    string? Email { get; }
    //long NumberRequests { get; }
    List<string>? ServicesIds { get; }
    string? SessionId { get; }
    HttpContext? HttpContext { get; }
}

public class UserClaimsHelper(IHttpContextAccessor httpContext) : IUserClaimsHelper
{
    public HttpContext? HttpContext => httpContext?.HttpContext;
    public ClaimsPrincipal? User => HttpContext?.User;
    public string UserId => (HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier))!;
    public string? UserRole => HttpContext?.User?.FindFirstValue(ClaimTypes.Role);
    public string? Email => HttpContext?.User?.FindFirstValue(ClaimTypes.Email);
    //public string? Email => HttpContext?.User?.FindFirstValue(ClaimTypes.Email);
    public List<string>? ServicesIds
    {
        get
        {
            var value = HttpContext?.User?.FindFirstValue(ClaimTypes2.ServicesIds);
            if (value == null) return null;
            return JsonSerializer.Deserialize<List<string>>(value);
        }
    }
    public string? SessionId => HttpContext?.User?.FindFirstValue(ClaimTypes2.SessionId);
    public string? SubscriptionId => HttpContext?.User?.FindFirstValue(ClaimTypes2.SubscriptionId);

    public string? CustomerId => HttpContext?.User?.FindFirstValue(ClaimTypes2.CustomerId);
}

