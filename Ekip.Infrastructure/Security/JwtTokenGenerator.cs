using Ekip.Application.Interfaces;
using Ekip.Domain.Entities.Identity.Entities;
using Ekip.Domain.Entities.ReadModels;
using Ekip.Infrastructure.Configurations;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JwtRegisteredClaimNames = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;

namespace Ekip.Infrastructure.Security
{
    public class JwtTokenGenerator : IJwtTokenGenerator
    {

        private readonly InfrastructureSettings _settings;
        public JwtTokenGenerator(IOptions<InfrastructureSettings> settings)
        {
            _settings = settings.Value;
        }


        public string GenerateToken(UserReadModel user)
        {
            return CreateToken(user.Id.ToString() , user.Email , user.UserName);
        }

        public string GenerateToken(User user)
        {
            return CreateToken(user.Id.ToString() , user.Email , user.UserName);
        }

        private string CreateToken(string userId , string email , string userName)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim("UserName", userName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.JwtSecret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: null,
                audience: null,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_settings.JwtExpiryMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


    }
}
