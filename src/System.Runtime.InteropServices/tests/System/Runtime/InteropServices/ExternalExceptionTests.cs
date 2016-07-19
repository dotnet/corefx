
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Xunit;

namespace System.Runtime.InteropServices.Tests
{
    public static class ExternalExceptionTests
    {
        private const int COR_E_EXTERNALEXCEPTION = unchecked((int)0x80004005);

        [Fact]
        public static void Ctor_Empty()
        {
            ExternalException exception = new ExternalException();
            Assert.Equal(-2147467259, exception.ErrorCode);
            Assert.Null(exception.InnerException);
            Assert.NotNull(exception.Message);
            Assert.NotEmpty(exception.Message);
            Assert.True(exception.Message.IndexOf(exception.GetType().FullName) == -1);
            Assert.Equal(COR_E_EXTERNALEXCEPTION, exception.HResult);
        }

        [Fact]
        public static void Ctor_String()
        {
            string message = "Created ExternalException";
            ExternalException exception;

            exception = new ExternalException(message);
            Assert.Equal(-2147467259, exception.ErrorCode);
            Assert.Null(exception.InnerException);
            Assert.Same(message, exception.Message);

            exception = new ExternalException((string)null);
            Assert.Equal(-2147467259, exception.ErrorCode);
            Assert.Null(exception.InnerException);
            Assert.NotNull(exception.Message);
            Assert.True(exception.Message.IndexOf(exception.GetType().FullName) != -1);

            exception = new ExternalException(string.Empty);
            Assert.Equal(-2147467259, exception.ErrorCode);
            Assert.Null(exception.InnerException);
            Assert.NotNull(exception.Message);
            Assert.Equal(string.Empty, exception.Message);
        }

        [Fact]
        public static void Ctor_String_Exception()
        {
            string message = "Created ExternalException";
            var innerException = new Exception("Created inner exception");
            var exception = new ExternalException(message, innerException);
            Assert.Equal(message, exception.Message);
            Assert.Equal(COR_E_EXTERNALEXCEPTION, exception.HResult);
            Assert.Same(innerException, exception.InnerException);
            Assert.Equal(innerException.HResult, exception.InnerException.HResult);
        }

        public static void Ctor_String_int()
        {
            ExternalException ex;
            string msg = "ERROR";

            ex = new ExternalException(msg, int.MinValue);
            Assert.Equal(int.MinValue, ex.ErrorCode);
        }

    }
}