// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;


internal static partial class Interop
{
    internal static partial class libutil
    {
        internal const string lib_name = "libutil";

        // Constants from sys/syslimits.h
        private const int PATH_MAX  = 1024;

        // Constants from sys/user.h
        private const int TDNAMLEN = 16;
        private const int WMESGLEN = 8;
        private const int LOGNAMELEN = 17;
        private const int LOCKNAMELEN = 8;
        private const int COMMLEN = 19;
        private const int KI_EMULNAMELEN = 16;
        private const int LOGINCLASSLEN = 17;
        private const int KI_NGROUPS = 16;

        // Constants from sys/_sigset.h
        private const int _SIG_WORDS = 4;

        // Constants from sys/sysctl.h
        private const int CTL_KERN = 1;
        private const int KERN_PROC = 14;
        private const int KERN_PROC_PATHNAME = 12;

        // Constants from proc_info.h
        private const int MAXTHREADNAMESIZE = 64;
        private const int PROC_PIDTASKALLINFO = 2;
        private const int PROC_PIDTHREADINFO = 5;
        private const int PROC_PIDLISTTHREADS = 6;
        private const int PROC_PIDPATHINFO_MAXSIZE = 4 * PATH_MAX;

        // From sys/_sigset.h
        [StructLayout(LayoutKind.Sequential)]
        internal unsafe struct sigset_t
        {
            fixed int bits[4];
        }

        [StructLayout(LayoutKind.Sequential)]
        internal unsafe struct uid_t
        {
            public uint id;
        }
        [StructLayout(LayoutKind.Sequential)]
        internal unsafe struct gid_t
        {
            uint id;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct timeval
        {
            public long tv_sec;
            public long tv_usec;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct vnode
        {
            long tv_sec;
            long tv_usec;
        }

        // sys/resource.h
        [StructLayout(LayoutKind.Sequential)]
        internal unsafe struct rusage
        {
            public timeval ru_utime;               /* user time used */
            public timeval ru_stime;               /* system time used */
            long    ru_maxrss;              /* max resident set size */
            long    ru_ixrss;               /* integral shared memory size */
            long    ru_idrss;               /* integral unshared data " */
            long    ru_isrss;               /* integral unshared stack " */
            long    ru_minflt;              /* page reclaims */
            long    ru_majflt;              /* page faults */
            long    ru_nswap;               /* swaps */
            long    ru_inblock;             /* block input operations */
            long    ru_oublock;             /* block output operations */
            long    ru_msgsnd;              /* messages sent */
            long    ru_msgrcv;              /* messages received */
            long    ru_nsignals;            /* signals received */
            long    ru_nvcsw;               /* voluntary context switches */
            long    ru_nivcsw;              /* involuntary " */
        }
        
        // From  sys/user.h
        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct kinfo_proc
        {
            public int ki_structsize;           /* size of this structure */
            int     ki_layout;                  /* reserved: layout identifier */
            void *ki_args;                      /* address of command arguments */
            void *ki_paddr;                     /* address of proc */
            void *ki_addr;                      /* kernel virtual addr of u-area */
            vnode *ki_tracep;                   /* pointer to trace file */
            vnode *ki_textvp;                   /* pointer to executable file */
            void *ki_fd;                        /* pointer to open file info */
            void *ki_vmspace;                   /* pointer to kernel vmspace struct */
            void    *ki_wchan;                  /* sleep address */
            public int ki_pid;                 /* Process identifier */
            public int ki_ppid;                /* parent process id */
            int ki_pgid;                        /* process group id */
            int ki_tpgid;                       /* tty process group id */
            int ki_sid;                         /* Process session ID */
            int ki_tsid;                        /* Terminal session ID */
            short   ki_jobc;                    /* job control counter */
            short   ki_spare_short1;            /* unused (just here for alignment) */
            int ki_tdev;                        /* controlling tty dev */
            sigset_t ki_siglist;                /* Signals arrived but not delivered */
            sigset_t ki_sigmask;                /* Current signal mask */
            sigset_t ki_sigignore;              /* Signals being ignored */
            sigset_t ki_sigcatch;               /* Signals being caught by user */
            public uid_t   ki_uid;              /* effective user id */
            uid_t   ki_ruid;                    /* Real user id */
            uid_t   ki_svuid;                   /* Saved effective user id */
            gid_t   ki_rgid;                    /* Real group id */
            gid_t   ki_svgid;                   /* Saved effective group id */
            short   ki_ngroups;                 /* number of groups */
            short   ki_spare_short2;            /* unused (just here for alignment) */
            fixed uint ki_groups[KI_NGROUPS];   /* groups */
            public ulong ki_size;               /* virtual size */
            public long ki_rssize;              /* current resident set size in pages */
            long ki_swrss;                      /* resident set size before last swap */
            long ki_tsize;                      /* text size (pages) XXX */
            long ki_dsize;                      /* data size (pages) XXX */
            long ki_ssize;                      /* stack size (pages) */
            ushort ki_xstat;                    /* Exit status for wait & stop signal */
            ushort ki_acflag;                   /* Accounting flags */
            uint ki_pctcpu;                     /* %cpu for process during ki_swtime */
            uint   ki_estcpu;                   /* Time averaged value of ki_cpticks */
            uint   ki_slptime;                  /* Time since last blocked */
            uint   ki_swtime;                   /* Time swapped in or out */
            uint   ki_cow;                      /* number of copy-on-write faults */
            ulong ki_runtime;                   /* Real time in microsec */
            public timeval ki_start;            /* starting time */
            timeval ki_childtime;               /* time used by process children */
            long    ki_flag;                    /* P_* flags */
            long    ki_kiflag;                  /* KI_* flags (below) */
            int     ki_traceflag;               /* Kernel trace points */
            byte    ki_stat;                    /* S* process status */
            public byte ki_nice;                /* Process "nice" value */
            byte    ki_lock;                    /* Process lock (prevent swap) count */
            byte    ki_rqindex;                 /* Run queue index */
            byte  ki_oncpu_old;                 /* Which cpu we are on (legacy) */
            byte  ki_lastcpu_old;               /* Last cpu we were on (legacy) */
            public fixed byte    ki_tdname[TDNAMLEN+1]; /* thread name */
            fixed byte    ki_wmesg[WMESGLEN+1];         /* wchan message */
            fixed byte    ki_login[LOGNAMELEN+1];       /* setlogin name */
            fixed byte    ki_lockname[LOCKNAMELEN+1];   /* lock name */
            public fixed byte    ki_comm[COMMLEN+1];    /* command name */
            fixed byte    ki_emul[KI_EMULNAMELEN+1];    /* emulation name */
            fixed byte    ki_loginclass[LOGINCLASSLEN+1]; /* login class */
        }

        [StructLayout(LayoutKind.Sequential)]
        internal unsafe struct sysctl_name
        {
            public fixed int name[4];
        }

        [DllImport("libc", SetLastError = true)]
        private static extern unsafe int sysctl(
            int [] name,
            uint namelen,
            byte* oldp,
           ulong *oldlenp,
           void *newp,
           ulong newlen);
        
        /// <summary>
        /// Queries the OS for the PIDs for all running processes
        /// </summary>
        /// <param name="buffer">A pointer to the memory block where the PID array will start</param>
        /// <param name="buffersize">The length of the block of memory allocated for the PID array</param>
        /// <returns>Returns the number of elements (PIDs) in the buffer</returns>
        [DllImport(lib_name, SetLastError = true)]
        private static extern unsafe kinfo_proc * kinfo_getallproc(ref int cnt);

        /// <summary>
        /// Queries the OS for the list of all running processes and returns the PID for each
        /// </summary>
        /// <returns>Returns a list of PIDs corresponding to all running processes</returns>
        internal static unsafe int[] proc_listallpids()
        {
            int numProcesses=0;

            // Get the number of processes currently running to know how much data to allocate
            kinfo_proc * entries = kinfo_getallproc(ref numProcesses);

            if (numProcesses <= 0)
            {
                throw new Win32Exception(SR.CantGetAllPids);
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

                fixed (int* pBuffer = &processes[0])
                {
                    //numProcesses = proc_listallpids(pBuffer, processes.Length * sizeof(int));
                    if (numProcesses <= 0)
                    {
                        throw new Win32Exception(SR.CantGetAllPids);
                    }
                }
            }
            while (numProcesses == processes.Length);

            // Remove extra elements
            Array.Resize<int>(ref processes, numProcesses);

            return processes;
        }


        /// <param name="pid">The PID of the process</param>
        /// <returns>
        /// The amount of data actually returned. If this size matches the bufferSize parameter then
        /// the data is valid. If the sizes do not match then the data is invalid, most likely due 
        /// to not having enough permissions to query for the data of that specific process
        /// </returns>
        [DllImport(lib_name, SetLastError = true)]
        private static extern unsafe kinfo_proc * kinfo_getproc(int pid);


        /// <summary>
        /// Gets executable name for process given it's PID
        /// </summary>
        /// <param name="pid">The PID of the process</param>
        public static unsafe string getProcPath(int pid)
        {
            int[] name = new int[4];
            byte* pBuffer = stackalloc byte[PATH_MAX];
            ulong bytesLength = PATH_MAX;

            name[0] = CTL_KERN;
            name[1] = KERN_PROC;
            name[2] = KERN_PROC_PATHNAME;
            name[3] = pid;

            int ret = sysctl(name, 4 , pBuffer, &bytesLength, null, 0);
            string pn = System.Text.Encoding.UTF8.GetString(pBuffer,(int)bytesLength-1);

            if (ret  != 0 ) {
                return null;
            }
            return System.Text.Encoding.UTF8.GetString(pBuffer,(int)bytesLength-1);
        }

        /// <summary>
        /// Gets the process information for a given process
        /// </summary>
        /// <param name="pid">The PID (process ID) of the process</param>
        /// <returns>
        /// Returns a valid proc_taskallinfo struct for valid processes that the caller 
        /// has permission to access; otherwise, returns null
        /// </returns>
        static public unsafe ProcessInfo GetProcessInfoById(int pid)
        {
            // Negative PIDs are invalid
            if (pid < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(pid));
            }

            string  path = getProcPath(pid);

            // Get the process information for the specified pid
            ProcessInfo info = new ProcessInfo();
            kinfo_proc *result = kinfo_getproc(pid);
            if (result != null) {
                info.ProcessName = path;
                info.BasePriority = result->ki_nice;
                info.VirtualBytes = (long)result->ki_size;
                info.WorkingSet = result->ki_rssize;
                Marshal.FreeHGlobal((IntPtr)result);
            }

            return info;
        }

        /// <summary>
        /// Gets the rusage information for the process identified by the PID
        /// </summary>
        /// <param name="pid">The process to retrieve the rusage for</param>
        /// <returns>On success, returns a struct containing info about the process; on
        /// failure or when the caller doesn't have permissions to the process, throws a Win32Exception
        /// </returns>
        internal static unsafe rusage proc_pid_rusage(int pid)
        {
            // Negative PIDs are invalid
            if (pid < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(pid), SR.NegativePidNotSupported);
            }

            rusage info = new rusage();

            // Get the PIDs rusage info
            kinfo_proc *result = kinfo_getproc(pid);
            if (result == null)
            {
                throw new InvalidOperationException(SR.RUsageFailure);
            }

            return info;
        }
    }
}
