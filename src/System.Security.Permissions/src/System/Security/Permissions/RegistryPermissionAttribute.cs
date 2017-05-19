﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Security.Permissions
{
    [AttributeUsage((AttributeTargets)(109), AllowMultiple = true, Inherited = false)]
    public sealed partial class RegistryPermissionAttribute : CodeAccessSecurityAttribute
    {
        public RegistryPermissionAttribute(SecurityAction action) : base(default(SecurityAction)) { }
        [Obsolete("Please use the ViewAndModify property instead.")]
        public string All { get; set; }
        public string ChangeAccessControl { get; set; }
        public string Create { get; set; }
        public string Read { get; set; }
        public string ViewAccessControl { get; set; }
        public string ViewAndModify { get; set; }
        public string Write { get; set; }
        public override IPermission CreatePermission() { return default(IPermission); }
    }
}
