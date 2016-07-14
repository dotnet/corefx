namespace System.Security.Permissions
{
    public sealed partial class UIPermission : System.Security.CodeAccessPermission, System.Security.Permissions.IUnrestrictedPermission
    {
        public UIPermission(System.Security.Permissions.PermissionState state) { }
        public UIPermission(System.Security.Permissions.UIPermissionClipboard clipboardFlag) { }
        public UIPermission(System.Security.Permissions.UIPermissionWindow windowFlag) { }
        public UIPermission(System.Security.Permissions.UIPermissionWindow windowFlag, System.Security.Permissions.UIPermissionClipboard clipboardFlag) { }
        public System.Security.Permissions.UIPermissionClipboard Clipboard { get { return default(System.Security.Permissions.UIPermissionClipboard); } set { } }
        public System.Security.Permissions.UIPermissionWindow Window { get { return default(System.Security.Permissions.UIPermissionWindow); } set { } }
        public override System.Security.IPermission Copy() { return default(System.Security.IPermission); }
        //    public override void FromXml(System.Security.SecurityElement esd) { }
        public override System.Security.IPermission Intersect(System.Security.IPermission target) { return default(System.Security.IPermission); }
        public override bool IsSubsetOf(System.Security.IPermission target) { return default(bool); }
        public bool IsUnrestricted() { return default(bool); }
        //    public override System.Security.SecurityElement ToXml() { return default(System.Security.SecurityElement); }
        public override System.Security.IPermission Union(System.Security.IPermission target) { return default(System.Security.IPermission); }
    }
}
