## About
A simple library to make Serilog integration to .NET projects a bit easier.

Supports (out of a box):

1. Standard set of sinks - Console, File, Seq, MS SQL
2. One unusual PersistenceFile sink
3. A few common enrichers - FromLogContext, WithMachineName, Environment
4. Expressions - to add ability filter logs or create different files with different logs, etc.
5. ASP .NET Core logging middleware
6. Extension methods to log with context
7. Integration with .NET Core dependency injection
8. Logger setup using configuration (settings file)


## How to use

Main purpose is to use in .NET Core projects based on Host builder or WEB builder solutions.
And, in the same time can be used in .NET Core console applications with Dependency Injection.
Samples shown below.

**Also, a source code contains two samples of integration WEB and Console.**

Add to your project NuGet package ``` dotnet add package SerilogExtended --version 1.0.0 ```

### WEB application
First of all before read a serilog settings file in your implementation - add a self debug.
It can be extremely helpful to fix configuration file issues.

```csharp
    // add this line of code to enable serilog self log
    // it is useful to check serilog.json configuration error(s)
    // this line of code have to be before CreateHostBuilder method
    // use your file path from settings or configuration
    LoggerDebug.Enable("c:/w.o.r.k/serilog_web_sample_debug.txt");
```

Add ``` .UseSerilog() ``` to host configuration if you plan to use HTTP request logging middleware.
And (optional) add ``` app.UseSerilogRequestMiddleware(new LogEnricher()); ```

```csharp
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
```

Update a Startup.cs file in your application.

```csharp
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddRazorPages();
        services.AddSerilogLogging(Configuration); // Add this line
    }
```

```csharp
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // add this to use diagnostics http logs
        // log enricher implementation can be replaced with custom one
        app.UseSerilogRequestMiddleware(new LogEnricher());

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(name: "default", pattern: "{controller=Main}/{action=Index}/{id?}");
            endpoints.MapRazorPages();
        });
    }
```

The ``` LogEnricher ``` class in this sample is replaceable part. You can implement your own class that implements ``` ILogEnricher ``` 

See example in source code

### Console applications
A principle is the same. Create a service collection and add an extension method.

```csharp
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // add this line of code to enable serilog self log
            // it is useful to check serilog.json configuration error(s)
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
```

Sample how to add and call from controller:
```csharp
public class LogController : Controller
{
    private readonly ILogger<LogController> _logger;

    public LogController(ILogger<LogController> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpPost]
    public IActionResult Warning()
    {
        _logger.LogWarning("Test warning message");
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
```
And, finally create or update serilog settings file. Or add a serilog section into a appsettings file

Note, that mistakes in configuration file can stop logger to write any logs at all.

You can check this with Self debug option (described at the beginning).

Configuration samples provided with app samples in source code. A minimal one is:
```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Verbose"
    },
    "Using": [ "Serilog.Sinks.PersistentFile", "Serilog.Expressions" ],
    "WriteTo": [
      {
        "Name": "Logger",
        "Args": {
          "configureLogger": {
            "WriteTo": [
              {
                "Name": "PersistentFile",
                "Args": {
                  "path": "logs/all-events.log",
                  "outputTemplate": "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                  "persistentFileRollingInterval": "Day",
                  "rollOnFileSizeLimit": false,
                  "preserveLogFilename": true,
                  "shared": true
                }
              }
            ]
          }
        }
      }
    ],
    "Properties": {
      "Application": "Your_App_Name"
    }
  }
}
```

## Persistent File
This implementation uses a custom [Persistent File sink](https://github.com/dfacto-lab/serilog-sinks-file).

A goal is to keep latest log records in a file and keep it name. Can be helpful when other tools based on a file name. Basic serilog configuration settings for persistence file shown above.

## Log with class and method names
Exists not only one way to achieve this. And, in the same time every approach has it own props and cons. From a performance issues to complexity of using, namespaces and extension tricks.

In this library implementation created two ``` ILogger ``` extensions methods:
- LogWithContext - can write a log message with a ``` LogLevel ``` provided parameter.
- LogExceptionWithContext - will write down an exception.

A caller class name, method name and line number will be added to log output:

```
[INF] Test informational message using context (at LogController.InformationalWithContext, line 60)
```

```
Test exception message using context (at LogController.ExceptionWithContext, line 75)
System.DivideByZeroException: Attempted to divide by zero.
   at SerilogExtendedWebExample.Controllers.LogController.ExceptionWithContext()
```

## Useful link(s)
- [Serilog Wiki](https://github.com/serilog/serilog/wiki)
- [Best Practices](https://benfoster.io/blog/serilog-best-practices/)