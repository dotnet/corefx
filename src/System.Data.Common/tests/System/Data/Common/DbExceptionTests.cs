// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Data.Common.Tests
{
    public class DbExceptionTests
    {
        [Fact]
        public void DefaultArgumentsShowed()
        {
            var defaultException = new CustomDbException();
            Assert.Equal("External component has thrown an exception.", defaultException.Message);
            Assert.Null(defaultException.InnerException);
            Assert.Equal(-2147467259, defaultException.ErrorCode);
        }

        [Fact]
        public void ArgumentsStored()
        {
            Assert.Equal("test", new CustomDbException("test").Message);
            Assert.Equal("another test", new CustomDbException("test", new Exception("another test")).InnerException.Message);
            Assert.Equal(4060, new CustomDbException("Login failed", 4060).ErrorCode);
        }

        [Fact]
        public void ArgumentsFromSerializationInfo()
        {
            var si = new System.Runtime.Serialization.SerializationInfo(typeof(CustomDbException), new System.Runtime.Serialization.FormatterConverter());
            si.AddValue("ClassName", string.Empty);
            si.AddValue("Message", string.Empty);
            si.AddValue("InnerException", new ArgumentException());
            si.AddValue("HelpURL", string.Empty);
            si.AddValue("StackTraceString", string.Empty);
            si.AddValue("RemoteStackTraceString", string.Empty);
            si.AddValue("RemoteStackIndex", 0);
            si.AddValue("ExceptionMethod", string.Empty);
            si.AddValue("HResult", 1);
            si.AddValue("Source", string.Empty);

            var sc = new System.Runtime.Serialization.StreamingContext();

            var exFromSerializationInfo = new CustomDbException(si, sc);

            Assert.Equal(string.Empty, exFromSerializationInfo.Message);
            Assert.IsType<ArgumentException>(exFromSerializationInfo.InnerException);
            Assert.Equal(1, exFromSerializationInfo.ErrorCode);
        }


        private class CustomDbException : DbException
        {
            public CustomDbException() : base()
            {
            }

            public CustomDbException(string message) : base(message)
            {
            }

            public CustomDbException(string message, int errorCode) : base(message, errorCode)
            {
            }

            public CustomDbException(string message, Exception innerException) : base(message, innerException)
            {
            }

            public CustomDbException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) 
                : base(info, context)
            {
            }
        }
    }
}
