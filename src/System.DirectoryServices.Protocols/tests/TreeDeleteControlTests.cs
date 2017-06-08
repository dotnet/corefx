using Xunit;

namespace System.DirectoryServices.Protocols.Tests
{
    public class TreeDeleteControlTests
    {
        [Fact]
        public void Ctor_Default()
        {
            var control = new TreeDeleteControl();
            Assert.True(control.IsCritical);
            Assert.True(control.ServerSide);
            Assert.Equal("1.2.840.113556.1.4.805", control.Type);
            
            Assert.Empty(control.GetValue());
        }
    }
}
