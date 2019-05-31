// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading;
using System.IO;
using System.Collections;
using System.Reflection;

namespace System.Diagnostics
{
    internal static class TraceInternal
    {
        private class TraceProvider : DebugProvider
        {
            public override void Fail(string message, string detailMessage) { TraceInternal.Fail(message, detailMessage); }
            public override void OnIndentLevelChanged(int indentLevel)
            {
                lock (TraceInternal.critSec)
                {
                    foreach (TraceListener listener in Listeners)
                    {
                        listener.IndentLevel = indentLevel;
                    }
                }
            }

            public override void OnIndentSizeChanged(int indentSize)
            {
                lock (TraceInternal.critSec)
                {
                    foreach (TraceListener listener in Listeners)
                    {
                        listener.IndentSize = indentSize;
                    }
                }
            }
            public override void Write(string message) { TraceInternal.Write(message); }
            public override void WriteLine(string message) { TraceInternal.WriteLine(message); }
        }

        private static volatile string s_appName = null;
        private static volatile TraceListenerCollection s_listeners;
        private static volatile bool s_autoFlush;
        private static volatile bool s_useGlobalLock;
        private static volatile bool s_settingsInitialized;


        // this is internal so TraceSource can use it.  We want to lock on the same object because both TraceInternal and 
        // TraceSource could be writing to the same listeners at the same time. 
        internal static readonly object critSec = new object();

        public static TraceListenerCollection Listeners
        {
            get
            {
                InitializeSettings();
                if (s_listeners == null)
                {
                    lock (critSec)
                    {
                        if (s_listeners == null)
                        {
                            // This is where we override default DebugProvider because we know
                            // for sure that we have some Listeners to write to.
                            Debug.SetProvider(new TraceProvider());
                            // In the absence of config support, the listeners by default add
                            // DefaultTraceListener to the listener collection.
                            s_listeners = new TraceListenerCollection();
                            TraceListener defaultListener = new DefaultTraceListener();
                            defaultListener.IndentLevel = Debug.IndentLevel;
                            defaultListener.IndentSize = Debug.IndentSize;
                            s_listeners.Add(defaultListener);
                        }
                    }
                }
                return s_listeners;
            }
        }

        internal static string AppName
        {
            get
            {
                if (s_appName == null)
                {
                    s_appName = Assembly.GetEntryAssembly()?.GetName().Name ?? string.Empty;
                }
                return s_appName;
            }
        }

        public static bool AutoFlush
        {
            get
            {
                InitializeSettings();
                return s_autoFlush;
            }

            set
            {
                InitializeSettings();
                s_autoFlush = value;
            }
        }

        public static bool UseGlobalLock
        {
            get
            {
                InitializeSettings();
                return s_useGlobalLock;
            }

            set
            {
                InitializeSettings();
                s_useGlobalLock = value;
            }
        }

        public static int IndentLevel
        {
            get { return Debug.IndentLevel; }

            set
            {
                Debug.IndentLevel = value;
            }
        }

        public static int IndentSize
        {
            get
            {
                return Debug.IndentSize;
            }

            set
            {
                Debug.IndentSize = value;
            }
        }

        public static void Indent()
        {
             Debug.IndentLevel++;
        }

        public static void Unindent()
        {
            Debug.IndentLevel--;
        }

        public static void Flush()
        {
            if (s_listeners != null)
            {
                if (UseGlobalLock)
                {
                    lock (critSec)
                    {
                        foreach (TraceListener listener in Listeners)
                        {
                            listener.Flush();
                        }
                    }
                }
                else
                {
                    foreach (TraceListener listener in Listeners)
                    {
                        if (!listener.IsThreadSafe)
                        {
                            lock (listener)
                            {
                                listener.Flush();
                            }
                        }
                        else
                        {
                            listener.Flush();
                        }
                    }
                }
            }
        }

        public static void Close()
        {
            if (s_listeners != null)
            {
                // Use global lock
                lock (critSec)
                {
                    foreach (TraceListener listener in Listeners)
                    {
                        listener.Close();
                    }
                }
            }
        }

        public static void Assert(bool condition)
        {
            if (condition) return;
            Fail(string.Empty);
        }

        public static void Assert(bool condition, string message)
        {
            if (condition) return;
            Fail(message);
        }

        public static void Assert(bool condition, string message, string detailMessage)
        {
            if (condition) return;
            Fail(message, detailMessage);
        }

        public static void Fail(string message)
        {
            if (UseGlobalLock)
            {
                lock (critSec)
                {
                    foreach (TraceListener listener in Listeners)
                    {
                        listener.Fail(message);
                        if (AutoFlush) listener.Flush();
                    }
                }
            }
            else
            {
                foreach (TraceListener listener in Listeners)
                {
                    if (!listener.IsThreadSafe)
                    {
                        lock (listener)
                        {
                            listener.Fail(message);
                            if (AutoFlush) listener.Flush();
                        }
                    }
                    else
                    {
                        listener.Fail(message);
                        if (AutoFlush) listener.Flush();
                    }
                }
            }
        }

        public static void Fail(string message, string detailMessage)
        {
            if (UseGlobalLock)
            {
                lock (critSec)
                {
                    foreach (TraceListener listener in Listeners)
                    {
                        listener.Fail(message, detailMessage);
                        if (AutoFlush) listener.Flush();
                    }
                }
            }
            else
            {
                foreach (TraceListener listener in Listeners)
                {
                    if (!listener.IsThreadSafe)
                    {
                        lock (listener)
                        {
                            listener.Fail(message, detailMessage);
                            if (AutoFlush) listener.Flush();
                        }
                    }
                    else
                    {
                        listener.Fail(message, detailMessage);
                        if (AutoFlush) listener.Flush();
                    }
                }
            }
        }

        private static void InitializeSettings()
        {
            if (!s_settingsInitialized)
            {
                // we should avoid 2 threads altering the state concurrently for predictable behavior
                // though it may not be strictly necessary at present
                lock (critSec)
                {
                    if (!s_settingsInitialized)
                    {
                        s_autoFlush = DiagnosticsConfiguration.AutoFlush;
                        s_useGlobalLock = DiagnosticsConfiguration.UseGlobalLock;
                        s_settingsInitialized = true;
                    }
                }
            }
        }

        // This method refreshes all the data from the configuration file, so that updated to the configuration file are mirrored
        // in the System.Diagnostics.Trace class
        internal static void Refresh()
        {
            lock (critSec)
            {
                s_settingsInitialized = false;
                s_listeners = null;
                Debug.IndentSize = DiagnosticsConfiguration.IndentSize;
            }
            InitializeSettings();
        }

        public static void TraceEvent(TraceEventType eventType, int id, string format, params object[] args)
        {
            TraceEventCache EventCache = new TraceEventCache();

            if (UseGlobalLock)
            {
                lock (critSec)
                {
                    if (args == null)
                    {
                        foreach (TraceListener listener in Listeners)
                        {
                            listener.TraceEvent(EventCache, AppName, eventType, id, format);
                            if (AutoFlush) listener.Flush();
                        }
                    }
                    else
                    {
                        foreach (TraceListener listener in Listeners)
                        {
                            listener.TraceEvent(EventCache, AppName, eventType, id, format, args);
                            if (AutoFlush) listener.Flush();
                        }
                    }
                }
            }
            else
            {
                if (args == null)
                {
                    foreach (TraceListener listener in Listeners)
                    {
                        if (!listener.IsThreadSafe)
                        {
                            lock (listener)
                            {
                                listener.TraceEvent(EventCache, AppName, eventType, id, format);
                                if (AutoFlush) listener.Flush();
                            }
                        }
                        else
                        {
                            listener.TraceEvent(EventCache, AppName, eventType, id, format);
                            if (AutoFlush) listener.Flush();
                        }
                    }
                }
                else
                {
                    foreach (TraceListener listener in Listeners)
                    {
                        if (!listener.IsThreadSafe)
                        {
                            lock (listener)
                            {
                                listener.TraceEvent(EventCache, AppName, eventType, id, format, args);
                                if (AutoFlush) listener.Flush();
                            }
                        }
                        else
                        {
                            listener.TraceEvent(EventCache, AppName, eventType, id, format, args);
                            if (AutoFlush) listener.Flush();
                        }
                    }
                }
            }
        }


        public static void Write(string message)
        {
            if (UseGlobalLock)
            {
                lock (critSec)
                {
                    foreach (TraceListener listener in Listeners)
                    {
                        listener.Write(message);
                        if (AutoFlush) listener.Flush();
                    }
                }
            }
            else
            {
                foreach (TraceListener listener in Listeners)
                {
                    if (!listener.IsThreadSafe)
                    {
                        lock (listener)
                        {
                            listener.Write(message);
                            if (AutoFlush) listener.Flush();
                        }
                    }
                    else
                    {
                        listener.Write(message);
                        if (AutoFlush) listener.Flush();
                    }
                }
            }
        }

        public static void Write(object value)
        {
            if (UseGlobalLock)
            {
                lock (critSec)
                {
                    foreach (TraceListener listener in Listeners)
                    {
                        listener.Write(value);
                        if (AutoFlush) listener.Flush();
                    }
                }
            }
            else
            {
                foreach (TraceListener listener in Listeners)
                {
                    if (!listener.IsThreadSafe)
                    {
                        lock (listener)
                        {
                            listener.Write(value);
                            if (AutoFlush) listener.Flush();
                        }
                    }
                    else
                    {
                        listener.Write(value);
                        if (AutoFlush) listener.Flush();
                    }
                }
            }
        }

        public static void Write(string message, string category)
        {
            if (UseGlobalLock)
            {
                lock (critSec)
                {
                    foreach (TraceListener listener in Listeners)
                    {
                        listener.Write(message, category);
                        if (AutoFlush) listener.Flush();
                    }
                }
            }
            else
            {
                foreach (TraceListener listener in Listeners)
                {
                    if (!listener.IsThreadSafe)
                    {
                        lock (listener)
                        {
                            listener.Write(message, category);
                            if (AutoFlush) listener.Flush();
                        }
                    }
                    else
                    {
                        listener.Write(message, category);
                        if (AutoFlush) listener.Flush();
                    }
                }
            }
        }

        public static void Write(object value, string category)
        {
            if (UseGlobalLock)
            {
                lock (critSec)
                {
                    foreach (TraceListener listener in Listeners)
                    {
                        listener.Write(value, category);
                        if (AutoFlush) listener.Flush();
                    }
                }
            }
            else
            {
                foreach (TraceListener listener in Listeners)
                {
                    if (!listener.IsThreadSafe)
                    {
                        lock (listener)
                        {
                            listener.Write(value, category);
                            if (AutoFlush) listener.Flush();
                        }
                    }
                    else
                    {
                        listener.Write(value, category);
                        if (AutoFlush) listener.Flush();
                    }
                }
            }
        }

        public static void WriteLine(string message)
        {
            if (UseGlobalLock)
            {
                lock (critSec)
                {
                    foreach (TraceListener listener in Listeners)
                    {
                        listener.WriteLine(message);
                        if (AutoFlush) listener.Flush();
                    }
                }
            }
            else
            {
                foreach (TraceListener listener in Listeners)
                {
                    if (!listener.IsThreadSafe)
                    {
                        lock (listener)
                        {
                            listener.WriteLine(message);
                            if (AutoFlush) listener.Flush();
                        }
                    }
                    else
                    {
                        listener.WriteLine(message);
                        if (AutoFlush) listener.Flush();
                    }
                }
            }
        }

        public static void WriteLine(object value)
        {
            if (UseGlobalLock)
            {
                lock (critSec)
                {
                    foreach (TraceListener listener in Listeners)
                    {
                        listener.WriteLine(value);
                        if (AutoFlush) listener.Flush();
                    }
                }
            }
            else
            {
                foreach (TraceListener listener in Listeners)
                {
                    if (!listener.IsThreadSafe)
                    {
                        lock (listener)
                        {
                            listener.WriteLine(value);
                            if (AutoFlush) listener.Flush();
                        }
                    }
                    else
                    {
                        listener.WriteLine(value);
                        if (AutoFlush) listener.Flush();
                    }
                }
            }
        }

        public static void WriteLine(string message, string category)
        {
            if (UseGlobalLock)
            {
                lock (critSec)
                {
                    foreach (TraceListener listener in Listeners)
                    {
                        listener.WriteLine(message, category);
                        if (AutoFlush) listener.Flush();
                    }
                }
            }
            else
            {
                foreach (TraceListener listener in Listeners)
                {
                    if (!listener.IsThreadSafe)
                    {
                        lock (listener)
                        {
                            listener.WriteLine(message, category);
                            if (AutoFlush) listener.Flush();
                        }
                    }
                    else
                    {
                        listener.WriteLine(message, category);
                        if (AutoFlush) listener.Flush();
                    }
                }
            }
        }

        public static void WriteLine(object value, string category)
        {
            if (UseGlobalLock)
            {
                lock (critSec)
                {
                    foreach (TraceListener listener in Listeners)
                    {
                        listener.WriteLine(value, category);
                        if (AutoFlush) listener.Flush();
                    }
                }
            }
            else
            {
                foreach (TraceListener listener in Listeners)
                {
                    if (!listener.IsThreadSafe)
                    {
                        lock (listener)
                        {
                            listener.WriteLine(value, category);
                            if (AutoFlush) listener.Flush();
                        }
                    }
                    else
                    {
                        listener.WriteLine(value, category);
                        if (AutoFlush) listener.Flush();
                    }
                }
            }
        }

        public static void WriteIf(bool condition, string message)
        {
            if (condition)
                Write(message);
        }

        public static void WriteIf(bool condition, object value)
        {
            if (condition)
                Write(value);
        }

        public static void WriteIf(bool condition, string message, string category)
        {
            if (condition)
                Write(message, category);
        }

        public static void WriteIf(bool condition, object value, string category)
        {
            if (condition)
                Write(value, category);
        }

        public static void WriteLineIf(bool condition, string message)
        {
            if (condition)
                WriteLine(message);
        }

        public static void WriteLineIf(bool condition, object value)
        {
            if (condition)
                WriteLine(value);
        }

        public static void WriteLineIf(bool condition, string message, string category)
        {
            if (condition)
                WriteLine(message, category);
        }

        public static void WriteLineIf(bool condition, object value, string category)
        {
            if (condition)
                WriteLine(value, category);
        }
    }
}
