// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
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
        private static readonly ContextCallback s_executionContextCallback = ExecuteWithExecutionContext;
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

            _operationState = default;
            _readerCompletion = default;
            _writerCompletion = default;

            _pool = options.Pool;
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

            BufferSegmentPool.IncrementActivePipes();
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

            lock (_sync)
            {
                AllocateWriteHeadUnsynchronized(sizeHint);
            }

            // Slice the AvailableMemory to the WritableBytes size
            int end = _writingHead.End;
            Memory<byte> availableMemory = _writingHead.AvailableMemory;
            availableMemory = availableMemory.Slice(end);
            return availableMemory;
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

            lock (_sync)
            {
                AllocateWriteHeadUnsynchronized(sizeHint);
            }

            // Slice the AvailableMemory to the WritableBytes size
            int end = _writingHead.End;
            Span<byte> availableSpan = _writingHead.AvailableMemory.Span;
            availableSpan = availableSpan.Slice(end);
            return availableSpan;
        }

        private void AllocateWriteHeadUnsynchronized(int sizeHint)
        {
            _operationState.BeginWrite();
            if (_writingHead == null)
            {
                // We need to allocate memory to write since nobody has written before
                BufferSegment newSegment = BufferSegmentPool.GetSegment();
                newSegment.SetMemory(_pool.Rent(GetSegmentSize(sizeHint)));

                // Set all the pointers
                _writingHead = _readHead = _readTail = newSegment;
            }
            else
            {
                int bytesLeftInBuffer = _writingHead.WritableBytes;

                if (bytesLeftInBuffer == 0 || bytesLeftInBuffer < sizeHint)
                {
                    BufferSegment newSegment = BufferSegmentPool.GetSegment();
                    newSegment.SetMemory(_pool.Rent(GetSegmentSize(sizeHint)));

                    _writingHead.SetNext(newSegment);
                    _writingHead = newSegment;
                }
            }
        }

        private int GetSegmentSize(int sizeHint)
        {
            // First we need to handle case where hint is smaller than minimum segment size
            sizeHint = Math.Max(_minimumSegmentSize, sizeHint);
            // After that adjust it to fit into pools max buffer size
            var adjustedToMaximumSize = Math.Min(_pool.MaxBufferSize, sizeHint);
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

            // Always move the read tail to the write head
            _readTail = _writingHead;
            _readTailIndex = _writingHead.End;
            _length += _currentWriteLength;

            // Do not reset if reader is complete
            if (_pauseWriterThreshold > 0 &&
                _length >= _pauseWriterThreshold &&
                !_readerCompletion.IsCompleted)
            {
                _writerAwaitable.Reset();
            }

            _currentWriteLength = 0;

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Advance(int bytesWritten)
        {
            if (_writingHead == null)
            {
                ThrowHelper.ThrowInvalidOperationException_NotWritingNoAlloc();
            }

            if (bytesWritten >= 0)
            {
                Debug.Assert(_writingHead.Next == null);

                Memory<byte> buffer = _writingHead.AvailableMemory;

                if (_writingHead.End > buffer.Length - bytesWritten)
                {
                    ThrowHelper.ThrowInvalidOperationException_AdvancingPastBufferSize();
                }

                // if bytesWritten is zero, these do nothing
                _writingHead.End += bytesWritten;
                _currentWriteLength += bytesWritten;
            }
            else
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.bytesWritten);
            }
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
                TrySchedule(_readerScheduler, s_invokeCompletionCallbacks, completionCallbacks);
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
            bool readCompleted;

            CompletionData completionData = default;

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
                    long consumedBytes = new ReadOnlySequence<byte>(returnStart, _readHeadIndex, consumedSegment, consumedIndex).Length;
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
                        ThrowHelper.ThrowInvalidOperationException_BackpressureDeadlock();
                    }
                    _readerAwaitable.Reset();
                }

                readCompleted = _operationState.TryEndRead();
            }

            if (readCompleted)
            {
                // If read completed sucessfully, scehudle next early, prior to clean up
                TrySchedule(_writerScheduler, completionData);
            }

            // Always reset and return the segments prior to any read exception
            BufferSegmentPool.ReturnSegments(returnStart, returnEndExclusive: returnEnd);

            if (!readCompleted)
            {
                // If read not completed sucessfully throw exception
                ThrowHelper.ThrowInvalidOperationException_NoReadToComplete();
            }
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
                TrySchedule(_writerScheduler, s_invokeCompletionCallbacks, completionCallbacks);
            }

            TrySchedule(_writerScheduler, completionData);
        }

        internal void OnWriterCompleted(Action<Exception, object> callback, object state)
        {
            if (callback == null)
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
                TrySchedule(_readerScheduler, s_invokeCompletionCallbacks, completionCallbacks);
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
            if (callback == null)
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
                TrySchedule(_writerScheduler, s_invokeCompletionCallbacks, completionCallbacks);
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

        private static void TrySchedule(PipeScheduler scheduler, Action<object> action, object state)
        {
            if (action != null)
            {
                scheduler.UnsafeSchedule(action, state);
            }
        }

        private static void TrySchedule(PipeScheduler scheduler, in CompletionData completionData)
        {
            // Nothing to do
            if (completionData.Completion == null)
            {
                return;
            }

            // Ultimately, we need to call either
            // 1. The sync context with a delegate
            // 2. The scheduler with a delegate
            // That delegate and state will either be the action passed in directly
            // or it will be that specified delegate wrapped in ExecutionContext.Run

            if (completionData.SynchronizationContext == null)
            {
                // We don't have a SynchronizationContext so execute on the specified scheduler
                if (completionData.ExecutionContext == null)
                {
                    // We can run directly, this should be the default fast path
                    scheduler.UnsafeSchedule(completionData.Completion, completionData.CompletionState);
                    return;
                }

                // We also have to run on the specified execution context so run the scheduler and execute the
                // delegate on the execution context
                scheduler.UnsafeSchedule(s_scheduleWithExecutionContextCallback, completionData);
            }
            else
            {
                if (completionData.ExecutionContext == null)
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
            BufferSegment segment = null;
            lock (_sync)
            {
                if (_disposed)
                {
                    return;
                }

                _disposed = true;
                // Return all segments
                // if _readHead is null we need to try return _commitHead
                // because there might be a block allocated for writing
                segment = _readHead ?? _readTail;

                _writingHead = null;
                _readHead = null;
                _readTail = null;
            }

            // Reset the segments outside lock
            BufferSegmentPool.DecrementActivePipes();
            BufferSegmentPool.ReturnSegments(segment, returnEndExclusive: null);
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

        private class BufferSegmentPool
        {
            private static ThreadLocal<BufferSegmentPool> s_pools = null;

            private static readonly Func<BufferSegmentPool> s_poolFactory = new Func<BufferSegmentPool>(PoolFactory);
            private static int s_activePools;
            private static readonly object s_sync = new object();
            private static int s_activePipes;
            private static int s_maxSegmentsPerThread;

            private readonly ThreadLocal<BufferSegmentPool> _owningPool;

            private int _count;
            private BufferSegment _firstSegment;
            private BufferSegment _lastSegment;


            private BufferSegmentPool(ThreadLocal<BufferSegmentPool> owningPool)
            {
                _owningPool = owningPool;
            }

            public static void DecrementActivePipes()
            {
                lock (s_sync)
                {
                    int activePipes = s_activePipes - 1;
                    if (activePipes == 0)
                    {
                        // No active pipes, discard the pool
                        s_pools = null;
                        s_maxSegmentsPerThread = 0;
                        s_activePools = 0;
                    }

                    s_activePipes = activePipes;

                    CalculateSegmentsPerThread();
                }
            }

            public static void IncrementActivePipes()
            {
                lock (s_sync)
                {
                    int activePipes = s_activePipes + 1;
                    if (activePipes == 1)
                    {
                        // First active pipe, create the pool
                        s_pools = new ThreadLocal<BufferSegmentPool>(s_poolFactory);
                    }

                    s_activePipes = activePipes;

                    CalculateSegmentsPerThread();
                }
            }

            public static BufferSegment GetSegment()
            {
                Debug.Assert(s_pools != null);

                return s_pools.Value.GetOrCreateSegment();
            }

            private BufferSegment GetOrCreateSegment()
            {
                BufferSegmentPool pool = s_pools.Value;

                BufferSegment segment = _firstSegment;
                if (segment != null)
                {
                    BufferSegment nextSegment = segment.NextSegment;
                    _firstSegment = nextSegment;
                    if (nextSegment == null)
                    {
                        _lastSegment = null;
                    }

                    segment.NextSegment = null;
                    return segment;
                }

                return new BufferSegment();
            }

            public static void ReturnSegments(BufferSegment returnStart, BufferSegment returnEndExclusive)
            {
                BufferSegmentPool pool = s_pools?.Value;

                if (pool == null || pool._count >= s_maxSegmentsPerThread)
                {
                    // Pool full or no pool; just reset, don't return
                    while (returnStart != null && returnStart != returnEndExclusive)
                    {
                        BufferSegment nextSegment = returnStart.NextSegment;
                        returnStart.ResetMemory();
                        returnStart = nextSegment;
                    }
                }
                else if (returnStart != returnEndExclusive)
                {
                    pool.ResetAndReturnSegments(returnStart, returnEndExclusive);
                }
            }

            private void ResetAndReturnSegments(BufferSegment returnStart, BufferSegment returnEndExclusive)
            {
                Debug.Assert(_count < s_maxSegmentsPerThread);
                Debug.Assert(returnStart != returnEndExclusive);

                int count = _count;
                int maxSegments = s_maxSegmentsPerThread;

                BufferSegment lastSegment = _lastSegment;
                BufferSegment currentSegment = returnStart;

                if (_firstSegment == null)
                {
                    _firstSegment = currentSegment;
                }
                else
                {
                    lastSegment.NextSegment = currentSegment;
                }

                while (currentSegment != null && currentSegment != returnEndExclusive)
                {
                    BufferSegment nextSegment = currentSegment.NextSegment;
                    currentSegment.ResetMemory();

                    if (count < maxSegments)
                    {
                        count++;
                        lastSegment = currentSegment;
                    }

                    if (count >= maxSegments)
                    {
                        currentSegment.NextSegment = null;
                    }

                    currentSegment = nextSegment;
                }

                _count = count;
                _lastSegment = lastSegment;
            }

            private static void CalculateSegmentsPerThread()
            {
                if (s_activePools > 0)
                {
                    s_maxSegmentsPerThread = Math.Max(SegmentPoolSize, (s_activePipes * SegmentPoolSize) / s_activePools);
                }
                else
                {
                    s_maxSegmentsPerThread = 0;
                }
            }

            private static BufferSegmentPool PoolFactory()
            {
                lock (s_sync)
                {
                    BufferSegmentPool pool = new BufferSegmentPool(s_pools);
                    s_activePools++;
                    CalculateSegmentsPerThread();
                    return pool;
                }
            }

            ~BufferSegmentPool()
            {
                if (_owningPool == s_pools)
                {
                    lock (s_sync)
                    {
                        if (_owningPool == s_pools)
                        {
                            s_activePools--;

                            CalculateSegmentsPerThread();
                        }
                    }
                }
            }
        }
    }
}
