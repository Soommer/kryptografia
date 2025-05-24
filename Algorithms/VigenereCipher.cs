using System.Text;

namespace kryptografia.Algorithms
{
    using System.Text;

    public class VigenereCipher : IEncryptionAlgorithm
    {
        public Task<string> EncryptAsync(string plainText, string key = null)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Key must not be empty.");

            StringBuilder result = new();
            key = NormalizeKey(key, plainText.Length);

            for (int i = 0; i < plainText.Length; i++)
            {
                char p = plainText[i];
                char k = key[i];

                if (char.IsLetter(p))
                {
                    char baseChar = char.IsUpper(p) ? 'A' : 'a';
                    int offset = (p - baseChar + (char.ToUpper(k) - 'A')) % 26;
                    result.Append((char)(baseChar + offset));
                }
                else
                {
                    result.Append(p); 
                }
            }

            return Task.FromResult(result.ToString());
        }

        public Task<string> DecryptAsync(string cipherText, string key = null)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Key must not be empty.");

            StringBuilder result = new();
            key = NormalizeKey(key, cipherText.Length);

            for (int i = 0; i < cipherText.Length; i++)
            {
                char c = cipherText[i];
                char k = key[i];

                if (char.IsLetter(c))
                {
                    char baseChar = char.IsUpper(c) ? 'A' : 'a';
                    int offset = (c - baseChar - (char.ToUpper(k) - 'A') + 26) % 26;
                    result.Append((char)(baseChar + offset));
                }
                else
                {
                    result.Append(c); 
                }
            }

            return Task.FromResult(result.ToString());
        }

        private string NormalizeKey(string key, int length)
        {
            key = key.ToUpper();
            StringBuilder extendedKey = new();

            while (extendedKey.Length < length)
            {
                extendedKey.Append(key);
            }

            return extendedKey.ToString()[..length];
        }
    }

}
