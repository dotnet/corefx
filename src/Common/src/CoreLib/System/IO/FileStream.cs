// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;
using System.Diagnostics;

namespace System.IO
{
    public partial class FileStream : Stream
    {
        private const FileShare DefaultShare = FileShare.Read;
        private const bool DefaultIsAsync = false;
        internal const int DefaultBufferSize = 4096;

        private byte[] _buffer;
        private int _bufferLength;
        private readonly SafeFileHandle _fileHandle;

        /// <summary>Whether the file is opened for reading, writing, or both.</summary>
        private readonly FileAccess _access;

        /// <summary>The path to the opened file.</summary>
        private readonly string _path;

        /// <summary>The next available byte to be read from the _buffer.</summary>
        private int _readPos;

        /// <summary>The number of valid bytes in _buffer.</summary>
        private int _readLength;

        /// <summary>The next location in which a write should occur to the buffer.</summary>
        private int _writePos;

        /// <summary>
        /// Whether asynchronous read/write/flush operations should be performed using async I/O.
        /// On Windows FileOptions.Asynchronous controls how the file handle is configured, 
        /// and then as a result how operations are issued against that file handle.  On Unix, 
        /// there isn't any distinction around how file descriptors are created for async vs 
        /// sync, but we still differentiate how the operations are issued in order to provide
        /// similar behavioral semantics and performance characteristics as on Windows.  On
        /// Windows, if non-async, async read/write requests just delegate to the base stream,
        /// and no attempt is made to synchronize between sync and async operations on the stream;
        /// if async, then async read/write requests are implemented specially, and sync read/write
        /// requests are coordinated with async ones by implementing the sync ones over the async
        /// ones.  On Unix, we do something similar.  If non-async, async read/write requests just
        /// delegate to the base stream, and no attempt is made to synchronize.  If async, we use
        /// a semaphore to coordinate both sync and async operations.
        /// </summary>
        private readonly bool _useAsyncIO;

        /// <summary>cached task for read ops that complete synchronously</summary>
        private Task<int> _lastSynchronouslyCompletedTask = null;

        /// <summary>
        /// Currently cached position in the stream.  This should always mirror the underlying file's actual position,
        /// and should only ever be out of sync if another stream with access to this same file manipulates it, at which
        /// point we attempt to error out.
        /// </summary>
        private long _filePosition;

        /// <summary>Whether the file stream's handle has been exposed.</summary>
        private bool _exposedHandle;

        /// <summary>Caches whether Serialization Guard has been disabled for file writes</summary>
        private static int s_cachedSerializationSwitch = 0;

        [Obsolete("This constructor has been deprecated.  Please use new FileStream(SafeFileHandle handle, FileAccess access) instead.  https://go.microsoft.com/fwlink/?linkid=14202")]
        public FileStream(IntPtr handle, FileAccess access)
            : this(handle, access, true, DefaultBufferSize, false)
        {
        }

        [Obsolete("This constructor has been deprecated.  Please use new FileStream(SafeFileHandle handle, FileAccess access) instead, and optionally make a new SafeFileHandle with ownsHandle=false if needed.  https://go.microsoft.com/fwlink/?linkid=14202")]
        public FileStream(IntPtr handle, FileAccess access, bool ownsHandle)
            : this(handle, access, ownsHandle, DefaultBufferSize, false)
        {
        }

        [Obsolete("This constructor has been deprecated.  Please use new FileStream(SafeFileHandle handle, FileAccess access, int bufferSize) instead, and optionally make a new SafeFileHandle with ownsHandle=false if needed.  https://go.microsoft.com/fwlink/?linkid=14202")]
        public FileStream(IntPtr handle, FileAccess access, bool ownsHandle, int bufferSize)
            : this(handle, access, ownsHandle, bufferSize, false)
        {
        }

        [Obsolete("This constructor has been deprecated.  Please use new FileStream(SafeFileHandle handle, FileAccess access, int bufferSize, bool isAsync) instead, and optionally make a new SafeFileHandle with ownsHandle=false if needed.  https://go.microsoft.com/fwlink/?linkid=14202")]
        public FileStream(IntPtr handle, FileAccess access, bool ownsHandle, int bufferSize, bool isAsync)
        {
            SafeFileHandle safeHandle = new SafeFileHandle(handle, ownsHandle: ownsHandle);
            try
            {
                ValidateAndInitFromHandle(safeHandle, access, bufferSize, isAsync);
            }
            catch
            {
                // We don't want to take ownership of closing passed in handles
                // *unless* the constructor completes successfully.
                GC.SuppressFinalize(safeHandle);

                // This would also prevent Close from being called, but is unnecessary
                // as we've removed the object from the finalizer queue.
                //
                // safeHandle.SetHandleAsInvalid();
                throw;
            }

            // Note: Cleaner to set the following fields in ValidateAndInitFromHandle,
            // but we can't as they're readonly.
            _access = access;
            _useAsyncIO = isAsync;

            // As the handle was passed in, we must set the handle field at the very end to
            // avoid the finalizer closing the handle when we throw errors.
            _fileHandle = safeHandle;
        }

        public FileStream(SafeFileHandle handle, FileAccess access)
            : this(handle, access, DefaultBufferSize)
        {
        }

        public FileStream(SafeFileHandle handle, FileAccess access, int bufferSize)
            : this(handle, access, bufferSize, GetDefaultIsAsync(handle))
        {
        }

        private void ValidateAndInitFromHandle(SafeFileHandle handle, FileAccess access, int bufferSize, bool isAsync)
        {
            if (handle.IsInvalid)
                throw new ArgumentException(SR.Arg_InvalidHandle, nameof(handle));

            if (access < FileAccess.Read || access > FileAccess.ReadWrite)
                throw new ArgumentOutOfRangeException(nameof(access), SR.ArgumentOutOfRange_Enum);
            if (bufferSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(bufferSize), SR.ArgumentOutOfRange_NeedPosNum);

            if (handle.IsClosed)
                throw new ObjectDisposedException(SR.ObjectDisposed_FileClosed);
            if (handle.IsAsync.HasValue && isAsync != handle.IsAsync.GetValueOrDefault())
                throw new ArgumentException(SR.Arg_HandleNotAsync, nameof(handle));

            _exposedHandle = true;
            _bufferLength = bufferSize;

            InitFromHandle(handle, access, isAsync);
        }

        public FileStream(SafeFileHandle handle, FileAccess access, int bufferSize, bool isAsync)
        {
            ValidateAndInitFromHandle(handle, access, bufferSize, isAsync);

            // Note: Cleaner to set the following fields in ValidateAndInitFromHandle,
            // but we can't as they're readonly.
            _access = access;
            _useAsyncIO = isAsync;

            // As the handle was passed in, we must set the handle field at the very end to
            // avoid the finalizer closing the handle when we throw errors.
            _fileHandle = handle;
        }

        public FileStream(string path, FileMode mode) :
            this(path, mode, (mode == FileMode.Append ? FileAccess.Write : FileAccess.ReadWrite), DefaultShare, DefaultBufferSize, DefaultIsAsync)
        { }

        public FileStream(string path, FileMode mode, FileAccess access) :
            this(path, mode, access, DefaultShare, DefaultBufferSize, DefaultIsAsync)
        { }

        public FileStream(string path, FileMode mode, FileAccess access, FileShare share) :
            this(path, mode, access, share, DefaultBufferSize, DefaultIsAsync)
        { }

        public FileStream(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize) :
            this(path, mode, access, share, bufferSize, DefaultIsAsync)
        { }

        public FileStream(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize, bool useAsync) :
            this(path, mode, access, share, bufferSize, useAsync ? FileOptions.Asynchronous : FileOptions.None)
        { }

        public FileStream(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize, FileOptions options)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path), SR.ArgumentNull_Path);
            if (path.Length == 0)
                throw new ArgumentException(SR.Argument_EmptyPath, nameof(path));

            // don't include inheritable in our bounds check for share
            FileShare tempshare = share & ~FileShare.Inheritable;
            string badArg = null;

            if (mode < FileMode.CreateNew || mode > FileMode.Append)
                badArg = nameof(mode);
            else if (access < FileAccess.Read || access > FileAccess.ReadWrite)
                badArg = nameof(access);
            else if (tempshare < FileShare.None || tempshare > (FileShare.ReadWrite | FileShare.Delete))
                badArg = nameof(share);

            if (badArg != null)
                throw new ArgumentOutOfRangeException(badArg, SR.ArgumentOutOfRange_Enum);

            // NOTE: any change to FileOptions enum needs to be matched here in the error validation
            if (options != FileOptions.None && (options & ~(FileOptions.WriteThrough | FileOptions.Asynchronous | FileOptions.RandomAccess | FileOptions.DeleteOnClose | FileOptions.SequentialScan | FileOptions.Encrypted | (FileOptions)0x20000000 /* NoBuffering */)) != 0)
                throw new ArgumentOutOfRangeException(nameof(options), SR.ArgumentOutOfRange_Enum);

            if (bufferSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(bufferSize), SR.ArgumentOutOfRange_NeedPosNum);

            // Write access validation
            if ((access & FileAccess.Write) == 0)
            {
                if (mode == FileMode.Truncate || mode == FileMode.CreateNew || mode == FileMode.Create || mode == FileMode.Append)
                {
                    // No write access, mode and access disagree but flag access since mode comes first
                    throw new ArgumentException(SR.Format(SR.Argument_InvalidFileModeAndAccessCombo, mode, access), nameof(access));
                }
            }

            if ((access & FileAccess.Read) != 0 && mode == FileMode.Append)
                throw new ArgumentException(SR.Argument_InvalidAppendMode, nameof(access));

            string fullPath = Path.GetFullPath(path);

            _path = fullPath;
            _access = access;
            _bufferLength = bufferSize;

            if ((options & FileOptions.Asynchronous) != 0)
                _useAsyncIO = true;

            if ((access & FileAccess.Write) == FileAccess.Write)
            {
                SerializationInfo.ThrowIfDeserializationInProgress("AllowFileWrites", ref s_cachedSerializationSwitch);
            }

            _fileHandle = OpenHandle(mode, share, options);

            try
            {
                Init(mode, share, path);
            }
            catch
            {
                // If anything goes wrong while setting up the stream, make sure we deterministically dispose
                // of the opened handle.
                _fileHandle.Dispose();
                _fileHandle = null;
                throw;
            }
        }

        [Obsolete("This property has been deprecated.  Please use FileStream's SafeFileHandle property instead.  https://go.microsoft.com/fwlink/?linkid=14202")]
        public virtual IntPtr Handle { get { return SafeFileHandle.DangerousGetHandle(); } }

        public virtual void Lock(long position, long length)
        {
            if (position < 0 || length < 0)
            {
                throw new ArgumentOutOfRangeException(position < 0 ? nameof(position) : nameof(length), SR.ArgumentOutOfRange_NeedNonNegNum);
            }

            if (_fileHandle.IsClosed)
            {
                throw Error.GetFileNotOpen();
            }

            LockInternal(position, length);
        }

        public virtual void Unlock(long position, long length)
        {
            if (position < 0 || length < 0)
            {
                throw new ArgumentOutOfRangeException(position < 0 ? nameof(position) : nameof(length), SR.ArgumentOutOfRange_NeedNonNegNum);
            }

            if (_fileHandle.IsClosed)
            {
                throw Error.GetFileNotOpen();
            }

            UnlockInternal(position, length);
        }

        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            // If we have been inherited into a subclass, the following implementation could be incorrect
            // since it does not call through to Flush() which a subclass might have overridden.  To be safe 
            // we will only use this implementation in cases where we know it is safe to do so,
            // and delegate to our base class (which will call into Flush) when we are not sure.
            if (GetType() != typeof(FileStream))
                return base.FlushAsync(cancellationToken);

            return FlushAsyncInternal(cancellationToken);
        }

        public override int Read(byte[] array, int offset, int count)
        {
            ValidateReadWriteArgs(array, offset, count);
            return _useAsyncIO ?
                ReadAsyncTask(array, offset, count, CancellationToken.None).GetAwaiter().GetResult() :
                ReadSpan(new Span<byte>(array, offset, count));
        }

        public override int Read(Span<byte> buffer)
        {
            if (GetType() == typeof(FileStream) && !_useAsyncIO)
            {
                if (_fileHandle.IsClosed)
                {
                    throw Error.GetFileNotOpen();
                }
                return ReadSpan(buffer);
            }
            else
            {
                // This type is derived from FileStream and/or the stream is in async mode.  If this is a 
                // derived type, it may have overridden Read(byte[], int, int) prior to this Read(Span<byte>)
                // overload being introduced.  In that case, this Read(Span<byte>) overload should use the behavior
                // of Read(byte[],int,int) overload.  Or if the stream is in async mode, we can't call the
                // synchronous ReadSpan, so we similarly call the base Read, which will turn delegate to
                // Read(byte[],int,int), which will do the right thing if we're in async mode.
                return base.Read(buffer);
            }
        }

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer), SR.ArgumentNull_Buffer);
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (buffer.Length - offset < count)
                throw new ArgumentException(SR.Argument_InvalidOffLen /*, no good single parameter name to pass*/);

            // If we have been inherited into a subclass, the following implementation could be incorrect
            // since it does not call through to Read() which a subclass might have overridden.  
            // To be safe we will only use this implementation in cases where we know it is safe to do so,
            // and delegate to our base class (which will call into Read/ReadAsync) when we are not sure.
            // Similarly, if we weren't opened for asynchronous I/O, call to the base implementation so that
            // Read is invoked asynchronously.
            if (GetType() != typeof(FileStream) || !_useAsyncIO)
                return base.ReadAsync(buffer, offset, count, cancellationToken);

            if (cancellationToken.IsCancellationRequested)
                return Task.FromCanceled<int>(cancellationToken);

            if (IsClosed)
                throw Error.GetFileNotOpen();

            return ReadAsyncTask(buffer, offset, count, cancellationToken);
        }

        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            if (!_useAsyncIO || GetType() != typeof(FileStream))
            {
                // If we're not using async I/O, delegate to the base, which will queue a call to Read.
                // Or if this isn't a concrete FileStream, a derived type may have overridden ReadAsync(byte[],...),
                // which was introduced first, so delegate to the base which will delegate to that.
                return base.ReadAsync(buffer, cancellationToken);
            }

            if (cancellationToken.IsCancellationRequested)
            {
                return new ValueTask<int>(Task.FromCanceled<int>(cancellationToken));
            }

            if (IsClosed)
            {
                throw Error.GetFileNotOpen();
            }

            Task<int> t = ReadAsyncInternal(buffer, cancellationToken, out int synchronousResult);
            return t != null ?
                new ValueTask<int>(t) :
                new ValueTask<int>(synchronousResult);
        }

        private Task<int> ReadAsyncTask(byte[] array, int offset, int count, CancellationToken cancellationToken)
        {
            Task<int> t = ReadAsyncInternal(new Memory<byte>(array, offset, count), cancellationToken, out int synchronousResult);

            if (t == null)
            {
                t = _lastSynchronouslyCompletedTask;
                Debug.Assert(t == null || t.IsCompletedSuccessfully, "Cached task should have completed successfully");

                if (t == null || t.Result != synchronousResult)
                {
                    _lastSynchronouslyCompletedTask = t = Task.FromResult(synchronousResult);
                }
            }

            return t;
        }

        public override void Write(byte[] array, int offset, int count)
        {
            ValidateReadWriteArgs(array, offset, count);
            if (_useAsyncIO)
            {
                WriteAsyncInternal(new ReadOnlyMemory<byte>(array, offset, count), CancellationToken.None).GetAwaiter().GetResult();
            }
            else
            {
                WriteSpan(new ReadOnlySpan<byte>(array, offset, count));
            }
        }

        public override void Write(ReadOnlySpan<byte> buffer)
        {
            if (GetType() == typeof(FileStream) && !_useAsyncIO)
            {
                if (_fileHandle.IsClosed)
                {
                    throw Error.GetFileNotOpen();
                }
                WriteSpan(buffer);
            }
            else
            {
                // This type is derived from FileStream and/or the stream is in async mode.  If this is a 
                // derived type, it may have overridden Write(byte[], int, int) prior to this Write(ReadOnlySpan<byte>)
                // overload being introduced.  In that case, this Write(ReadOnlySpan<byte>) overload should use the behavior
                // of Write(byte[],int,int) overload.  Or if the stream is in async mode, we can't call the
                // synchronous WriteSpan, so we similarly call the base Write, which will turn delegate to
                // Write(byte[],int,int), which will do the right thing if we're in async mode.
                base.Write(buffer);
            }
        }

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer), SR.ArgumentNull_Buffer);
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (buffer.Length - offset < count)
                throw new ArgumentException(SR.Argument_InvalidOffLen /*, no good single parameter name to pass*/);

            // If we have been inherited into a subclass, the following implementation could be incorrect
            // since it does not call through to Write() or WriteAsync() which a subclass might have overridden.  
            // To be safe we will only use this implementation in cases where we know it is safe to do so,
            // and delegate to our base class (which will call into Write/WriteAsync) when we are not sure.
            if (!_useAsyncIO || GetType() != typeof(FileStream))
                return base.WriteAsync(buffer, offset, count, cancellationToken);

            if (cancellationToken.IsCancellationRequested)
                return Task.FromCanceled(cancellationToken);

            if (IsClosed)
                throw Error.GetFileNotOpen();

            return WriteAsyncInternal(new ReadOnlyMemory<byte>(buffer, offset, count), cancellationToken).AsTask();
        }

        public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        {
            if (!_useAsyncIO || GetType() != typeof(FileStream))
            {
                // If we're not using async I/O, delegate to the base, which will queue a call to Write.
                // Or if this isn't a concrete FileStream, a derived type may have overridden WriteAsync(byte[],...),
                // which was introduced first, so delegate to the base which will delegate to that.
                return base.WriteAsync(buffer, cancellationToken);
            }

            if (cancellationToken.IsCancellationRequested)
            {
                return new ValueTask(Task.FromCanceled<int>(cancellationToken));
            }

            if (IsClosed)
            {
                throw Error.GetFileNotOpen();
            }

            return WriteAsyncInternal(buffer, cancellationToken);
        }

        /// <summary>
        /// Clears buffers for this stream and causes any buffered data to be written to the file.
        /// </summary>
        public override void Flush()
        {
            // Make sure that we call through the public virtual API
            Flush(flushToDisk: false);
        }

        /// <summary>
        /// Clears buffers for this stream, and if <param name="flushToDisk"/> is true, 
        /// causes any buffered data to be written to the file.
        /// </summary>
        public virtual void Flush(bool flushToDisk)
        {
            if (IsClosed) throw Error.GetFileNotOpen();

            FlushInternalBuffer();

            if (flushToDisk && CanWrite)
            {
                FlushOSBuffer();
            }
        }

        /// <summary>Gets a value indicating whether the current stream supports reading.</summary>
        public override bool CanRead
        {
            get { return !_fileHandle.IsClosed && (_access & FileAccess.Read) != 0; }
        }

        /// <summary>Gets a value indicating whether the current stream supports writing.</summary>
        public override bool CanWrite
        {
            get { return !_fileHandle.IsClosed && (_access & FileAccess.Write) != 0; }
        }

        /// <summary>Validates arguments to Read and Write and throws resulting exceptions.</summary>
        /// <param name="array">The buffer to read from or write to.</param>
        /// <param name="offset">The zero-based offset into the array.</param>
        /// <param name="count">The maximum number of bytes to read or write.</param>
        private void ValidateReadWriteArgs(byte[] array, int offset, int count)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array), SR.ArgumentNull_Buffer);
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (array.Length - offset < count)
                throw new ArgumentException(SR.Argument_InvalidOffLen /*, no good single parameter name to pass*/);
            if (_fileHandle.IsClosed)
                throw Error.GetFileNotOpen();
        }

        /// <summary>Sets the length of this stream to the given value.</summary>
        /// <param name="value">The new length of the stream.</param>
        public override void SetLength(long value)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (_fileHandle.IsClosed)
                throw Error.GetFileNotOpen();
            if (!CanSeek)
                throw Error.GetSeekNotSupported();
            if (!CanWrite)
                throw Error.GetWriteNotSupported();

            SetLengthInternal(value);
        }

        public virtual SafeFileHandle SafeFileHandle
        {
            get
            {
                Flush();
                _exposedHandle = true;
                return _fileHandle;
            }
        }

        /// <summary>Gets the path that was passed to the constructor.</summary>
        public virtual string Name { get { return _path ?? SR.IO_UnknownFileName; } }

        /// <summary>Gets a value indicating whether the stream was opened for I/O to be performed synchronously or asynchronously.</summary>
        public virtual bool IsAsync
        {
            get { return _useAsyncIO; }
        }

        /// <summary>Gets the length of the stream in bytes.</summary>
        public override long Length
        {
            get
            {
                if (_fileHandle.IsClosed) throw Error.GetFileNotOpen();
                if (!CanSeek) throw Error.GetSeekNotSupported();
                return GetLengthInternal();
            }
        }

        /// <summary>
        /// Verify that the actual position of the OS's handle equals what we expect it to.
        /// This will fail if someone else moved the UnixFileStream's handle or if
        /// our position updating code is incorrect.
        /// </summary>
        private void VerifyOSHandlePosition()
        {
            bool verifyPosition = _exposedHandle; // in release, only verify if we've given out the handle such that someone else could be manipulating it
#if DEBUG
            verifyPosition = true; // in debug, always make sure our position matches what the OS says it should be
#endif
            if (verifyPosition && CanSeek)
            {
                long oldPos = _filePosition; // SeekCore will override the current _position, so save it now
                long curPos = SeekCore(_fileHandle, 0, SeekOrigin.Current);
                if (oldPos != curPos)
                {
                    // For reads, this is non-fatal but we still could have returned corrupted 
                    // data in some cases, so discard the internal buffer. For writes, 
                    // this is a problem; discard the buffer and error out.
                    _readPos = _readLength = 0;
                    if (_writePos > 0)
                    {
                        _writePos = 0;
                        throw new IOException(SR.IO_FileStreamHandlePosition);
                    }
                }
            }
        }

        /// <summary>Verifies that state relating to the read/write buffer is consistent.</summary>
        [Conditional("DEBUG")]
        private void AssertBufferInvariants()
        {
            // Read buffer values must be in range: 0 <= _bufferReadPos <= _bufferReadLength <= _bufferLength
            Debug.Assert(0 <= _readPos && _readPos <= _readLength && _readLength <= _bufferLength);

            // Write buffer values must be in range: 0 <= _bufferWritePos <= _bufferLength
            Debug.Assert(0 <= _writePos && _writePos <= _bufferLength);

            // Read buffering and write buffering can't both be active
            Debug.Assert((_readPos == 0 && _readLength == 0) || _writePos == 0);
        }

        /// <summary>Validates that we're ready to read from the stream.</summary>
        private void PrepareForReading()
        {
            if (_fileHandle.IsClosed)
                throw Error.GetFileNotOpen();
            if (_readLength == 0 && !CanRead)
                throw Error.GetReadNotSupported();

            AssertBufferInvariants();
        }

        /// <summary>Gets or sets the position within the current stream</summary>
        public override long Position
        {
            get
            {
                if (_fileHandle.IsClosed)
                    throw Error.GetFileNotOpen();

                if (!CanSeek)
                    throw Error.GetSeekNotSupported();

                AssertBufferInvariants();
                VerifyOSHandlePosition();

                // We may have read data into our buffer from the handle, such that the handle position
                // is artificially further along than the consumer's view of the stream's position.
                // Thus, when reading, our position is really starting from the handle position negatively
                // offset by the number of bytes in the buffer and positively offset by the number of
                // bytes into that buffer we've read.  When writing, both the read length and position
                // must be zero, and our position is just the handle position offset positive by how many
                // bytes we've written into the buffer.
                return (_filePosition - _readLength) + _readPos + _writePos;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), SR.ArgumentOutOfRange_NeedNonNegNum);

                Seek(value, SeekOrigin.Begin);
            }
        }

        internal virtual bool IsClosed => _fileHandle.IsClosed;

        private static bool IsIoRelatedException(Exception e) =>
            // These all derive from IOException
            //     DirectoryNotFoundException
            //     DriveNotFoundException
            //     EndOfStreamException
            //     FileLoadException
            //     FileNotFoundException
            //     PathTooLongException
            //     PipeException 
            e is IOException ||
            // Note that SecurityException is only thrown on runtimes that support CAS
            // e is SecurityException || 
            e is UnauthorizedAccessException ||
            e is NotSupportedException ||
            (e is ArgumentException && !(e is ArgumentNullException));

        /// <summary>
        /// Gets the array used for buffering reading and writing.  
        /// If the array hasn't been allocated, this will lazily allocate it.
        /// </summary>
        /// <returns>The buffer.</returns>
        private byte[] GetBuffer()
        {
            Debug.Assert(_buffer == null || _buffer.Length == _bufferLength);
            if (_buffer == null)
            {
                _buffer = new byte[_bufferLength];
                OnBufferAllocated();
            }

            return _buffer;
        }

        partial void OnBufferAllocated();

        /// <summary>
        /// Flushes the internal read/write buffer for this stream.  If write data has been buffered,
        /// that data is written out to the underlying file.  Or if data has been buffered for 
        /// reading from the stream, the data is dumped and our position in the underlying file 
        /// is rewound as necessary.  This does not flush the OS buffer.
        /// </summary>
        private void FlushInternalBuffer()
        {
            AssertBufferInvariants();
            if (_writePos > 0)
            {
                FlushWriteBuffer();
            }
            else if (_readPos < _readLength && CanSeek)
            {
                FlushReadBuffer();
            }
        }

        /// <summary>Dumps any read data in the buffer and rewinds our position in the stream, accordingly, as necessary.</summary>
        private void FlushReadBuffer()
        {
            // Reading is done by blocks from the file, but someone could read
            // 1 byte from the buffer then write.  At that point, the OS's file
            // pointer is out of sync with the stream's position.  All write 
            // functions should call this function to preserve the position in the file.

            AssertBufferInvariants();
            Debug.Assert(_writePos == 0, "FileStream: Write buffer must be empty in FlushReadBuffer!");

            int rewind = _readPos - _readLength;
            if (rewind != 0)
            {
                Debug.Assert(CanSeek, "FileStream will lose buffered read data now.");
                SeekCore(_fileHandle, rewind, SeekOrigin.Current);
            }
            _readPos = _readLength = 0;
        }

        /// <summary>
        /// Reads a byte from the file stream.  Returns the byte cast to an int
        /// or -1 if reading from the end of the stream.
        /// </summary>
        public override int ReadByte()
        {
            PrepareForReading();

            byte[] buffer = GetBuffer();
            if (_readPos == _readLength)
            {
                FlushWriteBuffer();
                _readLength = FillReadBufferForReadByte();
                _readPos = 0;
                if (_readLength == 0)
                {
                    return -1;
                }
            }

            return buffer[_readPos++];
        }

        /// <summary>
        /// Writes a byte to the current position in the stream and advances the position
        /// within the stream by one byte.
        /// </summary>
        /// <param name="value">The byte to write to the stream.</param>
        public override void WriteByte(byte value)
        {
            PrepareForWriting();

            // Flush the write buffer if it's full
            if (_writePos == _bufferLength)
                FlushWriteBufferForWriteByte();

            // We now have space in the buffer. Store the byte.
            GetBuffer()[_writePos++] = value;
        }

        /// <summary>
        /// Validates that we're ready to write to the stream,
        /// including flushing a read buffer if necessary.
        /// </summary>
        private void PrepareForWriting()
        {
            if (_fileHandle.IsClosed)
                throw Error.GetFileNotOpen();

            // Make sure we're good to write.  We only need to do this if there's nothing already
            // in our write buffer, since if there is something in the buffer, we've already done 
            // this checking and flushing.
            if (_writePos == 0)
            {
                if (!CanWrite) throw Error.GetWriteNotSupported();
                FlushReadBuffer();
                Debug.Assert(_bufferLength > 0, "_bufferSize > 0");
            }
        }

        ~FileStream()
        {
            // Preserved for compatibility since FileStream has defined a 
            // finalizer in past releases and derived classes may depend
            // on Dispose(false) call.
            Dispose(false);
        }

        public override IAsyncResult BeginRead(byte[] array, int offset, int numBytes, AsyncCallback callback, object state)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (numBytes < 0)
                throw new ArgumentOutOfRangeException(nameof(numBytes), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (array.Length - offset < numBytes)
                throw new ArgumentException(SR.Argument_InvalidOffLen);

            if (IsClosed) throw new ObjectDisposedException(SR.ObjectDisposed_FileClosed);
            if (!CanRead) throw new NotSupportedException(SR.NotSupported_UnreadableStream);

            if (!IsAsync)
                return base.BeginRead(array, offset, numBytes, callback, state);
            else
                return TaskToApm.Begin(ReadAsyncTask(array, offset, numBytes, CancellationToken.None), callback, state);
        }

        public override IAsyncResult BeginWrite(byte[] array, int offset, int numBytes, AsyncCallback callback, object state)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (numBytes < 0)
                throw new ArgumentOutOfRangeException(nameof(numBytes), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (array.Length - offset < numBytes)
                throw new ArgumentException(SR.Argument_InvalidOffLen);

            if (IsClosed) throw new ObjectDisposedException(SR.ObjectDisposed_FileClosed);
            if (!CanWrite) throw new NotSupportedException(SR.NotSupported_UnwritableStream);

            if (!IsAsync)
                return base.BeginWrite(array, offset, numBytes, callback, state);
            else
                return TaskToApm.Begin(WriteAsyncInternal(new ReadOnlyMemory<byte>(array, offset, numBytes), CancellationToken.None).AsTask(), callback, state);
        }

        public override int EndRead(IAsyncResult asyncResult)
        {
            if (asyncResult == null)
                throw new ArgumentNullException(nameof(asyncResult));

            if (!IsAsync)
                return base.EndRead(asyncResult);
            else
                return TaskToApm.End<int>(asyncResult);
        }

        public override void EndWrite(IAsyncResult asyncResult)
        {
            if (asyncResult == null)
                throw new ArgumentNullException(nameof(asyncResult));

            if (!IsAsync)
                base.EndWrite(asyncResult);
            else
                TaskToApm.End(asyncResult);
        }
    }
}
