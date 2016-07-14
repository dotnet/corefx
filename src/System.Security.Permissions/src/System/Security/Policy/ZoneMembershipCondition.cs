namespace System.Security.Policy
{
    public sealed partial class ZoneMembershipCondition : System.Security.ISecurityEncodable, System.Security.Policy.IMembershipCondition
    {
        public ZoneMembershipCondition(System.Security.SecurityZone zone) { }
        public System.Security.SecurityZone SecurityZone { get { return default(System.Security.SecurityZone); } set { } }
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
