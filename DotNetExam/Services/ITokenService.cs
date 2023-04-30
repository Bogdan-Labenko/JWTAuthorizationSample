using DotNetExam.Models;
using System.Security.Claims;

namespace DotNetExam.Services
{
    public interface ITokenService
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();
        string GetEmailFromAccessToken(HttpRequest request);
        public byte[] GetSecurityKey();
    }
}
