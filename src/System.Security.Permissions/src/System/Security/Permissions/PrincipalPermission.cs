namespace System.Security.Permissions
{
    public sealed partial class PrincipalPermission : System.Security.IPermission, System.Security.ISecurityEncodable, System.Security.Permissions.IUnrestrictedPermission
    {
        public PrincipalPermission(System.Security.Permissions.PermissionState state) { }
        public PrincipalPermission(string name, string role) { }
        public PrincipalPermission(string name, string role, bool isAuthenticated) { }
        public System.Security.IPermission Copy() { return default(System.Security.IPermission); }
        public void Demand() { }
        public override bool Equals(object obj) { return default(bool); }
        //    public void FromXml(System.Security.SecurityElement elem) { }
        public override int GetHashCode() { return default(int); }
        public System.Security.IPermission Intersect(System.Security.IPermission target) { return default(System.Security.IPermission); }
        public bool IsSubsetOf(System.Security.IPermission target) { return default(bool); }
        public bool IsUnrestricted() { return default(bool); }
        public override string ToString() { return default(string); }
        //    public System.Security.SecurityElement ToXml() { return default(System.Security.SecurityElement); }
        public System.Security.IPermission Union(System.Security.IPermission other) { return default(System.Security.IPermission); }
    }
}
