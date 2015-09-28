// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Microsoft.Win32.SafeHandles
{
    [System.Security.SecurityCritical]
    sealed partial class SafeFileHandle : SafeHandle
    {
        /// <summary>A handle value of -1.</summary>
        private static readonly IntPtr s_invalidHandle = new IntPtr(-1);

        private SafeFileHandle(bool ownsHandle)
            : base(s_invalidHandle, ownsHandle)
        {
        }

        /// <summary>Opens the specified file with the requested flags and mode.</summary>
        /// <param name="path">The path to the file.</param>
        /// <param name="flags">The flags with which to open the file.</param>
        /// <param name="mode">The mode for opening the file.</param>
        /// <returns>A SafeFileHandle for the opened file.</returns>
        internal static SafeFileHandle Open(string path, Interop.Sys.OpenFlags flags, int mode)
        {
            Debug.Assert(path != null);

            SafeFileHandle handle = new SafeFileHandle(ownsHandle: true);

            // If we fail to open the file due to a path not existing, we need to know whether to blame
            // the file itself or its directory.  If we're creating the file, then we blame the directory,
            // otherwise we blame the file.
            bool enoentDueToDirectory = (flags & Interop.Sys.OpenFlags.O_CREAT) != 0;

            // Open the file. 
            int fd;
            while (Interop.CheckIo(fd = Interop.Sys.Open(path, flags, mode), path, isDirectory: enoentDueToDirectory,
                errorRewriter: e => (e.Error == Interop.Error.EISDIR) ? Interop.Error.EACCES.Info() : e)) ;
            handle.SetHandle(fd);

            // Make sure it's not a directory; we do this after opening it once we have a file descriptor 
            // to avoid race conditions.
            Interop.Sys.FileStatus status;
            if (Interop.Sys.FStat(fd, out status) != 0)
            {
                handle.Dispose();
                throw Interop.GetExceptionForIoErrno(Interop.Sys.GetLastErrorInfo(), path);
            }
            if ((status.Mode & Interop.Sys.FileTypes.S_IFMT) == Interop.Sys.FileTypes.S_IFDIR)
            {
                handle.Dispose();
                throw Interop.GetExceptionForIoErrno(Interop.Error.EACCES.Info(), path, isDirectory: true);
            }

            return handle;
        }

        /// <summary>Opens a SafeFileHandle for a file descriptor created by a provided delegate.</summary>
        /// <param name="fdFunc">
        /// The function that creates the file descriptor. Returns the file descriptor on success, or -1 on error,
        /// with Marshal.GetLastWin32Error() set to the error code.
        /// </param>
        /// <returns>The created SafeFileHandle.</returns>
        internal static SafeFileHandle Open(Func<int> fdFunc)
        {
            var handle = new SafeFileHandle(ownsHandle: true);
            int fd;
            while (Interop.CheckIo(fd = fdFunc())) ;
            handle.SetHandle(fd);
            return handle;
        }

        private void SetHandle(int fd)
        {
            SetHandle((IntPtr)fd);
            Debug.Assert(!IsInvalid, "File descriptor is invalid");
        }

        [System.Security.SecurityCritical]
        protected override bool ReleaseHandle()
        {
            int fd = (int)handle;

            // When the SafeFileHandle was opened, we likely issued an flock on the created descriptor in order to add 
            // an advisory lock.  This lock should be removed via closing the file descriptor, but close can be
            // interrupted, and we don't retry closes.  As such, we could end up leaving the file locked,
            // which could prevent subsequent usage of the file until this process dies.  To avoid that, we proactively
            // try to release the lock before we close the handle. (If it's not locked, there's no behavioral
            // problem trying to unlock it.)
            Interop.Sys.FLock(fd, Interop.Sys.LockOperations.LOCK_UN); // ignore any errors

            // Close the descriptor. Although close is documented to potentially fail with EINTR, we never want
            // to retry, as the descriptor could actually have been closed, been subsequently reassigned, and
            // be in use elsewhere in the process.  Instead, we simply check whether the call was successful.
            int result = Interop.Sys.Close(fd);
#if DEBUG
            if (result != 0)
            {
                Debug.Fail(string.Format(
                    "Close failed with result {0} and error {1}", 
                    result, Interop.Sys.GetLastErrorInfo()));
            }
#endif
            return result == 0;
        }

        public override bool IsInvalid
        {
            [System.Security.SecurityCritical]
            get
            {
                long h = (long)handle;
                return h < 0 || h > int.MaxValue;
            }
        }
    }
}
