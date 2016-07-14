namespace System.Security
{
    public sealed partial class NamedPermissionSet : System.Security.PermissionSet
    {
        public NamedPermissionSet(System.Security.NamedPermissionSet permSet) : base(default(System.Security.Permissions.PermissionState)) { }
        public NamedPermissionSet(string name) : base(default(System.Security.Permissions.PermissionState)) { }
        public NamedPermissionSet(string name, System.Security.Permissions.PermissionState state) : base(default(System.Security.Permissions.PermissionState)) { }
        public NamedPermissionSet(string name, System.Security.PermissionSet permSet) : base(default(System.Security.Permissions.PermissionState)) { }
        public string Description { get { return default(string); } set { } }
        public string Name { get { return default(string); } set { } }
        public override System.Security.PermissionSet Copy() { return default(System.Security.PermissionSet); }
        public System.Security.NamedPermissionSet Copy(string name) { return default(System.Security.NamedPermissionSet); }
        public override bool Equals(object obj) { return default(bool); }
        //    public override void FromXml(System.Security.SecurityElement et) { }
        public override int GetHashCode() { return default(int); }
        //    public override System.Security.SecurityElement ToXml() { return default(System.Security.SecurityElement); }
    }
}
