// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Win32.SafeHandles;

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace System.Net.Sockets
{
#if DEBUG
    internal sealed class SafeOverlappedFree : DebugSafeHandle
    {
#else
    internal sealed class SafeOverlappedFree : SafeHandleZeroOrMinusOneIsInvalid
    {
#endif
        private static readonly SafeOverlappedFree s_zero = new SafeOverlappedFree(false);

        private SafeCloseSocket _socketHandle;

        internal static SafeOverlappedFree Zero { get { return s_zero; } }

        private SafeOverlappedFree() : base(true) { }
        private SafeOverlappedFree(bool ownsHandle) : base(ownsHandle) { }

        public static SafeOverlappedFree Alloc()
        {
            SafeOverlappedFree result = Interop.mincore_obsolete.LocalAlloc_SafeOverlappedFree(
                                            Interop.mincore_obsolete.LPTR,
                                            (UIntPtr)Interop.mincore_obsolete.OverlappedSize);

            if (result.IsInvalid)
            {
                result.SetHandleAsInvalid();
                throw new OutOfMemoryException();
            }

            return result;
        }

        public static SafeOverlappedFree Alloc(SafeCloseSocket socketHandle)
        {
            SafeOverlappedFree result = Alloc();
            result._socketHandle = socketHandle;
            return result;
        }

        public void Close(bool resetOwner)
        {
            if (resetOwner)
            {
                _socketHandle = null;
            }

            Dispose();
        }

        unsafe override protected bool ReleaseHandle()
        {
            SafeCloseSocket socketHandle = _socketHandle;
            if (socketHandle != null && !socketHandle.IsInvalid)
            {
                // We are being finalized while the I/O operation associated
                // with the current overlapped is still pending (e.g. on app
                // domain shutdown). The socket has to be closed first to
                // avoid reuse after delete of the native overlapped structure.
                socketHandle.Dispose();
            }

            // Release the native overlapped structure.
            return Interop.mincore_obsolete.LocalFree(handle) == IntPtr.Zero;
        }
    }
}

