// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace System.Diagnostics.Tracing
{
    /// <summary>
    /// 
    /// 
    /// Logger uses DiagnosticListener in its implementation, which means that you can listen to the data presented here using DiagnosticListener.AllListeners
    /// </summary>
    public class Logger : DiagnosticListener, ILogger
    {
        /// <summary>
        /// Creates a new logger with the given name.   A Logger is a DiagnosticSource which understands LogLevel (verbosity levels)
        /// </summary>
        /// <param name="name">The name of the logger.  This is often the comonent name.</param>
        /// <param name="maxLevel">The most verbose message that will ever be logged.  It defaults to Debug which, being the most
        /// verbose level, means any message will be logged.   Useful in the context of security to make it less likely that 
        /// private (debug) information does not get logged.  It also increases the efficienctly of IsEnabled if you only wish
        /// to EVER turn low-verbosity events.</param>
        public Logger(string name, LogLevel maxLevel = LogLevel.Debug) : base(name)
        {
            _maxLevel = maxLevel;  
        }

        /// <summary>
        /// Returns true if 'level' is less verbose (more important) than the current level for the Logger.  
        /// </summary>
        public virtual bool IsEnabled(LogLevel level)
        {
            if (level > _maxLevel)
                return false;
            if (_filter != null)
            {
                foreach (Func<string, LogLevel, bool> filter in _filter.GetInvocationList())
                {
                    if (filter(Name, level))
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Because calling this method typically requires a anonymous type to be generated for the 'arguments'
        /// value if this logging happens in a 'hot' code path you should have a 'IsEnabled' check before calling
        /// it to avoid having to make the object just to throw it away if logging is off.   
        /// </summary>
        public virtual void Log(string logItemName, LogLevel level, object arguments = null)
        {
            if (IsEnabled(level))
            {
                 Write(logItemName, new LoggerArguments { Level = level, LoggerName = Name, Arguments = arguments });
            }
        }

        /// <summary>
        /// If the logger is enabled for 'level' then geneate a 'activityName'.Start message with the given argument.
        /// It returns an IDisposable that when Disposed() will send a 'activityName'.Stop message with the SAME arguments 
        /// object.  Thus the arguments object itself can be used as an ID that links the two together.
        /// </summary>
        public virtual IDisposable ActivityStart(string activityName, LogLevel level = LogLevel.Critical, object arguments = null)
        {
            var activity = new LoggerActivity(this, level, activityName, arguments);
            if (IsEnabled(level))
                Log(activityName + ".Start", level, arguments);
            else
                activity.Disposed = true;
            return activity;
        }

        /// <summary>
        /// Subscribe to the Logger, sending events to 'observer' for any logging that is enabled for verbosity 'level'.  
        /// 
        /// Note that in multi-subscriber cases you MAY get messages that have a more verbose level than you asked for.
        /// You can filter them out yourself.   If there is only one subscriber (or all subscribers subscribe at the 
        /// same verbosity.  It works fine.   
        /// </summary>
        public virtual IDisposable Subscribe(IObserver<KeyValuePair<string, object>> observer, Func<string, LogLevel, bool> filter)
        {
            _filter += filter;
            return base.Subscribe(observer);
        }

        #region private
        private sealed class LoggerActivity : IDisposable
        {
            public LoggerActivity(Logger logger, LogLevel level, string activityName, object arguments)
            {
                _logger = logger;
                _level = level;
                _activityName = activityName;
                _arguments = arguments;
            }
            public bool Disposed;

            public void Dispose()
            {
                if (!Disposed)
                {
                    Disposed = true;
                    _logger.Log(_activityName + ".Stop", _level, _arguments);
                }
            }
            Logger _logger;
            LogLevel _level;
            string _activityName;
            object _arguments;
        }

        private LogLevel _maxLevel;
        private Func<string, LogLevel, bool> _filter;
        #endregion 
    }

    /// <summary>
    /// LoggerArguments is the payload that you get back from the LogStream.   The string in the KeyValue pair
    /// is the name of the logging message, and the object is a LoggerArguments.  
    /// </summary>
    public class LoggerArguments
    {
        /// <summary>
        /// This is the verbosity level that was passed to the 'Log' function 
        /// </summary>
        public LogLevel Level;
        /// <summary>
        /// This is the name of the logger that logged the event 
        /// </summary>
        public string LoggerName;
        /// <summary>
        /// This is the 'arguments parameter passed to the 'Log' method.  
        /// </summary>
        public object Arguments;
        // TODO should we add a IsActivity? (They can get that from the suffix).  
    }

    /// <summary>
    /// This is identical to System.Diagnostics.Tracing.EventLevel.  It is only here to avoid taking a dependency on that type. 
    /// They should be kept in sync. 
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// Log always. This level is not supported for subscribers. 
        /// </summary>
        LogAlways = 0,
        /// <summary>
        /// Only critical errors
        /// </summary>
        Critical,
        /// <summary>
        /// All errors, including previous levels
        /// </summary>
        Error,
        /// <summary>
        /// All warnings, including previous levels
        /// </summary>
        Warning,
        /// <summary>
        /// All informational, including previous levels
        /// </summary>
        Informational,
        /// <summary>
        /// All verbose, including previous levels 
        /// </summary>
        Verbose,  // = 5
        /// <summary>
        /// All events, including previous levels 
        /// </summary>
        Debug  // = 6
    }
}
