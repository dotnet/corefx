// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
namespace System.Security
{
    public partial interface IStackWalk
    {
        void Assert();
        void Demand();
        void Deny();
        void PermitOnly();
    }
    public partial class PermissionSet : System.Collections.ICollection, System.Collections.IEnumerable, System.Runtime.Serialization.IDeserializationCallback, System.Security.ISecurityEncodable, System.Security.IStackWalk
    {
        public PermissionSet(System.Security.Permissions.PermissionState state) { }
        public PermissionSet(System.Security.PermissionSet permSet) { }
        public virtual int Count { get { throw null; } }
        public virtual bool IsReadOnly { get { throw null; } }
        public virtual bool IsSynchronized { get { throw null; } }
        public virtual object SyncRoot { get { throw null; } }
        public System.Security.IPermission AddPermission(System.Security.IPermission perm) { throw null; }
        protected virtual System.Security.IPermission AddPermissionImpl(System.Security.IPermission perm) { throw null; }
        public void Assert() { }
        public bool ContainsNonCodeAccessPermissions() { throw null; }
        [System.ObsoleteAttribute]
        public static byte[] ConvertPermissionSet(string inFormat, byte[] inData, string outFormat) { throw null; }
        public virtual System.Security.PermissionSet Copy() { throw null; }
        public virtual void CopyTo(System.Array array, int index) { }
        public void Demand() { }
        [System.ObsoleteAttribute]
        public void Deny() { }
        public override bool Equals(object o) { throw null; }
        public virtual void FromXml(System.Security.SecurityElement et) { }
        public System.Collections.IEnumerator GetEnumerator() { throw null; }
        protected virtual System.Collections.IEnumerator GetEnumeratorImpl() { throw null; }
        public override int GetHashCode() { throw null; }
        public System.Security.IPermission GetPermission(System.Type permClass) { throw null; }
        protected virtual System.Security.IPermission GetPermissionImpl(System.Type permClass) { throw null; }
        public System.Security.PermissionSet Intersect(System.Security.PermissionSet other) { throw null; }
        public bool IsEmpty() { throw null; }
        public bool IsSubsetOf(System.Security.PermissionSet target) { throw null; }
        public bool IsUnrestricted() { throw null; }
        public void PermitOnly() { }
        public System.Security.IPermission RemovePermission(System.Type permClass) { throw null; }
        protected virtual System.Security.IPermission RemovePermissionImpl(System.Type permClass) { throw null; }
        public static void RevertAssert() { }
        public System.Security.IPermission SetPermission(System.Security.IPermission perm) { throw null; }
        protected virtual System.Security.IPermission SetPermissionImpl(System.Security.IPermission perm) { throw null; }
        void System.Runtime.Serialization.IDeserializationCallback.OnDeserialization(object sender) { }
        public override string ToString() { throw null; }
        public virtual System.Security.SecurityElement ToXml() { throw null; }
        public System.Security.PermissionSet Union(System.Security.PermissionSet other) { throw null; }
    }
}
namespace System.Security.Permissions
{
    public enum PermissionState
    {
        None = 0,
        Unrestricted = 1,
    }
}