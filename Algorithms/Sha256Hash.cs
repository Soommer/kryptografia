using System.Security.Cryptography;
using System.Text;

namespace kryptografia.Algorithms
{
    public class Sha256Hash : IEncryptionAlgorithm
    {
        public Task<string> EncryptAsync(string plainText, string key = null)
        {
            using var sha256 = SHA256.Create();
            byte[] inputBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] hashBytes = sha256.ComputeHash(inputBytes);

            var result = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            return Task.FromResult(result);
        }

        public Task<string> DecryptAsync(string cipherText, string key = null)
        {
            throw new NotSupportedException("SHA-256 cannot be decrypted.");
        }
    }
}
