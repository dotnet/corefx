// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------

namespace System.Diagnostics
{
    public partial class Process : System.IDisposable
    {
        public void Dispose() { }
        /// <summary>
        /// Release all resources used by this process.
        /// </summary>
        /// <param name="disposing">
        /// true to release both managed and unmanaged resources; false to release only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing) { }
    }
    /// <summary>
    /// Provides a strongly typed collection of <see cref="ProcessModule" />
    /// objects.
    /// </summary>
    public partial class ProcessModuleCollection : System.Collections.ICollection
    {
        public int Count { get { return default(int); } }
        bool System.Collections.ICollection.IsSynchronized { get { return default(bool); } }
        object System.Collections.ICollection.SyncRoot { get { return default(object); } }
        void System.Collections.ICollection.CopyTo(System.Array array, int index) { }
        public System.Collections.IEnumerator GetEnumerator() { return default(System.Collections.IEnumerator); }
    }
    /// <summary>
    /// Provides a strongly typed collection of <see cref="ProcessThread" />
    /// objects.
    /// </summary>
    public partial class ProcessThreadCollection : System.Collections.ICollection
    {
        public int Count { get { return default(int); } }
        bool System.Collections.ICollection.IsSynchronized { get { return default(bool); } }
        object System.Collections.ICollection.SyncRoot { get { return default(object); } }
        void System.Collections.ICollection.CopyTo(System.Array array, int index) { }
        public System.Collections.IEnumerator GetEnumerator() { return default(System.Collections.IEnumerator); }
    }
    /// <summary>
    /// Specifies a set of values that are used when you start a process.
    /// </summary>
    public sealed partial class ProcessStartInfo
    {
        /// <summary>
        /// Gets or sets the user password in clear text to use when starting the process.
        /// </summary>
        /// <returns>
        /// The user password in clear text.
        /// </returns>
        public string PasswordInClearText { get { return default(string); } set { } }
    }
}
namespace Microsoft.Win32.SafeHandles
{
    // Members from SafeHandleMinusOneOrZeroIsInvalid needed after removing base

    /// <summary>
    /// Provides a managed wrapper for a process handle.
    /// </summary>
    [System.Security.SecurityCritical]
    public sealed partial class SafeProcessHandle : System.Runtime.InteropServices.SafeHandle
    {
        public override bool IsInvalid {[System.Security.SecurityCritical] get { return false; } }
    }
}
