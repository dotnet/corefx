// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.Serialization;
using System.Security.Permissions;

namespace System.Security
{
    [Serializable]
    [System.Runtime.CompilerServices.TypeForwardedFrom("mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
    public partial class HostProtectionException : System.SystemException
    {
        private const string ProtectedResourcesName = "ProtectedResources";
        private const string DemandedResourcesName = "DemandedResources";

        public HostProtectionException() { }

        public HostProtectionException(string message) { }

        public HostProtectionException(string message, Exception e) { }

        public HostProtectionException(string message, HostProtectionResource protectedResources, HostProtectionResource demandedResources) { }

        protected HostProtectionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            ProtectedResources = (HostProtectionResource)info.GetValue(ProtectedResourcesName, typeof(HostProtectionResource));
            DemandedResources = (HostProtectionResource)info.GetValue(DemandedResourcesName, typeof(HostProtectionResource));
        }

        public HostProtectionResource DemandedResources { get; } = default;

        public HostProtectionResource ProtectedResources { get; } = default;

        public override string ToString() => base.ToString();

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(ProtectedResourcesName, ProtectedResources, typeof(HostProtectionResource));
            info.AddValue(DemandedResourcesName, DemandedResources, typeof(HostProtectionResource));
        }
    }
}
