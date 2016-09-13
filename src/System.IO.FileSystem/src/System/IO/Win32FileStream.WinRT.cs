// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Win32.SafeHandles;
using System.Diagnostics;

namespace System.IO
{
    internal sealed partial class Win32FileStream : FileStreamBase
    {
        private static bool GetDefaultIsAsync(SafeFileHandle handle)
        {
            return handle.IsAsync.HasValue ? handle.IsAsync.Value : DefaultIsAsync;
        }

        private unsafe bool? IsHandleSynchronous()
        {
            // Do NOT use this method on any type other than DISK. Reading or writing to a pipe may
            // cause an app to block incorrectly, introducing a deadlock (depending on whether a write
            // will wake up an already-blocked thread or this Win32FileStream's thread).

            byte* bytes = stackalloc byte[1];
            int numBytesReadWritten;
            int r = -1;

            // If the handle is a pipe, ReadFile will block until there
            // has been a write on the other end.  We'll just have to deal with it,
            // For the read end of a pipe, you can mess up and 
            // accidentally read synchronously from an async pipe.
            if (_canRead)
                r = Interop.mincore.ReadFile(_handle, bytes, 0, out numBytesReadWritten, IntPtr.Zero);
            else if (_canWrite)
                r = Interop.mincore.WriteFile(_handle, bytes, 0, out numBytesReadWritten, IntPtr.Zero);

            if (r == 0)
            {
                int errorCode = GetLastWin32ErrorAndDisposeHandleIfInvalid();
                switch (errorCode)
                {
                    case ERROR_INVALID_PARAMETER:
                        return false;
                    case Interop.mincore.Errors.ERROR_INVALID_HANDLE:
                        throw Win32Marshal.GetExceptionForWin32Error(errorCode);
                }
            }

            return true;
        }

        private void VerifyHandleIsSync(int fileType)
        {
            // The technique here only really works for FILE_TYPE_DISK. FileMode is the right thing to check, but it currently
            // isn't available in WinRT.

            if (fileType == Interop.mincore.FileTypes.FILE_TYPE_DISK)
            {
                // If we can't check the handle, just assume it is ok.
                if (!(IsHandleSynchronous() ?? true))
                    throw new ArgumentException(SR.Arg_HandleNotSync, "handle");
            }
        }
    }
}