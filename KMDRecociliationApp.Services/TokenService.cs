using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using KMDRecociliationApp.Domain.Entities;
using KMDRecociliationApp.Domain.DTO;
using Microsoft.AspNetCore.Http.HttpResults;

namespace KMDRecociliationApp.Services
{
    public interface ITokenService
    {
        string GenerateToken(string mobilenumber, out DateTime? expiresAt, out DateTime? createdAt);
    }
    public class TokenService : ITokenService
    {
        private readonly string _secret;

        public TokenService(IConfiguration configuration)
        {
            _secret = configuration["Jwt:Key"];
        }

        public string GenerateToken(string mobilenumber,out DateTime? expiresAt,out DateTime? createdAt)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secret);
           
            createdAt = DateTime.UtcNow;
            expiresAt = DateTime.UtcNow.AddHours(12);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                new Claim(ClaimTypes.MobilePhone, mobilenumber)
                }),
                Expires = expiresAt,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
   
}