// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Xunit;

namespace System.Tests
{
    public static class ExceptionTests
    {
        [Fact]
        public static void Exception_GetType()
        {
            Assert.Equal(typeof(Exception), (new Exception()).GetType());
            Assert.Equal(typeof(NullReferenceException), (new NullReferenceException()).GetType());
        }

        [Fact]
        [SkipOnTargetFramework(TargetFrameworkMonikers.UapAot, "Exception.TargetSite always returns null on UapAot.")]
        public static void Exception_TargetSite_Jit()
        {
            bool caught = false;

            try
            {
                throw new Exception();
            }
            catch (Exception ex)
            {
                caught = true;

                Assert.Equal(MethodInfo.GetCurrentMethod(), ex.TargetSite);
            }

            Assert.True(caught);
        }

        [Fact]
        [SkipOnTargetFramework(~TargetFrameworkMonikers.UapAot, "Exception.TargetSite always returns null on UapAot.")]
        public static void Exception_TargetSite_Aot()
        {
            bool caught = false;

            try
            {
                throw new Exception();
            }
            catch (Exception ex)
            {
                caught = true;

                Assert.Null(ex.TargetSite);
            }

            Assert.True(caught);
        }

        [Fact]
        public static void ThrowStatementDoesNotResetExceptionStackLineSameMethod()
        {
            var callStack = new Stack<(string, string, int)>();

            try
            {
                // Keep the two statements below together
                callStack.Push(GetSourceInformation());
                ThrowAndRethrowSameMethod(callStack);
            }
            catch (Exception ex)
            {
                VerifyCallStack(callStack, ex.StackTrace);
            }
        }

        private static void ThrowAndRethrowSameMethod(Stack<(string, string, int)> callStack)
        {
            try
            {
                // Keep the two statements below together
                callStack.Push(GetSourceInformation());
                throw new Exception("Boom!");
            }
            catch
            {
                throw;
            }
        }

        [Fact]
        public static void ThrowStatementDoesNotResetExceptionStackLineOtherMethod()
        {
            var callStack = new Stack<(string, string, int)>();

            try
            {
                // Keep the two statements below together
                callStack.Push(GetSourceInformation());
                ThrowAndRethrowOtherMethod(callStack);
            }
            catch (Exception ex)
            {
                VerifyCallStack(callStack, ex.StackTrace);
            }
        }

        private static void ThrowAndRethrowOtherMethod(Stack<(string, string, int)> callStack)
        {
            try
            {
                // Keep the two statements below together
                callStack.Push(GetSourceInformation());
                ThrowException(callStack);
            }
            catch
            {
                throw;
            }
        }

        private static void ThrowException(Stack<(string, string, int)> callStack)
        {
            // Keep the two statements below together
            callStack.Push(GetSourceInformation());
            throw new Exception("Boom!");
        }

        private static void VerifyCallStack(
            Stack<(string CallerMemberName, string SourceFilePath, int SourceLineNumber)> expectedCallStack, string reportedCallStack)
        {
            const string FrameParserRegex = @"\s+at\s.+\.(?<memberName>[^(]+)\([^)]*\)\sin\s(?<filePath>.*)\:line\s(?<lineNumber>[\d]+)";

            using (var sr = new StringReader(reportedCallStack))
            {
                string frame;
                while (!string.IsNullOrEmpty(frame = sr.ReadLine()))
                {
                    var reportedFrame = expectedCallStack.Pop();
                    var match = Regex.Match(frame, FrameParserRegex);
                    Assert.True(match.Success);
                    Assert.Equal(reportedFrame.CallerMemberName, match.Groups["memberName"].Value);
                    Assert.Equal(reportedFrame.SourceFilePath, match.Groups["filePath"].Value);
                    Assert.Equal(reportedFrame.SourceLineNumber + 1, Convert.ToInt32(match.Groups["lineNumber"].Value));
                }
            }
        }

        private static (string, string, int) GetSourceInformation(
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            return (memberName, sourceFilePath, sourceLineNumber);
        }
    }

    public class ExceptionDerivedTests: Exception
    {
        [Fact]
        [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework)]
        public static void Exception_SerializeObjectState()
        {
            var excp = new ExceptionDerivedTests();
            Assert.Throws<PlatformNotSupportedException>( () => excp.SerializeObjectState += (exception, eventArgs) => eventArgs.AddSerializedState(null));
            Assert.Throws<PlatformNotSupportedException>( () => excp.SerializeObjectState -= (exception, eventArgs) => eventArgs.AddSerializedState(null));
        }
    }
}
