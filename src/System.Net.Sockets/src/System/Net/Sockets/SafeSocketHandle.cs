// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Win32.SafeHandles;

using System.Diagnostics;
using System.Threading;

namespace System.Net.Sockets
{
    // This class implements a safe socket handle.
    // It uses an inner and outer SafeHandle to do so.  The inner
    // SafeHandle holds the actual socket, but only ever has one
    // reference to it.  The outer SafeHandle guards the inner
    // SafeHandle with real ref counting.  When the outer SafeHandle
    // is cleaned up, it releases the inner SafeHandle - since
    // its ref is the only ref to the inner SafeHandle, it deterministically
    // gets closed at that point - no races with concurrent IO calls.
    // This allows Close() on the outer SafeHandle to deterministically
    // close the inner SafeHandle, in turn allowing the inner SafeHandle
    // to block the user thread in case a graceful close has been
    // requested.  (It's not legal to block any other thread - such closes
    // are always abortive.)
    public sealed partial class SafeSocketHandle : SafeHandleMinusOneIsInvalid
    {
#if DEBUG
        private SocketError _closeSocketResult = unchecked((SocketError)0xdeadbeef);
        private SocketError _closeSocketLinger = unchecked((SocketError)0xdeadbeef);
        private int _closeSocketThread;
        private int _closeSocketTick;
#endif
        private int _ownClose;

        public SafeSocketHandle(IntPtr preexistingHandle, bool ownsHandle)
            : base(ownsHandle)
        {
            SetHandleAndValid(preexistingHandle);
        }

        private SafeSocketHandle() : base(true) { }

        private bool TryOwnClose()
        {
            return Interlocked.CompareExchange(ref _ownClose, 1, 0) == 0;
        }

        private volatile bool _released;
        private bool _hasShutdownSend;

        internal void TrackShutdown(SocketShutdown how)
        {
            if (how == SocketShutdown.Send ||
                how == SocketShutdown.Both)
            {
                _hasShutdownSend = true;
            }
        }

        public override bool IsInvalid
        {
            get
            {
                return IsClosed || base.IsInvalid;
            }
        }

        protected override bool ReleaseHandle()
        {
            _released = true;
            bool shouldClose = TryOwnClose();

            if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"shouldClose={shouldClose}");

            // When shouldClose is true, the user called Dispose on the SafeHandle.
            // When it is false, the handle was closed from the Socket via CloseAsIs.
            if (shouldClose)
            {
                CloseHandle(abortive: true, canceledOperations: false);
            }

            return true;
        }

        internal void CloseAsIs(bool abortive)
        {
#if DEBUG
            // If this throws it could be very bad.
            try
            {
#endif
                bool shouldClose = TryOwnClose();

                if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"shouldClose={shouldClose}");

                Dispose();

                if (shouldClose)
                {
                    bool canceledOperations = false;

                    // Wait until it's safe.
                    SpinWait sw = new SpinWait();
                    while (!_released)
                    {
                        // The socket was not released due to the SafeHandle being used.
                        // Try to make those on-going calls return.
                        // On Linux, TryUnblockSocket will unblock current operations but it doesn't prevent
                        // a new one from starting. So we must call TryUnblockSocket multiple times.
                        canceledOperations |= TryUnblockSocket(abortive);
                        sw.SpinOnce();
                    }

                    CloseHandle(abortive, canceledOperations);
                }
#if DEBUG
            }
            catch (Exception exception) when (!ExceptionCheck.IsFatal(exception))
            {
                NetEventSource.Fail(this, $"handle:{handle}, error:{exception}");
                throw;
            }
#endif
        }

        private bool CloseHandle(bool abortive, bool canceledOperations)
        {
            bool ret = false;

#if DEBUG
            try
            {
#endif
                if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"handle:{handle}");

                canceledOperations |= OnHandleClose();

                // In case we cancel operations, switch to an abortive close.
                // Unless the user requested a normal close using Socket.Shutdown.
                if (canceledOperations && !_hasShutdownSend)
                {
                    abortive = true;
                }

                SocketError errorCode = DoCloseHandle(abortive);
                return ret = errorCode == SocketError.Success;
#if DEBUG
            }
            catch (Exception exception)
            {
                if (!ExceptionCheck.IsFatal(exception))
                {
                    NetEventSource.Fail(this, $"handle:{handle}, error:{exception}");
                }

                ret = true;  // Avoid a second assert.
                throw;
            }
            finally
            {
                _closeSocketThread = Environment.CurrentManagedThreadId;
                _closeSocketTick = Environment.TickCount;
                if (!ret)
                {
                    NetEventSource.Fail(this, $"ReleaseHandle failed. handle:{handle}");
                }
            }
#endif
        }

        private void SetHandleAndValid(IntPtr handle)
        {
            Debug.Assert(!IsClosed);

            base.SetHandle(handle);

            if (IsInvalid)
            {
                // CloseAsIs musn't wait for a release.
                TryOwnClose();

                // Mark handle as invalid, so it won't be released.
                SetHandleAsInvalid();
            }
        }
    }
}
