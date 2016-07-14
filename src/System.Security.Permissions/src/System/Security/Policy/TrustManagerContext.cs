namespace System.Security.Policy
{
    public partial class TrustManagerContext
    {
        public TrustManagerContext() { }
        public TrustManagerContext(System.Security.Policy.TrustManagerUIContext uiContext) { }
        public virtual bool IgnorePersistedDecision { get { return default(bool); } set { } }
        public virtual bool KeepAlive { get { return default(bool); } set { } }
        public virtual bool NoPrompt { get { return default(bool); } set { } }
        public virtual bool Persist { get { return default(bool); } set { } }
        //    public virtual System.ApplicationIdentity PreviousApplicationIdentity { get { return default(System.ApplicationIdentity); } set { } }
        public virtual System.Security.Policy.TrustManagerUIContext UIContext { get { return default(System.Security.Policy.TrustManagerUIContext); } set { } }
    }
    public enum TrustManagerUIContext
    {
        Install = 0,
        Run = 2,
        Upgrade = 1,
    }
}
