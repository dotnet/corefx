using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Xunit;

namespace System.DirectoryServices.Protocols.Tests
{
    public class DirectoryExceptionTests
    {
        [Fact]
        public void Ctor_Default()
        {
            var exception = new DirectoryException();
            Assert.NotEmpty(exception.Message);
            Assert.Null(exception.InnerException);
        }

        [Fact]
        public void Ctor_Message()
        {
            var exception = new DirectoryException("message");
            Assert.Equal("message", exception.Message);
            Assert.Null(exception.InnerException);
        }

        [Fact]
        public void Ctor_Message_InnerException()
        {
            var innerException = new Exception();
            var exception = new DirectoryException("message", innerException);
            Assert.Equal("message", exception.Message);
            Assert.Same(innerException, exception.InnerException);
        }

        [Fact]
        public void SubClass_Deserialize_ThrowsPlatformNotSupportedException()
        {
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, new SubException());

                stream.Position = 0;
                TargetInvocationException ex = Assert.Throws<TargetInvocationException>(() => formatter.Deserialize(stream));
                Assert.IsType<PlatformNotSupportedException>(ex.InnerException);
            }
        }

        [Serializable]
        public class SubException : DirectoryException
        {
            public SubException() : base() { }
            public SubException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        }
    }
}
