namespace System.Security.Permissions{
    [System.AttributeUsageAttribute((System.AttributeTargets)(109), AllowMultiple = true, Inherited = false)]
    public sealed partial class ZoneIdentityPermissionAttribute : System.Security.Permissions.CodeAccessSecurityAttribute
    {
        public ZoneIdentityPermissionAttribute(System.Security.Permissions.SecurityAction action) : base(default(System.Security.Permissions.SecurityAction)) { }
        public System.Security.SecurityZone Zone { get { return default(System.Security.SecurityZone); } set { } }
        public override System.Security.IPermission CreatePermission() { return default(System.Security.IPermission); }
    }
}
