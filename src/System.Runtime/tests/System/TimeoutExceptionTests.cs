// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using ExceptionUtility = System.Tests.ExceptionUtility;

namespace System.Tests
{
    public static class TimeoutExceptionTests
    {
        private const int COR_E_TIMEOUT = unchecked((int)0x80131516);

        [Fact]
        public static void Ctor_Empty()
        {
            var exception = new TimeoutException();
            ExceptionUtility.ValidateExceptionProperties(exception, hResult: COR_E_TIMEOUT, validateMessage: false);
        }

        [Fact]
        public static void Ctor_String()
        {
            string message = "overflow";
            var exception = new TimeoutException(message);
            ExceptionUtility.ValidateExceptionProperties(exception, hResult: COR_E_TIMEOUT, message: message);
        }

        [Fact]
        public static void Ctor_String_Exception()
        {
            string message = "overflow";
            var innerException = new Exception("Inner exception");
            var exception = new TimeoutException(message, innerException);
            ExceptionUtility.ValidateExceptionProperties(exception, hResult: COR_E_TIMEOUT, innerException: innerException, message: message);
        }
    }
}
