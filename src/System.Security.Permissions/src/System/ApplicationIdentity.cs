using System.Runtime.Serialization;
using System.Security;

namespace System
{
    [Serializable]
    public sealed class ApplicationIdentity : ISerializable
    {
        private ApplicationIdentity() { }
        [SecurityCritical]
        private ApplicationIdentity(SerializationInfo info, StreamingContext context) { }

        [SecuritySafeCritical]
        public ApplicationIdentity(string applicationIdentityFullName) { }

        public string FullName { get { return null; } }

        public string CodeBase { get { return null; } }

        public override string ToString() { return null; }

        /// <internalonly/>
        [SecurityCritical]  // auto-generated_required
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context) { }
    }
}
