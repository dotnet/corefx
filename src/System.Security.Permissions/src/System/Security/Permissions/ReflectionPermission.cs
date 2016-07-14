namespace System.Security.Permissions
{
    public sealed partial class ReflectionPermission : System.Security.CodeAccessPermission, System.Security.Permissions.IUnrestrictedPermission
    {
        public ReflectionPermission(System.Security.Permissions.PermissionState state) { }
        public ReflectionPermission(System.Security.Permissions.ReflectionPermissionFlag flag) { }
        public System.Security.Permissions.ReflectionPermissionFlag Flags { get { return default(System.Security.Permissions.ReflectionPermissionFlag); } set { } }
        public override System.Security.IPermission Copy() { return default(System.Security.IPermission); }
        //    public override void FromXml(System.Security.SecurityElement esd) { }
        public override System.Security.IPermission Intersect(System.Security.IPermission target) { return default(System.Security.IPermission); }
        public override bool IsSubsetOf(System.Security.IPermission target) { return default(bool); }
        public bool IsUnrestricted() { return default(bool); }
        //    public override System.Security.SecurityElement ToXml() { return default(System.Security.SecurityElement); }
        public override System.Security.IPermission Union(System.Security.IPermission other) { return default(System.Security.IPermission); }
    }
}
