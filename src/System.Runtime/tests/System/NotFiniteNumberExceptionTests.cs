// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Xunit;

namespace System.Tests
{
    public static class NotFiniteNumberExceptionTests
    {
        private const int COR_E_NOTFINITENUMBER = -2146233048;
        // TODO: Add tests for other methods
        [Fact]
        public static void Ctor_Empty()
        {
            var exception = new NotFiniteNumberException();
            Assert.NotNull(exception);
            Assert.NotEmpty(exception.Message);
            Assert.Equal(COR_E_NOTFINITENUMBER, exception.HResult);
        }

        [Fact]
        public static void Ctor_String()
        {
            string message = "Created NotFiniteNumberException";
            var exception = new NotFiniteNumberException(message);
            Assert.Equal(message, exception.Message);
            Assert.Equal(COR_E_NOTFINITENUMBER, exception.HResult);
        }

        [Fact]
        public static void Ctor_String_Exception()
        {
            string message = "Created NotFiniteNumberException";
            var innerException = new Exception("Created inner exception");
            var exception = new NotFiniteNumberException(message, innerException);
            Assert.Equal(message, exception.Message);
            Assert.Equal(COR_E_NOTFINITENUMBER, exception.HResult);
            Assert.Same(innerException, exception.InnerException);
            Assert.Equal(innerException.HResult, exception.InnerException.HResult);
        }
    }
}
