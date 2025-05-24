namespace kryptografia.Models
{
    public class EncryptionRequest
    {
        public string PlainText { get; set; }
        public string Algorithm { get; set; } 
        public string Key { get; set; } 
    }
}

