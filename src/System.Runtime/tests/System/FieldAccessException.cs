// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Runtime.Tests
{
    public static class FieldAccessExceptionTests
    {
        private const int COR_E_FIELDACCESS = unchecked((int)0x80131507);

        [Fact]
        public static void TestCtor_Empty()
        {
            var exception = new FieldAccessException();

            Assert.Equal("Attempted to access a field that is not accessible by the caller.", exception.Message);
            Assert.Equal(COR_E_FIELDACCESS, exception.HResult);
        }

        [Fact]
        public static void TestCtor_String()
        {
            var exception = new FieldAccessException("Created FieldAccessException");

            Assert.Equal("Created FieldAccessException", exception.Message);
            Assert.Equal(COR_E_FIELDACCESS, exception.HResult);
        }

        [Fact]
        public static void TestCtor_String_Exception()
        {
            var innerException = new Exception("Created inner exception");
            var exception = new FieldAccessException("Created FieldAccessException", innerException);

            Assert.Equal("Created FieldAccessException", exception.Message);
            Assert.Equal("Created inner exception", exception.InnerException.Message);
            Assert.Equal(innerException.HResult, exception.InnerException.HResult);
        }
    }
}
