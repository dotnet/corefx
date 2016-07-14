using Xunit;

namespace System.Security.Permissions.Tests
{
    public class CodeConnectAccessTests
    {
        [Fact]
        public static void CodeConnectAccessCallMethods()
        {
            Policy.CodeConnectAccess cca = new Policy.CodeConnectAccess("test", 0);
            string teststring = Policy.CodeConnectAccess.AnyScheme;
            int testint = Policy.CodeConnectAccess.DefaultPort;
            testint = Policy.CodeConnectAccess.OriginPort;
            teststring = Policy.CodeConnectAccess.OriginScheme;
            cca = Policy.CodeConnectAccess.CreateAnySchemeAccess(0);
            cca = Policy.CodeConnectAccess.CreateOriginSchemeAccess(0);
            cca = new Policy.CodeConnectAccess("test", 0);
            bool testbool = cca.Equals(new object());
            testint = cca.GetHashCode();
        }
    }
}
