// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Microsoft.Win32.SafeHandles
{
    internal sealed class SafeLsaMemoryHandle : SafeBuffer
    {
        private SafeLsaMemoryHandle() : base(true) { }

        // 0 is an Invalid Handle
        internal SafeLsaMemoryHandle(IntPtr handle) : base(true)
        {
            SetHandle(handle);
        }

        override protected bool ReleaseHandle()
        {
            return Interop.Advapi32.LsaFreeMemory(handle) == 0;
        }
    }

    internal sealed class SafeLsaPolicyHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private SafeLsaPolicyHandle() : base(true) { }

        // 0 is an Invalid Handle
        internal SafeLsaPolicyHandle(IntPtr handle) : base(true)
        {
            SetHandle(handle);
        }

        override protected bool ReleaseHandle()
        {
            return Interop.Advapi32.LsaClose(handle) == 0;
        }
    }

    internal sealed class SafeLsaReturnBufferHandle : SafeBuffer
    {
        private SafeLsaReturnBufferHandle() : base(true) { }

        // 0 is an Invalid Handle
        internal SafeLsaReturnBufferHandle(IntPtr handle) : base(true)
        {
            SetHandle(handle);
        }

        override protected bool ReleaseHandle()
        {
            // LsaFreeReturnBuffer returns an NTSTATUS
            return Interop.SspiCli.LsaFreeReturnBuffer(handle) >= 0;
        }
    }
}
