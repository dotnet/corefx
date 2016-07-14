using Xunit;

namespace System.Security.Permissions.Tests
{
    public class PolicyTests
    {
        [Fact]
        public static void PolicyExceptionCallMethods()
        {
            Policy.PolicyException pe = new Policy.PolicyException();
            pe = new Policy.PolicyException("test");
        }
        [Fact]
        public static void PolicyLevelCallMethods()
        {
            Policy.PolicyLevel pl = (Policy.PolicyLevel)Activator.CreateInstance(typeof(Policy.PolicyLevel), true);
            NamedPermissionSet nps = new NamedPermissionSet("test");
            pl.AddNamedPermissionSet(nps);
            nps = pl.ChangeNamedPermissionSet("test", new PermissionSet(new Permissions.PermissionState()));
            Policy.PolicyLevel.CreateAppDomainLevel();
            nps = pl.GetNamedPermissionSet("test");
            pl.Recover();
            NamedPermissionSet nps2 = pl.RemoveNamedPermissionSet(nps);
            nps2 = pl.RemoveNamedPermissionSet("test");
            pl.Reset();
            Policy.Evidence evidence = new Policy.Evidence();
            Policy.PolicyStatement ps = pl.Resolve(evidence);
            Policy.CodeGroup cg = pl.ResolveMatchingCodeGroups(evidence);
        }
    [Fact]
        public static void PolicyStatementCallMethods()
        {
            Policy.PolicyStatement ps = new Policy.PolicyStatement(new PermissionSet(new PermissionState()));
            Policy.PolicyStatement ps2 = ps.Copy();
            bool equals = ps.Equals(ps2);
            int hash = ps.GetHashCode();
        }
    }
}
