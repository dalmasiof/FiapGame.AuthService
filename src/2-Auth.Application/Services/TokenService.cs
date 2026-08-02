using DTOs;
using Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Services
{
    public class TokenService(
        IConfiguration config,
        IRsaKeyProvider rsaKeyProvider) : ITokenService
    {
        public string GerarToken(LerLoginDTO login)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, login.IdLogin.ToString()),
                new Claim(ClaimTypes.Email, login.Email),
                new Claim(ClaimTypes.Role, login.TipoUsuario.ToString()),
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = config["Jwt:Issuer"],
                Audience = config["Jwt:Audience"],
                SigningCredentials = rsaKeyProvider.SigningCredentials
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
