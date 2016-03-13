// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.ComponentModel.EventBasedAsync.Tests
{
    public class RunWorkerCompletedEventArgsTests
    {
        public static IEnumerable<object[]> TestInput
        {
            get
            {
                return new[]
                {
                    new object[] { 42, null, false, (Type)null },
                    new object[] { null, null, true, typeof(OperationCanceledException) },
                    // dummy exceptions
                    new object[] { null, new FormatException(), false, typeof(FormatException) },
                    new object[] { null, new DllNotFoundException(), true, typeof(OperationCanceledException) }
                };
            }
        }

        [Theory]
        [InlineData(null)]
        [InlineData("non null")]
        public static void CtorTest(object expectedResult)
        {
            var target = new RunWorkerCompletedEventArgs(expectedResult, null, false);
            Assert.Same(expectedResult, target.Result);
            Assert.Null(target.UserState);
        }

        [Theory]
        [MemberData(nameof(TestInput))]
        public static void ResultPropertyTest(object expectedResult, Exception expectedError, bool cancelled, Type expectedExceptionType)
        {
            var target = new RunWorkerCompletedEventArgs(expectedResult, expectedError, cancelled);

            if (expectedExceptionType == null) // if null should NOT throw
            {
                Assert.Equal(expectedResult, target.Result);
            }
            else
            {
                Exception error = Assert.Throws(expectedExceptionType, () => target.Result);
                if (expectedError != null && !cancelled)
                {
                    Assert.Same(expectedError, error);
                }
            }
        }
    }
}
