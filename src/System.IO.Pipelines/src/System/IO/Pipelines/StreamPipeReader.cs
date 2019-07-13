// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;

namespace System.IO.Pipelines
{
    internal class StreamPipeReader : PipeReader
    {
        internal const int InitialSegmentPoolSize = 4; // 16K
        internal const int MaxSegmentPoolSize = 256; // 1MB

        private readonly int _bufferSize;
        private readonly int _minimumReadThreshold;
        private readonly MemoryPool<byte> _pool;

        private CancellationTokenSource _internalTokenSource;
        private bool _isReaderCompleted;
        private bool _isStreamCompleted;

        private BufferSegment _readHead;
        private int _readIndex;

        private BufferSegment _readTail;
        private long _bufferedBytes;
        private bool _examinedEverything;
        private readonly object _lock = new object();

        // Mutable struct! Don't make this readonly
        private BufferSegmentStack _bufferSegmentPool;
        private readonly bool _leaveOpen;
        private ValueTaskAsyncReader _asyncReader;
        private volatile bool _readInProgress;

        /// <summary>
        /// Creates a new StreamPipeReader.
        /// </summary>
        /// <param name="readingStream">The stream to read from.</param>
        /// <param name="options">The options to use.</param>
        public StreamPipeReader(Stream readingStream, StreamPipeReaderOptions options)
        {
            InnerStream = readingStream ?? throw new ArgumentNullException(nameof(readingStream));

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _bufferSegmentPool = new BufferSegmentStack(InitialSegmentPoolSize);
            _minimumReadThreshold = Math.Min(options.MinimumReadSize, options.BufferSize);
            _pool = options.Pool == MemoryPool<byte>.Shared ? null : options.Pool;
            _bufferSize = _pool == null ? options.BufferSize : Math.Min(options.BufferSize, _pool.MaxBufferSize);
            _leaveOpen = options.LeaveOpen;
        }

        /// <summary>
        /// Gets the inner stream that is being read from.
        /// </summary>
        public Stream InnerStream { get; }

        /// <inheritdoc />
        public override void AdvanceTo(SequencePosition consumed)
        {
            AdvanceTo(consumed, consumed);
        }

        private CancellationTokenSource InternalTokenSource
        {
            get
            {
                lock (_lock)
                {
                    if (_internalTokenSource == null)
                    {
                        _internalTokenSource = new CancellationTokenSource();
                    }
                    return _internalTokenSource;
                }
            }
        }

        /// <inheritdoc />
        public override void AdvanceTo(SequencePosition consumed, SequencePosition examined)
        {
            ThrowIfCompleted();

            AdvanceTo((BufferSegment)consumed.GetObject(), consumed.GetInteger(), (BufferSegment)examined.GetObject(), examined.GetInteger());
        }

        private void AdvanceTo(BufferSegment consumedSegment, int consumedIndex, BufferSegment examinedSegment, int examinedIndex)
        {
            if (consumedSegment == null || examinedSegment == null)
            {
                return;
            }

            if (_readHead == null)
            {
                ThrowHelper.ThrowInvalidOperationException_AdvanceToInvalidCursor();
            }

            BufferSegment returnStart = _readHead;
            BufferSegment returnEnd = consumedSegment;

            long consumedBytes = BufferSegment.GetLength(returnStart, _readIndex, consumedSegment, consumedIndex);

            _bufferedBytes -= consumedBytes;

            Debug.Assert(_bufferedBytes >= 0);

            _examinedEverything = false;

            if (examinedSegment == _readTail)
            {
                // If we examined everything, we force ReadAsync to actually read from the underlying stream
                // instead of returning a ReadResult from TryRead.
                _examinedEverything = examinedIndex == _readTail.End;
            }

            // Two cases here:
            // 1. All data is consumed. If so, we empty clear everything so we don't hold onto any 
            // excess memory.
            // 2. A segment is entirely consumed but there is still more data in nextSegments
            //  We are allowed to remove an extra segment. by setting returnEnd to be the next block.
            // 3. We are in the middle of a segment.
            //  Move _readHead and _readIndex to consumedSegment and index
            if (_bufferedBytes == 0)
            {
                returnEnd = null;
                _readHead = null;
                _readTail = null;
                _readIndex = 0;
            }
            else if (consumedIndex == returnEnd.Length)
            {
                BufferSegment nextBlock = returnEnd.NextSegment;
                _readHead = nextBlock;
                _readIndex = 0;
                returnEnd = nextBlock;
            }
            else
            {
                _readHead = consumedSegment;
                _readIndex = consumedIndex;
            }

            // Remove all blocks that are freed (except the last one)
            while (returnStart != returnEnd)
            {
                BufferSegment next = returnStart.NextSegment;
                returnStart.ResetMemory();
                ReturnSegmentUnsynchronized(returnStart);
                returnStart = next;
            }
        }

        /// <inheritdoc />
        public override void CancelPendingRead()
        {
            InternalTokenSource.Cancel();
        }

        /// <inheritdoc />
        public override void Complete(Exception exception = null)
        {
            if (_isReaderCompleted)
            {
                return;
            }

            _isReaderCompleted = true;
            _asyncReader?.Dispose();

            BufferSegment segment = _readHead;
            while (segment != null)
            {
                BufferSegment returnSegment = segment;
                segment = segment.NextSegment;

                returnSegment.ResetMemory();
            }

            if (!_leaveOpen)
            {
                InnerStream.Dispose();
            }
        }

        /// <inheritdoc />
        public override void OnWriterCompleted(Action<Exception, object> callback, object state)
        {
        }

        /// <inheritdoc />
        public override ValueTask<ReadResult> ReadAsync(CancellationToken cancellationToken = default)
        {
            bool isAsync = false;
            if (_readInProgress)
            {
                // Throw if there are overlapping reads.
                ThrowConcurrentReadsNotSupported();
            }
            _readInProgress = true;

            try
            {
                ThrowIfCompleted();

                // PERF: store InternalTokenSource locally to avoid querying it twice (which acquires a lock)
                CancellationTokenSource tokenSource = InternalTokenSource;
                if (TryReadInternal(tokenSource, out ReadResult readResult))
                {
                    return new ValueTask<ReadResult>(readResult);
                }

                if (_isStreamCompleted)
                {
                    return new ValueTask<ReadResult>(new ReadResult(buffer: default, isCanceled: false, isCompleted: true));
                }

                var reg = new CancellationTokenRegistration();
                if (cancellationToken.CanBeCanceled)
                {
                    reg = cancellationToken.UnsafeRegister(state => ((StreamPipeReader)state).Cancel(), this);
                }

                using (reg)
                {
                    try
                    {
                        AllocateReadTail();

                        Memory<byte> buffer = _readTail.AvailableMemory.Slice(_readTail.End);

                        ValueTask<int> resultTask = InnerStream.ReadAsync(buffer, tokenSource.Token);
                        int length;
                        if (resultTask.IsCompletedSuccessfully)
                        {
                            length = resultTask.Result;
                        }
                        else
                        {
                            isAsync = true;
                            ValueTaskAsyncReader asyncReader = (_asyncReader ??= new ValueTaskAsyncReader(this));
                            return asyncReader.AwaitTask(resultTask, tokenSource, cancellationToken);
                        }

                        Debug.Assert(length + _readTail.End <= _readTail.AvailableMemory.Length);

                        _readTail.End += length;
                        _bufferedBytes += length;

                        if (length == 0)
                        {
                            _isStreamCompleted = true;
                        }

                        return new ValueTask<ReadResult>(new ReadResult(GetCurrentReadOnlySequence(), isCanceled: false, _isStreamCompleted));
                    }
                    catch (OperationCanceledException oce)
                    {
                        ClearCancellationToken();

                        if (tokenSource.IsCancellationRequested && !cancellationToken.IsCancellationRequested)
                        {
                            // Catch cancellation and translate it into setting isCanceled = true
                            return new ValueTask<ReadResult>(new ReadResult(GetCurrentReadOnlySequence(), isCanceled: true, _isStreamCompleted));
                        }
                        else
                        {
                            return new ValueTask<ReadResult>(Task.FromException<ReadResult>(oce));
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                return new ValueTask<ReadResult>(Task.FromException<ReadResult>(ex));
            }
            finally
            {
                if (!isAsync)
                {
                    Debug.Assert(_readInProgress);
                    _readInProgress = false;
                }
            }
        }

        static void ThrowConcurrentReadsNotSupported()
        {
            throw new InvalidOperationException("Concurrent reads are not supported");
        }

        private void ClearCancellationToken()
        {
            lock (_lock)
            {
                _internalTokenSource = null;
            }
        }

        private void ThrowIfCompleted()
        {
            if (_isReaderCompleted)
            {
                ThrowHelper.ThrowInvalidOperationException_NoReadingAllowed();
            }
        }

        public override bool TryRead(out ReadResult result)
        {
            if (_readInProgress)
            {
                // Throw if there are overlapping reads.
                ThrowConcurrentReadsNotSupported();
            }

            ThrowIfCompleted();

            return TryReadInternal(InternalTokenSource, out result);
        }

        private bool TryReadInternal(CancellationTokenSource source, out ReadResult result)
        {
            bool isCancellationRequested = source.IsCancellationRequested;
            if (isCancellationRequested || _bufferedBytes > 0 && (!_examinedEverything || _isStreamCompleted))
            {
                if (isCancellationRequested)
                {
                    ClearCancellationToken();
                }

                ReadOnlySequence<byte> buffer = _readHead == null ? default : GetCurrentReadOnlySequence();

                result = new ReadResult(buffer, isCancellationRequested, _isStreamCompleted);
                return true;
            }

            result = new ReadResult();
            return false;
        }

        private ReadOnlySequence<byte> GetCurrentReadOnlySequence()
        {
            return new ReadOnlySequence<byte>(_readHead, _readIndex, _readTail, _readTail.End);
        }

        private void AllocateReadTail()
        {
            if (_readHead == null)
            {
                Debug.Assert(_readTail == null);
                _readHead = AllocateSegment();
                _readTail = _readHead;
            }
            else if (_readTail.WritableBytes < _minimumReadThreshold)
            {
                BufferSegment nextSegment = AllocateSegment();
                _readTail.SetNext(nextSegment);
                _readTail = nextSegment;
            }
        }

        private BufferSegment AllocateSegment()
        {
            BufferSegment nextSegment = CreateSegmentUnsynchronized();

            if (_pool is null)
            {
                nextSegment.SetOwnedMemory(ArrayPool<byte>.Shared.Rent(_bufferSize));
            }
            else
            {
                nextSegment.SetOwnedMemory(_pool.Rent(_bufferSize));
            }

            return nextSegment;
        }

        private BufferSegment CreateSegmentUnsynchronized()
        {
            if (_bufferSegmentPool.TryPop(out BufferSegment segment))
            {
                return segment;
            }

            return new BufferSegment();
        }

        private void ReturnSegmentUnsynchronized(BufferSegment segment)
        {
            Debug.Assert(segment != _readHead, "Returning _readHead segment that's in use!");
            Debug.Assert(segment != _readTail, "Returning _readTail segment that's in use!");

            if (_bufferSegmentPool.Count < MaxSegmentPoolSize)
            {
                _bufferSegmentPool.Push(segment);
            }
        }

        private void Cancel()
        {
            InternalTokenSource.Cancel();
        }

        private class ValueTaskAsyncReader : IValueTaskSource<ReadResult>, IDisposable
        {
            private readonly StreamPipeReader _reader;
            private readonly Task _readTask;
            private readonly CancellationTokenSource _disposeCts = new CancellationTokenSource();
            private readonly Gate _gate = new Gate();
            private ManualResetValueTaskSourceCore<ReadResult> _mrvts;

            private ValueTask<int> _valueTask;
            private CancellationToken _cancellationToken;
            private CancellationTokenSource _tokenSource;

            public ValueTaskAsyncReader(StreamPipeReader reader)
            {
                _reader = reader;
                _readTask = ReadAsync();
            }

            public ValueTask<ReadResult> AwaitTask(ValueTask<int> valueTask, CancellationTokenSource tokenSource, CancellationToken cancellationToken)
            {
                _valueTask = valueTask;
                _cancellationToken = cancellationToken;
                _tokenSource = tokenSource;
                _gate.Release();

                return new ValueTask<ReadResult>(this, _mrvts.Version);
            }

            private async Task ReadAsync()
            {
                while (!_disposeCts.IsCancellationRequested)
                {
                    try
                    {
                        await _gate.WaitAsync(_disposeCts.Token).ConfigureAwait(false);

                        int length = await _valueTask.ConfigureAwait(false);
                        _cancellationToken.ThrowIfCancellationRequested();

                        Debug.Assert(length + _reader._readTail.End <= _reader._readTail.AvailableMemory.Length);

                        _reader._readTail.End += length;
                        _reader._bufferedBytes += length;

                        if (length == 0)
                        {
                            _reader._isStreamCompleted = true;
                        }

                        _mrvts.SetResult(new ReadResult(_reader.GetCurrentReadOnlySequence(), isCanceled: false, _reader._isStreamCompleted));
                    }
                    catch (OperationCanceledException oce)
                    {
                        _reader.ClearCancellationToken();

                        if (_tokenSource.IsCancellationRequested && !_cancellationToken.IsCancellationRequested)
                        {
                            // Catch cancellation and translate it into setting isCanceled = true
                            _mrvts.SetResult(new ReadResult(_reader.GetCurrentReadOnlySequence(), isCanceled: true, _reader._isStreamCompleted));
                        }
                        else
                        {
                            _mrvts.SetException(oce);
                        }
                    }
                    catch (Exception ex)
                    {
                        _mrvts.SetException(ex);
                    }
                }

                _disposeCts.Dispose();
            }

            ReadResult IValueTaskSource<ReadResult>.GetResult(short token)
            {
                ReadResult result = _mrvts.GetResult(token);

                _valueTask = default;
                _cancellationToken = default;
                _tokenSource = null;
                _mrvts.Reset();

                Debug.Assert(_reader._readInProgress);
                _reader._readInProgress = false;

                return result;
            }

            ValueTaskSourceStatus IValueTaskSource<ReadResult>.GetStatus(short token)
                => _mrvts.GetStatus(token);

            void IValueTaskSource<ReadResult>.OnCompleted(Action<object> continuation, object state, short token, ValueTaskSourceOnCompletedFlags flags)
                => _mrvts.OnCompleted(continuation, state, token, flags);

            public void Dispose() => _disposeCts.Cancel();
        }
    }
}
