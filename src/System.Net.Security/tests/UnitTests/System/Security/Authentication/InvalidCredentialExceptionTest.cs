using Xunit;

namespace System.Security.Authentication
{
    public class InvalidCredentialExceptionTest
    {
        [Fact]
        public void Constructor()
        {
            InvalidCredentialException invalidCredentialException = new InvalidCredentialException();

            Assert.Equal("System error.", invalidCredentialException.Message);
        }

        [Fact]
        public void Constructor_String()
        {
            InvalidCredentialException invalidCredentialException = new InvalidCredentialException("base was called");

            Assert.Equal("base was called", invalidCredentialException.Message);
        }

        [Fact]
        public void Constructor_String_Exception()
        {
            InvalidCredentialException invalidCredentialException = new InvalidCredentialException("base was called", new Exception("this is the inner exception message"));

            Assert.Equal("base was called", invalidCredentialException.Message);
            Assert.Equal("this is the inner exception message", invalidCredentialException.InnerException.Message);
        }
    }
}