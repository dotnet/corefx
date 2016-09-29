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
    internal static class Interop
    {
        #region Windows

        [DllImport("api-ms-win-core-io-l1-1-0.dll", SetLastError = true)]
        private static extern unsafe bool CancelIoEx(SafeHandle handle, NativeOverlapped* lpOverlapped);

        internal static unsafe bool CancelIoEx(SafeHandle handle)
        {
            return CancelIoEx(handle, null);
        }

        [DllImport("api-ms-win-core-namedpipe-l1-2-1.dll", CharSet = CharSet.Unicode, SetLastError = true, BestFitMapping = false, EntryPoint = "GetNamedPipeHandleStateW")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetNamedPipeHandleState(
            SafePipeHandle hNamedPipe,
            IntPtr lpState,
            out int lpCurInstances,
            IntPtr lpMaxCollectionCount,
            IntPtr lpCollectDataTimeout,
            StringBuilder lpUserName,
            int nMaxUserNameSize);

        internal static bool TryGetImpersonationUserName(SafePipeHandle handle, out string userName)
        {
            const int CREDUI_MAX_USERNAME_LENGTH = 513;
            int serverInstances;
            var builder = new StringBuilder(CREDUI_MAX_USERNAME_LENGTH + 1);

            if (GetNamedPipeHandleState(handle, IntPtr.Zero, out serverInstances, IntPtr.Zero, IntPtr.Zero, builder, builder.Capacity))
            {
                userName = builder.ToString();
                return true;
            }

            userName = "";
            return false;
        }

        internal static bool TryGetNumberOfServerInstances(SafePipeHandle handle, out int numberOfServerInstances)
        {
            int serverInstances;

            if (GetNamedPipeHandleState(handle, IntPtr.Zero, out serverInstances, IntPtr.Zero, IntPtr.Zero, null, 0))
            {
                numberOfServerInstances = serverInstances;
                return true;
            }

            numberOfServerInstances = 0;
            return false;
        }

        #endregion

        #region Unix

        [DllImport("libc", SetLastError = true)]
        private static extern unsafe int gethostname(byte* name, int len);

        internal static unsafe bool TryGetHostName(out string hostName)
        {
            const int HOST_NAME_MAX = 255; // man gethostname
            const int ArrLength = HOST_NAME_MAX + 1;

            byte* name = stackalloc byte[ArrLength];
            int result = gethostname(name, ArrLength);

            if (result == 0)
            {
                hostName = Marshal.PtrToStringAnsi((IntPtr)name);
                return true;
            }

            hostName = "";
            return false;
        }

        #endregion
    }
}
