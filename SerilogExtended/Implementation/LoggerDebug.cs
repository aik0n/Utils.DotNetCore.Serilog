namespace SerilogExtended.Implementation
{
    /// <summary>
    /// Wrapper class to enable Serilog self log
    /// </summary>
    public static class LoggerDebug
    {
        /// <summary>
        /// This method enable Serilog self log
        /// </summary>
        /// <param name="path">A path to diagnostics log file</param>
        public static void Enable(string path)
        {
            var file = File.CreateText(path);
            Serilog.Debugging.SelfLog.Enable(TextWriter.Synchronized(file));
        }
    }
}