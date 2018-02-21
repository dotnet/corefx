// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading;

namespace System.IO.Pipelines
{
    /// <summary>
    /// Default <see cref="PipeWriter"/> and <see cref="PipeReader"/> implementation.
    /// </summary>
    public sealed partial class Pipe
    {
        private sealed class DefaultPipeReader : PipeReader, IPipeAwaiter<ReadResult>
        {
            private readonly Pipe _pipe;

            public DefaultPipeReader(Pipe pipe)
            {
                _pipe = pipe;
            }

            public override bool TryRead(out ReadResult result) => _pipe.TryRead(out result);

            public override PipeAwaiter<ReadResult> ReadAsync(CancellationToken cancellationToken = default) => _pipe.ReadAsync(cancellationToken);

            public override void AdvanceTo(SequencePosition consumed) => _pipe.AdvanceReader(consumed);

            public override void AdvanceTo(SequencePosition consumed, SequencePosition examined) => _pipe.AdvanceReader(consumed, examined);

            public override void CancelPendingRead() => _pipe.CancelPendingRead();

            public override void Complete(Exception exception = null) => _pipe.CompleteReader(exception);

            public override void OnWriterCompleted(Action<Exception, object> callback, object state) => _pipe.OnWriterCompleted(callback, state);

            public bool IsCompleted => _pipe.IsReadAsyncCompleted;

            public ReadResult GetResult() => _pipe.GetReadAsyncResult();

            public void OnCompleted(Action continuation) => _pipe.OnReadAsyncCompleted(continuation);
        }
    }
}
