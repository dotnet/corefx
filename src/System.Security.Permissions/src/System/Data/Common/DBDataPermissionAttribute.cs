﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Security.Permissions;

namespace System.Data.Common
{
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor| AttributeTargets.Method,
        AllowMultiple =true, Inherited =false)]
    public abstract class DBDataPermissionAttribute : CodeAccessSecurityAttribute
    {
        protected DBDataPermissionAttribute(SecurityAction action) : base(action) { }
        public bool AllowBlankPassword { get; set; }
        public string ConnectionString { get; set; }
        public KeyRestrictionBehavior KeyRestrictionBehavior { get; set; }
        public string KeyRestrictions { get; set; }
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public bool ShouldSerializeConnectionString() { return false; }
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public bool ShouldSerializeKeyRestrictions() { return false; }
    }
}
