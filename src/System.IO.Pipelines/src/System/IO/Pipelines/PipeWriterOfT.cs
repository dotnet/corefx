// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Threading;
using System.Threading.Tasks;

namespace System.IO.Pipelines
{
    /// <summary>
    /// Defines a class that provides a pipeline to which data can be written.
    /// </summary>
    public abstract class PipeWriter<T> : IBufferWriter<T>
    {
        /// <summary>
        /// Marks the <see cref="PipeWriter{T}"/> as being complete, meaning no more items will be written to it.
        /// </summary>
        /// <param name="exception">Optional <see cref="Exception"/> indicating a failure that's causing the pipeline to complete.</param>
        public abstract void Complete(Exception exception = null);

        /// <summary>
        /// Cancel the pending <see cref="FlushAsync"/> operation. If there is none, cancels next <see cref="FlushAsync"/> operation, without completing the <see cref="PipeWriter{T}"/>.
        /// </summary>
        public abstract void CancelPendingFlush();

        /// <summary>
        /// Registers a callback that gets executed when the <see cref="PipeReader{T}"/> side of the pipe is completed
        /// </summary>
        public abstract void OnReaderCompleted(Action<Exception, object> callback, object state);

        /// <summary>
        /// Makes bytes written available to <see cref="PipeReader{T}"/> and runs <see cref="PipeReader{T}.ReadAsync"/> continuation.
        /// </summary>
        public abstract ValueTask<FlushResult> FlushAsync(CancellationToken cancellationToken = default);

        /// <inheritdoc />
        public abstract void Advance(int count);

        /// <inheritdoc />
        public abstract Memory<T> GetMemory(int sizeHint = 0);

        /// <inheritdoc />
        public abstract Span<T> GetSpan(int sizeHint = 0);

        /// <summary>
        /// Writes <paramref name="source"/> to the pipe and makes data accessible to <see cref="PipeReader"/>
        /// </summary>
        public virtual ValueTask<FlushResult> WriteAsync(ReadOnlyMemory<T> source, CancellationToken cancellationToken = default)
        {
            this.Write(source.Span);
            return FlushAsync(cancellationToken);
        }
    }
}
