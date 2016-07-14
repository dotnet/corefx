namespace System.Security.Permissions
{
    public sealed partial class FileIOPermission : System.Security.CodeAccessPermission, System.Security.Permissions.IUnrestrictedPermission
    {
        public FileIOPermission(System.Security.Permissions.FileIOPermissionAccess access, string path) { }
        public FileIOPermission(System.Security.Permissions.FileIOPermissionAccess access, string[] pathList) { }
        public FileIOPermission(System.Security.Permissions.PermissionState state) { }
        public System.Security.Permissions.FileIOPermissionAccess AllFiles { get { return default(System.Security.Permissions.FileIOPermissionAccess); } set { } }
        public System.Security.Permissions.FileIOPermissionAccess AllLocalFiles { get { return default(System.Security.Permissions.FileIOPermissionAccess); } set { } }
        public void AddPathList(System.Security.Permissions.FileIOPermissionAccess access, string path) { }
        public void AddPathList(System.Security.Permissions.FileIOPermissionAccess access, string[] pathList) { }
        public override System.Security.IPermission Copy() { return default(System.Security.IPermission); }
        public override bool Equals(object obj) { return default(bool); }
        //    public override void FromXml(System.Security.SecurityElement esd) { }
        public override int GetHashCode() { return default(int); }
        public string[] GetPathList(System.Security.Permissions.FileIOPermissionAccess access) { return default(string[]); }
        public override System.Security.IPermission Intersect(System.Security.IPermission target) { return default(System.Security.IPermission); }
        public override bool IsSubsetOf(System.Security.IPermission target) { return default(bool); }
        public bool IsUnrestricted() { return default(bool); }
        public void SetPathList(System.Security.Permissions.FileIOPermissionAccess access, string path) { }
        public void SetPathList(System.Security.Permissions.FileIOPermissionAccess access, string[] pathList) { }
        //    public override System.Security.SecurityElement ToXml() { return default(System.Security.SecurityElement); }
        public override System.Security.IPermission Union(System.Security.IPermission other) { return default(System.Security.IPermission); }
    }
}
