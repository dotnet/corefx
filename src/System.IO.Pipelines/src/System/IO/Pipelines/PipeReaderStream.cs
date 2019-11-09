﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Buffers;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace System.IO.Pipelines
{
    internal sealed class PipeReaderStream : Stream
    {
        private readonly PipeReader _pipeReader;

        public PipeReaderStream(PipeReader pipeReader, bool leaveOpen)
        {
            Debug.Assert(pipeReader != null);
            _pipeReader = pipeReader;
            LeaveOpen = leaveOpen;
        }

        protected override void Dispose(bool disposing)
        {
            if (!LeaveOpen)
            {
                _pipeReader.Complete();
            }
            base.Dispose(disposing);
        }

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override long Length => throw new NotSupportedException();

        public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

        internal bool LeaveOpen { get; set; }

        public override void Flush()
        {
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return ReadAsync(buffer, offset, count).GetAwaiter().GetResult();
        }

        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

        public override void SetLength(long value) => throw new NotSupportedException();

        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

        public sealed override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state) =>
            TaskToApm.Begin(ReadAsync(buffer, offset, count, default), callback, state);

        public sealed override int EndRead(IAsyncResult asyncResult) =>
            TaskToApm.End<int>(asyncResult);

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return ReadAsyncInternal(new Memory<byte>(buffer, offset, count), cancellationToken).AsTask();
        }

#if !NETSTANDARD2_0
        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            return ReadAsyncInternal(buffer, cancellationToken);
        }
#endif

        private async ValueTask<int> ReadAsyncInternal(Memory<byte> buffer, CancellationToken cancellationToken)
        {
            ReadResult result = await _pipeReader.ReadAsync(cancellationToken).ConfigureAwait(false);

            if (result.IsCanceled)
            {
                ThrowHelper.ThrowOperationCanceledException_ReadCanceled();
            }

            ReadOnlySequence<byte> sequence = result.Buffer;
            long bufferLength = sequence.Length;
            SequencePosition consumed = sequence.Start;

            try
            {
                if (bufferLength != 0)
                {
                    int actual = (int)Math.Min(bufferLength, buffer.Length);

                    ReadOnlySequence<byte> slice = actual == bufferLength ? sequence : sequence.Slice(0, actual);
                    consumed = slice.End;
                    slice.CopyTo(buffer.Span);

                    return actual;
                }

                if (result.IsCompleted)
                {
                    return 0;
                }
            }
            finally
            {
                _pipeReader.AdvanceTo(consumed);
            }

            // This is a buggy PipeReader implementation that returns 0 byte reads even though the PipeReader
            // isn't completed or canceled
            ThrowHelper.ThrowInvalidOperationException_InvalidZeroByteRead();
            return 0;
        }

        public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
        {
            // Delegate to CopyToAsync on the PipeReader
            return _pipeReader.CopyToAsync(destination, cancellationToken);
        }
    }
}
