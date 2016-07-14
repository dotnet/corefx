namespace System.Security.Policy
{
    public sealed partial class PolicyStatement : System.Security.ISecurityEncodable, System.Security.ISecurityPolicyEncodable
    {
        public PolicyStatement(System.Security.PermissionSet permSet) { }
        public PolicyStatement(System.Security.PermissionSet permSet, System.Security.Policy.PolicyStatementAttribute attributes) { }
        public System.Security.Policy.PolicyStatementAttribute Attributes { get { return default(System.Security.Policy.PolicyStatementAttribute); } set { } }
        public string AttributeString { get { return default(string); } }
        public System.Security.PermissionSet PermissionSet { get { return default(System.Security.PermissionSet); } set { } }
        public System.Security.Policy.PolicyStatement Copy() { return default(System.Security.Policy.PolicyStatement); }
        public override bool Equals(object obj) { return default(bool); }
        //    public void FromXml(System.Security.SecurityElement et) { }
        //    public void FromXml(System.Security.SecurityElement et, System.Security.Policy.PolicyLevel level) { }
        public override int GetHashCode() { return default(int); }
        //    public System.Security.SecurityElement ToXml() { return default(System.Security.SecurityElement); }
        //    public System.Security.SecurityElement ToXml(System.Security.Policy.PolicyLevel level) { return default(System.Security.SecurityElement); }
    }
}
