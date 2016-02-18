// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Runtime.Tests
{
    public static class MissingMethodExceptionTests
    {
        private const int COR_E_MISSINGMETHOD = unchecked((int)0x80131513);

        [Fact]
        public static void TestCtor_Empty()
        {
            var exception = new MissingMethodException();

            Assert.Equal("Attempted to access a missing method.", exception.Message);
            Assert.Equal(COR_E_MISSINGMETHOD, exception.HResult);
        }

        [Fact]
        public static void TestCtor_String()
        {
            var exception = new MissingMethodException("Created MissingMethodException");

            Assert.Equal("Created MissingMethodException", exception.Message);
            Assert.Equal(COR_E_MISSINGMETHOD, exception.HResult);
        }

        [Fact]
        public static void TestCtor_Exception()
        {
            var innerException = new Exception("Created inner exception");
            var exception = new MissingMethodException("Created MissingMethodException", innerException);

            Assert.Equal("Created MissingMethodException", exception.Message);
            Assert.Equal(COR_E_MISSINGMETHOD, exception.HResult);
            Assert.Equal("Created inner exception", exception.InnerException.Message);
            Assert.Equal(innerException.HResult, exception.InnerException.HResult);
        }
    }
}
