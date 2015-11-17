// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Win32.SafeHandles;
using System.Diagnostics.CodeAnalysis;
using System.Security;
using System.Threading;
using System.Threading.Tasks;

namespace System.IO.Pipes
{
    /// <summary>
    /// Named pipe server
    /// </summary>
    public sealed partial class NamedPipeServerStream : PipeStream
    {
        // Use the maximum number of server instances that the system resources allow
        private const int MaxAllowedServerInstances = -1;

        [SecuritySafeCritical]
        public NamedPipeServerStream(String pipeName)
            : this(pipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Byte, PipeOptions.None, 0, 0, HandleInheritability.None)
        {
        }

        [SecuritySafeCritical]
        public NamedPipeServerStream(String pipeName, PipeDirection direction)
            : this(pipeName, direction, 1, PipeTransmissionMode.Byte, PipeOptions.None, 0, 0, HandleInheritability.None)
        {
        }

        [SecuritySafeCritical]
        public NamedPipeServerStream(String pipeName, PipeDirection direction, int maxNumberOfServerInstances)
            : this(pipeName, direction, maxNumberOfServerInstances, PipeTransmissionMode.Byte, PipeOptions.None, 0, 0, HandleInheritability.None)
        {
        }

        [SecuritySafeCritical]
        public NamedPipeServerStream(String pipeName, PipeDirection direction, int maxNumberOfServerInstances, PipeTransmissionMode transmissionMode)
            : this(pipeName, direction, maxNumberOfServerInstances, transmissionMode, PipeOptions.None, 0, 0, HandleInheritability.None)
        {
        }

        [SecuritySafeCritical]
        public NamedPipeServerStream(String pipeName, PipeDirection direction, int maxNumberOfServerInstances, PipeTransmissionMode transmissionMode, PipeOptions options)
            : this(pipeName, direction, maxNumberOfServerInstances, transmissionMode, options, 0, 0, HandleInheritability.None)
        { 
        }

        [SecuritySafeCritical]
        public NamedPipeServerStream(String pipeName, PipeDirection direction, int maxNumberOfServerInstances, PipeTransmissionMode transmissionMode, PipeOptions options, int inBufferSize, int outBufferSize)
            : this(pipeName, direction, maxNumberOfServerInstances, transmissionMode, options, inBufferSize, outBufferSize, HandleInheritability.None)
        { 
        }

        /// <summary>
        /// Full named pipe server constructor
        /// </summary>
        /// <param name="pipeName">Pipe name</param>
        /// <param name="direction">Pipe direction: In, Out or InOut (duplex). 
        /// Win32 note: this gets OR'd into dwOpenMode to CreateNamedPipe
        /// </param>
        /// <param name="maxNumberOfServerInstances">Maximum number of server instances. Specify a fixed value between 
        /// 1 and 254 (Windows)/greater than 1 (Unix), or use NamedPipeServerStream.MaxAllowedServerInstances to use the 
        /// maximum amount allowed by system resources.</param>
        /// <param name="transmissionMode">Byte mode or message mode.
        /// Win32 note: this gets used for dwPipeMode. CreateNamedPipe allows you to specify PIPE_TYPE_BYTE/MESSAGE
        /// and PIPE_READMODE_BYTE/MESSAGE independently, but this sets type and readmode to match.
        /// </param>
        /// <param name="options">PipeOption enum: None, Asynchronous, or Writethrough
        /// Win32 note: this gets passed in with dwOpenMode to CreateNamedPipe. Asynchronous corresponds to 
        /// FILE_FLAG_OVERLAPPED option. PipeOptions enum doesn't expose FIRST_PIPE_INSTANCE option because
        /// this sets that automatically based on the number of instances specified.
        /// </param>
        /// <param name="inBufferSize">Incoming buffer size, 0 or higher.
        /// Note: this size is always advisory; OS uses a suggestion.
        /// </param>
        /// <param name="outBufferSize">Outgoing buffer size, 0 or higher (see above)</param>
        /// <param name="pipeSecurity">PipeSecurity, or null for default security descriptor</param>
        /// <param name="inheritability">Whether handle is inheritable</param>
        /// <param name="additionalAccessRights">Combination (logical OR) of PipeAccessRights.TakeOwnership, 
        /// PipeAccessRights.AccessSystemSecurity, and PipeAccessRights.ChangePermissions</param>
        [SecuritySafeCritical]
        private NamedPipeServerStream(String pipeName, PipeDirection direction, int maxNumberOfServerInstances,
                PipeTransmissionMode transmissionMode, PipeOptions options, int inBufferSize, int outBufferSize,
                HandleInheritability inheritability)
            : base(direction, transmissionMode, outBufferSize)
        {
            if (pipeName == null)
            {
                throw new ArgumentNullException("pipeName");
            }
            if (pipeName.Length == 0)
            {
                throw new ArgumentException(SR.Argument_NeedNonemptyPipeName);
            }
            if ((options & ~(PipeOptions.WriteThrough | PipeOptions.Asynchronous)) != 0)
            {
                throw new ArgumentOutOfRangeException("options", SR.ArgumentOutOfRange_OptionsInvalid);
            }
            if (inBufferSize < 0)
            {
                throw new ArgumentOutOfRangeException("inBufferSize", SR.ArgumentOutOfRange_NeedNonNegNum);
            }
            ValidateMaxNumberOfServerInstances(maxNumberOfServerInstances);
            // inheritability will always be None since this private constructor is only called from other constructors from which
            // inheritability is always set to None. Desktop has a public constructor to allow setting it to something else, but Core
            // doesnt.
            if (inheritability < HandleInheritability.None || inheritability > HandleInheritability.Inheritable)
            {
                throw new ArgumentOutOfRangeException("inheritability", SR.ArgumentOutOfRange_HandleInheritabilityNoneOrInheritable);
            }

            Create(pipeName, direction, maxNumberOfServerInstances, transmissionMode,
                options, inBufferSize, outBufferSize, inheritability);
        }

        // Create a NamedPipeServerStream from an existing server pipe handle.
        [SecuritySafeCritical]
        public NamedPipeServerStream(PipeDirection direction, bool isAsync, bool isConnected, SafePipeHandle safePipeHandle)
            : base(direction, PipeTransmissionMode.Byte, 0)
        {
            if (safePipeHandle == null)
            {
                throw new ArgumentNullException("safePipeHandle");
            }
            if (safePipeHandle.IsInvalid)
            {
                throw new ArgumentException(SR.Argument_InvalidHandle, "safePipeHandle");
            }
            ValidateHandleIsPipe(safePipeHandle);
            
            InitializeHandle(safePipeHandle, true, isAsync);

            if (isConnected)
            {
                State = PipeState.Connected;
            }
        }

        ~NamedPipeServerStream()
        {
            Dispose(false);
        }

        public Task WaitForConnectionAsync()
        {
            return WaitForConnectionAsync(CancellationToken.None);
        }

        // Server can only connect from Disconnected state
        [SecurityCritical]
        [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands", Justification = "Consistent with security model")]
        private void CheckConnectOperationsServer()
        {
            // we're not checking whether already connected; this allows us to throw IOException
            // "pipe is being closed" if other side is closing (as does win32) or no-op if
            // already connected

            if (State == PipeState.Closed)
            {
                throw Error.GetPipeNotOpen();
            }
            if (InternalHandle != null && InternalHandle.IsClosed) // only check IsClosed if we have a handle
            {
                throw Error.GetPipeNotOpen();
            }
            if (State == PipeState.Broken)
            {
                throw new IOException(SR.IO_PipeBroken);
            }
        }

        // Server is allowed to disconnect from connected and broken states
        [SecurityCritical]
        private void CheckDisconnectOperations()
        {
            if (State == PipeState.WaitingToConnect)
            {
                throw new InvalidOperationException(SR.InvalidOperation_PipeNotYetConnected);
            }
            if (State == PipeState.Disconnected)
            {
                throw new InvalidOperationException(SR.InvalidOperation_PipeAlreadyDisconnected);
            }
            if (InternalHandle == null && CheckOperationsRequiresSetHandle)
            {
                throw new InvalidOperationException(SR.InvalidOperation_PipeHandleNotSet);
            }
            if ((State == PipeState.Closed) || (InternalHandle != null && InternalHandle.IsClosed))
            {
                throw Error.GetPipeNotOpen();
            }
        }
    }

#if RunAs
    // Users will use this delegate to specify a method to call while impersonating the client 
    // (see NamedPipeServerStream.RunAsClient).
    public delegate void PipeStreamImpersonationWorker();
#endif
}
