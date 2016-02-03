// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Microsoft.Win32.SafeHandles
{
    public sealed partial class SafeMemoryMappedFileHandle : SafeHandle
    {
        internal SafeMemoryMappedFileHandle()
            : base(IntPtr.Zero, true)
        {
        }

        protected override bool ReleaseHandle()
        {
            return Interop.mincore.CloseHandle(handle);
        }

        public override bool IsInvalid
        {
            [SecurityCritical]
            get { return handle == IntPtr.Zero || handle == new IntPtr(-1); }
        }
    }
}
