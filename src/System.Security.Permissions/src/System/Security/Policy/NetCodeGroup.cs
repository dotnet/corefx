namespace System.Security.Policy
{
    public sealed partial class NetCodeGroup : System.Security.Policy.CodeGroup
    {
        public static readonly string AbsentOriginScheme;
        public static readonly string AnyOtherOriginScheme;
        public NetCodeGroup(System.Security.Policy.IMembershipCondition membershipCondition) : base(default(System.Security.Policy.IMembershipCondition), default(System.Security.Policy.PolicyStatement)) { }
        public override string AttributeString { get { return default(string); } }
        public override string MergeLogic { get { return default(string); } }
        public override string PermissionSetName { get { return default(string); } }
        public void AddConnectAccess(string originScheme, System.Security.Policy.CodeConnectAccess connectAccess) { }
        public override System.Security.Policy.CodeGroup Copy() { return default(System.Security.Policy.CodeGroup); }
        //    protected override void CreateXml(System.Security.SecurityElement element, System.Security.Policy.PolicyLevel level) { }
        public override bool Equals(object o) { return default(bool); }
        public System.Collections.DictionaryEntry[] GetConnectAccessRules() { return default(System.Collections.DictionaryEntry[]); }
        public override int GetHashCode() { return default(int); }
        //    protected override void ParseXml(System.Security.SecurityElement e, System.Security.Policy.PolicyLevel level) { }
        public void ResetConnectAccess() { }
        public override System.Security.Policy.PolicyStatement Resolve(System.Security.Policy.Evidence evidence) { return default(System.Security.Policy.PolicyStatement); }
        public override System.Security.Policy.CodeGroup ResolveMatchingCodeGroups(System.Security.Policy.Evidence evidence) { return default(System.Security.Policy.CodeGroup); }
    }
}
