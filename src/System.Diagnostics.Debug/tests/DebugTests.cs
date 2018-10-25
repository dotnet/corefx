// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using Xunit;

namespace System.Diagnostics.Tests
{
    public abstract class DebugTests
    {
        protected abstract bool DebugUsesTraceListeners { get; }

        public DebugTests()
        {
            if (DebugUsesTraceListeners)
            {
                // Triggers code to wire up TraceListeners with Debug
                Assert.Equal(1, Trace.Listeners.Count);
            }
        }

        protected void VerifyLogged(Action test, string expectedOutput)
        {
            FieldInfo writeCoreHook = typeof(DebugProvider).GetField("s_WriteCore", BindingFlags.Static | BindingFlags.NonPublic);

            // First use our test logger to verify the output
            var originalWriteCoreHook = writeCoreHook.GetValue(null);
            writeCoreHook.SetValue(null, new Action<string>(WriteLogger.s_instance.WriteCore));

            try
            {
                WriteLogger.s_instance.Clear();
                test();
#if DEBUG
                Assert.Equal(expectedOutput, WriteLogger.s_instance.LoggedOutput);
#else
                Assert.Equal(string.Empty, WriteLogger.s_instance.LoggedOutput);
#endif
            }
            finally
            {
                writeCoreHook.SetValue(null, originalWriteCoreHook);
            }

            // Then also use the actual logger for this platform, just to verify
            // that nothing fails.
            test();
        }

        protected void VerifyAssert(Action test, params string[] expectedOutputStrings)
        {
            FieldInfo writeCoreHook = typeof(DebugProvider).GetField("s_WriteCore", BindingFlags.Static | BindingFlags.NonPublic);
            s_defaultProvider = Debug.SetProvider(WriteLogger.s_instance);

            var originalWriteCoreHook = writeCoreHook.GetValue(null);
            writeCoreHook.SetValue(null, new Action<string>(WriteLogger.s_instance.WriteCore));

            try
            {
                WriteLogger.s_instance.Clear();
                test();
#if DEBUG
                for (int i = 0; i < expectedOutputStrings.Length; i++)
                {
                    Assert.Contains(expectedOutputStrings[i], WriteLogger.s_instance.LoggedOutput);
                    Assert.Contains(expectedOutputStrings[i], WriteLogger.s_instance.AssertUIOutput);
                }
#else
                Assert.Equal(string.Empty, WriteLogger.s_instance.LoggedOutput);
                Assert.Equal(string.Empty, WriteLogger.s_instance.AssertUIOutput);
#endif

            }
            finally
            {
                writeCoreHook.SetValue(null, originalWriteCoreHook);
                Debug.SetProvider(s_defaultProvider);
            }
        }

        private static DebugProvider s_defaultProvider;
        private class WriteLogger : DebugProvider
        {
            public static readonly WriteLogger s_instance = new WriteLogger();

            private WriteLogger() { }

            public string LoggedOutput { get; private set; }

            public string AssertUIOutput { get; private set; }

            public void Clear()
            {
                LoggedOutput = string.Empty;
                AssertUIOutput = string.Empty;
            }

            public override void ShowDialog(string stackTrace, string message, string detailMessage, string errorSource)
            {
                AssertUIOutput += stackTrace + message + detailMessage + errorSource;
            }

            public void WriteCore(string message)
            {
                Assert.NotNull(message);
                LoggedOutput += message;
            }
        }
    }
}
