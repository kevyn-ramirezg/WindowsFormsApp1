using System.Security.Cryptography;
using System.Text;

namespace WindowsFormsApp1.Utils
{
    public static class HashHelper
    {
        public static string Sha256(string plain)
        {
            if (plain == null) plain = string.Empty;
            using (var sha = SHA256.Create())
            {
                var data = Encoding.UTF8.GetBytes(plain);
                var hash = sha.ComputeHash(data);
                var sb = new StringBuilder(hash.Length * 2);
                foreach (var b in hash) sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }
    }
}
