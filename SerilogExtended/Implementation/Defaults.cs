namespace SerilogExtended.Implementation
{
    /// <summary>
    /// Default names and constants
    /// </summary>
    public static class Defaults
    {
        /// <summary>
        /// HTTP custom header name to use Correlation Id property
        /// in diagnostics HTTP log outputs
        /// </summary>
        public const string CorrelationHeaderName = "X-Correlation-Id";

        /// <summary>
        /// Correlation Id property inner name
        /// </summary>
        public const string CorrelationIdName = "CorrelationId";
    }
}