// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Win32.SafeHandles;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Diagnostics
{
    public partial class Process : IDisposable
    {
        /// <summary>
        /// Puts a Process component in state to interact with operating system processes that run in a 
        /// special mode by enabling the native property SeDebugPrivilege on the current thread.
        /// </summary>
        public static void EnterDebugMode()
        {
            SetPrivilege(Interop.SeDebugPrivilege, (int)Interop.SE_PRIVILEGE_ENABLED);
        }

        /// <summary>
        /// Takes a Process component out of the state that lets it interact with operating system processes 
        /// that run in a special mode.
        /// </summary>
        public static void LeaveDebugMode()
        {
            SetPrivilege(Interop.SeDebugPrivilege, 0);
        }

        /// <summary>Stops the associated process immediately.</summary>
        public void Kill()
        {
            SafeProcessHandle handle = null;
            try
            {
                handle = GetProcessHandle(Interop.PROCESS_TERMINATE);
                if (!Interop.mincore.TerminateProcess(handle, -1))
                    throw new Win32Exception();
            }
            finally
            {
                ReleaseProcessHandle(handle);
            }
        }

        /// <summary>
        /// Instructs the Process component to wait the specified number of milliseconds for the associated process to exit.
        /// </summary>
        public bool WaitForExit(int milliseconds)
        {
            SafeProcessHandle handle = null;
            bool exited;
            ProcessWaitHandle processWaitHandle = null;
            try
            {
                handle = GetProcessHandle(Interop.SYNCHRONIZE, false);
                if (handle.IsInvalid)
                {
                    exited = true;
                }
                else
                {
                    processWaitHandle = new ProcessWaitHandle(handle);
                    if (processWaitHandle.WaitOne(milliseconds))
                    {
                        exited = true;
                        _signaled = true;
                    }
                    else
                    {
                        exited = false;
                        _signaled = false;
                    }
                }
            }
            finally
            {
                if (processWaitHandle != null)
                {
                    processWaitHandle.Dispose();
                }

                // If we have a hard timeout, we cannot wait for the streams
                if (_output != null && milliseconds == -1)
                {
                    _output.WaitUtilEOF();
                }

                if (_error != null && milliseconds == -1)
                {
                    _error.WaitUtilEOF();
                }

                ReleaseProcessHandle(handle);
            }

            if (exited && _watchForExit)
            {
                RaiseOnExited();
            }

            return exited;
        }

        /// <summary>Gets the main module for the associated process.</summary>
        public ProcessModule MainModule
        {
            get
            {
                // We only return null if we couldn't find a main module. This could be because
                // the process hasn't finished loading the main module (most likely).
                // On NT, the first module is the main module.
                EnsureState(State.HaveId | State.IsLocal);
                ModuleInfo module = NtProcessManager.GetFirstModuleInfo(_processId);
                return new ProcessModule(module);
            }
        }

        /// <summary>Gets a value indicating whether the associated process has been terminated.</summary>
        /// <param name="exitCode">The exit code, if it could be determined; otherwise, null.</param>
        private bool GetHasExited(out int? exitCode)
        {
            SafeProcessHandle handle = null;
            try
            {
                handle = GetProcessHandle(Interop.PROCESS_QUERY_INFORMATION | Interop.SYNCHRONIZE, false);
                if (handle.IsInvalid)
                {
                    exitCode = null;
                    return true;
                }
                else
                {
                    int localExitCode;

                    // Although this is the wrong way to check whether the process has exited,
                    // it was historically the way we checked for it, and a lot of code then took a dependency on
                    // the fact that this would always be set before the pipes were closed, so they would read
                    // the exit code out after calling ReadToEnd() or standard output or standard error. In order
                    // to allow 259 to function as a valid exit code and to break as few people as possible that
                    // took the ReadToEnd dependency, we check for an exit code before doing the more correct
                    // check to see if we have been signalled.
                    if (Interop.mincore.GetExitCodeProcess(handle, out localExitCode) && localExitCode != Interop.STILL_ACTIVE)
                    {
                        exitCode = localExitCode;
                        return true;
                    }
                    else
                    {
                        // The best check for exit is that the kernel process object handle is invalid, 
                        // or that it is valid and signaled.  Checking if the exit code != STILL_ACTIVE 
                        // does not guarantee the process is closed,
                        // since some process could return an actual STILL_ACTIVE exit code (259).
                        if (!_signaled) // if we just came from WaitForExit, don't repeat
                        {
                            using (var wh = new ProcessWaitHandle(handle))
                            {
                                _signaled = wh.WaitOne(0);
                            }
                        }
                        if (_signaled)
                        {
                            if (!Interop.mincore.GetExitCodeProcess(handle, out localExitCode))
                                throw new Win32Exception();

                            exitCode = localExitCode;
                            return true;
                        }
                    }
                }

                exitCode = null;
                return false;
            }
            finally
            {
                ReleaseProcessHandle(handle);
            }
        }

        /// <summary>Gets the time that the associated process exited.</summary>
        private DateTime ExitTimeCore
        {
            get { return GetProcessTimes().ExitTime; }
        }

        /// <summary>Gets the amount of time the process has spent running code inside the operating system core.</summary>
        public TimeSpan PrivilegedProcessorTime
        {
            get { return GetProcessTimes().PrivilegedProcessorTime; }
        }

        /// <summary>Gets the time the associated process was started.</summary>
        public DateTime StartTime
        {
            get { return GetProcessTimes().StartTime; }
        }

        /// <summary>
        /// Gets the amount of time the associated process has spent utilizing the CPU.
        /// It is the sum of the <see cref='System.Diagnostics.Process.UserProcessorTime'/> and
        /// <see cref='System.Diagnostics.Process.PrivilegedProcessorTime'/>.
        /// </summary>
        public TimeSpan TotalProcessorTime
        {
            get { return GetProcessTimes().TotalProcessorTime; }
        }

        /// <summary>
        /// Gets the amount of time the associated process has spent running code
        /// inside the application portion of the process (not the operating system core).
        /// </summary>
        public TimeSpan UserProcessorTime
        {
            get { return GetProcessTimes().UserProcessorTime; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the associated process priority
        /// should be temporarily boosted by the operating system when the main window
        /// has focus.
        /// </summary>
        private bool PriorityBoostEnabledCore
        {
            get
            {
                SafeProcessHandle handle = null;
                try
                {
                    handle = GetProcessHandle(Interop.PROCESS_QUERY_INFORMATION);
                    bool disabled;
                    if (!Interop.mincore.GetProcessPriorityBoost(handle, out disabled))
                    {
                        throw new Win32Exception();
                    }
                    return !disabled;
                }
                finally
                {
                    ReleaseProcessHandle(handle);
                }
            }
            set
            {
                SafeProcessHandle handle = null;
                try
                {
                    handle = GetProcessHandle(Interop.PROCESS_SET_INFORMATION);
                    if (!Interop.mincore.SetProcessPriorityBoost(handle, !value))
                        throw new Win32Exception();
                }
                finally
                {
                    ReleaseProcessHandle(handle);
                }
            }
        }

        /// <summary>
        /// Gets or sets the overall priority category for the associated process.
        /// </summary>
        private ProcessPriorityClass PriorityClassCore
        {
            get
            {
                SafeProcessHandle handle = null;
                try
                {
                    handle = GetProcessHandle(Interop.PROCESS_QUERY_INFORMATION);
                    int value = Interop.mincore.GetPriorityClass(handle);
                    if (value == 0)
                    {
                        throw new Win32Exception();
                    }
                    return (ProcessPriorityClass)value;
                }
                finally
                {
                    ReleaseProcessHandle(handle);
                }
            }
            set
            {
                SafeProcessHandle handle = null;
                try
                {
                    handle = GetProcessHandle(Interop.PROCESS_SET_INFORMATION);
                    if (!Interop.mincore.SetPriorityClass(handle, (int)value))
                    {
                        throw new Win32Exception();
                    }
                }
                finally
                {
                    ReleaseProcessHandle(handle);
                }
            }
        }

        /// <summary>
        /// Gets or sets which processors the threads in this process can be scheduled to run on.
        /// </summary>
        private IntPtr ProcessorAffinityCore
        {
            get
            {
                SafeProcessHandle handle = null;
                try
                {
                    handle = GetProcessHandle(Interop.PROCESS_QUERY_INFORMATION);
                    IntPtr processAffinity, systemAffinity;
                    if (!Interop.mincore.GetProcessAffinityMask(handle, out processAffinity, out systemAffinity))
                        throw new Win32Exception();
                    return processAffinity;
                }
                finally
                {
                    ReleaseProcessHandle(handle);
                }
            }
            set
            {
                SafeProcessHandle handle = null;
                try
                {
                    handle = GetProcessHandle(Interop.PROCESS_SET_INFORMATION);
                    if (!Interop.mincore.SetProcessAffinityMask(handle, value))
                        throw new Win32Exception();
                }
                finally
                {
                    ReleaseProcessHandle(handle);
                }
            }
        }

        /// <summary>Get the minimum and maximum working set limits.</summary>
        private void GetWorkingSetLimits(out IntPtr minWorkingSet, out IntPtr maxWorkingSet)
        {
            SafeProcessHandle handle = null;
            try
            {
                handle = GetProcessHandle(Interop.PROCESS_QUERY_INFORMATION);

                // GetProcessWorkingSetSize is not exposed in coresys.  We use 
                // GetProcessWorkingSetSizeEx instead which also returns FLAGS which give some flexibility
                // in terms of Min and Max values which we neglect.
                int ignoredFlags;
                if (!Interop.mincore.GetProcessWorkingSetSizeEx(handle, out minWorkingSet, out maxWorkingSet, out ignoredFlags))
                {
                    throw new Win32Exception();
                }
            }
            finally
            {
                ReleaseProcessHandle(handle);
            }
        }

        /// <summary>Gets the ID of the current process.</summary>
        private static int GetCurrentProcessId()
        {
            return unchecked((int)Interop.mincore.GetCurrentProcessId());
        }

        /// <summary>
        /// Opens a long-term handle to the process, with all access.  If a handle exists,
        /// then it is reused.  If the process has exited, it throws an exception.
        /// </summary>
        private SafeProcessHandle OpenProcessHandle()
        {
            return OpenProcessHandle(Interop.PROCESS_ALL_ACCESS);
        }

        /// <summary>Sets one or both of the minimum and maximum working set limits.</summary>
        /// <param name="newMin">The new minimum working set limit, or null not to change it.</param>
        /// <param name="newMax">The new maximum working set limit, or null not to change it.</param>
        /// <param name="resultingMin">The resulting minimum working set limit after any changes applied.</param>
        /// <param name="resultingMax">The resulting maximum working set limit after any changes applied.</param>
        private void SetWorkingSetLimitsCore(IntPtr? newMin, IntPtr? newMax, out IntPtr resultingMin, out IntPtr resultingMax)
        {
            SafeProcessHandle handle = null;
            try
            {
                handle = GetProcessHandle(Interop.PROCESS_QUERY_INFORMATION | Interop.PROCESS_SET_QUOTA);
                IntPtr min, max;
                int ignoredFlags;
                if (!Interop.mincore.GetProcessWorkingSetSizeEx(handle, out min, out max, out ignoredFlags))
                {
                    throw new Win32Exception();
                }

                if (newMin.HasValue)
                {
                    min = newMin.Value;
                }
                if (newMax.HasValue)
                {
                    max = newMax.Value;
                }

                if ((long)min > (long)max)
                {
                    if (newMin != null)
                    {
                        throw new ArgumentException(SR.BadMinWorkset);
                    }
                    else
                    {
                        throw new ArgumentException(SR.BadMaxWorkset);
                    }
                }

                // We use SetProcessWorkingSetSizeEx which gives an option to follow
                // the max and min value even in low-memory and abundant-memory situtions.
                // However, we do not use these flags to emulate the existing behavior
                if (!Interop.mincore.SetProcessWorkingSetSizeEx(handle, min, max, 0))
                {
                    throw new Win32Exception();
                }

                // The value may be rounded/changed by the OS, so go get it
                if (!Interop.mincore.GetProcessWorkingSetSizeEx(handle, out min, out max, out ignoredFlags))
                {
                    throw new Win32Exception();
                }

                resultingMin = min;
                resultingMax = max;
            }
            finally
            {
                ReleaseProcessHandle(handle);
            }
        }

        /// <summary>Starts the process using the supplied start info.</summary>
        /// <param name="startInfo">The start info with which to start the process.</param>
        private bool StartCore(ProcessStartInfo startInfo)
        {
            if (startInfo.StandardOutputEncoding != null && !startInfo.RedirectStandardOutput)
            {
                throw new InvalidOperationException(SR.StandardOutputEncodingNotAllowed);
            }

            if (startInfo.StandardErrorEncoding != null && !startInfo.RedirectStandardError)
            {
                throw new InvalidOperationException(SR.StandardErrorEncodingNotAllowed);
            }

            // See knowledge base article Q190351 for an explanation of the following code.  Noteworthy tricky points:
            //    * The handles are duplicated as non-inheritable before they are passed to CreateProcess so
            //      that the child process can not close them
            //    * CreateProcess allows you to redirect all or none of the standard IO handles, so we use
            //      GetStdHandle for the handles that are not being redirected

            //Cannot start a new process and store its handle if the object has been disposed, since finalization has been suppressed.            
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }

            StringBuilder commandLine = BuildCommandLine(startInfo.FileName, startInfo.Arguments);

            Interop.STARTUPINFO startupInfo = new Interop.STARTUPINFO();
            Interop.PROCESS_INFORMATION processInfo = new Interop.PROCESS_INFORMATION();
            Interop.SECURITY_ATTRIBUTES unused_SecAttrs = new Interop.SECURITY_ATTRIBUTES();
            SafeProcessHandle procSH = new SafeProcessHandle();
            SafeThreadHandle threadSH = new SafeThreadHandle();
            bool retVal;
            int errorCode = 0;
            // handles used in parent process
            SafeFileHandle standardInputWritePipeHandle = null;
            SafeFileHandle standardOutputReadPipeHandle = null;
            SafeFileHandle standardErrorReadPipeHandle = null;
            GCHandle environmentHandle = new GCHandle();
            lock (s_createProcessLock)
            {
                try
                {
                    // set up the streams
                    if (startInfo.RedirectStandardInput || startInfo.RedirectStandardOutput || startInfo.RedirectStandardError)
                    {
                        if (startInfo.RedirectStandardInput)
                        {
                            CreatePipe(out standardInputWritePipeHandle, out startupInfo.hStdInput, true);
                        }
                        else
                        {
                            startupInfo.hStdInput = new SafeFileHandle(Interop.mincore.GetStdHandle(Interop.STD_INPUT_HANDLE), false);
                        }

                        if (startInfo.RedirectStandardOutput)
                        {
                            CreatePipe(out standardOutputReadPipeHandle, out startupInfo.hStdOutput, false);
                        }
                        else
                        {
                            startupInfo.hStdOutput = new SafeFileHandle(Interop.mincore.GetStdHandle(Interop.STD_OUTPUT_HANDLE), false);
                        }

                        if (startInfo.RedirectStandardError)
                        {
                            CreatePipe(out standardErrorReadPipeHandle, out startupInfo.hStdError, false);
                        }
                        else
                        {
                            startupInfo.hStdError = new SafeFileHandle(Interop.mincore.GetStdHandle(Interop.STD_ERROR_HANDLE), false);
                        }

                        startupInfo.dwFlags = Interop.STARTF_USESTDHANDLES;
                    }

                    // set up the creation flags paramater
                    int creationFlags = 0;
                    if (startInfo.CreateNoWindow) creationFlags |= Interop.CREATE_NO_WINDOW;

                    // set up the environment block parameter
                    IntPtr environmentPtr = (IntPtr)0;
                    if (startInfo._environmentVariables != null)
                    {
                        creationFlags |= Interop.CREATE_UNICODE_ENVIRONMENT;
                        byte[] environmentBytes = EnvironmentVariablesToByteArray(startInfo._environmentVariables);
                        environmentHandle = GCHandle.Alloc(environmentBytes, GCHandleType.Pinned);
                        environmentPtr = environmentHandle.AddrOfPinnedObject();
                    }
                    string workingDirectory = startInfo.WorkingDirectory;
                    if (workingDirectory == string.Empty)
                        workingDirectory = Directory.GetCurrentDirectory();

                    try { }
                    finally
                    {
                        retVal = Interop.mincore.CreateProcess(
                                null,                // we don't need this since all the info is in commandLine
                                commandLine,         // pointer to the command line string
                                unused_SecAttrs, // address to process security attributes, we don't need to inheriat the handle
                                unused_SecAttrs, // address to thread security attributes.
                                true,                // handle inheritance flag
                                creationFlags,       // creation flags
                                environmentPtr,      // pointer to new environment block
                                workingDirectory,    // pointer to current directory name
                                startupInfo,         // pointer to STARTUPINFO
                                processInfo      // pointer to PROCESS_INFORMATION
                            );
                        if (!retVal)
                            errorCode = Marshal.GetLastWin32Error();
                        if (processInfo.hProcess != (IntPtr)0 && processInfo.hProcess != (IntPtr)Interop.INVALID_HANDLE_VALUE)
                            procSH.InitialSetHandle(processInfo.hProcess);
                        if (processInfo.hThread != (IntPtr)0 && processInfo.hThread != (IntPtr)Interop.INVALID_HANDLE_VALUE)
                            threadSH.InitialSetHandle(processInfo.hThread);
                    }
                    if (!retVal)
                    {
                        if (errorCode == Interop.ERROR_BAD_EXE_FORMAT || errorCode == Interop.ERROR_EXE_MACHINE_TYPE_MISMATCH)
                        {
                            throw new Win32Exception(errorCode, SR.InvalidApplication);
                        }
                        throw new Win32Exception(errorCode);
                    }
                }
                finally
                {
                    // free environment block
                    if (environmentHandle.IsAllocated)
                    {
                        environmentHandle.Free();
                    }

                    startupInfo.Dispose();
                }
            }

            if (startInfo.RedirectStandardInput)
            {
                Encoding enc = GetEncoding((int)Interop.mincore.GetConsoleCP());
                _standardInput = new StreamWriter(new FileStream(standardInputWritePipeHandle, FileAccess.Write, 4096, false), enc, 4096);
                _standardInput.AutoFlush = true;
            }
            if (startInfo.RedirectStandardOutput)
            {
                Encoding enc = (startInfo.StandardOutputEncoding != null) ? startInfo.StandardOutputEncoding : GetEncoding((int)Interop.mincore.GetConsoleOutputCP());
                _standardOutput = new StreamReader(new FileStream(standardOutputReadPipeHandle, FileAccess.Read, 4096, false), enc, true, 4096);
            }
            if (startInfo.RedirectStandardError)
            {
                Encoding enc = (startInfo.StandardErrorEncoding != null) ? startInfo.StandardErrorEncoding : GetEncoding((int)Interop.mincore.GetConsoleOutputCP());
                _standardError = new StreamReader(new FileStream(standardErrorReadPipeHandle, FileAccess.Read, 4096, false), enc, true, 4096);
            }

            bool ret = false;
            if (!procSH.IsInvalid)
            {
                SetProcessHandle(procSH);
                SetProcessId((int)processInfo.dwProcessId);
                threadSH.Dispose();
                ret = true;
            }

            return ret;
        }

        // -----------------------------
        // ---- PAL layer ends here ----
        // -----------------------------

        /// <summary>Gets timing information for the current process.</summary>
        private ProcessThreadTimes GetProcessTimes()
        {
            SafeProcessHandle handle = null;
            try
            {
                handle = GetProcessHandle(Interop.PROCESS_QUERY_LIMITED_INFORMATION, false);
                if (handle.IsInvalid)
                {
                    throw new InvalidOperationException(SR.Format(SR.ProcessHasExited, _processId.ToString(CultureInfo.CurrentCulture)));
                }

                ProcessThreadTimes processTimes = new ProcessThreadTimes();
                if (!Interop.mincore.GetProcessTimes(handle, 
                    out processTimes._create, out processTimes._exit, 
                    out processTimes._kernel, out processTimes._user))
                {
                    throw new Win32Exception();
                }
                return processTimes;
            }
            finally
            {
                ReleaseProcessHandle(handle);
            }
        }

        private static void SetPrivilege(string privilegeName, int attrib)
        {
            SafeTokenHandle hToken = null;
            Interop.LUID debugValue = new Interop.LUID();

            // this is only a "pseudo handle" to the current process - no need to close it later
            SafeProcessHandle processHandle = Interop.mincore.GetCurrentProcess();

            // get the process token so we can adjust the privilege on it.  We DO need to
            // close the token when we're done with it.
            if (!Interop.mincore.OpenProcessToken(processHandle, Interop.TOKEN_ADJUST_PRIVILEGES, out hToken))
            {
                throw new Win32Exception();
            }

            try
            {
                if (!Interop.mincore.LookupPrivilegeValue(null, privilegeName, out debugValue))
                {
                    throw new Win32Exception();
                }

                Interop.TokenPrivileges tkp = new Interop.TokenPrivileges();
                tkp.Luid = debugValue;
                tkp.Attributes = attrib;

                Interop.mincore.AdjustTokenPrivileges(hToken, false, tkp, 0, IntPtr.Zero, IntPtr.Zero);

                // AdjustTokenPrivileges can return true even if it failed to
                // set the privilege, so we need to use GetLastError
                if (Marshal.GetLastWin32Error() != Interop.ERROR_SUCCESS)
                {
                    throw new Win32Exception();
                }
            }
            finally
            {
#if FEATURE_TRACESWITCH
                Debug.WriteLineIf(_processTracing.TraceVerbose, "Process - CloseHandle(processToken)");
#endif
                if (hToken != null)
                {
                    hToken.Dispose();
                }
            }
        }

        /// <devdoc>
        ///     Gets a short-term handle to the process, with the given access.  
        ///     If a handle is stored in current process object, then use it.
        ///     Note that the handle we stored in current process object will have all access we need.
        /// </devdoc>
        /// <internalonly/>
        private SafeProcessHandle GetProcessHandle(int access, bool throwIfExited)
        {
#if FEATURE_TRACESWITCH
            Debug.WriteLineIf(_processTracing.TraceVerbose, "GetProcessHandle(access = 0x" + access.ToString("X8", CultureInfo.InvariantCulture) + ", throwIfExited = " + throwIfExited + ")");
#if DEBUG
            if (_processTracing.TraceVerbose) {
                StackFrame calledFrom = new StackTrace(true).GetFrame(0);
                Debug.WriteLine("   called from " + calledFrom.GetFileName() + ", line " + calledFrom.GetFileLineNumber());
            }
#endif
#endif
            if (_haveProcessHandle)
            {
                if (throwIfExited)
                {
                    // Since haveProcessHandle is true, we know we have the process handle
                    // open with at least SYNCHRONIZE access, so we can wait on it with 
                    // zero timeout to see if the process has exited.
                    using (ProcessWaitHandle waitHandle = new ProcessWaitHandle(_processHandle))
                    {
                        if (waitHandle.WaitOne(0))
                        {
                            if (_haveProcessId)
                                throw new InvalidOperationException(SR.Format(SR.ProcessHasExited, _processId.ToString(CultureInfo.CurrentCulture)));
                            else
                                throw new InvalidOperationException(SR.ProcessHasExitedNoId);
                        }
                    }
                }
                return _processHandle;
            }
            else
            {
                EnsureState(State.HaveId | State.IsLocal);
                SafeProcessHandle handle = SafeProcessHandle.InvalidHandle;
                handle = ProcessManager.OpenProcess(_processId, access, throwIfExited);
                if (throwIfExited && (access & Interop.PROCESS_QUERY_INFORMATION) != 0)
                {
                    if (Interop.mincore.GetExitCodeProcess(handle, out _exitCode) && _exitCode != Interop.STILL_ACTIVE)
                    {
                        throw new InvalidOperationException(SR.Format(SR.ProcessHasExited, _processId.ToString(CultureInfo.CurrentCulture)));
                    }
                }
                return handle;
            }
        }

        /// <devdoc>
        ///     Gets a short-term handle to the process, with the given access.  If a handle exists,
        ///     then it is reused.  If the process has exited, it throws an exception.
        /// </devdoc>
        /// <internalonly/>
        private SafeProcessHandle GetProcessHandle(int access)
        {
            return GetProcessHandle(access, true);
        }

        private SafeProcessHandle OpenProcessHandle(Int32 access)
        {
            if (!_haveProcessHandle)
            {
                //Cannot open a new process handle if the object has been disposed, since finalization has been suppressed.            
                if (_disposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                SetProcessHandle(GetProcessHandle(access));
            }
            return _processHandle;
        }

        private static void CreatePipeWithSecurityAttributes(out SafeFileHandle hReadPipe, out SafeFileHandle hWritePipe, Interop.SECURITY_ATTRIBUTES lpPipeAttributes, int nSize)
        {
            bool ret = Interop.mincore.CreatePipe(out hReadPipe, out hWritePipe, lpPipeAttributes, nSize);
            if (!ret || hReadPipe.IsInvalid || hWritePipe.IsInvalid)
            {
                throw new Win32Exception();
            }
        }

        // Using synchronous Anonymous pipes for process input/output redirection means we would end up 
        // wasting a worker threadpool thread per pipe instance. Overlapped pipe IO is desirable, since 
        // it will take advantage of the NT IO completion port infrastructure. But we can't really use 
        // Overlapped I/O for process input/output as it would break Console apps (managed Console class 
        // methods such as WriteLine as well as native CRT functions like printf) which are making an
        // assumption that the console standard handles (obtained via GetStdHandle()) are opened
        // for synchronous I/O and hence they can work fine with ReadFile/WriteFile synchrnously!
        private void CreatePipe(out SafeFileHandle parentHandle, out SafeFileHandle childHandle, bool parentInputs)
        {
            Interop.SECURITY_ATTRIBUTES securityAttributesParent = new Interop.SECURITY_ATTRIBUTES();
            securityAttributesParent.bInheritHandle = true;

            SafeFileHandle hTmp = null;
            try
            {
                if (parentInputs)
                {
                    CreatePipeWithSecurityAttributes(out childHandle, out hTmp, securityAttributesParent, 0);
                }
                else
                {
                    CreatePipeWithSecurityAttributes(out hTmp,
                                                          out childHandle,
                                                          securityAttributesParent,
                                                          0);
                }
                // Duplicate the parent handle to be non-inheritable so that the child process 
                // doesn't have access. This is done for correctness sake, exact reason is unclear.
                // One potential theory is that child process can do something brain dead like 
                // closing the parent end of the pipe and there by getting into a blocking situation
                // as parent will not be draining the pipe at the other end anymore. 
                SafeProcessHandle currentProcHandle = Interop.mincore.GetCurrentProcess();
                if (!Interop.mincore.DuplicateHandle(currentProcHandle,
                                                     hTmp,
                                                     currentProcHandle,
                                                     out parentHandle,
                                                     0,
                                                     false,
                                                     Interop.DUPLICATE_SAME_ACCESS))
                {
                    throw new Win32Exception();
                }
            }
            finally
            {
                if (hTmp != null && !hTmp.IsInvalid)
                {
                    hTmp.Dispose();
                }
            }
        }

        private static byte[] EnvironmentVariablesToByteArray(Dictionary<string, string> sd)
        {
            // get the keys
            string[] keys = new string[sd.Count];
            byte[] envBlock = null;
            sd.Keys.CopyTo(keys, 0);

            // sort both by the keys
            // Windows 2000 requires the environment block to be sorted by the key
            // It will first converting the case the strings and do ordinal comparison.

            // We do not use Array.Sort(keys, values, IComparer) since it is only supported
            // in System.Runtime contract from 4.20.0.0 and Test.Net depends on System.Runtime 4.0.10.0
            // we workaround this by sorting only the keys and then lookup the values form the keys.
            Array.Sort(keys, StringComparer.OrdinalIgnoreCase);

            // create a list of null terminated "key=val" strings
            StringBuilder stringBuff = new StringBuilder();
            for (int i = 0; i < sd.Count; ++i)
            {
                stringBuff.Append(keys[i]);
                stringBuff.Append('=');
                stringBuff.Append(sd[keys[i]]);
                stringBuff.Append('\0');
            }
            // an extra null at the end indicates end of list.
            stringBuff.Append('\0');
            envBlock = Encoding.Unicode.GetBytes(stringBuff.ToString());
            return envBlock;
        }
    }
}
