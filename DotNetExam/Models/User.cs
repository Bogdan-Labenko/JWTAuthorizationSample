namespace DotNetExam.Models
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
        public Roles Role { get; set; }
    }
    public enum Roles
    {
        Default = 0,
        Admin = 1
    }
}
