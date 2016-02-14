// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*============================================================
**
** Class:  SafeTokenHandle 
**
** A wrapper for a process handle
**
** 
===========================================================*/

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;

namespace Microsoft.Win32.SafeHandles
{
    [SecurityCritical]
    public sealed class SafeTokenHandle : SafeHandle
    {
        private const int DefaultInvalidHandleValue = 0;

        private SafeTokenHandle() : base(new IntPtr(DefaultInvalidHandleValue), true) { }

        public SafeTokenHandle(IntPtr handle)
            : base(new IntPtr(DefaultInvalidHandleValue), true)
        {
            SetHandle(handle);
        }

        public SafeTokenHandle(IntPtr handle, bool ownsHandle)
            : base(new IntPtr(DefaultInvalidHandleValue), ownsHandle)
        {
            SetHandle(handle);
        }

        public static SafeTokenHandle InvalidHandle
        {
            get
            {
                return new SafeTokenHandle(IntPtr.Zero);
            }
        }

        public override bool IsInvalid
        {
            [SecurityCritical]
            get
            {
                return handle == IntPtr.Zero || handle == new IntPtr(-1);
            }
        }

        [SecurityCritical]
        protected override bool ReleaseHandle()
        {
            return Interop.mincore.CloseHandle(handle);
        }
    }
}
