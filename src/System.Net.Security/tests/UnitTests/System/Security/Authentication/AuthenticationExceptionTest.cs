using Xunit;

namespace System.Security.Authentication
{
    public class AuthenticationExceptionTest
    {
        [Fact]
        public void Constructor()
        {
            AuthenticationException authenticationException = new AuthenticationException();

            Assert.Equal("System error.", authenticationException.Message);
        }

        [Fact]
        public void Constructor_String()
        {
            AuthenticationException authenticationException = new AuthenticationException("base was called");

            Assert.Equal("base was called", authenticationException.Message);
        }

        [Fact]
        public void Constructor_String_Exception()
        {
            AuthenticationException authenticationException = new AuthenticationException("base was called", new Exception("this is the inner exception message"));

            Assert.Equal("base was called", authenticationException.Message);
            Assert.Equal("this is the inner exception message", authenticationException.InnerException.Message);
        }
    }
}