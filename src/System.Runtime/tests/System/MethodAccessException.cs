// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Runtime.Tests
{
    public static class MethodAccessExceptionTests
    {
        private const int COR_E_METHODACCESS = unchecked((int)0x80131510);

        [Fact]
        public static void TestCtor_Empty()
        {
            var exception = new MethodAccessException();

            Assert.Equal("Attempt to access the method failed.", exception.Message);
            Assert.Equal(COR_E_METHODACCESS, exception.HResult);
        }

        [Fact]
        public static void TestCtor_String()
        {
            var exception = new MethodAccessException("Created MethodAccessException");

            Assert.Equal("Created MethodAccessException", exception.Message);
            Assert.Equal(COR_E_METHODACCESS, exception.HResult);
        }

        [Fact]
        public static void TestCtor_Exception()
        {
            var innerException = new Exception("Created inner exception");
            var exception = new MethodAccessException("Created MethodAccessException", innerException);

            Assert.Equal("Created MethodAccessException", exception.Message);
            Assert.Equal(COR_E_METHODACCESS, exception.HResult);
            Assert.Equal("Created inner exception", exception.InnerException.Message);
            Assert.Equal(innerException.HResult, exception.InnerException.HResult);
        }
    }
}
