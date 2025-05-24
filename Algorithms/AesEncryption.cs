using System.Security.Cryptography;
using System.Text;


namespace kryptografia.Algorithms
{
    public class AesEncryption : IEncryptionAlgorithm
    {
        public async Task<string> EncryptAsync(string plainText, string key = null)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Key required");

            using Aes aes = Aes.Create();
            aes.Key = GetAesKeyBytes(key);
            aes.GenerateIV();

            using var encryptor = aes.CreateEncryptor();
            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

            byte[] result = aes.IV.Concat(cipherBytes).ToArray();
            return Convert.ToBase64String(result);
        }

        public async Task<string> DecryptAsync(string ciperText, string key = null)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Key is required");

            byte[] fullCipher = Convert.FromBase64String(ciperText);
            byte[] iv = fullCipher.Take(16).ToArray();
            byte[] cipherBytes = fullCipher.Skip(16).ToArray();

            using Aes aes = Aes.Create();
            aes.Key = GetAesKeyBytes(key);
            aes.IV = iv;

            using var decryptor = aes.CreateDecryptor();
            byte[] decryptedBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);

            return Encoding.UTF8.GetString(decryptedBytes);
        }

        private byte[] GetAesKeyBytes(string key)
        {
            using var sha256 = SHA256.Create();
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            return sha256.ComputeHash(keyBytes).Take(16).ToArray(); 
        }
    }
}
