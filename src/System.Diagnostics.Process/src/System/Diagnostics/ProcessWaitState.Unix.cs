// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace System.Diagnostics
{
    // Overview
    // --------
    // We have a few constraints we're working under here:
    // - waitpid is used on Unix to get the exit status (including exit code) of a child process, but the first call
    //   to it after the child has completed will reap the child removing the chance of subsequent calls getting status.
    // - The Process design allows for multiple independent Process objects to be handed out, and each of those
    //   objects may be used concurrently with each other, even if they refer to the same underlying process.
    //   Same with ProcessWaitHandle objects.  This is based on the Windows design where anyone with a handle to the
    //   process can retrieve completion information about that process.
    // - There is no good Unix equivalent to a process handle nor to being able to asynchronously be notified
    //   of a process' exit (without more intrusive mechanisms like ptrace), which means such support
    //   needs to be layered on top of waitpid.
    // 
    // As a result, we have the following scheme:
    // - We maintain a static/shared table that maps process ID to ProcessWaitState objects.
    //   Access to this table requires taking a global lock, so we try to minimize the number of
    //   times we need to access the table, primarily just the first time a Process object needs
    //   access to process exit/wait information and subsequently when that Process object gets GC'd.
    // - Each process holds a ProcessWaitState.Holder object; when that object is constructed,
    //   it ensures there's an appropriate entry in the mapping table and increments that entry's ref count.
    // - When a Process object is dropped and its ProcessWaitState.Holder is finalized, it'll
    //   decrement the ref count, and when no more process objects exist for a particular process ID,
    //   that entry in the table will be cleaned up.
    // - This approach effectively allows for multiple independent Process objects for the same process ID to all
    //   share the same ProcessWaitState.  And since they are sharing the same wait state object,
    //   the wait state object uses its own lock to protect the per-process state.  This includes
    //   caching exit / exit code / exit time information so that a Process object for a process that's already
    //   had waitpid called for it can get at its exit information.
    //
    // A negative ramification of this is that if a process exits, but there are outstanding wait handles 
    // handed out (and rooted, so they can't be GC'd), and then a new process is created and the pid is recycled, 
    // new calls to get that process's wait state will get the old process's wait state.  However, pid recycling
    // will be a more general issue, since pids are the only identifier we have to a process, so if a Process
    // object is created for a particular pid, then that process goes away and a new one comes in with the same pid,
    // our Process object will silently switch to referring to the new pid.  Unix systems typically have a simple
    // policy for pid recycling, which is that they start at a low value, increment up to a system maximum (e.g.
    // 32768), and then wrap around and start reusing value that aren't currently in use.  On Linux, 
    // proc/sys/kernel/pid_max defines the max pid value.  Given the conditions that would be required for this
    // to happen, it's possible but unlikely.

    /// <summary>Exit information and waiting capabilities for a process.</summary>
    internal sealed class ProcessWaitState : IDisposable
    {
        /// <summary>
        /// Finalizable holder for a process wait state. Instantiating one
        /// will ensure that a wait state object exists for a process, will
        /// grab it, and will increment its ref count.  Dropping or disposing
        /// one will decrement the ref count and clean up after it if the ref
        /// count hits zero.
        /// </summary>
        internal sealed class Holder : IDisposable
        {
            internal ProcessWaitState _state;

            internal Holder(int processId, bool isNewChild = false)
            {
                _state = ProcessWaitState.AddRef(processId, isNewChild);
            }

            ~Holder()
            {
                // Don't try to Dispose resources (like ManualResetEvents) if 
                // the process is shutting down.
                if (_state != null && !Environment.HasShutdownStarted)
                {
                    _state.ReleaseRef();
                }
            }

            public void Dispose()
            {
                if (_state != null)
                {
                    GC.SuppressFinalize(this);
                    _state.ReleaseRef();
                    _state = null;
                }
            }
        }

        /// <summary>
        /// Global table that maps process IDs of non-child Processes to the associated shared wait state information.
        /// </summary>
        private static readonly Dictionary<int, ProcessWaitState> s_processWaitStates =
            new Dictionary<int, ProcessWaitState>();

        /// <summary>
        /// Global table that maps process IDs of child Processes to the associated shared wait state information.
        /// </summary>
        private static readonly Dictionary<int, ProcessWaitState> s_childProcessWaitStates =
            new Dictionary<int, ProcessWaitState>();

        /// <summary>
        /// Ensures that the mapping table contains an entry for the process ID,
        /// increments its ref count, and returns it.
        /// </summary>
        /// <param name="processId">The process ID for which we need wait state.</param>
        /// <returns>The wait state object.</returns>
        internal static ProcessWaitState AddRef(int processId, bool isNewChild)
        {
            lock (s_childProcessWaitStates)
            {
                ProcessWaitState pws;
                if (isNewChild)
                {
                    pws = new ProcessWaitState(processId, isChild: true);
                    s_childProcessWaitStates.Add(processId, pws);
                    pws._outstandingRefCount++; // For Holder
                    pws._outstandingRefCount++; // Decremented in CheckChildren
                }
                else
                {
                    lock (s_processWaitStates)
                    {
                        // We are referencing an existing process.
                        // This may be a child process, so we check s_childProcessWaitStates too.
                        if (!s_childProcessWaitStates.TryGetValue(processId, out pws) &&
                            !s_processWaitStates.TryGetValue(processId, out pws))
                        {
                            pws = new ProcessWaitState(processId, isChild: false);
                            s_processWaitStates.Add(processId, pws);
                        }
                        pws._outstandingRefCount++;
                    }
                }
                return pws;
            }
        }

        /// <summary>
        /// Decrements the ref count on the wait state object, and if it's the last one,
        /// removes it from the table.
        /// </summary>
        internal bool ReleaseRef()
        {
            ProcessWaitState pws;
            Dictionary<int, ProcessWaitState> waitStates = _isChild ? s_childProcessWaitStates : s_processWaitStates;
            bool removed = false;
            lock (waitStates)
            {
                bool foundState = waitStates.TryGetValue(_processId, out pws);
                Debug.Assert(foundState);
                if (foundState)
                {
                    --pws._outstandingRefCount;
                    if (pws._outstandingRefCount == 0)
                    {
                        waitStates.Remove(_processId);
                        removed = true;
                    }
                    else
                    {
                        pws = null;
                    }
                }
            }
            pws?.Dispose();
            return removed;
        }

        /// <summary>
        /// Synchronization object used to protect all instance state.  Any number of
        /// Process and ProcessWaitHandle objects may be using a ProcessWaitState
        /// instance concurrently.
        /// </summary>
        private readonly object _gate = new object();
        /// <summary>ID of the associated process.</summary>
        private readonly int _processId;
        /// <summary>Associated process is a child process.</summary>
        private readonly bool _isChild;

        /// <summary>If a wait operation is in progress, the Task that represents it; otherwise, null.</summary>
        private Task _waitInProgress;
        /// <summary>The number of alive users of this object.</summary>
        private int _outstandingRefCount;

        /// <summary>Whether the associated process exited.</summary>
        private bool _exited;
        /// <summary>If the process exited, it's exit code, or null if we were unable to determine one.</summary>
        private int? _exitCode;
        /// <summary>
        /// The approximate time the process exited.  We do not have the ability to know exact time a process
        /// exited, so we approximate it by storing the time that we discovered it exited.
        /// </summary>
        private DateTime _exitTime;
        /// <summary>A lazily-initialized event set when the process exits.</summary>
        private ManualResetEvent _exitedEvent;

        /// <summary>Initialize the wait state object.</summary>
        /// <param name="processId">The associated process' ID.</param>
        private ProcessWaitState(int processId, bool isChild)
        {
            Debug.Assert(processId >= 0);
            _processId = processId;
            _isChild = isChild;
        }

        /// <summary>Releases managed resources used by the ProcessWaitState.</summary>
        public void Dispose()
        {
            Debug.Assert(!Monitor.IsEntered(_gate));

            lock (_gate)
            {
                if (_exitedEvent != null)
                {
                    _exitedEvent.Dispose();
                    _exitedEvent = null;
                }
            }
        }

        /// <summary>Notes that the process has exited.</summary>
        private void SetExited()
        {
            Debug.Assert(Monitor.IsEntered(_gate));

            _exited = true;
            _exitTime = DateTime.Now;
            _exitedEvent?.Set();
        }

        /// <summary>Ensures an exited event has been initialized and returns it.</summary>
        /// <returns></returns>
        internal ManualResetEvent EnsureExitedEvent()
        {
            Debug.Assert(!Monitor.IsEntered(_gate));

            lock (_gate)
            {
                // If we already have an initialized event, just return it.
                if (_exitedEvent == null)
                {
                    // If we don't, create one, and if the process hasn't yet exited,
                    // make sure we have a task that's actively monitoring the completion state.
                    _exitedEvent = new ManualResetEvent(initialState: _exited);
                    if (!_exited)
                    {
                        if (!_isChild)
                        {
                            // If we haven't exited, we need to spin up an asynchronous operation that
                            // will completed the exitedEvent when the other process exits. If there's already
                            // another operation underway, then we'll just tack ours onto the end of it.
                            _waitInProgress = _waitInProgress == null ?
                                WaitForExitAsync() :
                                _waitInProgress.ContinueWith((_, state) => ((ProcessWaitState)state).WaitForExitAsync(),
                                    this, CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.Default).Unwrap();
                        }
                    }
                }
                return _exitedEvent;
            }
        }

        internal DateTime ExitTime
        {
            get
            {
                lock (_gate)
                {
                    Debug.Assert(_exited);
                    return _exitTime;
                }
            }
        }

        internal bool HasExited
        {
            get
            {
                int? ignored;
                return GetExited(out ignored);
            }
        }

        internal bool GetExited(out int? exitCode)
        {
            lock (_gate)
            {
                // Have we already exited?  If so, return the cached results.
                if (_exited)
                {
                    exitCode = _exitCode;
                    return true;
                }

                // Is another wait operation in progress?  If so, then we haven't exited,
                // and that task owns the right to call CheckForNonChildExit.
                if (_waitInProgress != null)
                {
                    exitCode = null;
                    return false;
                }

                // We don't know if we've exited, but no one else is currently
                // checking, so check.
                CheckForNonChildExit();

                // We now have an up-to-date snapshot for whether we've exited,
                // and if we have, what the exit code is (if we were able to find out).
                exitCode = _exitCode;
                return _exited;
            }
        }

        private void CheckForNonChildExit()
        {
            Debug.Assert(Monitor.IsEntered(_gate));
            if (!_isChild)
            {
                // We won't be able to get an exit code, but we'll at least be able to determine if the process is
                // still running.
                int killResult = Interop.Sys.Kill(_processId, Interop.Sys.Signals.None); // None means don't send a signal
                if (killResult == 0)
                {
                    // Process is still running.  This could also be a defunct process that has completed
                    // its work but still has an entry in the processes table due to its parent not yet
                    // having waited on it to clean it up.
                    return;
                }
                else // error from kill
                {
                    Interop.Error errno = Interop.Sys.GetLastError();
                    if (errno == Interop.Error.ESRCH)
                    {
                        // Couldn't find the process; assume it's exited
                        SetExited();
                        return;
                    }
                    else if (errno == Interop.Error.EPERM)
                    {
                        // Don't have permissions to the process; assume it's alive
                        return;
                    }
                    else Debug.Fail("Unexpected errno value from kill");
                }

                SetExited();
            }
        }

        /// <summary>Waits for the associated process to exit.</summary>
        /// <param name="millisecondsTimeout">The amount of time to wait, or -1 to wait indefinitely.</param>
        /// <returns>true if the process exited; false if the timeout occurred.</returns>
        internal bool WaitForExit(int millisecondsTimeout)
        {
            Debug.Assert(!Monitor.IsEntered(_gate));

            if (_isChild)
            {
                lock (_gate)
                {
                    // If we already know that the process exited, we're done.
                    if (_exited)
                    {
                        return true;
                    }
                }
                ManualResetEvent exitEvent = EnsureExitedEvent();
                return exitEvent.WaitOne(millisecondsTimeout);
            }
            else
            {
                // Track the time the we start waiting.
                long startTime = Stopwatch.GetTimestamp();

                // Polling loop
                while (true)
                {
                    bool createdTask = false;
                    CancellationTokenSource cts = null;
                    Task waitTask;

                    // We're in a polling loop... determine how much time remains
                    int remainingTimeout = millisecondsTimeout == Timeout.Infinite ?
                        Timeout.Infinite :
                        (int)Math.Max(millisecondsTimeout - ((Stopwatch.GetTimestamp() - startTime) / (double)Stopwatch.Frequency * 1000), 0);

                    lock (_gate)
                    {
                        // If we already know that the process exited, we're done.
                        if (_exited)
                        {
                            return true;
                        }

                        // If a timeout of 0 was supplied, then we simply need to poll
                        // to see if the process has already exited.
                        if (remainingTimeout == 0)
                        {
                            // If there's currently a wait-in-progress, then we know the other process
                            // hasn't exited (barring races and the polling interval).
                            if (_waitInProgress != null)
                            {
                                return false;
                            }

                            // No one else is checking for the process' exit... so check.
                            // We're currently holding the _gate lock, so we don't want to
                            // allow CheckForNonChildExit to block indefinitely.
                            CheckForNonChildExit();
                            return _exited;
                        }

                        // The process has not yet exited (or at least we don't know it yet)
                        // so we need to wait for it to exit, outside of the lock.
                        // If there's already a wait in progress, we'll do so later
                        // by waiting on that existing task.  Otherwise, we'll spin up
                        // such a task.
                        if (_waitInProgress != null)
                        {
                            waitTask = _waitInProgress;
                        }
                        else
                        {
                            createdTask = true;
                            CancellationToken token = remainingTimeout == Timeout.Infinite ?
                                CancellationToken.None :
                                (cts = new CancellationTokenSource(remainingTimeout)).Token;
                            waitTask = WaitForExitAsync(token);
                        }
                    } // lock(_gate)

                    if (createdTask)
                    {
                        // We created this task, and it'll get canceled automatically after our timeout.
                        // This Wait should only wake up when either the process has exited or the timeout
                        // has expired.  Either way, we'll loop around again; if the process exited, that'll
                        // be caught first thing in the loop where we check _exited, and if it didn't exit,
                        // our remaining time will be zero, so we'll do a quick remaining check and bail.
                        waitTask.Wait();
                        cts?.Dispose();
                    }
                    else
                    {
                        // It's someone else's task.  We'll wait for it to complete. This could complete
                        // either because our remainingTimeout expired or because the task completed,
                        // which could happen because the process exited or because whoever created
                        // that task gave it a timeout.  In any case, we'll loop around again, and the loop
                        // will catch these cases, potentially issuing another wait to make up any
                        // remaining time.
                        waitTask.Wait(remainingTimeout);
                    }
                }
            }
        }

        /// <summary>Spawns an asynchronous polling loop for process completion.</summary>
        /// <param name="cancellationToken">A token to monitor to exit the polling loop.</param>
        /// <returns>The task representing the loop.</returns>
        private Task WaitForExitAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            Debug.Assert(Monitor.IsEntered(_gate));
            Debug.Assert(_waitInProgress == null);
            Debug.Assert(!_isChild);

            return _waitInProgress = Task.Run(async delegate // Task.Run used because of potential blocking in CheckForNonChildExit
            {
                // Arbitrary values chosen to balance delays with polling overhead.  Start with fast polling
                // to handle quickly completing processes, but fall back to longer polling to minimize
                // overhead for those that take longer to complete.
                const int StartingPollingIntervalMs = 1, MaxPollingIntervalMs = 100;
                int pollingIntervalMs = StartingPollingIntervalMs;

                try
                {
                    // While we're not canceled
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        // Poll
                        lock (_gate)
                        {
                            if (!_exited)
                            {
                                CheckForNonChildExit();
                            }
                            if (_exited) // may have been updated by CheckForNonChildExit
                            {
                                return;
                            }
                        }

                        // Wait
                        try
                        {
                            await Task.Delay(pollingIntervalMs, cancellationToken);
                            pollingIntervalMs = Math.Min(pollingIntervalMs * 2, MaxPollingIntervalMs);
                        }
                        catch (OperationCanceledException) { }
                    }
                }
                finally
                {
                    // Task is no longer active
                    lock (_gate)
                    {
                        _waitInProgress = null;
                    }
                }
            });
        }

        private bool TryReapChild()
        {
            lock (_gate)
            {
                if (_exited)
                {
                    return false;
                }

                // Try to get the state of the child process
                int exitCode;
                int waitResult = Interop.Sys.WaitIdExitedNoHang(_processId, out exitCode, keepWaitable: false);

                if (waitResult == _processId)
                {
                    _exitCode = exitCode;

                    SetExited();
                    return true;
                }
                else if (waitResult == 0)
                {
                    // Process is still running
                }
                else
                {
                    // Unexpected.
                    int errorCode = Marshal.GetLastWin32Error();
                    Environment.FailFast("Error while reaping child. errno = " + errorCode);
                }
                return false;
            }
        }

        internal static void CheckChildren(bool reapAll)
        {
            // This is called on SIGCHLD from a native thread.
            // A lock in Process ensures no new processes are spawned while we are checking.
            lock (s_childProcessWaitStates)
            {
                bool checkAll = false;

                // Check terminated processes.
                int pid;
                do
                {
                    // Find a process that terminated without reaping it yet.
                    int exitCode;
                    pid = Interop.Sys.WaitIdExitedNoHang(-1, out exitCode, keepWaitable: true);
                    if (pid > 0)
                    {
                        if (s_childProcessWaitStates.TryGetValue(pid, out ProcessWaitState pws))
                        {
                            // Known Process.
                            if (pws.TryReapChild())
                            {
                                pws.ReleaseRef();
                            }
                        }
                        else
                        {
                            // unlikely: This is not a managed Process, so we are not responsible for reaping.
                            // Fall back to checking all Processes.
                            checkAll = true;
                            break;
                        }
                    }
                    else if (pid == 0)
                    {
                        // No more terminated children.
                    }
                    else
                    {
                        // Unexpected.
                        int errorCode = Marshal.GetLastWin32Error();
                        Environment.FailFast("Error while checking for terminated children. errno = " + errorCode);
                    }
                } while (pid > 0);

                if (checkAll)
                {
                    // We track things to unref so we don't invalidate our iterator by changing s_childProcessWaitStates. 
                    ProcessWaitState firstToRemove = null;
                    List<ProcessWaitState> additionalToRemove = null;
                    foreach (KeyValuePair<int, ProcessWaitState> kv in s_childProcessWaitStates)
                    {
                        ProcessWaitState pws = kv.Value;
                        if (pws.TryReapChild())
                        {
                            if (firstToRemove == null)
                            {
                                firstToRemove = pws;
                            }
                            else
                            {
                                if (additionalToRemove == null)
                                {
                                    additionalToRemove = new List<ProcessWaitState>();
                                }
                                additionalToRemove.Add(pws);
                            }
                        }
                    }

                    if (firstToRemove != null)
                    {
                        firstToRemove.ReleaseRef();
                        if (additionalToRemove != null)
                        {
                            foreach (ProcessWaitState pws in additionalToRemove)
                            {
                                pws.ReleaseRef();
                            }
                        }
                    }
                }

                if (reapAll)
                {
                    do
                    {
                        int exitCode;
                        pid = Interop.Sys.WaitIdExitedNoHang(-1, out exitCode, keepWaitable: false);
                    } while (pid > 0);
                }
            }
        }
    }
}
