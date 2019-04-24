// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Microsoft.Win32.SafeHandles;

namespace System.Diagnostics
{
    public partial class Process : IDisposable
    {
        private bool _haveMainWindow;
        private IntPtr _mainWindowHandle;
        private string _mainWindowTitle;

        private bool _haveResponding;
        private bool _responding;

        private bool StartCore(ProcessStartInfo startInfo)
        {
            return startInfo.UseShellExecute
                ? StartWithShellExecuteEx(startInfo)
                : StartWithCreateProcess(startInfo);
        }

        private unsafe bool StartWithShellExecuteEx(ProcessStartInfo startInfo)
        {
            if (!string.IsNullOrEmpty(startInfo.UserName) || startInfo.Password != null)
                throw new InvalidOperationException(SR.CantStartAsUser);

            if (startInfo.RedirectStandardInput || startInfo.RedirectStandardOutput || startInfo.RedirectStandardError)
                throw new InvalidOperationException(SR.CantRedirectStreams);

            if (startInfo.StandardInputEncoding != null)
                throw new InvalidOperationException(SR.StandardInputEncodingNotAllowed);

            if (startInfo.StandardErrorEncoding != null)
                throw new InvalidOperationException(SR.StandardErrorEncodingNotAllowed);

            if (startInfo.StandardOutputEncoding != null)
                throw new InvalidOperationException(SR.StandardOutputEncodingNotAllowed);

            if (startInfo._environmentVariables != null)
                throw new InvalidOperationException(SR.CantUseEnvVars);

            string arguments;
            if (startInfo.ArgumentList.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                Process.AppendArguments(sb, startInfo.ArgumentList);
                arguments = sb.ToString();
            }
            else
            {
                arguments = startInfo.Arguments;
            }

            fixed (char* fileName = startInfo.FileName.Length > 0 ? startInfo.FileName : null)
            fixed (char* verb = startInfo.Verb.Length > 0 ? startInfo.Verb : null)
            fixed (char* parameters = arguments.Length > 0 ? arguments : null)
            fixed (char* directory = startInfo.WorkingDirectory.Length > 0 ? startInfo.WorkingDirectory : null)
            {
                Interop.Shell32.SHELLEXECUTEINFO shellExecuteInfo = new Interop.Shell32.SHELLEXECUTEINFO()
                {
                    cbSize = (uint)sizeof(Interop.Shell32.SHELLEXECUTEINFO),
                    lpFile = fileName,
                    lpVerb = verb,
                    lpParameters = parameters,
                    lpDirectory = directory,
                    fMask = Interop.Shell32.SEE_MASK_NOCLOSEPROCESS | Interop.Shell32.SEE_MASK_FLAG_DDEWAIT
                };

                if (startInfo.ErrorDialog)
                    shellExecuteInfo.hwnd = startInfo.ErrorDialogParentHandle;
                else
                    shellExecuteInfo.fMask |= Interop.Shell32.SEE_MASK_FLAG_NO_UI;

                switch (startInfo.WindowStyle)
                {
                    case ProcessWindowStyle.Hidden:
                        shellExecuteInfo.nShow = Interop.Shell32.SW_HIDE;
                        break;
                    case ProcessWindowStyle.Minimized:
                        shellExecuteInfo.nShow = Interop.Shell32.SW_SHOWMINIMIZED;
                        break;
                    case ProcessWindowStyle.Maximized:
                        shellExecuteInfo.nShow = Interop.Shell32.SW_SHOWMAXIMIZED;
                        break;
                    default:
                        shellExecuteInfo.nShow = Interop.Shell32.SW_SHOWNORMAL;
                        break;
                }

                ShellExecuteHelper executeHelper = new ShellExecuteHelper(&shellExecuteInfo);
                if (!executeHelper.ShellExecuteOnSTAThread())
                {
                    int error = executeHelper.ErrorCode;
                    if (error == 0)
                    {
                        error = GetShellError(shellExecuteInfo.hInstApp);
                    }

                    switch (error)
                    {
                        case Interop.Errors.ERROR_BAD_EXE_FORMAT:
                        case Interop.Errors.ERROR_EXE_MACHINE_TYPE_MISMATCH:
                            throw new Win32Exception(error, SR.InvalidApplication);
                        case Interop.Errors.ERROR_CALL_NOT_IMPLEMENTED:
                            // This happens on Windows Nano
                            throw new PlatformNotSupportedException(SR.UseShellExecuteNotSupported);
                        default:
                            throw new Win32Exception(error);
                    }
                }

                if (shellExecuteInfo.hProcess != IntPtr.Zero)
                {
                    SetProcessHandle(new SafeProcessHandle(shellExecuteInfo.hProcess));
                    return true;
                }
            }

            return false;
        }

        private int GetShellError(IntPtr error)
        {
            switch ((long)error)
            {
                case Interop.Shell32.SE_ERR_FNF:
                    return Interop.Errors.ERROR_FILE_NOT_FOUND;
                case Interop.Shell32.SE_ERR_PNF:
                    return Interop.Errors.ERROR_PATH_NOT_FOUND;
                case Interop.Shell32.SE_ERR_ACCESSDENIED:
                    return Interop.Errors.ERROR_ACCESS_DENIED;
                case Interop.Shell32.SE_ERR_OOM:
                    return Interop.Errors.ERROR_NOT_ENOUGH_MEMORY;
                case Interop.Shell32.SE_ERR_DDEFAIL:
                case Interop.Shell32.SE_ERR_DDEBUSY:
                case Interop.Shell32.SE_ERR_DDETIMEOUT:
                    return Interop.Errors.ERROR_DDE_FAIL;
                case Interop.Shell32.SE_ERR_SHARE:
                    return Interop.Errors.ERROR_SHARING_VIOLATION;
                case Interop.Shell32.SE_ERR_NOASSOC:
                    return Interop.Errors.ERROR_NO_ASSOCIATION;
                case Interop.Shell32.SE_ERR_DLLNOTFOUND:
                    return Interop.Errors.ERROR_DLL_NOT_FOUND;
                default:
                    return (int)(long)error;
            }
        }

        internal unsafe class ShellExecuteHelper
        {
            private Interop.Shell32.SHELLEXECUTEINFO* _executeInfo;
            private bool _succeeded;
            private bool _notpresent;

            public ShellExecuteHelper(Interop.Shell32.SHELLEXECUTEINFO* executeInfo)
            {
                _executeInfo = executeInfo;
            }

            private void ShellExecuteFunction()
            {
                try
                {
                    if (!(_succeeded = Interop.Shell32.ShellExecuteExW(_executeInfo)))
                        ErrorCode = Marshal.GetLastWin32Error();
                }
                catch (EntryPointNotFoundException)
                {
                    _notpresent = true;
                }
            }

            public bool ShellExecuteOnSTAThread()
            {
                // ShellExecute() requires STA in order to work correctly.

                if (Thread.CurrentThread.GetApartmentState() != ApartmentState.STA)
                {
                    ThreadStart threadStart = new ThreadStart(ShellExecuteFunction);
                    Thread executionThread = new Thread(threadStart);
                    executionThread.SetApartmentState(ApartmentState.STA);
                    executionThread.Start();
                    executionThread.Join();
                }
                else
                {
                    ShellExecuteFunction();
                }

                if (_notpresent)
                    throw new PlatformNotSupportedException(SR.UseShellExecuteNotSupported);

                return _succeeded;
            }

            public int ErrorCode { get; private set; }
        }

        private string GetMainWindowTitle()
        {
            IntPtr handle = MainWindowHandle;
            if (handle == IntPtr.Zero)
                return string.Empty;

            int length = Interop.User32.GetWindowTextLengthW(handle);

            if (length == 0)
            {
#if DEBUG
                // We never used to throw here, want to surface possible mistakes on our part
                int error = Marshal.GetLastWin32Error();
                Debug.Assert(error == 0, $"Failed GetWindowTextLengthW(): { new Win32Exception(error).Message }");
#endif
                return string.Empty;
            }

            length++; // for null terminator, which GetWindowTextLengthW does not include in the length
            Span<char> title = length <= 256 ? stackalloc char[256] : new char[length];
            unsafe
            {
                fixed (char* titlePtr = title)
                {
                    length = Interop.User32.GetWindowTextW(handle, titlePtr, title.Length); // returned length does not include null terminator
                }
            }
#if DEBUG
            if (length == 0)
            {
                // We never used to throw here, want to surface possible mistakes on our part
                int error = Marshal.GetLastWin32Error();
                Debug.Assert(error == 0, $"Failed GetWindowTextW(): { new Win32Exception(error).Message }");
            }
#endif
            return title.Slice(0, length).ToString();
        }

        public IntPtr MainWindowHandle
        {
            get
            {
                if (!_haveMainWindow)
                {
                    EnsureState(State.IsLocal | State.HaveId);
                    _mainWindowHandle = ProcessManager.GetMainWindowHandle(_processId);

                    _haveMainWindow = true;
                }
                return _mainWindowHandle;
            }
        }

        private bool CloseMainWindowCore()
        {
            const int GWL_STYLE = -16; // Retrieves the window styles.
            const int WS_DISABLED = 0x08000000; // WindowStyle disabled. A disabled window cannot receive input from the user.
            const int WM_CLOSE = 0x0010; // WindowMessage close.

            IntPtr mainWindowHandle = MainWindowHandle;
            if (mainWindowHandle == (IntPtr)0)
            {
                return false;
            }

            int style = Interop.User32.GetWindowLong(mainWindowHandle, GWL_STYLE);
            if ((style & WS_DISABLED) != 0)
            {
                return false;
            }

            Interop.User32.PostMessageW(mainWindowHandle, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
            return true;
        }

        public string MainWindowTitle
        {
            get
            {
                if (_mainWindowTitle == null)
                {
                    _mainWindowTitle = GetMainWindowTitle();
                }

                return _mainWindowTitle;
            }
        }

        private bool IsRespondingCore()
        {
            const int WM_NULL = 0x0000;
            const int SMTO_ABORTIFHUNG = 0x0002;

            IntPtr mainWindow = MainWindowHandle;
            if (mainWindow == (IntPtr)0)
            {
                return true;
            }

            IntPtr result;
            return Interop.User32.SendMessageTimeout(mainWindow, WM_NULL, IntPtr.Zero, IntPtr.Zero, SMTO_ABORTIFHUNG, 5000, out result) != (IntPtr)0;
        }

        public bool Responding
        {
            get
            {
                if (!_haveResponding)
                {
                    _responding = IsRespondingCore();
                    _haveResponding = true;
                }

                return _responding;
            }
        }

        private bool WaitForInputIdleCore(int milliseconds)
        {
            const int WAIT_OBJECT_0 = 0x00000000;
            const int WAIT_FAILED = unchecked((int)0xFFFFFFFF);
            const int WAIT_TIMEOUT = 0x00000102;

            bool idle;
            using (SafeProcessHandle handle = GetProcessHandle(Interop.Advapi32.ProcessOptions.SYNCHRONIZE | Interop.Advapi32.ProcessOptions.PROCESS_QUERY_INFORMATION))
            {
                int ret = Interop.User32.WaitForInputIdle(handle, milliseconds);
                switch (ret)
                {
                    case WAIT_OBJECT_0:
                        idle = true;
                        break;
                    case WAIT_TIMEOUT:
                        idle = false;
                        break;
                    case WAIT_FAILED:
                    default:
                        throw new InvalidOperationException(SR.InputIdleUnkownError);
                }
            }
            return idle;
        }

        /// <summary>Checks whether the argument is a direct child of this process.</summary>
        /// <remarks>
        /// A child process is a process which has this process's id as its parent process id and which started after this process did.
        /// </remarks>
        private bool IsParentOf(Process possibleChild) =>
            StartTime < possibleChild.StartTime
            && Id == possibleChild.ParentProcessId;

        /// <summary>
        /// Get the process's parent process id.
        /// </summary>
        private unsafe int ParentProcessId
        {
            get
            {
                Interop.NtDll.PROCESS_BASIC_INFORMATION info = default;

                using (SafeProcessHandle handle = GetProcessHandle(Interop.Advapi32.ProcessOptions.PROCESS_QUERY_INFORMATION))
                {
                    if (Interop.NtDll.NtQueryInformationProcess(handle, Interop.NtDll.PROCESSINFOCLASS.ProcessBasicInformation, &info, (uint)sizeof(Interop.NtDll.PROCESS_BASIC_INFORMATION), out _) != 0)
                        throw new Win32Exception(SR.ProcessInformationUnavailable);

                    return (int)info.InheritedFromUniqueProcessId;
                }
            }
        }

        private bool Equals(Process process) =>
            Id == process.Id
            && StartTime == process.StartTime;

        private IEnumerable<Exception> KillTree()
        {
            // The process's structures will be preserved as long as a handle is held pointing to them, even if the process exits or 
            // is terminated. A handle is held here to ensure a stable reference to the process during execution.
            using (SafeProcessHandle handle = GetProcessHandle(Interop.Advapi32.ProcessOptions.PROCESS_QUERY_LIMITED_INFORMATION, throwIfExited: false))
            {
                // If the process has exited, the handle is invalid.
                if (handle.IsInvalid)
                    return Enumerable.Empty<Exception>();

                return KillTree(handle);
            }
        }

        private IEnumerable<Exception> KillTree(SafeProcessHandle handle)
        {
            Debug.Assert(!handle.IsInvalid);

            List<Exception> exceptions = new List<Exception>();

            try
            {
                // Kill the process, so that no further children can be created.
                //
                // This method can return before stopping has completed. Down the road, could possibly wait for termination to complete before continuing.
                Kill();
            }
            catch (Win32Exception e)
            {
                exceptions.Add(e);
            }

            IReadOnlyList<(Process Process, SafeProcessHandle Handle)> children = GetProcessHandlePairs(p => SafePredicateTest(() => IsParentOf(p)));
            try
            {
                foreach ((Process Process, SafeProcessHandle Handle) child in children)
                {
                    IEnumerable<Exception> exceptionsFromChild = child.Process.KillTree(child.Handle);
                    exceptions.AddRange(exceptionsFromChild);
                }
            }
            finally
            {
                foreach ((Process Process, SafeProcessHandle Handle) child in children)
                {
                    child.Process.Dispose();
                    child.Handle.Dispose();
                }
            }

            return exceptions;
        }

        private IReadOnlyList<(Process Process, SafeProcessHandle Handle)> GetProcessHandlePairs(Func<Process, bool> predicate)
        {
            return GetProcesses()
                .Select(p => (Process: p, Handle: SafeGetHandle(p)))
                .Where(p => !p.Handle.IsInvalid && predicate(p.Process))
                .ToList();

            SafeProcessHandle SafeGetHandle(Process process)
            {
                try
                {
                    return process.GetProcessHandle(Interop.Advapi32.ProcessOptions.PROCESS_QUERY_LIMITED_INFORMATION, false);
                }
                catch (Win32Exception)
                {
                    return SafeProcessHandle.InvalidHandle;
                }
            }
        }
    }
}
