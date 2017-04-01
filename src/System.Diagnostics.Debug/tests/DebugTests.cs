// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Xunit;

namespace System.Diagnostics.Tests
{
    // These tests test the static Debug class. They cannot be run in parallel
    [Collection("System.Diagnostics.Debug")]
    public class DebugTests
    {
        private readonly string s_newline = // avoid Environment direct dependency, due to it being visible from both System.Private.Corelib and System.Runtime.Extensions
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "\r\n" : "\n";

        [Fact]
        public void Asserts()
        {
            VerifyLogged(() => { Debug.Assert(true); }, "");
            VerifyLogged(() => { Debug.Assert(true, "assert passed"); }, "");
            VerifyLogged(() => { Debug.Assert(true, "assert passed", "nothing is wrong"); }, "");
            VerifyLogged(() => { Debug.Assert(true, "assert passed", "nothing is wrong {0} {1}", 'a', 'b'); }, "");

            VerifyAssert(() => { Debug.Assert(false); }, "");
            VerifyAssert(() => { Debug.Assert(false, "assert passed"); }, "assert passed");
            VerifyAssert(() => { Debug.Assert(false, "assert passed", "nothing is wrong"); }, "assert passed", "nothing is wrong");
            VerifyAssert(() => { Debug.Assert(false, "assert passed", "nothing is wrong {0} {1}", 'a', 'b'); }, "assert passed", "nothing is wrong a b");      
        }

        [Fact]
        public void Fail()
        {
            VerifyAssert(() => { Debug.Fail("something bad happened"); }, "something bad happened");
            VerifyAssert(() => { Debug.Fail("something bad happened", "really really bad"); }, "something bad happened", "really really bad");        
        }

        [Fact]
        public void Write()
        {
            VerifyLogged(() => { Debug.Write(5); }, "5");
            VerifyLogged(() => { Debug.Write((string)null); }, "");
            VerifyLogged(() => { Debug.Write((object)null); }, "");
            VerifyLogged(() => { Debug.Write(5, "category"); }, "category:5");
            VerifyLogged(() => { Debug.Write((object)null, "category"); }, "category:");
            VerifyLogged(() => { Debug.Write("logged"); }, "logged");
            VerifyLogged(() => { Debug.Write("logged", "category"); }, "category:logged");
            VerifyLogged(() => { Debug.Write("logged", (string)null); }, "logged");

            string longString = new string('d', 8192);
            VerifyLogged(() => { Debug.Write(longString); }, longString);
        }

        [Fact]
        public void Print()
        {
            VerifyLogged(() => { Debug.Print("logged"); }, "logged");
            VerifyLogged(() => { Debug.Print("logged {0}", 5); }, "logged 5");
        }

        [Fact]
        public void WriteLine()
        {
            VerifyLogged(() => { Debug.WriteLine(5); }, "5" + s_newline);
            VerifyLogged(() => { Debug.WriteLine((string)null); }, s_newline);
            VerifyLogged(() => { Debug.WriteLine((object)null); }, s_newline);
            VerifyLogged(() => { Debug.WriteLine(5, "category"); }, "category:5" + s_newline);
            VerifyLogged(() => { Debug.WriteLine((object)null, "category"); }, "category:" + s_newline);
            VerifyLogged(() => { Debug.WriteLine("logged"); }, "logged" + s_newline);
            VerifyLogged(() => { Debug.WriteLine("logged", "category"); }, "category:logged" + s_newline);
            VerifyLogged(() => { Debug.WriteLine("logged", (string)null); }, "logged" + s_newline);
            VerifyLogged(() => { Debug.WriteLine("{0} {1}", 'a', 'b'); }, "a b" + s_newline);
        }

        [Fact]
        public void WriteIf()
        {
            VerifyLogged(() => { Debug.WriteIf(true, 5); }, "5");
            VerifyLogged(() => { Debug.WriteIf(false, 5); }, "");

            VerifyLogged(() => { Debug.WriteIf(true, 5, "category"); }, "category:5");
            VerifyLogged(() => { Debug.WriteIf(false, 5, "category"); }, "");

            VerifyLogged(() => { Debug.WriteIf(true, "logged"); }, "logged");
            VerifyLogged(() => { Debug.WriteIf(false, "logged"); }, "");

            VerifyLogged(() => { Debug.WriteIf(true, "logged", "category"); }, "category:logged");
            VerifyLogged(() => { Debug.WriteIf(false, "logged", "category"); }, "");
        }

        [Fact]
        public void WriteLineIf()
        {
            VerifyLogged(() => { Debug.WriteLineIf(true, 5); }, "5" + s_newline);
            VerifyLogged(() => { Debug.WriteLineIf(false, 5); }, "");

            VerifyLogged(() => { Debug.WriteLineIf(true, 5, "category"); }, "category:5" + s_newline);
            VerifyLogged(() => { Debug.WriteLineIf(false, 5, "category"); }, "");

            VerifyLogged(() => { Debug.WriteLineIf(true, "logged"); }, "logged" + s_newline);
            VerifyLogged(() => { Debug.WriteLineIf(false, "logged"); }, "");

            VerifyLogged(() => { Debug.WriteLineIf(true, "logged", "category"); }, "category:logged" + s_newline);
            VerifyLogged(() => { Debug.WriteLineIf(false, "logged", "category"); }, "");     
        }

        [Theory]
        [InlineData(2)]
        [InlineData(4)]
        [InlineData(3)]
        public void Indentation(int indentSize)
        {
            Debug.IndentLevel = 0;
            Debug.IndentSize = indentSize;
            VerifyLogged(() => { Debug.WriteLine("pizza"); }, "pizza" + s_newline);
            Debug.Indent();
            string expectedIndent = new string(' ', indentSize);
            VerifyLogged(() => { Debug.WriteLine("pizza"); }, expectedIndent + "pizza" + s_newline);
            Debug.Indent();
            expectedIndent = new string(' ', indentSize * 2);
            VerifyLogged(() => { Debug.WriteLine("pizza"); }, expectedIndent + "pizza" + s_newline);
            Debug.Unindent();
            Debug.Unindent();
        }


        static void VerifyLogged(Action test, string expectedOutput)
        {
            FieldInfo writeCoreHook = typeof(Debug).GetField("s_WriteCore", BindingFlags.Static | BindingFlags.NonPublic);

            // First use our test logger to verify the output
            var originalWriteCoreHook = writeCoreHook.GetValue(null);
            writeCoreHook.SetValue(null, new Action<string>(WriteLogger.Instance.WriteCore));

            try
            {
                WriteLogger.Instance.Clear();
                test();
#if DEBUG                
                Assert.Equal(expectedOutput, WriteLogger.Instance.LoggedOutput);
#else
                Assert.Equal(string.Empty, WriteLogger.Instance.LoggedOutput);
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
            FieldInfo writeCoreHook = typeof(Debug).GetField("s_WriteCore", BindingFlags.Static | BindingFlags.NonPublic);
            FieldInfo showAssertDialogHook = typeof(Debug).GetField("s_ShowAssertDialog", BindingFlags.Static | BindingFlags.NonPublic);

            var originalWriteCoreHook = writeCoreHook.GetValue(null);
            writeCoreHook.SetValue(null, new Action<string>(WriteLogger.Instance.WriteCore));

            var originalShowAssertDialogHook = showAssertDialogHook.GetValue(null);
            showAssertDialogHook.SetValue(null, new Action<string, string, string>(WriteLogger.Instance.ShowAssertDialog));

            try
            {
                WriteLogger.Instance.Clear();
                test();
#if DEBUG
                for (int i = 0; i < expectedOutputStrings.Length; i++)
                {
                    Assert.Contains(expectedOutputStrings[i], WriteLogger.Instance.LoggedOutput);
                    Assert.Contains(expectedOutputStrings[i], WriteLogger.Instance.AssertUIOutput);
                }
#else
                Assert.Equal(string.Empty, WriteLogger.Instance.LoggedOutput);
                Assert.Equal(string.Empty, WriteLogger.Instance.AssertUIOutput);
#endif

            }
            finally
            {
                writeCoreHook.SetValue(null, originalWriteCoreHook);
                showAssertDialogHook.SetValue(null, originalShowAssertDialogHook);
            }
        }

        class WriteLogger
        {
            public static readonly WriteLogger Instance = new WriteLogger();

            private WriteLogger()
            {

            }

            public string LoggedOutput
            {
                get;
                private set;
            }

            public string AssertUIOutput
            {
                get;
                private set;
            }

            public void Clear()
            {
                LoggedOutput = string.Empty;
                AssertUIOutput = string.Empty;
            }

            public void ShowAssertDialog(string stackTrace, string message, string detailMessage)
            {
                AssertUIOutput += stackTrace + message + detailMessage;
            }

            public void WriteCore(string message)
            {
                Assert.NotNull(message);
                LoggedOutput += message;
            }
        }
    }
}
