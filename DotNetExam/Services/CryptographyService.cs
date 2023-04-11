using System.Security.Cryptography;
using System.Text;

namespace DotNetExam.Services
{
    public class CryptographyService
    {
        public string GetSha256Hash(string value)
        {
            using (var sha256Hash = SHA256.Create())
            {
                // Конвертация входной строки в массив байтов
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(value));

                // Преобразование массива байтов в строку шестнадцатеричных чисел
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
