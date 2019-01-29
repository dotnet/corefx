using System.Threading;
using System.Threading.Tasks;

namespace System.IO.Pipelines
{
    internal class BytePipeWriterWrapper : PipeWriter
    {
        private readonly PipeWriter<byte> _writer;

        public BytePipeWriterWrapper(PipeWriter<byte> writer)
        {
            _writer = writer;
        }

        public override void Advance(int count) => _writer.Advance(count);

        public override void CancelPendingFlush() => _writer.CancelPendingFlush();

        public override void Complete(Exception exception = null) => _writer.Complete(exception);

        public override ValueTask<FlushResult> FlushAsync(CancellationToken cancellationToken = default) =>
            _writer.FlushAsync(cancellationToken);

        public override Memory<byte> GetMemory(int sizeHint = 0) => _writer.GetMemory(sizeHint);

        public override Span<byte> GetSpan(int sizeHint = 0) => _writer.GetSpan(sizeHint);

        public override void OnReaderCompleted(Action<Exception, object> callback, object state) =>
            _writer.OnReaderCompleted(callback, state);
    }
}
