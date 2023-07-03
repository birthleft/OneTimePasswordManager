using Microsoft.Extensions.Primitives;
using System.Text;
using System.Text.RegularExpressions;

namespace OneTimePasswordManager.Utilities
{
    public static class PasswordUtility
    {
        public static string Generate(string input)
        {
            Random random = new();
            using (var sha1 = System.Security.Cryptography.SHA1.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = sha1.ComputeHash(inputBytes);

                StringBuilder stringBuilder = new StringBuilder();
                foreach (byte hashByte in hashBytes)
                {
                    var character = hashByte.ToString("x2");
                    if (random.NextDouble() < 0.5)
                    {
                        stringBuilder.Append(character);
                    }
                    else
                    {
                        stringBuilder.Append(character.ToUpper());
                    }

                    if (stringBuilder.Length >= 10)
                    {
                        break;
                    }
                }
                return stringBuilder.ToString();
            }
        }
    }
}
