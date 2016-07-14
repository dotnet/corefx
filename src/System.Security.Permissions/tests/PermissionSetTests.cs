using Xunit;

namespace System.Security.Permissions.Tests
{
    public class PermissionSetTests
    {
        [Fact]
        public static void PermissionSetCallMethods()
        {
            PermissionSet ps = new PermissionSet(new PermissionState());
            ps.Assert();
            bool containspermissions = ps.ContainsNonCodeAccessPermissions();
            byte[] testbytearray = PermissionSet.ConvertPermissionSet("TestFormat", new byte[1], "TestFormat");
            PermissionSet ps2 = ps.Copy();
            ps.CopyTo(new int[1], 0);
            ps.Demand();
            ps.Deny();
            ps.Equals(ps2);
            System.Collections.IEnumerator ie = ps.GetEnumerator();
            int hash = ps.GetHashCode();
            PermissionSet ps3 = ps.Intersect(ps2);
            bool isempty = ps.IsEmpty();
            bool issubsetof = ps.IsSubsetOf(ps2);
            bool isunrestricted = ps.IsUnrestricted();
            ps.PermitOnly();
            string s = ps.ToString();
            PermissionSet ps4 = ps.Union(ps2);
        }
        [Fact]
        public static void PermissionSetAttributeCallMethods()
        {
            PermissionSetAttribute psa = new PermissionSetAttribute(new Permissions.SecurityAction());
            IPermission ip = psa.CreatePermission();
            PermissionSet ps = psa.CreatePermissionSet();
        }
        [Fact]
        public static void NamedPermissionSetCallMethods()
        {
            NamedPermissionSet nps = new NamedPermissionSet("Test");
            PermissionSet ps = nps.Copy();
            NamedPermissionSet nps2 = nps.Copy("Test");
            nps.Equals(nps2);
            int hash = nps.GetHashCode();
        }
    }
}
