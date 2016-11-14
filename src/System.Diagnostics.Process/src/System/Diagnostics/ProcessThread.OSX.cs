// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Diagnostics
{
    public partial class ProcessThread
    {
        /// <summary>
        /// Returns or sets the priority level of the associated thread.  The priority level is
        /// not an absolute level, but instead contributes to the actual thread priority by
        /// considering the priority class of the process.
        /// </summary>
        private ThreadPriorityLevel PriorityLevelCore
        {
            get { throw new PlatformNotSupportedException(); }
            set { throw new PlatformNotSupportedException(); }
        }

        /// <summary>
        /// Returns the amount of time the thread has spent running code inside the operating
        /// system core.
        /// </summary>
        public TimeSpan PrivilegedProcessorTime => new TimeSpan((long)GetThreadInfo().pth_system_time);

        /// <summary>Returns the time the associated thread was started.</summary>
        public DateTime StartTime
        {
            get { throw new PlatformNotSupportedException(); }
        }

        /// <summary>
        /// Returns the amount of time the associated thread has spent utilizing the CPU.
        /// It is the sum of the System.Diagnostics.ProcessThread.UserProcessorTime and
        /// System.Diagnostics.ProcessThread.PrivilegedProcessorTime.
        /// </summary>
        public TimeSpan TotalProcessorTime
        {
            get
            {
                Interop.libproc.proc_threadinfo info = GetThreadInfo();
                return new TimeSpan((long)(info.pth_user_time + info.pth_system_time));
            }
        }

        /// <summary>
        /// Returns the amount of time the associated thread has spent running code
        /// inside the application (not the operating system core).
        /// </summary>
        public TimeSpan UserProcessorTime => new TimeSpan((long)GetThreadInfo().pth_user_time);

        // -----------------------------
        // ---- PAL layer ends here ----
        // -----------------------------

        private Interop.libproc.proc_threadinfo GetThreadInfo()
        {
            Interop.libproc.proc_threadinfo? info = Interop.libproc.GetThreadInfoById(_processId, _threadInfo._threadId);
            if (!info.HasValue)
            {
                throw new InvalidOperationException(SR.Format(SR.ThreadExited, Id));
            }
            return info.GetValueOrDefault();
        }
    }
}
