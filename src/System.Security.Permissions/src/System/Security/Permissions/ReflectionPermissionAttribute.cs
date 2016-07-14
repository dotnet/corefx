namespace System.Security.Permissions
{
    [System.AttributeUsageAttribute((System.AttributeTargets)(109), AllowMultiple = true, Inherited = false)]
    public sealed partial class ReflectionPermissionAttribute : System.Security.Permissions.CodeAccessSecurityAttribute
    {
        public ReflectionPermissionAttribute(System.Security.Permissions.SecurityAction action) : base(default(System.Security.Permissions.SecurityAction)) { }
        public System.Security.Permissions.ReflectionPermissionFlag Flags { get { return default(System.Security.Permissions.ReflectionPermissionFlag); } set { } }
        public bool MemberAccess { get { return default(bool); } set { } }
        [System.ObsoleteAttribute]
        public bool ReflectionEmit { get { return default(bool); } set { } }
        public bool RestrictedMemberAccess { get { return default(bool); } set { } }
        [System.ObsoleteAttribute("not enforced in 2.0+")]
        public bool TypeInformation { get { return default(bool); } set { } }
        public override System.Security.IPermission CreatePermission() { return default(System.Security.IPermission); }
    }
}
