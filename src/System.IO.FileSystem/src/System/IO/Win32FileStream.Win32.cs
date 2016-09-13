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
            return handle.IsAsync ?? !IsHandleSynchronous(handle) ?? DefaultIsAsync;
        }

        private static unsafe bool? IsHandleSynchronous(SafeFileHandle fileHandle)
        {
            if (fileHandle.IsInvalid)
                return null;

            Interop.NtDll.IO_STATUS_BLOCK ioStatus;
            uint fileMode;

            int status = Interop.NtDll.NtQueryInformationFile(
                FileHandle: fileHandle,
                IoStatusBlock: out ioStatus,
                FileInformation: &fileMode,
                Length: sizeof(uint),
                FileInformationClass: Interop.NtDll.FileModeInformation);

            switch (status)
            {
                case 0:
                    // We we're successful
                    break;
                case Interop.NtDll.STATUS_INVALID_HANDLE:
                    fileHandle.Dispose();
                    throw Win32Marshal.GetExceptionForWin32Error(Interop.mincore.Errors.ERROR_INVALID_HANDLE);
                default:
                    // Something else is preventing access
                    Debug.Fail("Unable to get the file mode information, status was" + status.ToString());
                    return null;
            }

            // If either of these two flags are set, the file handle is synchronous (not overlapped)
            return (fileMode & (Interop.NtDll.FILE_SYNCHRONOUS_IO_ALERT | Interop.NtDll.FILE_SYNCHRONOUS_IO_NONALERT)) > 0;
        }

        private void VerifyHandleIsSync(int fileType)
        {
            // As we can accurately check the handle type when we have access to NtQueryInformationFile we don't need to skip for
            // any particular file handle type.

            // If the handle was passed in without an explicit async setting, we already looked it up in GetDefaultIsAsync
            if (!_handle.IsAsync.HasValue) return;

            // If we can't check the handle, just assume it is ok.
            if (!(IsHandleSynchronous(_handle) ?? true))
                throw new ArgumentException(SR.Arg_HandleNotSync, "handle");
        }
    }
}