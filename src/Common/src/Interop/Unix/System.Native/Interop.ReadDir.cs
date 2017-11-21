// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Win32.SafeHandles;

internal static partial class Interop
{
    internal static partial class Sys
    {
        private static readonly int s_readBufferSize = GetReadDirRBufferSize();

        internal enum NodeType : int
        {
            DT_UNKNOWN  =  0,
            DT_FIFO     =  1,
            DT_CHR      =  2,
            DT_DIR      =  4,
            DT_BLK      =  6,
            DT_REG      =  8,
            DT_LNK      = 10,
            DT_SOCK     = 12,
            DT_WHT      = 14
        }

        [StructLayout(LayoutKind.Sequential)]
        private unsafe struct InternalDirectoryEntry
        {
            internal IntPtr     Name;
            internal int        NameLength;
            internal NodeType   InodeType;
        }

        internal struct DirectoryEntry
        {
            internal NodeType   InodeType;
            internal string     InodeName;
        }

        [DllImport(Libraries.SystemNative, EntryPoint = "SystemNative_OpenDir", SetLastError = true)]
        internal static extern Microsoft.Win32.SafeHandles.SafeDirectoryHandle OpenDir(string path);

        [DllImport(Libraries.SystemNative, EntryPoint = "SystemNative_GetReadDirRBufferSize", SetLastError = false)]
        internal static extern int GetReadDirRBufferSize();

        [DllImport(Libraries.SystemNative, EntryPoint = "SystemNative_ReadDirR", SetLastError = false)]
        private static extern unsafe int ReadDirR(IntPtr dir, byte* buffer, int bufferSize, out InternalDirectoryEntry outputEntry);

        [DllImport(Libraries.SystemNative, EntryPoint = "SystemNative_CloseDir", SetLastError = true)]
        internal static extern int CloseDir(IntPtr dir);

        // The calling pattern for ReadDir is described in src/Native/System.Native/pal_readdir.cpp
        internal static int ReadDir(SafeDirectoryHandle dir, out DirectoryEntry outputEntry)
        {
            bool addedRef = false;
            try
            {
                // We avoid a native string copy into InternalDirectoryEntry.
                // - If the platform suppors reading into a buffer, the data is read directly into the stackalloced buffer.
                // - If the platform does not support reading into a buffer, the information returned in
                // InternalDirectoryEntry points to native memory owned by the SafeDirectoryHandle.
                // We extend the reference until we have copied all data from that native memory to ensure
                // it does not become invalid by a CloseDir; or by a concurrent ReadDir call.
                int readerCount = Interlocked.Increment(ref dir.ReadDirCounter);
                if (readerCount != 1)
                {
                    ThrowConcurrentReadNotSupported();
                }
                dir.DangerousAddRef(ref addedRef);

                unsafe
                {
                    // note: s_readBufferSize is zero when the native implementation does not support reading into a buffer.
                    byte* buffer = stackalloc byte[s_readBufferSize];
                    InternalDirectoryEntry temp;
                    int ret = ReadDirR(dir.DangerousGetHandle(), buffer, s_readBufferSize, out temp);
                    outputEntry = ret == 0 ?
                                new DirectoryEntry() { InodeName = GetDirectoryEntryName(temp), InodeType = temp.InodeType } : 
                                default(DirectoryEntry);

                    return ret;
                }
            }
            finally
            {
                if (addedRef)
                {
                    dir.DangerousRelease();
                }
                Interlocked.Decrement(ref dir.ReadDirCounter);
            }
        }

        private static unsafe string GetDirectoryEntryName(InternalDirectoryEntry dirEnt)
        {
            if (dirEnt.NameLength == -1)
                return Marshal.PtrToStringAnsi(dirEnt.Name);
            else
                return Marshal.PtrToStringAnsi(dirEnt.Name, dirEnt.NameLength);
        }

        private static void ThrowConcurrentReadNotSupported()
        {
            throw new NotSupportedException("Concurrent directory enumeration is not supported.");
        }
    }
}
