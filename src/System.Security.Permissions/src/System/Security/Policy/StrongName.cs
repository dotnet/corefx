namespace System.Security.Policy
{
    public sealed partial class StrongName : System.Security.Policy.EvidenceBase, System.Security.Policy.IIdentityPermissionFactory
    {
        public StrongName(System.Security.Permissions.StrongNamePublicKeyBlob blob, string name, System.Version version) { }
        public string Name { get { return default(string); } }
        public System.Security.Permissions.StrongNamePublicKeyBlob PublicKey { get { return default(System.Security.Permissions.StrongNamePublicKeyBlob); } }
        public System.Version Version { get { return default(System.Version); } }
        public object Copy() { return default(object); }
        public System.Security.IPermission CreateIdentityPermission(System.Security.Policy.Evidence evidence) { return default(System.Security.IPermission); }
        public override bool Equals(object o) { return default(bool); }
        public override int GetHashCode() { return default(int); }
        public override string ToString() { return default(string); }
    }
}
