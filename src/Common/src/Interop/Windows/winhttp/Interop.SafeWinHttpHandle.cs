// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.InteropServices;

using Microsoft.Win32.SafeHandles;

internal partial class Interop
{
    internal partial class WinHttp
    {
        // Issue 2501: Fold SafeWinHttpHandleWithCallback and SafeWinHttpHandle into a single type.
        //
        // SafeWinHttpHandleWithCallback is a better pattern; SafeWinHttpHandle is probably a better
        // name. The implementation for the former should probably be folded into the latter.

        internal class SafeWinHttpHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
            public SafeWinHttpHandle() : base(true)
            {
            }

            public static void DisposeAndClearHandle(ref SafeWinHttpHandle winHttpHandler)
            {
                if (winHttpHandler != null)
                {
                    winHttpHandler.Dispose();
                    winHttpHandler = null;
                }
            }

            // The SafeHandle base class is inconsistent w.r.t. how Dispose() and DangerousRelease()
            // are implemented. Both methods can cause the ref count on the SafeHandle to be decremented.
            // And when the ref count reaches zero, ReleaseHandle() is called and the handle is marked
            // as 'closed'. However, there is a separate internal state bit to track Dispose() calls.
            // Calling Dispose() more than once is a no-op.  However, calling DangerousRelease() and then
            // Dispose() will cause ObjectDisposedException to be thrown if the previous DangerousRelease()
            // caused the handle to be closed. This seems inconsistent especially since both methods can
            // decrement the ref count and cause ReleaseHandle() to be called.
            //
            // The SafeWinHttpHandle class requires special lifetime management. Multiple classes such as
            // WinHttpHandler, WinHttpRequestStream, and WinHttpResponseStream interact together with the
            // same WinHttp handles for a single request/response operation. In addition, there is a parent-child
            // hierarchy among the handles which causes additional ref counts to be added to keep the handles
            // valid. Due to these requirements, the use pattern of SafeWinHttpHandle typically uses DangerousAddRef()
            // and DangerousRelease() to help manage ref counts. Thus, to work around the limitations of the
            // current SafeHandle implementation, it is important that Dispose() be a no-op if the handle has
            // already been closed due to final release caused by DangerousRelease().
            protected override void Dispose(bool disposing)
            {
                if (!IsClosed)
                {
                    base.Dispose(disposing);
                }
            }
        
            // Important: WinHttp API calls should not happen while another WinHttp call for the same handle did not 
            // return. During finalization that was not initiated by the Dispose pattern we don't expect any other WinHttp
            // calls in progress.
            protected override bool ReleaseHandle()
            {
                // TODO(Issue 2500): Add logging so we know when the handle gets closed.
                return Interop.WinHttp.WinHttpCloseHandle(handle);
            }
        }

        internal sealed class SafeWinHttpHandleWithCallback : SafeWinHttpHandle
        {
            // Add a reference to this object so that the AppDomain doesn't get unloaded before the last WinHttp callback.
            public void AttachCallback()
            {
                bool success = false;
                DangerousAddRef(ref success);
                if (!success)
                {
                    throw new InvalidOperationException("Could not increase SafeHandle reference.");
                }
            }

            public void DetachCallback()
            {
                DangerousRelease();
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    // We need to initiate the asynchronous process of closing the WinHttp handle:
                    // 1. Ensure that all other WinHttp function calls for this handle have returned.
                    // 2. Call WinHttpCloseHandle.
                    // 3. Wait for WINHTTP_CALLBACK_STATUS_HANDLE_CLOSING in the callback.
                    //    On this event, call DetachCallback.
                    //
                    // WinHttp guarantees that no other calls to the callback are made for this handle after 
                    // WINHTTP_CALLBACK_STATUS_HANDLE_CLOSING has been received.
                    // Holding the reference to the SafeHandle object ensures that the appdomain where the SafeHandle 
                    // lives until it is guaranteed that no further callback calls are made from the native (WinHttp) side.
                    Interop.WinHttp.WinHttpCloseHandle(handle);
                }
            }

            protected override bool ReleaseHandle()
            {
                return base.ReleaseHandle();
            }
        }
    }
}
