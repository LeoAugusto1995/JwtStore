using System.Security.Cryptography;

namespace JwtStore.Core.Contexts.AccountContext.ValueObjects
{
    public class Password
    {
        private const string Valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private const string Special = "!@#$%^&*()_+-=[]{}|;':\",./<>?";
        protected Password() { }
        public Password(string? password = null)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrWhiteSpace(password))
                password = GenerateRandomPassword();

            Hash = Hashing(password);
        }


        public string Hash { get; } = string.Empty;
        public string ResetCode { get; } = Guid.NewGuid().ToString("N")[..8].ToUpper();

        private static string GenerateRandomPassword(int length = 16, bool includeSpecialCharacters = true, bool uppercase = false)
        {
            var chars = includeSpecialCharacters ? Valid + Special : Valid;
            var startRandom = uppercase ? 26 : 0;
            var index = 0;
            var res = new char[length];
            var rnd = new Random();

            while (index < length)
                res[index++] = chars[rnd.Next(startRandom, chars.Length)];

            return new string(res);
        }
        private static string Hashing(string password, short saltSize = 16, short keySize = 32, int iterations = 10000, char splitChar = '.')
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("Senha é obrigatória.", nameof(password));

            if (saltSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(saltSize));

            if (keySize <= 0)
                throw new ArgumentOutOfRangeException(nameof(keySize));

            if (iterations <= 0)
                throw new ArgumentOutOfRangeException(nameof(iterations));

            password += Configuration.Secrets.PasswordSaltKey;

            byte[] salt = RandomNumberGenerator.GetBytes(saltSize);
            byte[] key = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                iterations,
                HashAlgorithmName.SHA256,
                keySize);

            return string.Join(
                splitChar,
                iterations,
                Convert.ToBase64String(salt),
                Convert.ToBase64String(key));
        }

        private static bool Verify(string hash, string password, short keySize = 32, int iterations = 10000, char splitChar = '.')
        {
            if (string.IsNullOrEmpty(hash) || string.IsNullOrEmpty(password))
                return false;

            if (keySize <= 0 || iterations <= 0)
                return false;

            var parts = hash.Split(splitChar, 3);
            if (parts.Length != 3 ||
                !int.TryParse(parts[0], out int hashIterations) ||
                hashIterations != iterations)
            {
                return false;
            }

            try
            {
                byte[] salt = Convert.FromBase64String(parts[1]);
                byte[] expectedKey = Convert.FromBase64String(parts[2]);

                if (expectedKey.Length != keySize)
                    return false;

                password += Configuration.Secrets.PasswordSaltKey;

                byte[] keyToCheck = Rfc2898DeriveBytes.Pbkdf2(
                    password,
                    salt,
                    hashIterations,
                    HashAlgorithmName.SHA256,
                    keySize);

                return CryptographicOperations.FixedTimeEquals(keyToCheck, expectedKey);
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
