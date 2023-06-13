using System.Security.Cryptography;
using System.Text;

namespace API.Utils
{
    public class HashGenerator
    {
        public static string GetSHA256Hash(string input)
        {
            StringBuilder builder = new();
            byte[] hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));

            foreach (byte b in hashBytes)
                builder.Append(b.ToString("x2"));

            return builder.ToString();
        }

        public static string GetSHA256Hash(string input, string salt)
        {
            return GetSHA256Hash(input + salt);
        }

        public static string GenerateSalt(int length)
        {
            Random random = new();
            StringBuilder builder = new();

            for (; length > 0; length--)
            {
                switch (random.Next(1, 4))
                {
                    case 1:
                        builder.Append((char)random.Next(49, 58));
                        break;
                    case 2:
                        builder.Append((char)random.Next(65, 91));
                        break;
                    case 3:
                        builder.Append((char)random.Next(97, 122));
                        break;
                }
            }

            return builder.ToString();
        }
    }
}
