// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using System.Security;
using System.Threading;

namespace System.IO.Pipes
{
    /// <summary>
    /// Named pipe client. Use this to open the client end of a named pipes created with 
    /// NamedPipeServerStream.
    /// </summary>
    public sealed partial class NamedPipeClientStream : PipeStream
    {
        [SecurityCritical]
        private bool TryConnect(int timeout, CancellationToken cancellationToken)
        {
            // timeout and cancellationToken are currently ignored: [ActiveIssue(812, PlatformID.AnyUnix)]
            // We should figure out if there's a good way to cancel calls to Open, such as
            // by sending a signal that causes an EINTR, and then in handling the EINTR result
            // poll the cancellationToken to see if cancellation was requested.

            try
            {
                // Open the file.  For In or Out, this will block until a client has connected.
                // Unfortunately for InOut it won't, which is different from the Windows behavior;
                // on Unix it won't block for InOut until it actually performs a read or write operation.
                var clientHandle = Microsoft.Win32.SafeHandles.SafePipeHandle.Open(
                    _normalizedPipePath, 
                    TranslateFlags(_direction, _pipeOptions, _inheritability),
                    (int)Interop.Sys.Permissions.S_IRWXU);

                // Pipe successfully opened.  Store our client handle.
                InitializeHandle(clientHandle, isExposed: false, isAsync: (_pipeOptions & PipeOptions.Asynchronous) != 0);
                State = PipeState.Connected;
                return true;
            }
            catch (FileNotFoundException)
            {
                // The FIFO file doesn't yet exist.
                return false;
            }
        }

        public int NumberOfServerInstances
        {
            [SecurityCritical]
            [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands", Justification = "Security model of pipes: demand at creation but no subsequent demands")]
            get
            {
                CheckPipePropertyOperations();
                throw new PlatformNotSupportedException(); // no way to determine this accurately
            }
        }

        // -----------------------------
        // ---- PAL layer ends here ----
        // -----------------------------

    }
}
