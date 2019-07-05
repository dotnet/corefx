// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Diagnostics;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;

namespace System.IO.Pipelines
{
    internal class StreamPipeReader : PipeReader, IValueTaskSource<ReadResult>
    {
        internal const int InitialSegmentPoolSize = 4; // 16K
        internal const int MaxSegmentPoolSize = 256; // 1MB

        private readonly int _bufferSize;
        private readonly int _minimumReadThreshold;
        private readonly MemoryPool<byte> _pool;

        private bool _isReaderCompleted;
        private bool _isStreamCompleted;
        private PipeAwaitable _awaitable;
        private Task _streamReadTask = Task.CompletedTask;
        private ExceptionDispatchInfo _edi;
        private readonly CancellationTokenSource _completeCts = new CancellationTokenSource();

        private BufferSegment _readHead;
        private int _readIndex;

        private BufferSegment _readTail;
        private long _bufferedBytes;
        private object _lock = new object();

        // Mutable struct! Don't make this readonly
        private BufferSegmentStack _bufferSegmentPool;
        private bool _leaveOpen;

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
            _awaitable = new PipeAwaitable(completed: false, useSynchronizationContext: true);
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

        /// <inheritdoc />
        public override void AdvanceTo(SequencePosition consumed, SequencePosition examined)
        {
            ThrowIfCompleted();

            AdvanceTo((BufferSegment)consumed.GetObject(), consumed.GetInteger(), (BufferSegment)examined.GetObject(), examined.GetInteger());
        }

        private void AdvanceTo(BufferSegment consumedSegment, int consumedIndex, BufferSegment examinedSegment, int examinedIndex)
        {
            if (consumedSegment == null || examinedSegment == null || !_streamReadTask.IsCompleted)
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

            var examinedEverything = false;

            if (examinedSegment == _readTail)
            {
                // If we examined everything, we force ReadAsync to actually read from the underlying stream
                // instead of returning a ReadResult from TryRead.
                examinedEverything = examinedIndex == _readTail.End;
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

            if (examinedEverything)
            {
                _awaitable.SetUncompleted();
            }
        }

        /// <inheritdoc />
        public override void CancelPendingRead()
        {
            CompletionData completionData;
            lock (_lock)
            {
                _awaitable.Cancel(out completionData);
            }

            DispatchCompletion(completionData);
        }

        /// <inheritdoc />
        public override void Complete(Exception exception = null)
        {
            if (_isReaderCompleted)
            {
                return;
            }

            _isReaderCompleted = true;

            // Make an attempt to cancel any call to Stream.ReadAsync
            _completeCts.Cancel();

            if (!_leaveOpen)
            {
                InnerStream.Dispose();
            }

            // Return the memory after potentially disposing the stream
            BufferSegment segment = _readHead;
            while (segment != null)
            {
                BufferSegment returnSegment = segment;
                segment = segment.NextSegment;

                returnSegment.ResetMemory();
            }

            _completeCts.Dispose();
        }

        /// <inheritdoc />
        public override void OnWriterCompleted(Action<Exception, object> callback, object state)
        {
        }

        /// <inheritdoc />
        public override ValueTask<ReadResult> ReadAsync(CancellationToken cancellationToken = default)
        {
            // TODO ReadyAsync needs to throw if there are overlapping reads.
            ThrowIfCompleted();

            if (TryReadInternal(out ReadResult readResult))
            {
                return new ValueTask<ReadResult>(readResult);
            }

            if (_isStreamCompleted)
            {
                return new ValueTask<ReadResult>(new ReadResult(buffer: default, isCanceled: false, isCompleted: true));
            }

            if (_streamReadTask.IsCompleted)
            {
                _streamReadTask = ReadStreamAsync(cancellationToken);

                // Completed the stream read inline because it was synchronous and there was no exception thrown
                if (_streamReadTask.IsCompleted && _edi == null)
                {
                    return new ValueTask<ReadResult>(GetReadResult());
                }
            }

            return new ValueTask<ReadResult>(this, 0);
        }

        private async Task ReadStreamAsync(CancellationToken cancellationToken)
        {
            CancellationTokenSource cts = null;
            CancellationToken effectiveToken;

            if (cancellationToken.CanBeCanceled)
            {
                cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _completeCts.Token);
                effectiveToken = cts.Token;
            }
            else
            {
                effectiveToken = _completeCts.Token;
            }

            using (cts)
            {
                try
                {
                    AllocateReadTail();

                    Memory<byte> buffer = _readTail.AvailableMemory.Slice(_readTail.End);

                    int length = await InnerStream.ReadAsync(buffer, effectiveToken).ConfigureAwait(false);

                    Debug.Assert(length + _readTail.End <= _readTail.AvailableMemory.Length);

                    _readTail.End += length;
                    _bufferedBytes += length;

                    if (length == 0)
                    {
                        _isStreamCompleted = true;
                    }
                }
                catch (Exception ex)
                {
                    _edi = ExceptionDispatchInfo.Capture(ex);
                }
            }

            CompletionData completionData;

            lock (_lock)
            {
                _awaitable.Complete(out completionData);
            }

            DispatchCompletion(completionData);
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
            ThrowIfCompleted();

            return TryReadInternal(out result);
        }

        private bool TryReadInternal(out ReadResult result)
        {
            if (_awaitable.IsCompleted || (_bufferedBytes > 0 && (_awaitable.IsCompleted || _isStreamCompleted)))
            {
                result = GetReadResult();
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

        private ReadResult GetReadResult()
        {
            var isCancellationRequested = _awaitable.ObserveCancellation();

            ReadOnlySequence<byte> buffer = _readHead == null ? default : GetCurrentReadOnlySequence();

            return new ReadResult(buffer, isCancellationRequested, _isStreamCompleted);
        }

        public ReadResult GetResult(short token)
        {
            ExceptionDispatchInfo edi = _edi;
            _edi = null;
            edi?.Throw();

            return GetReadResult();
        }

        public ValueTaskSourceStatus GetStatus(short token)
        {
            if (_awaitable.IsCompleted)
            {
                if (_edi != null)
                {
                    return ValueTaskSourceStatus.Faulted;
                }

                return ValueTaskSourceStatus.Succeeded;
            }
            return ValueTaskSourceStatus.Pending;
        }

        public void OnCompleted(Action<object> continuation, object state, short token, ValueTaskSourceOnCompletedFlags flags)
        {
            CompletionData completionData;
            bool doubleCompletion;

            lock (_lock)
            {
                _awaitable.OnCompleted(continuation, state, flags, out completionData, out doubleCompletion);
            }

            DispatchCompletion(completionData);
        }

        private static void DispatchCompletion(in CompletionData completionData)
        {
            if (completionData.Completion is null)
            {
                return;
            }

            PipeScheduler.ThreadPool.UnsafeSchedule(completionData.Completion, completionData.CompletionState);
        }
    }
}
