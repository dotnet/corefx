// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
        private sealed class DefaultPipeReader : PipeReader, IValueTaskSource<ReadResult>
        {
            private readonly Pipe _pipe;

            public DefaultPipeReader(Pipe pipe)
            {
                _pipe = pipe;
            }

            public override bool TryRead(out ReadResult result) => _pipe.TryRead(out result);

            public override ValueTask<ReadResult> ReadAsync(CancellationToken cancellationToken = default) => _pipe.ReadAsync(cancellationToken);

            public override void AdvanceTo(SequencePosition consumed) => _pipe.AdvanceReader(consumed);

            public override void AdvanceTo(SequencePosition consumed, SequencePosition examined) => _pipe.AdvanceReader(consumed, examined);

            internal override PipeReaderStream CreateStream() => new DefaultPipeReaderStream(this);

            public override void CancelPendingRead() => _pipe.CancelPendingRead();

            public override void Complete(Exception exception = null) => _pipe.CompleteReader(exception);

            public override void OnWriterCompleted(Action<Exception, object> callback, object state) => _pipe.OnWriterCompleted(callback, state);

            public ValueTaskSourceStatus GetStatus(short token) => _pipe.GetReadAsyncStatus();

            public ReadResult GetResult(short token) => _pipe.GetReadAsyncResult();

            public void OnCompleted(Action<object> continuation, object state, short token, ValueTaskSourceOnCompletedFlags flags) => _pipe.OnReadAsyncCompleted(continuation, state, flags);

            private sealed class DefaultPipeReaderStream : PipeReaderStream
            {
                private readonly Pipe _pipe;

                internal DefaultPipeReaderStream(DefaultPipeReader reader) : base(reader) => _pipe = reader._pipe;

                public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default(CancellationToken))
                {
                    ValueTask<int> syncResult = default;
                    ValueTask<ReadResult> asyncResult = default;
                    return _pipe.TryReadAsync(buffer.Span, cancellationToken, ref syncResult, ref asyncResult) ?
                        syncResult :
                        FinishReadAsync(buffer, asyncResult);
                }
            }
        }
    }
}
