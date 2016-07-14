namespace System.Security.Permissions
{
    [System.AttributeUsageAttribute((System.AttributeTargets)(109), AllowMultiple = true, Inherited = false)]
    public abstract partial class CodeAccessSecurityAttribute : System.Security.Permissions.SecurityAttribute
    {
        protected CodeAccessSecurityAttribute(System.Security.Permissions.SecurityAction action) : base(default(System.Security.Permissions.SecurityAction)) { }
    }
}
