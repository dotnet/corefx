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
                callStack.Push(GetSourceInformation(1));
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
                if (!PlatformDetection.IsFullFramework)
                    callStack.Push(GetSourceInformation(1));
                throw new Exception("Boom!");
            }
            catch
            {
                if (PlatformDetection.IsFullFramework)
                    callStack.Push(GetSourceInformation(1));
                throw;
            }
        }

        [Fact]
        public static void ThrowStatementDoesNotResetExceptionStackLineOtherMethod()
        {
            var callStack = new Stack<(string, string, int)>();

            try
            {
                callStack.Push(GetSourceInformation(1));
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
                if (!PlatformDetection.IsFullFramework)
                    callStack.Push(GetSourceInformation(1));
                ThrowException(callStack);
            }
            catch
            {
                if (PlatformDetection.IsFullFramework)
                {
                    var throwFrame = callStack.Pop();
                    callStack.Push(GetSourceInformation(3));
                    callStack.Push(throwFrame);
                }
                throw;
            }
        }

        private static void ThrowException(Stack<(string, string, int)> callStack)
        {
            callStack.Push(GetSourceInformation(1));
            throw new Exception("Boom!");
        }

        private static void VerifyCallStack(
            Stack<(string CallerMemberName, string SourceFilePath, int SourceLineNumber)> expectedCallStack, string reportedCallStack)
        {
            Console.WriteLine("* ExceptionTests - reported call stack:\n{0}", reportedCallStack);
            const string frameParserRegex = @"\s+at\s.+\.(?<memberName>[^(.]+)\([^)]*\)\sin\s(?<filePath>.*)\:line\s(?<lineNumber>[\d]+)";

            using (var sr = new StringReader(reportedCallStack))
            {
                string frame;
                while (!string.IsNullOrEmpty(frame = sr.ReadLine()))
                {
                    var exptectedFrame = expectedCallStack.Pop();
                    var match = Regex.Match(frame, frameParserRegex);
                    Assert.True(match.Success);
                    Assert.Equal(exptectedFrame.CallerMemberName, match.Groups["memberName"].Value);
                    Assert.Equal(exptectedFrame.SourceFilePath, match.Groups["filePath"].Value);
                    Assert.Equal(exptectedFrame.SourceLineNumber, Convert.ToInt32(match.Groups["lineNumber"].Value));
                }
            }
        }

        private static (string, string, int) GetSourceInformation(
            int offset,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            return (memberName, sourceFilePath, sourceLineNumber + offset);
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
