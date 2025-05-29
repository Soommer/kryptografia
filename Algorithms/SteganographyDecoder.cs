namespace kryptografia.Algorithms
{
    using kryptografia.Controllers;
    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.PixelFormats;
    using System.Text;

    public class SteganographyDecoder: ISteganographyDecoder
    {
        public async Task<byte[]> ExtractImageAsync(Stream imageStream)
        {
            using var image = await Image.LoadAsync<Rgba32>(imageStream);

            var bits = new List<int>();
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Rgba32 pixel = image[x, y];
                    bits.Add(pixel.R & 1);
                    bits.Add(pixel.G & 1);
                    bits.Add(pixel.B & 1);
                }
            }

            byte[] GetBytes(IEnumerable<int> bitsSource, int count) =>
                bitsSource.Take(count * 8).Chunk(8).Select(b => (byte)b.Select((bit, i) => bit << (7 - i)).Sum()).ToArray();

            var lengthBytes = GetBytes(bits, 4);
            int messageLength = BitConverter.ToInt32(lengthBytes);

            var imageBytes = GetBytes(bits.Skip(32), messageLength);
            return imageBytes;
        }

        public async Task<string> ExtractMessageAsync(Stream imageStream)
        {
            using var image = await Image.LoadAsync<Rgba32>(imageStream);
            int width = image.Width;
            int height = image.Height;

            var bits = new List<int>();

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Rgba32 pixel = image[x, y];
                    bits.Add(pixel.R & 1);
                    bits.Add(pixel.G & 1);
                    bits.Add(pixel.B & 1);
                }
            }

            byte[] GetBytes(IEnumerable<int> bitsSource, int count)
            {
                return bitsSource
                    .Take(count * 8)
                    .Chunk(8)
                    .Select(b => (byte)b.Select((bit, i) => bit << (7 - i)).Sum())
                    .ToArray();
            }

            var lengthBytes = GetBytes(bits, 4);
            int messageLength = BitConverter.ToInt32(lengthBytes);

            if (messageLength == 0)
                throw new InvalidOperationException("No hidden message found.");

            var messageBytes = GetBytes(bits.Skip(32), messageLength);
            return Encoding.UTF8.GetString(messageBytes);
        }
    }
}
