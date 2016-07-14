namespace System.Security.Policy
{
    public sealed partial class StrongNameMembershipCondition : System.Security.ISecurityEncodable, System.Security.ISecurityPolicyEncodable, System.Security.Policy.IMembershipCondition
    {
        public StrongNameMembershipCondition(System.Security.Permissions.StrongNamePublicKeyBlob blob, string name, System.Version version) { }
        public string Name { get { return default(string); } set { } }
        public System.Security.Permissions.StrongNamePublicKeyBlob PublicKey { get { return default(System.Security.Permissions.StrongNamePublicKeyBlob); } set { } }
        public System.Version Version { get { return default(System.Version); } set { } }
        public bool Check(System.Security.Policy.Evidence evidence) { return default(bool); }
        public System.Security.Policy.IMembershipCondition Copy() { return default(System.Security.Policy.IMembershipCondition); }
        public override bool Equals(object o) { return default(bool); }
        //    public void FromXml(System.Security.SecurityElement e) { }
        //    public void FromXml(System.Security.SecurityElement e, System.Security.Policy.PolicyLevel level) { }
        public override int GetHashCode() { return default(int); }
        public override string ToString() { return default(string); }
        //    public System.Security.SecurityElement ToXml() { return default(System.Security.SecurityElement); }
        //    public System.Security.SecurityElement ToXml(System.Security.Policy.PolicyLevel level) { return default(System.Security.SecurityElement); }
    }
}
