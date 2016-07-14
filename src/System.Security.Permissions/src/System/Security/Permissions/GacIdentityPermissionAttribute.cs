namespace System.Security.Permissions
{
    [System.AttributeUsageAttribute((System.AttributeTargets)(109), AllowMultiple = true, Inherited = false)]
    public sealed partial class GacIdentityPermissionAttribute : System.Security.Permissions.CodeAccessSecurityAttribute
    {
        public GacIdentityPermissionAttribute(System.Security.Permissions.SecurityAction action) : base(default(System.Security.Permissions.SecurityAction)) { }
        public override System.Security.IPermission CreatePermission() { return default(System.Security.IPermission); }
    }
}
