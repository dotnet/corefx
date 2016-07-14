using Xunit;

namespace System.Security.Permissions.Tests
{
    public class HostSecurityManagerTests
    {
        [Fact]
        public static void CallMethods()
        {
            HostSecurityManager hsm = new HostSecurityManager();
            Policy.ApplicationTrust at = hsm.DetermineApplicationTrust(new Policy.Evidence(), new Policy.Evidence(), new Policy.TrustManagerContext());
            Policy.Evidence e = hsm.ProvideAppDomainEvidence(new Policy.Evidence());
            PermissionSet ps = hsm.ResolvePolicy(new Policy.Evidence());
        }
    }
}
