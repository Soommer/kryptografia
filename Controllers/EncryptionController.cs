using kryptografia.Models;
using kryptografia.Services;
using kryptografia.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace kryptografia.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EncryptionController : ControllerBase
    {
        private readonly IEncryptionService _service;
        private readonly ILogger<EncryptionController> _logger;

        private const int MaxTextLength = 500;

        public EncryptionController(IEncryptionService service, ILogger<EncryptionController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpPost("encrypt")]
        public async Task<ActionResult<EncryptionResponse>> Encrypt([FromBody] EncryptionRequest request)
        {
            _logger.LogInformation("Encrypt request received: Algorithm={Algorithm}", request.Algorithm);

            if (request.PlainText?.Length > MaxTextLength)
            {
                _logger.LogWarning("Encrypt request rejected: message too long (>{Length})", MaxTextLength);
                return BadRequest(new { error = $"Message exceeds maximum allowed length of {MaxTextLength} characters." });
            }

            try
            {
                var result = await _service.EncryptAsync(request.PlainText, request.Algorithm, request.Key);
                _logger.LogInformation("Encryption successful for algorithm {Algorithm}", request.Algorithm);
                return Ok(new EncryptionResponse { CipherText = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Encryption failed for algorithm {Algorithm}", request.Algorithm);
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("decrypt")]
        public async Task<ActionResult<EncryptionResponse>> Decrypt([FromBody] EncryptionRequest request)
        {
            _logger.LogInformation("Decrypt request received: Algorithm={Algorithm}", request.Algorithm);

            if (request.PlainText?.Length > MaxTextLength)
            {
                _logger.LogWarning("Decrypt request rejected: message too long (>{Length})", MaxTextLength);
                return BadRequest(new { error = $"Message exceeds maximum allowed length of {MaxTextLength} characters." });
            }

            try
            {
                var result = await _service.DecryptAsync(request.PlainText, request.Algorithm, request.Key);
                _logger.LogInformation("Decryption successful for algorithm {Algorithm}", request.Algorithm);
                return Ok(new EncryptionResponse { CipherText = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Decryption failed for algorithm {Algorithm}", request.Algorithm);
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("generate-rsa-keys")]
        public async Task<ActionResult<RsaKeyPairResponse>> GenerateRsaKeyPair()
        {
            var (publicKey, privateKey) = RsaKeyGenerator.GenerateKeyPair();

            var response = new RsaKeyPairResponse
            {
                PublicKeyXml = publicKey,
                PrivateKeyXml = privateKey
            };

            return Ok(response);
        }
    }
}
