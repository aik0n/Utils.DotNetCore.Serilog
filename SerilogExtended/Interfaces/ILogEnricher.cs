using Microsoft.AspNetCore.Http;
using Serilog;

namespace SerilogExtended.Interfaces
{
    /// <summary>
    /// Interface to mark HTTP enricher implementations
    /// </summary>
    public interface ILogEnricher
    {
        void EnrichFromRequest(IDiagnosticContext diagnosticContext, HttpContext httpContext);
    }
}