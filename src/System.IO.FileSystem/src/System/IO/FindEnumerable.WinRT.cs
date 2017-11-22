// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

namespace System.IO
{
    internal partial class FindEnumerable<T> : IEnumerable<T>
    {
        private unsafe partial class FindEnumerator : CriticalFinalizerObject, IEnumerator<T>
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool GetData()
            {
                if (!Interop.Kernel32.GetFileInformationByHandleEx(
                   _directoryHandle,
                   Interop.Kernel32.FILE_INFO_BY_HANDLE_CLASS.FileFullDirectoryInfo,
                   _buffer,
                   (uint)_buffer.Length))
                {
                    int error = Marshal.GetLastWin32Error();
                    switch (error)
                    {
                        case Interop.Errors.ERROR_NO_MORE_FILES:
                            NoMoreFiles();
                            return false;
                        default:
                            throw Win32Marshal.GetExceptionForWin32Error(Marshal.GetLastWin32Error(), _currentPath);
                    }
                }

                return true;
            }
        }
    }
}
