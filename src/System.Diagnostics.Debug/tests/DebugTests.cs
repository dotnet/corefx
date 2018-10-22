// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using Xunit;

namespace System.Diagnostics.Tests
{
    // These tests test the static Debug class. They cannot be run in parallel
    [Collection("System.Diagnostics.Debug")]
    public class DebugTests
    {
        // [Fact] // uncomment when running this xunit method alone
        public void Bug_SkipsIndentationOnFirstWrite()
        {
            // This test shows an existing indentation bug in Debug class
                // First Debug.Write call won't respect the indentation. Desktop does not have this bug.

            Debug.Indent();
            int expectedIndentation = Debug.IndentLevel * Debug.IndentSize;
            int wrongIndentation = 0; // should match with expectedIndentation
            
            VerifyLogged(() => Debug.Write("pizza"),        new string(' ', wrongIndentation) +     "pizza"); // Inconsistent with Desktop
            
            // After first WriteLine invocation we get proper indentation
            VerifyLogged(() => Debug.WriteLine("pizza"),    new string(' ', 0) +                    "pizza" + Environment.NewLine);

            VerifyLogged(() => Debug.WriteLine("pizza"),    new string(' ', expectedIndentation) +  "pizza" + Environment.NewLine);
            VerifyLogged(() => Debug.Write("pizza"),        new string(' ', expectedIndentation) +  "pizza");
            VerifyLogged(() => Debug.WriteLine("pizza"),    new string(' ', 0) +                    "pizza" + Environment.NewLine); 
            VerifyLogged(() => Debug.WriteLine("pizza"),    new string(' ', expectedIndentation) +  "pizza" + Environment.NewLine);
            Debug.Unindent();
        }

        // [Fact] // uncomment when running this xunit method alone
        public void Bug_SkipsIndentationOnFirstWriteLine()
        {
            // This test shows an existing indentation bug in Debug class
                // First Debug.WriteLine call won't respect the indentation. Desktop does not have this bug.

            Debug.Indent();
            int expectedIndentation = Debug.IndentLevel * Debug.IndentSize;
            int wrongIndentation = 0; // should match with expectedIndentation

            // After first WriteLine invocation we get proper indentation
            VerifyLogged(() => Debug.WriteLine("pizza"),    new string(' ', wrongIndentation) +     "pizza" + Environment.NewLine); // Inconsistent with Desktop

            VerifyLogged(() => Debug.WriteLine("pizza"),    new string(' ', expectedIndentation) +  "pizza" + Environment.NewLine);
            VerifyLogged(() => Debug.Write("pizza"),        new string(' ', expectedIndentation) +  "pizza");
            VerifyLogged(() => Debug.WriteLine("pizza"),    new string(' ', 0) +                    "pizza" + Environment.NewLine); 
            VerifyLogged(() => Debug.WriteLine("pizza"),    new string(' ', expectedIndentation) +  "pizza" + Environment.NewLine);
            Debug.Unindent();
        }

        // [Fact] // uncomment when running this xunit method alone
        public void Bug_DebugSumsUpTraceAndDebugIndentation()
        {
            // In Core:
               // - The existing indentation amount for Trace is currently: `Debug.IndentLevel * Debug.IndentSize + Trace.IndentLevel * Trace.IndentSize`. 
               // The Trace indentation amount is just `Trace.IndentLevel * Trace.IndentSize`

            // Set same values of IndentSize and IndentLevel for both Trace and Debug, to ignore bug in: DesktopDiscrepancy_DebugIndentationNotInSyncWithTrace
            Debug.IndentSize = 4;
            Trace.IndentSize = 4;
            Debug.Indent();
            Trace.Indent();

            int expected = Debug.IndentSize * Debug.IndentLevel;
            int actual = expected + Trace.IndentLevel * Trace.IndentSize; // should match with expected

            Debug.WriteLine("pizza"); // Skip first call, to ignore bug in: Bug_SkipsIndentationOnFirstWriteLine
            VerifyLogged(() => Debug.WriteLine("pizza"),    new string(' ', expected) +  "pizza" + Environment.NewLine);
            Trace.WriteLine("pizza"); // Wires up Debug with TraceListeners
            VerifyLogged(() => Trace.WriteLine("pizza"),    new string(' ', actual) +  "pizza" + Environment.NewLine);

            // reset
            Debug.Unindent();
            Trace.Unindent();
            Trace.Refresh();
        }

        // [Fact] // uncomment when running this xunit method alone
        public void WriteNull()
        {
            // reset
            Debug.IndentSize = 4;
            Trace.IndentSize = 4;
            Debug.Indent();
            Trace.Indent();

            int expected = Debug.IndentSize * Debug.IndentLevel;
            int actual = expected + Trace.IndentLevel * Trace.IndentSize; // should match with expected

            Debug.WriteLine(null); // Skip first call, to ignore bug in: Bug_SkipsIndentationOnFirstWriteLine
            VerifyLogged(() => Debug.WriteLine(null), new string(' ', expected) + Environment.NewLine);
            Trace.WriteLine(null); // Wires up Debug with TraceListeners
            VerifyLogged(() => Trace.WriteLine(null), new string(' ', actual) + Environment.NewLine);

            // reset
            Debug.Unindent();
            Trace.Unindent();
            Trace.Refresh();
        }
        
        [Fact]
        public void DesktopDiscrepancy_DebugIndentationNotInSyncWithTrace()
        {
            // This test shows an existing indentation bug in Debug class:
                // In Desktop, Debug and Trace have the same values for IndentLevel and IndentSize.
                // In Core, Updating Trace IndentLevel or IndentSize won't update that for Debug and vice versa.

            // reset
            int originalDebugIndentSize = Debug.IndentSize;
            int originalTraceIndentSize = Trace.IndentSize;
            Debug.IndentLevel = 0;
            Trace.IndentLevel = 0;

            // test if Debug gets changed when Trace changes
            Trace.IndentSize = 7;
            Assert.Equal(7, Trace.IndentSize);
            Assert.NotEqual(7, Debug.IndentSize); // Inconsistent with Desktop, should be 7
            Trace.Indent();
            Assert.Equal(1, Trace.IndentLevel);
            Assert.NotEqual(1, Debug.IndentLevel); // Inconsistent with Desktop, should be 1

            // reset
            Trace.Unindent();
            Trace.IndentSize = originalTraceIndentSize;

            // test if Trace gets changed when Debug changes
            Debug.IndentSize = 7;
            Assert.Equal(7, Debug.IndentSize);
            Assert.NotEqual(7, Trace.IndentSize); // Inconsistent with Desktop, should be 7
            Debug.Indent();
            Assert.Equal(1, Debug.IndentLevel);
            Assert.NotEqual(1, Trace.IndentLevel); // Inconsistent with Desktop, should be 1
            
            // reset
            Debug.Unindent();
            Debug.IndentSize = originalDebugIndentSize;
            Trace.Refresh();
        }

        [Fact]
        public void ClearTraceListeners_StopsWritingToDebugger()
        {
            VerifyLogged(() => Debug.Write("pizza"), "pizza");
            VerifyLogged(() => Trace.Write("pizza"), "pizza"); // Wires up Debug with TraceListeners
            Trace.Listeners.Clear();
            VerifyLogged(() => Debug.Write("pizza"), string.Empty); // new behavior, corrects behavior discrepancy with Desktop
            VerifyLogged(() => Trace.Write("pizza"), string.Empty);
            Trace.Refresh();
        }

        [Fact]
        public void Asserts()
        {
            VerifyLogged(() => Debug.Assert(true), "");
            VerifyLogged(() => Debug.Assert(true, "assert passed"), "");
            VerifyLogged(() => Debug.Assert(true, "assert passed", "nothing is wrong"), "");
            VerifyLogged(() => Debug.Assert(true, "assert passed", "nothing is wrong {0} {1}", 'a', 'b'), "");

            VerifyAssert(() => Debug.Assert(false), "");
            VerifyAssert(() => Debug.Assert(false, "assert passed"), "assert passed");
            VerifyAssert(() => Debug.Assert(false, "assert passed", "nothing is wrong"), "assert passed", "nothing is wrong");
            VerifyAssert(() => Debug.Assert(false, "assert passed", "nothing is wrong {0} {1}", 'a', 'b'), "assert passed", "nothing is wrong a b");      
        }

        [Fact]
        public void Fail()
        {
            VerifyAssert(() => Debug.Fail("something bad happened"), "something bad happened");
            VerifyAssert(() => Debug.Fail("something bad happened", "really really bad"), "something bad happened", "really really bad");        
        }

        [Fact]
        public void Write()
        {
            VerifyLogged(() => Debug.Write(5), "5");
            VerifyLogged(() => Debug.Write((string)null), "");
            VerifyLogged(() => Debug.Write((object)null), "");
            VerifyLogged(() => Debug.Write(5, "category"), "category:5");
            VerifyLogged(() => Debug.Write((object)null, "category"), "category:");
            VerifyLogged(() => Debug.Write("logged"), "logged");
            VerifyLogged(() => Debug.Write("logged", "category"), "category:logged");
            VerifyLogged(() => Debug.Write("logged", (string)null), "logged");

            string longString = new string('d', 8192);
            VerifyLogged(() => Debug.Write(longString), longString);
        }

        [Fact]
        public void Print()
        {
            VerifyLogged(() => Debug.Print("logged"), "logged");
            VerifyLogged(() => Debug.Print("logged {0}", 5), "logged 5");
        }

        [Fact]
        public void WriteLine()
        {
            VerifyLogged(() => Debug.WriteLine(5), "5" + Environment.NewLine);
            VerifyLogged(() => Debug.WriteLine((string)null), Environment.NewLine);
            VerifyLogged(() => Debug.WriteLine((object)null), Environment.NewLine);
            VerifyLogged(() => Debug.WriteLine(5, "category"), "category:5" + Environment.NewLine);
            VerifyLogged(() => Debug.WriteLine((object)null, "category"), "category:" + Environment.NewLine);
            VerifyLogged(() => Debug.WriteLine("logged"), "logged" + Environment.NewLine);
            VerifyLogged(() => Debug.WriteLine("logged", "category"), "category:logged" + Environment.NewLine);
            VerifyLogged(() => Debug.WriteLine("logged", (string)null), "logged" + Environment.NewLine);
            VerifyLogged(() => Debug.WriteLine("{0} {1}", 'a', 'b'), "a b" + Environment.NewLine);
        }

        [Fact]
        public void WriteIf()
        {
            VerifyLogged(() => Debug.WriteIf(true, 5), "5");
            VerifyLogged(() => Debug.WriteIf(false, 5), "");

            VerifyLogged(() => Debug.WriteIf(true, 5, "category"), "category:5");
            VerifyLogged(() => Debug.WriteIf(false, 5, "category"), "");

            VerifyLogged(() => Debug.WriteIf(true, "logged"), "logged");
            VerifyLogged(() => Debug.WriteIf(false, "logged"), "");

            VerifyLogged(() => Debug.WriteIf(true, "logged", "category"), "category:logged");
            VerifyLogged(() => Debug.WriteIf(false, "logged", "category"), "");
        }

        [Fact]
        public void WriteLineIf()
        {
            VerifyLogged(() => Debug.WriteLineIf(true, 5), "5" + Environment.NewLine);
            VerifyLogged(() => Debug.WriteLineIf(false, 5), "");

            VerifyLogged(() => Debug.WriteLineIf(true, 5, "category"), "category:5" + Environment.NewLine);
            VerifyLogged(() => Debug.WriteLineIf(false, 5, "category"), "");

            VerifyLogged(() => Debug.WriteLineIf(true, "logged"), "logged" + Environment.NewLine);
            VerifyLogged(() => Debug.WriteLineIf(false, "logged"), "");

            VerifyLogged(() => Debug.WriteLineIf(true, "logged", "category"), "category:logged" + Environment.NewLine);
            VerifyLogged(() => Debug.WriteLineIf(false, "logged", "category"), "");     
        }

        [Theory]
        [InlineData(-1, 0)]
        [InlineData(0, 0)]
        [InlineData(1, 1)]
        [InlineData(2, 2)]
        [InlineData(3, 3)]
        [InlineData(4, 4)]
        public void IndentSize_Set_GetReturnsExpected(int indentSize, int expectedIndentSize)
        {
            Debug.IndentLevel = 0;

            Debug.IndentSize = indentSize;
            Assert.Equal(expectedIndentSize, Debug.IndentSize);
            VerifyLogged(() => Debug.WriteLine("pizza"), "pizza" + Environment.NewLine);

            // Indent once.
            Debug.Indent();
            string expectedIndentOnce = new string(' ', expectedIndentSize);
            VerifyLogged(() => Debug.WriteLine("pizza"), expectedIndentOnce + "pizza" + Environment.NewLine);

            // Indent again.
            Debug.Indent();
            string expectedIndentTwice = new string(' ', expectedIndentSize * 2);
            VerifyLogged(() => Debug.WriteLine("pizza"), expectedIndentTwice + "pizza" + Environment.NewLine);

            // Unindent.
            Debug.Unindent();
            Debug.Unindent();
        }

        [Theory]
        [InlineData(-1, 0)]
        [InlineData(0, 0)]
        [InlineData(1, 1)]
        public void IndentLevel_Set_GetReturnsExpected(int indentLevel, int expectedIndentLevel)
        {
            Debug.IndentLevel = indentLevel;
            Assert.Equal(expectedIndentLevel, Debug.IndentLevel);
            string expectedIndentOnce = new string(' ', expectedIndentLevel * Debug.IndentSize);
            VerifyLogged(() => Debug.WriteLine("pizza"), expectedIndentOnce + "pizza" + Environment.NewLine);

            // Indent once.
            Debug.Indent();
#if DEBUG
            Assert.Equal(expectedIndentLevel + 1, Debug.IndentLevel);
#else
            Assert.Equal(expectedIndentLevel, Debug.IndentLevel);
#endif
            string expectedIndentTwice = new string(' ', (expectedIndentLevel + 1) * Debug.IndentSize);
            VerifyLogged(() => Debug.WriteLine("pizza"), expectedIndentTwice + "pizza" + Environment.NewLine);

            // Unindent.
            Debug.Unindent();
            Assert.Equal(expectedIndentLevel, Debug.IndentLevel);
            Debug.Unindent();
        }

        static void VerifyLogged(Action test, string expectedOutput)
        {
            // .NET Core has an internal extensibility point for unit tests.
            if (PlatformDetection.IsFullFramework || PlatformDetection.IsUap)
            {
                return;
            }

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

        static void VerifyAssert(Action test, params string[] expectedOutputStrings)
        {
            // .NET Core has an internal extensibility point for unit tests.
            if (PlatformDetection.IsFullFramework || PlatformDetection.IsUap)
            {
                return;
            }

            FieldInfo writeCoreHook = typeof(DebugProvider).GetField("s_WriteCore", BindingFlags.Static | BindingFlags.NonPublic);
            Debug.SetProvider(WriteLogger.s_instance);

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

        private static readonly DebugProvider s_defaultProvider = new DebugProvider();
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
