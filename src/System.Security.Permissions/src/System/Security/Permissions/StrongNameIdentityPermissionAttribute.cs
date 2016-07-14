namespace System.Security.Permissions
{
    [System.AttributeUsageAttribute((System.AttributeTargets)(109), AllowMultiple = true, Inherited = false)]
    public sealed partial class StrongNameIdentityPermissionAttribute : System.Security.Permissions.CodeAccessSecurityAttribute
    {
        public StrongNameIdentityPermissionAttribute(System.Security.Permissions.SecurityAction action) : base(default(System.Security.Permissions.SecurityAction)) { }
        public string Name { get { return default(string); } set { } }
        public string PublicKey { get { return default(string); } set { } }
        public string Version { get { return default(string); } set { } }
        public override System.Security.IPermission CreatePermission() { return default(System.Security.IPermission); }
    }
}
