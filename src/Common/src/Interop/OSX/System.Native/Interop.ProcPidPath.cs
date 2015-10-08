// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Sys
    {
        private const int MAXPATHLEN = 1024;
        private const int PROC_PIDPATHINFO_MAXSIZE = 4 * MAXPATHLEN;

        /// <summary>
        /// Gets the full path to the executable file identified by the specified PID
        /// </summary>
        /// <param name="pid">The PID of the running process</param>
        /// <param name="buffer">A pointer to an allocated block of memory that will be filled with the process path</param>
        /// <param name="bufferSize">The size of the buffer, should be PROC_PIDPATHINFO_MAXSIZE</param>
        /// <returns>Returns the length of the path returned on success</returns>
        [DllImport(Libraries.SystemNative, SetLastError = true)]
        private static unsafe extern int ProcPidPath(
            int pid, 
            byte* buffer, 
            uint bufferSize);

        /// <summary>
        /// Gets the full path to the executable file identified by the specified PID
        /// </summary>
        /// <param name="pid">The PID of the running process</param>
        /// <returns>Returns the full path to the process executable</returns>
        internal static unsafe string ProcPidPath(int pid)
        {
            // Negative PIDs are invalid
            if (pid < 0)
            {
                throw new ArgumentOutOfRangeException("pid", SR.NegativePidNotSupported);
            }

            // The path is a fixed buffer size, so use that and trim it after
            int result = 0;
            byte* pBuffer = stackalloc byte[PROC_PIDPATHINFO_MAXSIZE];
            result = ProcPidPath(pid, pBuffer, (uint)(PROC_PIDPATHINFO_MAXSIZE * Marshal.SizeOf<byte>()));
            if (result <= 0)
            {
                throw new System.ComponentModel.Win32Exception();
            }

            return System.Text.Encoding.UTF8.GetString(pBuffer, result);
        }
    }
}
