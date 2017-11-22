// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;

namespace System.IO
{
    internal partial class FindEnumerable<T> : IEnumerable<T>
    {
        private unsafe partial class FindEnumerator : CriticalFinalizerObject, IEnumerator<T>
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool GetData()
            {
                uint status = Interop.NtDll.NtQueryDirectoryFile(
                    FileHandle: _directoryHandle,
                    Event: IntPtr.Zero,
                    ApcRoutine: IntPtr.Zero,
                    ApcContext: IntPtr.Zero,
                    IoStatusBlock: out Interop.NtDll.IO_STATUS_BLOCK statusBlock,
                    FileInformation: _buffer,
                    Length: (uint)_buffer.Length,
                    FileInformationClass: Interop.NtDll.FILE_INFORMATION_CLASS.FileFullDirectoryInformation,
                    ReturnSingleEntry: false,
                    FileName: null,
                    RestartScan: false);

                switch (status)
                {
                    case Interop.StatusOptions.STATUS_NO_MORE_FILES:
                        NoMoreFiles();
                        return false;
                    case Interop.StatusOptions.STATUS_SUCCESS:
                        Debug.Assert(statusBlock.Information.ToInt64() != 0);
                        return true;
                    default:
                        throw Win32Marshal.GetExceptionForWin32Error((int)Interop.Advapi32.LsaNtStatusToWinError(status), _currentPath);
                }
            }
        }
    }
}
