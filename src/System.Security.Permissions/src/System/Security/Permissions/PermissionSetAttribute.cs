﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Security.Permissions
{
    [System.AttributeUsageAttribute((System.AttributeTargets)(109), AllowMultiple = true, Inherited = false)]
    public sealed partial class PermissionSetAttribute : System.Security.Permissions.CodeAccessSecurityAttribute
    {
        public PermissionSetAttribute(System.Security.Permissions.SecurityAction action) : base(default(System.Security.Permissions.SecurityAction)) { }
        public string File { get; set; }
        public string Hex { get; set; }
        public string Name { get; set; }
        public bool UnicodeEncoded { get; set; }
        public string XML { get; set; }
        public override System.Security.IPermission CreatePermission() { return default(System.Security.IPermission); }
        public System.Security.PermissionSet CreatePermissionSet() { return default(System.Security.PermissionSet); }
    }
}
