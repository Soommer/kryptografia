namespace kryptografia.Models
{
    public class SteganographyImageEmbedRequest
    {
        public IFormFile HostImage { get; set; }
        public IFormFile HiddenImage { get; set; }
    }
}
