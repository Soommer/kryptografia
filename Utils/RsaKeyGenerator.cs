using System.Security.Cryptography;

namespace kryptografia.Utils
{
    public static class RsaKeyGenerator
    {
        public static (string PublicKeyXml, string PrivateKeyXml) GenerateKeyPair()
        {
            using var rsa = RSA.Create(2048);
            string publicKey = rsa.ToXmlString(false); 
            string privateKey = rsa.ToXmlString(true); 
            return (publicKey, privateKey);
        }
    }
}
