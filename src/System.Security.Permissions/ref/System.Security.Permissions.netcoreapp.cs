// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------

namespace System.Xaml.Permissions
{
    public sealed partial class XamlLoadPermission : System.Security.CodeAccessPermission, System.Security.Permissions.IUnrestrictedPermission
    {
        public XamlLoadPermission(System.Collections.Generic.IEnumerable<System.Xaml.Permissions.XamlAccessLevel> allowedAccess) { }
        public XamlLoadPermission(System.Security.Permissions.PermissionState state) { }
        public XamlLoadPermission(System.Xaml.Permissions.XamlAccessLevel allowedAccess) { }
        public System.Collections.Generic.IList<System.Xaml.Permissions.XamlAccessLevel> AllowedAccess { get { throw null; } }
        public override System.Security.IPermission Copy() { throw null; }
        public override bool Equals(object obj) { throw null; }
        public override void FromXml(System.Security.SecurityElement elem) { }
        public override int GetHashCode() { throw null; }
        public bool Includes(System.Xaml.Permissions.XamlAccessLevel requestedAccess) { throw null; }
        public override System.Security.IPermission Intersect(System.Security.IPermission target) { throw null; }
        public override bool IsSubsetOf(System.Security.IPermission target) { throw null; }
        public bool IsUnrestricted() { throw null; }
        public override System.Security.SecurityElement ToXml() { throw null; }
        public override System.Security.IPermission Union(System.Security.IPermission other) { throw null; }
    }
}
