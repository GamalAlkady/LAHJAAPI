using LAHJAAPI.Models;
using LAHJAAPI.Utilities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LAHJAAPI;

public class MyClaimsTransformation(
    ClaimsChange claimsChange,
    UserManager<ApplicationUser> userManager) : IClaimsTransformation
{
    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        ClaimsIdentity claimsIdentity = new ClaimsIdentity();
        if (principal.Identity != null && principal.Identity.IsAuthenticated)
        {
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!principal.HasClaim(c => c.Type == ClaimTypes2.SubscriptionId))
            {
                var user = await userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user != null && user.SubscriptionId != null)
                {
                    claimsIdentity.AddClaim(new Claim(ClaimTypes2.SubscriptionId, user.SubscriptionId.ToString()));
                }
            }

            if (!principal.HasClaim(c => c.Type == ClaimTypes2.CustomerId))
            {
                var user = await userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    claimsIdentity.AddClaim(new Claim(ClaimTypes2.CustomerId, user.CustomerId ?? ""));
                }
            }
        }

        principal.AddIdentity(claimsIdentity);
        return principal;
    }
}
