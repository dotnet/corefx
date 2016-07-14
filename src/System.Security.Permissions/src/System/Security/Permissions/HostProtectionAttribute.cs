namespace System.Security.Permissions
{
    [System.AttributeUsageAttribute((System.AttributeTargets)(4205), AllowMultiple = true, Inherited = false)]
    public sealed partial class HostProtectionAttribute : System.Security.Permissions.CodeAccessSecurityAttribute
    {
        public HostProtectionAttribute() : base(default(System.Security.Permissions.SecurityAction)) { }
        public HostProtectionAttribute(System.Security.Permissions.SecurityAction action) : base(default(System.Security.Permissions.SecurityAction)) { }
        public bool ExternalProcessMgmt { get { return default(bool); } set { } }
        public bool ExternalThreading { get { return default(bool); } set { } }
        public bool MayLeakOnAbort { get { return default(bool); } set { } }
        public System.Security.Permissions.HostProtectionResource Resources { get { return default(System.Security.Permissions.HostProtectionResource); } set { } }
        public bool SecurityInfrastructure { get { return default(bool); } set { } }
        public bool SelfAffectingProcessMgmt { get { return default(bool); } set { } }
        public bool SelfAffectingThreading { get { return default(bool); } set { } }
        public bool SharedState { get { return default(bool); } set { } }
        public bool Synchronization { get { return default(bool); } set { } }
        public bool UI { get { return default(bool); } set { } }
        public override System.Security.IPermission CreatePermission() { return default(System.Security.IPermission); }
    }
}
