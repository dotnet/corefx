// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net
{
    internal partial class HttpRequestStream : Stream
    {
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override bool CanRead => true;

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

        public override void Write(byte[] buffer, int offset, int size) => throw new InvalidOperationException(SR.net_readonlystream);

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int size, AsyncCallback callback, object state)
        {
            throw new InvalidOperationException(SR.net_readonlystream);
        }

        public override void EndWrite(IAsyncResult asyncResult) => throw new InvalidOperationException(SR.net_readonlystream);

        internal bool Closed => _closed;

        protected override void Dispose(bool disposing)
        {
            if (NetEventSource.IsEnabled)
                NetEventSource.Enter(this);
            try
            {
                if (NetEventSource.IsEnabled)
                    NetEventSource.Info(this, "_closed:" + _closed);
                _closed = true;
            }
            finally
            {
                base.Dispose(disposing);
            }
            if (NetEventSource.IsEnabled)
                NetEventSource.Exit(this);
        }
    }
}
