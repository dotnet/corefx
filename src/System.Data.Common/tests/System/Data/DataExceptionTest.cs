// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Data.Tests
{
    public class DataExceptionTest
    {
        [Fact]
        public void DefaultArgumentsShowed()
        {
            var defaultException = new CustomDataException();
            Assert.Equal("Data Exception.", defaultException.Message);
            Assert.Null(defaultException.InnerException);
            Assert.Equal(-2146232032, defaultException.HResult);
        }

        [Fact]
        public void ArgumentsStored()
        {
            var dataException = new CustomDataException("test", new Exception("inner exception"));
            Assert.Equal("test", dataException.Message);
            Assert.Equal("inner exception", dataException.InnerException.Message);
            Assert.Equal(-2146233087, dataException.HResult);
        }

        [Fact]
        public void ArgumentsFromSerializationInfo()
        {
            var si = new System.Runtime.Serialization.SerializationInfo(typeof(CustomDataException), new System.Runtime.Serialization.FormatterConverter());
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

            var exFromSerializationInfo = new CustomDataException(si, sc);

            Assert.Equal(string.Empty, exFromSerializationInfo.Message);
            Assert.IsType<ArgumentException>(exFromSerializationInfo.InnerException);
            Assert.Equal(1, exFromSerializationInfo.HResult);
        }

        [Serializable]
        private class CustomDataException : DataException
        {
            public CustomDataException() : base()
            {
            }

            public CustomDataException(string message) : base(message)
            {
            }

            public CustomDataException(string message, Exception innerException) : base(message, innerException)
            {
            }

            public CustomDataException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
                : base(info, context)
            {
                // For exposing the protected constructor
            }
        }
    }
}
