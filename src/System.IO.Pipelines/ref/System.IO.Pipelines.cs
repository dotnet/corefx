// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------

namespace System.IO.Pipelines
{
    public partial struct FlushResult
    {
        private int _dummy;
        public FlushResult(bool isCanceled, bool isCompleted) { throw null; }
        public bool IsCanceled { get { throw null; } }
        public bool IsCompleted { get { throw null; } }
    }
    public partial interface IDuplexPipe
    {
        System.IO.Pipelines.PipeReader Input { get; }
        System.IO.Pipelines.PipeWriter Output { get; }
    }
    public partial interface IDuplexPipe<T>
    {
        System.IO.Pipelines.PipeReader<T> Input { get; }
        System.IO.Pipelines.PipeWriter<T> Output { get; }
    }
    public sealed partial class Pipe
    {
        public Pipe() { }
        public Pipe(System.IO.Pipelines.PipeOptions options) { }
        public System.IO.Pipelines.PipeReader Reader { get { throw null; } }
        public System.IO.Pipelines.PipeWriter Writer { get { throw null; } }
        public void Reset() { }
    }
    public sealed partial class Pipe<T>
    {
        public Pipe() { }
        public Pipe(System.IO.Pipelines.PipeOptions<T> options) { }
        public System.IO.Pipelines.PipeReader<T> Reader { get { throw null; } }
        public System.IO.Pipelines.PipeWriter<T> Writer { get { throw null; } }
        public void Reset() { }
    }
    public partial class PipeOptions<T>
    {
        public PipeOptions(System.Buffers.MemoryPool<T> pool = null, System.IO.Pipelines.PipeScheduler readerScheduler = null, System.IO.Pipelines.PipeScheduler writerScheduler = null, long? pauseWriterThreshold = null, long? resumeWriterThreshold = null, int? minimumSegmentSize = null, bool useSynchronizationContext = true) { }
        public static System.IO.Pipelines.PipeOptions<T> Default { get { throw null; } }
        public int MinimumSegmentSize { get { throw null; } }
        public long PauseWriterThreshold { get { throw null; } }
        public System.Buffers.MemoryPool<T> Pool { get { throw null; } }
        public System.IO.Pipelines.PipeScheduler ReaderScheduler { get { throw null; } }
        public long ResumeWriterThreshold { get { throw null; } }
        public bool UseSynchronizationContext { get { throw null; } }
        public System.IO.Pipelines.PipeScheduler WriterScheduler { get { throw null; } }
    }
    public partial class PipeOptions : System.IO.Pipelines.PipeOptions<byte>
    {
        public PipeOptions(System.Buffers.MemoryPool<byte> pool = null, System.IO.Pipelines.PipeScheduler readerScheduler = null, System.IO.Pipelines.PipeScheduler writerScheduler = null, long pauseWriterThreshold = (long)32768, long resumeWriterThreshold = (long)16384, int minimumSegmentSize = 2048, bool useSynchronizationContext = true) { }
        public new static System.IO.Pipelines.PipeOptions Default { get { throw null; } }
    }
    public abstract partial class PipeReader
    {
        protected PipeReader() { }
        public abstract void AdvanceTo(System.SequencePosition consumed);
        public abstract void AdvanceTo(System.SequencePosition consumed, System.SequencePosition examined);
        public abstract void CancelPendingRead();
        public abstract void Complete(System.Exception exception = null);
        public abstract void OnWriterCompleted(System.Action<System.Exception, object> callback, object state);
        public abstract System.Threading.Tasks.ValueTask<System.IO.Pipelines.ReadResult> ReadAsync(System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken));
        public abstract bool TryRead(out System.IO.Pipelines.ReadResult result);
    }
    public abstract partial class PipeReader<T>
    {
        protected PipeReader() { }
        public abstract void AdvanceTo(System.SequencePosition consumed);
        public abstract void AdvanceTo(System.SequencePosition consumed, System.SequencePosition examined);
        public abstract void CancelPendingRead();
        public abstract void Complete(System.Exception exception = null);
        public abstract void OnWriterCompleted(System.Action<System.Exception, object> callback, object state);
        public abstract System.Threading.Tasks.ValueTask<System.IO.Pipelines.ReadResult<T>> ReadAsync(System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken));
        public abstract bool TryRead(out System.IO.Pipelines.ReadResult<T> result);
    }
    public abstract partial class PipeScheduler
    {
        protected PipeScheduler() { }
        public static System.IO.Pipelines.PipeScheduler Inline { get { throw null; } }
        public static System.IO.Pipelines.PipeScheduler ThreadPool { get { throw null; } }
        public abstract void Schedule(System.Action<object> action, object state);
    }
    public abstract partial class PipeWriter<T> : System.Buffers.IBufferWriter<T>
    {
        protected PipeWriter() { }
        public abstract void Advance(int count);
        public abstract void CancelPendingFlush();
        public abstract void Complete(System.Exception exception = null);
        public abstract System.Threading.Tasks.ValueTask<System.IO.Pipelines.FlushResult> FlushAsync(System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken));
        public abstract System.Memory<T> GetMemory(int sizeHint = 0);
        public abstract System.Span<T> GetSpan(int sizeHint = 0);
        public abstract void OnReaderCompleted(System.Action<System.Exception, object> callback, object state);
        public virtual System.Threading.Tasks.ValueTask<System.IO.Pipelines.FlushResult> WriteAsync(System.ReadOnlyMemory<T> source, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
    }
    public abstract partial class PipeWriter : System.IO.Pipelines.PipeWriter<byte> { }
    public readonly partial struct ReadResult
    {
        private readonly object _dummy;
        public ReadResult(System.Buffers.ReadOnlySequence<byte> buffer, bool isCanceled, bool isCompleted) { throw null; }
        public System.Buffers.ReadOnlySequence<byte> Buffer { get { throw null; } }
        public bool IsCanceled { get { throw null; } }
        public bool IsCompleted { get { throw null; } }
        public static implicit operator System.IO.Pipelines.ReadResult(System.IO.Pipelines.ReadResult<byte> byteResult) { throw null; }
    }
    public readonly partial struct ReadResult<T>
    {
        private readonly object _dummy;
        public ReadResult(System.Buffers.ReadOnlySequence<T> buffer, bool isCanceled, bool isCompleted) { throw null; }
        public System.Buffers.ReadOnlySequence<T> Buffer { get { throw null; } }
        public bool IsCanceled { get { throw null; } }
        public bool IsCompleted { get { throw null; } }
    }
}
