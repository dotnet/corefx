namespace System.Security.Policy
{
    public abstract partial class CodeGroup
    {
        protected CodeGroup(System.Security.Policy.IMembershipCondition membershipCondition, System.Security.Policy.PolicyStatement policy) { }
        public virtual string AttributeString { get { return default(string); } }
        public System.Collections.IList Children { get { return default(System.Collections.IList); } set { } }
        public string Description { get { return default(string); } set { } }
        public System.Security.Policy.IMembershipCondition MembershipCondition { get { return default(System.Security.Policy.IMembershipCondition); } set { } }
        public abstract string MergeLogic { get; }
        public string Name { get { return default(string); } set { } }
        public virtual string PermissionSetName { get { return default(string); } }
        public System.Security.Policy.PolicyStatement PolicyStatement { get { return default(System.Security.Policy.PolicyStatement); } set { } }
        public void AddChild(System.Security.Policy.CodeGroup group) { }
        public abstract System.Security.Policy.CodeGroup Copy();
        //    protected virtual void CreateXml(System.Security.SecurityElement element, System.Security.Policy.PolicyLevel level) { }
        public override bool Equals(object o) { return default(bool); }
        public bool Equals(System.Security.Policy.CodeGroup cg, bool compareChildren) { return default(bool); }
        //    public void FromXml(System.Security.SecurityElement e) { }
        //    public void FromXml(System.Security.SecurityElement e, System.Security.Policy.PolicyLevel level) { }
        public override int GetHashCode() { return default(int); }
        //    protected virtual void ParseXml(System.Security.SecurityElement e, System.Security.Policy.PolicyLevel level) { }
        public void RemoveChild(System.Security.Policy.CodeGroup group) { }
        public abstract System.Security.Policy.PolicyStatement Resolve(System.Security.Policy.Evidence evidence);
        public abstract System.Security.Policy.CodeGroup ResolveMatchingCodeGroups(System.Security.Policy.Evidence evidence);
        //    public System.Security.SecurityElement ToXml() { return default(System.Security.SecurityElement); }
        //    public System.Security.SecurityElement ToXml(System.Security.Policy.PolicyLevel level) { return default(System.Security.SecurityElement); }
    }
}
