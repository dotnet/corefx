using System.Collections.Generic;

namespace System.Diagnostics.Tracing
{
    interface ILogger
    {
        bool IsEnabled(LogLevel level);
        void Log(string logItemName, LogLevel level, object arguments = null);
        IDisposable ActivityStart(string activityName, LogLevel level = LogLevel.Critical, object arguments = null);
        IDisposable Subscribe(IObserver<KeyValuePair<string, object>> observer, LogLevel level);
    }
}
