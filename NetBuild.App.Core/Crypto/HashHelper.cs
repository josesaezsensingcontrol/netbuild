using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace NetBuild.App.Core.Crypto
{
    public static class HashHelper
    {
        public static string HashPassword(string password, out string salt, int iterationCount = 10000)
        {
            byte[] saltBytes = new byte[128 / 8];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(saltBytes);

            salt = Convert.ToBase64String(saltBytes);

            string hashed = Convert.ToBase64String(
                KeyDerivation.Pbkdf2(
                    password: password,
                    salt: saltBytes,
                    prf: KeyDerivationPrf.HMACSHA1,
                    iterationCount: iterationCount,
                    numBytesRequested: 256 / 8
                )
            );

            return hashed;
        }

        public static bool ValidateHashedPassword(string password, string passwordSalt, string hashedPassword, int iterationCount = 10000)
        {
            var salt = Convert.FromBase64String(passwordSalt);

            string hashed = Convert.ToBase64String(
                KeyDerivation.Pbkdf2(
                    password: password,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA1,
                    iterationCount: iterationCount,
                    numBytesRequested: 256 / 8
                )
            );

            return hashed == hashedPassword;
        }
    }
}
