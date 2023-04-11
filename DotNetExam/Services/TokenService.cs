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
        public string GenerateAccessToken(ClaimsIdentity claims)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var issuer = _configuration["JWT:Issuer"];
            var audience = _configuration["JWT:Audience"];
            var secKey = GetSecurityKey();
            var expirationTime = DateTime.UtcNow.AddMinutes(Convert.ToInt32(_configuration["JWT:AccessTokenExpirationMinutes"]));
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Issuer = issuer,
                Audience = audience,
                Subject = claims,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secKey), SecurityAlgorithms.HmacSha256Signature),
                Expires = expirationTime
            };
            var accesToken = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
            return tokenHandler.WriteToken(accesToken);
        }
        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
        public byte[] GetSecurityKey()
        {
            var key = _configuration["JWT:SecretKey"];
            if (key == null) throw new Exception("Secret key not found!");
            return Encoding.UTF8.GetBytes(key);
        }
    }
}
