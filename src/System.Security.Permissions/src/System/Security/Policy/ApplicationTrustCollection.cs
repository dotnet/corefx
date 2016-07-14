namespace System.Security.Policy
{
    public sealed partial class ApplicationTrustCollection : System.Collections.ICollection, System.Collections.IEnumerable
    {
        internal ApplicationTrustCollection() { }
        public int Count { get { return default(int); } }
        public bool IsSynchronized { get { return default(bool); } }
        public System.Security.Policy.ApplicationTrust this[int index] { get { return default(System.Security.Policy.ApplicationTrust); } }
        public System.Security.Policy.ApplicationTrust this[string appFullName] { get { return default(System.Security.Policy.ApplicationTrust); } }
        public object SyncRoot { get { return default(object); } }
        public int Add(System.Security.Policy.ApplicationTrust trust) { return default(int); }
        public void AddRange(System.Security.Policy.ApplicationTrust[] trusts) { }
        public void AddRange(System.Security.Policy.ApplicationTrustCollection trusts) { }
        public void Clear() { }
        public void CopyTo(System.Security.Policy.ApplicationTrust[] array, int index) { }
        //    public System.Security.Policy.ApplicationTrustCollection Find(System.ApplicationIdentity applicationIdentity, System.Security.Policy.ApplicationVersionMatch versionMatch) { return default(System.Security.Policy.ApplicationTrustCollection); }
        public System.Security.Policy.ApplicationTrustEnumerator GetEnumerator() { return default(System.Security.Policy.ApplicationTrustEnumerator); }
        //    public void Remove(System.ApplicationIdentity applicationIdentity, System.Security.Policy.ApplicationVersionMatch versionMatch) { }
        public void Remove(System.Security.Policy.ApplicationTrust trust) { }
        public void RemoveRange(System.Security.Policy.ApplicationTrust[] trusts) { }
        public void RemoveRange(System.Security.Policy.ApplicationTrustCollection trusts) { }
        void System.Collections.ICollection.CopyTo(System.Array array, int index) { }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return default(System.Collections.IEnumerator); }
    }
}
