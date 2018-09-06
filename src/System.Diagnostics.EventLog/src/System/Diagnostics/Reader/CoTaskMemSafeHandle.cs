// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*============================================================
**
**
** Purpose:
** This internal class is a SafeHandle implementation over a
** native CoTaskMem allocated via StringToCoTaskMemAuto.
**
============================================================*/

using System.Runtime.InteropServices;

namespace System.Diagnostics.Eventing.Reader
{
    //
    // Marked as SecurityCritical due to link demands from inherited
    // SafeHandle members.
    //
    [System.Security.SecurityCritical]
    internal sealed class CoTaskMemSafeHandle : SafeHandle
    {
        internal CoTaskMemSafeHandle()
            : base(IntPtr.Zero, true)
        {
        }

        internal void SetMemory(IntPtr handle)
        {
            SetHandle(handle);
        }

        internal IntPtr GetMemory()
        {
            return handle;
        }

        public override bool IsInvalid
        {
            get
            {
                return IsClosed || handle == IntPtr.Zero;
            }
        }

        protected override bool ReleaseHandle()
        {
            Marshal.FreeCoTaskMem(handle);
            handle = IntPtr.Zero;
            return true;
        }

        //
        // DONT compare CoTaskMemSafeHandle with CoTaskMemSafeHandle.Zero
        // use IsInvalid instead. Zero is provided where a NULL handle needed
        //

        public static CoTaskMemSafeHandle Zero
        {
            get
            {
                return new CoTaskMemSafeHandle();
            }
        }
    }
}
