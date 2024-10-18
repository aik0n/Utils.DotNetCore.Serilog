using Microsoft.Extensions.Logging;
using Utils.DotNetCore.ILogger.Extensions;

namespace SerilogExtendedConsoleExample
{
    public class WorkRunner
    {
        private readonly ILogger<WorkRunner> _logger;

        public WorkRunner(ILogger<WorkRunner> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task DoJob()
        {
            _logger.LogInformation("Application is running...");

            await Task.Delay(TimeSpan.FromMilliseconds(2000));

            _logger.LogError("Test error message");

            _logger.LogWithContext(LogLevel.Information, "Message with context");

            _logger.LogInformation("Application has finished.");
        }
    }
}