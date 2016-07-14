namespace System.Security
{
    public abstract partial class SecurityState
    {
        protected SecurityState() { }
        public abstract void EnsureState();
        public bool IsStateAvailable() { return default(bool); }
    }
}
