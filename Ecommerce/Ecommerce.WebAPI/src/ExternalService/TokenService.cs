using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Ecommerce.Core.src.Entities;
using Ecommerce.Service.src.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;

namespace Ecommerce.WebAPI.src.ExternalService
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _cache;


        public TokenService(IConfiguration configuration, IMemoryCache cache)
        {
            _configuration = configuration;
            _cache = cache;
        }
        public string GetToken(User foundUser)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, foundUser.Email!),
                new Claim(ClaimTypes.NameIdentifier, foundUser.Id.ToString()),
                new Claim(ClaimTypes.Role, foundUser.Role.ToString()),
                new Claim("UserId", foundUser.Id.ToString())
            };
            var jwtKey = _configuration["Secrets:JwtKey"] ?? throw new ArgumentNullException("JwtKey is not found in appsettings.json");
            var securityKey = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)), SecurityAlgorithms.HmacSha256Signature);

            // token handler
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = securityKey,
                Issuer = _configuration["Secrets:Issuer"],
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            // Store the token with a consistent key
            _cache.Set("JwtToken", token);
            return tokenHandler.WriteToken(token);
        }


        public async Task<string> InvalidateTokenAsync()
        {
            var storedToken = _cache.Get("JwtToken");
            if (storedToken is not null)
            {
                _cache.Remove("JwtToken");
                await Task.CompletedTask;
                return "removed";
            }
            else
            {
                await Task.CompletedTask;
                return "already removed";
            }
        }

        public Guid VerifyToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtKey = _configuration["Secrets:JwtKey"];

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey!)),
                ValidateIssuer = true,
                ValidIssuer = _configuration["Secrets:Issuer"],
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out var validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var userIdClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == "UserId");

            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                throw new SecurityTokenException("Invalid token or user ID claim not found.");
            }
            return userId;
        }
    }
}