namespace System.Security.Permissions
{
    [System.AttributeUsageAttribute((System.AttributeTargets)(109), AllowMultiple = true, Inherited = false)]
    public sealed partial class PermissionSetAttribute : System.Security.Permissions.CodeAccessSecurityAttribute
    {
        public PermissionSetAttribute(System.Security.Permissions.SecurityAction action) : base(default(System.Security.Permissions.SecurityAction)) { }
        public string File { get { return default(string); } set { } }
        public string Hex { get { return default(string); } set { } }
        public string Name { get { return default(string); } set { } }
        public bool UnicodeEncoded { get { return default(bool); } set { } }
        public string XML { get { return default(string); } set { } }
        public override System.Security.IPermission CreatePermission() { return default(System.Security.IPermission); }
        public System.Security.PermissionSet CreatePermissionSet() { return default(System.Security.PermissionSet); }
    }
}
