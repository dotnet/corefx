namespace System.Security.Policy
{
    public sealed partial class HashMembershipCondition : /* System.Runtime.Serialization.IDeserializationCallback, System.Runtime.Serialization.ISerializable, */ System.Security.ISecurityEncodable, System.Security.ISecurityPolicyEncodable, System.Security.Policy.IMembershipCondition
    {
        public HashMembershipCondition(System.Security.Cryptography.HashAlgorithm hashAlg, byte[] value) { }
        public System.Security.Cryptography.HashAlgorithm HashAlgorithm { get { return default(System.Security.Cryptography.HashAlgorithm); } set { } }
        public byte[] HashValue { get { return default(byte[]); } set { } }
        public bool Check(System.Security.Policy.Evidence evidence) { return default(bool); }
        public System.Security.Policy.IMembershipCondition Copy() { return default(System.Security.Policy.IMembershipCondition); }
        public override bool Equals(object o) { return default(bool); }
        //    public void FromXml(System.Security.SecurityElement e) { }
        //    public void FromXml(System.Security.SecurityElement e, System.Security.Policy.PolicyLevel level) { }
        public override int GetHashCode() { return default(int); }
        //    void System.Runtime.Serialization.IDeserializationCallback.OnDeserialization(object sender) { }
        //    void System.Runtime.Serialization.ISerializable.GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
        public override string ToString() { return default(string); }
        //    public System.Security.SecurityElement ToXml() { return default(System.Security.SecurityElement); }
        //    public System.Security.SecurityElement ToXml(System.Security.Policy.PolicyLevel level) { return default(System.Security.SecurityElement); }
    }
}
