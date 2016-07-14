namespace System.Security
{
    public partial class HostProtectionException : System.SystemException
    {
        public HostProtectionException() { }
        // protected HostProtectionException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
        public HostProtectionException(string message) { }
        public HostProtectionException(string message, System.Exception e) { }
        public HostProtectionException(string message, System.Security.Permissions.HostProtectionResource protectedResources, System.Security.Permissions.HostProtectionResource demandedResources) { }
        public System.Security.Permissions.HostProtectionResource DemandedResources { get { return default(System.Security.Permissions.HostProtectionResource); } }
        public System.Security.Permissions.HostProtectionResource ProtectedResources { get { return default(System.Security.Permissions.HostProtectionResource); } }
        // public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
        public override string ToString() { return default(string); }
    }
}
