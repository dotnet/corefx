// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;

namespace System.Xaml.Permissions
{
    [Serializable]
    public sealed class XamlLoadPermission : CodeAccessPermission, IUnrestrictedPermission
    {
        private IList<XamlAccessLevel> _emptyAccessLevel = new ReadOnlyCollection<XamlAccessLevel>(Array.Empty<XamlAccessLevel>());
        public XamlLoadPermission(PermissionState state) { }
        public XamlLoadPermission(XamlAccessLevel allowedAccess) { }
        public XamlLoadPermission(IEnumerable<XamlAccessLevel> allowedAccess) { }
#if NETCOREAPP3_0
        [ComVisible(false)]
        public override bool Equals(object obj) { return false; }
        [ComVisible(false)]
        public override int GetHashCode() { return base.GetHashCode(); }
#endif 
        public IList<XamlAccessLevel> AllowedAccess 
        { 
            get 
            {
                return _emptyAccessLevel;
            }
            private set
            {
            }
        }
        public override IPermission Copy() { return default(IPermission); }
        public override void FromXml(SecurityElement elem) { }
        public bool Includes(XamlAccessLevel requestedAccess) { return false;  }
        public override IPermission Intersect(IPermission target) { return default(IPermission); }
        public override bool IsSubsetOf(IPermission target) { return false; }
        public override SecurityElement ToXml() { return default(SecurityElement); }
        public override IPermission Union(IPermission other) { return default(IPermission); }
        public bool IsUnrestricted() { return true; }
    }
}
