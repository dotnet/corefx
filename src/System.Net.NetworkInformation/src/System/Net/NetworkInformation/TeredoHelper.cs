// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using System.Net;

namespace System.Net.NetworkInformation
{
    // This class wraps the native API NotifyStableUnicastIpAddressTable.  The native function's behavior is:
    // 
    // 1. If the address table is already stable, it returns ERROR_SUCCESS and a Mib table handle that we must free.
    //    The passed-in callback will never be called, and the cancelHandle is set to NULL.
    //
    // 2. If the address table is not stable, it returns ERROR_IO_PENDING.  The table handle is set to NULL,
    //    and the cancelHandle is set to a valid handle.  The callback will be called (on a native threadpool thread)
    //    EVERY TIME the address table becomes stable until CancelMibChangeNotify2 is called on the cancelHandle
    //    (via cancelHandle.Dispose()).
    //
    // CancelMibChangeNotify2 guarantees that, by the time it returns, all calls to the callback will be complete
    // and that no new calls to the callback will be issued.
    //
    // The major concerns of the class are: 1) making sure none of the managed objects needed to handle a native
    // callback are GC'd before the callback, and 2) making sure no native callbacks will try to call into an unloaded
    // AppDomain.

    internal class TeredoHelper
    {
        // Holds a list of all pending calls to NotifyStableUnicastIpAddressTable.  Also used as a lock to protect its
        // contents.
        private static List<TeredoHelper> pendingNotifications;

        private readonly Action<object> callback;
        private readonly object state;

        private bool runCallbackCalled;

        // We explicitly keep a copy of this to prevent it from getting GC'd.
        private readonly StableUnicastIpAddressTableDelegate onStabilizedDelegate;

        // Used to cancel notification after receiving the first callback, or when the AppDomain is going down.
        private SafeCancelMibChangeNotify cancelHandle;

        static TeredoHelper()
        {
            pendingNotifications = new List<TeredoHelper>();
        }

        private TeredoHelper(Action<object> callback, object state)
        {
            this.callback = callback;
            this.state = state;
            this.onStabilizedDelegate = new StableUnicastIpAddressTableDelegate(OnStabilized);
            this.runCallbackCalled = false;
        }

        // Returns true if the address table is already stable.  Otherwise, calls callback when it becomes stable.
        // 'Unsafe' because it does not flow ExecutionContext to the callback.
        public static bool UnsafeNotifyStableUnicastIpAddressTable(Action<object> callback, object state)
        {
            GlobalLog.Assert(callback != null,
                "UnsafeNotifyStableUnicastIpAddressTable called without specifying callback!");

            TeredoHelper helper = new TeredoHelper(callback, state);

            uint err = UnsafeCommonNativeMethods.ErrorCodes.ERROR_SUCCESS;
            SafeFreeMibTable table = null;

            lock (pendingNotifications)
            {
                // If OnAppDomainUnload gets the lock first, tell our caller that we'll finish async.  Their AppDomain 
                // is about to go down anyways.  If we do, hold the lock until we've added helper to the 
                // pendingNotifications list (if we're going to complete asynchronously).
                if (Environment.HasShutdownStarted)
                {
                    return false;
                }

                err = UnsafeNetInfoNativeMethods.NotifyStableUnicastIpAddressTable(AddressFamily.Unspecified,
                    out table, helper.onStabilizedDelegate, IntPtr.Zero, out helper.cancelHandle);

                if (table != null)
                {
                    table.Dispose();
                }

                if (err == UnsafeCommonNativeMethods.ErrorCodes.ERROR_IO_PENDING)
                {
                    GlobalLog.Assert(!helper.cancelHandle.IsInvalid,
                        "CancelHandle invalid despite returning ERROR_IO_PENDING");

                    // Async completion: add us to the pendingNotifications list so we'll be canceled in the
                    // event of an AppDomain unload.
                    pendingNotifications.Add(helper);
                    return false;
                }
            }

            if (err != UnsafeCommonNativeMethods.ErrorCodes.ERROR_SUCCESS)
            {
                throw new Win32Exception((int)err);
            }

            return true;
        }

        private void RunCallback(object o)
        {
            GlobalLog.Assert(runCallbackCalled, "RunCallback called without setting runCallbackCalled!");

            // If OnAppDomainUnload beats us to the lock, do nothing: the AppDomain is going down soon anyways.
            // Otherwise, wait until the call to CancelMibChangeNotify2 is done before giving it up.
            lock (pendingNotifications)
            {
                if (Environment.HasShutdownStarted)
                {
                    return;
                }

#if DEBUG
                bool successfullyRemoved = pendingNotifications.Remove(this);
                GlobalLog.Assert(successfullyRemoved,
                    "RunCallback for a TeredoHelper which is not in pendingNotifications!");
#else
                pendingNotifications.Remove(this);
#endif

                GlobalLog.Assert(cancelHandle != null && !cancelHandle.IsInvalid,
                    "Invalid cancelHandle in RunCallback");

                cancelHandle.Dispose();
            }

            callback.Invoke(state);
        }

        // This callback gets run on a native worker thread, which we don't want to allow arbitrary user code to
        // execute on (it will block AppDomain unload, for one).  Free the MibTable and delegate (exactly once)
        // to the managed ThreadPool for the rest of the processing.
        //
        // We can't use SafeHandle here for table because the marshaller doesn't support them in reverse p/invokes.
        // We won't get an AppDomain unload here anyways, because OnAppDomainUnloaded will block until all of these
        // callbacks are done.
        private void OnStabilized(IntPtr context, IntPtr table)
        {
            UnsafeNetInfoNativeMethods.FreeMibTable(table);

            // Lock the TeredoHelper instance to ensure that only the first call to OnStabilized will get to call 
            // RunCallback.  This is the only place that TeredoHelpers get locked, as individual instances are not
            // exposed to higher layers, so there's no chance for deadlock.
            if (!runCallbackCalled)
            {
                lock (this)
                {
                    if (!runCallbackCalled)
                    {
                        runCallbackCalled = true;
                        ThreadPool.QueueUserWorkItem(RunCallback, null);
                    }
                }
            }
        }
    }
}