namespace System.Security.Policy
{
    public partial class PolicyException : System.SystemException
    {
        public PolicyException() { }
        //    protected PolicyException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
        public PolicyException(string message) { }
        public PolicyException(string message, System.Exception exception) { }
    }
}
