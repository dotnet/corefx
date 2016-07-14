namespace System.Security.Policy
{
    public partial class CodeConnectAccess
    {
        public static readonly string AnyScheme;
        public static readonly int DefaultPort;
        public static readonly int OriginPort;
        public static readonly string OriginScheme;
        public CodeConnectAccess(string allowScheme, int allowPort) { }
        public int Port { get { return default(int); } }
        public string Scheme { get { return default(string); } }
        public static System.Security.Policy.CodeConnectAccess CreateAnySchemeAccess(int allowPort) { return default(System.Security.Policy.CodeConnectAccess); }
        public static System.Security.Policy.CodeConnectAccess CreateOriginSchemeAccess(int allowPort) { return default(System.Security.Policy.CodeConnectAccess); }
        public override bool Equals(object o) { return default(bool); }
        public override int GetHashCode() { return default(int); }
    }
}
