// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Security;
using System.Security.Permissions;

namespace System.Web
{
    public sealed class AspNetHostingPermission :  CodeAccessPermission, IUnrestrictedPermission
    {
        public AspNetHostingPermission(PermissionState state) { }
        public AspNetHostingPermission(AspNetHostingPermissionLevel level) { }
        public AspNetHostingPermissionLevel Level { get; set; }
        public bool IsUnrestricted() => false;
        public override IPermission Copy() { return null; }
        public override IPermission Union(IPermission target) { return null; }
        public override IPermission Intersect(IPermission target) { return null; }
        public override bool IsSubsetOf(IPermission target) => false;
        public override void FromXml(SecurityElement securityElement) { }
        public override SecurityElement ToXml() { return null; }
    }
}