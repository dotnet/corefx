// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Threading;

namespace Microsoft.Win32.SafeHandles
{
    internal sealed class SafePerfProviderHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private SafePerfProviderHandle() : base(true) { }

        protected override bool ReleaseHandle()
        {
            IntPtr tempProviderHandle = handle;

            if (Interlocked.Exchange(ref handle, IntPtr.Zero) != IntPtr.Zero)
            {
                uint Status = UnsafeNativeMethods.PerfStopProvider(tempProviderHandle);
                Debug.Assert(Status == (uint)UnsafeNativeMethods.ERROR_SUCCESS, "PerfStopProvider() fails");
                // ERROR_INVALID_PARAMETER
            }
            return true;
        }
    }
}
