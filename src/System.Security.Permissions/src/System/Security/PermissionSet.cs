namespace System.Security
{
    public partial class PermissionSet : System.Collections.ICollection, System.Collections.IEnumerable, /* System.Runtime.Serialization.IDeserializationCallback, */ System.Security.ISecurityEncodable, System.Security.IStackWalk
    {
        public PermissionSet(System.Security.Permissions.PermissionState state) { }
        public PermissionSet(System.Security.PermissionSet permSet) { }
        public virtual int Count { get { return default(int); } }
        public virtual bool IsReadOnly { get { return default(bool); } }
        public virtual bool IsSynchronized { get { return default(bool); } }
        public virtual object SyncRoot { get { return default(object); } }
        public System.Security.IPermission AddPermission(System.Security.IPermission perm) { return default(System.Security.IPermission); }
        public void Assert() { }
        public bool ContainsNonCodeAccessPermissions() { return default(bool); }
        public static byte[] ConvertPermissionSet(string inFormat, byte[] inData, string outFormat) { return default(byte[]); }
        public virtual System.Security.PermissionSet Copy() { return default(System.Security.PermissionSet); }
        public virtual void CopyTo(System.Array array, int index) { }
        public void Demand() { }
        public void Deny() { }
        public override bool Equals(object obj) { return default(bool); }
        //    public virtual void FromXml(System.Security.SecurityElement et) { }
        public System.Collections.IEnumerator GetEnumerator() { return default(System.Collections.IEnumerator); }
        public override int GetHashCode() { return default(int); }
        public System.Security.IPermission GetPermission(System.Type permClass) { return default(System.Security.IPermission); }
        public System.Security.PermissionSet Intersect(System.Security.PermissionSet other) { return default(System.Security.PermissionSet); }
        public bool IsEmpty() { return default(bool); }
        public bool IsSubsetOf(System.Security.PermissionSet target) { return default(bool); }
        public bool IsUnrestricted() { return default(bool); }
        public void PermitOnly() { }
        public System.Security.IPermission RemovePermission(System.Type permClass) { return default(System.Security.IPermission); }
        public static void RevertAssert() { }
        public System.Security.IPermission SetPermission(System.Security.IPermission perm) { return default(System.Security.IPermission); }
        //    void System.Runtime.Serialization.IDeserializationCallback.OnDeserialization(object sender) { }
        public override string ToString() { return default(string); }
        //    public virtual System.Security.SecurityElement ToXml() { return default(System.Security.SecurityElement); }
        public System.Security.PermissionSet Union(System.Security.PermissionSet other) { return default(System.Security.PermissionSet); }
    }
}
