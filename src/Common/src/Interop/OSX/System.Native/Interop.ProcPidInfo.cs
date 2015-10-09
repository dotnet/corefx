// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Sys
    {
        // Constants from sys/param.h
        private const int MAXCOMLEN = 16;

        // Constants from proc_info.h
        private const int MAXTHREADNAMESIZE = 64;
        private const int PROC_PIDTASKALLINFO = 2;
        private const int PROC_PIDTHREADINFO = 5;
        private const int PROC_PIDLISTTHREADS = 6;

        // Defines from proc_info.h
        internal enum ThreadRunState
        {
            TH_STATE_RUNNING            = 1,
            TH_STATE_STOPPED            = 2,
            TH_STATE_WAITING            = 3,
            TH_STATE_UNINTERRUPTIBLE    = 4,
            TH_STATE_HALTED             = 5
        }

        // Defines in proc_info.h
        [Flags]
        internal enum ThreadFlags
        {
            TH_FLAGS_SWAPPED    = 0x1,
            TH_FLAGS_IDLE       = 0x2
        }

        [StructLayout(LayoutKind.Sequential)]
        internal unsafe struct ProcBsdInfo
        {
            internal uint       pbi_flags;
            internal uint       pbi_status;
            internal uint       pbi_xstatus;
            internal uint       pbi_pid;
            internal uint       pbi_ppid;
            internal uint       pbi_uid;
            internal uint       pbi_gid;
            internal uint       pbi_ruid;
            internal uint       pbi_rgid;
            internal uint       pbi_svuid;
            internal uint       pbi_svgid;
            internal uint       reserved;
            internal fixed byte pbi_comm[MAXCOMLEN];
            internal fixed byte pbi_name[MAXCOMLEN * 2];
            internal uint       pbi_nfiles;
            internal uint       pbi_pgid;
            internal uint       pbi_pjobc;
            internal uint       e_tdev;
            internal uint       e_tpgid;
            internal int        pbi_nice;
            internal ulong      pbi_start_tvsec;
            internal ulong      pbi_start_tvusec;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct ProcTaskInfo
        {
            internal ulong   pti_virtual_size;
            internal ulong   pti_resident_size;
            internal ulong   pti_total_user;
            internal ulong   pti_total_system;
            internal ulong   pti_threads_user;
            internal ulong   pti_threads_system;
            internal int     pti_policy;
            internal int     pti_faults;
            internal int     pti_pageins;
            internal int     pti_cow_faults;
            internal int     pti_messages_sent;
            internal int     pti_messages_received;
            internal int     pti_syscalls_mach;
            internal int     pti_syscalls_unix;
            internal int     pti_csw;
            internal int     pti_threadnum;
            internal int     pti_numrunning;
            internal int     pti_priority;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct ProcTaskAllInfo
        {
            internal ProcBsdInfo    pbsd;
            internal ProcTaskInfo   ptinfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal unsafe struct ProcThreadInfo
        {
            internal ulong      pth_user_time;
            internal ulong      pth_system_time;
            internal int        pth_cpu_usage;
            internal int        pth_policy;
            internal int        pth_run_state;
            internal int        pth_flags;
            internal int        pth_sleep_time;
            internal int        pth_curpri;
            internal int        pth_priority;
            internal int        pth_maxpriority;
            internal fixed byte pth_name[MAXTHREADNAMESIZE];
        }

        /// <summary>
        /// Gets information about a process given it's PID
        /// </summary>
        /// <param name="pid">The PID of the process</param>
        /// <param name="flavor">Should be PROC_PIDTASKALLINFO</param>
        /// <param name="arg">Flavor dependent value</param>
        /// <param name="buffer">A pointer to a block of memory (of size ProcTaskAllInfo) allocated that will contain the data</param>
        /// <param name="bufferSize">The size of the allocated block above</param>
        /// <returns>
        /// The amount of data actually returned. If this size matches the bufferSize parameter then
        /// the data is valid. If the sizes do not match then the data is invalid, most likely due 
        /// to not having enough permissions to query for the data of that specific process
        /// </returns>
        [DllImport(Interop.Libraries.SystemNative, SetLastError = true)]
        private static unsafe extern int ProcPidInfo(
            int pid,
            int flavor,
            ulong arg,
            void* buffer,
            int bufferSize);

        /// <summary>
        /// Gets the process information for a given process
        /// </summary>
        /// <param name="pid">The PID (process ID) of the process</param>
        /// <returns>
        /// Returns a valid ProcTaskAllInfo struct for valid processes that the caller 
        /// has permission to access; otherwise, returns null
        /// </returns>
        internal static unsafe ProcTaskAllInfo? GetProcessInfoById(int pid)
        {
            // Negative PIDs are invalid
            if (pid < 0)
            {
                throw new ArgumentOutOfRangeException("pid");
            }

            // Get the process information for the specified pid
            int size = Marshal.SizeOf<ProcTaskAllInfo>();
            ProcTaskAllInfo info = default(ProcTaskAllInfo);
            int result = ProcPidInfo(pid, PROC_PIDTASKALLINFO, 0, &info, size);
            return (result == size ? new ProcTaskAllInfo?(info) : null);
        }

        /// <summary>
        /// Gets the thread information for the given thread
        /// </summary>
        /// <param name="thread">The ID of the thread to query for information</param>
        /// <returns>
        /// Returns a valid ProcThreadInfo struct for valid threads that the caller
        /// has permissions to access; otherwise, returns null
        /// </returns>
        internal static unsafe ProcThreadInfo? GetThreadInfoById(int pid, ulong thread)
        {
            // Negative PIDs are invalid
            if (pid < 0)
            {
                throw new ArgumentOutOfRangeException("pid");
            }

            // Negative TIDs are invalid
            if (thread < 0)
            {
                throw new ArgumentOutOfRangeException("thread");
            }

            // Get the thread information for the specified thread in the specified process
            int size = Marshal.SizeOf<ProcThreadInfo>();
            ProcThreadInfo info = default(ProcThreadInfo);
            int result = ProcPidInfo(pid, PROC_PIDTHREADINFO, (ulong)thread, &info, size);
            return (result == size ? new ProcThreadInfo?(info) : null);
        }

        internal static unsafe List<KeyValuePair<ulong, ProcThreadInfo?>> GetAllThreadsInProcess(int pid)
        {
            // Negative PIDs are invalid
            if (pid < 0)
            {
                throw new ArgumentOutOfRangeException("pid");
            }

            int result = 0;
            int size = 20; // start assuming 20 threads is enough
            ulong[] threadIds = null;
            var threads = new List<KeyValuePair<ulong, ProcThreadInfo?>>();

            // We have no way of knowning how many threads the process has (and therefore how big our buffer should be)
            // so while the return value of the function is the same as our buffer size (meaning it completely filled
            // our buffer), double our buffer size and try again. This ensures that we don't miss any threads
            do
            {
                threadIds = new ulong[size];
                fixed (ulong* pBuffer = threadIds)
                {
                    result = ProcPidInfo(pid, PROC_PIDLISTTHREADS, 0, pBuffer, Marshal.SizeOf<ulong>() * threadIds.Length);
                }

                if (result <= 0)
                {
                    // If we were unable to access the information, just return the empty list.  
                    // This is likely to happen for privileged processes, if the process went away
                    // by the time we tried to query it, etc.
                    return threads;
                }
                else
                {
                    checked
                    {
                        size *= 2;
                    }
                }
            }
            while (result == Marshal.SizeOf<ulong>() * threadIds.Length);

            System.Diagnostics.Debug.Assert((result % Marshal.SizeOf<ulong>()) == 0);

            // Loop over each thread and get the thread info
            int count = (int)(result / Marshal.SizeOf<ulong>());
            threads.Capacity = count;
            for (int i = 0; i < count; i++)
            {        
                threads.Add(new KeyValuePair<ulong, ProcThreadInfo?>(threadIds[i], GetThreadInfoById(pid, threadIds[i])));
            }

            return threads;
        }
    }
}
