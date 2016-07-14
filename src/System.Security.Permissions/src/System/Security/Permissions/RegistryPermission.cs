namespace System.Security.Permissions
{
    public sealed partial class RegistryPermission : System.Security.CodeAccessPermission, System.Security.Permissions.IUnrestrictedPermission
    {
        public RegistryPermission(System.Security.Permissions.PermissionState state) { }
        public RegistryPermission(System.Security.Permissions.RegistryPermissionAccess access, System.Security.AccessControl.AccessControlActions control, string pathList) { }
        public RegistryPermission(System.Security.Permissions.RegistryPermissionAccess access, string pathList) { }
        public void AddPathList(System.Security.Permissions.RegistryPermissionAccess access, string pathList) { }
        public override System.Security.IPermission Copy() { return default(System.Security.IPermission); }
        //    public override void FromXml(System.Security.SecurityElement esd) { }
        public string GetPathList(System.Security.Permissions.RegistryPermissionAccess access) { return default(string); }
        public override System.Security.IPermission Intersect(System.Security.IPermission target) { return default(System.Security.IPermission); }
        public override bool IsSubsetOf(System.Security.IPermission target) { return default(bool); }
        public bool IsUnrestricted() { return default(bool); }
        public void SetPathList(System.Security.Permissions.RegistryPermissionAccess access, string pathList) { }
        //    public override System.Security.SecurityElement ToXml() { return default(System.Security.SecurityElement); }
        public override System.Security.IPermission Union(System.Security.IPermission other) { return default(System.Security.IPermission); }
    }
}
