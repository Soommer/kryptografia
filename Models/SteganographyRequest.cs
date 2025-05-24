namespace kryptografia.Models
{
    public class SteganographyEmbedRequest
    {
        public IFormFile Image { get; set; }
        public string Message { get; set; }
    }

    public class SteganographyExtractRequest
    {
        public IFormFile Image { get; set; }
    }
}
