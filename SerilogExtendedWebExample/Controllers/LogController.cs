using Microsoft.AspNetCore.Mvc;
using Utils.DotNetCore.ILogger.Extensions;

namespace SerilogExtendedWebExample.Controllers
{
    public class LogController : Controller
    {
        private readonly ILogger<LogController> _logger;

        public LogController(ILogger<LogController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost]
        public IActionResult Verbose()
        {
            _logger.LogTrace("Test verbose message");
            return Ok();
        }

        [HttpPost]
        public IActionResult Debug()
        {
            _logger.LogDebug("Test debug message");
            return Ok();
        }

        [HttpPost]
        public IActionResult Informational()
        {
            _logger.LogInformation("Test informational message");
            return Ok();
        }

        [HttpPost]
        public IActionResult Warning()
        {
            _logger.LogWarning("Test warning message");
            return Ok();
        }

        [HttpPost]
        public IActionResult Error()
        {
            _logger.LogError("Test error message");
            return Ok();
        }

        [HttpPost]
        public IActionResult Fatal()
        {
            _logger.LogCritical("Test critical message");
            return Ok();
        }

        [HttpPost]
        public IActionResult InformationalWithContext()
        {
            _logger.LogWithContext(LogLevel.Information, "Test informational message using context");
            return Ok();
        }

        [HttpPost]
        public IActionResult ExceptionWithContext()
        {
            var divider = 0;

            try
            {
                var result = 10 / divider;
            }
            catch (Exception ex)
            {
                _logger.LogExceptionWithContext("Test exception message using context", ex);
            }

            return Ok();
        }
    }
}