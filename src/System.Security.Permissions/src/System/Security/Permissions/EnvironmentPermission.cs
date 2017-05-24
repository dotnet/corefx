﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Security.Permissions
{
    public sealed partial class EnvironmentPermission : CodeAccessPermission, IUnrestrictedPermission
    {
        public EnvironmentPermission(EnvironmentPermissionAccess flag, string pathList) { }
        public EnvironmentPermission(PermissionState state) { }
        public void AddPathList(EnvironmentPermissionAccess flag, string pathList) { }
        public override IPermission Copy() { return default(IPermission); }
        public override void FromXml(SecurityElement esd) { }
        public string GetPathList(EnvironmentPermissionAccess flag) { return null; }
        public override IPermission Intersect(IPermission target) { return default(IPermission); }
        public override bool IsSubsetOf(IPermission target) { return false; }
        public bool IsUnrestricted() { return false; }
        public void SetPathList(EnvironmentPermissionAccess flag, string pathList) { }
        public override SecurityElement ToXml() { return default(SecurityElement); }
        public override IPermission Union(IPermission other) { return default(IPermission); }
    }
}
