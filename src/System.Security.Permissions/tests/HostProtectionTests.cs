using Xunit;

namespace System.Security.Permissions.Tests
{
    public class HostProtectionTests
    {
        [Fact]
        public static void HostProtectionExceptionCallMethods()
        {
            HostProtectionException hpe = new HostProtectionException();
            hpe.ToString();
        }
        [Fact]
        public static void HostProtectionAttributeCallMethods()
        {
            HostProtectionAttribute hpa = new HostProtectionAttribute();
            IPermission ip = hpa.CreatePermission();
        }
    }
}
