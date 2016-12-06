//------------------------------------------------------------------------------
// <copyright file="UnsafeNativeMethods.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Win32 {
    using System;
    using System.Security.Permissions;
    using Microsoft.Win32.SafeHandles;
    using System.Configuration;

    // Safehandle for crypt context handles
    [System.Security.SuppressUnmanagedCodeSecurityAttribute()]
    internal sealed class SafeCryptContextHandle : SafeHandleZeroOrMinusOneIsInvalid {
        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
        internal SafeCryptContextHandle()
            : base(true) {
        }

        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
        internal SafeCryptContextHandle(IntPtr handle, bool ownsHandle)
            : base(ownsHandle) {
            SetHandle(handle);
        }

        override protected bool ReleaseHandle() {
            if (handle != IntPtr.Zero) {
                UnsafeNativeMethods.CryptReleaseContext(this, 0);
                return true;
            }
            return false;
        }
    }
}

