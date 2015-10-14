// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;

namespace System.Diagnostics.Tracing
{
    /// <summary>
    /// Logger is meant for diagnostic logging.     It is meant to be reasonably familiar to both users of other
    /// common logging APIs (e.g. NLOG, Log4Net etc), and act at as a front-end facade for them.   In particular 
    /// it has the concept of a LogLevel (verbosity) and Logger names (thus you can have a logger per component).   
    /// 
    /// System.Diagnostics.Tracing.Logger serves much the same purpose as System.Diagnostics.Tracing.EventSource.   
    /// In general the guidance is to use EventSource if you have the choice.   There are two main reasons why
    /// you may wish to use Logger rather than EventSource 
    /// 
    ///   1) You need multi-tenancy.   There is a single 'hub' in a given appdomain/process for all EventSources 
    ///      in that appdomain/process.   Thus there can only be one EventSource call 'MyProgram'.   This is a
    ///      problem if like ASP.NET, which wishes to support many independent tenants in the same appdomain.  
    ///      Because the events form one tenant will mix with the others.  
    ///   2) Dependency Injection - Logger supports ILogger, which allows you to build your application so that
    ///      the exact implemenation of the logging system can be passed into it.   Logger has support for this.
    /// 
    /// If you don't have these requirements you should seriously consider using EventSource instead.   However
    /// if you are using ASP.NET 5, they support both (1) and (2) above, and it is best to stick with Logger.  
    /// 
    /// Logger leverages DiagnosticListener as its 'plumbing'.   Thus Every Logger is a DiagnosticListener which  
    /// means that you can discover them using DiagnosticListener.AllListeners.   Once you have the Logger you want, 
    /// you can use the Subscribe() method to connect to the stream of data.  
    /// 
    /// System.Diagnostics.Tracing.EventSource will have plumbing to treat all DiagnosticSource (and thus all 
    /// Loggers), as EventSources (as long as the payloads are serializable).   Thus it is also possible access
    /// the data stream of Loggers from an EventListener and therefore ETW and LTTng.  
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
        /// Returns true if 'level' is less verbose (more important) than the current level for the Logger.  This
        /// should be used on all hot code paths before 'Log' is called so that even argument setup is skipped if
        /// logging is off.   
        /// </summary>
        public virtual bool IsEnabled(LogLevel level)
        {
            if (level > _maxLevel)
                return false;
            foreach(var filter in _filters)
            {
                if (filter(level))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Log a message.   A message has a name, level (verbosity) and optional data (arguments).  The arguments 
        /// is typically a anonymous type with as many fields as needed by the message.   The expectation is that
        /// you can process the result programatically (by subscribing to it), but you can also render the message
        /// as a string of the from  MessageName Level=level FieldName1=Value1  FieldName2=Value2 ...
        /// 
        /// Because calling this method typically requires a anonymous type to be generated for the 'arguments'
        /// value if this logging happens in a 'hot' code path you should have a 'IsEnabled' check before calling
        /// it to avoid having to make the object just to throw it away if logging is off.   
        /// </summary>
        /// <param name="messageName">The name for this logging message.  (e.g. ExceptionOccured, or DataItemRetrieved)</param>
        /// <param name="level">The verbosity of this message.</param>
        /// <param name="arguments">Any additional data.  Typically this is an anoymous type object.</param>
        public virtual void Log(string messageName, LogLevel level, object arguments = null)
        {
            if (IsEnabled(level))
            {
                 Write(messageName, new LoggerArguments { Level = level, LoggerName = Name, Arguments = arguments });
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
        public virtual IDisposable Subscribe(IObserver<KeyValuePair<string, object>> observer, Predicate<LogLevel> filter)
        {
            _filters = _filters.Concat(new[] { filter }).ToArray();
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
        private Predicate<LogLevel>[] _filters = new Predicate<LogLevel>[0];
        private ILogger[] _loggers = new ILogger[0];
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
