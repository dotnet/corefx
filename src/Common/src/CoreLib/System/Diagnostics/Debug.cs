// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Do not remove this, it is needed to retain calls to these conditional methods in release builds
#define DEBUG
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace System.Diagnostics
{
    /// <summary>
    /// Provides a set of properties and methods for debugging code.
    /// </summary>
    public static partial class Debug
    {
        private static readonly object s_lock = new object();

        public static bool AutoFlush { get { return true; } set { } }

        [ThreadStatic]
        private static int s_indentLevel;
        public static int IndentLevel
        {
            get
            {
                return s_indentLevel;
            }
            set
            {
                s_indentLevel = value < 0 ? 0 : value;
            }
        }

        private static int s_indentSize = 4;
        public static int IndentSize
        {
            get
            {
                return s_indentSize;
            }
            set
            {
                s_indentSize = value < 0 ? 0 : value;
            }
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public static void Close() { }

        [System.Diagnostics.Conditional("DEBUG")]
        public static void Flush() { }

        [System.Diagnostics.Conditional("DEBUG")]
        public static void Indent()
        {
            IndentLevel++;
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public static void Unindent()
        {
            IndentLevel--;
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public static void Print(string message)
        {
            Write(message);
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public static void Print(string format, params object[] args)
        {
            Write(string.Format(null, format, args));
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public static void Assert(bool condition)
        {
            Assert(condition, string.Empty, string.Empty);
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public static void Assert(bool condition, string message)
        {
            Assert(condition, message, string.Empty);
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public static void Assert(bool condition, string message, string detailMessage)
        {
            if (!condition)
            {
                string stackTrace;
                try
                {
                    stackTrace = new StackTrace(0, true).ToString();
                }
                catch
                {
                    stackTrace = "";
                }
                WriteLine(FormatAssert(stackTrace, message, detailMessage));
                s_ShowDialog(stackTrace, message, detailMessage, "Assertion Failed");
            }
        }

        internal static void ContractFailure(bool condition, string message, string detailMessage, string failureKindMessage)
        {
            if (!condition)
            {
                string stackTrace;
                try
                {
                    stackTrace = new StackTrace(0, true).ToString();
                }
                catch
                {
                    stackTrace = "";
                }
                WriteLine(FormatAssert(stackTrace, message, detailMessage));
                s_ShowDialog(stackTrace, message, detailMessage, SR.GetResourceString(failureKindMessage));
            }
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public static void Fail(string message)
        {
            Assert(false, message, string.Empty);
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public static void Fail(string message, string detailMessage)
        {
            Assert(false, message, detailMessage);
        }

        private static string FormatAssert(string stackTrace, string message, string detailMessage)
        {
            string newLine = GetIndentString() + Environment.NewLine;
            return SR.DebugAssertBanner + newLine
                   + SR.DebugAssertShortMessage + newLine
                   + message + newLine
                   + SR.DebugAssertLongMessage + newLine
                   + detailMessage + newLine
                   + stackTrace;
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public static void Assert(bool condition, string message, string detailMessageFormat, params object[] args)
        {
            Assert(condition, message, string.Format(detailMessageFormat, args));
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public static void WriteLine(string message)
        {
            Write(message + Environment.NewLine);
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public static void Write(string message)
        {
            lock (s_lock)
            {
                if (message == null)
                {
                    s_WriteCore(string.Empty);
                    return;
                }
                if (s_needIndent)
                {
                    message = GetIndentString() + message;
                    s_needIndent = false;
                }
                s_WriteCore(message);
                if (message.EndsWith(Environment.NewLine))
                {
                    s_needIndent = true;
                }
            }
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public static void WriteLine(object value)
        {
            WriteLine(value?.ToString());
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public static void WriteLine(object value, string category)
        {
            WriteLine(value?.ToString(), category);
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public static void WriteLine(string format, params object[] args)
        {
            WriteLine(string.Format(null, format, args));
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public static void WriteLine(string message, string category)
        {
            if (category == null)
            {
                WriteLine(message);
            }
            else
            {
                WriteLine(category + ":" + message);
            }
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public static void Write(object value)
        {
            Write(value?.ToString());
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public static void Write(string message, string category)
        {
            if (category == null)
            {
                Write(message);
            }
            else
            {
                Write(category + ":" + message);
            }
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public static void Write(object value, string category)
        {
            Write(value?.ToString(), category);
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public static void WriteIf(bool condition, string message)
        {
            if (condition)
            {
                Write(message);
            }
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public static void WriteIf(bool condition, object value)
        {
            if (condition)
            {
                Write(value);
            }
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public static void WriteIf(bool condition, string message, string category)
        {
            if (condition)
            {
                Write(message, category);
            }
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public static void WriteIf(bool condition, object value, string category)
        {
            if (condition)
            {
                Write(value, category);
            }
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public static void WriteLineIf(bool condition, object value)
        {
            if (condition)
            {
                WriteLine(value);
            }
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public static void WriteLineIf(bool condition, object value, string category)
        {
            if (condition)
            {
                WriteLine(value, category);
            }
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public static void WriteLineIf(bool condition, string message)
        {
            if (condition)
            {
                WriteLine(message);
            }
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public static void WriteLineIf(bool condition, string message, string category)
        {
            if (condition)
            {
                WriteLine(message, category);
            }
        }

        private static bool s_needIndent;

        private static string s_indentString;

        private static string GetIndentString()
        {
            int indentCount = IndentSize * IndentLevel;
            if (s_indentString?.Length == indentCount)
            {
                return s_indentString;
            }
            return s_indentString = new string(' ', indentCount);
        }

        private sealed class DebugAssertException : Exception
        {
            internal DebugAssertException(string message, string detailMessage, string stackTrace) :
                base(message + Environment.NewLine + detailMessage + Environment.NewLine + stackTrace)
            {
            }
        }

        // internal and not readonly so that the tests can swap this out.
        internal static Action<string, string, string, string> s_ShowDialog = ShowDialog;

        internal static Action<string> s_WriteCore = WriteCore;
    }
}
