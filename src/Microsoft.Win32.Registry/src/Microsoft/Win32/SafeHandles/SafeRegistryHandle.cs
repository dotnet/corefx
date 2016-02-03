// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

namespace Microsoft.Win32.SafeHandles
{
    [System.Security.SecurityCritical]
    public sealed class SafeRegistryHandle : SafeHandle
    {
        [System.Security.SecurityCritical]
        internal SafeRegistryHandle() : base(IntPtr.Zero, true) { }

        [System.Security.SecurityCritical]
        public SafeRegistryHandle(IntPtr preexistingHandle, bool ownsHandle) : base(IntPtr.Zero, ownsHandle)
        {
            SetHandle(preexistingHandle);
        }

        public override bool IsInvalid
        {
            [System.Security.SecurityCritical]
            get
            { return handle == new IntPtr(0) || handle == new IntPtr(-1); }
        }

        [System.Security.SecurityCritical]
        override protected bool ReleaseHandle()
        {
            return (Interop.mincore.RegCloseKey(handle) == Interop.mincore.Errors.ERROR_SUCCESS);
        }
    }
}

