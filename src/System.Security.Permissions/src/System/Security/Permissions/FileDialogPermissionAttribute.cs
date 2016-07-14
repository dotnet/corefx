namespace System.Security.Permissions
{
    [System.AttributeUsageAttribute((System.AttributeTargets)(109), AllowMultiple = true, Inherited = false)]
    public sealed partial class FileDialogPermissionAttribute : System.Security.Permissions.CodeAccessSecurityAttribute
    {
        public FileDialogPermissionAttribute(System.Security.Permissions.SecurityAction action) : base(default(System.Security.Permissions.SecurityAction)) { }
        public bool Open { get { return default(bool); } set { } }
        public bool Save { get { return default(bool); } set { } }
        public override System.Security.IPermission CreatePermission() { return default(System.Security.IPermission); }
    }
}
