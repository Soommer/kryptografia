using kryptografia.Algorithms;
using kryptografia.Controllers;

namespace kryptografia.Services
{
    public class EncryptionService : IEncryptionService
    {
        private readonly EncryptionFactory _factory;

        public EncryptionService(EncryptionFactory factory)
        {
            _factory = factory;
        }

        public async Task<string> EncryptAsync(string text, string algorithm, string key)
        {
            var algo = _factory.GetAlgorithm(algorithm);
            return await algo.EncryptAsync(text, key);
        }

        public async Task<string> DecryptAsync(string text, string algorithm, string key)
        {
            var algo = _factory.GetAlgorithm(algorithm);
            return await algo.DecryptAsync(text, key);
        }
    }

}
