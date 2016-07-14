namespace System.Security
{
    public sealed partial class SecurityContext : System.IDisposable
    {
        internal SecurityContext() { }
        public static System.Security.SecurityContext Capture() { return default(System.Security.SecurityContext); }
        public System.Security.SecurityContext CreateCopy() { return default(System.Security.SecurityContext); }
        public void Dispose() { }
        public static bool IsFlowSuppressed() { return default(bool); }
        public static bool IsWindowsIdentityFlowSuppressed() { return default(bool); }
        public static void RestoreFlow() { }
        public static void Run(System.Security.SecurityContext securityContext, System.Threading.ContextCallback callback, object state) { }
        //    public static System.Threading.AsyncFlowControl SuppressFlow() { return default(System.Threading.AsyncFlowControl); }
        //    public static System.Threading.AsyncFlowControl SuppressFlowWindowsIdentity() { return default(System.Threading.AsyncFlowControl); }
    }
}
