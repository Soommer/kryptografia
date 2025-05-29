namespace kryptografia.Models
{
    public class EncryptionResponse
    {
        public string CipherText { get; set; }
        public EncryptionMetrics Metrics { get; set; }
    }

    public class EncryptionMetrics
    {
        public long ElapsedMilliseconds { get; set; }
        public long MemoryUsedBytes { get; set; }
    }

}
