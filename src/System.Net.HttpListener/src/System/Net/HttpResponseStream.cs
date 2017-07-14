// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net
{
    internal sealed partial class HttpResponseStream : Stream
    {
        private bool _closed;
        internal bool Closed => _closed;

        public override bool CanRead => false;
        public override bool CanSeek => false;
        public override bool CanWrite => true;

        public override void Flush() { }
        public override Task FlushAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public override long Length => throw new NotSupportedException(SR.net_noseek);

        public override long Position
        {
            get => throw new NotSupportedException(SR.net_noseek);
            set => throw new NotSupportedException(SR.net_noseek);
        }

        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException(SR.net_noseek);

        public override void SetLength(long value) => throw new NotSupportedException(SR.net_noseek);

        public override int Read(byte[] buffer, int offset, int size) => throw new InvalidOperationException(SR.net_writeonlystream);

        public override IAsyncResult BeginRead(byte[] buffer, int offset, int size, AsyncCallback callback, object state)
        {
            throw new InvalidOperationException(SR.net_writeonlystream);
        }

        public override int EndRead(IAsyncResult asyncResult) => throw new InvalidOperationException(SR.net_writeonlystream);
        
        public new Task<int> ReadAsync(Byte[] buffer, int offset, int count)
        {
            return ReadAsync(buffer, offset, count, CancellationToken.None);
        }
        
        public override Task<int> ReadAsync(byte[] buffer, int offset, int size, CancellationToken cancellationToken)
        {
            return Task<int>.Factory.FromAsync(
                (callback, state) => this.BeginRead(buffer, offset, size, callback, state),
                iar => this.EndRead(iar),
                null);
        }

        public override void Write(byte[] buffer, int offset, int size)
        {
            if (NetEventSource.IsEnabled)
            {
                NetEventSource.Enter(this);
                NetEventSource.Info(this, "buffer.Length:" + buffer.Length + " size:" + size + " offset:" + offset);
            }
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }
            if (offset < 0 || offset > buffer.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }
            if (size < 0 || size > buffer.Length - offset)
            {
                throw new ArgumentOutOfRangeException(nameof(size));
            }
            if (_closed)
            {
                if (NetEventSource.IsEnabled) NetEventSource.Exit(this);
                return;
            }

            WriteCore(buffer, offset, size);
        }

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int size, AsyncCallback callback, object state)
        {
            if (NetEventSource.IsEnabled) NetEventSource.Info(this, "buffer.Length:" + buffer.Length + " size:" + size + " offset:" + offset);
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }
            if (offset < 0 || offset > buffer.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }
            if (size < 0 || size > buffer.Length - offset)
            {
                throw new ArgumentOutOfRangeException(nameof(size));
            }

            return BeginWriteCore(buffer, offset, size, callback, state);
        }

        public override void EndWrite(IAsyncResult asyncResult)
        {
            if (NetEventSource.IsEnabled)
            {
                NetEventSource.Enter(this);
                NetEventSource.Info(this, $"asyncResult:{asyncResult}");
            }
            if (asyncResult == null)
            {
                throw new ArgumentNullException(nameof(asyncResult));
            }

            EndWriteCore(asyncResult);
        }
        
        public new Task WriteAsync(Byte[] buffer, int offset, int size)
        {
            return WriteAsync(buffer, offset, size, CancellationToken.None);
        }
        
        public override Task WriteAsync(byte[] buffer, int offset, int size, CancellationToken cancellationToken)
        {
            return Task.Factory.FromAsync(
                (callback, state) => this.BeginWrite(buffer, offset, size, callback, state),
                iar => this.EndWrite(iar),
                null);
        }

        protected override void Dispose(bool disposing)
        {
            if (NetEventSource.IsEnabled) NetEventSource.Enter(this);
            try
            {
                if (disposing)
                {
                    if (NetEventSource.IsEnabled) NetEventSource.Info(this, "_closed:" + _closed);
                    if (_closed)
                    {
                        if (NetEventSource.IsEnabled) NetEventSource.Exit(this);
                        return;
                    }
                    _closed = true;
                    DisposeCore();
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
            if (NetEventSource.IsEnabled) NetEventSource.Exit(this);
        }
    }
}
