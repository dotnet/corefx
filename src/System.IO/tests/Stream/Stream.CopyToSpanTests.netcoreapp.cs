// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace System.IO.Tests
{
    public partial class StreamCopyToSpanTests
    {
        [Fact]
        public void CopyTo_InvalidArgsThrows()
        {
            using Stream s = new MemoryStream();

            AssertExtensions.Throws<ArgumentNullException>("action", () => s.CopyTo(null, null, 0));
            AssertExtensions.Throws<ArgumentOutOfRangeException>("bufferSize", () => s.CopyTo((_, __) => { }, null, 0));

            AssertExtensions.Throws<ArgumentNullException>("func", () => s.CopyToAsync(null, null, 0, default));
            AssertExtensions.Throws<ArgumentOutOfRangeException>("bufferSize", () => s.CopyToAsync((_, __, ___) => Task.CompletedTask, null, 0, default));
        }

        [Fact]
        public void CopyToAsync_PrecanceledToken_Cancels()
        {
            using var src = new MemoryStream();
            Assert.Equal(TaskStatus.Canceled, src.CopyToAsync((_, __, ___) => Task.CompletedTask, null, 4096, new CancellationToken(true)).Status);
        }

        [Fact]
        public async Task CopyToAsync_CancellationToken_Propagated()
        {
            using var src = new MemoryStream();
            src.WriteByte(0);
            src.Position = 0;

            var cancellationToken = new CancellationToken();
            await src.CopyToAsync(
                (_, __, token) => Task.Run(() => Assert.Equal(cancellationToken, token)),
                null,
                4096,
                cancellationToken
            );
        }

        [Fact]
        public async Task CopyToAsync_State_Propagated()
        {
            using var src = new MemoryStream();
            src.WriteByte(0);
            src.Position = 0;

            var expected = 42;
            await src.CopyToAsync(
                (state, _, __) => Task.Run(() => Assert.Equal(expected, state)),
                expected,
                4096,
                default
            );
        }

        [Fact]
        public void CopyTo_AllDataCopied()
        {
            using var src = new MemoryStream();
            src.Write(Enumerable.Range(0, 10000).Select(i => (byte)i).ToArray(), 0, 256);
            src.Position = 0;

            using var dst = new MemoryStream();
            src.CopyTo((span, _) => dst.Write(span), null, 4096);

            Assert.Equal<byte>(src.ToArray(), dst.ToArray());
        }

        [Fact]
        public async Task CopyToAsync_AllDataCopied()
        {
            using var src = new MemoryStream();
            src.Write(Enumerable.Range(0, 10000).Select(i => (byte)i).ToArray(), 0, 256);
            src.Position = 0;

            using var dst = new MemoryStream();
            await src.CopyToAsync((_, memory, ___) => dst.WriteAsync(memory).AsTask(), null, 4096, default);

            Assert.Equal<byte>(src.ToArray(), dst.ToArray());
        }
    }
}
