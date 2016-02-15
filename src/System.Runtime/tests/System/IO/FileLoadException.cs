// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.IO.Tests
{
    public static class FileLoadExceptionTests
    {
        [Fact]
        public static void TestCtor_Empty()
        {
            var exception = new FileLoadException();
            ExceptionUtility.ValidateExceptionProperties(exception, hResult: HResults.COR_E_FILELOAD, validateMessage: false);
            Assert.Null(exception.FileName);
        }

        [Fact]
        public static void TestCtor_String()
        {
            string message = "this is not the file you're looking for";
            var exception = new FileLoadException(message);
            ExceptionUtility.ValidateExceptionProperties(exception, hResult: HResults.COR_E_FILELOAD, message: message);
            Assert.Null(exception.FileName);
        }

        [Fact]
        public static void TestCtor_String_Exception()
        {
            string message = "this is not the file you're looking for";
            var innerException = new Exception("Inner exception");
            var exception = new FileLoadException(message, innerException);
            ExceptionUtility.ValidateExceptionProperties(exception, hResult: HResults.COR_E_FILELOAD, innerException: innerException, message: message);
            Assert.Equal(null, exception.FileName);
        }

        [Fact]
        public static void TestCtor_String_String()
        {
            string message = "this is not the file you're looking for";
            string fileName = "file.txt";
            var exception = new FileLoadException(message, fileName);
            ExceptionUtility.ValidateExceptionProperties(exception, hResult: HResults.COR_E_FILELOAD, message: message);
            Assert.Equal(fileName, exception.FileName);
        }

        [Fact]
        public static void TestCtor_String_String_Exception()
        {
            string message = "this is not the file you're looking for";
            string fileName = "file.txt";
            var innerException = new Exception("Inner exception");
            var exception = new FileLoadException(message, fileName, innerException);
            ExceptionUtility.ValidateExceptionProperties(exception, hResult: HResults.COR_E_FILELOAD, innerException: innerException, message: message);
            Assert.Equal(fileName, exception.FileName);
        }

        [Fact]
        public static void TestToString()
        {
            string message = "this is not the file you're looking for";
            string fileName = "file.txt";
            var innerException = new Exception("Inner exception");
            var exception = new FileLoadException(message, fileName, innerException);

            var toString = exception.ToString();
            Assert.Contains(": " + message, toString);
            Assert.Contains(": '" + fileName + "'", toString);
            Assert.Contains("---> " + innerException.ToString(), toString);

            // set the stack trace
            try { throw exception; }
            catch
            {
                Assert.False(string.IsNullOrEmpty(exception.StackTrace));
                Assert.Contains(exception.StackTrace, exception.ToString());
            }
        }
    }
}
