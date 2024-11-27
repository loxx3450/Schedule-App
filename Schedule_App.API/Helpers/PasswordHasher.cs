using System.Security.Cryptography;

namespace Schedule_App.API.Helpers
{
    /// <summary>
    /// Hashes password with PBKDF2 and salt.
    /// </summary>
    public static class PasswordHasher
    {
        /// <summary>
        /// Length of salt in bytes.
        /// </summary>
        private const int SALT_LENGTH = 16;

        /// <summary>
        /// Length of hash in bytes.
        /// </summary>
        private const int HASH_LENGTH = 20;

        /// <summary>
        /// Count of iterations for hashing
        /// </summary>
        private const int ITERATIONS_COUNT = 100000;


        /// <summary>
        /// Verifies if a password is equal to a hash.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <param name="hashedPassword">The hash.</param>
        /// <returns>Could be verified?</returns>
        public static bool Verify(string password, string hashedPassword)
        {
            //Get hash bytes
            byte[] hashBytes = Convert.FromBase64String(hashedPassword);

            //Get salt
            byte[] salt = new byte[SALT_LENGTH];
            Array.Copy(hashBytes, salt, SALT_LENGTH);

            //Create hash with given salt
            byte[] hash = MakeHash(password, salt);

            //Verification
            for (int i = 0; i < HASH_LENGTH; ++i)
            {
                if (hashBytes[i + SALT_LENGTH] != hash[i])
                {
                    return false;
                }
            }

            return true;
        }


        /// <summary>
        /// Creates a hash from a password with 100000 iterations and SHA256 algorithm.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <returns>The hash.</returns>
        public static string Hash(string password)
        {
            byte[] salt = MakeSalt();
            byte[] hash = MakeHash(password, salt);

            byte[] hashBytes = Combine(salt, hash);

            return Convert.ToBase64String(hashBytes);
        }


        /// <summary>
        /// Creates salt.
        /// </summary>
        /// <returns>The salt.</returns>
        private static byte[] MakeSalt()
        {
            byte[] salt = new byte[SALT_LENGTH];
            RandomNumberGenerator.Fill(salt);

            return salt;
        }


        /// <summary>
        /// Creates hash basing on password and salt.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <param name="salt">The salt.</param>
        /// <returns>The hash.</returns>
        private static byte[] MakeHash(string password, byte[] salt)
        {
            Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(password, salt, ITERATIONS_COUNT, HashAlgorithmName.SHA256);
            return pbkdf2.GetBytes(HASH_LENGTH);
        }


        /// <summary>
        /// Combines salt and hash.
        /// </summary>
        /// <param name="salt">The salt.</param>
        /// <param name="hash">The hash.</param>
        /// <returns>The combined hash in bytes.</returns>
        private static byte[] Combine(byte[] salt, byte[] hash)
        {
            byte[] hashBytes = new byte[SALT_LENGTH + HASH_LENGTH];

            Array.Copy(salt, 0, hashBytes, 0, SALT_LENGTH);
            Array.Copy(hash, 0, hashBytes, SALT_LENGTH, HASH_LENGTH);

            return hashBytes;
        }
    }
}
