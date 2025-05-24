namespace kryptografia.Controllers
{
    public interface IEncryptionService
    {
        Task<string> EncryptAsync(string plainText, string algorithm, string key = null);
        Task<string> DecryptAsync(string cipherText, string algorithm, string key = null);
    }
}
