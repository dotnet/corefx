namespace System.Security.Permissions
{
    [System.AttributeUsageAttribute((System.AttributeTargets)(68), AllowMultiple = true, Inherited = false)]
    public sealed partial class PrincipalPermissionAttribute : System.Security.Permissions.CodeAccessSecurityAttribute
    {
        public PrincipalPermissionAttribute(System.Security.Permissions.SecurityAction action) : base(default(System.Security.Permissions.SecurityAction)) { }
        public bool Authenticated { get { return default(bool); } set { } }
        public string Name { get { return default(string); } set { } }
        public string Role { get { return default(string); } set { } }
        public override System.Security.IPermission CreatePermission() { return default(System.Security.IPermission); }
    }
}
