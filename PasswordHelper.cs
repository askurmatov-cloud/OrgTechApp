using System;
using System.Security.Cryptography;
using System.Text;

namespace OrgTechApp
{
    public static class PasswordHelper
    {
        public static string Hash(string salt, string password)
        {
            // SQL Server HASHBYTES('SHA2_256', N'...') хэширует UTF-16LE.
            using (var sha = SHA256.Create())
            {
                var bytes = Encoding.Unicode.GetBytes((salt ?? string.Empty) + ":" + (password ?? string.Empty));
                return Convert.ToHexString(sha.ComputeHash(bytes));
            }
        }

        public static string NewSalt()
        {
            return Guid.NewGuid().ToString("N").Substring(0, 8);
        }
    }
}
