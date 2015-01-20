// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Win32.SafeHandles;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;

namespace System.Diagnostics
{
    /// <devdoc>
    ///    <para>
    ///       Provides access to local and remote
    ///       processes. Enables you to start and stop system processes.
    ///    </para>
    /// </devdoc>
    public partial class Process : IDisposable
    {
        private bool _haveProcessId;
        private int _processId;
        private bool _haveProcessHandle;
        private SafeProcessHandle _processHandle;
        private bool _isRemoteMachine;
        private string _machineName;
        private ProcessInfo _processInfo;

        private ProcessThreadCollection _threads;
        private ProcessModuleCollection _modules;

        private bool _haveWorkingSetLimits;
        private IntPtr _minWorkingSet;
        private IntPtr _maxWorkingSet;

        private bool _haveProcessorAffinity;
        private IntPtr _processorAffinity;

        private bool _havePriorityClass;
        private ProcessPriorityClass _priorityClass;

        private ProcessStartInfo _startInfo;

        private bool _watchForExit;
        private bool _watchingForExit;
        private EventHandler _onExited;
        private bool _exited;
        private int _exitCode;
        private bool _signaled;

        private DateTime _exitTime;
        private bool _haveExitTime;

        private bool _priorityBoostEnabled;
        private bool _havePriorityBoostEnabled;

        private bool _raisedOnExited;
        private RegisteredWaitHandle _registeredWaitHandle;
        private WaitHandle _waitHandle;
        private StreamReader _standardOutput;
        private StreamWriter _standardInput;
        private StreamReader _standardError;
        private bool _disposed;

        private static object s_createProcessLock = new object();

        private StreamReadMode _outputStreamReadMode;
        private StreamReadMode _errorStreamReadMode;

        // Support for asynchrously reading streams
        public event DataReceivedEventHandler OutputDataReceived;
        public event DataReceivedEventHandler ErrorDataReceived;

        // Abstract the stream details
        internal AsyncStreamReader _output;
        internal AsyncStreamReader _error;
        internal bool _pendingOutputRead;
        internal bool _pendingErrorRead;
#if FEATURE_TRACESWITCH
        internal static TraceSwitch _processTracing =
#if DEBUG
            new TraceSwitch("processTracing", "Controls debug output from Process component");
#else
            null;
#endif
#endif

        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of the <see cref='System.Diagnostics.Process'/> class.
        ///    </para>
        /// </devdoc>
        public Process()
        {
            _machineName = ".";
            _outputStreamReadMode = StreamReadMode.Undefined;
            _errorStreamReadMode = StreamReadMode.Undefined;
        }

        private Process(string machineName, bool isRemoteMachine, int processId, ProcessInfo processInfo)
        {
            _processInfo = processInfo;
            _machineName = machineName;
            _isRemoteMachine = isRemoteMachine;
            _processId = processId;
            _haveProcessId = true;
            _outputStreamReadMode = StreamReadMode.Undefined;
            _errorStreamReadMode = StreamReadMode.Undefined;
        }

        public SafeProcessHandle SafeHandle
        {
            get
            {
                EnsureState(State.Associated);
                return OpenProcessHandle();
            }
        }

        /// <devdoc>
        ///     Returns whether this process component is associated with a real process.
        /// </devdoc>
        /// <internalonly/>
        bool Associated
        {
            get { return _haveProcessId || _haveProcessHandle; }
        }

        /// <devdoc>
        ///    <para>
        ///       Gets the base priority of
        ///       the associated process.
        ///    </para>
        /// </devdoc>
        public int BasePriority
        {
            get
            {
                EnsureState(State.HaveProcessInfo);
                return _processInfo._basePriority;
            }
        }

        /// <devdoc>
        ///    <para>
        ///       Gets
        ///       the
        ///       value that was specified by the associated process when it was terminated.
        ///    </para>
        /// </devdoc>
        public int ExitCode
        {
            get
            {
                EnsureState(State.Exited);
                return _exitCode;
            }
        }

        /// <devdoc>
        ///    <para>
        ///       Gets a
        ///       value indicating whether the associated process has been terminated.
        ///    </para>
        /// </devdoc>
        public bool HasExited
        {
            get
            {
                if (!_exited)
                {
                    EnsureState(State.Associated);

                    int? localExitCode;
                    _exited = GetHasExited(out localExitCode);
                    if (localExitCode.HasValue)
                    {
                        _exitCode = localExitCode.Value;
                    }

                    if (_exited)
                    {
                        RaiseOnExited();
                    }
                }
                return _exited;
            }
        }

        /// <devdoc>
        ///    <para>
        ///       Gets the time that the associated process exited.
        ///    </para>
        /// </devdoc>
        public DateTime ExitTime
        {
            get
            {
                if (!_haveExitTime)
                {
                    EnsureState(State.Exited);
                    _exitTime = ExitTimeCore;
                    _haveExitTime = true;
                }
                return _exitTime;
            }
        }

        /// <devdoc>
        ///    <para>
        ///       Gets the number of handles that are associated
        ///       with the process.
        ///    </para>
        /// </devdoc>
        public int HandleCount
        {
            get
            {
                EnsureState(State.HaveProcessInfo);
                return _processInfo._handleCount;
            }
        }

        /// <devdoc>
        ///    <para>
        ///       Gets
        ///       the unique identifier for the associated process.
        ///    </para>
        /// </devdoc>
        public int Id
        {
            get
            {
                EnsureState(State.HaveId);
                return _processId;
            }
        }

        /// <devdoc>
        ///    <para>
        ///       Gets
        ///       the name of the computer on which the associated process is running.
        ///    </para>
        /// </devdoc>
        public string MachineName
        {
            get
            {
                EnsureState(State.Associated);
                return _machineName;
            }
        }

        /// <devdoc>
        ///    <para>
        ///       Gets or sets the maximum allowable working set for the associated
        ///       process.
        ///    </para>
        /// </devdoc>
        public IntPtr MaxWorkingSet
        {
            get
            {
                EnsureWorkingSetLimits();
                return _maxWorkingSet;
            }
            set
            {
                SetWorkingSetLimits(null, value);
            }
        }

        /// <devdoc>
        ///    <para>
        ///       Gets or sets the minimum allowable working set for the associated
        ///       process.
        ///    </para>
        /// </devdoc>
        public IntPtr MinWorkingSet
        {
            get
            {
                EnsureWorkingSetLimits();
                return _minWorkingSet;
            }
            set
            {
                SetWorkingSetLimits(value, null);
            }
        }

        public ProcessModuleCollection Modules
        {
            get
            {
                if (_modules == null)
                {
                    EnsureState(State.HaveId | State.IsLocal);
                    ModuleInfo[] moduleInfos = ProcessManager.GetModuleInfos(_processId);
                    ProcessModule[] newModulesArray = new ProcessModule[moduleInfos.Length];
                    for (int i = 0; i < moduleInfos.Length; i++)
                    {
                        newModulesArray[i] = new ProcessModule(moduleInfos[i]);
                    }
                    ProcessModuleCollection newModules = new ProcessModuleCollection(newModulesArray);
                    _modules = newModules;
                }
                return _modules;
            }
        }

        [System.Runtime.InteropServices.ComVisible(false)]
        public long NonpagedSystemMemorySize64
        {
            get
            {
                EnsureState(State.HaveProcessInfo);
                return _processInfo._poolNonpagedBytes;
            }
        }

        [System.Runtime.InteropServices.ComVisible(false)]
        public long PagedMemorySize64
        {
            get
            {
                EnsureState(State.HaveProcessInfo);
                return _processInfo._pageFileBytes;
            }
        }

        [System.Runtime.InteropServices.ComVisible(false)]
        public long PagedSystemMemorySize64
        {
            get
            {
                EnsureState(State.HaveProcessInfo);
                return _processInfo._poolPagedBytes;
            }
        }

        [System.Runtime.InteropServices.ComVisible(false)]
        public long PeakPagedMemorySize64
        {
            get
            {
                EnsureState(State.HaveProcessInfo);
                return _processInfo._pageFileBytesPeak;
            }
        }

        [System.Runtime.InteropServices.ComVisible(false)]
        public long PeakWorkingSet64
        {
            get
            {
                EnsureState(State.HaveProcessInfo);
                return _processInfo._workingSetPeak;
            }
        }

        [System.Runtime.InteropServices.ComVisible(false)]
        public long PeakVirtualMemorySize64
        {
            get
            {
                EnsureState(State.HaveProcessInfo);
                return _processInfo._virtualBytesPeak;
            }
        }

        /// <devdoc>
        ///    <para>
        ///       Gets or sets a value indicating whether the associated process priority
        ///       should be temporarily boosted by the operating system when the main window
        ///       has focus.
        ///    </para>
        /// </devdoc>
        public bool PriorityBoostEnabled
        {
            get
            {
                if (!_havePriorityBoostEnabled)
                {
                    _priorityBoostEnabled = PriorityBoostEnabledCore;
                    _havePriorityBoostEnabled = true;
                }
                return _priorityBoostEnabled;
            }
            set
            {
                PriorityBoostEnabledCore = value;
                _priorityBoostEnabled = value;
                _havePriorityBoostEnabled = true;
            }
        }

        /// <devdoc>
        ///    <para>
        ///       Gets or sets the overall priority category for the
        ///       associated process.
        ///    </para>
        /// </devdoc>
        public ProcessPriorityClass PriorityClass
        {
            get
            {
                if (!_havePriorityClass)
                {
                    _priorityClass = PriorityClassCore;
                    _havePriorityClass = true;
                }
                return _priorityClass;
            }
            set
            {
                if (!Enum.IsDefined(typeof(ProcessPriorityClass), value))
                {
                    throw new ArgumentException(SR.Format(SR.InvalidEnumArgument, "value", (int)value, typeof(ProcessPriorityClass)));
                }

                PriorityClassCore = value;
                _priorityClass = value;
                _havePriorityClass = true;
            }
        }

        [System.Runtime.InteropServices.ComVisible(false)]
        public long PrivateMemorySize64
        {
            get
            {
                EnsureState(State.HaveProcessInfo);
                return _processInfo._privateBytes;
            }
        }

        /// <devdoc>
        ///    <para>
        ///       Gets
        ///       the friendly name of the process.
        ///    </para>
        /// </devdoc>
        public string ProcessName
        {
            get
            {
                EnsureState(State.HaveProcessInfo);
                return _processInfo._processName;
            }
        }

        /// <devdoc>
        ///    <para>
        ///       Gets
        ///       or sets which processors the threads in this process can be scheduled to run on.
        ///    </para>
        /// </devdoc>
        public IntPtr ProcessorAffinity
        {
            get
            {
                if (!_haveProcessorAffinity)
                {
                    _processorAffinity = ProcessorAffinityCore;
                    _haveProcessorAffinity = true;
                }
                return _processorAffinity;
            }
            set
            {
                ProcessorAffinityCore = value;
                _processorAffinity = value;
                _haveProcessorAffinity = true;
            }
        }

        public int SessionId
        {
            get
            {
                EnsureState(State.HaveProcessInfo);
                return _processInfo._sessionId;
            }
        }

        /// <devdoc>
        ///    <para>
        ///       Gets or sets the properties to pass into the <see cref='System.Diagnostics.Process.Start'/> method for the <see cref='System.Diagnostics.Process'/>
        ///       .
        ///    </para>
        /// </devdoc>
        public ProcessStartInfo StartInfo
        {
            get
            {
                if (_startInfo == null)
                {
                    _startInfo = new ProcessStartInfo(this);
                }
                return _startInfo;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                _startInfo = value;
            }
        }

        /// <devdoc>
        ///    <para>
        ///       Gets the set of threads that are running in the associated
        ///       process.
        ///    </para>
        /// </devdoc>
        public ProcessThreadCollection Threads
        {
            get
            {
                if (_threads == null)
                {
                    EnsureState(State.HaveProcessInfo);
                    int count = _processInfo._threadInfoList.Count;
                    ProcessThread[] newThreadsArray = new ProcessThread[count];
                    for (int i = 0; i < count; i++)
                    {
                        newThreadsArray[i] = new ProcessThread(_isRemoteMachine, (ThreadInfo)_processInfo._threadInfoList[i]);
                    }
                    ProcessThreadCollection newThreads = new ProcessThreadCollection(newThreadsArray);
                    _threads = newThreads;
                }
                return _threads;
            }
        }

        [System.Runtime.InteropServices.ComVisible(false)]
        public long VirtualMemorySize64
        {
            get
            {
                EnsureState(State.HaveProcessInfo);
                return _processInfo._virtualBytes;
            }
        }

        /// <devdoc>
        ///    <para>
        ///       Gets or sets whether the <see cref='System.Diagnostics.Process.Exited'/>
        ///       event is fired
        ///       when the process terminates.
        ///    </para>
        /// </devdoc>
        public bool EnableRaisingEvents
        {
            get
            {
                return _watchForExit;
            }
            set
            {
                if (value != _watchForExit)
                {
                    if (Associated)
                    {
                        if (value)
                        {
                            OpenProcessHandle();
                            EnsureWatchingForExit();
                        }
                        else
                        {
                            StopWatchingForExit();
                        }
                    }
                    _watchForExit = value;
                }
            }
        }


        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public StreamWriter StandardInput
        {
            get
            {
                if (_standardInput == null)
                {
                    throw new InvalidOperationException(SR.CantGetStandardIn);
                }

                return _standardInput;
            }
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public StreamReader StandardOutput
        {
            get
            {
                if (_standardOutput == null)
                {
                    throw new InvalidOperationException(SR.CantGetStandardOut);
                }

                if (_outputStreamReadMode == StreamReadMode.Undefined)
                {
                    _outputStreamReadMode = StreamReadMode.SyncMode;
                }
                else if (_outputStreamReadMode != StreamReadMode.SyncMode)
                {
                    throw new InvalidOperationException(SR.CantMixSyncAsyncOperation);
                }

                return _standardOutput;
            }
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public StreamReader StandardError
        {
            get
            {
                if (_standardError == null)
                {
                    throw new InvalidOperationException(SR.CantGetStandardError);
                }

                if (_errorStreamReadMode == StreamReadMode.Undefined)
                {
                    _errorStreamReadMode = StreamReadMode.SyncMode;
                }
                else if (_errorStreamReadMode != StreamReadMode.SyncMode)
                {
                    throw new InvalidOperationException(SR.CantMixSyncAsyncOperation);
                }

                return _standardError;
            }
        }

        [System.Runtime.InteropServices.ComVisible(false)]
        public long WorkingSet64
        {
            get
            {
                EnsureState(State.HaveProcessInfo);
                return _processInfo._workingSet;
            }
        }

        public event EventHandler Exited
        {
            add
            {
                _onExited += value;
            }
            remove
            {
                _onExited -= value;
            }
        }

        /// <devdoc>
        ///     Release the temporary handle we used to get process information.
        ///     If we used the process handle stored in the process object (we have all access to the handle,) don't release it.
        /// </devdoc>
        /// <internalonly/>
        void ReleaseProcessHandle(SafeProcessHandle handle)
        {
            if (handle == null)
            {
                return;
            }

            if (_haveProcessHandle && handle == _processHandle)
            {
                return;
            }
#if FEATURE_TRACESWITCH
            Debug.WriteLineIf(_processTracing.TraceVerbose, "Process - CloseHandle(process)");
#endif
            handle.Dispose();
        }

        /// <devdoc>
        ///     This is called from the threadpool when a proces exits.
        /// </devdoc>
        /// <internalonly/>
        private void CompletionCallback(object context, bool wasSignaled)
        {
            StopWatchingForExit();
            RaiseOnExited();
        }

        /// <internalonly/>
        /// <devdoc>
        ///    <para>
        ///       Free any resources associated with this component.
        ///    </para>
        /// </devdoc>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    //Dispose managed and unmanaged resources
                    Close();
                }
                _disposed = true;
            }
        }

        /// <devdoc>
        ///    <para>
        ///       Frees any resources associated with this component.
        ///    </para>
        /// </devdoc>
        internal void Close()
        {
            if (Associated)
            {
                if (_haveProcessHandle)
                {
                    StopWatchingForExit();
#if FEATURE_TRACESWITCH
                    Debug.WriteLineIf(_processTracing.TraceVerbose, "Process - CloseHandle(process) in Close()");
#endif
                    _processHandle.Dispose();
                    _processHandle = null;
                    _haveProcessHandle = false;
                }
                _haveProcessId = false;
                _isRemoteMachine = false;
                _machineName = ".";
                _raisedOnExited = false;

                //Don't call close on the Readers and writers
                //since they might be referenced by somebody else while the 
                //process is still alive but this method called.
                _standardOutput = null;
                _standardInput = null;
                _standardError = null;

                _output = null;
                _error = null;

                Refresh();
            }
        }

        /// <devdoc>
        ///     Helper method for checking preconditions when accessing properties.
        /// </devdoc>
        /// <internalonly/>
        void EnsureState(State state)
        {
            if ((state & State.Associated) != (State)0)
                if (!Associated)
                    throw new InvalidOperationException(SR.NoAssociatedProcess);

            if ((state & State.HaveId) != (State)0)
            {
                if (!_haveProcessId)
                {
                    if (_haveProcessHandle)
                    {
                        SetProcessId(ProcessManager.GetProcessIdFromHandle(_processHandle));
                    }
                    else
                    {
                        EnsureState(State.Associated);
                        throw new InvalidOperationException(SR.ProcessIdRequired);
                    }
                }
            }

            if ((state & State.IsLocal) != (State)0 && _isRemoteMachine)
            {
                throw new NotSupportedException(SR.NotSupportedRemote);
            }

            if ((state & State.HaveProcessInfo) != (State)0)
            {
                if (_processInfo == null)
                {
                    if ((state & State.HaveId) == (State)0) EnsureState(State.HaveId);
                    ProcessInfo[] processInfos = ProcessManager.GetProcessInfos(_machineName);
                    for (int i = 0; i < processInfos.Length; i++)
                    {
                        if (processInfos[i]._processId == _processId)
                        {
                            _processInfo = processInfos[i];
                            break;
                        }
                    }
                    if (_processInfo == null)
                    {
                        throw new InvalidOperationException(SR.NoProcessInfo);
                    }
                }
            }

            if ((state & State.Exited) != (State)0)
            {
                if (!HasExited)
                {
                    throw new InvalidOperationException(SR.WaitTillExit);
                }

                if (!_haveProcessHandle)
                {
                    throw new InvalidOperationException(SR.NoProcessHandle);
                }
            }
        }

        /// <devdoc>
        ///     Make sure we are watching for a process exit.
        /// </devdoc>
        /// <internalonly/>
        void EnsureWatchingForExit()
        {
            if (!_watchingForExit)
            {
                lock (this)
                {
                    if (!_watchingForExit)
                    {
                        Debug.Assert(_haveProcessHandle, "Process.EnsureWatchingForExit called with no process handle");
                        Debug.Assert(Associated, "Process.EnsureWatchingForExit called with no associated process");
                        _watchingForExit = true;
                        try
                        {
                            _waitHandle = new ProcessWaitHandle(_processHandle);
                            _registeredWaitHandle = ThreadPool.RegisterWaitForSingleObject(_waitHandle,
                                new WaitOrTimerCallback(CompletionCallback), null, -1, true);
                        }
                        catch
                        {
                            _watchingForExit = false;
                            throw;
                        }
                    }
                }
            }
        }

        /// <devdoc>
        ///     Make sure we have obtained the min and max working set limits.
        /// </devdoc>
        /// <internalonly/>
        void EnsureWorkingSetLimits()
        {
            if (!_haveWorkingSetLimits)
            {
                GetWorkingSetLimits(out _minWorkingSet, out _maxWorkingSet);
                _haveWorkingSetLimits = true;
            }
        }

        /// <devdoc>
        ///     Helper to set minimum or maximum working set limits.
        /// </devdoc>
        /// <internalonly/>
        void SetWorkingSetLimits(IntPtr? min, IntPtr? max)
        {
            SetWorkingSetLimitsCore(min, max, out _minWorkingSet, out _maxWorkingSet);
            _haveWorkingSetLimits = true;
        }

        /// <devdoc>
        ///    <para>
        ///       Returns a new <see cref='System.Diagnostics.Process'/> component given a process identifier and
        ///       the name of a computer in the network.
        ///    </para>
        /// </devdoc>
        public static Process GetProcessById(int processId, string machineName)
        {
            if (!ProcessManager.IsProcessRunning(processId, machineName))
            {
                throw new ArgumentException(SR.Format(SR.MissingProccess, processId.ToString(CultureInfo.CurrentCulture)));
            }

            return new Process(machineName, ProcessManager.IsRemoteMachine(machineName), processId, null);
        }

        /// <devdoc>
        ///    <para>
        ///       Returns a new <see cref='System.Diagnostics.Process'/> component given the
        ///       identifier of a process on the local computer.
        ///    </para>
        /// </devdoc>
        public static Process GetProcessById(int processId)
        {
            return GetProcessById(processId, ".");
        }

        /// <devdoc>
        ///    <para>
        ///       Creates an array of <see cref='System.Diagnostics.Process'/> components that are
        ///       associated
        ///       with process resources on the
        ///       local computer. These process resources share the specified process name.
        ///    </para>
        /// </devdoc>
        public static Process[] GetProcessesByName(string processName)
        {
            return GetProcessesByName(processName, ".");
        }

        /// <devdoc>
        ///    <para>
        ///       Creates an array of <see cref='System.Diagnostics.Process'/> components that are associated with process resources on a
        ///       remote computer. These process resources share the specified process name.
        ///    </para>
        /// </devdoc>
        public static Process[] GetProcessesByName(string processName, string machineName)
        {
            if (processName == null) processName = String.Empty;
            Process[] procs = GetProcesses(machineName);
            List<Process> list = new List<Process>();

            for (int i = 0; i < procs.Length; i++)
            {
                if (String.Equals(processName, procs[i].ProcessName, StringComparison.OrdinalIgnoreCase))
                {
                    list.Add(procs[i]);
                }
                else
                {
                    procs[i].Dispose();
                }
            }

            Process[] temp = new Process[list.Count];
            list.CopyTo(temp, 0);
            return temp;
        }

        /// <devdoc>
        ///    <para>
        ///       Creates a new <see cref='System.Diagnostics.Process'/>
        ///       component for each process resource on the local computer.
        ///    </para>
        /// </devdoc>
        public static Process[] GetProcesses()
        {
            return GetProcesses(".");
        }

        /// <devdoc>
        ///    <para>
        ///       Creates a new <see cref='System.Diagnostics.Process'/>
        ///       component for each
        ///       process resource on the specified computer.
        ///    </para>
        /// </devdoc>
        public static Process[] GetProcesses(string machineName)
        {
            bool isRemoteMachine = ProcessManager.IsRemoteMachine(machineName);
            ProcessInfo[] processInfos = ProcessManager.GetProcessInfos(machineName);
            Process[] processes = new Process[processInfos.Length];
            for (int i = 0; i < processInfos.Length; i++)
            {
                ProcessInfo processInfo = processInfos[i];
                processes[i] = new Process(machineName, isRemoteMachine, processInfo._processId, processInfo);
            }
#if FEATURE_TRACESWITCH
            Debug.WriteLineIf(_processTracing.TraceVerbose, "Process.GetProcesses(" + machineName + ")");
#if DEBUG
            if (_processTracing.TraceVerbose) {
                Debug.Indent();
                for (int i = 0; i < processInfos.Length; i++) {
                    Debug.WriteLine(processes[i].Id + ": " + processes[i].ProcessName);
                }
                Debug.Unindent();
            }
#endif
#endif
            return processes;
        }

        /// <devdoc>
        ///    <para>
        ///       Returns a new <see cref='System.Diagnostics.Process'/>
        ///       component and associates it with the current active process.
        ///    </para>
        /// </devdoc>
        public static Process GetCurrentProcess()
        {
            return new Process(".", false, GetCurrentProcessId(), null);
        }

        /// <devdoc>
        ///    <para>
        ///       Raises the <see cref='System.Diagnostics.Process.Exited'/> event.
        ///    </para>
        /// </devdoc>
        protected void OnExited()
        {
            EventHandler exited = _onExited;
            if (exited != null)
            {
                exited(this, EventArgs.Empty);
            }
        }

        /// <devdoc>
        ///     Raise the Exited event, but make sure we don't do it more than once.
        /// </devdoc>
        /// <internalonly/>
        void RaiseOnExited()
        {
            if (!_raisedOnExited)
            {
                lock (this)
                {
                    if (!_raisedOnExited)
                    {
                        _raisedOnExited = true;
                        OnExited();
                    }
                }
            }
        }

        /// <devdoc>
        ///    <para>
        ///       Discards any information about the associated process
        ///       that has been cached inside the process component. After <see cref='System.Diagnostics.Process.Refresh'/> is called, the
        ///       first request for information for each property causes the process component
        ///       to obtain a new value from the associated process.
        ///    </para>
        /// </devdoc>
        public void Refresh()
        {
            _processInfo = null;
            _threads = null;
            _modules = null;
            _exited = false;
            _signaled = false;
            _haveWorkingSetLimits = false;
            _haveProcessorAffinity = false;
            _havePriorityClass = false;
            _haveExitTime = false;
            _havePriorityBoostEnabled = false;
        }

        /// <devdoc>
        ///     Helper to associate a process handle with this component.
        /// </devdoc>
        /// <internalonly/>
        void SetProcessHandle(SafeProcessHandle processHandle)
        {
            _processHandle = processHandle;
            _haveProcessHandle = true;
            if (_watchForExit)
            {
                EnsureWatchingForExit();
            }
        }

        /// <devdoc>
        ///     Helper to associate a process id with this component.
        /// </devdoc>
        /// <internalonly/>
        void SetProcessId(int processId)
        {
            _processId = processId;
            _haveProcessId = true;
        }

        /// <devdoc>
        ///    <para>
        ///       Starts a process specified by the <see cref='System.Diagnostics.Process.StartInfo'/> property of this <see cref='System.Diagnostics.Process'/>
        ///       component and associates it with the
        ///    <see cref='System.Diagnostics.Process'/> . If a process resource is reused 
        ///       rather than started, the reused process is associated with this <see cref='System.Diagnostics.Process'/>
        ///       component.
        ///    </para>
        /// </devdoc>
        public bool Start()
        {
            Close();
            ProcessStartInfo startInfo = StartInfo;
            if (startInfo.FileName.Length == 0)
                throw new InvalidOperationException(SR.FileNameMissing);

            return StartCore(startInfo);
        }

        private static StringBuilder BuildCommandLine(string executableFileName, string arguments)
        {
            // Construct a StringBuilder with the appropriate command line
            // to pass to CreateProcess.  If the filename isn't already 
            // in quotes, we quote it here.  This prevents some security
            // problems (it specifies exactly which part of the string
            // is the file to execute).
            StringBuilder commandLine = new StringBuilder();
            string fileName = executableFileName.Trim();
            bool fileNameIsQuoted = (fileName.StartsWith("\"", StringComparison.Ordinal) && fileName.EndsWith("\"", StringComparison.Ordinal));
            if (!fileNameIsQuoted)
            {
                commandLine.Append("\"");
            }

            commandLine.Append(fileName);

            if (!fileNameIsQuoted)
            {
                commandLine.Append("\"");
            }

            if (!String.IsNullOrEmpty(arguments))
            {
                commandLine.Append(" ");
                commandLine.Append(arguments);
            }

            return commandLine;
        }

        // In most scenario 437 is the codepage used for Console encoding. However this encoding is not available by default and so we use the try{} catch{} pattern and use UTF8 in case of failure.
        // This ensures that if the user uses Encoding.RegisterProvider to register the encoding the Process class can automatically get the codepage as well.
        private static Encoding GetEncoding(int codePage)
        {
            Encoding enc = null;
            try
            {
                enc = Encoding.GetEncoding(codePage);
            }
            catch (NotSupportedException)
            {
                // There is no data available for the above codePage so we will use UTF8 instead with emitPrefix set to false.
                enc = new UTF8Encoding(false);
            }
            return enc;
        }

        /// <devdoc>
        ///    <para>
        ///       Starts a process resource by specifying the name of a
        ///       document or application file. Associates the process resource with a new <see cref='System.Diagnostics.Process'/>
        ///       component.
        ///    </para>
        /// </devdoc>
        public static Process Start(string fileName)
        {
            return Start(new ProcessStartInfo(fileName));
        }

        /// <devdoc>
        ///    <para>
        ///       Starts a process resource by specifying the name of an
        ///       application and a set of command line arguments. Associates the process resource
        ///       with a new <see cref='System.Diagnostics.Process'/>
        ///       component.
        ///    </para>
        /// </devdoc>
        public static Process Start(string fileName, string arguments)
        {
            return Start(new ProcessStartInfo(fileName, arguments));
        }

        /// <devdoc>
        ///    <para>
        ///       Starts a process resource specified by the process start
        ///       information passed in, for example the file name of the process to start.
        ///       Associates the process resource with a new <see cref='System.Diagnostics.Process'/>
        ///       component.
        ///    </para>
        /// </devdoc>
        public static Process Start(ProcessStartInfo startInfo)
        {
            Process process = new Process();
            if (startInfo == null) throw new ArgumentNullException("startInfo");
            process.StartInfo = startInfo;
            if (process.Start())
            {
                return process;
            }
            return null;
        }

        /// <devdoc>
        ///     Make sure we are not watching for process exit.
        /// </devdoc>
        /// <internalonly/>
        void StopWatchingForExit()
        {
            if (_watchingForExit)
            {
                lock (this)
                {
                    if (_watchingForExit)
                    {
                        _watchingForExit = false;
                        _registeredWaitHandle.Unregister(null);
                        _waitHandle.Dispose();
                        _waitHandle = null;
                        _registeredWaitHandle = null;
                    }
                }
            }
        }

        public override string ToString()
        {
            if (Associated)
            {
                string processName = ProcessName;
                if (processName.Length != 0)
                {
                    return String.Format(CultureInfo.CurrentCulture, "{0} ({1})", base.ToString(), processName);
                }
            }
            return base.ToString();
        }

        /// <devdoc>
        ///    <para>
        ///       Instructs the <see cref='System.Diagnostics.Process'/> component to wait
        ///       indefinitely for the associated process to exit.
        ///    </para>
        /// </devdoc>
        public void WaitForExit()
        {
            WaitForExit(-1);
        }

        // Support for working asynchronously with streams
        /// <devdoc>
        /// <para>
        /// Instructs the <see cref='System.Diagnostics.Process'/> component to start
        /// reading the StandardOutput stream asynchronously. The user can register a callback
        /// that will be called when a line of data terminated by \n,\r or \r\n is reached, or the end of stream is reached
        /// then the remaining information is returned. The user can add an event handler to OutputDataReceived.
        /// </para>
        /// </devdoc>
        [System.Runtime.InteropServices.ComVisible(false)]
        public void BeginOutputReadLine()
        {
            if (_outputStreamReadMode == StreamReadMode.Undefined)
            {
                _outputStreamReadMode = StreamReadMode.AsyncMode;
            }
            else if (_outputStreamReadMode != StreamReadMode.AsyncMode)
            {
                throw new InvalidOperationException(SR.CantMixSyncAsyncOperation);
            }

            if (_pendingOutputRead)
                throw new InvalidOperationException(SR.PendingAsyncOperation);

            _pendingOutputRead = true;
            // We can't detect if there's a pending sychronous read, stream also doesn't.
            if (_output == null)
            {
                if (_standardOutput == null)
                {
                    throw new InvalidOperationException(SR.CantGetStandardOut);
                }

                Stream s = _standardOutput.BaseStream;
                _output = new AsyncStreamReader(this, s, OutputReadNotifyUser, _standardOutput.CurrentEncoding);
            }
            _output.BeginReadLine();
        }


        /// <devdoc>
        /// <para>
        /// Instructs the <see cref='System.Diagnostics.Process'/> component to start
        /// reading the StandardError stream asynchronously. The user can register a callback
        /// that will be called when a line of data terminated by \n,\r or \r\n is reached, or the end of stream is reached
        /// then the remaining information is returned. The user can add an event handler to ErrorDataReceived.
        /// </para>
        /// </devdoc>
        [System.Runtime.InteropServices.ComVisible(false)]
        public void BeginErrorReadLine()
        {
            if (_errorStreamReadMode == StreamReadMode.Undefined)
            {
                _errorStreamReadMode = StreamReadMode.AsyncMode;
            }
            else if (_errorStreamReadMode != StreamReadMode.AsyncMode)
            {
                throw new InvalidOperationException(SR.CantMixSyncAsyncOperation);
            }

            if (_pendingErrorRead)
            {
                throw new InvalidOperationException(SR.PendingAsyncOperation);
            }

            _pendingErrorRead = true;
            // We can't detect if there's a pending sychronous read, stream also doesn't.
            if (_error == null)
            {
                if (_standardError == null)
                {
                    throw new InvalidOperationException(SR.CantGetStandardError);
                }

                Stream s = _standardError.BaseStream;
                _error = new AsyncStreamReader(this, s, ErrorReadNotifyUser, _standardError.CurrentEncoding);
            }
            _error.BeginReadLine();
        }

        /// <devdoc>
        /// <para>
        /// Instructs the <see cref='System.Diagnostics.Process'/> component to cancel the asynchronous operation
        /// specified by BeginOutputReadLine().
        /// </para>
        /// </devdoc>
        [System.Runtime.InteropServices.ComVisible(false)]
        public void CancelOutputRead()
        {
            if (_output != null)
            {
                _output.CancelOperation();
            }
            else
            {
                throw new InvalidOperationException(SR.NoAsyncOperation);
            }

            _pendingOutputRead = false;
        }

        /// <devdoc>
        /// <para>
        /// Instructs the <see cref='System.Diagnostics.Process'/> component to cancel the asynchronous operation
        /// specified by BeginErrorReadLine().
        /// </para>
        /// </devdoc>
        [System.Runtime.InteropServices.ComVisible(false)]
        public void CancelErrorRead()
        {
            if (_error != null)
            {
                _error.CancelOperation();
            }
            else
            {
                throw new InvalidOperationException(SR.NoAsyncOperation);
            }

            _pendingErrorRead = false;
        }

        internal void OutputReadNotifyUser(String data)
        {
            // To avoid race between remove handler and raising the event
            DataReceivedEventHandler outputDataReceived = OutputDataReceived;
            if (outputDataReceived != null)
            {
                DataReceivedEventArgs e = new DataReceivedEventArgs(data);
                outputDataReceived(this, e);  // Call back to user informing data is available
            }
        }

        internal void ErrorReadNotifyUser(String data)
        {
            // To avoid race between remove handler and raising the event
            DataReceivedEventHandler errorDataReceived = ErrorDataReceived;
            if (errorDataReceived != null)
            {
                DataReceivedEventArgs e = new DataReceivedEventArgs(data);
                errorDataReceived(this, e); // Call back to user informing data is available.
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// This enum defines the operation mode for redirected process stream.
        /// We don't support switching between synchronous mode and asynchronous mode.
        /// </summary>
        private enum StreamReadMode
        {
            Undefined,
            SyncMode,
            AsyncMode
        }

        /// <summary>A desired internal state.</summary>
        private enum State
        {
            HaveId = 0x1,
            IsLocal = 0x2,
            HaveProcessInfo = 0x8,
            Exited = 0x10,
            Associated = 0x20,
        }
    }
}