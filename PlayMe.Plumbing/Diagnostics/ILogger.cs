namespace PlayMe.Plumbing.Diagnostics
{
    public interface ILogger
    {
            void Debug(string format, params object[] args);
            void Error(string format, params object[] args);
            void Fatal(string format, params object[] args);
            void Info(string format, params object[] args);
            void Trace(string format, params object[] args);
            void Warn(string format, params object[] args);

            bool IsDebugEnabled { get; }
            bool IsErrorEnabled { get; }
            bool IsFatalEnabled { get; }
            bool IsInfoEnabled { get; }
            bool IsTraceEnabled { get; }
            bool IsWarnEnabled { get; }
    }
}
