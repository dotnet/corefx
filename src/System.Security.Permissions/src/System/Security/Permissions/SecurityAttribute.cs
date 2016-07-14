namespace System.Security.Permissions
{
    [System.AttributeUsageAttribute((System.AttributeTargets)(109), AllowMultiple = true, Inherited = false)]
    public abstract partial class SecurityAttribute : System.Attribute
    {
        protected SecurityAttribute(System.Security.Permissions.SecurityAction action) { }
        public System.Security.Permissions.SecurityAction Action { get { return default(System.Security.Permissions.SecurityAction); } set { } }
        public bool Unrestricted { get { return default(bool); } set { } }
        public abstract System.Security.IPermission CreatePermission();
    }
}
