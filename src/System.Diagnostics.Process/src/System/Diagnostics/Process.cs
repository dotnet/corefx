// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Win32.SafeHandles;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security;
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

        // Support for asynchronously reading streams
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
                return _processInfo.BasePriority;
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
                    UpdateHasExited();
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

        public long NonpagedSystemMemorySize64
        {
            get
            {
                EnsureState(State.HaveProcessInfo);
                return _processInfo.PoolNonPagedBytes;
            }
        }

        public long PagedMemorySize64
        {
            get
            {
                EnsureState(State.HaveProcessInfo);
                return _processInfo.PageFileBytes;
            }
        }

        public long PagedSystemMemorySize64
        {
            get
            {
                EnsureState(State.HaveProcessInfo);
                return _processInfo.PoolPagedBytes;
            }
        }

        public long PeakPagedMemorySize64
        {
            get
            {
                EnsureState(State.HaveProcessInfo);
                return _processInfo.PageFileBytesPeak;
            }
        }

        public long PeakWorkingSet64
        {
            get
            {
                EnsureState(State.HaveProcessInfo);
                return _processInfo.WorkingSetPeak;
            }
        }

        public long PeakVirtualMemorySize64
        {
            get
            {
                EnsureState(State.HaveProcessInfo);
                return _processInfo.VirtualBytesPeak;
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

        public long PrivateMemorySize64
        {
            get
            {
                EnsureState(State.HaveProcessInfo);
                return _processInfo.PrivateBytes;
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
                return _processInfo.ProcessName;
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
                return _processInfo.SessionId;
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
                    if (Associated)
                    {
                        throw new InvalidOperationException(SR.CantGetProcessStartInfo);
                    }

                    _startInfo = new ProcessStartInfo();
                }
                return _startInfo;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                
                if (Associated)
                {
                    throw new InvalidOperationException(SR.CantSetProcessStartInfo);
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
                        newThreadsArray[i] = new ProcessThread(_isRemoteMachine, _processId, (ThreadInfo)_processInfo._threadInfoList[i]);
                    }

                    ProcessThreadCollection newThreads = new ProcessThreadCollection(newThreadsArray);
                    _threads = newThreads;
                }
                return _threads;
            }
        }

        public long VirtualMemorySize64
        {
            get
            {
                EnsureState(State.HaveProcessInfo);
                return _processInfo.VirtualBytes;
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

        public long WorkingSet64
        {
            get
            {
                EnsureState(State.HaveProcessInfo);
                return _processInfo.WorkingSet;
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
        private void ReleaseProcessHandle(SafeProcessHandle handle)
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
        ///     This is called from the threadpool when a process exits.
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

                CloseCore();
                Refresh();
            }
        }

        /// <devdoc>
        ///     Helper method for checking preconditions when accessing properties.
        /// </devdoc>
        /// <internalonly/>
        private void EnsureState(State state)
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
                    _processInfo = ProcessManager.GetProcessInfo(_processId, _machineName);
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
        private void EnsureWatchingForExit()
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
        private void EnsureWorkingSetLimits()
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
        private void SetWorkingSetLimits(IntPtr? min, IntPtr? max)
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
                processes[i] = new Process(machineName, isRemoteMachine, processInfo.ProcessId, processInfo);
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
        private void RaiseOnExited()
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
            _haveWorkingSetLimits = false;
            _haveProcessorAffinity = false;
            _havePriorityClass = false;
            _haveExitTime = false;
            _havePriorityBoostEnabled = false;
            RefreshCore();
        }

        /// <summary>
        /// Opens a long-term handle to the process, with all access.  If a handle exists,
        /// then it is reused.  If the process has exited, it throws an exception.
        /// </summary>
        private SafeProcessHandle OpenProcessHandle()
        {
            if (!_haveProcessHandle)
            {
                //Cannot open a new process handle if the object has been disposed, since finalization has been suppressed.            
                if (_disposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                SetProcessHandle(GetProcessHandle());
            }
            return _processHandle;
        }

        /// <devdoc>
        ///     Helper to associate a process handle with this component.
        /// </devdoc>
        /// <internalonly/>
        private void SetProcessHandle(SafeProcessHandle processHandle)
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
        private void SetProcessId(int processId)
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
            {
                throw new InvalidOperationException(SR.FileNameMissing);
            }
            if (startInfo.StandardOutputEncoding != null && !startInfo.RedirectStandardOutput)
            {
                throw new InvalidOperationException(SR.StandardOutputEncodingNotAllowed);
            }
            if (startInfo.StandardErrorEncoding != null && !startInfo.RedirectStandardError)
            {
                throw new InvalidOperationException(SR.StandardErrorEncodingNotAllowed);
            }

            //Cannot start a new process and store its handle if the object has been disposed, since finalization has been suppressed.            
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }

            return StartCore(startInfo);
        }

        private static Encoding GetEncoding(int codePage)
        {
            return EncodingHelper.GetSupportedConsoleEncoding(codePage);
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
            if (startInfo == null)
                throw new ArgumentNullException("startInfo");

            process.StartInfo = startInfo;
            return process.Start() ? 
                process : 
                null;
        }

        /// <devdoc>
        ///     Make sure we are not watching for process exit.
        /// </devdoc>
        /// <internalonly/>
        private void StopWatchingForExit()
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
            WaitForExit(Timeout.Infinite);
        }

        /// <summary>
        /// Instructs the Process component to wait the specified number of milliseconds for 
        /// the associated process to exit.
        /// </summary>
        public bool WaitForExit(int milliseconds)
        {
            bool exited = WaitForExitCore(milliseconds);
            if (exited && _watchForExit)
            {
                RaiseOnExited();
            }
            return exited;
        }

        /// <devdoc>
        /// <para>
        /// Instructs the <see cref='System.Diagnostics.Process'/> component to start
        /// reading the StandardOutput stream asynchronously. The user can register a callback
        /// that will be called when a line of data terminated by \n,\r or \r\n is reached, or the end of stream is reached
        /// then the remaining information is returned. The user can add an event handler to OutputDataReceived.
        /// </para>
        /// </devdoc>
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
            // We can't detect if there's a pending synchronous read, stream also doesn't.
            if (_output == null)
            {
                if (_standardOutput == null)
                {
                    throw new InvalidOperationException(SR.CantGetStandardOut);
                }

                Stream s = _standardOutput.BaseStream;
                _output = new AsyncStreamReader(s, OutputReadNotifyUser, _standardOutput.CurrentEncoding);
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
            // We can't detect if there's a pending synchronous read, stream also doesn't.
            if (_error == null)
            {
                if (_standardError == null)
                {
                    throw new InvalidOperationException(SR.CantGetStandardError);
                }

                Stream s = _standardError.BaseStream;
                _error = new AsyncStreamReader(s, ErrorReadNotifyUser, _standardError.CurrentEncoding);
            }
            _error.BeginReadLine();
        }

        /// <devdoc>
        /// <para>
        /// Instructs the <see cref='System.Diagnostics.Process'/> component to cancel the asynchronous operation
        /// specified by BeginOutputReadLine().
        /// </para>
        /// </devdoc>
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
