namespace System.Security.Policy
{
    public sealed partial class ApplicationTrust : System.Security.Policy.EvidenceBase, System.Security.ISecurityEncodable
    {
        public ApplicationTrust() { }
        //    public ApplicationTrust(System.ApplicationIdentity applicationIdentity) { }
        public ApplicationTrust(System.Security.PermissionSet defaultGrantSet, System.Collections.Generic.IEnumerable<System.Security.Policy.StrongName> fullTrustAssemblies) { }
        //    public System.ApplicationIdentity ApplicationIdentity { get { return default(System.ApplicationIdentity); } set { } }
        public System.Security.Policy.PolicyStatement DefaultGrantSet { get { return default(System.Security.Policy.PolicyStatement); } set { } }
        public object ExtraInfo { get { return default(object); } set { } }
        public System.Collections.Generic.IList<System.Security.Policy.StrongName> FullTrustAssemblies { get { return default(System.Collections.Generic.IList<System.Security.Policy.StrongName>); } }
        public bool IsApplicationTrustedToRun { get { return default(bool); } set { } }
        public bool Persist { get { return default(bool); } set { } }
        //    public void FromXml(System.Security.SecurityElement element) { }
        //    public System.Security.SecurityElement ToXml() { return default(System.Security.SecurityElement); }
    }
}
