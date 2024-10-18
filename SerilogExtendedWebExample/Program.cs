using Serilog;
using SerilogExtended.Implementation;

namespace SerilogExtendedWebExample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                // add this line of code to enable serilog selflog
                // it is usefull to check serilog.json configuration error(s)
                // this line of code have to be before CreateHostBuilder method
                // use your file path from settings or configuration
                LoggerDebug.Enable("c:/w.o.r.k/serilog_web_sample_debug.txt");

                var host = CreateHostBuilder(args).Build();

                Log.Information("Starting web host");

                host.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                // Ensure proper disposal of loggers
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
                Host.CreateDefaultBuilder(args)
                    // add this to use serilog asp net package        
                    .UseSerilog()
                    .ConfigureAppConfiguration(webBuilder =>
                    {
                        webBuilder.AddJsonFile("serilog.json", optional: false, reloadOnChange: true);
                    })
                    .ConfigureWebHostDefaults(webBuilder =>
                    {
                        webBuilder.UseStartup<Startup>();
                    });
    }
}