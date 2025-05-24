namespace kryptografia.Algorithms
{
    public class EncryptionFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public EncryptionFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IEncryptionAlgorithm GetAlgorithm(string algorithmName)
        {
            return algorithmName.ToLower() switch
            {
                "cezar" => _serviceProvider.GetRequiredService<CaesarCipher>(),
                "vigenere" => _serviceProvider.GetRequiredService<VigenereCipher>(),
                "sha256" => _serviceProvider.GetRequiredService<Sha256Hash>(),
                "aes" => _serviceProvider.GetRequiredService<AesEncryption>(),
                "rsa" => _serviceProvider.GetRequiredService<RsaEncryption>(),
                _ => throw new ArgumentException($"Unsupported algorithm: {algorithmName}")
            };
        }
    }

}
