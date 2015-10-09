// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Sys
    {
        [DllImport(Libraries.SystemNative, SetLastError = true)]
        internal static unsafe extern int ProcListAllPids(int* buffer, int buffersize);

        /// <summary>
        /// Queries the OS for the list of all running processes and returns the PID for each
        /// </summary>
        /// <returns>Returns a list of PIDs corresponding to all running processes</returns>
        internal static unsafe int[] ProcListAllPids()
        {
            // Get the number of processes currently running to know how much data to allocate
            int numProcesses = ProcListAllPids(null, 0);
            if (numProcesses <= 0)
            {
                throw new System.ComponentModel.Win32Exception(SR.CantGetAllPids);
            }

            int[] processes;

            do
            {
                // Create a new array for the processes (plus a 10% buffer in case new processes have spawned)
                // Since we don't know how many threads there could be, if result == size, that could mean two things
                // 1) We guessed exactly how many processes there are
                // 2) There are more processes that we didn't get since our buffer is too small
                // To make sure it isn't #2, when the result == size, increase the buffer and try again
                processes = new int[(int)(numProcesses * 1.10)];

                fixed (int* pBuffer = processes)
                {
                    numProcesses = ProcListAllPids(pBuffer, processes.Length * Marshal.SizeOf<int>());
                    if (numProcesses <= 0)
                    {
                        throw new System.ComponentModel.Win32Exception(SR.CantGetAllPids);
                    }
                }
            }
            while (numProcesses == processes.Length);

            // Remove extra elements
            Array.Resize<int>(ref processes, numProcesses);

            return processes;
        }
    }
}
