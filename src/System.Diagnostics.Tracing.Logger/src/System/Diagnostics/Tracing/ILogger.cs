using System.Collections.Generic;

namespace System.Diagnostics.Tracing
{
    /// <summary>
    /// A interface for logging.  It is supported by System.Diagnostics.Tracing.Logger, but it could in theory have
    /// other implementations.   See Logger for more details on expected use.  
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// returns true if the given verbosity level is active (will be logged if 'Log' is called).  
        /// </summary>
        bool IsEnabled(LogLevel level);

        /// <summary>
        /// Log a single message.   A message has a name, a Verbosity level and a object that contains any other
        /// information needed by the message.   Typical this object is created using an anonymous type.  
        /// </summary>

        void Log(string messageName, LogLevel level, object arguments = null);

        /// <summary>
        /// Indicats the begining of a new Activity.  Formally an activity is simply something that has a 
        /// begining (start) and end (stop).   Like log messages Activities have names log levels and an 
        /// argument object, however it returns an IDisposable, that is indended to be used in a 'using' 
        /// clause that establishes the scope for the activity.   When the returns IDisposable is disposed
        /// the 'Stop' message is sent.  
        /// 
        /// Typically the intent of an activity is to form this scope, so that information either logged
        /// as part of the start, stop, (or any message in between), can be assciated with the activity
        /// (and thus with each other).    It is expected that Activities nest, and thus form stacks. 
        /// Typically activities that do not propery nest are treated as errors.   
        /// </summary>
        IDisposable ActivityStart(string activityName, LogLevel level = LogLevel.Critical, object arguments = null);
    }
}
