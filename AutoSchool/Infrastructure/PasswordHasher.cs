using System.Security.Cryptography;
using System.Text;

namespace AutoSchool.Infrastructure
{
    public static class PasswordHasher
    {
        public static string Hash(string password)
        {
            using SHA256 sha256 = SHA256.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(password);
            byte[] hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}