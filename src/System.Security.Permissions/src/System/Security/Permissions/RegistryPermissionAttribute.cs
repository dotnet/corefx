namespace System.Security.Permissions
{
    [System.AttributeUsageAttribute((System.AttributeTargets)(109), AllowMultiple = true, Inherited = false)]
    public sealed partial class RegistryPermissionAttribute : System.Security.Permissions.CodeAccessSecurityAttribute
    {
        public RegistryPermissionAttribute(System.Security.Permissions.SecurityAction action) : base(default(System.Security.Permissions.SecurityAction)) { }
        [System.ObsoleteAttribute("use newer properties")]
        public string All { get { return default(string); } set { } }
        public string ChangeAccessControl { get { return default(string); } set { } }
        public string Create { get { return default(string); } set { } }
        public string Read { get { return default(string); } set { } }
        public string ViewAccessControl { get { return default(string); } set { } }
        public string ViewAndModify { get { return default(string); } set { } }
        public string Write { get { return default(string); } set { } }
        public override System.Security.IPermission CreatePermission() { return default(System.Security.IPermission); }
    }
}
