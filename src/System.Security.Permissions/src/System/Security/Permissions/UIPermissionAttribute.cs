﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Security.Permissions
{
    [AttributeUsage((AttributeTargets)(109), AllowMultiple = true, Inherited = false)]
    public sealed partial class UIPermissionAttribute : CodeAccessSecurityAttribute
    {
        public UIPermissionAttribute(SecurityAction action) : base(default(SecurityAction)) { }
        public UIPermissionClipboard Clipboard { get; set; }
        public UIPermissionWindow Window { get; set; }
        public override IPermission CreatePermission() { return default(IPermission); }
    }
}
