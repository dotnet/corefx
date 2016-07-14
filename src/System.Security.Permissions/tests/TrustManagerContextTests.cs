using Xunit;

namespace System.Security.Permissions.Tests
{
    public class TrustManagerContextTests
    {
        [Fact]
        public static void TrustManagerContextCallMethods()
        {
            Policy.TrustManagerContext tmc = new Policy.TrustManagerContext();
            tmc = new Policy.TrustManagerContext(new Policy.TrustManagerUIContext());
        }
    }
}
