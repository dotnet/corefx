// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Quic.Implementations
{
    internal abstract class QuicStreamProvider : IDisposable
    {
        internal abstract long StreamId { get; }

        internal abstract bool CanRead { get; }

        internal abstract int Read(Span<byte> buffer);

        internal abstract ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default);

        internal abstract void ShutdownRead();

        internal abstract bool CanWrite { get; }

        internal abstract void Write(ReadOnlySpan<byte> buffer);

        internal abstract ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default);

        internal abstract void ShutdownWrite();

        internal abstract void Flush();

        internal abstract Task FlushAsync(CancellationToken cancellationToken);

        public abstract void Dispose();
    }
}
