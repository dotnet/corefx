// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Runtime.CompilerServices.Tests
{
    public static class RuntimeWrappedExceptionTests
    {
        private const int COR_E_RUNTIMEWRAPPED = -2146233026;

        [Fact]
        public static void Ctor_Null()
        {
            var exception = new RuntimeWrappedException(null);
            Assert.NotNull(exception);
            Assert.NotEmpty(exception.Message);
            Assert.Equal(COR_E_RUNTIMEWRAPPED, exception.HResult);
            Assert.Null(exception.WrappedException);
        }

        [Fact]
        public static void Ctor_Exception()
        {
            var wrappedException = new Exception("Created inner exception");
            var exception = new RuntimeWrappedException(wrappedException);
            Assert.NotNull(exception);
            Assert.NotEmpty(exception.Message);
            Assert.Equal(COR_E_RUNTIMEWRAPPED, exception.HResult);
            Assert.Equal(wrappedException, exception.WrappedException);
        }

        [Fact]
        public static void Ctor_Not_Exception()
        {
            var runtimeWrappedExceptionArgument = 1;
            var exception = new RuntimeWrappedException(runtimeWrappedExceptionArgument);
            Assert.NotNull(exception);
            Assert.NotEmpty(exception.Message);
            Assert.Equal(COR_E_RUNTIMEWRAPPED, exception.HResult);
            Assert.Equal(runtimeWrappedExceptionArgument, exception.WrappedException);
        }
    }
}
