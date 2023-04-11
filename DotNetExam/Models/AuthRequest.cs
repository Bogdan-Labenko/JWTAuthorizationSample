using System.Security.Cryptography;
using System.Text;

namespace DotNetExam.Models
{
    public class AuthRequest
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string PasswordHash
        {
            get
            {
                var sha256 = SHA256.Create();
                var hashedPasswordBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(Password));
                return Convert.ToBase64String(hashedPasswordBytes);
            }
        }
    }
}
