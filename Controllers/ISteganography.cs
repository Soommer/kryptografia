

namespace kryptografia.Controllers
{
    public interface ISteganography
    {
        Task<byte[]> EmbedMessageAsync(Stream imageStream, string message);

        Task<byte[]> EmbedImageAsync(Stream hostStream, Stream hiddenStream);
    }

    public interface ISteganographyDecoder
    {
        Task<string> ExtractMessageAsync(Stream imageStream);
        Task<byte[]> ExtractImageAsync(Stream imageStream);
    }
}
