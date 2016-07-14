namespace System.Security.Policy
{
    public sealed partial class Zone : System.Security.Policy.EvidenceBase, System.Security.Policy.IIdentityPermissionFactory
    {
        public Zone(System.Security.SecurityZone zone) { }
        public System.Security.SecurityZone SecurityZone { get { return default(System.Security.SecurityZone); } }
        public object Copy() { return default(object); }
        public static System.Security.Policy.Zone CreateFromUrl(string url) { return default(System.Security.Policy.Zone); }
        public System.Security.IPermission CreateIdentityPermission(System.Security.Policy.Evidence evidence) { return default(System.Security.IPermission); }
        public override bool Equals(object o) { return default(bool); }
        public override int GetHashCode() { return default(int); }
        public override string ToString() { return default(string); }
    }
}
