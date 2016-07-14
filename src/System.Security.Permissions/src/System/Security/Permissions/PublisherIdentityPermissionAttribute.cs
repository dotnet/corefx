namespace System.Security.Permissions
{
    [System.AttributeUsageAttribute((System.AttributeTargets)(109), AllowMultiple = true, Inherited = false)]
    public sealed partial class PublisherIdentityPermissionAttribute : System.Security.Permissions.CodeAccessSecurityAttribute
    {
        public PublisherIdentityPermissionAttribute(System.Security.Permissions.SecurityAction action) : base(default(System.Security.Permissions.SecurityAction)) { }
        public string CertFile { get { return default(string); } set { } }
        public string SignedFile { get { return default(string); } set { } }
        public string X509Certificate { get { return default(string); } set { } }
        public override System.Security.IPermission CreatePermission() { return default(System.Security.IPermission); }
    }
}
