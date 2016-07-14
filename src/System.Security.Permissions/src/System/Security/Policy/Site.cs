namespace System.Security.Policy
{
    public sealed partial class Site : System.Security.Policy.EvidenceBase, System.Security.Policy.IIdentityPermissionFactory
    {
        public Site(string name) { }
        public string Name { get { return default(string); } }
        public object Copy() { return default(object); }
        public static System.Security.Policy.Site CreateFromUrl(string url) { return default(System.Security.Policy.Site); }
        public System.Security.IPermission CreateIdentityPermission(System.Security.Policy.Evidence evidence) { return default(System.Security.IPermission); }
        public override bool Equals(object o) { return default(bool); }
        public override int GetHashCode() { return default(int); }
        public override string ToString() { return default(string); }
    }
}
