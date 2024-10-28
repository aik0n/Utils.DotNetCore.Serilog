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
            var info = new FileInfo(path);
            if (false == info.Directory.Exists)
            {
                info.Directory.Create();
            }

            var fileStream = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            var streamWriter = new StreamWriter(fileStream) { AutoFlush = true };
            Serilog.Debugging.SelfLog.Enable(TextWriter.Synchronized(streamWriter));
        }
    }
}