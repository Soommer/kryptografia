using kryptografia.Algorithms;
using kryptografia.Models;
using Microsoft.AspNetCore.Mvc;

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
                return BadRequest("Image and message are required.");
            }

            try
            {
                using var stream = request.Image.OpenReadStream();
                var resultImage = await _encoder.EmbedMessageAsync(stream, request.Message);

                _logger.LogInformation("Message successfully embedded in image.");
                return File(resultImage, "image/png", "steganography.png");
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
                return BadRequest("Image is required.");
            }

            try
            {
                using var stream = request.Image.OpenReadStream();
                var message = await _decoder.ExtractMessageAsync(stream);

                _logger.LogInformation("Message successfully extracted from image.");
                return Ok(new { message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while extracting message from image.");
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}

