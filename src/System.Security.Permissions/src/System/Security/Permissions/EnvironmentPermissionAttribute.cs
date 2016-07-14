namespace System.Security.Permissions
{
    [System.AttributeUsageAttribute((System.AttributeTargets)(109), AllowMultiple = true, Inherited = false)]
    public sealed partial class EnvironmentPermissionAttribute : System.Security.Permissions.CodeAccessSecurityAttribute
    {
        public EnvironmentPermissionAttribute(System.Security.Permissions.SecurityAction action) : base(default(System.Security.Permissions.SecurityAction)) { }
        public string All { get { return default(string); } set { } }
        public string Read { get { return default(string); } set { } }
        public string Write { get { return default(string); } set { } }
        public override System.Security.IPermission CreatePermission() { return default(System.Security.IPermission); }
    }
}
