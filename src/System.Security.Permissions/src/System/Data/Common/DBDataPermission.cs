﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Security;
using System.Security.Permissions;

namespace System.Data.Common
{
    [Serializable]
    [SecurityPermission(SecurityAction.InheritanceDemand, ControlEvidence = true, ControlPolicy = true)]
    public abstract class DBDataPermission : CodeAccessPermission, IUnrestrictedPermission
    {
        [Obsolete("DBDataPermission() has been deprecated.  Use the DBDataPermission(PermissionState.None) constructor.  http://go.microsoft.com/fwlink/?linkid=14202", true)]
        protected DBDataPermission() { }
        protected DBDataPermission(DBDataPermission dataPermission) { }
        protected DBDataPermission(DBDataPermissionAttribute attribute) { }
        protected DBDataPermission(PermissionState state) { }
        [Obsolete("DBDataPermission(PermissionState state,Boolean allowBlankPassword) has been deprecated.  Use the DBDataPermission(PermissionState.None) constructor.  http://go.microsoft.com/fwlink/?linkid=14202", true)]
        protected DBDataPermission(PermissionState state, bool blankPassword) { }
        public bool AllowBlankPassword { get; set; }
        public virtual void Add(string connectionString, string restrictions, KeyRestrictionBehavior behavior) { }
        protected void Clear() { }
        protected virtual DBDataPermission CreateInstance() => null;
        public override IPermission Copy() => this;
        public override void FromXml(SecurityElement elem) { }
        public override IPermission Intersect(IPermission target) => null;
        public override bool IsSubsetOf(IPermission target) => false;
        public bool IsUnrestricted() => false;
        public override SecurityElement ToXml() => null;
        public override IPermission Union(IPermission other) { return default(IPermission); }
    }
}
