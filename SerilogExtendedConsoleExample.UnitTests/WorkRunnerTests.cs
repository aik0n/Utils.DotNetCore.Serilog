using Microsoft.Extensions.Logging;
using NSubstitute;

namespace SerilogExtendedConsoleExample.UnitTests
{
    public class WorkRunnerTests
    {
        private const string Message_App_Running = "Application is running...";
        private const string Message_Test_Error = "Test error message";
        private const string Message_App_Finished = "Application has finished.";
        private const string Message_With_Context = "Message with context";

        private readonly ILogger<WorkRunner> _mockLogger;
        private readonly WorkRunner _app;

        public WorkRunnerTests()
        {
            _mockLogger = Substitute.For<ILogger<WorkRunner>>();
            _app = new WorkRunner(_mockLogger);
        }

        [Fact]
        public async Task WorkRunner_ShouldWrite_AppRunning_LogMessage()
        {
            await _app.DoJob();

            _mockLogger.Received(1).Log(
                LogLevel.Information,
                Arg.Any<EventId>(),
                Arg.Is<object>(v => v.ToString().Contains(Message_App_Running)),
                Arg.Any<Exception>(),
                Arg.Any<Func<object, Exception?, string>>()
            );
        }

        [Fact]
        public async Task WorkRunner_ShouldWrite_TestError_LogMessage()
        {
            await _app.DoJob();

            _mockLogger.Received(1).Log(
                LogLevel.Error,
                Arg.Any<EventId>(),
                Arg.Is<object>(v => v.ToString().Contains(Message_Test_Error)),
                Arg.Any<Exception>(),
                Arg.Any<Func<object, Exception?, string>>()
            );

        }

        [Fact]
        public async Task WorkRunner_ShouldWrite_AppFinished_LogMessage()
        {
            await _app.DoJob();

            _mockLogger.Received(1).Log(
                LogLevel.Information,
                Arg.Any<EventId>(),
                Arg.Is<object>(v => v.ToString().Contains(Message_App_Finished)),
                Arg.Any<Exception>(),
                Arg.Any<Func<object, Exception?, string>>()
            );
        }

        [Fact]
        public async Task WorkRunner_ShouldWriteLogs_InProperOrder()
        {
            await _app.DoJob();

            Received.InOrder(() =>
            {
                _mockLogger.Log(
                    LogLevel.Information,
                    Arg.Any<EventId>(),
                    Arg.Is<object>(v => v.ToString().Contains(Message_App_Running)),
                    Arg.Any<Exception>(),
                    Arg.Any<Func<object, Exception?, string>>()
                );

                _mockLogger.Log(
                    LogLevel.Error,
                    Arg.Any<EventId>(),
                    Arg.Is<object>(v => v.ToString().Contains(Message_Test_Error)),
                    Arg.Any<Exception>(),
                    Arg.Any<Func<object, Exception?, string>>()
                );

                _mockLogger.Log(
                    LogLevel.Information,
                    Arg.Any<EventId>(),
                    Arg.Is<object>(v => v.ToString().Contains(Message_App_Finished)),
                    Arg.Any<Exception>(),
                    Arg.Any<Func<object, Exception?, string>>()
                );
            });
        }

        [Fact]
        public async Task WorkRunner_ShouldWriteMessages_WhenCalledMultiple()
        {
            await _app.DoJob();
            await _app.DoJob();

            _mockLogger.Received(2).Log(
                LogLevel.Information,
                Arg.Any<EventId>(),
                Arg.Is<object>(v => v.ToString().Contains(Message_App_Running)),
                Arg.Any<Exception>(),
                Arg.Any<Func<object, Exception?, string>>()
            );

            _mockLogger.Received(2).Log(
                LogLevel.Error,
                Arg.Any<EventId>(),
                Arg.Is<object>(v => v.ToString().Contains(Message_Test_Error)),
                Arg.Any<Exception>(),
                Arg.Any<Func<object, Exception?, string>>()
            );

            _mockLogger.Received(2).Log(
                LogLevel.Information,
                Arg.Any<EventId>(),
                Arg.Is<object>(v => v.ToString().Contains(Message_App_Finished)),
                Arg.Any<Exception>(),
                Arg.Any<Func<object, Exception?, string>>()
            );
        }

        [Fact]
        public async Task WorkRunner_MessageWithContext_ShouldWriteLogMessage()
        {
            await _app.DoJob();

            _mockLogger.Received(1).Log(
                LogLevel.Information,
                Arg.Any<EventId>(),
                Arg.Is<object>(v => v.ToString().Contains(Message_With_Context)),

                Arg.Any<Exception>(),
                Arg.Any<Func<object, Exception?, string>>()
            );

            _mockLogger.Received(1).Log(
                LogLevel.Information,
                Arg.Any<EventId>(),
                Arg.Is<object>(v => v.ToString().Contains("line")),
                Arg.Any<Exception>(),
                Arg.Any<Func<object, Exception?, string>>()
            );

            _mockLogger.Received(1).Log(
                LogLevel.Information,
                Arg.Any<EventId>(),
                Arg.Is<object>(v => v.ToString().Contains(nameof(WorkRunner))),
                Arg.Any<Exception>(),
                Arg.Any<Func<object, Exception?, string>>()
            );

            _mockLogger.Received(1).Log(
                LogLevel.Information,
                Arg.Any<EventId>(),
                Arg.Is<object>(v => v.ToString().Contains(nameof(WorkRunner.DoJob))),
                Arg.Any<Exception>(),
                Arg.Any<Func<object, Exception?, string>>()
            );
        }
    }
}