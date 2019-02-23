// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;

namespace System.IO.Pipelines
{
    /// <summary>
    /// Default <see cref="PipeWriter"/> and <see cref="PipeReader"/> implementation.
    /// </summary>
    public sealed partial class Pipe
    {
        internal const int SegmentPoolSize = 16;

        private static readonly Action<object> s_signalReaderAwaitable = state => ((Pipe)state).ReaderCancellationRequested();
        private static readonly Action<object> s_signalWriterAwaitable = state => ((Pipe)state).WriterCancellationRequested();
        private static readonly Action<object> s_invokeCompletionCallbacks = state => ((PipeCompletionCallbacks)state).Execute();

        // These callbacks all point to the same methods but are different delegate types
        private static readonly ContextCallback s_executionContextRawCallback = ExecuteWithoutExecutionContext;
        private static readonly SendOrPostCallback s_syncContextExecutionContextCallback = ExecuteWithExecutionContext;
        private static readonly SendOrPostCallback s_syncContextExecuteWithoutExecutionContextCallback = ExecuteWithoutExecutionContext;
        private static readonly Action<object> s_scheduleWithExecutionContextCallback = ExecuteWithExecutionContext;

        // This sync objects protects the following state:
        // 1. _commitHead & _commitHeadIndex
        // 2. _length
        // 3. _readerAwaitable & _writerAwaitable
        private readonly object _sync = new object();

        private readonly MemoryPool<byte> _pool;
        private readonly int _minimumSegmentSize;
        private readonly long _pauseWriterThreshold;
        private readonly long _resumeWriterThreshold;

        private readonly PipeScheduler _readerScheduler;
        private readonly PipeScheduler _writerScheduler;

        private int _pooledSegmentCount;
        private readonly SegmentAsValue[] _bufferSegmentPool;

        private readonly DefaultPipeReader _reader;
        private readonly DefaultPipeWriter _writer;

        private readonly bool _useSynchronizationContext;

        private long _length;
        private long _currentWriteLength;

        private PipeAwaitable _readerAwaitable;
        private PipeAwaitable _writerAwaitable;

        private PipeCompletion _writerCompletion;
        private PipeCompletion _readerCompletion;

        // The read head which is the extent of the IPipelineReader's consumed bytes
        private BufferSegment _readHead;
        private int _readHeadIndex;

        // The commit head which is the extent of the bytes available to the IPipelineReader to consume
        private BufferSegment _readTail;
        private int _readTailIndex;

        // The write head which is the extent of the IPipelineWriter's written bytes
        private BufferSegment _writingHead;
        private Memory<byte> _writingMemory;
        private int _buffered;

        private PipeOperationState _operationState;

        private bool _disposed;

        internal long Length => _length;

        /// <summary>
        /// Initializes the <see cref="Pipe"/> using <see cref="PipeOptions.Default"/> as options.
        /// </summary>
        public Pipe() : this(PipeOptions.Default)
        {
        }

        /// <summary>
        /// Initializes the <see cref="Pipe"/> with the specified <see cref="PipeOptions"/>.
        /// </summary>
        public Pipe(PipeOptions options)
        {
            if (options == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.options);
            }

            _bufferSegmentPool = new SegmentAsValue[SegmentPoolSize];

            _operationState = default;
            _readerCompletion = default;
            _writerCompletion = default;

            // If we're using the default pool then mark it as null since we're just going to use the 
            // array pool under the covers
            _pool = options.Pool == MemoryPool<byte>.Shared ? null : options.Pool;
            _minimumSegmentSize = options.MinimumSegmentSize;
            _pauseWriterThreshold = options.PauseWriterThreshold;
            _resumeWriterThreshold = options.ResumeWriterThreshold;
            _readerScheduler = options.ReaderScheduler;
            _writerScheduler = options.WriterScheduler;
            _useSynchronizationContext = options.UseSynchronizationContext;
            _readerAwaitable = new PipeAwaitable(completed: false, _useSynchronizationContext);
            _writerAwaitable = new PipeAwaitable(completed: true, _useSynchronizationContext);
            _reader = new DefaultPipeReader(this);
            _writer = new DefaultPipeWriter(this);
        }

        private void ResetState()
        {
            _readerCompletion.Reset();
            _writerCompletion.Reset();
            _readerAwaitable = new PipeAwaitable(completed: false, _useSynchronizationContext);
            _writerAwaitable = new PipeAwaitable(completed: true, _useSynchronizationContext);
            _readTailIndex = 0;
            _readHeadIndex = 0;
            _currentWriteLength = 0;
            _length = 0;
        }

        internal Memory<byte> GetMemory(int sizeHint)
        {
            if (_writerCompletion.IsCompleted)
            {
                ThrowHelper.ThrowInvalidOperationException_NoWritingAllowed();
            }

            if (sizeHint < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.minimumSize);
            }

            AllocateWriteHeadIfNeeded(sizeHint);

            return _writingMemory;
        }

        internal Span<byte> GetSpan(int sizeHint)
        {
            if (_writerCompletion.IsCompleted)
            {
                ThrowHelper.ThrowInvalidOperationException_NoWritingAllowed();
            }

            if (sizeHint < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.minimumSize);
            }

            AllocateWriteHeadIfNeeded(sizeHint);

            return _writingMemory.Span;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AllocateWriteHeadIfNeeded(int sizeHint)
        {
            // If writing is currently active and enough space, don't need to take the lock to just set WritingActive.
            // IsWritingActive is needed to prevent the reader releasing the writers memory when it fully consumes currently written.
            if (!_operationState.IsWritingActive)
            {
                AllocateWriteHeadSynchronized(sizeHint);
            }
            else if (_writingMemory.Length == 0 || _writingMemory.Length < sizeHint)
            {
                AllocateWriteHead(sizeHint);
            }
        }

        private void AllocateWriteHead(int sizeHint)
        {
            Debug.Assert(_operationState.IsWritingActive);
            Debug.Assert(_writingHead != null);

            if (_buffered > 0)
            {
                // Flush buffered data to the segment
                _writingHead.End += _buffered;
                _buffered = 0;
            }

            AllocateAndLinkSegment(sizeHint);
        }

        private void AllocateWriteHeadSynchronized(int sizeHint)
        {
            Debug.Assert(!_operationState.IsWritingActive);

            BufferSegment newSegment = null;
            if (_writingHead is null)
            {
                // We definately always need to allocate memory to write since nobody has written before, 
                // we can now opportunistically do that out of lock as it doesn't change any state and it is expensive.
                newSegment = AllocateSegment(sizeHint);
            }

            lock (_sync)
            {
                _operationState.BeginWrite();

                // We need to recheck state as AdvanceReader may have deallocated it prior to setting writing active.
                if (_writingMemory.Length == 0 || _writingMemory.Length < sizeHint)
                {
                    // Need to allocate BufferSegment
                    // Note: If _writingHead == null then _writingMemory.Length == 0, so only need to check one.

                    if (_buffered > 0)
                    {
                        // If we have buffered data, we must already have a writing head.
                        Debug.Assert(_writingHead != null);

                        // Flush buffered data to the segment
                        _writingHead.End += _buffered;
                        _buffered = 0;
                    }

                    if (newSegment is null)
                    {
                        // We haven't opportunistically allocated as _writingHead != null when we entered, outside the lock.
                        newSegment = AllocateSegment(sizeHint);
                    }

                    if (_writingHead is null)
                    {
                        // If _writingHead is null, then we set all the pointers to the newSegment.
                        _writingHead = _readHead = _readTail = newSegment;
                    }
                    else
                    {
                        // Otherwise update the _writingHead to the newSegment.
                        LinkWriteSegmentAndAdvance(newSegment);
                    }

                    // Set the _writingMemory to the newly allocated memory.
                    _writingMemory = newSegment.AvailableMemory;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AllocateAndLinkSegment(int sizeHint = 0)
        {
            BufferSegment newSegment = AllocateSegment(sizeHint);
            LinkWriteSegmentAndAdvance(newSegment);
            _writingMemory = newSegment.AvailableMemory;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void LinkWriteSegmentAndAdvance(BufferSegment newSegment)
        {
            _writingHead.SetNext(newSegment);
            _writingHead = newSegment;
        }

        private BufferSegment AllocateSegment(int sizeHint)
        {
            BufferSegment newSegment = CreateSegmentSynchronized();

            if (_pool is null)
            {
                // Use the array pool
                newSegment.SetOwnedMemory(ArrayPool<byte>.Shared.Rent(GetSegmentSize(sizeHint)));
            }
            else if (sizeHint <= _pool.MaxBufferSize)
            {
                // Use the specified pool if it fits
                newSegment.SetOwnedMemory(_pool.Rent(GetSegmentSize(sizeHint, _pool.MaxBufferSize)));
            }
            else
            {
                // We can't use the pool so allocate an array
                newSegment.SetUnownedMemory(new byte[sizeHint]);
            }

            return newSegment;
        }

        private int GetSegmentSize(int sizeHint, int maxBufferSize = int.MaxValue)
        {
            // First we need to handle case where hint is smaller than minimum segment size
            sizeHint = Math.Max(_minimumSegmentSize, sizeHint);
            // After that adjust it to fit into pools max buffer size
            var adjustedToMaximumSize = Math.Min(maxBufferSize, sizeHint);
            return adjustedToMaximumSize;
        }

        internal bool CommitUnsynchronized()
        {
            _operationState.EndWrite();

            if (_currentWriteLength == 0)
            {
                // Nothing written to commit
                return true;
            }

            // Update the writing head
            _writingHead.End += _buffered;

            // Always move the read tail to the write head
            _readTail = _writingHead;
            _readTailIndex = _writingHead.End;

            long oldLength = _length;
            _length += _currentWriteLength;

            // Do not reset if reader is complete
            if (_pauseWriterThreshold > 0 &&
                oldLength < _pauseWriterThreshold &&
                _length >= _pauseWriterThreshold &&
                !_readerCompletion.IsCompleted)
            {
                _writerAwaitable.SetUncompleted();
            }

            _currentWriteLength = 0;
            _buffered = 0;

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Advance(int bytesWritten)
        {
            if ((uint)bytesWritten > (uint)_writingMemory.Length)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.bytesWritten);
            }

            AdvanceCore(bytesWritten);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AdvanceCore(int bytesWritten)
        {
            _currentWriteLength += bytesWritten;
            _buffered += bytesWritten;
            _writingMemory = _writingMemory.Slice(bytesWritten);
        }

        internal ValueTask<FlushResult> FlushAsync(CancellationToken cancellationToken)
        {
            CompletionData completionData;
            ValueTask<FlushResult> result;
            lock (_sync)
            {
                var wasEmpty = CommitUnsynchronized();

                // AttachToken before completing reader awaiter in case cancellationToken is already completed
                _writerAwaitable.BeginOperation(cancellationToken, s_signalWriterAwaitable, this);

                // If the writer is completed (which it will be most of the time) then return a completed ValueTask
                if (_writerAwaitable.IsCompleted)
                {
                    var flushResult = new FlushResult();
                    GetFlushResult(ref flushResult);
                    result = new ValueTask<FlushResult>(flushResult);
                }
                else
                {
                    // Otherwise it's async
                    result = new ValueTask<FlushResult>(_writer, token: 0);
                }

                // Complete reader only if new data was pushed into the pipe
                // Avoid throwing in between completing the reader and scheduling the callback
                // if the intent is to allow pipe to continue reading the data
                if (!wasEmpty)
                {
                    _readerAwaitable.Complete(out completionData);
                }
                else
                {
                    completionData = default;
                }

                // I couldn't find a way for flush to induce backpressure deadlock
                // if it always adds new data to pipe and wakes up the reader but assert anyway
                Debug.Assert(_writerAwaitable.IsCompleted || _readerAwaitable.IsCompleted);
            }

            TrySchedule(_readerScheduler, completionData);

            return result;
        }

        internal void CompleteWriter(Exception exception)
        {
            CompletionData completionData;
            PipeCompletionCallbacks completionCallbacks;
            bool readerCompleted;

            lock (_sync)
            {
                // Commit any pending buffers
                CommitUnsynchronized();

                completionCallbacks = _writerCompletion.TryComplete(exception);
                _readerAwaitable.Complete(out completionData);
                readerCompleted = _readerCompletion.IsCompleted;
            }

            if (readerCompleted)
            {
                CompletePipe();
            }

            if (completionCallbacks != null)
            {
                ScheduleCallbacks(_readerScheduler, completionCallbacks);
            }

            TrySchedule(_readerScheduler, completionData);
        }

        internal void AdvanceReader(in SequencePosition consumed)
        {
            AdvanceReader(consumed, consumed);
        }

        internal void AdvanceReader(in SequencePosition consumed, in SequencePosition examined)
        {
            // If the reader is completed
            if (_readerCompletion.IsCompleted)
            {
                ThrowHelper.ThrowInvalidOperationException_NoReadingAllowed();
            }

            // TODO: Use new SequenceMarshal.TryGetReadOnlySequenceSegment to get the correct data
            // directly casting only works because the type value in ReadOnlySequenceSegment is 0
            AdvanceReader((BufferSegment)consumed.GetObject(), consumed.GetInteger(), (BufferSegment)examined.GetObject(), examined.GetInteger());
        }

        private void AdvanceReader(BufferSegment consumedSegment, int consumedIndex, BufferSegment examinedSegment, int examinedIndex)
        {
            BufferSegment returnStart = null;
            BufferSegment returnEnd = null;

            CompletionData completionData = default;
            bool isReadComplete;

            lock (_sync)
            {
                var examinedEverything = false;
                if (examinedSegment == _readTail)
                {
                    examinedEverything = examinedIndex == _readTailIndex;
                }

                if (consumedSegment != null)
                {
                    if (_readHead == null)
                    {
                        ThrowHelper.ThrowInvalidOperationException_AdvanceToInvalidCursor();
                        return;
                    }

                    returnStart = _readHead;
                    returnEnd = consumedSegment;

                    // Check if we crossed _maximumSizeLow and complete backpressure
                    long consumedBytes = GetLength(returnStart, _readHeadIndex, consumedSegment, consumedIndex);
                    long oldLength = _length;
                    _length -= consumedBytes;

                    if (oldLength >= _resumeWriterThreshold &&
                        _length < _resumeWriterThreshold)
                    {
                        _writerAwaitable.Complete(out completionData);
                    }

                    // Check if we consumed entire last segment
                    // if we are going to return commit head we need to check that there is no writing operation that
                    // might be using tailspace
                    if (consumedIndex == returnEnd.Length && !_operationState.IsWritingActive)
                    {
                        BufferSegment nextBlock = returnEnd.NextSegment;
                        if (_readTail == returnEnd)
                        {
                            _readTail = nextBlock;
                            _readTailIndex = 0;
                        }

                        _readHead = nextBlock;
                        _readHeadIndex = 0;

                        // Reset the writing head to null if it's the return block
                        // then null it out as we're about to reset that memory
                        if (_writingHead == returnEnd)
                        {
                            // If we're about to null out the _writingHead then assert the list is empty
                            Debug.Assert(_readHead == null);
                            Debug.Assert(_readTail == null);
                            _writingHead = null;
                            _writingMemory = default;
                        }

                        returnEnd = nextBlock;
                    }
                    else
                    {
                        _readHead = consumedSegment;
                        _readHeadIndex = consumedIndex;
                    }
                }

                // We reset the awaitable to not completed if we've examined everything the producer produced so far
                // but only if writer is not completed yet
                if (examinedEverything && !_writerCompletion.IsCompleted)
                {
                    // Prevent deadlock where reader awaits new data and writer await backpressure
                    if (!_writerAwaitable.IsCompleted)
                    {
                        ThrowHelper.ThrowInvalidOperationException_BackpressureDeadlock(_resumeWriterThreshold);
                    }
                    _readerAwaitable.SetUncompleted();
                }

                isReadComplete = _operationState.TryEndRead();
            }

            if (isReadComplete)
            {
                TrySchedule(_writerScheduler, completionData);
            }

            if (returnStart != null && returnStart != returnEnd)
            {
                ReturnSegments(returnStart, returnEnd);
            }

            if (!isReadComplete)
            {
                // Segments need to be returned (above) prior to the throw.
                ThrowHelper.ThrowInvalidOperationException_NoReadToComplete();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long GetLength(BufferSegment startSegment, int startIndex, BufferSegment endSegment, int endIndex)
        {
            return (endSegment.RunningIndex + (uint)endIndex) - (startSegment.RunningIndex + (uint)startIndex);
        }

        internal void CompleteReader(Exception exception)
        {
            PipeCompletionCallbacks completionCallbacks;
            CompletionData completionData;
            bool writerCompleted;

            lock (_sync)
            {
                // If we're reading, treat clean up that state before continuting
                if (_operationState.IsReadingActive)
                {
                    _operationState.EndRead();
                }

                // REVIEW: We should consider cleaning up all of the allocated memory
                // on the reader side now.

                completionCallbacks = _readerCompletion.TryComplete(exception);
                _writerAwaitable.Complete(out completionData);
                writerCompleted = _writerCompletion.IsCompleted;
            }

            if (writerCompleted)
            {
                CompletePipe();
            }

            if (completionCallbacks != null)
            {
                ScheduleCallbacks(_writerScheduler, completionCallbacks);
            }

            TrySchedule(_writerScheduler, completionData);
        }

        internal void OnWriterCompleted(Action<Exception, object> callback, object state)
        {
            if (callback is null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.callback);
            }

            PipeCompletionCallbacks completionCallbacks;
            lock (_sync)
            {
                completionCallbacks = _writerCompletion.AddCallback(callback, state);
            }

            if (completionCallbacks != null)
            {
                ScheduleCallbacks(_readerScheduler, completionCallbacks);
            }
        }

        internal void CancelPendingRead()
        {
            CompletionData completionData;
            lock (_sync)
            {
                _readerAwaitable.Cancel(out completionData);
            }
            TrySchedule(_readerScheduler, completionData);
        }

        internal void CancelPendingFlush()
        {
            CompletionData completionData;
            lock (_sync)
            {
                _writerAwaitable.Cancel(out completionData);
            }
            TrySchedule(_writerScheduler, completionData);
        }

        internal void OnReaderCompleted(Action<Exception, object> callback, object state)
        {
            if (callback is null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.callback);
            }

            PipeCompletionCallbacks completionCallbacks;
            lock (_sync)
            {
                completionCallbacks = _readerCompletion.AddCallback(callback, state);
            }

            if (completionCallbacks != null)
            {
                ScheduleCallbacks(_writerScheduler, completionCallbacks);
            }
        }

        internal ValueTask<ReadResult> ReadAsync(CancellationToken token)
        {
            if (_readerCompletion.IsCompleted)
            {
                ThrowHelper.ThrowInvalidOperationException_NoReadingAllowed();
            }

            ValueTask<ReadResult> result;
            lock (_sync)
            {
                _readerAwaitable.BeginOperation(token, s_signalReaderAwaitable, this);

                // If the awaitable is already complete then return the value result directly
                if (_readerAwaitable.IsCompleted)
                {
                    GetReadResult(out ReadResult readResult);
                    result = new ValueTask<ReadResult>(readResult);
                }
                else
                {
                    // Otherwise it's async
                    result = new ValueTask<ReadResult>(_reader, token: 0);
                }
            }

            return result;
        }

        internal bool TryRead(out ReadResult result)
        {
            lock (_sync)
            {
                if (_readerCompletion.IsCompleted)
                {
                    ThrowHelper.ThrowInvalidOperationException_NoReadingAllowed();
                }

                if (_length > 0 || _readerAwaitable.IsCompleted)
                {
                    GetReadResult(out result);
                    return true;
                }

                if (_readerAwaitable.IsRunning)
                {
                    ThrowHelper.ThrowInvalidOperationException_AlreadyReading();
                }

                _operationState.BeginReadTentative();
                result = default;
                return false;
            }
        }

        private static void ScheduleCallbacks(PipeScheduler scheduler, PipeCompletionCallbacks completionCallbacks)
        {
            Debug.Assert(completionCallbacks != null);

            scheduler.UnsafeSchedule(s_invokeCompletionCallbacks, completionCallbacks);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void TrySchedule(PipeScheduler scheduler, in CompletionData completionData)
        {
            Action<object> completion = completionData.Completion;
            // Nothing to do
            if (completion is null)
            {
                return;
            }

            // Ultimately, we need to call either
            // 1. The sync context with a delegate
            // 2. The scheduler with a delegate
            // That delegate and state will either be the action passed in directly
            // or it will be that specified delegate wrapped in ExecutionContext.Run
            if (completionData.SynchronizationContext is null && completionData.ExecutionContext is null)
            {
                // Common fast-path
                scheduler.UnsafeSchedule(completion, completionData.CompletionState);
            }
            else
            {
                ScheduleWithContext(scheduler, in completionData);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void ScheduleWithContext(PipeScheduler scheduler, in CompletionData completionData)
        {
            Debug.Assert(completionData.SynchronizationContext != null || completionData.ExecutionContext != null);

            if (completionData.SynchronizationContext is null)
            {
                // We also have to run on the specified execution context so run the scheduler and execute the
                // delegate on the execution context
                scheduler.UnsafeSchedule(s_scheduleWithExecutionContextCallback, completionData);
            }
            else
            {
                if (completionData.ExecutionContext is null)
                {
                    // We need to box the struct here since there's no generic overload for state
                    completionData.SynchronizationContext.Post(s_syncContextExecuteWithoutExecutionContextCallback, completionData);
                }
                else
                {
                    // We need to execute the callback with the execution context
                    completionData.SynchronizationContext.Post(s_syncContextExecutionContextCallback, completionData);
                }
            }
        }

        private static void ExecuteWithoutExecutionContext(object state)
        {
            CompletionData completionData = (CompletionData)state;
            completionData.Completion(completionData.CompletionState);
        }

        private static void ExecuteWithExecutionContext(object state)
        {
            CompletionData completionData = (CompletionData)state;
            Debug.Assert(completionData.ExecutionContext != null);
            ExecutionContext.Run(completionData.ExecutionContext, s_executionContextRawCallback, state);
        }

        private void CompletePipe()
        {
            BufferSegment segment;
            lock (_sync)
            {
                if (_disposed)
                {
                    return;
                }

                _disposed = true;
                // Get segment chain to return
                // if _readHead is null we need to try return _commitHead
                // because there might be a block allocated for writing
                segment = _readHead ?? _readTail;

                _writingHead = null;
                _readHead = null;
                _readTail = null;
            }

            // Return all segments
            if (segment != null)
            {
                ReturnSegments(segment, toExclusive: null);
            }
        }

        internal ValueTaskSourceStatus GetReadAsyncStatus()
        {
            if (_readerAwaitable.IsCompleted)
            {
                if (_writerCompletion.IsFaulted)
                {
                    return ValueTaskSourceStatus.Faulted;
                }

                return ValueTaskSourceStatus.Succeeded;
            }
            return ValueTaskSourceStatus.Pending;
        }

        internal void OnReadAsyncCompleted(Action<object> continuation, object state, ValueTaskSourceOnCompletedFlags flags)
        {
            CompletionData completionData;
            bool doubleCompletion;
            lock (_sync)
            {
                _readerAwaitable.OnCompleted(continuation, state, flags, out completionData, out doubleCompletion);
            }
            if (doubleCompletion)
            {
                Writer.Complete(ThrowHelper.CreateInvalidOperationException_NoConcurrentOperation());
            }
            TrySchedule(_readerScheduler, completionData);
        }

        internal ReadResult GetReadAsyncResult()
        {
            ReadResult result;
            CancellationTokenRegistration cancellationTokenRegistration = default;
            CancellationToken cancellationToken = default;
            try
            {
                lock (_sync)
                {
                    if (!_readerAwaitable.IsCompleted)
                    {
                        ThrowHelper.ThrowInvalidOperationException_GetResultNotCompleted();
                    }

                    cancellationTokenRegistration = _readerAwaitable.ReleaseCancellationTokenRegistration(out cancellationToken);
                    GetReadResult(out result);
                }
            }
            finally
            {
                cancellationTokenRegistration.Dispose();
                cancellationToken.ThrowIfCancellationRequested();
            }

            return result;
        }

        private void GetReadResult(out ReadResult result)
        {
            bool isCompleted = _writerCompletion.IsCompletedOrThrow();
            bool isCanceled = _readerAwaitable.ObserveCancellation();

            // No need to read end if there is no head
            BufferSegment head = _readHead;
            if (head != null)
            {
                // Reading commit head shared with writer
                var readOnlySequence = new ReadOnlySequence<byte>(head, _readHeadIndex, _readTail, _readTailIndex);
                result = new ReadResult(readOnlySequence, isCanceled, isCompleted);
            }
            else
            {
                result = new ReadResult(default, isCanceled, isCompleted);
            }

            if (isCanceled)
            {
                _operationState.BeginReadTentative();
            }
            else
            {
                _operationState.BeginRead();
            }
        }

        internal ValueTaskSourceStatus GetFlushAsyncStatus()
        {
            if (_writerAwaitable.IsCompleted)
            {
                if (_readerCompletion.IsFaulted)
                {
                    return ValueTaskSourceStatus.Faulted;
                }

                return ValueTaskSourceStatus.Succeeded;
            }
            return ValueTaskSourceStatus.Pending;
        }

        internal FlushResult GetFlushAsyncResult()
        {
            FlushResult result = default;
            CancellationToken cancellationToken = default;
            CancellationTokenRegistration cancellationTokenRegistration = default;

            try
            {
                lock (_sync)
                {
                    if (!_writerAwaitable.IsCompleted)
                    {
                        ThrowHelper.ThrowInvalidOperationException_GetResultNotCompleted();
                    }

                    GetFlushResult(ref result);

                    cancellationTokenRegistration = _writerAwaitable.ReleaseCancellationTokenRegistration(out cancellationToken);
                }
            }
            finally
            {
                cancellationTokenRegistration.Dispose();
                cancellationToken.ThrowIfCancellationRequested();
            }

            return result;
        }

        private void GetFlushResult(ref FlushResult result)
        {
            // Change the state from to be canceled -> observed
            if (_writerAwaitable.ObserveCancellation())
            {
                result._resultFlags |= ResultFlags.Canceled;
            }
            if (_readerCompletion.IsCompletedOrThrow())
            {
                result._resultFlags |= ResultFlags.Completed;
            }
        }

        internal ValueTask<FlushResult> WriteAsync(ReadOnlyMemory<byte> source, CancellationToken cancellationToken)
        {
            if (_writerCompletion.IsCompleted)
            {
                ThrowHelper.ThrowInvalidOperationException_NoWritingAllowed();
            }

            // Allocate whatever the pool gives us so we can write, this also marks the
            // state as writing
            AllocateWriteHeadIfNeeded(0);

            if (source.Length <= _writingMemory.Length)
            {
                source.CopyTo(_writingMemory);

                AdvanceCore(source.Length);
            }
            else
            {
                // This is the multi segment copy
                WriteMultiSegment(source.Span);
            }

            return FlushAsync(cancellationToken);
        }

        private void WriteMultiSegment(ReadOnlySpan<byte> source)
        {
            Span<byte> destination = _writingMemory.Span;

            while (true)
            {
                int writable = Math.Min(destination.Length, source.Length);
                source.Slice(0, writable).CopyTo(destination);
                source = source.Slice(writable);
                AdvanceCore(writable);

                if (source.Length == 0)
                {
                    break;
                }

                // We filled the segment
                _writingHead.End += writable;
                _buffered = 0;

                AllocateAndLinkSegment();

                destination = _writingMemory.Span;
            }
        }

        internal void OnFlushAsyncCompleted(Action<object> continuation, object state, ValueTaskSourceOnCompletedFlags flags)
        {
            CompletionData completionData;
            bool doubleCompletion;
            lock (_sync)
            {
                _writerAwaitable.OnCompleted(continuation, state, flags, out completionData, out doubleCompletion);
            }
            if (doubleCompletion)
            {
                Reader.Complete(ThrowHelper.CreateInvalidOperationException_NoConcurrentOperation());
            }
            TrySchedule(_writerScheduler, completionData);
        }

        private void ReaderCancellationRequested()
        {
            CompletionData completionData;
            lock (_sync)
            {
                _readerAwaitable.CancellationTokenFired(out completionData);
            }
            TrySchedule(_readerScheduler, completionData);
        }

        private void WriterCancellationRequested()
        {
            CompletionData completionData;
            lock (_sync)
            {
                _writerAwaitable.CancellationTokenFired(out completionData);
            }
            TrySchedule(_writerScheduler, completionData);
        }

        /// <summary>
        /// Gets the <see cref="PipeReader"/> for this pipe.
        /// </summary>
        public PipeReader Reader => _reader;

        /// <summary>
        /// Gets the <see cref="PipeWriter"/> for this pipe.
        /// </summary>
        public PipeWriter Writer => _writer;

        /// <summary>
        /// Resets the pipe
        /// </summary>
        public void Reset()
        {
            lock (_sync)
            {
                if (!_disposed)
                {
                    ThrowHelper.ThrowInvalidOperationException_ResetIncompleteReaderWriter();
                }

                _disposed = false;
                ResetState();
            }
        }

        private BufferSegment CreateSegmentSynchronized()
        {
            SegmentAsValue[] segmentPool = _bufferSegmentPool;
            lock (segmentPool)
            {
                int index = _pooledSegmentCount - 1;
                if ((uint)index < (uint)segmentPool.Length)
                {
                    _pooledSegmentCount = index;
                    return segmentPool[index];
                }
            }

            return new BufferSegment();
        }

        private void ReturnSegments(BufferSegment from, BufferSegment toExclusive)
        {
            Debug.Assert(from != null);
            Debug.Assert(from != toExclusive);

            // Reset the Segments and return their data out of lock
            ValueSegmentList segmentsToReturn = default;
            ref var startSegment = ref segmentsToReturn.Segment00;
            int count = 0;
            do
            {
                BufferSegment next = from.NextSegment;
                Debug.Assert(next != null || toExclusive == null);

                from.ResetMemory();

                if ((uint)count < (uint)SegmentPoolSize)
                {
                    // Store in temporary list while preforming expensive resets
                    Unsafe.Add(ref startSegment, count) = from;
                    count++;
                }

                from = next;
            } while (from != toExclusive);

            // Add the Segments back to pool from the temporary list under lock
            SegmentAsValue[] segmentPool = _bufferSegmentPool;
            lock (segmentPool)
            {
                int index = _pooledSegmentCount;
                for (int i = 0; i < count; i++)
                {
                    if ((uint)index < (uint)segmentPool.Length)
                    {
                        segmentPool[index] = Unsafe.Add(ref startSegment, i);
                        index++;
                    }
                }

                _pooledSegmentCount = index;
            }
        }

        // Used to avoid covariant checks on the array
        private readonly struct SegmentAsValue
        {
            private readonly BufferSegment _bufferSegment;
            public SegmentAsValue(BufferSegment bufferSegment) => _bufferSegment = bufferSegment;
            public static implicit operator SegmentAsValue(BufferSegment b) => new SegmentAsValue(b);
            public static implicit operator BufferSegment(SegmentAsValue s) => s._bufferSegment;
        }

        // Temporary list to hold Segments return while being reset
#pragma warning disable CS0649
        private ref struct ValueSegmentList
        {
            public BufferSegment Segment00;
            public BufferSegment Segment01;
            public BufferSegment Segment02;
            public BufferSegment Segment03;
            public BufferSegment Segment04;
            public BufferSegment Segment05;
            public BufferSegment Segment06;
            public BufferSegment Segment07;
            public BufferSegment Segment08;
            public BufferSegment Segment09;
            public BufferSegment Segment10;
            public BufferSegment Segment11;
            public BufferSegment Segment12;
            public BufferSegment Segment13;
            public BufferSegment Segment14;
            public BufferSegment Segment15;
        }
#pragma warning enable CS0649
    }
}
