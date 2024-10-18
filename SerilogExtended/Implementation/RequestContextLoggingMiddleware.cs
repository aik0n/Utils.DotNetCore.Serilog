using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Serilog.Context;

namespace SerilogExtended.Implementation
{
    /// <summary>
    /// Add a Correlation Id parameter to Log Context
    /// </summary>
    public class RequestContextLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestContextLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext context)
        {
            string correlationId = GetCorrelationId(context);

            using (LogContext.PushProperty(Defaults.CorrelationIdName, correlationId))
            {
                return _next.Invoke(context);
            }
        }

        private static string GetCorrelationId(HttpContext context)
        {
            context.Request.Headers.TryGetValue(Defaults.CorrelationHeaderName, out StringValues correlationId);

            return correlationId.FirstOrDefault() ?? context.TraceIdentifier;
        }
    }
}