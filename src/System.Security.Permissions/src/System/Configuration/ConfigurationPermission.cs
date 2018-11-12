// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Security;
using System.Security.Permissions;

namespace System.Configuration
{
    public sealed class ConfigurationPermission : CodeAccessPermission, IUnrestrictedPermission
    {
        public ConfigurationPermission(PermissionState state) { }
        public bool IsUnrestricted() => false;
        public override IPermission Copy () { return default(IPermission); }
        public override IPermission Union(IPermission target) { return default(IPermission); }
        public override IPermission Intersect(IPermission target) { return default(IPermission); }
        public override bool IsSubsetOf(IPermission target) => false;
        public override void FromXml(SecurityElement securityElement) { }
        public override SecurityElement ToXml() { return default(SecurityElement); }
    }
}

