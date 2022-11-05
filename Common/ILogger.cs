using System;

namespace Common
{
    public enum LogLevel
    {
        Debug,
        Info,
        Warn,
        Error
    }

    public interface ILogger
    {
        void Log(LogLevel logLevel, string message);
        void Log(LogLevel logLevel, string message, Exception ex);
    }
}
