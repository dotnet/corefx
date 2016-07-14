namespace System.Security.Permissions
{
    public sealed partial class FileDialogPermission : System.Security.CodeAccessPermission, System.Security.Permissions.IUnrestrictedPermission
    {
        public FileDialogPermission(System.Security.Permissions.FileDialogPermissionAccess access) { }
        public FileDialogPermission(System.Security.Permissions.PermissionState state) { }
        public System.Security.Permissions.FileDialogPermissionAccess Access { get { return default(System.Security.Permissions.FileDialogPermissionAccess); } set { } }
        public override System.Security.IPermission Copy() { return default(System.Security.IPermission); }
        //    public override void FromXml(System.Security.SecurityElement esd) { }
        public override System.Security.IPermission Intersect(System.Security.IPermission target) { return default(System.Security.IPermission); }
        public override bool IsSubsetOf(System.Security.IPermission target) { return default(bool); }
        public bool IsUnrestricted() { return default(bool); }
        //    public override System.Security.SecurityElement ToXml() { return default(System.Security.SecurityElement); }
        public override System.Security.IPermission Union(System.Security.IPermission target) { return default(System.Security.IPermission); }
    }
}
