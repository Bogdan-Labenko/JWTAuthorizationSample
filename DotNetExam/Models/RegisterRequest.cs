using Azure.Identity;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;

namespace DotNetExam.Entities
{
    public class RegisterRequest
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; } = null!;

        [Required]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [DataType(DataType.Password)]
        [Display(Name = "Подтвердить пароль")]
        public string PasswordConfirm { get; set; } = null!;
        [Display(Name = "Хеш пароля")]
        public string PasswordHash
        {
            get
            {
                var sha256 = SHA256.Create();
                var hashedPasswordBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(Password));
                return Convert.ToBase64String(hashedPasswordBytes);
            }
        }
        [Required]
        [Display(Name = "Имя")]
        public string UserName { get; set; } = null!;
    }
}
