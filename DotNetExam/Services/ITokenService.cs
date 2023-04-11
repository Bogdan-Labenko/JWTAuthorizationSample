using DotNetExam.Models;
using System.Security.Claims;

namespace DotNetExam.Services
{
    public interface ITokenService
    {
        string GenerateAccessToken(ClaimsIdentity claims);
        string GenerateRefreshToken();
        public byte[] GetSecurityKey();
    }
}
