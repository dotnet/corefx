using System.Collections.Generic;

namespace System.Diagnostics.Tracing
{
    /// <summary>
    /// A generic interface for logging.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Checks if the given LogLevel is enabled.
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        bool IsEnabled(LogLevel level);

        /// <summary>
        /// Log a single item
        /// </summary>
        /// <param name="logItemName"></param>
        /// <param name="level"></param>
        /// <param name="arguments"></param>
        void Log(string logItemName, LogLevel level, object arguments = null);

        /// <summary>
        /// Start of new Activity
        /// </summary>
        /// <param name="activityName"></param>
        /// <param name="level"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        IDisposable ActivityStart(string activityName, LogLevel level = LogLevel.Critical, object arguments = null);
    }
}
