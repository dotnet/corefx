// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Win32.SafeHandles;

namespace System.Net.NetworkInformation
{
    // This class guarantees that any in-progress notifications will be canceled before the AppDomain gets unloaded.
    // CancelMibChangeNotify2 guarantees that after it returns, the callback will NEVER be called.  It may block
    // for a small amount of time if the callback is currently in progress, which is fine (and, intentional).

    internal class SafeCancelMibChangeNotify : SafeHandleZeroOrMinusOneIsInvalid
    {
        public SafeCancelMibChangeNotify() : base(true) { }

        protected override bool ReleaseHandle()
        {
            uint err = Interop.IpHlpApi.CancelMibChangeNotify2(base.handle);
            base.handle = IntPtr.Zero;
            return (err == Interop.IpHlpApi.ERROR_SUCCESS);
        }
    }
}
