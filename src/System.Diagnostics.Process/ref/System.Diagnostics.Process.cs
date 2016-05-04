// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------


namespace Microsoft.Win32.SafeHandles
{
    /// <summary>
    /// Provides a managed wrapper for a process handle.
    /// </summary>
    public sealed partial class SafeProcessHandle : System.Runtime.InteropServices.SafeHandle
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SafeProcessHandle" />
        /// class from the specified handle, indicating whether to release the handle during the finalization
        /// phase.
        /// </summary>
        /// <param name="existingHandle">The handle to be wrapped.</param>
        /// <param name="ownsHandle">
        /// true to reliably let <see cref="SafeProcessHandle" /> release
        /// the handle during the finalization phase; otherwise, false.
        /// </param>
        public SafeProcessHandle(System.IntPtr existingHandle, bool ownsHandle) : base(default(System.IntPtr), default(bool)) { }
        protected override bool ReleaseHandle() { return default(bool); }
    }
}
namespace System.Diagnostics
{
    /// <summary>
    /// Provides data for the <see cref="Process.OutputDataReceived" /> and
    /// <see cref="Process.ErrorDataReceived" /> events.
    /// </summary>
    public partial class DataReceivedEventArgs : System.EventArgs
    {
        internal DataReceivedEventArgs() { }
        /// <summary>
        /// Gets the line of characters that was written to a redirected <see cref="Process" />
        /// output stream.
        /// </summary>
        /// <returns>
        /// The line that was written by an associated <see cref="Process" /> to
        /// its redirected <see cref="StandardOutput" /> or
        /// <see cref="Process.StandardError" /> stream.
        /// </returns>
        public string Data { get { return default(string); } }
    }
    /// <summary>
    /// Represents the method that will handle the <see cref="Process.OutputDataReceived" />
    /// event or <see cref="Process.ErrorDataReceived" /> event of a
    /// <see cref="Process" />.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="DataReceivedEventArgs" /> that contains the event data.</param>
    public delegate void DataReceivedEventHandler(object sender, System.Diagnostics.DataReceivedEventArgs e);
    /// <summary>
    /// Provides access to local and remote processes and enables you to start and stop local system
    /// processes.
    /// </summary>
    public partial class Process
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Process" /> class.
        /// </summary>
        public Process() { }
        /// <summary>
        /// Gets the base priority of the associated process.
        /// </summary>
        /// <returns>
        /// The base priority, which is computed from the <see cref="PriorityClass" />
        /// of the associated process.
        /// </returns>
        /// <exception cref="PlatformNotSupportedException">
        /// The platform is Windows 98 or Windows Millennium Edition (Windows Me); set the
        /// <see cref="ProcessStartInfo.UseShellExecute" /> property to false to access this property on Windows 98 and Windows Me.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The process has exited.-or- The process has not started, so there is no process ID.
        /// </exception>
        public int BasePriority { get { return default(int); } }
        /// <summary>
        /// Gets or sets whether the <see cref="Exited" /> event should be
        /// raised when the process terminates.
        /// </summary>
        /// <returns>
        /// true if the <see cref="Exited" /> event should be raised when
        /// the associated process is terminated (through either an exit or a call to
        /// <see cref="Kill" />); otherwise, false. The default is false.
        /// </returns>
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool EnableRaisingEvents { get { return default(bool); } set { } }
        /// <summary>
        /// Gets the value that the associated process specified when it terminated.
        /// </summary>
        /// <returns>
        /// The code that the associated process specified when it terminated.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// The process has not exited.-or- The process <see cref="Process.Handle" />
        /// is not valid.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// You are trying to access the <see cref="ExitCode" /> property
        /// for a process that is running on a remote computer. This property is available only for processes
        /// that are running on the local computer.
        /// </exception>
        public int ExitCode { get { return default(int); } }
        /// <summary>
        /// Gets the time that the associated process exited.
        /// </summary>
        /// <returns>
        /// A <see cref="DateTime" /> that indicates when the associated process was terminated.
        /// </returns>
        /// <exception cref="PlatformNotSupportedException">
        /// The platform is Windows 98 or Windows Millennium Edition (Windows Me), which does not support
        /// this property.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// You are trying to access the <see cref="ExitTime" /> property
        /// for a process that is running on a remote computer. This property is available only for processes
        /// that are running on the local computer.
        /// </exception>
        public System.DateTime ExitTime { get { return default(System.DateTime); } }
        /// <summary>
        /// Gets a value indicating whether the associated process has been terminated.
        /// </summary>
        /// <returns>
        /// true if the operating system process referenced by the <see cref="Process" />
        /// component has terminated; otherwise, false.
        /// </returns>
        /// <exception cref="InvalidOperationException">There is no process associated with the object.</exception>
        /// <exception cref="System.ComponentModel.Win32Exception">
        /// The exit code for the process could not be retrieved.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// You are trying to access the <see cref="HasExited" /> property
        /// for a process that is running on a remote computer. This property is available only for processes
        /// that are running on the local computer.
        /// </exception>
        public bool HasExited { get { return default(bool); } }
        /// <summary>
        /// Gets the unique identifier for the associated process.
        /// </summary>
        /// <returns>
        /// The system-generated unique identifier of the process that is referenced by this
        /// <see cref="Process" /> instance.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// The process's <see cref="Id" /> property has not been set.-or-
        /// There is no process associated with this <see cref="Process" /> object.
        /// </exception>
        /// <exception cref="PlatformNotSupportedException">
        /// The platform is Windows 98 or Windows Millennium Edition (Windows Me); set the
        /// <see cref="ProcessStartInfo.UseShellExecute" /> property to false to access this property on Windows 98 and Windows Me.
        /// </exception>
        public int Id { get { return default(int); } }
        /// <summary>
        /// Gets the name of the computer the associated process is running on.
        /// </summary>
        /// <returns>
        /// The name of the computer that the associated process is running on.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// There is no process associated with this <see cref="Process" /> object.
        /// </exception>
        public string MachineName { get { return default(string); } }
        /// <summary>
        /// Gets the main module for the associated process.
        /// </summary>
        /// <returns>
        /// The <see cref="ProcessModule" /> that was used to start the process.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// You are trying to access the <see cref="MainModule" /> property
        /// for a process that is running on a remote computer. This property is available only for processes
        /// that are running on the local computer.
        /// </exception>
        /// <exception cref="System.ComponentModel.Win32Exception">
        /// A 32-bit process is trying to access the modules of a 64-bit process.
        /// </exception>
        /// <exception cref="PlatformNotSupportedException">
        /// The platform is Windows 98 or Windows Millennium Edition (Windows Me); set
        /// <see cref="ProcessStartInfo.UseShellExecute" /> to false to access this property on Windows 98 and Windows Me.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The process <see cref="Id" /> is not available.-or- The process
        /// has exited.
        /// </exception>
        public System.Diagnostics.ProcessModule MainModule { get { return default(System.Diagnostics.ProcessModule); } }
        /// <summary>
        /// Gets or sets the maximum allowable working set size, in bytes, for the associated process.
        /// </summary>
        /// <returns>
        /// The maximum working set size that is allowed in memory for the process, in bytes.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// The maximum working set size is invalid. It must be greater than or equal to the minimum working
        /// set size.
        /// </exception>
        /// <exception cref="System.ComponentModel.Win32Exception">
        /// Working set information cannot be retrieved from the associated process resource.-or- The process
        /// identifier or process handle is zero because the process has not been started.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// You are trying to access the <see cref="MaxWorkingSet" /> property
        /// for a process that is running on a remote computer. This property is available only for processes
        /// that are running on the local computer.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The process <see cref="Id" /> is not available.-or- The process
        /// has exited.
        /// </exception>
        /// <exception cref="PlatformNotSupportedException">
        /// The platform is Windows 98 or Windows Millennium Edition (Windows Me), which does not support
        /// this property.
        /// </exception>
        public System.IntPtr MaxWorkingSet { get { return default(System.IntPtr); } set { } }
        /// <summary>
        /// Gets or sets the minimum allowable working set size, in bytes, for the associated process.
        /// </summary>
        /// <returns>
        /// The minimum working set size that is required in memory for the process, in bytes.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// The minimum working set size is invalid. It must be less than or equal to the maximum working
        /// set size.
        /// </exception>
        /// <exception cref="System.ComponentModel.Win32Exception">
        /// Working set information cannot be retrieved from the associated process resource.-or- The process
        /// identifier or process handle is zero because the process has not been started.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// You are trying to access the <see cref="MinWorkingSet" /> property
        /// for a process that is running on a remote computer. This property is available only for processes
        /// that are running on the local computer.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The process <see cref="Id" /> is not available.-or- The process
        /// has exited.
        /// </exception>
        /// <exception cref="PlatformNotSupportedException">
        /// The platform is Windows 98 or Windows Millennium Edition (Windows Me), which does not support
        /// this property.
        /// </exception>
        public System.IntPtr MinWorkingSet { get { return default(System.IntPtr); } set { } }
        /// <summary>
        /// Gets the modules that have been loaded by the associated process.
        /// </summary>
        /// <returns>
        /// An array of type <see cref="ProcessModule" /> that represents the modules
        /// that have been loaded by the associated process.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// You are attempting to access the <see cref="Modules" /> property
        /// for a process that is running on a remote computer. This property is available only for processes
        /// that are running on the local computer.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The process <see cref="Id" /> is not available.
        /// </exception>
        /// <exception cref="PlatformNotSupportedException">
        /// The platform is Windows 98 or Windows Millennium Edition (Windows Me); set
        /// <see cref="ProcessStartInfo.UseShellExecute" /> to false to access this property on Windows 98 and Windows Me.
        /// </exception>
        /// <exception cref="System.ComponentModel.Win32Exception">
        /// You are attempting to access the <see cref="Modules" /> property
        /// for either the system process or the idle process. These processes do not have modules.
        /// </exception>
        public System.Diagnostics.ProcessModuleCollection Modules { get { return default(System.Diagnostics.ProcessModuleCollection); } }
        /// <summary>
        /// Gets the amount of nonpaged system memory, in bytes, allocated for the associated process.
        /// </summary>
        /// <returns>
        /// The amount of system memory, in bytes, allocated for the associated process that cannot be
        /// written to the virtual memory paging file.
        /// </returns>
        /// <exception cref="PlatformNotSupportedException">
        /// The platform is Windows 98 or Windows Millennium Edition (Windows Me), which does not support
        /// this property.
        /// </exception>
        public long NonpagedSystemMemorySize64 { get { return default(long); } }
        /// <summary>
        /// Gets the amount of paged memory, in bytes, allocated for the associated process.
        /// </summary>
        /// <returns>
        /// The amount of memory, in bytes, allocated in the virtual memory paging file for the associated
        /// process.
        /// </returns>
        /// <exception cref="PlatformNotSupportedException">
        /// The platform is Windows 98 or Windows Millennium Edition (Windows Me), which does not support
        /// this property.
        /// </exception>
        public long PagedMemorySize64 { get { return default(long); } }
        /// <summary>
        /// Gets the amount of pageable system memory, in bytes, allocated for the associated process.
        /// </summary>
        /// <returns>
        /// The amount of system memory, in bytes, allocated for the associated process that can be written
        /// to the virtual memory paging file.
        /// </returns>
        /// <exception cref="PlatformNotSupportedException">
        /// The platform is Windows 98 or Windows Millennium Edition (Windows Me), which does not support
        /// this property.
        /// </exception>
        public long PagedSystemMemorySize64 { get { return default(long); } }
        /// <summary>
        /// Gets the maximum amount of memory in the virtual memory paging file, in bytes, used by the
        /// associated process.
        /// </summary>
        /// <returns>
        /// The maximum amount of memory, in bytes, allocated in the virtual memory paging file for the
        /// associated process since it was started.
        /// </returns>
        /// <exception cref="PlatformNotSupportedException">
        /// The platform is Windows 98 or Windows Millennium Edition (Windows Me), which does not support
        /// this property.
        /// </exception>
        public long PeakPagedMemorySize64 { get { return default(long); } }
        /// <summary>
        /// Gets the maximum amount of virtual memory, in bytes, used by the associated process.
        /// </summary>
        /// <returns>
        /// The maximum amount of virtual memory, in bytes, allocated for the associated process since
        /// it was started.
        /// </returns>
        /// <exception cref="PlatformNotSupportedException">
        /// The platform is Windows 98 or Windows Millennium Edition (Windows Me), which does not support
        /// this property.
        /// </exception>
        public long PeakVirtualMemorySize64 { get { return default(long); } }
        /// <summary>
        /// Gets the maximum amount of physical memory, in bytes, used by the associated process.
        /// </summary>
        /// <returns>
        /// The maximum amount of physical memory, in bytes, allocated for the associated process since
        /// it was started.
        /// </returns>
        /// <exception cref="PlatformNotSupportedException">
        /// The platform is Windows 98 or Windows Millennium Edition (Windows Me), which does not support
        /// this property.
        /// </exception>
        public long PeakWorkingSet64 { get { return default(long); } }
        /// <summary>
        /// Gets or sets a value indicating whether the associated process priority should temporarily
        /// be boosted by the operating system when the main window has the focus.
        /// </summary>
        /// <returns>
        /// true if dynamic boosting of the process priority should take place for a process when it is
        /// taken out of the wait state; otherwise, false. The default is false.
        /// </returns>
        /// <exception cref="System.ComponentModel.Win32Exception">
        /// Priority boost information could not be retrieved from the associated process resource.
        /// </exception>
        /// <exception cref="PlatformNotSupportedException">
        /// The platform is Windows 98 or Windows Millennium Edition (Windows Me), which does not support
        /// this property.-or- The process identifier or process handle is zero. (The process has not been started.)
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// You are attempting to access the <see cref="PriorityBoostEnabled" />
        /// property for a process that is running on a remote computer. This property is available
        /// only for processes that are running on the local computer.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The process <see cref="Id" /> is not available.
        /// </exception>
        public bool PriorityBoostEnabled { get { return default(bool); } set { } }
        /// <summary>
        /// Gets or sets the overall priority category for the associated process.
        /// </summary>
        /// <returns>
        /// The priority category for the associated process, from which the
        /// <see cref="BasePriority" /> of the process is calculated.
        /// </returns>
        /// <exception cref="System.ComponentModel.Win32Exception">
        /// Process priority information could not be set or retrieved from the associated process resource.-or-
        /// The process identifier or process handle is zero. (The process has not been started.)
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// You are attempting to access the <see cref="PriorityClass" />
        /// property for a process that is running on a remote computer. This property is available only
        /// for processes that are running on the local computer.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The process <see cref="Id" /> is not available.
        /// </exception>
        /// <exception cref="PlatformNotSupportedException">
        /// You have set the <see cref="PriorityClass" /> to AboveNormal
        /// or BelowNormal when using Windows 98 or Windows Millennium Edition (Windows Me). These platforms
        /// do not support those values for the priority class.
        /// </exception>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">
        /// Priority class cannot be set because it does not use a valid value, as defined in the
        /// <see cref="ProcessPriorityClass" /> enumeration.
        /// </exception>
        public System.Diagnostics.ProcessPriorityClass PriorityClass { get { return default(System.Diagnostics.ProcessPriorityClass); } set { } }
        /// <summary>
        /// Gets the amount of private memory, in bytes, allocated for the associated process.
        /// </summary>
        /// <returns>
        /// The amount of memory, in bytes, allocated for the associated process that cannot be shared
        /// with other processes.
        /// </returns>
        /// <exception cref="PlatformNotSupportedException">
        /// The platform is Windows 98 or Windows Millennium Edition (Windows Me), which does not support
        /// this property.
        /// </exception>
        public long PrivateMemorySize64 { get { return default(long); } }
        /// <summary>
        /// Gets the privileged processor time for this process.
        /// </summary>
        /// <returns>
        /// A <see cref="TimeSpan" /> that indicates the amount of time that the process has
        /// spent running code inside the operating system core.
        /// </returns>
        /// <exception cref="PlatformNotSupportedException">
        /// The platform is Windows 98 or Windows Millennium Edition (Windows Me), which does not support
        /// this property.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// You are attempting to access the <see cref="PrivilegedProcessorTime" />
        /// property for a process that is running on a remote computer. This property is available
        /// only for processes that are running on the local computer.
        /// </exception>
        public System.TimeSpan PrivilegedProcessorTime { get { return default(System.TimeSpan); } }
        /// <summary>
        /// Gets the name of the process.
        /// </summary>
        /// <returns>
        /// The name that the system uses to identify the process to the user.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// The process does not have an identifier, or no process is associated with the
        /// <see cref="Process" />.-or- The associated process has exited.
        /// </exception>
        /// <exception cref="PlatformNotSupportedException">
        /// The platform is Windows 98 or Windows Millennium Edition (Windows Me); set
        /// <see cref="ProcessStartInfo.UseShellExecute" /> to false to access this property on Windows 98 and Windows Me.
        /// </exception>
        /// <exception cref="NotSupportedException">The process is not on this computer.</exception>
        public string ProcessName { get { return default(string); } }
        /// <summary>
        /// Gets or sets the processors on which the threads in this process can be scheduled to run.
        /// </summary>
        /// <returns>
        /// A bitmask representing the processors that the threads in the associated process can run on.
        /// The default depends on the number of processors on the computer. The default value is 2 n -1, where
        /// n is the number of processors.
        /// </returns>
        /// <exception cref="System.ComponentModel.Win32Exception">
        /// <see cref="ProcessorAffinity" /> information could not be set
        /// or retrieved from the associated process resource.-or- The process identifier or process handle
        /// is zero. (The process has not been started.)
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// You are attempting to access the <see cref="ProcessorAffinity" />
        /// property for a process that is running on a remote computer. This property is available
        /// only for processes that are running on the local computer.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The process <see cref="Id" /> was not available.-or- The process
        /// has exited.
        /// </exception>
        public System.IntPtr ProcessorAffinity { get { return default(System.IntPtr); } set { } }
        /// <summary>
        /// Gets the native handle to this process.
        /// </summary>
        /// <returns>
        /// The native handle to this process.
        /// </returns>
        public Microsoft.Win32.SafeHandles.SafeProcessHandle SafeHandle { get { return default(Microsoft.Win32.SafeHandles.SafeProcessHandle); } }
        /// <summary>
        /// Gets the Terminal Services session identifier for the associated process.
        /// </summary>
        /// <returns>
        /// The Terminal Services session identifier for the associated process.
        /// </returns>
        /// <exception cref="NullReferenceException">There is no session associated with this process.</exception>
        /// <exception cref="InvalidOperationException">
        /// There is no process associated with this session identifier.-or-The associated process is not
        /// on this machine.
        /// </exception>
        /// <exception cref="PlatformNotSupportedException">
        /// The <see cref="SessionId" /> property is not supported on Windows
        /// 98.
        /// </exception>
        public int SessionId { get { return default(int); } }
        /// <summary>
        /// Gets a stream used to read the error output of the application.
        /// </summary>
        /// <returns>
        /// A <see cref="IO.StreamReader" /> that can be used to read the standard error stream
        /// of the application.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// The <see cref="StandardError" /> stream has not been defined
        /// for redirection; ensure <see cref="ProcessStartInfo.RedirectStandardError" />
        /// is set to true and <see cref="ProcessStartInfo.UseShellExecute" />
        /// is set to false.- or - The <see cref="StandardError" /> stream
        /// has been opened for asynchronous read operations with <see cref="BeginErrorReadLine" />.
        /// </exception>
        public System.IO.StreamReader StandardError { get { return default(System.IO.StreamReader); } }
        /// <summary>
        /// Gets a stream used to write the input of the application.
        /// </summary>
        /// <returns>
        /// A <see cref="IO.StreamWriter" /> that can be used to write the standard input stream
        /// of the application.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// The <see cref="StandardInput" /> stream has not been defined
        /// because <see cref="ProcessStartInfo.RedirectStandardInput" /> is set
        /// to false.
        /// </exception>
        public System.IO.StreamWriter StandardInput { get { return default(System.IO.StreamWriter); } }
        /// <summary>
        /// Gets a stream used to read the textual output of the application.
        /// </summary>
        /// <returns>
        /// A <see cref="IO.StreamReader" /> that can be used to read the standard output stream
        /// of the application.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// The <see cref="StandardOutput" /> stream has not been defined
        /// for redirection; ensure <see cref="ProcessStartInfo.RedirectStandardOutput" />
        /// is set to true and <see cref="ProcessStartInfo.UseShellExecute" />
        /// is set to false.- or - The <see cref="StandardOutput" /> stream
        /// has been opened for asynchronous read operations with <see cref="BeginOutputReadLine" />.
        /// </exception>
        public System.IO.StreamReader StandardOutput { get { return default(System.IO.StreamReader); } }
        /// <summary>
        /// Gets or sets the properties to pass to the <see cref="Process.Start" />
        /// method of the <see cref="Process" />.
        /// </summary>
        /// <returns>
        /// The <see cref="ProcessStartInfo" /> that represents the data with which
        /// to start the process. These arguments include the name of the executable file or document
        /// used to start the process.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// The value that specifies the <see cref="StartInfo" /> is null.
        /// </exception>
        public System.Diagnostics.ProcessStartInfo StartInfo { get { return default(System.Diagnostics.ProcessStartInfo); } set { } }
        /// <summary>
        /// Gets the time that the associated process was started.
        /// </summary>
        /// <returns>
        /// An object  that indicates when the process started. An exception is thrown if the process is
        /// not running.
        /// </returns>
        /// <exception cref="PlatformNotSupportedException">
        /// The platform is Windows 98 or Windows Millennium Edition (Windows Me), which does not support
        /// this property.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// You are attempting to access the <see cref="StartTime" /> property
        /// for a process that is running on a remote computer. This property is available only for processes
        /// that are running on the local computer.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The process has exited.-or-The process has not been started.
        /// </exception>
        /// <exception cref="System.ComponentModel.Win32Exception">
        /// An error occurred in the call to the Windows function.
        /// </exception>
        public System.DateTime StartTime { get { return default(System.DateTime); } }
        /// <summary>
        /// Gets the set of threads that are running in the associated process.
        /// </summary>
        /// <returns>
        /// An array of type <see cref="ProcessThread" /> representing the operating
        /// system threads currently running in the associated process.
        /// </returns>
        /// <exception cref="SystemException">
        /// The process does not have an <see cref="Id" />, or no process
        /// is associated with the <see cref="Process" /> instance.-or- The associated
        /// process has exited.
        /// </exception>
        /// <exception cref="PlatformNotSupportedException">
        /// The platform is Windows 98 or Windows Millennium Edition (Windows Me); set
        /// <see cref="ProcessStartInfo.UseShellExecute" /> to false to access this property on Windows 98 and Windows Me.
        /// </exception>
        public System.Diagnostics.ProcessThreadCollection Threads { get { return default(System.Diagnostics.ProcessThreadCollection); } }
        /// <summary>
        /// Gets the total processor time for this process.
        /// </summary>
        /// <returns>
        /// A <see cref="TimeSpan" /> that indicates the amount of time that the associated process
        /// has spent utilizing the CPU. This value is the sum of the
        /// <see cref="UserProcessorTime" /> and the <see cref="PrivilegedProcessorTime" />.
        /// </returns>
        /// <exception cref="PlatformNotSupportedException">
        /// The platform is Windows 98 or Windows Millennium Edition (Windows Me), which does not support
        /// this property.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// You are attempting to access the <see cref="TotalProcessorTime" />
        /// property for a process that is running on a remote computer. This property is available
        /// only for processes that are running on the local computer.
        /// </exception>
        public System.TimeSpan TotalProcessorTime { get { return default(System.TimeSpan); } }
        /// <summary>
        /// Gets the user processor time for this process.
        /// </summary>
        /// <returns>
        /// A <see cref="TimeSpan" /> that indicates the amount of time that the associated process
        /// has spent running code inside the application portion of the process (not inside the operating
        /// system core).
        /// </returns>
        /// <exception cref="PlatformNotSupportedException">
        /// The platform is Windows 98 or Windows Millennium Edition (Windows Me), which does not support
        /// this property.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// You are attempting to access the <see cref="UserProcessorTime" />
        /// property for a process that is running on a remote computer. This property is available
        /// only for processes that are running on the local computer.
        /// </exception>
        public System.TimeSpan UserProcessorTime { get { return default(System.TimeSpan); } }
        /// <summary>
        /// Gets the amount of the virtual memory, in bytes, allocated for the associated process.
        /// </summary>
        /// <returns>
        /// The amount of virtual memory, in bytes, allocated for the associated process.
        /// </returns>
        /// <exception cref="PlatformNotSupportedException">
        /// The platform is Windows 98 or Windows Millennium Edition (Windows Me), which does not support
        /// this property.
        /// </exception>
        public long VirtualMemorySize64 { get { return default(long); } }
        /// <summary>
        /// Gets the amount of physical memory, in bytes, allocated for the associated process.
        /// </summary>
        /// <returns>
        /// The amount of physical memory, in bytes, allocated for the associated process.
        /// </returns>
        /// <exception cref="PlatformNotSupportedException">
        /// The platform is Windows 98 or Windows Millennium Edition (Windows Me), which does not support
        /// this property.
        /// </exception>
        public long WorkingSet64 { get { return default(long); } }
        /// <summary>
        /// Occurs when an application writes to its redirected <see cref="StandardError" />
        /// stream.
        /// </summary>
        public event System.Diagnostics.DataReceivedEventHandler ErrorDataReceived { add { } remove { } }
        /// <summary>
        /// Occurs when a process exits.
        /// </summary>
        public event System.EventHandler Exited { add { } remove { } }
        /// <summary>
        /// Occurs each time an application writes a line to its redirected
        /// <see cref="StandardOutput" /> stream.
        /// </summary>
        public event System.Diagnostics.DataReceivedEventHandler OutputDataReceived { add { } remove { } }
        /// <summary>
        /// Begins asynchronous read operations on the redirected <see cref="StandardError" />
        /// stream of the application.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The <see cref="ProcessStartInfo.RedirectStandardError" /> property is
        /// false.- or - An asynchronous read operation is already in progress on the
        /// <see cref="StandardError" /> stream.- or - The <see cref="StandardError" /> stream has
        /// been used by a synchronous read operation.
        /// </exception>
        public void BeginErrorReadLine() { }
        /// <summary>
        /// Begins asynchronous read operations on the redirected
        /// <see cref="StandardOutput" /> stream of the application.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The <see cref="ProcessStartInfo.RedirectStandardOutput" /> property is
        /// false.- or - An asynchronous read operation is already in progress on the
        /// <see cref="StandardOutput" /> stream.- or - The <see cref="StandardOutput" /> stream has
        /// been used by a synchronous read operation.
        /// </exception>
        public void BeginOutputReadLine() { }
        /// <summary>
        /// Cancels the asynchronous read operation on the redirected
        /// <see cref="StandardError" /> stream of an application.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The <see cref="StandardError" /> stream is not enabled for asynchronous
        /// read operations.
        /// </exception>
        public void CancelErrorRead() { }
        /// <summary>
        /// Cancels the asynchronous read operation on the redirected
        /// <see cref="StandardOutput" /> stream of an application.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The <see cref="StandardOutput" /> stream is not enabled for asynchronous
        /// read operations.
        /// </exception>
        public void CancelOutputRead() { }
        /// <summary>
        /// Puts a <see cref="Process" /> component in state to interact with operating
        /// system processes that run in a special mode by enabling the native property SeDebugPrivilege
        /// on the current thread.
        /// </summary>
        public static void EnterDebugMode() { }
        /// <summary>
        /// Gets a new <see cref="Process" /> component and associates it with the
        /// currently active process.
        /// </summary>
        /// <returns>
        /// A new <see cref="Process" /> component associated with the process resource
        /// that is running the calling application.
        /// </returns>
        public static System.Diagnostics.Process GetCurrentProcess() { return default(System.Diagnostics.Process); }
        /// <summary>
        /// Returns a new <see cref="Process" /> component, given the identifier
        /// of a process on the local computer.
        /// </summary>
        /// <param name="processId">The system-unique identifier of a process resource.</param>
        /// <returns>
        /// A <see cref="Process" /> component that is associated with the local
        /// process resource identified by the <paramref name="processId" /> parameter.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// The process specified by the <paramref name="processId" /> parameter is not running. The identifier
        /// might be expired.
        /// </exception>
        /// <exception cref="InvalidOperationException">The process was not started by this object.</exception>
        public static System.Diagnostics.Process GetProcessById(int processId) { return default(System.Diagnostics.Process); }
        /// <summary>
        /// Returns a new <see cref="Process" /> component, given a process identifier
        /// and the name of a computer on the network.
        /// </summary>
        /// <param name="processId">The system-unique identifier of a process resource.</param>
        /// <param name="machineName">The name of a computer on the network.</param>
        /// <returns>
        /// A <see cref="Process" /> component that is associated with a remote process
        /// resource identified by the <paramref name="processId" /> parameter.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// The process specified by the <paramref name="processId" /> parameter is not running. The identifier
        /// might be expired.-or- The <paramref name="machineName" /> parameter syntax is invalid. The
        /// name might have length zero (0).
        /// </exception>
        /// <exception cref="ArgumentNullException">The <paramref name="machineName" /> parameter is null.</exception>
        /// <exception cref="InvalidOperationException">The process was not started by this object.</exception>
        public static System.Diagnostics.Process GetProcessById(int processId, string machineName) { return default(System.Diagnostics.Process); }
        /// <summary>
        /// Creates a new <see cref="Process" /> component for each process resource
        /// on the local computer.
        /// </summary>
        /// <returns>
        /// An array of type <see cref="Process" /> that represents all the process
        /// resources running on the local computer.
        /// </returns>
        public static System.Diagnostics.Process[] GetProcesses() { return default(System.Diagnostics.Process[]); }
        /// <summary>
        /// Creates a new <see cref="Process" /> component for each process resource
        /// on the specified computer.
        /// </summary>
        /// <param name="machineName">The computer from which to read the list of processes.</param>
        /// <returns>
        /// An array of type <see cref="Process" /> that represents all the process
        /// resources running on the specified computer.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// The <paramref name="machineName" /> parameter syntax is invalid. It might have length zero
        /// (0).
        /// </exception>
        /// <exception cref="ArgumentNullException">The <paramref name="machineName" /> parameter is null.</exception>
        /// <exception cref="PlatformNotSupportedException">
        /// The operating system platform does not support this operation on remote computers.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// There are problems accessing the performance counter API's used to get process information.
        /// This exception is specific to Windows NT, Windows 2000, and Windows XP.
        /// </exception>
        /// <exception cref="System.ComponentModel.Win32Exception">
        /// A problem occurred accessing an underlying system API.
        /// </exception>
        public static System.Diagnostics.Process[] GetProcesses(string machineName) { return default(System.Diagnostics.Process[]); }
        /// <summary>
        /// Creates an array of new <see cref="Process" /> components and associates
        /// them with all the process resources on the local computer that share the specified process
        /// name.
        /// </summary>
        /// <param name="processName">The friendly name of the process.</param>
        /// <returns>
        /// An array of type <see cref="Process" /> that represents the process resources
        /// running the specified application or file.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// There are problems accessing the performance counter API's used to get process information.
        /// This exception is specific to Windows NT, Windows 2000, and Windows XP.
        /// </exception>
        public static System.Diagnostics.Process[] GetProcessesByName(string processName) { return default(System.Diagnostics.Process[]); }
        /// <summary>
        /// Creates an array of new <see cref="Process" /> components and associates
        /// them with all the process resources on a remote computer that share the specified process
        /// name.
        /// </summary>
        /// <param name="processName">The friendly name of the process.</param>
        /// <param name="machineName">The name of a computer on the network.</param>
        /// <returns>
        /// An array of type <see cref="Process" /> that represents the process resources
        /// running the specified application or file.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// The <paramref name="machineName" /> parameter syntax is invalid. It might have length zero
        /// (0).
        /// </exception>
        /// <exception cref="ArgumentNullException">The <paramref name="machineName" /> parameter is null.</exception>
        /// <exception cref="PlatformNotSupportedException">
        /// The operating system platform does not support this operation on remote computers.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// There are problems accessing the performance counter API's used to get process information.
        /// This exception is specific to Windows NT, Windows 2000, and Windows XP.
        /// </exception>
        /// <exception cref="System.ComponentModel.Win32Exception">
        /// A problem occurred accessing an underlying system API.
        /// </exception>
        public static System.Diagnostics.Process[] GetProcessesByName(string processName, string machineName) { return default(System.Diagnostics.Process[]); }
        /// <summary>
        /// Immediately stops the associated process.
        /// </summary>
        /// <exception cref="System.ComponentModel.Win32Exception">
        /// The associated process could not be terminated. -or-The process is terminating.-or- The associated
        /// process is a Win16 executable.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// You are attempting to call <see cref="Kill" /> for a process
        /// that is running on a remote computer. The method is available only for processes running on
        /// the local computer.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The process has already exited. -or-There is no process associated with this
        /// <see cref="Process" /> object.
        /// </exception>
        public void Kill() { }
        /// <summary>
        /// Takes a <see cref="Process" /> component out of the state that lets it
        /// interact with operating system processes that run in a special mode.
        /// </summary>
        public static void LeaveDebugMode() { }
        /// <summary>
        /// Raises the <see cref="Exited" /> event.
        /// </summary>
        protected void OnExited() { }
        /// <summary>
        /// Discards any information about the associated process that has been cached inside the process
        /// component.
        /// </summary>
        public void Refresh() { }
        /// <summary>
        /// Starts (or reuses) the process resource that is specified by the
        /// <see cref="StartInfo" /> property of this <see cref="Process" /> component and associates it
        /// with the component.
        /// </summary>
        /// <returns>
        /// true if a process resource is started; false if no new process resource is started (for example,
        /// if an existing process is reused).
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// No file name was specified in the <see cref="Process" /> component's
        /// <see cref="StartInfo" />.-or- The
        /// <see cref="ProcessStartInfo.UseShellExecute" /> member of the <see cref="StartInfo" /> property is true while
        /// <see cref="ProcessStartInfo.RedirectStandardInput" />,
        /// <see cref="ProcessStartInfo.RedirectStandardOutput" />, or <see cref="ProcessStartInfo.RedirectStandardError" /> is true.
        /// </exception>
        /// <exception cref="System.ComponentModel.Win32Exception">
        /// There was an error in opening the associated file.
        /// </exception>
        /// <exception cref="ObjectDisposedException">The process object has already been disposed.</exception>
        public bool Start() { return default(bool); }
        /// <summary>
        /// Starts the process resource that is specified by the parameter containing process start information
        /// (for example, the file name of the process to start) and associates the resource with a new
        /// <see cref="Process" /> component.
        /// </summary>
        /// <param name="startInfo">
        /// The <see cref="ProcessStartInfo" /> that contains the information that
        /// is used to start the process, including the file name and any command-line arguments.
        /// </param>
        /// <returns>
        /// A new <see cref="Process" /> that is associated with the process resource,
        /// or null if no process resource is started. Note that a new process that’s started alongside
        /// already running instances of the same process will be independent from the others. In addition,
        /// Start may return a non-null Process with its <see cref="HasExited" />
        /// property already set to true. In this case, the started process may have activated an existing
        /// instance of itself and then exited.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// No file name was specified in the <paramref name="startInfo" /> parameter's
        /// <see cref="ProcessStartInfo.FileName" /> property.-or- The <see cref="ProcessStartInfo.UseShellExecute" />
        /// property of the <paramref name="startInfo" /> parameter is true and the
        /// <see cref="ProcessStartInfo.RedirectStandardInput" />, <see cref="ProcessStartInfo.RedirectStandardOutput" />, or
        /// <see cref="ProcessStartInfo.RedirectStandardError" /> property is also true.-or-The
        /// <see cref="ProcessStartInfo.UseShellExecute" /> property of the <paramref name="startInfo" /> parameter is true and the
        /// <see cref="ProcessStartInfo.UserName" /> property is not null or empty or the
        /// <see cref="ProcessStartInfo.Password" /> property is not null.
        /// </exception>
        /// <exception cref="ArgumentNullException">The <paramref name="startInfo" /> parameter is null.</exception>
        /// <exception cref="ObjectDisposedException">The process object has already been disposed.</exception>
        /// <exception cref="IO.FileNotFoundException">
        /// The file specified in the <paramref name="startInfo" /> parameter's
        /// <see cref="ProcessStartInfo.FileName" /> property could not be found.
        /// </exception>
        /// <exception cref="System.ComponentModel.Win32Exception">
        /// An error occurred when opening the associated file. -or-The sum of the length of the arguments
        /// and the length of the full path to the process exceeds 2080. The error message associated with this
        /// exception can be one of the following: "The data area passed to a system call is too small." or
        /// "Access is denied."
        /// </exception>
        public static System.Diagnostics.Process Start(System.Diagnostics.ProcessStartInfo startInfo) { return default(System.Diagnostics.Process); }
        /// <summary>
        /// Starts a process resource by specifying the name of a document or application file and associates
        /// the resource with a new <see cref="Process" /> component.
        /// </summary>
        /// <param name="fileName">The name of a document or application file to run in the process.</param>
        /// <returns>
        /// A new <see cref="Process" /> that is associated with the process resource,
        /// or null if no process resource is started. Note that a new process that’s started alongside
        /// already running instances of the same process will be independent from the others. In addition,
        /// Start may return a non-null Process with its <see cref="HasExited" />
        /// property already set to true. In this case, the started process may have activated an existing
        /// instance of itself and then exited.
        /// </returns>
        /// <exception cref="System.ComponentModel.Win32Exception">
        /// An error occurred when opening the associated file.
        /// </exception>
        /// <exception cref="ObjectDisposedException">The process object has already been disposed.</exception>
        /// <exception cref="IO.FileNotFoundException">
        /// The PATH environment variable has a string containing quotes.
        /// </exception>
        public static System.Diagnostics.Process Start(string fileName) { return default(System.Diagnostics.Process); }
        /// <summary>
        /// Starts a process resource by specifying the name of an application and a set of command-line
        /// arguments, and associates the resource with a new <see cref="Process" />
        /// component.
        /// </summary>
        /// <param name="fileName">The name of an application file to run in the process.</param>
        /// <param name="arguments">Command-line arguments to pass when starting the process.</param>
        /// <returns>
        /// A new <see cref="Process" /> that is associated with the process resource,
        /// or null if no process resource is started. Note that a new process that’s started alongside
        /// already running instances of the same process will be independent from the others. In addition,
        /// Start may return a non-null Process with its <see cref="HasExited" />
        /// property already set to true. In this case, the started process may have activated an existing
        /// instance of itself and then exited.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// The <paramref name="fileName" /> or <paramref name="arguments" /> parameter is null.
        /// </exception>
        /// <exception cref="System.ComponentModel.Win32Exception">
        /// An error occurred when opening the associated file. -or-The sum of the length of the arguments
        /// and the length of the full path to the process exceeds 2080. The error message associated with this
        /// exception can be one of the following: "The data area passed to a system call is too small." or
        /// "Access is denied."
        /// </exception>
        /// <exception cref="ObjectDisposedException">The process object has already been disposed.</exception>
        /// <exception cref="IO.FileNotFoundException">
        /// The PATH environment variable has a string containing quotes.
        /// </exception>
        public static System.Diagnostics.Process Start(string fileName, string arguments) { return default(System.Diagnostics.Process); }
        /// <summary>
        /// Instructs the <see cref="Process" /> component to wait indefinitely for
        /// the associated process to exit.
        /// </summary>
        /// <exception cref="System.ComponentModel.Win32Exception">The wait setting could not be accessed.</exception>
        /// <exception cref="SystemException">
        /// No process <see cref="Id" /> has been set, and a
        /// <see cref="Process.Handle" /> from which the <see cref="Id" /> property can be determined
        /// does not exist.-or- There is no process associated with this
        /// <see cref="Process" /> object.-or- You are attempting to call
        /// <see cref="Process.WaitForExit" /> for a process that is running on a remote computer. This method is available only for processes
        /// that are running on the local computer.
        /// </exception>
        public void WaitForExit() { }
        /// <summary>
        /// Instructs the <see cref="Process" /> component to wait the specified
        /// number of milliseconds for the associated process to exit.
        /// </summary>
        /// <param name="milliseconds">
        /// The amount of time, in milliseconds, to wait for the associated process to exit. The maximum
        /// is the largest possible value of a 32-bit integer, which represents infinity to the operating system.
        /// </param>
        /// <returns>
        /// true if the associated process has exited; otherwise, false.
        /// </returns>
        /// <exception cref="System.ComponentModel.Win32Exception">The wait setting could not be accessed.</exception>
        /// <exception cref="SystemException">
        /// No process <see cref="Id" /> has been set, and a
        /// <see cref="Process.Handle" /> from which the <see cref="Id" /> property can be determined
        /// does not exist.-or- There is no process associated with this
        /// <see cref="Process" /> object.-or- You are attempting to call
        /// <see cref="WaitForExit(Int32)" /> for a process that is running on a remote computer. This method is available only for processes
        /// that are running on the local computer.
        /// </exception>
        public bool WaitForExit(int milliseconds) { return default(bool); }
    }
    /// <summary>
    /// Represents a.dll or .exe file that is loaded into a particular process.
    /// </summary>
    public partial class ProcessModule
    {
        internal ProcessModule() { }
        /// <summary>
        /// Gets the memory address where the module was loaded.
        /// </summary>
        /// <returns>
        /// The load address of the module.
        /// </returns>
        public System.IntPtr BaseAddress { get { return default(System.IntPtr); } }
        /// <summary>
        /// Gets the memory address for the function that runs when the system loads and runs the module.
        /// </summary>
        /// <returns>
        /// The entry point of the module.
        /// </returns>
        public System.IntPtr EntryPointAddress { get { return default(System.IntPtr); } }
        /// <summary>
        /// Gets the full path to the module.
        /// </summary>
        /// <returns>
        /// The fully qualified path that defines the location of the module.
        /// </returns>
        public string FileName { get { return default(string); } }
        /// <summary>
        /// Gets the amount of memory that is required to load the module.
        /// </summary>
        /// <returns>
        /// The size, in bytes, of the memory that the module occupies.
        /// </returns>
        public int ModuleMemorySize { get { return default(int); } }
        /// <summary>
        /// Gets the name of the process module.
        /// </summary>
        /// <returns>
        /// The name of the module.
        /// </returns>
        public string ModuleName { get { return default(string); } }
        /// <summary>
        /// Converts the name of the module to a string.
        /// </summary>
        /// <returns>
        /// The value of the <see cref="ModuleName" /> property.
        /// </returns>
        public override string ToString() { return default(string); }
    }
    /// <summary>
    /// Provides a strongly typed collection of <see cref="ProcessModule" />
    /// objects.
    /// </summary>
    public partial class ProcessModuleCollection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessModuleCollection" />
        /// class, with no associated <see cref="ProcessModule" /> instances.
        /// </summary>
        protected ProcessModuleCollection() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessModuleCollection" />
        /// class, using the specified array of <see cref="ProcessModule" /> instances.
        /// </summary>
        /// <param name="processModules">
        /// An array of <see cref="ProcessModule" /> instances with which to initialize
        /// this <see cref="ProcessModuleCollection" /> instance.
        /// </param>
        public ProcessModuleCollection(System.Diagnostics.ProcessModule[] processModules) { }
        /// <summary>
        /// Gets an index for iterating over the set of process modules.
        /// </summary>
        /// <param name="index">The zero-based index value of the module in the collection.</param>
        /// <returns>
        /// A <see cref="ProcessModule" /> that indexes the modules in the collection
        /// </returns>
        public System.Diagnostics.ProcessModule this[int index] { get { return default(System.Diagnostics.ProcessModule); } }
        /// <summary>
        /// Determines whether the specified process module exists in the collection.
        /// </summary>
        /// <param name="module">
        /// A <see cref="ProcessModule" /> instance that indicates the module to
        /// find in this collection.
        /// </param>
        /// <returns>
        /// true if the module exists in the collection; otherwise, false.
        /// </returns>
        public bool Contains(System.Diagnostics.ProcessModule module) { return default(bool); }
        /// <summary>
        /// Copies an array of <see cref="ProcessModule" /> instances to the collection,
        /// at the specified index.
        /// </summary>
        /// <param name="array">
        /// An array of <see cref="ProcessModule" /> instances to add to the collection.
        /// </param>
        /// <param name="index">The location at which to add the new instances.</param>
        public void CopyTo(System.Diagnostics.ProcessModule[] array, int index) { }
        /// <summary>
        /// Provides the location of a specified module within the collection.
        /// </summary>
        /// <param name="module">The <see cref="ProcessModule" /> whose index is retrieved.</param>
        /// <returns>
        /// The zero-based index that defines the location of the module within the
        /// <see cref="ProcessModuleCollection" />.
        /// </returns>
        public int IndexOf(System.Diagnostics.ProcessModule module) { return default(int); }
    }
    /// <summary>
    /// Indicates the priority that the system associates with a process. This value, together with
    /// the priority value of each thread of the process, determines each thread's base priority level.
    /// </summary>
    public enum ProcessPriorityClass
    {
        /// <summary>
        /// Specifies that the process has priority above Normal but below
        /// <see cref="High" />.
        /// </summary>
        AboveNormal = 32768,
        /// <summary>
        /// Specifies that the process has priority above Idle but below Normal.
        /// </summary>
        BelowNormal = 16384,
        /// <summary>
        /// Specifies that the process performs time-critical tasks that must be executed immediately,
        /// such as the Task List dialog, which must respond quickly when called by the user, regardless of the
        /// load on the operating system. The threads of the process preempt the threads of normal or idle priority
        /// class processes.
        /// </summary>
        High = 128,
        /// <summary>
        /// Specifies that the threads of this process run only when the system is idle, such as a screen
        /// saver. The threads of the process are preempted by the threads of any process running in a higher
        /// priority class.
        /// </summary>
        Idle = 64,
        /// <summary>
        /// Specifies that the process has no special scheduling needs.
        /// </summary>
        Normal = 32,
        /// <summary>
        /// Specifies that the process has the highest possible priority.
        /// </summary>
        RealTime = 256,
    }
    /// <summary>
    /// Specifies a set of values that are used when you start a process.
    /// </summary>
    public sealed partial class ProcessStartInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessStartInfo" /> class
        /// without specifying a file name with which to start the process.
        /// </summary>
        public ProcessStartInfo() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessStartInfo" /> class
        /// and specifies a file name such as an application or document with which to start the process.
        /// </summary>
        /// <param name="fileName">An application or document with which to start a process.</param>
        public ProcessStartInfo(string fileName) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessStartInfo" /> class,
        /// specifies an application file name with which to start the process, and specifies a set of
        /// command-line arguments to pass to the application.
        /// </summary>
        /// <param name="fileName">An application with which to start a process.</param>
        /// <param name="arguments">Command-line arguments to pass to the application when the process starts.</param>
        public ProcessStartInfo(string fileName, string arguments) { }
        /// <summary>
        /// Gets or sets the set of command-line arguments to use when starting the application.
        /// </summary>
        /// <returns>
        /// A single string containing the arguments to pass to the target application specified in the
        /// <see cref="FileName" /> property. The default is an
        /// empty string (""). On Windows Vista and earlier versions of the Windows operating system,
        /// the length of the arguments added to the length of the full path to the process must be less
        /// than 2080. On Windows 7 and later versions, the length must be less than 32699.Arguments are
        /// parsed and interpreted by the target application, so must align with the expectations of that
        /// application. For.NET applications as demonstrated in the Examples below, spaces are interpreted
        /// as a separator between multiple arguments. A single argument that includes spaces must be
        /// surrounded by quotation marks, but those quotation marks are not carried through to the target
        /// application. In include quotation marks in the final parsed argument, triple-escape each mark.
        /// </returns>
        public string Arguments { get { return default(string); } set { } }
        /// <summary>
        /// Gets or sets a value indicating whether to start the process in a new window.
        /// </summary>
        /// <returns>
        /// true if the process should be started without creating a new window to contain it; otherwise,
        /// false. The default is false.
        /// </returns>
        public bool CreateNoWindow { get { return default(bool); } set { } }
        /// <summary>
        /// Gets or sets a value that identifies the domain to use when starting the process.
        /// </summary>
        /// <returns>
        /// The Active Directory domain to use when starting the process. The domain property is primarily
        /// of interest to users within enterprise environments that use Active Directory.
        /// </returns>
        public string Domain { get { return default(string); } set { } }
        /// <summary>
        /// Gets the environment variables that apply to this process and its child processes.
        /// </summary>
        /// <returns>
        /// A generic dictionary containing the environment variables that apply to this process and its
        /// child processes. The default is null.
        /// </returns>
        [System.ComponentModel.DefaultValueAttribute(null)]
        public System.Collections.Generic.IDictionary<string, string> Environment { get { return default(System.Collections.Generic.IDictionary<string, string>); } }
        /// <summary>
        /// Gets or sets the application or document to start.
        /// </summary>
        /// <returns>
        /// The name of the application to start, or the name of a document of a file type that is associated
        /// with an application and that has a default open action available to it. The default is an empty
        /// string ("").
        /// </returns>
        public string FileName { get { return default(string); } set { } }
        /// <summary>
        /// Gets or sets a value that indicates whether the Windows user profile is to be loaded from the
        /// registry.
        /// </summary>
        /// <returns>
        /// true if the Windows user profile should be loaded; otherwise, false. The default is false.
        /// </returns>
        public bool LoadUserProfile { get { return default(bool); } set { } }
        /// <summary>
        /// Gets or sets a value that indicates whether the error output of an application is written
        /// to the <see cref="Process.StandardError" /> stream.
        /// </summary>
        /// <returns>
        /// true if error output should be written to <see cref="Process.StandardError" />
        /// ; otherwise, false. The default is false.
        /// </returns>
        public bool RedirectStandardError { get { return default(bool); } set { } }
        /// <summary>
        /// Gets or sets a value indicating whether the input for an application is read from the
        /// <see cref="Process.StandardInput" /> stream.
        /// </summary>
        /// <returns>
        /// true if input should be read from <see cref="Process.StandardInput" />;
        /// otherwise, false. The default is false.
        /// </returns>
        public bool RedirectStandardInput { get { return default(bool); } set { } }
        /// <summary>
        /// Gets or sets a value that indicates whether the textual output of an application is written
        /// to the <see cref="Process.StandardOutput" /> stream.
        /// </summary>
        /// <returns>
        /// true if output should be written to <see cref="Process.StandardOutput" />
        /// ; otherwise, false. The default is false.
        /// </returns>
        public bool RedirectStandardOutput { get { return default(bool); } set { } }
        /// <summary>
        /// Gets or sets the preferred encoding for error output.
        /// </summary>
        /// <returns>
        /// An object that represents the preferred encoding for error output. The default is null.
        /// </returns>
        public System.Text.Encoding StandardErrorEncoding { get { return default(System.Text.Encoding); } set { } }
        /// <summary>
        /// Gets or sets the preferred encoding for standard output.
        /// </summary>
        /// <returns>
        /// An object that represents the preferred encoding for standard output. The default is null.
        /// </returns>
        public System.Text.Encoding StandardOutputEncoding { get { return default(System.Text.Encoding); } set { } }
        /// <summary>
        /// Gets or sets the user name to be used when starting the process.
        /// </summary>
        /// <returns>
        /// The user name to use when starting the process.
        /// </returns>
        public string UserName { get { return default(string); } set { } }
        /// <summary>
        /// Gets or sets a value indicating whether to use the operating system shell to start the process.
        /// </summary>
        /// <returns>
        /// true if the shell should be used when starting the process; false if the process should be
        /// created directly from the executable file. The default is true.
        /// </returns>
        public bool UseShellExecute { get { return default(bool); } set { } }
        /// <summary>
        /// When the <see cref="UseShellExecute" /> property is
        /// false, gets or sets the working directory for the process to be started. When
        /// <see cref="UseShellExecute" /> is true, gets or sets the directory that contains the process to be started.
        /// </summary>
        /// <returns>
        /// When <see cref="UseShellExecute" /> is true, the fully
        /// qualified name of the directory that contains the process to be started. When the
        /// <see cref="UseShellExecute" /> property is false, the working directory for the process to be started. The default is
        /// an empty string ("").
        /// </returns>
        public string WorkingDirectory { get { return default(string); } set { } }
    }
    /// <summary>
    /// Represents an operating system process thread.
    /// </summary>
    public partial class ProcessThread
    {
        internal ProcessThread() { }
        /// <summary>
        /// Gets the base priority of the thread.
        /// </summary>
        /// <returns>
        /// The base priority of the thread, which the operating system computes by combining the process
        /// priority class with the priority level of the associated thread.
        /// </returns>
        public int BasePriority { get { return default(int); } }
        /// <summary>
        /// Gets the current priority of the thread.
        /// </summary>
        /// <returns>
        /// The current priority of the thread, which may deviate from the base priority based on how the
        /// operating system is scheduling the thread. The priority may be temporarily boosted for an active
        /// thread.
        /// </returns>
        public int CurrentPriority { get { return default(int); } }
        /// <summary>
        /// Gets the unique identifier of the thread.
        /// </summary>
        /// <returns>
        /// The unique identifier associated with a specific thread.
        /// </returns>
        public int Id { get { return default(int); } }
        /// <summary>
        /// Sets the preferred processor for this thread to run on.
        /// </summary>
        /// <returns>
        /// The preferred processor for the thread, used when the system schedules threads, to determine
        /// which processor to run the thread on.
        /// </returns>
        /// <exception cref="System.ComponentModel.Win32Exception">
        /// The system could not set the thread to start on the specified processor.
        /// </exception>
        /// <exception cref="PlatformNotSupportedException">
        /// The platform is Windows 98 or Windows Millennium Edition.
        /// </exception>
        /// <exception cref="NotSupportedException">The process is on a remote computer.</exception>
        public int IdealProcessor { set { } }
        /// <summary>
        /// Gets or sets a value indicating whether the operating system should temporarily boost the priority
        /// of the associated thread whenever the main window of the thread's process receives the focus.
        /// </summary>
        /// <returns>
        /// true to boost the thread's priority when the user interacts with the process's interface; otherwise,
        /// false. The default is false.
        /// </returns>
        /// <exception cref="System.ComponentModel.Win32Exception">
        /// The priority boost information could not be retrieved.-or-The priority boost information could
        /// not be set.
        /// </exception>
        /// <exception cref="PlatformNotSupportedException">
        /// The platform is Windows 98 or Windows Millennium Edition.
        /// </exception>
        /// <exception cref="NotSupportedException">The process is on a remote computer.</exception>
        public bool PriorityBoostEnabled { get { return default(bool); } set { } }
        /// <summary>
        /// Gets or sets the priority level of the thread.
        /// </summary>
        /// <returns>
        /// One of the <see cref="ThreadPriorityLevel" /> values, specifying a range
        /// that bounds the thread's priority.
        /// </returns>
        /// <exception cref="System.ComponentModel.Win32Exception">
        /// The thread priority level information could not be retrieved. -or-The thread priority level
        /// could not be set.
        /// </exception>
        /// <exception cref="PlatformNotSupportedException">
        /// The platform is Windows 98 or Windows Millennium Edition.
        /// </exception>
        /// <exception cref="NotSupportedException">The process is on a remote computer.</exception>
        public System.Diagnostics.ThreadPriorityLevel PriorityLevel { get { return default(System.Diagnostics.ThreadPriorityLevel); } set { } }
        /// <summary>
        /// Gets the amount of time that the thread has spent running code inside the operating system
        /// core.
        /// </summary>
        /// <returns>
        /// A <see cref="TimeSpan" /> indicating the amount of time that the thread has spent
        /// running code inside the operating system core.
        /// </returns>
        /// <exception cref="System.ComponentModel.Win32Exception">The thread time could not be retrieved.</exception>
        /// <exception cref="PlatformNotSupportedException">
        /// The platform is Windows 98 or Windows Millennium Edition.
        /// </exception>
        /// <exception cref="NotSupportedException">The process is on a remote computer.</exception>
        public System.TimeSpan PrivilegedProcessorTime { get { return default(System.TimeSpan); } }
        /// <summary>
        /// Sets the processors on which the associated thread can run.
        /// </summary>
        /// <returns>
        /// An <see cref="IntPtr" /> that points to a set of bits, each of which represents a
        /// processor that the thread can run on.
        /// </returns>
        /// <exception cref="System.ComponentModel.Win32Exception">The processor affinity could not be set.</exception>
        /// <exception cref="PlatformNotSupportedException">
        /// The platform is Windows 98 or Windows Millennium Edition.
        /// </exception>
        /// <exception cref="NotSupportedException">The process is on a remote computer.</exception>
        public System.IntPtr ProcessorAffinity { set { } }
        /// <summary>
        /// Gets the memory address of the function that the operating system called that started this
        /// thread.
        /// </summary>
        /// <returns>
        /// The thread's starting address, which points to the application-defined function that the thread
        /// executes.
        /// </returns>
        /// <exception cref="PlatformNotSupportedException">
        /// The platform is Windows 98 or Windows Millennium Edition.
        /// </exception>
        /// <exception cref="NotSupportedException">The process is on a remote computer.</exception>
        public System.IntPtr StartAddress { get { return default(System.IntPtr); } }
        /// <summary>
        /// Gets the time that the operating system started the thread.
        /// </summary>
        /// <returns>
        /// A <see cref="DateTime" /> representing the time that was on the system when the operating
        /// system started the thread.
        /// </returns>
        /// <exception cref="System.ComponentModel.Win32Exception">The thread time could not be retrieved.</exception>
        /// <exception cref="PlatformNotSupportedException">
        /// The platform is Windows 98 or Windows Millennium Edition.
        /// </exception>
        /// <exception cref="NotSupportedException">The process is on a remote computer.</exception>
        public System.DateTime StartTime { get { return default(System.DateTime); } }
        /// <summary>
        /// Gets the current state of this thread.
        /// </summary>
        /// <returns>
        /// A <see cref="Diagnostics.ThreadState" /> that indicates the thread's execution, for
        /// example, running, waiting, or terminated.
        /// </returns>
        /// <exception cref="PlatformNotSupportedException">
        /// The platform is Windows 98 or Windows Millennium Edition.
        /// </exception>
        /// <exception cref="NotSupportedException">The process is on a remote computer.</exception>
        public System.Diagnostics.ThreadState ThreadState { get { return default(System.Diagnostics.ThreadState); } }
        /// <summary>
        /// Gets the total amount of time that this thread has spent using the processor.
        /// </summary>
        /// <returns>
        /// A <see cref="TimeSpan" /> that indicates the amount of time that the thread has had
        /// control of the processor.
        /// </returns>
        /// <exception cref="System.ComponentModel.Win32Exception">The thread time could not be retrieved.</exception>
        /// <exception cref="PlatformNotSupportedException">
        /// The platform is Windows 98 or Windows Millennium Edition.
        /// </exception>
        /// <exception cref="NotSupportedException">The process is on a remote computer.</exception>
        public System.TimeSpan TotalProcessorTime { get { return default(System.TimeSpan); } }
        /// <summary>
        /// Gets the amount of time that the associated thread has spent running code inside the application.
        /// </summary>
        /// <returns>
        /// A <see cref="TimeSpan" /> indicating the amount of time that the thread has spent
        /// running code inside the application, as opposed to inside the operating system core.
        /// </returns>
        /// <exception cref="System.ComponentModel.Win32Exception">The thread time could not be retrieved.</exception>
        /// <exception cref="PlatformNotSupportedException">
        /// The platform is Windows 98 or Windows Millennium Edition.
        /// </exception>
        /// <exception cref="NotSupportedException">The process is on a remote computer.</exception>
        public System.TimeSpan UserProcessorTime { get { return default(System.TimeSpan); } }
        /// <summary>
        /// Gets the reason that the thread is waiting.
        /// </summary>
        /// <returns>
        /// A <see cref="ThreadWaitReason" /> representing the reason that the thread
        /// is in the wait state.
        /// </returns>
        /// <exception cref="InvalidOperationException">The thread is not in the wait state.</exception>
        /// <exception cref="PlatformNotSupportedException">
        /// The platform is Windows 98 or Windows Millennium Edition.
        /// </exception>
        /// <exception cref="NotSupportedException">The process is on a remote computer.</exception>
        public System.Diagnostics.ThreadWaitReason WaitReason { get { return default(System.Diagnostics.ThreadWaitReason); } }
        /// <summary>
        /// Resets the ideal processor for this thread to indicate that there is no single ideal processor.
        /// In other words, so that any processor is ideal.
        /// </summary>
        /// <exception cref="System.ComponentModel.Win32Exception">The ideal processor could not be reset.</exception>
        /// <exception cref="PlatformNotSupportedException">
        /// The platform is Windows 98 or Windows Millennium Edition.
        /// </exception>
        /// <exception cref="NotSupportedException">The process is on a remote computer.</exception>
        public void ResetIdealProcessor() { }
    }
    /// <summary>
    /// Provides a strongly typed collection of <see cref="ProcessThread" />
    /// objects.
    /// </summary>
    public partial class ProcessThreadCollection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessThreadCollection" />
        /// class, with no associated <see cref="ProcessThread" /> instances.
        /// </summary>
        protected ProcessThreadCollection() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessThreadCollection" />
        /// class, using the specified array of <see cref="ProcessThread" /> instances.
        /// </summary>
        /// <param name="processThreads">
        /// An array of <see cref="ProcessThread" /> instances with which to initialize
        /// this <see cref="ProcessThreadCollection" /> instance.
        /// </param>
        public ProcessThreadCollection(System.Diagnostics.ProcessThread[] processThreads) { }
        /// <summary>
        /// Gets an index for iterating over the set of process threads.
        /// </summary>
        /// <param name="index">The zero-based index value of the thread in the collection.</param>
        /// <returns>
        /// A <see cref="ProcessThread" /> that indexes the threads in the collection.
        /// </returns>
        public System.Diagnostics.ProcessThread this[int index] { get { return default(System.Diagnostics.ProcessThread); } }
        /// <summary>
        /// Appends a process thread to the collection.
        /// </summary>
        /// <param name="thread">The thread to add to the collection.</param>
        /// <returns>
        /// The zero-based index of the thread in the collection.
        /// </returns>
        public int Add(System.Diagnostics.ProcessThread thread) { return default(int); }
        /// <summary>
        /// Determines whether the specified process thread exists in the collection.
        /// </summary>
        /// <param name="thread">
        /// A <see cref="ProcessThread" /> instance that indicates the thread to
        /// find in this collection.
        /// </param>
        /// <returns>
        /// true if the thread exists in the collection; otherwise, false.
        /// </returns>
        public bool Contains(System.Diagnostics.ProcessThread thread) { return default(bool); }
        /// <summary>
        /// Copies an array of <see cref="ProcessThread" /> instances to the collection,
        /// at the specified index.
        /// </summary>
        /// <param name="array">
        /// An array of <see cref="ProcessThread" /> instances to add to the collection.
        /// </param>
        /// <param name="index">The location at which to add the new instances.</param>
        public void CopyTo(System.Diagnostics.ProcessThread[] array, int index) { }
        /// <summary>
        /// Provides the location of a specified thread within the collection.
        /// </summary>
        /// <param name="thread">The <see cref="ProcessThread" /> whose index is retrieved.</param>
        /// <returns>
        /// The zero-based index that defines the location of the thread within the
        /// <see cref="ProcessThreadCollection" />.
        /// </returns>
        public int IndexOf(System.Diagnostics.ProcessThread thread) { return default(int); }
        /// <summary>
        /// Inserts a process thread at the specified location in the collection.
        /// </summary>
        /// <param name="index">The zero-based index indicating the location at which to insert the thread.</param>
        /// <param name="thread">The thread to insert into the collection.</param>
        public void Insert(int index, System.Diagnostics.ProcessThread thread) { }
        /// <summary>
        /// Deletes a process thread from the collection.
        /// </summary>
        /// <param name="thread">The thread to remove from the collection.</param>
        public void Remove(System.Diagnostics.ProcessThread thread) { }
    }
    /// <summary>
    /// Specifies the priority level of a thread.
    /// </summary>
    public enum ThreadPriorityLevel
    {
        /// <summary>
        /// Specifies one step above the normal priority for the associated
        /// <see cref="ProcessPriorityClass" />.
        /// </summary>
        AboveNormal = 1,
        /// <summary>
        /// Specifies one step below the normal priority for the associated
        /// <see cref="ProcessPriorityClass" />.
        /// </summary>
        BelowNormal = -1,
        /// <summary>
        /// Specifies highest priority. This is two steps above the normal priority for the associated
        /// <see cref="ProcessPriorityClass" />.
        /// </summary>
        Highest = 2,
        /// <summary>
        /// Specifies idle priority. This is the lowest possible priority value of all threads, independent
        /// of the value of the associated <see cref="ProcessPriorityClass" />.
        /// </summary>
        Idle = -15,
        /// <summary>
        /// Specifies lowest priority. This is two steps below the normal priority for the associated
        /// <see cref="ProcessPriorityClass" />.
        /// </summary>
        Lowest = -2,
        /// <summary>
        /// Specifies normal priority for the associated <see cref="ProcessPriorityClass" />.
        /// </summary>
        Normal = 0,
        /// <summary>
        /// Specifies time-critical priority. This is the highest priority of all threads, independent
        /// of the value of the associated <see cref="ProcessPriorityClass" />.
        /// </summary>
        TimeCritical = 15,
    }
    /// <summary>
    /// Specifies the current execution state of the thread.
    /// </summary>
    public enum ThreadState
    {
        /// <summary>
        /// A state that indicates the thread has been initialized, but has not yet started.
        /// </summary>
        Initialized = 0,
        /// <summary>
        /// A state that indicates the thread is waiting to use a processor because no processor is free.
        /// The thread is prepared to run on the next available processor.
        /// </summary>
        Ready = 1,
        /// <summary>
        /// A state that indicates the thread is currently using a processor.
        /// </summary>
        Running = 2,
        /// <summary>
        /// A state that indicates the thread is about to use a processor. Only one thread can be in this
        /// state at a time.
        /// </summary>
        Standby = 3,
        /// <summary>
        /// A state that indicates the thread has finished executing and has exited.
        /// </summary>
        Terminated = 4,
        /// <summary>
        /// A state that indicates the thread is waiting for a resource, other than the processor, before
        /// it can execute. For example, it might be waiting for its execution stack to be paged in from disk.
        /// </summary>
        Transition = 6,
        /// <summary>
        /// The state of the thread is unknown.
        /// </summary>
        Unknown = 7,
        /// <summary>
        /// A state that indicates the thread is not ready to use the processor because it is waiting for
        /// a peripheral operation to complete or a resource to become free. When the thread is ready, it will
        /// be rescheduled.
        /// </summary>
        Wait = 5,
    }
    /// <summary>
    /// Specifies the reason a thread is waiting.
    /// </summary>
    public enum ThreadWaitReason
    {
        /// <summary>
        /// The thread is waiting for event pair high.
        /// </summary>
        EventPairHigh = 7,
        /// <summary>
        /// The thread is waiting for event pair low.
        /// </summary>
        EventPairLow = 8,
        /// <summary>
        /// Thread execution is delayed.
        /// </summary>
        ExecutionDelay = 4,
        /// <summary>
        /// The thread is waiting for the scheduler.
        /// </summary>
        Executive = 0,
        /// <summary>
        /// The thread is waiting for a free virtual memory page.
        /// </summary>
        FreePage = 1,
        /// <summary>
        /// The thread is waiting for a local procedure call to arrive.
        /// </summary>
        LpcReceive = 9,
        /// <summary>
        /// The thread is waiting for reply to a local procedure call to arrive.
        /// </summary>
        LpcReply = 10,
        /// <summary>
        /// The thread is waiting for a virtual memory page to arrive in memory.
        /// </summary>
        PageIn = 2,
        /// <summary>
        /// The thread is waiting for a virtual memory page to be written to disk.
        /// </summary>
        PageOut = 12,
        /// <summary>
        /// Thread execution is suspended.
        /// </summary>
        Suspended = 5,
        /// <summary>
        /// The thread is waiting for system allocation.
        /// </summary>
        SystemAllocation = 3,
        /// <summary>
        /// The thread is waiting for an unknown reason.
        /// </summary>
        Unknown = 13,
        /// <summary>
        /// The thread is waiting for a user request.
        /// </summary>
        UserRequest = 6,
        /// <summary>
        /// The thread is waiting for the system to allocate virtual memory.
        /// </summary>
        VirtualMemory = 11,
    }
}
