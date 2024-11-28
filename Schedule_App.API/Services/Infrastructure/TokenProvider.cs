using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Schedule_App.Core.Models;
using System.Security.Claims;
using System.Text;

namespace Schedule_App.API.Services.Infrastructure
{
    public class TokenProvider
    {
        private readonly IConfiguration _configuration;

        public TokenProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(Teacher teacher)
        {
            string secretKey = _configuration.GetSection("AppSettings:Token").Value!;
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var expirationTime = DateTime.Now.AddMinutes(_configuration.GetValue<double>("AppSettings: ExpireTimeInMins"));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                [
                    new Claim(ClaimTypes.Role, nameof(Teacher)),
                    new Claim(ClaimTypes.NameIdentifier, teacher.Id.ToString()),
                    new Claim(ClaimTypes.Name, teacher.Username),
                ]),
                Expires = expirationTime,
                SigningCredentials = credentials,
            };

            var handler = new JsonWebTokenHandler();

            string token = handler.CreateToken(tokenDescriptor);

            return token;
        }
    }
}
