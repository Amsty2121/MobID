using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace MobID.MainGateway.Utility
{
    public static class RandomUtility
    {
        public static string GetRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            var stringBuilder = new StringBuilder(length);

            for (int i = 0; i < length; i++)
            {
                stringBuilder.Append(chars[random.Next(chars.Length)]);
            }

            return stringBuilder.ToString();
        }

        public static int GetRandomInt(int min, int max)
        {
            var random = new Random();
            return random.Next(min, max + 1);
        }

        public static int GetCalculatedRandomInt(int previousIndex)
        {
            var seed = GenerateSeedFromActionId(previousIndex);

            var random = new Random(seed);

            return random.Next(1, 11);
        }

        private static int GenerateSeedFromActionId(int previousIndex)
        {
            var seed = (previousIndex + 100).ToString();
            using (SHA256 sha256 = SHA256.Create())
            {
                var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(seed));
                return BitConverter.ToInt32(hashBytes, 0);
            }
        }
    }
}
