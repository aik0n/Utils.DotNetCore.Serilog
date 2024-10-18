using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using SerilogExtended.Implementation;

namespace SerilogExtendedConsoleExample
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // add this line of code to enable serilog selflog
            // it is usefull to check serilog.json configuration error(s)
            // this line should before any methods/hosts run
            // use your file path from settings or configuration
            LoggerDebug.Enable("c:/w.o.r.k/serilog_console_sample_debug.txt");

            // Step 1: Build configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("serilog.json", optional: false, reloadOnChange: true)
                .Build();

            // Step 2: Configure DI and Serilog
            var serviceProvider = ConfigureServices(configuration);

            try
            {
                // Step 3: Resolve and run the application
                var worker = serviceProvider.GetRequiredService<WorkRunner>();
                // Your application logic
                await worker.DoJob();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application terminated unexpectedly");
            }
            finally
            {
                // Ensure proper disposal of loggers
                Log.CloseAndFlush();
            }
        }

        private static IServiceProvider ConfigureServices(IConfiguration configuration)
        {
            // Create the service collection
            var services = new ServiceCollection();

            // Step 2.1: Add Serilog from the LoggingLibrary and pass the configuration
            services.AddSerilogLogging(configuration);

            // Step 2.2: Register other services (e.g., App)
            services.AddTransient<WorkRunner>();

            // Step 2.3: Build the service provider (DI container)
            return services.BuildServiceProvider();
        }
    }
}