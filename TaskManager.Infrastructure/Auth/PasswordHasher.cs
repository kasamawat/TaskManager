using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Application.Interfaces;

namespace TaskManager.Infrastructure.Auth
{
    public class PasswordHasher : IPasswordHasher
    {
        private readonly int SaltSize = 16; // 128 bit
        private readonly int KeySize = 32; // 256 bit
        private readonly int Iterations = 10000; // can edit
        public string Hash(string password)
        {
            using var algorithm = new Rfc2898DeriveBytes(
                password,
                SaltSize,
                Iterations,
                HashAlgorithmName.SHA256
            );

            var salt = algorithm.Salt;
            var key = algorithm.GetBytes(KeySize);

            // format: iterations.salt.key (base64)
            return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(key)}";
        }

        public bool VerifyPassword(string hash, string password)
        {
            var parts = hash.Split('.', 3);
            if(parts.Length != 3) 
            {
                return false;
            }

            var iterations = int.Parse(parts[0]);
            var salt = Convert.FromBase64String(parts[1]);
            var key = Convert.FromBase64String(parts[2]);

            using var algorithm = new Rfc2898DeriveBytes(
                password,
                salt,
                iterations,
                HashAlgorithmName.SHA256
            );

            var keyToCheck = algorithm.GetBytes(KeySize);

            return CryptographicOperations.FixedTimeEquals(keyToCheck, key);
        }
    }
}
