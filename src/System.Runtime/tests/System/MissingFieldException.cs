// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Runtime.Tests
{
    public static class MissingFieldExceptionTests
    {
        private const int COR_E_MISSINGFIELD = unchecked((int)0x80131511);

        [Fact]
        public static void TestCtor_Empty()
        {
            var exception = new MissingFieldException();

            Assert.Equal("Attempted to access a non-existing field.", exception.Message);
            Assert.Equal(COR_E_MISSINGFIELD, exception.HResult);
        }

        [Fact]
        public static void TestCtor_String()
        {
            var exception = new MissingFieldException("Created MissingFieldException");

            Assert.Equal("Created MissingFieldException", exception.Message);
            Assert.Equal(COR_E_MISSINGFIELD, exception.HResult);
        }

        [Fact]
        public static void TestCtor_String_Exception()
        {
            var innerException = new Exception("Created inner exception");
            var exception = new MissingFieldException("Created MissingFieldException", innerException);

            Assert.Equal("Created MissingFieldException", exception.Message);
            Assert.Equal(COR_E_MISSINGFIELD, exception.HResult);
            Assert.Equal("Created inner exception", exception.InnerException.Message);
            Assert.Equal(innerException.HResult, exception.InnerException.HResult);
        }
    }
}
