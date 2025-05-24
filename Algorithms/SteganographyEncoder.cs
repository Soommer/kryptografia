namespace kryptografia.Algorithms
{
    using kryptografia.Controllers;
    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.PixelFormats;
    using System.Text;

    public class SteganographyEncoder : ISteganography
    {
        public async Task<byte[]> EmbedMessageAsync(Stream imageStream, string message)
        {
            var messageBytes = Encoding.UTF8.GetBytes(message);
            int messageLength = messageBytes.Length;

            using var image = await Image.LoadAsync<Rgba32>(imageStream);
            int width = image.Width;
            int height = image.Height;

            byte[] lengthPrefix = BitConverter.GetBytes(messageLength);
            byte[] fullMessage = lengthPrefix.Concat(messageBytes).ToArray();

            int totalBitsNeeded = fullMessage.Length * 8;
            int totalAvailableBits = width * height * 3; 

            if (totalBitsNeeded > totalAvailableBits)
                throw new InvalidOperationException("Image is too small to embed the message.");

            int bitIndex = 0;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (bitIndex >= totalBitsNeeded)
                        break;

                    Rgba32 pixel = image[x, y];
                    byte[] colors = new[] { pixel.R, pixel.G, pixel.B };

                    for (int i = 0; i < 3; i++)
                    {
                        if (bitIndex >= totalBitsNeeded)
                            break;

                        int byteIndex = bitIndex / 8;
                        int bitInByte = 7 - (bitIndex % 8);
                        int bit = (fullMessage[byteIndex] >> bitInByte) & 1;

                        colors[i] = (byte)((colors[i] & 0xFE) | bit);
                        bitIndex++;
                    }

                    image[x, y] = new Rgba32(colors[0], colors[1], colors[2], pixel.A);
                }
            }

            using var output = new MemoryStream();
            await image.SaveAsPngAsync(output);
            return output.ToArray();
        }
    }

}
