// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Internal.Cryptography.Pal;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Microsoft.Win32.SafeHandles
{
    public sealed class SafeX509ChainHandle : SafeHandle
    {
        private SafeX509ChainHandle()
            : base(IntPtr.Zero, true)
        {
        }

        public override bool IsInvalid
        {
            get { return handle == IntPtr.Zero; }
        }

        public static SafeX509ChainHandle InvalidHandle
        {
            get { return SafeHandleCache<SafeX509ChainHandle>.GetInvalidHandle(() => new SafeX509ChainHandle()); }
        }

        protected override bool ReleaseHandle()
        {
            return ChainPal.ReleaseSafeX509ChainHandle(handle);
        }

        protected override void Dispose(bool disposing)
        {
            if (!SafeHandleCache<SafeX509ChainHandle>.IsCachedInvalidHandle(this))
            {
                base.Dispose(disposing);
            }
        }
    }
}
