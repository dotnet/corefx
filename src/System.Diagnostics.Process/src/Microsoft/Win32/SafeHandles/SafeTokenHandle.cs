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
    internal sealed class SafeTokenHandle : SafeHandle
    {
        private const int DefaultInvalidHandleValue = 0;

        internal static readonly SafeTokenHandle InvalidHandle = new SafeTokenHandle(new IntPtr(DefaultInvalidHandleValue));

        internal SafeTokenHandle() : base(new IntPtr(DefaultInvalidHandleValue), true) { }

        internal SafeTokenHandle(IntPtr handle)
            : base(new IntPtr(DefaultInvalidHandleValue), true)
        {
            SetHandle(handle);
        }

        public override bool IsInvalid
        {
            get { return handle == IntPtr.Zero || handle == new IntPtr(-1); }
        }

        protected override bool ReleaseHandle()
        {
            return Interop.Kernel32.CloseHandle(handle);
        }
    }
}
