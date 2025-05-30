using kryptografia.Algorithms;
using kryptografia.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace kryptografia.Controllers
{


    [ApiController]
    [Route("steganography")]
    public class SteganographyController : ControllerBase
    {
        private readonly ISteganography _encoder;
        private readonly ISteganographyDecoder _decoder;
        private readonly ILogger<SteganographyController> _logger;

        public SteganographyController(
            ISteganography encoder,
            ISteganographyDecoder decoder,
            ILogger<SteganographyController> logger)
        {
            _encoder = encoder;
            _decoder = decoder;
            _logger = logger;
        }

        [HttpPost("embed")]
        public async Task<IActionResult> Embed([FromForm] SteganographyEmbedRequest request)
        {
            _logger.LogInformation("Embed request received. Message length: {Length}", request?.Message?.Length ?? 0);

            if (request.Image == null || string.IsNullOrEmpty(request.Message))
            {
                _logger.LogWarning("Invalid embed request: missing image or message.");
                return BadRequest(new { error = "Image and message are required." });
            }

            try
            {
                using var stream = request.Image.OpenReadStream();

                var stopwatch = Stopwatch.StartNew();
                long beforeMemory = GC.GetTotalMemory(false);

                var resultImage = await _encoder.EmbedMessageAsync(stream, request.Message);

                stopwatch.Stop();
                long afterMemory = GC.GetTotalMemory(false);

                _logger.LogInformation("Message successfully embedded in image.");
                var file = File(resultImage, "image/png", "steganography.png");
                Response.Headers.Add("Czas", $"{stopwatch.ElapsedMilliseconds}");
                Response.Headers.Add("Ram", $"{afterMemory - beforeMemory}");
                return file;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while embedding message in image.");
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("extract")]
        public async Task<IActionResult> Extract([FromForm] SteganographyExtractRequest request)
        {
            _logger.LogInformation("Extract request received.");

            if (request.Image == null)
            {
                _logger.LogWarning("Invalid extract request: image is missing.");
                return BadRequest((new { error = "Image is required." }));
            }

            try
            {
                using var stream = request.Image.OpenReadStream();

                var stopwatch = Stopwatch.StartNew();
                long beforeMemory = GC.GetTotalMemory(false);

                var message = await _decoder.ExtractMessageAsync(stream);

                stopwatch.Stop();
                long afterMemory = GC.GetTotalMemory(false);

                _logger.LogInformation("Message successfully extracted from image.");
                return Ok(new EncryptionResponse {
                    CipherText = message, Metrics = new EncryptionMetrics {
                    ElapsedMilliseconds = stopwatch.ElapsedMilliseconds,
                    MemoryUsedBytes = afterMemory - beforeMemory
                } });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while extracting message from image.");
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("embed-image")]
        public async Task<IActionResult> EmbedImage([FromForm] SteganographyImageEmbedRequest request)
        {
            if (request.HostImage == null || request.HiddenImage == null)
                return BadRequest(new { error = "Both images are required." });

            using var host = request.HostImage.OpenReadStream();
            using var hidden = request.HiddenImage.OpenReadStream();

            var stopwatch = Stopwatch.StartNew();
            long beforeMemory = GC.GetTotalMemory(false);

            if (request.HiddenImage.Length > request.HostImage.Length)
                return BadRequest(new {error="Hidden image must not be larger than host image." });

            try
            {
                var resultImage = await _encoder.EmbedImageAsync(host, hidden);

                stopwatch.Stop();
                long afterMemory = GC.GetTotalMemory(false);

                var file = File(resultImage, "image/png", "hidden-image.png");

                Response.Headers.Add("Czas", $"{stopwatch.ElapsedMilliseconds}");
                Response.Headers.Add("Ram", $"{afterMemory - beforeMemory}");

                return  file;
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("extract-image")]
        public async Task<IActionResult> ExtractImage([FromForm] SteganographyImageExtractRequest request)
        {
            if (request.Image == null)
                return BadRequest(new { error = "Image is required." });

            var stopwatch = Stopwatch.StartNew();
            long beforeMemory = GC.GetTotalMemory(false);

            using var stream = request.Image.OpenReadStream();

            try
            {
                var result = await _decoder.ExtractImageAsync(stream);

                stopwatch.Stop();
                long afterMemory = GC.GetTotalMemory(false);

                Response.Headers.Add("Czas", $"{stopwatch.ElapsedMilliseconds}");
                Response.Headers.Add("Ram", $"{afterMemory - beforeMemory}");

                return File(result, "image/png", "recovered.png");
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}

