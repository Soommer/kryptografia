namespace kryptografia.Controllers
{
    public interface ISteganography
    {
        Task<byte[]> EmbedMessageAsync(Stream imageStream, string message);
    }

    public interface ISteganographyDecoder
    {
        Task<string> ExtractMessageAsync(Stream imageStream);
    }
}
