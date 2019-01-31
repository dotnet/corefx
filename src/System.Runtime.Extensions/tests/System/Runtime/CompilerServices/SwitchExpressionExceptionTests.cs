// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;
using Xunit;

namespace System.Runtime.CompilerServices.Tests
{
    public class SwitchExpressionExceptionTests
    {
        [Fact]
        public static void DefaultConstructor()
        {
            var ex = new SwitchExpressionException();

            Assert.NotNull(ex.Message);
        }

        [Fact]
        public static void Constructor_StringAsObjectArg()
        {
            string message = "stringInput";
            var ex = new SwitchExpressionException(message);

            Assert.NotEqual(message, ex.Message);
            Assert.Same(message, ex.UnmatchedValue);
        }

        [Fact]
        public void UnmatchedValue()
        {
            var ex = new SwitchExpressionException(34);
            Assert.Equal(34, ex.UnmatchedValue);
            Assert.Contains(ex.UnmatchedValue.ToString(), ex.Message);

            var data = new byte[] { 1, 2, 3 };
            ex = new SwitchExpressionException(data);
            Assert.Same(data, ex.UnmatchedValue);
            Assert.Contains(ex.UnmatchedValue.ToString(), ex.Message);

            ex = new SwitchExpressionException(true);
            Assert.Equal(true, ex.UnmatchedValue);
            Assert.Contains(ex.UnmatchedValue.ToString(), ex.Message);

            ex = new SwitchExpressionException("34");
            Assert.Same("34", ex.UnmatchedValue);
            Assert.Contains(ex.UnmatchedValue.ToString(), ex.Message);

            ex = new SwitchExpressionException(null);
            Assert.Null(ex.UnmatchedValue);
        }
    }
}
