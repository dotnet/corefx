namespace System.Security.Permissions
{
    [System.AttributeUsageAttribute((System.AttributeTargets)(109), AllowMultiple = true, Inherited = false)]
    public sealed partial class SecurityPermissionAttribute : System.Security.Permissions.CodeAccessSecurityAttribute
    {
        public SecurityPermissionAttribute(System.Security.Permissions.SecurityAction action) : base(default(System.Security.Permissions.SecurityAction)) { }
        public bool Assertion { get { return default(bool); } set { } }
        public bool BindingRedirects { get { return default(bool); } set { } }
        public bool ControlAppDomain { get { return default(bool); } set { } }
        public bool ControlDomainPolicy { get { return default(bool); } set { } }
        public bool ControlEvidence { get { return default(bool); } set { } }
        public bool ControlPolicy { get { return default(bool); } set { } }
        public bool ControlPrincipal { get { return default(bool); } set { } }
        public bool ControlThread { get { return default(bool); } set { } }
        public bool Execution { get { return default(bool); } set { } }
        public System.Security.Permissions.SecurityPermissionFlag Flags { get { return default(System.Security.Permissions.SecurityPermissionFlag); } set { } }
        public bool Infrastructure { get { return default(bool); } set { } }
        public bool RemotingConfiguration { get { return default(bool); } set { } }
        public bool SerializationFormatter { get { return default(bool); } set { } }
        public bool SkipVerification { get { return default(bool); } set { } }
        public bool UnmanagedCode { get { return default(bool); } set { } }
        public override System.Security.IPermission CreatePermission() { return default(System.Security.IPermission); }
    }
}
