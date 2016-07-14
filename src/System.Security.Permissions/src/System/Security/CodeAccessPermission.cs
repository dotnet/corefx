namespace System.Security
{
    public abstract partial class CodeAccessPermission : System.Security.IPermission, System.Security.ISecurityEncodable, System.Security.IStackWalk
    {
        protected CodeAccessPermission() { }
        public void Assert() { }
        public abstract System.Security.IPermission Copy();
        public void Demand() { }
        public void Deny() { }
        public override bool Equals(object obj) { return default(bool); }
        //    public abstract void FromXml(System.Security.SecurityElement elem);
        public override int GetHashCode() { return default(int); }
        public abstract System.Security.IPermission Intersect(System.Security.IPermission target);
        public abstract bool IsSubsetOf(System.Security.IPermission target);
        public void PermitOnly() { }
        public override string ToString() { return default(string); }
        //    public abstract System.Security.SecurityElement ToXml();
        public virtual System.Security.IPermission Union(System.Security.IPermission other) { return default(System.Security.IPermission); }
    }
}