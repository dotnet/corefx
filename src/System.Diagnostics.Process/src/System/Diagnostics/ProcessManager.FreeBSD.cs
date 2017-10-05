// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace System.Diagnostics
{
    internal static partial class ProcessManager
    {
        /// <summary>Gets the IDs of all processes on the current machine.</summary>
        public static int[] GetProcessIds()
        {
            return Interop.libutil.proc_listallpids();
        }

        /// <summary>Gets process infos for each process on the specified machine.</summary>
        /// <param name="machineName">The target machine.</param>
        /// <returns>An array of process infos, one per found process.</returns>
        public static ProcessInfo[] GetProcessInfos(string machineName)
        {
            ThrowIfRemoteMachine(machineName);

            int[] procIds = GetProcessIds(machineName);

            // Iterate through all process IDs to load information about each process
            var processes = new List<ProcessInfo>(procIds.Length);
            foreach (int pid in procIds)
            {
                ProcessInfo pi = CreateProcessInfo(pid);
                if (pi != null)
                {
                    processes.Add(pi);
                }
            }

            return processes.ToArray();
        }

        // -----------------------------
        // ---- PAL layer ends here ----
        // -----------------------------

        private static ProcessInfo CreateProcessInfo(int pid)
        {
            // Negative PIDs aren't valid
            if (pid < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(pid));
            }

            ProcessInfo procInfo = new ProcessInfo()
            {
                ProcessId = pid
            };

            // Try to get the task info. This can fail if the user permissions don't permit
            // this user context to query the specified process
            ProcessInfo info = Interop.libutil.GetProcessInfoById(pid);
            //if (info.HasValue)
            if (true)
            {
                // Set the values we have; all the other values don't have meaning or don't exist
                //unsafe { procInfo.ProcessName = Marshal.PtrToStringAnsi(new IntPtr(temp.pbsd.pbi_comm)); }
                procInfo.ProcessName = info.ProcessName;
                procInfo.BasePriority = info.BasePriority;
                procInfo.VirtualBytes = info.VirtualBytes;
                procInfo.WorkingSet = info.WorkingSet;
                procInfo.SessionId = info.SessionId;
                //procInfo._threadInfoList = info._threadInfoList;
                foreach (ThreadInfo ti in info._threadInfoList)
                {
                    procInfo._threadInfoList.Add(ti);
                }
            }

            return procInfo;
        }

        /// <summary>Gets an array of module infos for the specified process.</summary>
        /// <param name="processId">The ID of the process whose modules should be enumerated.</param>
        /// <returns>The array of modules.</returns>
        internal static ProcessModuleCollection GetModules(int processId)
        {
            // We don't have a good way of getting all of the modules of the particular process,
            // but we can at least get the path to the executable file for the process, and
            // other than for debugging tools, that's the main reason consumers of Modules care about it,
            // and why MainModule exists.
            try
            {
                string exePath = Interop.libutil.getProcPath(processId);
                if (!string.IsNullOrEmpty(exePath))
                {
                    return new ProcessModuleCollection(1)
                    {
                        new ProcessModule()
                        {
                            FileName = exePath,
                            ModuleName = Path.GetFileName(exePath)
                        }
                    };
                }
            }
            catch { } // eat all errors

            return new ProcessModuleCollection(0);
        }

        // ----------------------------------
        // ---- Unix PAL layer ends here ----
        // ----------------------------------

        //private static System.Diagnostics.ThreadState ConvertOsxThreadRunStateToThreadState(Interop.libproc.ThreadRunState state)
        private static System.Diagnostics.ThreadState  ConvertOsxThreadRunStateToThreadState()
        {
            return System.Diagnostics.ThreadState.Running;
            //switch (state)
            //{

                //case Interop.libproc.ThreadRunState.TH_STATE_RUNNING:
                //    return System.Diagnostics.ThreadState.Running;
                //case Interop.libproc.ThreadRunState.TH_STATE_STOPPED:
                //    return System.Diagnostics.ThreadState.Terminated;
                //case Interop.libproc.ThreadRunState.TH_STATE_HALTED:
                //    return System.Diagnostics.ThreadState.Wait;
                //case Interop.libproc.ThreadRunState.TH_STATE_UNINTERRUPTIBLE:
                //    return System.Diagnostics.ThreadState.Running;
                //case Interop.libproc.ThreadRunState.TH_STATE_WAITING:
                //    return System.Diagnostics.ThreadState.Standby;
            //    default:
            //        throw new ArgumentOutOfRangeException(nameof(state));
           // }
        }

        //private static System.Diagnostics.ThreadWaitReason ConvertOsxThreadFlagsToWaitReason(Interop.libproc.ThreadFlags flags)
        private static System.Diagnostics.ThreadWaitReason ConvertOsxThreadFlagsToWaitReason()
        {
             return System.Diagnostics.ThreadWaitReason.Unknown;

            // Since ThreadWaitReason isn't a flag, we have to do a mapping and will lose some information.
            //if ((flags & Interop.libproc.ThreadFlags.TH_FLAGS_SWAPPED) == Interop.libproc.ThreadFlags.TH_FLAGS_SWAPPED)
            //    return System.Diagnostics.ThreadWaitReason.PageOut;
            //else
            //    return System.Diagnostics.ThreadWaitReason.Unknown; // There isn't a good mapping for anything else
        }
    }    
}
