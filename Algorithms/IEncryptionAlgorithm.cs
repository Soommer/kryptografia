namespace kryptografia.Algorithms
{
    public interface IEncryptionAlgorithm
    {
        Task<string> EncryptAsync(string plainText, string key = null);
        Task<string> DecryptAsync(string cipherText, string key = null);
    }

}
