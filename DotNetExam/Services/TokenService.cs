using Azure.Core;
using DotNetExam.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace DotNetExam.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string GenerateAccessToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, Convert.ToInt32(user.Role).ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var issuer = _configuration["JWT:Issuer"];
            var audience = _configuration["JWT:Audience"];
            var secKey = GetSecurityKey();
            var expirationTime = DateTime.UtcNow.AddMinutes(Convert.ToInt32(_configuration["JWT:AccessTokenExpirationMinutes"]));
            var accesToken = new JwtSecurityToken(issuer, audience, claims, null, expirationTime, new SigningCredentials(new SymmetricSecurityKey(secKey), SecurityAlgorithms.HmacSha256Signature));
            return tokenHandler.WriteToken(accesToken);
        }
        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public string GetEmailFromAccessToken(HttpRequest request)
        {
            var accessToken = request.Headers.Authorization;
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(accessToken.ToString().Substring("Bearer ".Length));
            return (string)jwt.Payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"];
        }

        public byte[] GetSecurityKey()
        {
            var key = _configuration["JWT:SecretKey"];
            if (key == null) throw new Exception("Secret key not found!");
            return Encoding.UTF8.GetBytes(key);
        }
    }
}
