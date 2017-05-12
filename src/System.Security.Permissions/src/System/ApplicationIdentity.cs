// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.Serialization;
using System.Security;

namespace System
{
#pragma warning disable CA2237 // ISerializable without SerializableAttribute since we don't want this type to be serializable in Core
    public sealed class ApplicationIdentity : ISerializable
    {
        private ApplicationIdentity() { }
        public ApplicationIdentity(string applicationIdentityFullName) { }
        private ApplicationIdentity(SerializationInfo info, StreamingContext context) { }
        public string FullName { get { return null; } }
        public string CodeBase { get { return null; } }
        public override string ToString() { return null; }
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context) { }
    }
}
