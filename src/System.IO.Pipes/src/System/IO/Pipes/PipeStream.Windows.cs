// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;

namespace System.IO.Pipes
{
    public abstract partial class PipeStream : Stream
    {
        private static readonly Task<int> ZeroTask = Task.FromResult(0);
        internal const bool CheckOperationsRequiresSetHandle = true;
        internal ThreadPoolBoundHandle _threadPoolBinding;

        internal static string GetPipePath(string serverName, string pipeName)
        {
            string normalizedPipePath = Path.GetFullPath(@"\\" + serverName + @"\pipe\" + pipeName);
            if (String.Equals(normalizedPipePath, @"\\.\pipe\anonymous", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentOutOfRangeException("pipeName", SR.ArgumentOutOfRange_AnonymousReserved);
            }
            return normalizedPipePath;
        }

        /// <summary>Throws an exception if the supplied handle does not represent a valid pipe.</summary>
        /// <param name="safePipeHandle">The handle to validate.</param>
        internal void ValidateHandleIsPipe(SafePipeHandle safePipeHandle)
        {
            // Check that this handle is infact a handle to a pipe.
            if (Interop.mincore.GetFileType(safePipeHandle) != Interop.mincore.FileTypes.FILE_TYPE_PIPE)
            {
                throw new IOException(SR.IO_InvalidPipeHandle);
            }
        }

        /// <summary>Initializes the handle to be used asynchronously.</summary>
        /// <param name="handle">The handle.</param>
        private void InitializeAsyncHandle(SafePipeHandle handle)
        {
            // If the handle is of async type, bind the handle to the ThreadPool so that we can use 
            // the async operations (it's needed so that our native callbacks get called).
            _threadPoolBinding = ThreadPoolBoundHandle.BindHandle(handle);
        }

        private void UninitializeAsyncHandle()
        {
            if (_threadPoolBinding != null)
                _threadPoolBinding.Dispose();
        }

        [SecurityCritical]
        private unsafe int ReadCore(byte[] buffer, int offset, int count)
        {
            Debug.Assert(_handle != null, "_handle is null");
            Debug.Assert(!_handle.IsClosed, "_handle is closed");
            Debug.Assert(CanRead, "can't read");
            Debug.Assert(buffer != null, "buffer is null");
            Debug.Assert(offset >= 0, "offset is negative");
            Debug.Assert(count >= 0, "count is negative");

            if (_isAsync)
            {
                return ReadAsyncCorePrivate(buffer, offset, count, CancellationToken.None).GetAwaiter().GetResult();
            }

            int errorCode = 0;
            int r = ReadFileNative(_handle, buffer, offset, count, null, out errorCode);

            if (r == -1)
            {
                // If the other side has broken the connection, set state to Broken and return 0
                if (errorCode == Interop.mincore.Errors.ERROR_BROKEN_PIPE ||
                    errorCode == Interop.mincore.Errors.ERROR_PIPE_NOT_CONNECTED)
                {
                    State = PipeState.Broken;
                    r = 0;
                }
                else
                {
                    throw Win32Marshal.GetExceptionForWin32Error(errorCode, String.Empty);
                }
            }
            _isMessageComplete = (errorCode != Interop.mincore.Errors.ERROR_MORE_DATA);

            Debug.Assert(r >= 0, "PipeStream's ReadCore is likely broken.");

            return r;
        }

        [SecuritySafeCritical]
        private Task<int> ReadAsyncCore(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            if (!CanRead)
            {
                throw __Error.GetReadNotSupported();
            }

            return ReadAsyncCorePrivate(buffer, offset, count, cancellationToken);
        }

        [SecurityCritical]
        private unsafe void WriteCore(byte[] buffer, int offset, int count)
        {
            Debug.Assert(_handle != null, "_handle is null");
            Debug.Assert(!_handle.IsClosed, "_handle is closed");
            Debug.Assert(CanWrite, "can't write");
            Debug.Assert(buffer != null, "buffer is null");
            Debug.Assert(offset >= 0, "offset is negative");
            Debug.Assert(count >= 0, "count is negative");

            if (_isAsync)
            {
                WriteAsyncCorePrivate(buffer, offset, count, CancellationToken.None).GetAwaiter().GetResult();
                return;
            }

            int errorCode = 0;
            int r = WriteFileNative(_handle, buffer, offset, count, null, out errorCode);

            if (r == -1)
            {
                throw WinIOError(errorCode);
            }
            Debug.Assert(r >= 0, "PipeStream's WriteCore is likely broken.");
            return;
        }

        [SecuritySafeCritical]
        private Task WriteAsyncCore(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            if (!CanWrite)
            {
                throw __Error.GetWriteNotSupported();
            }

            return WriteAsyncCorePrivate(buffer, offset, count, cancellationToken);
        }

        // Blocks until the other end of the pipe has read in all written buffer.
        [SecurityCritical]
        public void WaitForPipeDrain()
        {
            CheckWriteOperations();
            if (!CanWrite)
            {
                throw __Error.GetWriteNotSupported();
            }

            // Block until other end of the pipe has read everything.
            if (!Interop.mincore.FlushFileBuffers(_handle))
            {
                throw WinIOError(Marshal.GetLastWin32Error());
            }
        }

        // Gets the transmission mode for the pipe.  This is virtual so that subclassing types can 
        // override this in cases where only one mode is legal (such as anonymous pipes)
        public virtual PipeTransmissionMode TransmissionMode
        {
            [SecurityCritical]
            [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands", Justification = "Security model of pipes: demand at creation but no subsequent demands")]
            get
            {
                CheckPipePropertyOperations();

                if (_isFromExistingHandle)
                {
                    int pipeFlags;
                    if (!Interop.mincore.GetNamedPipeInfo(_handle, out pipeFlags, IntPtr.Zero, IntPtr.Zero,
                            IntPtr.Zero))
                    {
                        throw WinIOError(Marshal.GetLastWin32Error());
                    }
                    if ((pipeFlags & Interop.mincore.PipeOptions.PIPE_TYPE_MESSAGE) != 0)
                    {
                        return PipeTransmissionMode.Message;
                    }
                    else
                    {
                        return PipeTransmissionMode.Byte;
                    }
                }
                else
                {
                    return _transmissionMode;
                }
            }
        }

        // Gets the buffer size in the inbound direction for the pipe. This checks if pipe has read
        // access. If that passes, call to GetNamedPipeInfo will succeed.
        public virtual int InBufferSize
        {
            [SecurityCritical]
            [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
            get
            {
                CheckPipePropertyOperations();
                if (!CanRead)
                {
                    throw new NotSupportedException(SR.NotSupported_UnreadableStream);
                }

                int inBufferSize;
                if (!Interop.mincore.GetNamedPipeInfo(_handle, IntPtr.Zero, IntPtr.Zero, out inBufferSize, IntPtr.Zero))
                {
                    throw WinIOError(Marshal.GetLastWin32Error());
                }

                return inBufferSize;
            }
        }

        // Gets the buffer size in the outbound direction for the pipe. This uses cached version 
        // if it's an outbound only pipe because GetNamedPipeInfo requires read access to the pipe.
        // However, returning cached is good fallback, especially if user specified a value in 
        // the ctor.
        public virtual int OutBufferSize
        {
            [SecurityCritical]
            [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands", Justification = "Security model of pipes: demand at creation but no subsequent demands")]
            get
            {
                CheckPipePropertyOperations();
                if (!CanWrite)
                {
                    throw new NotSupportedException(SR.NotSupported_UnwritableStream);
                }

                int outBufferSize;

                // Use cached value if direction is out; otherwise get fresh version
                if (_pipeDirection == PipeDirection.Out)
                {
                    outBufferSize = _outBufferSize;
                }
                else if (!Interop.mincore.GetNamedPipeInfo(_handle, IntPtr.Zero, out outBufferSize,
                    IntPtr.Zero, IntPtr.Zero))
                {
                    throw WinIOError(Marshal.GetLastWin32Error());
                }

                return outBufferSize;
            }
        }

        public virtual PipeTransmissionMode ReadMode
        {
            [SecurityCritical]
            get
            {
                CheckPipePropertyOperations();

                // get fresh value if it could be stale
                if (_isFromExistingHandle || IsHandleExposed)
                {
                    UpdateReadMode();
                }
                return _readMode;
            }
            [SecurityCritical]
            [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands", Justification = "Security model of pipes: demand at creation but no subsequent demands")]
            set
            {
                // Nothing fancy here.  This is just a wrapper around the Win32 API.  Note, that NamedPipeServerStream
                // and the AnonymousPipeStreams override this.

                CheckPipePropertyOperations();
                if (value < PipeTransmissionMode.Byte || value > PipeTransmissionMode.Message)
                {
                    throw new ArgumentOutOfRangeException("value", SR.ArgumentOutOfRange_TransmissionModeByteOrMsg);
                }

                unsafe
                {
                    int pipeReadType = (int)value << 1;
                    if (!Interop.mincore.SetNamedPipeHandleState(_handle, &pipeReadType, IntPtr.Zero, IntPtr.Zero))
                    {
                        throw WinIOError(Marshal.GetLastWin32Error());
                    }
                    else
                    {
                        _readMode = value;
                    }
                }
            }
        }

        // -----------------------------
        // ---- PAL layer ends here ----
        // -----------------------------

        [SecurityCritical]
        private unsafe Task WriteAsyncCorePrivate(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            Debug.Assert(_handle != null, "_handle is null");
            Debug.Assert(!_handle.IsClosed, "_handle is closed");
            Debug.Assert(CanWrite, "can't write");
            Debug.Assert(buffer != null, "buffer == null");
            Debug.Assert(_isAsync, "WriteAsyncCorePrivate doesn't work on synchronous file streams!");
            Debug.Assert(offset >= 0, "offset is negative");
            Debug.Assert(count >= 0, "count is negative");

            // fixed doesn't work well with zero length arrays. Set the zero-byte flag in case
            // caller needs to do any cleanup
            if (buffer.Length == 0)
            {
                return Task.CompletedTask;
            }
            else
            {
                var completionSource = new PipeStreamCompletionSource(this, buffer, cancellationToken, isWrite: true);
                int errorCode = 0;

                // Queue an async WriteFile operation and pass in a packed overlapped
                int r = WriteFileNative(_handle, buffer, offset, count, completionSource.Overlapped, out errorCode);

                // WriteFile, the OS version, will return 0 on failure, but this WriteFileNative 
                // wrapper returns -1. This will return the following:
                // - On error, r==-1.
                // - On async requests that are still pending, r==-1 w/ hr==ERROR_IO_PENDING
                // - On async requests that completed sequentially, r==0
                // 
                // You will NEVER RELIABLY be able to get the number of buffer written back from this 
                // call when using overlapped structures!  You must not pass in a non-null 
                // lpNumBytesWritten to WriteFile when using overlapped structures!  This is by design 
                // NT behavior.
                if (r == -1 && errorCode != Interop.mincore.Errors.ERROR_IO_PENDING)
                {
                    completionSource.ReleaseResources();
                    throw WinIOError(errorCode);
                }

                completionSource.RegisterForCancellation();
                return completionSource.Task;
            }
        }

        [SecurityCritical]
        private unsafe int ReadFileNative(SafePipeHandle handle, byte[] buffer, int offset, int count,
                NativeOverlapped* overlapped, out int errorCode)
        {
            Debug.Assert(handle != null, "handle is null");
            Debug.Assert(offset >= 0, "offset is negative");
            Debug.Assert(count >= 0, "count is negative");
            Debug.Assert(buffer != null, "buffer == null");
            Debug.Assert((_isAsync && overlapped != null) || (!_isAsync && overlapped == null), "Async IO parameter screwup in call to ReadFileNative.");
            Debug.Assert(buffer.Length - offset >= count, "offset + count >= buffer length");

            // You can't use the fixed statement on an array of length 0. Note that async callers
            // check to avoid calling this first, so they can call user's callback
            if (buffer.Length == 0)
            {
                errorCode = 0;
                return 0;
            }

            int r = 0;
            int numBytesRead = 0;

            fixed (byte* p = buffer)
            {
                if (_isAsync)
                {
                    r = Interop.mincore.ReadFile(handle, p + offset, count, IntPtr.Zero, overlapped);
                }
                else
                {
                    r = Interop.mincore.ReadFile(handle, p + offset, count, out numBytesRead, IntPtr.Zero);
                }
            }

            if (r == 0)
            {
                errorCode = Marshal.GetLastWin32Error();

                // In message mode, the ReadFile can inform us that there is more data to come.
                if (errorCode == Interop.mincore.Errors.ERROR_MORE_DATA)
                {
                    return numBytesRead;
                }

                return -1;
            }
            else
            {
                errorCode = 0;
            }

            return numBytesRead;
        }

        [SecurityCritical]
        private unsafe int WriteFileNative(SafePipeHandle handle, byte[] buffer, int offset, int count,
                NativeOverlapped* overlapped, out int errorCode)
        {
            Debug.Assert(handle != null, "handle is null");
            Debug.Assert(offset >= 0, "offset is negative");
            Debug.Assert(count >= 0, "count is negative");
            Debug.Assert(buffer != null, "buffer == null");
            Debug.Assert((_isAsync && overlapped != null) || (!_isAsync && overlapped == null), "Async IO parameter screwup in call to WriteFileNative.");
            Debug.Assert(buffer.Length - offset >= count, "offset + count >= buffer length");

            // You can't use the fixed statement on an array of length 0. Note that async callers
            // check to avoid calling this first, so they can call user's callback
            if (buffer.Length == 0)
            {
                errorCode = 0;
                return 0;
            }

            int numBytesWritten = 0;
            int r = 0;

            fixed (byte* p = buffer)
            {
                if (_isAsync)
                {
                    r = Interop.mincore.WriteFile(handle, p + offset, count, IntPtr.Zero, overlapped);
                }
                else
                {
                    r = Interop.mincore.WriteFile(handle, p + offset, count, out numBytesWritten, IntPtr.Zero);
                }
            }

            if (r == 0)
            {
                errorCode = Marshal.GetLastWin32Error();
                return -1;
            }
            else
            {
                errorCode = 0;
            }

            return numBytesWritten;
        }

        [SecurityCritical]
        private unsafe Task<int> ReadAsyncCorePrivate(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            Debug.Assert(_handle != null, "_handle is null");
            Debug.Assert(!_handle.IsClosed, "_handle is closed");
            Debug.Assert(CanRead, "can't read");
            Debug.Assert(buffer != null, "buffer == null");
            Debug.Assert(_isAsync, "ReadAsyncCorePrivate doesn't work on synchronous file streams!");
            Debug.Assert(offset >= 0, "offset is negative");
            Debug.Assert(count >= 0, "count is negative");

            // handle zero-length buffers separately; fixed keyword ReadFileNative doesn't like
            // 0-length buffers. Call user callback and we're done
            if (buffer.Length == 0)
            {
                UpdateMessageCompletion(false);
                return ZeroTask;
            }
            else
            {
                var completionSource = new PipeStreamCompletionSource(this, buffer, cancellationToken, isWrite: false);

                // Queue an async ReadFile operation and pass in a packed overlapped
                int errorCode = 0;
                int r = ReadFileNative(_handle, buffer, offset, count, completionSource.Overlapped, out errorCode);

                // ReadFile, the OS version, will return 0 on failure, but this ReadFileNative wrapper
                // returns -1. This will return the following:
                // - On error, r==-1.
                // - On async requests that are still pending, r==-1 w/ hr==ERROR_IO_PENDING
                // - On async requests that completed sequentially, r==0
                // 
                // You will NEVER RELIABLY be able to get the number of buffer read back from this call 
                // when using overlapped structures!  You must not pass in a non-null lpNumBytesRead to
                // ReadFile when using overlapped structures!  This is by design NT behavior.
                if (r == -1)
                {
                    switch (errorCode)
                    {
                        // One side has closed its handle or server disconnected.
                        // Set the state to Broken and do some cleanup work
                        case Interop.mincore.Errors.ERROR_BROKEN_PIPE:
                        case Interop.mincore.Errors.ERROR_PIPE_NOT_CONNECTED:
                            State = PipeState.Broken;

                            // Clear the overlapped status bit for this special case. Failure to do so looks 
                            // like we are freeing a pending overlapped.
                            completionSource.Overlapped->InternalLow = IntPtr.Zero;
                            completionSource.ReleaseResources();
                            completionSource.SetCompletedSynchronously();
                            UpdateMessageCompletion(true);
                            return ZeroTask;

                        case Interop.mincore.Errors.ERROR_IO_PENDING:
                            break;

                        default:
                            throw Win32Marshal.GetExceptionForWin32Error(errorCode);
                    }
                }

                completionSource.RegisterForCancellation();
                return completionSource.Task;
            }
        }

        [SecurityCritical]
        internal static Interop.mincore.SECURITY_ATTRIBUTES GetSecAttrs(HandleInheritability inheritability)
        {
            Interop.mincore.SECURITY_ATTRIBUTES secAttrs = default(Interop.mincore.SECURITY_ATTRIBUTES);
            if ((inheritability & HandleInheritability.Inheritable) != 0)
            {
                secAttrs = new Interop.mincore.SECURITY_ATTRIBUTES();
                secAttrs.nLength = (uint)Marshal.SizeOf(secAttrs);
                secAttrs.bInheritHandle = true;
            }
            return secAttrs;
        }

        internal void UpdateMessageCompletion(bool completion)
        {
            // Set message complete to true because the pipe is broken as well.
            // Need this to signal to readers to stop reading.
            _isMessageComplete = (_state == PipeState.Broken || completion);
        }

        /// <summary>
        /// Determine pipe read mode from Win32 
        /// </summary>
        [SecurityCritical]
        private void UpdateReadMode()
        {
            int flags;
            if (!Interop.mincore.GetNamedPipeHandleState(SafePipeHandle, out flags, IntPtr.Zero, IntPtr.Zero,
                    IntPtr.Zero, IntPtr.Zero, 0))
            {
                throw WinIOError(Marshal.GetLastWin32Error());
            }

            if ((flags & Interop.mincore.PipeOptions.PIPE_READMODE_MESSAGE) != 0)
            {
                _readMode = PipeTransmissionMode.Message;
            }
            else
            {
                _readMode = PipeTransmissionMode.Byte;
            }
        }

        /// <summary>
        /// Filter out all pipe related errors and do some cleanup before calling __Error.WinIOError.
        /// </summary>
        /// <param name="errorCode"></param>
        [SecurityCritical]
        internal Exception WinIOError(int errorCode)
        {
            switch (errorCode)
            {
                case Interop.mincore.Errors.ERROR_BROKEN_PIPE:
                case Interop.mincore.Errors.ERROR_PIPE_NOT_CONNECTED:
                case Interop.mincore.Errors.ERROR_NO_DATA:
                    // Other side has broken the connection
                    _state = PipeState.Broken;
                    return new IOException(SR.IO_PipeBroken, Win32Marshal.MakeHRFromErrorCode(errorCode));

                case Interop.mincore.Errors.ERROR_HANDLE_EOF:
                    return __Error.GetEndOfFile();

                case Interop.mincore.Errors.ERROR_INVALID_HANDLE:
                    // For invalid handles, detect the error and mark our handle
                    // as invalid to give slightly better error messages.  Also
                    // help ensure we avoid handle recycling bugs.
                    _handle.SetHandleAsInvalid();
                    _state = PipeState.Broken;
                    break;
            }

            return Win32Marshal.GetExceptionForWin32Error(errorCode);
        }
    }
}
