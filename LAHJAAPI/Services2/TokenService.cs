using FluentResults;
using LAHJAAPI.Utilities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace LAHJAAPI.Services2
{

    public class TokenService(IOptions<AppSettings> options)
    {
        private readonly ILogger _logger;
        private AppSettings AppSettings => options.Value;



        public static string GenerateSecureToken(int length = 32)
        {
            byte[] randomBytes = new byte[length];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            return Convert.ToBase64String(randomBytes);
        }

        public string GenerateTemporary(List<Claim>? claims = null, DateTime? expires = null)
        {
            return GenerateToken(claims, AppSettings.Jwt.TempSecret, expires);
        }

        public string GenerateToken(List<Claim>? claims = null, string? secret = null, DateTime? expires = null)
        {
            claims = claims ?? [];
            expires ??= DateTime.UtcNow.AddDays(30);
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Exp, expires.ToString()));
            //claims.Add(new Claim(JwtRegisteredClaimNames.Exp, DateTimeOffset.UtcNow.AddDays(30).ToUnixTimeSeconds().ToString()));
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret ?? AppSettings.Jwt.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: AppSettings.Jwt.validIssuer,
                audience: AppSettings.Jwt.ValidAudience,
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public Result<ClaimsPrincipal> ValidateToken(string token, string? secret = null, string? audience = null)
        {
            if (string.IsNullOrEmpty(token))
            {
                return Result.Fail("Invalid token");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(secret ?? AppSettings.Jwt.Secret);

            try
            {
                var claimsPrincipal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {

                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = AppSettings.Jwt.validIssuer,
                    ValidAudience = audience ?? AppSettings.Jwt.ValidAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateLifetime = true // تحقق من انتهاء الصلاحية
                }, out SecurityToken validatedToken);


                return Result.Ok(claimsPrincipal);
            }
            catch (SecurityTokenExpiredException ex)
            {
                return Result.Fail(new Error($"The token is expired.").CausedBy(ex));
            }
            catch (Exception ex)
            {
                return Result.Fail(new Error($"Token validation failed: {ex.Message}").CausedBy(ex));
            }
        }

        public Result<ClaimsPrincipal> ValidateTemporaryToken(string token)
        {
            return ValidateToken(token, AppSettings.Jwt.TempSecret);
        }
    }

}
