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
        private const ulong SecondsToNanoSeconds = 1000000000;

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

        private const int KI_NSPARE_INT = 4;
        private const int KI_NSPARE_LONG = 12;
        private const int KI_NSPARE_PTR = 6;


        // Constants from sys/_sigset.h
        private const int _SIG_WORDS = 4;

        // Constants from sys/sysctl.h
        private const int CTL_KERN = 1;
        private const int KERN_PROC = 14;
        private const int KERN_PROC_PATHNAME = 12;
        private const int KERN_PROC_PROC = 8;
        private const int KERN_PROC_ALL = 0; 
        private const int KERN_PROC_PID  = 1;
        private const int KERN_PROC_INC_THREAD = 16;

        // Constants from proc_info.h
        private const int MAXTHREADNAMESIZE = 64;
        private const int PROC_PIDTASKALLINFO = 2;
        private const int PROC_PIDTHREADINFO = 5;
        private const int PROC_PIDLISTTHREADS = 6;
        private const int PROC_PIDPATHINFO_MAXSIZE = 4 * PATH_MAX;

        internal struct proc_stats
        {
            internal long startTime;        /* time_t */
            internal int nice;
            internal ulong userTime;        /* in ticks */
            internal ulong systemTime;      /* in ticks */
        }

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
            public timeval ru_utime;        /* user time used */
            public timeval ru_stime;        /* system time used */
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
            public int ki_sid;                         /* Process session ID */
            public int ki_tsid;                        /* Terminal session ID */
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
            fixed byte    ki_sparestrings[50]; /* spare string space */
            fixed int ki_spareints[KI_NSPARE_INT];    /* spare room for growth */
            int ki_oncpu;       /* Which cpu we are on */
            int ki_lastcpu;     /* Last cpu we were on */
            int ki_tracer;      /* Pid of tracing process */
            int ki_flag2;       /* P2_* flags */
            int ki_fibnum;      /* Default FIB number */
            uint   ki_cr_flags;        /* Credential flags */
            int ki_jid;         /* Process jail ID */
            public int ki_numthreads;      /* XXXKSE number of threads in total */
            public int ki_tid;         /* XXXKSE thread id */
            fixed byte ki_pri[4];    /* process priority */
            public rusage ki_rusage;   /* process rusage statistics */
            /* XXX - most fields in ki_rusage_ch are not (yet) filled in */
            rusage ki_rusage_ch;    /* rusage of children processes */
            void *ki_pcb;        /* kernel virtual addr of pcb */
            void    *ki_kstack;     /* kernel virtual addr of stack */
            void    *ki_udata;      /* User convenience pointer */
            public void *ki_tdaddr;  /* address of thread */

            fixed long ki_spareptrs[KI_NSPARE_PTR];   /* spare room for growth */
            fixed long    ki_sparelongs[KI_NSPARE_LONG];  /* spare room for growth */
            long    ki_sflag;       /* PS_* flags */
            public long    ki_tdflags;     /* XXXKSE kthread flag */
        }

        [StructLayout(LayoutKind.Sequential)]
        internal unsafe struct sysctl_name
        {
            public fixed int name[4];
        }

        [DllImport("libc", SetLastError = true)]
        private static extern unsafe int sysctl(
            int [] name,
            int namelen,
            byte* oldp,
           ulong *oldlenp,
           void *newp,
           ulong newlen);
        
        /// <summary>
        /// Queries the OS for the list of all running processes and returns the PID for each
        /// </summary>
        /// <returns>Returns a list of PIDs corresponding to all running processes</returns>
        internal static unsafe int[] proc_listallpids()
        {
            int numProcesses=0;
            int[] pids;

            kinfo_proc * entries = getProcInfo(0, false, out numProcesses);

            if (entries == null || numProcesses <= 0)
            {
                throw new Win32Exception(SR.CantGetAllPids);
            }

            Span<kinfo_proc>  list = new Span<kinfo_proc>(entries, numProcesses);
            pids = new int[numProcesses];
            int idx=0;
            // walk thorough process list and skip kernel threads
            for(int i=0; i < list.Length; i++) {
                if (list[i].ki_ppid == 0)
                {
                    // skip kernel threads
                    numProcesses-=1;
                }
                else
                {
                    pids[idx] = list[i].ki_pid;
                    idx += 1;
                }
            }
            // Remove extra elements
            Array.Resize<int>(ref pids, numProcesses);
            Marshal.FreeHGlobal((IntPtr)entries);

            return pids;
        }


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
        /// Gets information about process or thread(s)
        /// </summary>
        /// <param name="pid">The PID of the process. If PID is 0, this will return all processes</param>
        public static unsafe kinfo_proc* getProcInfo(int pid, bool threads, out int count)
        {
            int[] name = new int[4];
            ulong bytesLength = 0;
            byte* pBuffer;
            kinfo_proc* kinfo;

            count = -1;

            name[0] = CTL_KERN;
            name[1] = KERN_PROC;
            name[2] = (pid == 0) ? KERN_PROC_PROC : KERN_PROC_PID | (threads ? KERN_PROC_INC_THREAD : 0);
            name[3] = pid;

            int ret = sysctl(name, pid == 0 ? 3 : 4 , null , &bytesLength, null, 0);
            if (ret != 0 )
            {
                throw new ArgumentOutOfRangeException(nameof(pid));
            }
            //buffer =  new byte[bytesLength];

            pBuffer = (byte*)Marshal.AllocHGlobal((int)bytesLength);
            ret = sysctl(name, pid == 0 ? 3 : 4 , pBuffer, &bytesLength, null, 0);
            if (ret != 0 ) {
                Marshal.FreeHGlobal((IntPtr)pBuffer);
                throw new ArgumentOutOfRangeException(nameof(pid));
            }

            kinfo = (kinfo_proc*)pBuffer;
            if (kinfo->ki_structsize != sizeof(kinfo_proc))
            {
                // failed consistency check 
                Marshal.FreeHGlobal((IntPtr)pBuffer);
                throw new ArgumentOutOfRangeException(nameof(pid));
            }

            count = (int)bytesLength / sizeof(kinfo_proc);

            return kinfo;
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

            int count;
            kinfo_proc *kinfo = getProcInfo(pid, true, out count);
            if (kinfo == null || count < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(pid));
            }

            Span<kinfo_proc> process = new Span<kinfo_proc>(kinfo, count);

            // Get the process information for the specified pid
            ProcessInfo info = new ProcessInfo();

            info.ProcessName = Marshal.PtrToStringAnsi((IntPtr)kinfo->ki_comm);
            info.BasePriority = kinfo->ki_nice;
            info.VirtualBytes = (long)kinfo->ki_size;
            info.WorkingSet = kinfo->ki_rssize;
            info.SessionId = kinfo ->ki_sid;

            for(int i=0; i < process.Length; i++)
            {
                var ti = new ThreadInfo()
                {
                    _processId = pid,
                    _threadId = (ulong)process[i].ki_tid,
                    _basePriority = process[i].ki_nice,
                    _startAddress = (IntPtr)process[i].ki_tdaddr
                };
                info._threadInfoList.Add(ti);
            }
            Marshal.FreeHGlobal((IntPtr)kinfo);

            return info;
        }

        /// <summary>
        /// Gets the process information for a given process
        /// </summary>
        // 
        /// <param name="pid">The PID (process ID) of the process</param>
        /// <param name="tid">The TID (thread ID) of the process</param>
        /// <returns>
        /// Returns basic info about thread. If tis is 0, it will return
        /// info for process e.g. main thread.
        /// </returns>
        public unsafe static proc_stats getThreadInfo(int pid, int tid)
        {
            proc_stats ret = new proc_stats();
            int count;

            kinfo_proc* info =  getProcInfo(pid, (tid == 0 ? false: true), out count);
            if (info != null && count >= 1)
            {
                if (tid == 0)
                {
                    ret.startTime = info->ki_start.tv_sec;
                    ret.nice = info->ki_nice;
                    ret.userTime = (ulong)info->ki_rusage.ru_utime.tv_sec * SecondsToNanoSeconds + (ulong)info->ki_rusage.ru_utime.tv_usec;
                    ret.systemTime = (ulong)info->ki_rusage.ru_stime.tv_sec * SecondsToNanoSeconds + (ulong)info->ki_rusage.ru_stime.tv_usec;
                }
                else
                {
                    Span<kinfo_proc>  list = new Span<kinfo_proc>(info, count);
                    for(int i=0; i < list.Length; i++)
                    {
                        if (list[i].ki_tid == tid)
                        {
                            ret.startTime = list[i].ki_start.tv_sec;
                            ret.nice = list[i].ki_nice;
                            ret.userTime = (ulong)list[i].ki_rusage.ru_utime.tv_sec * SecondsToNanoSeconds + (ulong)list[i].ki_rusage.ru_utime.tv_usec;
                            ret.systemTime = (ulong)list[i].ki_rusage.ru_stime.tv_sec * SecondsToNanoSeconds + (ulong)list[i].ki_rusage.ru_stime.tv_usec;
                            break;
                        }
                    }
                }
                Marshal.FreeHGlobal((IntPtr)info);
            }

            return ret;
        }
    }
}
