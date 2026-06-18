using System.Security.Cryptography;
using System.Text;
using BCryptNet = BCrypt.Net.BCrypt;

namespace PokeGrading.Utilities
{
    public class PasswordService
    {
        public static string HashPassword(string password)
        {
            using MD5 md5 = MD5.Create();

            byte[] inputBytes =
                Encoding.UTF8.GetBytes(password);

            byte[] hashBytes =
                md5.ComputeHash(inputBytes);

            StringBuilder sb =
                new StringBuilder();

            foreach (byte b in hashBytes)
            {
                sb.Append(
                    b.ToString("x2"));
            }

            return sb.ToString();
        }

        public static bool VerifyPassword(
            string plainPassword,
            string storedPasswordHash)
        {
            if (string.IsNullOrWhiteSpace(plainPassword) ||
                string.IsNullOrWhiteSpace(storedPasswordHash))
            {
                return false;
            }

            if (storedPasswordHash.StartsWith("$2a$") ||
                storedPasswordHash.StartsWith("$2b$") ||
                storedPasswordHash.StartsWith("$2y$"))
            {
                return BCryptNet.Verify(
                    plainPassword,
                    storedPasswordHash);
            }

            return HashPassword(plainPassword) == storedPasswordHash;
        }
    }
}