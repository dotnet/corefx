namespace System.Security.Permissions
{
    [System.AttributeUsageAttribute((System.AttributeTargets)(109), AllowMultiple = true, Inherited = false)]
    public sealed partial class UIPermissionAttribute : System.Security.Permissions.CodeAccessSecurityAttribute
    {
        public UIPermissionAttribute(System.Security.Permissions.SecurityAction action) : base(default(System.Security.Permissions.SecurityAction)) { }
        public System.Security.Permissions.UIPermissionClipboard Clipboard { get { return default(System.Security.Permissions.UIPermissionClipboard); } set { } }
        public System.Security.Permissions.UIPermissionWindow Window { get { return default(System.Security.Permissions.UIPermissionWindow); } set { } }
        public override System.Security.IPermission CreatePermission() { return default(System.Security.IPermission); }
    }
}
