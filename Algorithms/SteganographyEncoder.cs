namespace kryptografia.Algorithms
{
    using kryptografia.Controllers;
    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.PixelFormats;
    using System.Text;

    public class SteganographyEncoder : ISteganography
    {
        public async Task<byte[]> EmbedImageAsync(Stream hostStream, Stream hiddenStream)
        {
            using var hostImage = await Image.LoadAsync<Rgba32>(hostStream);
            using var hiddenImage = await Image.LoadAsync<Rgba32>(hiddenStream);


            using var ms = new MemoryStream();
            await hiddenImage.SaveAsPngAsync(ms);
            byte[] hiddenBytes = ms.ToArray();

            byte[] lenghtPrefix = BitConverter.GetBytes(hiddenBytes.Length);
            byte[] fullPayload = lenghtPrefix.Concat(hiddenBytes).ToArray();

            int totalBits = fullPayload.Length * 8;
            if (totalBits > hostImage.Width * hostImage.Height * 3)
                throw new InvalidOperationException("Host image too small");

            int bitIndex = 0;
            for (int y = 0; y < hostImage.Height; y++)
            {
                for (int x = 0; x < hostImage.Width; x++)
                {
                    if(bitIndex >= totalBits) break;
                    Rgba32 pixels = hostImage[x, y];
                    byte[] colors = new[] { pixels.R, pixels.G, pixels.B, };

                    for (int i = 0; i < 3; i++)
                    {
                        if (bitIndex >= totalBits) break;

                        int byteIndex = bitIndex / 8;
                        int bitInByte = 7 - (bitIndex % 8);
                        int bit = (fullPayload[byteIndex] >> bitInByte) & 1;

                        colors[i] = (byte)((colors[i] & 0xFE) | bit);
                        bitIndex++;
                    }

                    hostImage[x, y] = new Rgba32(colors[0], colors[1], colors[2], pixels.A);

                }
            }
            var output = new MemoryStream();
            await hostImage.SaveAsPngAsync(output);
            return output.ToArray();
        }

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
