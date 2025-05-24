using System.Security.Cryptography;
using System.Text;


namespace kryptografia.Algorithms
{
    public class RsaEncryption : IEncryptionAlgorithm
    {
        public async Task<string> EncryptAsync(string plainText, string publicKeyXml)
        {
            if (string.IsNullOrWhiteSpace(publicKeyXml))
                throw new ArgumentException("Public key is required for RSA encryption.");

            using var rsa = RSA.Create();
            rsa.FromXmlString(publicKeyXml); 

            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] encryptedBytes = rsa.Encrypt(plainBytes, RSAEncryptionPadding.OaepSHA256);

            return Convert.ToBase64String(encryptedBytes);
        }

        public async Task<string> DecryptAsync(string cipherText, string privateKeyXml)
        {
            if (string.IsNullOrWhiteSpace(privateKeyXml))
                throw new ArgumentException("Private key is required for RSA decryption.");

            using var rsa = RSA.Create();
            rsa.FromXmlString(privateKeyXml);

            byte[] encryptedBytes = Convert.FromBase64String(cipherText);
            byte[] decryptedBytes = rsa.Decrypt(encryptedBytes, RSAEncryptionPadding.OaepSHA256);

            return Encoding.UTF8.GetString(decryptedBytes);
        }
    }
}
