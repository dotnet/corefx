// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace System.IO.Tests
{
    public class StreamCopyToTests
    {
        [Fact]
        public void IfCanSeekIsFalseLengthAndPositionShouldNotBeCalled()
        {
            var baseStream = new ConfigurablePropertyStream();
            baseStream.SetCanRead(true);
            baseStream.SetCanSeek(false);
            var trackingStream = new CallTrackingStream(baseStream);

            var dest = Stream.Null;
            trackingStream.CopyTo(dest);

            // When the FEATURE_CORECLR conditional is removed around
            // the CanSeek optimization, this should be uncommented
            // Assert.Equal(1, trackingStream.TimesCalled(nameof(trackingStream.CanSeek)));

            Assert.Equal(0, trackingStream.TimesCalled(nameof(trackingStream.Length)));
            Assert.Equal(0, trackingStream.TimesCalled(nameof(trackingStream.Position)));
            // We can't override CopyTo since it's not virtual, so checking TimesCalled
            // for CopyTo will result in 0. Instead, we check that Read was called,
            // and validate the parameters passed there.
            Assert.Equal(1, trackingStream.TimesCalled(nameof(trackingStream.Read)));

            byte[] buffer = trackingStream.ReadBuffer;
            int offset = trackingStream.ReadOffset;
            int count = trackingStream.ReadCount;

            Assert.NotNull(buffer);
            Assert.True(offset >= 0 && offset + count <= buffer.Length);
            Assert.True(count > 0); // the buffer can't be size 0
        }

        [Fact]
        public async void AsyncIfCanSeekIsFalseLengthAndPositionShouldNotBeCalled()
        {
            var baseStream = new ConfigurablePropertyStream();
            baseStream.SetCanRead(true);
            baseStream.SetCanSeek(false);
            var trackingStream = new CallTrackingStream(baseStream);

            var dest = Stream.Null;
            await trackingStream.CopyToAsync(dest);

            // When the FEATURE_CORECLR conditional is removed around
            // the CanSeek optimization, this should be uncommented
            // Assert.Equal(1, trackingStream.TimesCalled(nameof(trackingStream.CanSeek)));

            Assert.Equal(0, trackingStream.TimesCalled(nameof(trackingStream.Length)));
            Assert.Equal(0, trackingStream.TimesCalled(nameof(trackingStream.Position)));
            Assert.Equal(1, trackingStream.TimesCalled(nameof(trackingStream.CopyToAsync)));

            Assert.Same(dest, trackingStream.CopyToAsyncDestination);
            Assert.True(trackingStream.CopyToAsyncBufferSize > 0);
            Assert.Equal(CancellationToken.None, trackingStream.CopyToAsyncCancellationToken);
        }

        [Fact]
        public void IfCanSeekIsTrueLengthAndPositionShouldOnlyBeCalledOnce()
        {
            var baseStream = new ConfigurablePropertyStream();
            baseStream.SetCanRead(true);
            baseStream.SetCanSeek(true);
            var trackingStream = new CallTrackingStream(baseStream);

            var dest = Stream.Null;
            trackingStream.CopyTo(dest);

            Assert.True(trackingStream.TimesCalled(nameof(trackingStream.Length)) <= 1);
            Assert.True(trackingStream.TimesCalled(nameof(trackingStream.Position)) <= 1);
        }

        [Fact]
        public async void AsyncIfCanSeekIsTrueLengthAndPositionShouldOnlyBeCalledOnce()
        {
            var baseStream = new ConfigurablePropertyStream();
            baseStream.SetCanRead(true);
            baseStream.SetCanSeek(true);
            var trackingStream = new CallTrackingStream(baseStream);

            var dest = Stream.Null;
            await trackingStream.CopyToAsync(dest);

            Assert.True(trackingStream.TimesCalled(nameof(trackingStream.Length)) <= 1);
            Assert.True(trackingStream.TimesCalled(nameof(trackingStream.Position)) <= 1);
        }

        [Theory]
        [MemberData(nameof(LengthIsLessThanOrEqualToPosition))]
        public void IfLengthIsLessThanOrEqualToPositionCopyToShouldStillBeCalledWithAPositiveBufferSize(long length, long position)
        {
            // Streams with their Lengths <= their Positions, e.g.
            // new MemoryStream { Position = 3 }.SetLength(1)
            // should still be called CopyTo{Async} on with a
            // bufferSize of at least 1.

            var baseStream = new ConfigurablePropertyStream();
            baseStream.SetCanRead(true);
            baseStream.SetCanSeek(true);
            baseStream.SetLength(length);
            baseStream.Position = position;
            var trackingStream = new CallTrackingStream(baseStream);

            var dest = Stream.Null;
            trackingStream.CopyTo(dest);

            // No argument checking needed; see notes below
        }

        [Theory]
        [MemberData(nameof(LengthIsLessThanOrEqualToPosition))]
        public async void AsyncIfLengthIsLessThanOrEqualToPositionCopyToShouldStillBeCalledWithAPositiveBufferSize(long length, long position)
        {
            var baseStream = new ConfigurablePropertyStream();
            baseStream.SetCanRead(true);
            baseStream.SetCanSeek(true);
            baseStream.SetLength(length);
            baseStream.Position = position;
            var trackingStream = new CallTrackingStream(baseStream);

            var dest = Stream.Null;
            await trackingStream.CopyToAsync(dest);

            Assert.Same(dest, trackingStream.CopyToAsyncDestination);
            Assert.True(trackingStream.CopyToAsyncBufferSize > 0);
            Assert.Equal(CancellationToken.None, trackingStream.CopyToAsyncCancellationToken);
        }

        [Theory]
        [MemberData(nameof(LengthMinusPositionPositiveOverflows))]
        public void IfLengthMinusPositionPositiveOverflowsBufferSizeShouldStillBePositive(long length, long position)
        {
            // The new implementation of Stream.CopyTo calculates the bytes left
            // in the Stream by calling Length - Position. This can overflow to a
            // negative number, so this tests that if that happens we don't send
            // in a negative bufferSize.

            var baseStream = new ConfigurablePropertyStream();
            baseStream.SetCanRead(true);
            baseStream.SetCanSeek(true);
            baseStream.SetLength(length);
            baseStream.Position = position;
            var trackingStream = new CallTrackingStream(baseStream);

            var dest = Stream.Null;
            trackingStream.CopyTo(dest);

            // We don't need to do any argument checking here;
            // if bufferSize was null CopyTo will throw an exception
            // during argument validation
        }

        [Theory]
        [MemberData(nameof(LengthMinusPositionPositiveOverflows))]
        public async void AsyncIfLengthMinusPositionPositiveOverflowsBufferSizeShouldStillBePositive(long length, long position)
        {
            var baseStream = new ConfigurablePropertyStream();
            baseStream.SetCanRead(true);
            baseStream.SetCanSeek(true);
            baseStream.SetLength(length);
            baseStream.Position = position;
            var trackingStream = new CallTrackingStream(baseStream);

            var dest = Stream.Null;
            await trackingStream.CopyToAsync(dest);
            
            // We do need to check the arguments here, since both
            // of the mock streams override CopyToAsync and don't
            // validate the arguments

            Assert.Same(dest, trackingStream.CopyToAsyncDestination);
            Assert.True(trackingStream.CopyToAsyncBufferSize > 0);
            Assert.Equal(CancellationToken.None, trackingStream.CopyToAsyncCancellationToken);
        }

        // Member data

        public static IEnumerable<object[]> LengthIsLessThanOrEqualToPosition()
        {
            yield return new object[] { 5L, 5L }; // same number
            yield return new object[] { 3L, 5L }; // length is less than position
            yield return new object[] { -1L, -1L }; // negative numbers
            yield return new object[] { 0L, 0L }; // both zero
            yield return new object[] { -500L, 0L }; // negative number and zero
            yield return new object[] { 0L, 500L }; // zero and positive number
            yield return new object[] { -500L, 500L }; // negative and positive number
            yield return new object[] { long.MinValue, long.MaxValue }; // length - position <= 0 will fail (overflow), but length <= position won't
        }

        public static IEnumerable<object[]> LengthMinusPositionPositiveOverflows()
        {
            yield return new object[] { long.MaxValue, long.MinValue }; // length - position will be -1
            yield return new object[] { 1L, -long.MaxValue };
        }
    }
}
