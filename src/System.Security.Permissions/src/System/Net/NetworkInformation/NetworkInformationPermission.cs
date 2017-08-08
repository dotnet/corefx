// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Security;
using System.Security.Permissions;

namespace System.Net.NetworkInformation
{
    public sealed class NetworkInformationPermission : CodeAccessPermission, IUnrestrictedPermission
    {
        public NetworkInformationPermission(PermissionState state) { }
        public NetworkInformationPermission(NetworkInformationAccess access) { }
        public NetworkInformationAccess Access { get; }
        public void AddPermission(NetworkInformationAccess access) { }
        public bool IsUnrestricted() => true;
        public override IPermission Copy() => this;
        public override IPermission Union(IPermission target) => null;
        public override IPermission Intersect(IPermission target) => null;
        public override bool IsSubsetOf(IPermission target) => false;
        public override void FromXml(SecurityElement securityElement) { }
        public override SecurityElement ToXml() => null;
    }
}
