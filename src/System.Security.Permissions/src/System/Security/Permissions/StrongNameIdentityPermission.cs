namespace System.Security.Permissions
{
    public sealed partial class StrongNameIdentityPermission : System.Security.CodeAccessPermission
    {
        public StrongNameIdentityPermission(System.Security.Permissions.PermissionState state) { }
        public StrongNameIdentityPermission(System.Security.Permissions.StrongNamePublicKeyBlob blob, string name, System.Version version) { }
        public string Name { get { return default(string); } set { } }
        public System.Security.Permissions.StrongNamePublicKeyBlob PublicKey { get { return default(System.Security.Permissions.StrongNamePublicKeyBlob); } set { } }
        public System.Version Version { get { return default(System.Version); } set { } }
        public override System.Security.IPermission Copy() { return default(System.Security.IPermission); }
        //    public override void FromXml(System.Security.SecurityElement e) { }
        public override System.Security.IPermission Intersect(System.Security.IPermission target) { return default(System.Security.IPermission); }
        public override bool IsSubsetOf(System.Security.IPermission target) { return default(bool); }
        //    public override System.Security.SecurityElement ToXml() { return default(System.Security.SecurityElement); }
        public override System.Security.IPermission Union(System.Security.IPermission target) { return default(System.Security.IPermission); }
    }
}
