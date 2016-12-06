//------------------------------------------------------------------------------
// <copyright file="UnsafeNativeMethods.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Win32 {
    using System.Runtime.InteropServices;
    using System;
    using System.Security.Permissions;
    using Microsoft.Win32.SafeHandles;
    using System.Configuration;

    // Safehandle for memory handles
    [System.Security.SuppressUnmanagedCodeSecurityAttribute()]
    internal sealed class SafeNativeMemoryHandle : SafeHandleZeroOrMinusOneIsInvalid {
        private bool _useLocalFree = false;
        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
        internal SafeNativeMemoryHandle()
            : this(false) {
        }

        internal SafeNativeMemoryHandle(bool useLocalFree)
            : base(true) {
            _useLocalFree = useLocalFree;
        }

        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
        internal SafeNativeMemoryHandle(IntPtr handle, bool ownsHandle)
            : base(ownsHandle) {
            SetHandle(handle);
        }

        internal void SetDataHandle(IntPtr handle) {
            SetHandle(handle);
        }

        override protected bool ReleaseHandle() {
            if (handle != IntPtr.Zero) {
                if (_useLocalFree == true)
                    UnsafeNativeMethods.LocalFree(handle);
                else
                    Marshal.FreeHGlobal(handle);
                handle = IntPtr.Zero;
                return true;
            }
            return false;
        }
    }


}

