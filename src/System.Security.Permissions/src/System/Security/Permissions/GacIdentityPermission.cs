namespace System.Security.Permissions
{
    public sealed partial class GacIdentityPermission : System.Security.CodeAccessPermission
    {
        public GacIdentityPermission() { }
        public GacIdentityPermission(System.Security.Permissions.PermissionState state) { }
        public override System.Security.IPermission Copy() { return default(System.Security.IPermission); }
        //    public override void FromXml(System.Security.SecurityElement securityElement) { }
        public override System.Security.IPermission Intersect(System.Security.IPermission target) { return default(System.Security.IPermission); }
        public override bool IsSubsetOf(System.Security.IPermission target) { return default(bool); }
        //    public override System.Security.SecurityElement ToXml() { return default(System.Security.SecurityElement); }
        public override System.Security.IPermission Union(System.Security.IPermission target) { return default(System.Security.IPermission); }
    }
}
