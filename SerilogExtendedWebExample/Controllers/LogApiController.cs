using Microsoft.AspNetCore.Mvc;

namespace SerilogExtendedWebExample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogApiController : ControllerBase
    {
        private readonly ILogger<LogApiController> _logger;

        public LogApiController(ILogger<LogApiController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost(nameof(LogInformational))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult LogInformational()
        {
            _logger.LogInformation("Test message from Log API");

            return Ok();
        }
    }
}