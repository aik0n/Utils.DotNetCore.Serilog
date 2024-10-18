using AspNetCore.Serilog.RequestLoggingMiddleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using SerilogExtended.Interfaces;

namespace SerilogExtended.Implementation
{
    /// <summary>
    /// Extension to use custom Serilog implementation
    /// </summary>
    public static class LoggerExtensions
    {
        /// <summary>
        /// Add an extended Serilog to a service collection
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddSerilogLogging(this IServiceCollection services, IConfiguration configuration)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            services.AddSingleton(Log.Logger);
            services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));

            return services;
        }

        /// <summary>
        /// Enable HTTP log with informational form diagnostics context
        /// </summary>
        /// <param name="app"></param>
        /// <param name="enricher"></param>
        public static void UseSerilogRequestMiddleware(this IApplicationBuilder app, ILogEnricher enricher)
        {
            if (null != enricher)
            {
                app.UseSerilogRequestLogging(options => options.EnrichDiagnosticContext = enricher.EnrichFromRequest);
            }

            app.UseMiddleware<RequestContextLoggingMiddleware>();
        }
    }
}