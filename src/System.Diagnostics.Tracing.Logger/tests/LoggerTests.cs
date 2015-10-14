// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Xunit;
using kvp = System.Collections.Generic.KeyValuePair<string, object>;

namespace System.Diagnostics.Tracing.Tests
{
    /// <summary>
    /// Tests for TelemetrySource and TelemetryListener
    /// </summary>
    public class LoggerTest
    {
        /// <summary>
        /// Trivial example of passing an integer
        /// </summary>
        [Fact]
        public void IntPayload()
        {
            Logger logger = new Logger("Component1");

            List<kvp> result = new List<kvp>();
            IObserver<kvp> listListener = new ObserverToList<kvp>(result);
            logger.Subscribe(listListener, (name, level) => level <= LogLevel.Verbose);

            logger.Log("IntPayLoad", LogLevel.Verbose, 5);

            Assert.Equal(result.Count, 1);
            Assert.Equal(result[0].Key, "IntPayLoad");

            LoggerArguments loggerArgs = result[0].Value as LoggerArguments;
            Assert.Equal(loggerArgs.LoggerName, "Component1");
            Assert.Equal(loggerArgs.Level, LogLevel.Verbose);
            Assert.Equal(loggerArgs.Arguments, 5);
        }

        /// <summary>
        /// slightly less trivial of passing a structure with a couple of fields 
        /// </summary>
        [Fact]
        public void StructPayload()
        {
            Logger logger = new Logger("Component1");
            List<kvp> result = new List<kvp>();
            IObserver<kvp> listListener = new ObserverToList<kvp>(result);
            logger.Subscribe(listListener, (name, level) => level <= LogLevel.Verbose);

            logger.Log("StructPayLoad", LogLevel.Verbose, new Payload() { Name = "Hi", Id = 67 });

            Assert.Equal(result.Count, 1);
            Assert.Equal(result[0].Key, "StructPayLoad");
            LoggerArguments loggerArgs = result[0].Value as LoggerArguments;
            Assert.Equal(loggerArgs.LoggerName, "Component1");
            Assert.Equal(loggerArgs.Level, LogLevel.Verbose);
            var payload = loggerArgs.Arguments as Payload;
            Assert.Equal(payload.Name, "Hi");
            Assert.Equal(payload.Id, 67);
        }

        /// <summary>
        /// Simple tests for the IsEnabled method.
        /// </summary>
        [Fact]
        public void IsEnabled()
        {
            Logger logger = new Logger("Component1");
            Assert.Equal(logger.IsEnabled(LogLevel.Verbose), false);
            Assert.Equal(logger.IsEnabled(LogLevel.Critical), false);

            IObserver<kvp> listener1 = MakeObserver<kvp>();
            logger.Subscribe(listener1, (name, level) => level <= LogLevel.Critical);
            Assert.Equal(logger.IsEnabled(LogLevel.Verbose), false);
            Assert.Equal(logger.IsEnabled(LogLevel.Critical), true);

            IObserver<kvp> listener2 = MakeObserver<kvp>();
            logger.Subscribe(listener2, (name, level) => level <= LogLevel.Verbose);
            Assert.Equal(logger.IsEnabled(LogLevel.Verbose), true);
            Assert.Equal(logger.IsEnabled(LogLevel.Critical), true);
        }

        /// <summary>
        /// Ensure that logLevel is honored when logging messages
        /// </summary>
        [Fact]
        public void LogLevelTest()
        {
            Logger logger = new Logger("Component1");

            List<kvp> result = new List<kvp>();
            IObserver<kvp> listListener = new ObserverToList<kvp>(result);
            logger.Subscribe(listListener, (name, level) => level <= LogLevel.Critical);

            logger.Log("IntPayLoad", LogLevel.Verbose, 5);
            logger.Log("IntPayLoad", LogLevel.Critical, 6);
            logger.Log("IntPayLoad", LogLevel.Warning, 7);
            logger.Log("IntPayLoad", LogLevel.Error, 8);

            Assert.Equal(result.Count, 1);
            Assert.Equal(result[0].Key, "IntPayLoad");

            LoggerArguments loggerArgs = result[0].Value as LoggerArguments;
            Assert.Equal(loggerArgs.LoggerName, "Component1");
            Assert.Equal(loggerArgs.Level, LogLevel.Critical);
            Assert.Equal(loggerArgs.Arguments, 6);
        }

        /// <summary>
        /// Ensure that arguments are received by subscribers
        /// </summary>
        [Fact]
        public void SubclassLoggerArguments()
        {
            Logger logger = new Logger("Component1");

            List<kvp> result = new List<kvp>();
            IObserver<kvp> listListener = new ObserverToList<kvp>(result);
            logger.Subscribe(listListener, (name, level) => level <= LogLevel.Informational);

            logger.LogFormat("FormattedIntPayLoad", LogLevel.Informational, "payload value = %d", 5);
            Assert.Equal(result.Count, 1);
            Assert.Equal(result[0].Key, "FormattedIntPayLoad");

            LoggerArgumentsWithFormat loggerArgs = result[0].Value as LoggerArgumentsWithFormat;
            Assert.Equal(loggerArgs.LoggerName, "Component1");
            Assert.Equal(loggerArgs.Level, LogLevel.Informational);
            Assert.Equal(loggerArgs.Format, "payload value = %d");
            Assert.Equal(loggerArgs.Arguments, 5);
        }

        /// <summary>
        /// Basic test for activity
        /// </summary>
        [Fact]
        public void Activity()
        {
            Logger logger = new Logger("Component1");

            List<kvp> result = new List<kvp>();
            IObserver<kvp> listListener = new ObserverToList<kvp>(result);
            logger.Subscribe(listListener, (name, level) => level <= LogLevel.Verbose);

            using (var i = logger.ActivityStart("Main", LogLevel.Error, 4))
            {
                logger.Log("IntPayLoad", LogLevel.Verbose, 5);
            }
            Assert.Equal(result.Count, 3);

            Assert.Equal(result[0].Key, "Main.Start");
            LoggerArguments loggerArgs = result[0].Value as LoggerArguments;
            Assert.Equal(loggerArgs.LoggerName, "Component1");
            Assert.Equal(loggerArgs.Level, LogLevel.Error);
            Assert.Equal(loggerArgs.Arguments, 4);

            Assert.Equal(result[1].Key, "IntPayLoad");
            loggerArgs = result[1].Value as LoggerArguments;
            Assert.Equal(loggerArgs.LoggerName, "Component1");
            Assert.Equal(loggerArgs.Level, LogLevel.Verbose);
            Assert.Equal(loggerArgs.Arguments, 5);

            Assert.Equal(result[2].Key, "Main.Stop");
            loggerArgs = result[2].Value as LoggerArguments;
            Assert.Equal(loggerArgs.LoggerName, "Component1");
            Assert.Equal(loggerArgs.Level, LogLevel.Error);
            Assert.Equal(loggerArgs.Arguments, 4);

        }

        /// <summary>
        /// Test if it works when you have two subscribers active simultaneously
        /// </summary>
        [Fact]
        public void MultiSubscriber()
        {
            Logger logger = new Logger("Component1");

            List<kvp> result1 = new List<kvp>();
            IObserver<kvp> listListener1 = new ObserverToList<kvp>(result1);

            List<kvp> result2 = new List<kvp>();
            IObserver<kvp> listListener2 = new ObserverToList<kvp>(result2);

            logger.Subscribe(listListener1, (name, level) => level <= LogLevel.Verbose);
            logger.Subscribe(listListener2, (name, level) => level <= LogLevel.Verbose);

            logger.Log("IntPayLoad", LogLevel.Verbose, 5);

            Assert.Equal(result1.Count, 1);
            Assert.Equal(result1[0].Key, "IntPayLoad");
            LoggerArguments loggerArgs = result1[0].Value as LoggerArguments;
            Assert.Equal(loggerArgs.LoggerName, "Component1");
            Assert.Equal(loggerArgs.Level, LogLevel.Verbose);
            Assert.Equal(loggerArgs.Arguments, 5);

            Assert.Equal(result2.Count, 1);
            Assert.Equal(result2[0].Key, "IntPayLoad");
            loggerArgs = result2[0].Value as LoggerArguments;
            Assert.Equal(loggerArgs.LoggerName, "Component1");
            Assert.Equal(loggerArgs.Level, LogLevel.Verbose);
            Assert.Equal(loggerArgs.Arguments, 5);
        }

        /// <summary>
        /// Loglevel test in case of multisubscriber
        /// </summary>
        [Fact]
        public void MultiSubscriberLogLevel()
        {
            Logger logger = new Logger("Component1");

            List<kvp> result1 = new List<kvp>();
            IObserver<kvp> listListener1 = new ObserverToList<kvp>(result1);

            List<kvp> result2 = new List<kvp>();
            IObserver<kvp> listListener2 = new ObserverToList<kvp>(result2);

            logger.Subscribe(listListener1, (name, level) => level <= LogLevel.Critical);
            logger.Log("IntPayLoad", LogLevel.Error, 5);
            Assert.Equal(result1.Count, 0);

            // This causes effective log level of all subsribers to be Error
            logger.Subscribe(listListener2, (name, level) => level <= LogLevel.Error);
            logger.Log("IntPayLoad", LogLevel.Error, 5);

            // Both subscribers receive log information
            Assert.Equal(result1.Count, 1);
            Assert.Equal(result2.Count, 1);
        }

        /// <summary>
        /// Test if it works when you have two loggers
        /// </summary>
        [Fact]
        public void MultiLogger()
        {
            Logger logger1 = new Logger("Component1");
            Logger logger2 = new Logger("Component2");

            List<kvp> result = new List<kvp>();
            IObserver<kvp> listListener = new ObserverToList<kvp>(result);
            logger1.Subscribe(listListener, (name, level) => level <= LogLevel.Critical);
            logger2.Subscribe(listListener, (name, level) => level <= LogLevel.Error);

            logger1.Log("IntPayLoad", LogLevel.Critical, 5);
            logger2.Log("IntPayLoad", LogLevel.Critical, 6);
            logger1.Log("IntPayLoad", LogLevel.Error, 7);

            Assert.Equal(result.Count, 2);
            LoggerArguments loggerArgs = result[0].Value as LoggerArguments;
            Assert.Equal(loggerArgs.LoggerName, "Component1");

            loggerArgs = result[1].Value as LoggerArguments;
            Assert.Equal(loggerArgs.LoggerName, "Component2");
        }

        #region Helpers 
        /// <summary>
        /// Used to make an observer out of a action delegate. 
        /// </summary>
        private static IObserver<T> MakeObserver<T>(
            Action<T> onNext = null, Action onCompleted = null)
        {
            return new Observer<T>(onNext, onCompleted);
        }

        /// <summary>
        /// Used in the implementation of MakeObserver.  
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private class Observer<T> : IObserver<T>
        {
            public Observer(Action<T> onNext, Action onCompleted)
            {
                _onNext = onNext ?? new Action<T>(_ => { });
                _onCompleted = onCompleted ?? new Action(() => { });
            }

            public void OnCompleted() { _onCompleted(); }
            public void OnError(Exception error) { }
            public void OnNext(T value) { _onNext(value); }

            private Action<T> _onNext;
            private Action _onCompleted;
        }
        #endregion 
    }

    // Takes an IObserver and returns a List<T> that are the elements observed.
    // Will assert on error and 'Completed' is set if the 'OnCompleted' callback
    // is issued.  
    internal class ObserverToList<T> : IObserver<T>
    {
        public ObserverToList(List<T> output, Predicate<T> filter = null, string name = null)
        {
            _output = output;
            _output.Clear();
            _filter = filter;
            _name = name;
        }

        public bool Completed { get; private set; }

        #region private

        public void OnCompleted()
        {
            Completed = true;
        }

        public void OnError(Exception error)
        {
            // No errors should be thrown.
            throw new ShouldNotBeInvokedException();
        }

        public void OnNext(T value)
        {
            Assert.False(Completed);
            if (_filter == null || _filter(value))
                _output.Add(value);
        }

        private List<T> _output;
        private Predicate<T> _filter;
        private string _name;  // for debugging 
        #endregion
    }

    /// <summary>
    /// Trivial class used for payloads.  (Usually anonymous types are used.  
    /// </summary>
    internal class Payload
    {
        public string Name { get; set; }
        public int Id { get; set; }
    }

    public static class LoggerExtensions
    {
        public static void LogFormat(this Logger logger, string logItemName, LogLevel level, string format, object arguments = null)
        {
            if (logger.IsEnabled(level))
            {
                logger.Write(logItemName, new LoggerArgumentsWithFormat { Level = level, LoggerName = logger.Name, Format = format, Arguments = arguments });
            }
        }
    }

    internal class LoggerArgumentsWithFormat : LoggerArguments
    {
        public string Format { get; set; }
    }
}
