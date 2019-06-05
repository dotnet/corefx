// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Microsoft.Win32.SafeHandles;

namespace System.IO.Pipes.Tests
{
    /// <summary>
    /// The class contains interop declarations and helpers methods for them.
    /// </summary>
    internal static partial class Interop
    {
        private const string Kernel32 = "kernel32.dll";

        private const int ERROR_SUCCESS = 0x0;
        private const int ERROR_CANNOT_IMPERSONATE = 0x558;

        private const int LOAD_LIBRARY_SEARCH_SYSTEM32 = 0x00000800;

        private const int CREDUI_MAX_USERNAME_LENGTH = 513;

        sealed internal class SafeLibraryHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
            [DllImport(Kernel32, CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
            private static extern bool FreeLibrary(IntPtr hModule);

            internal SafeLibraryHandle() : base(true) { }

            override protected bool ReleaseHandle()
            {
                return FreeLibrary(handle);
            }
        }

        [DllImport(Kernel32, EntryPoint = "LoadLibraryExW", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern SafeLibraryHandle LoadLibraryEx(string libFilename, IntPtr reserved, int flags);

        [DllImport(Kernel32, SetLastError = true)]
        private static extern unsafe bool CancelIoEx(SafeHandle handle, NativeOverlapped* lpOverlapped);

        internal static unsafe bool CancelIoEx(SafeHandle handle)
        {
            return CancelIoEx(handle, null);
        }

        [DllImport(Kernel32, CharSet = CharSet.Unicode, SetLastError = true, BestFitMapping = false, ExactSpelling = true)]
        internal static extern unsafe bool GetNamedPipeHandleStateW(
            SafePipeHandle hNamedPipe,
            IntPtr lpState,
            out int lpCurInstances,
            IntPtr lpMaxCollectionCount,
            IntPtr lpCollectDataTimeout,
            StringBuilder lpUserName,
            int nMaxUserNameSize);

        internal static bool TryGetImpersonationUserName(SafePipeHandle handle, out string userName)
        {
            const int UserNameMaxLength = CREDUI_MAX_USERNAME_LENGTH + 1;
            var builder = new StringBuilder(UserNameMaxLength);

            if (GetNamedPipeHandleStateW(handle, IntPtr.Zero, out _, IntPtr.Zero, IntPtr.Zero, builder, builder.Capacity))
            {
                userName = builder.ToString();
                return true;
            }

            return TryHandleGetImpersonationUserNameError(handle, Marshal.GetLastWin32Error(), builder, out userName);
        }

        internal static bool TryGetNumberOfServerInstances(SafePipeHandle handle, out int numberOfServerInstances)
        {
            int serverInstances;

            if (GetNamedPipeHandleStateW(handle, IntPtr.Zero, out serverInstances, IntPtr.Zero, IntPtr.Zero, null, 0))
            {
                numberOfServerInstances = serverInstances;
                return true;
            }

            numberOfServerInstances = 0;
            return false;
        }

        // @todo: These are called by some Unix-specific tests. Those tests should really be split out into
        // partial classes and included only in Unix builds.
        internal static bool TryGetHostName(out string hostName) { throw new Exception("Should not call on Windows."); }
    }
}
