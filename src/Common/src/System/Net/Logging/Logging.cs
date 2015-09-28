// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;

namespace System.Net
{
    internal class Logging
    {
        private static volatile bool s_loggingEnabled = true;
        private static volatile bool s_loggingInitialized;

        private const int DefaultMaxDumpSize = 1024;
        private const bool DefaultUseProtocolTextOnly = false;

        private const string AttributeNameMaxSize = "maxdatasize";
        private const string AttributeNameTraceMode = "tracemode";
        private static readonly string[] s_supportedAttributes = new string[] { AttributeNameMaxSize, AttributeNameTraceMode };

        private const string AttributeValueProtocolOnly = "protocolonly";

        private const string TraceSourceWebName = "System.Net";
        private const string TraceSourceHttpListenerName = "System.Net.HttpListener";
        private const string TraceSourceSocketsName = "System.Net.Sockets";
        private const string TraceSourceWebSocketsName = "System.Net.WebSockets";
        private const string TraceSourceCacheName = "System.Net.Cache";
        private const string TraceSourceHttpName = "System.Net.Http";

        private static TraceSource s_webTraceSource;
        private static TraceSource s_httpListenerTraceSource;
        private static TraceSource s_socketsTraceSource;
        private static TraceSource s_webSocketsTraceSource;
        private static TraceSource s_cacheTraceSource;
        private static TraceSource s_traceSourceHttpName;

        private Logging()
        {
        }

        private static object s_internalSyncObject;
        private static object InternalSyncObject
        {
            get
            {
                if (s_internalSyncObject == null)
                {
                    object o = new Object();
                    Interlocked.CompareExchange(ref s_internalSyncObject, o, null);
                }

                return s_internalSyncObject;
            }
        }

        internal static bool On
        {
            get
            {
                if (!s_loggingInitialized)
                {
                    InitializeLogging();
                }

                return s_loggingEnabled;
            }
        }

        internal static bool IsVerbose(TraceSource traceSource)
        {
            return ValidateSettings(traceSource, TraceEventType.Verbose);
        }

        internal static TraceSource Web
        {
            get
            {
                if (!s_loggingInitialized)
                {
                    InitializeLogging();
                }

                if (!s_loggingEnabled)
                {
                    return null;
                }

                return s_webTraceSource;
            }
        }

        internal static TraceSource Http
        {
            get
            {
                if (!s_loggingInitialized)
                {
                    InitializeLogging();
                }

                if (!s_loggingEnabled)
                {
                    return null;
                }

                return s_traceSourceHttpName;
            }
        }

        internal static TraceSource HttpListener
        {
            get
            {
                if (!s_loggingInitialized)
                {
                    InitializeLogging();
                }

                if (!s_loggingEnabled)
                {
                    return null;
                }

                return s_httpListenerTraceSource;
            }
        }

        internal static TraceSource Sockets
        {
            get
            {
                if (!s_loggingInitialized)
                {
                    InitializeLogging();
                }

                if (!s_loggingEnabled)
                {
                    return null;
                }

                return s_socketsTraceSource;
            }
        }

        internal static TraceSource RequestCache
        {
            get
            {
                if (!s_loggingInitialized)
                {
                    InitializeLogging();
                }

                if (!s_loggingEnabled)
                {
                    return null;
                }

                return s_cacheTraceSource;
            }
        }

        internal static TraceSource WebSockets
        {
            get
            {
                if (!s_loggingInitialized)
                {
                    InitializeLogging();
                }

                if (!s_loggingEnabled)
                {
                    return null;
                }

                return s_webSocketsTraceSource;
            }
        }

        private static bool GetUseProtocolTextSetting(TraceSource traceSource)
        {
            return DefaultUseProtocolTextOnly;
        }

        private static int GetMaxDumpSizeSetting(TraceSource traceSource)
        {
            return DefaultMaxDumpSize;
        }

        // Sets up internal config settings for logging.
        private static void InitializeLogging()
        {
            lock (InternalSyncObject)
            {
                if (!s_loggingInitialized)
                {
                    bool loggingEnabled = false;
                    s_webTraceSource = new NclTraceSource(TraceSourceWebName);
                    s_httpListenerTraceSource = new NclTraceSource(TraceSourceHttpListenerName);
                    s_socketsTraceSource = new NclTraceSource(TraceSourceSocketsName);
                    s_webSocketsTraceSource = new NclTraceSource(TraceSourceWebSocketsName);
                    s_cacheTraceSource = new NclTraceSource(TraceSourceCacheName);
                    s_traceSourceHttpName = new NclTraceSource(TraceSourceHttpName);

                    GlobalLog.Print("Initalizating tracing");

                    try
                    {
                        loggingEnabled = (s_webTraceSource.Switch.ShouldTrace(TraceEventType.Critical) ||
                                          s_httpListenerTraceSource.Switch.ShouldTrace(TraceEventType.Critical) ||
                                          s_socketsTraceSource.Switch.ShouldTrace(TraceEventType.Critical) ||
                                          s_webSocketsTraceSource.Switch.ShouldTrace(TraceEventType.Critical) ||
                                          s_cacheTraceSource.Switch.ShouldTrace(TraceEventType.Critical) ||
                                          s_traceSourceHttpName.Switch.ShouldTrace(TraceEventType.Critical));
                    }
                    catch (SecurityException)
                    {
                        // These may throw if the caller does not have permission to hook up trace listeners.
                        // We treat this case as though logging were disabled.
                        Close();
                        loggingEnabled = false;
                    }

                    s_loggingEnabled = loggingEnabled;
                    s_loggingInitialized = true;
                }
            }
        }

        private static void Close()
        {
            if (s_webTraceSource != null)
            {
                s_webTraceSource.Close();
            }

            if (s_httpListenerTraceSource != null)
            {
                s_httpListenerTraceSource.Close();
            }

            if (s_socketsTraceSource != null)
            {
                s_socketsTraceSource.Close();
            }

            if (s_webSocketsTraceSource != null)
            {
                s_webSocketsTraceSource.Close();
            }

            if (s_cacheTraceSource != null)
            {
                s_cacheTraceSource.Close();
            }

            if (s_traceSourceHttpName != null)
            {
                s_traceSourceHttpName.Close();
            }
        }

        // Confirms logging is enabled, given current logging settings
        private static bool ValidateSettings(TraceSource traceSource, TraceEventType traceLevel)
        {
            if (!s_loggingEnabled)
            {
                return false;
            }

            if (!s_loggingInitialized)
            {
                InitializeLogging();
            }

            if (traceSource == null || !traceSource.Switch.ShouldTrace(traceLevel))
            {
                return false;
            }

            return true;
        }

        // Converts an object to a normalized string that can be printed
        // takes System.Net.ObjectNamedFoo and coverts to ObjectNamedFoo, 
        // except IPAddress, IPEndPoint, and Uri, which return ToString().
        private static string GetObjectName(object obj)
        {
            if (obj is Uri || obj is System.Net.IPAddress || obj is System.Net.IPEndPoint)
            {
                return obj.ToString();
            }
            else
            {
                return obj.GetType().Name;
            }
        }

        internal static void PrintLine(TraceSource traceSource, TraceEventType eventType, int id, string msg)
        {
            string logHeader = "[" + Environment.CurrentManagedThreadId.ToString("d4", CultureInfo.InvariantCulture) + "] ";
            traceSource.TraceEvent(eventType, id, logHeader + msg);
        }

        internal static void Associate(TraceSource traceSource, object objA, object objB)
        {
            if (!ValidateSettings(traceSource, TraceEventType.Information))
            {
                return;
            }

            string lineA = GetObjectName(objA) + "#" + Logging.HashString(objA);
            string lineB = GetObjectName(objB) + "#" + Logging.HashString(objB);

            PrintLine(traceSource, TraceEventType.Information, 0, "Associating " + lineA + " with " + lineB);
        }

        internal static void Enter(TraceSource traceSource, object obj, string method, string param)
        {
            if (!ValidateSettings(traceSource, TraceEventType.Information))
            {
                return;
            }

            Enter(traceSource, GetObjectName(obj) + "#" + Logging.HashString(obj), method, param);
        }

        internal static void Enter(TraceSource traceSource, object obj, string method, object paramObject)
        {
            if (!ValidateSettings(traceSource, TraceEventType.Information))
            {
                return;
            }

            Enter(traceSource, GetObjectName(obj) + "#" + Logging.HashString(obj), method, paramObject);
        }

        internal static void Enter(TraceSource traceSource, string obj, string method, string param)
        {
            if (!ValidateSettings(traceSource, TraceEventType.Information))
            {
                return;
            }

            Enter(traceSource, obj + "::" + method + "(" + param + ")");
        }

        internal static void Enter(TraceSource traceSource, string obj, string method, object paramObject)
        {
            if (!ValidateSettings(traceSource, TraceEventType.Information))
            {
                return;
            }

            string paramObjectValue = "";
            if (paramObject != null)
            {
                paramObjectValue = GetObjectName(paramObject) + "#" + Logging.HashString(paramObject);
            }

            Enter(traceSource, obj + "::" + method + "(" + paramObjectValue + ")");
        }

        internal static void Enter(TraceSource traceSource, string method, string parameters)
        {
            if (!ValidateSettings(traceSource, TraceEventType.Information))
            {
                return;
            }

            Enter(traceSource, method + "(" + parameters + ")");
        }

        internal static void Enter(TraceSource traceSource, string msg)
        {
            if (!ValidateSettings(traceSource, TraceEventType.Information))
            {
                return;
            }

            // Trace.CorrelationManager.StartLogicalOperation();
            PrintLine(traceSource, TraceEventType.Verbose, 0, msg);
        }

        internal static void Exit(TraceSource traceSource, object obj, string method, object retObject)
        {
            if (!ValidateSettings(traceSource, TraceEventType.Information))
            {
                return;
            }

            string retValue = "";
            if (retObject != null)
            {
                retValue = GetObjectName(retObject) + "#" + Logging.HashString(retObject);
            }

            Exit(traceSource, obj, method, retValue);
        }

        internal static void Exit(TraceSource traceSource, string obj, string method, object retObject)
        {
            if (!ValidateSettings(traceSource, TraceEventType.Information))
            {
                return;
            }

            string retValue = "";
            if (retObject != null)
            {
                retValue = GetObjectName(retObject) + "#" + Logging.HashString(retObject);
            }

            Exit(traceSource, obj, method, retValue);
        }

        internal static void Exit(TraceSource traceSource, object obj, string method, string retValue)
        {
            if (!ValidateSettings(traceSource, TraceEventType.Information))
            {
                return;
            }

            Exit(traceSource, GetObjectName(obj) + "#" + Logging.HashString(obj), method, retValue);
        }

        internal static void Exit(TraceSource traceSource, string obj, string method, string retValue)
        {
            if (!ValidateSettings(traceSource, TraceEventType.Information))
            {
                return;
            }

            if (!string.IsNullOrEmpty(retValue))
            {
                retValue = "\t-> " + retValue;
            }

            Exit(traceSource, obj + "::" + method + "() " + retValue);
        }

        internal static void Exit(TraceSource traceSource, string method, string parameters)
        {
            if (!ValidateSettings(traceSource, TraceEventType.Information))
            {
                return;
            }

            Exit(traceSource, method + "() " + parameters);
        }

        internal static void Exit(TraceSource traceSource, string msg)
        {
            if (!ValidateSettings(traceSource, TraceEventType.Information))
            {
                return;
            }

            PrintLine(traceSource, TraceEventType.Verbose, 0, "Exiting " + msg);
            // Trace.CorrelationManager.StopLogicalOperation();
        }

        internal static void Exception(TraceSource traceSource, object obj, string method, Exception e)
        {
            if (!ValidateSettings(traceSource, TraceEventType.Error))
            {
                return;
            }

            string infoLine = SR.Format(SR.net_log_exception, GetObjectLogHash(obj), method, e.Message);
            if (!string.IsNullOrEmpty(e.StackTrace))
            {
                infoLine += Environment.NewLine + e.StackTrace;
            }

            PrintLine(traceSource, TraceEventType.Error, 0, infoLine);
        }

        internal static void PrintInfo(TraceSource traceSource, string msg)
        {
            if (!ValidateSettings(traceSource, TraceEventType.Information))
            {
                return;
            }

            PrintLine(traceSource, TraceEventType.Information, 0, msg);
        }

        internal static void PrintInfo(TraceSource traceSource, object obj, string msg)
        {
            if (!ValidateSettings(traceSource, TraceEventType.Information))
            {
                return;
            }

            PrintLine(traceSource, TraceEventType.Information, 0,
                                   GetObjectName(obj) + "#" + Logging.HashString(obj)
                                   + " - " + msg);
        }

        internal static void PrintInfo(TraceSource traceSource, object obj, string method, string param)
        {
            if (!ValidateSettings(traceSource, TraceEventType.Information))
            {
                return;
            }

            PrintLine(traceSource, TraceEventType.Information, 0,
                                   GetObjectName(obj) + "#" + Logging.HashString(obj)
                                   + "::" + method + "(" + param + ")");
        }

        internal static void PrintWarning(TraceSource traceSource, string msg)
        {
            if (!ValidateSettings(traceSource, TraceEventType.Warning))
            {
                return;
            }

            PrintLine(traceSource, TraceEventType.Warning, 0, msg);
        }

        internal static void PrintWarning(TraceSource traceSource, object obj, string method, string msg)
        {
            if (!ValidateSettings(traceSource, TraceEventType.Warning))
            {
                return;
            }

            PrintLine(traceSource, TraceEventType.Warning, 0,
                                   GetObjectName(obj) + "#" + Logging.HashString(obj)
                                   + "::" + method + "() - " + msg);
        }

        internal static void PrintError(TraceSource traceSource, string msg)
        {
            if (!ValidateSettings(traceSource, TraceEventType.Error))
            {
                return;
            }

            PrintLine(traceSource, TraceEventType.Error, 0, msg);
        }

        internal static void PrintError(TraceSource traceSource, object obj, string method, string msg)
        {
            if (!ValidateSettings(traceSource, TraceEventType.Error))
            {
                return;
            }

            PrintLine(traceSource, TraceEventType.Error, 0,
                                   GetObjectName(obj) + "#" + Logging.HashString(obj)
                                   + "::" + method + "() - " + msg);
        }

        internal static string GetObjectLogHash(object obj)
        {
            return GetObjectName(obj) + "#" + Logging.HashString(obj);
        }

        internal static void Dump(TraceSource traceSource, object obj, string method, IntPtr bufferPtr, int length)
        {
            if (!ValidateSettings(traceSource, TraceEventType.Verbose) || bufferPtr == IntPtr.Zero || length < 0)
            {
                return;
            }

            byte[] buffer = new byte[length];
            Marshal.Copy(bufferPtr, buffer, 0, length);
            Dump(traceSource, obj, method, buffer, 0, length);
        }

        internal static void Dump(TraceSource traceSource, object obj, string method, byte[] buffer, int offset, int length)
        {
            if (!ValidateSettings(traceSource, TraceEventType.Verbose))
            {
                return;
            }

            if (buffer == null)
            {
                PrintLine(traceSource, TraceEventType.Verbose, 0, "(null)");
                return;
            }

            if (offset > buffer.Length)
            {
                PrintLine(traceSource, TraceEventType.Verbose, 0, "(offset out of range)");
                return;
            }

            PrintLine(traceSource, TraceEventType.Verbose, 0, "Data from " + GetObjectName(obj) + "#" + Logging.HashString(obj) + "::" + method);
            int maxDumpSize = GetMaxDumpSizeSetting(traceSource);
            if (length > maxDumpSize)
            {
                PrintLine(traceSource, TraceEventType.Verbose, 0, "(printing " + maxDumpSize.ToString(NumberFormatInfo.InvariantInfo) + " out of " + length.ToString(NumberFormatInfo.InvariantInfo) + ")");
                length = maxDumpSize;
            }

            if ((length < 0) || (length > buffer.Length - offset))
            {
                length = buffer.Length - offset;
            }

            do
            {
                int n = Math.Min(length, 16);
                string disp = string.Format(CultureInfo.CurrentCulture, "{0:X8} : ", offset);
                for (int i = 0; i < n; ++i)
                {
                    disp += string.Format(CultureInfo.CurrentCulture, "{0:X2}", buffer[offset + i]) + ((i == 7) ? '-' : ' ');
                }

                for (int i = n; i < 16; ++i)
                {
                    disp += "   ";
                }

                disp += ": ";
                for (int i = 0; i < n; ++i)
                {
                    disp += ((buffer[offset + i] < 0x20) || (buffer[offset + i] > 0x7e))
                                ? '.'
                                : (char)(buffer[offset + i]);
                }

                PrintLine(traceSource, TraceEventType.Verbose, 0, disp);
                offset += n;
                length -= n;
            } while (length > 0);
        }

        internal static string ObjectToString(object objectValue)
        {
            if (objectValue == null)
            {
                return "(null)";
            }
            else if (objectValue is string && ((string)objectValue).Length == 0)
            {
                return "(string.empty)";
            }
            else if (objectValue is Exception)
            {
                return ExceptionMessage(objectValue as Exception);
            }
            else if (objectValue is IntPtr)
            {
                return "0x" + ((IntPtr)objectValue).ToString("x");
            }
            else
            {
                return objectValue.ToString();
            }
        }
        internal static string HashString(object objectValue)
        {
            if (objectValue == null)
            {
                return "(null)";
            }
            else if (objectValue is string && ((string)objectValue).Length == 0)
            {
                return "(string.empty)";
            }
            else
            {
                return objectValue.GetHashCode().ToString(NumberFormatInfo.InvariantInfo);
            }
        }

        private static string ExceptionMessage(Exception exception)
        {
            if (exception == null)
            {
                return string.Empty;
            }

            if (exception.InnerException == null)
            {
                return exception.Message;
            }

            return exception.Message + " (" + ExceptionMessage(exception.InnerException) + ")";
        }

        private class NclTraceSource : TraceSource
        {
            internal NclTraceSource(string name) : base(name) { }

            protected internal string[] GetSupportedAttributes()
            {
                return Logging.s_supportedAttributes;
            }
        }
    }
}
