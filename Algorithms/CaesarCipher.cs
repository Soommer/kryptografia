namespace kryptografia.Algorithms
{
    public class CaesarCipher : IEncryptionAlgorithm
    {
        private int ParseKey(string key)
        {
            if (int.TryParse(key, out int shift))
                return shift % 26;

            throw new ArgumentException("Invalid Caesar cipher key. Must be an integer.");
        }

        public Task<string> EncryptAsync(string plainText, string key = null)
        {
            int shift = ParseKey(key);
            string resoult = new string(plainText.Select(c => ShiftChar(c, shift)).ToArray());
            return Task.FromResult(resoult);
        }

        public Task<string> DecryptAsync(string cipherText, string key = null)
        {
            int shift = ParseKey(key);
            string resoult = new string(cipherText.Select(c => ShiftChar(c, 26 - shift)).ToArray());
            return Task.FromResult(resoult);
        }

        private char ShiftChar(char c, int shift)
        {
            if (char.IsLetter(c))
            {
                char offset = char.IsUpper(c) ? 'A' : 'a';
                return (char)(((c - offset + shift) % 26) + offset);
            }

            return c;
        }
    }

}
