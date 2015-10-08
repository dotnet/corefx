// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Sys
    {
        // Constants from sys/resource.h
        private const int RUSAGE_SELF = 0;

        [StructLayout(LayoutKind.Sequential)]
        internal unsafe struct RUsageInfo
        {
            internal fixed byte     ri_uuid[16];
            internal ulong          ri_user_time;
            internal ulong          ri_system_time;
            internal ulong          ri_pkg_idle_wkups;
            internal ulong          ri_interrupt_wkups;
            internal ulong          ri_pageins;
            internal ulong          ri_wired_size;
            internal ulong          ri_resident_size;
            internal ulong          ri_phys_footprint;
            internal ulong          ri_proc_start_abstime;
            internal ulong          ri_proc_exit_abstime;
            internal ulong          ri_child_user_time;
            internal ulong          ri_child_system_time;
            internal ulong          ri_child_pkg_idle_wkups;
            internal ulong          ri_child_interrupt_wkups;
            internal ulong          ri_child_pageins;
            internal ulong          ri_child_elapsed_abstime;
            internal ulong          ri_diskio_bytesread;
            internal ulong          ri_diskio_byteswritten;
            internal ulong          ri_cpu_time_qos_default;
            internal ulong          ri_cpu_time_qos_maintenance;
            internal ulong          ri_cpu_time_qos_background;
            internal ulong          ri_cpu_time_qos_utility;
            internal ulong          ri_cpu_time_qos_legacy;
            internal ulong          ri_cpu_time_qos_user_initiated;
            internal ulong          ri_cpu_time_qos_user_interactive;
            internal ulong          ri_billed_system_time;
            internal ulong          ri_serviced_system_time;
        }

        /// <summary>
        /// Gets the rusage information for the process identified by the PID
        /// </summary>
        /// <param name="pid">The process to retrieve the rusage for</param>
        /// <param name="flavor">Should be RUSAGE_SELF to specify getting the info for the specified process</param>
        /// <param name="rusage_info_t">A buffer to be filled with rusage_info data</param>
        /// <returns>Returns 0 on success; on fail, -1 and errno is set with the error code</returns>
        [DllImport(Interop.Libraries.SystemNative, SetLastError = true)]
        private static unsafe extern int ProcPidRUsage(
            int pid,
            int flavor,
            RUsageInfo* rusage_info_t);

        /// <summary>
        /// Gets the rusage information for the process identified by the PID
        /// </summary>
        /// <param name="pid">The process to retrieve the rusage for</param>
        /// <returns>On success, returns a struct containing info about the process; on
        /// failure or when the caller doesn't have permissions to the process, throws a Win32Exception
        /// </returns>
        internal static unsafe RUsageInfo ProcPidRUsage(int pid)
        {
            // Negative PIDs are invalid
            if (pid < 0)
            {
                throw new ArgumentOutOfRangeException("pid", SR.NegativePidNotSupported);
            }

            RUsageInfo info = new RUsageInfo();

            // Get the PIDs rusage info
            int result = ProcPidRUsage(pid, RUSAGE_SELF, &info);
            if (result < 0)
            {
                throw new System.ComponentModel.Win32Exception(SR.RUsageFailure);
            }

            return info;
        }
    }
}
