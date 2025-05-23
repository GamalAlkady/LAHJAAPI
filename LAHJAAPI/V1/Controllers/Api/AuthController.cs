using LAHJAAPI.Models;
using LAHJAAPI.Services2;
using LAHJAAPI.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace LAHJAAPI.V1.Controllers.Api
{
    [AllowAnonymous]
    [Route("api")]
    [ApiController]
    public class AuthController(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        IOptions<AppSettings> options,
        TokenService tokenService
        ) : Controller
    {

        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("ExternalLogin")]
        //[ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Auth", new { returnUrl });

            //redirectUrl = "/api/auth/ExternalLoginCallback";
            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [AllowAnonymous]
        [HttpGet("ExternalLoginCallback")]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null)
        {
            var info = await signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                // إعادة التوجيه إلى صفحة تسجيل الدخول مع رسالة خطأ
                return Redirect(returnUrl + "/?status=error&&message=null information");
            }
            // تسجيل الدخول أو إنشاء حساب جديد
            var user = await userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
            if (user == null)
            {
                user = new ApplicationUser { UserName = info.Principal.FindFirstValue(ClaimTypes.Email), Email = info.Principal.FindFirstValue(ClaimTypes.Email) };
                var result = await userManager.CreateAsync(user);
                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors);
                }
                await userManager.AddLoginAsync(user, info);
            }
            // تسجيل الدخول للمستخدم باستخدام المصادقة
            await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
            var token = tokenService.GenerateToken([
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())]);
            return Redirect($"{returnUrl}?token={token}" ?? "/");
        }
    }
}