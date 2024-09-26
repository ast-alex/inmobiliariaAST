using System.Security.Cryptography;

namespace inmobiliariaAST.Services
{
    public class AuthenticationService
    {
        // Hash a password
        public string HashPassword(string password)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, 16, 10000, HashAlgorithmName.SHA256))
            {
                var hash = pbkdf2.GetBytes(20);
                var salt = pbkdf2.Salt;
                var hashBytes = new Byte[36];
                Array.Copy(salt, 0, hashBytes, 0, 16);
                Array.Copy(hash, 0, hashBytes, 16, 20);
                return Convert.ToBase64String(hashBytes);
            }
        }

        // Verify a password
        public bool VerifyPassword(string hashedPassword, string password)
        {
            var hashBytes = Convert.FromBase64String(hashedPassword);
            var salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);
            var storedHash = new byte[20];
            Array.Copy(hashBytes, 16, storedHash, 0, 20);

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256))
            {
                var hash = pbkdf2.GetBytes(20);
                for (int i = 0; i < 20; i++)
                {
                    if (hash[i] != storedHash[i])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

    }
}
