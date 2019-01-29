using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;

namespace System.IO.Pipelines
{
    internal class BytePipeReaderWrapper : PipeReader, IValueTaskSource<ReadResult>
    {
        private readonly Pipe<byte> _pipe;
        private readonly PipeReader<byte> _reader;

        public BytePipeReaderWrapper(Pipe<byte> pipe, PipeReader<byte> reader)
        {
            _pipe = pipe;
            _reader = reader;
        }

        public override void AdvanceTo(SequencePosition consumed) =>
            _reader.AdvanceTo(consumed);

        public override void AdvanceTo(SequencePosition consumed, SequencePosition examined) =>
            _reader.AdvanceTo(consumed, examined);

        public override void CancelPendingRead() => _reader.CancelPendingRead();

        public override void Complete(Exception exception = null) =>
            _reader.Complete(exception);

        public override void OnWriterCompleted(Action<Exception, object> callback, object state) =>
            _reader.OnWriterCompleted(callback, state);

        public override ValueTask<ReadResult> ReadAsync(CancellationToken cancellationToken = default)
        {
            ValueTask<ReadResult<byte>> readerValueTask = _reader.ReadAsync(cancellationToken);
            if (readerValueTask.IsCompleted)
                return new ValueTask<ReadResult>(readerValueTask.Result);
            else
                return new ValueTask<ReadResult>(this, token: 0);
        }

        public override bool TryRead(out ReadResult result)
        {
            bool success = _reader.TryRead(out ReadResult<byte> byteResult);
            result = byteResult;
            return success;
        }

        public ReadResult GetResult(short token) => _pipe.GetReadAsyncResult();

        public ValueTaskSourceStatus GetStatus(short token) => _pipe.GetReadAsyncStatus();

        public void OnCompleted(Action<object> continuation, object state, short token, ValueTaskSourceOnCompletedFlags flags)
        {
            throw new NotImplementedException();
        }
    }
}
