// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using Xunit;

namespace System.Memory.Tests
{
    public class LongReadOnlySequenceTests
    {
        private const int SegmentLength = int.MaxValue / 16;
        private const long DefaultSequenceLength = (long)int.MaxValue * 3 / 2;
        private const int DefaultStartOffset = SegmentLength / 2;

        private static ReadOnlySequence<T> NewSequence<T>(int startOffset = DefaultStartOffset, long sequenceLength = DefaultSequenceLength)
        {
            var array = new T[SegmentLength];

            BufferSegment<T> start;
            BufferSegment<T> end;
            long length = 0;

            if (startOffset == 0)
            {
                // first segment is empty
                start = new BufferSegment<T>(new T[0]);
                end = start;
                // second segment is empty
                end = end.Append(new T[0]);
                // third segment is non empty
                end = end.Append(array);
                length += SegmentLength;
            }
            else
            {
                // first segment is non empty
                start = new BufferSegment<T>(array);
                end = start;
                length += SegmentLength;
            }

            // two empty segments
            end = end.Append(new T[0]);
            end = end.Append(new T[0]);

            // add non empty segments except last
            while (length + SegmentLength < sequenceLength)
            {
                end = end.Append(array);
                length += SegmentLength;
            }

            // two empty segment before last non empty
            end = end.Append(new T[0]);
            end = end.Append(new T[0]);

            // last non empty segment
            end = end.Append(array);
            length += SegmentLength;

            var endOffset = (int)(sequenceLength - end.RunningIndex) + startOffset;

            if (endOffset == SegmentLength)
            {
                // last two segments is empty
                end = end.Append(new T[0]);
                end = end.Append(new T[0]);
                endOffset = 0;
            }

            return new ReadOnlySequence<T>(start, startOffset, end, endOffset);
        }

        private static int GetInteger(long offset, int startOffset = DefaultStartOffset) => (int)((offset % SegmentLength + startOffset) % SegmentLength);
        private static long GetRunningIndex(long offset, int startOffset = DefaultStartOffset) => offset + startOffset - GetInteger(offset, startOffset);
        private static long GetRunningIndex<T>(SequencePosition position) => ((BufferSegment<T>)position.GetObject()).RunningIndex;
        private static int GetSegmentsCount<T>(ReadOnlySequence<T> buffer) =>
            (int)((buffer.Length + buffer.Start.GetInteger() + SegmentLength - 1) / SegmentLength);

        private static long GetOffset(int segment, int integer, int startOffset = DefaultStartOffset) => (long)segment * SegmentLength + integer - startOffset;

        private readonly ReadOnlySequence<byte> _byteSequence = NewSequence<byte>();

        [Fact]
        public void SegmentStartIsConsideredInBoundsCheck()
        {
            var buffer = _byteSequence;

            var offset1 = SegmentLength + 25;
            SequencePosition c1 = buffer.GetPosition(offset1);
            Assert.Equal(GetInteger(offset1), c1.GetInteger());
            Assert.Equal(GetRunningIndex(offset1), GetRunningIndex<byte>(c1));

            var offset2 = buffer.Length - 1;
            SequencePosition c2 = buffer.GetPosition(offset2);
            Assert.Equal(GetInteger(offset2), c2.GetInteger());
            Assert.Equal(GetRunningIndex(offset2), GetRunningIndex<byte>(c2));

            ReadOnlySequence<byte> sliced = buffer.Slice(c1, c2);
            Assert.Equal(offset2 - offset1, sliced.Length);
            Assert.Equal(GetInteger(offset1), sliced.Start.GetInteger());
            Assert.Equal(GetRunningIndex(offset1), GetRunningIndex<byte>(sliced.Start));

            c1 = buffer.GetPosition(offset1, buffer.Start);
            Assert.Equal(GetInteger(offset1), c1.GetInteger());
            Assert.Equal(GetRunningIndex(offset1), GetRunningIndex<byte>(c1));

            c2 = buffer.GetPosition(offset2, buffer.Start);
            Assert.Equal(GetInteger(offset2), c2.GetInteger());
            Assert.Equal(GetRunningIndex(offset2), GetRunningIndex<byte>(c2));

            sliced = buffer.Slice(c1, c2);
            Assert.Equal(offset2 - offset1, sliced.Length);
            Assert.Equal(GetInteger(offset1), sliced.Start.GetInteger());
            Assert.Equal(GetRunningIndex(offset1), GetRunningIndex<byte>(sliced.Start));
        }

        [Fact]
        public void GetPositionPrefersNextSegment()
        {
            var buffer = _byteSequence;

            // second not empty segment
            var offset1 = GetOffset(1, 0);
            SequencePosition c1 = buffer.GetPosition(offset1);
            Assert.Equal(0, c1.GetInteger());
            Assert.Equal(GetRunningIndex(offset1), GetRunningIndex<byte>(c1));
            Assert.Equal(SegmentLength, ((BufferSegment<byte>)c1.GetObject()).Memory.Length);

            c1 = buffer.GetPosition(offset1, buffer.Start);
            Assert.Equal(0, c1.GetInteger());
            Assert.Equal(GetRunningIndex(offset1), GetRunningIndex<byte>(c1));
            Assert.Equal(SegmentLength, ((BufferSegment<byte>)c1.GetObject()).Memory.Length);

            // last not empty segment
            offset1 = GetOffset(GetSegmentsCount(buffer) - 1, 0);
            c1 = buffer.GetPosition(offset1);
            Assert.Equal(0, c1.GetInteger());
            Assert.Equal(GetRunningIndex(offset1), GetRunningIndex<byte>(c1));
            Assert.Equal(SegmentLength, ((BufferSegment<byte>)c1.GetObject()).Memory.Length);

            c1 = buffer.GetPosition(offset1, buffer.Start);
            Assert.Equal(0, c1.GetInteger());
            Assert.Equal(GetRunningIndex(offset1), GetRunningIndex<byte>(c1));
            Assert.Equal(SegmentLength, ((BufferSegment<byte>)c1.GetObject()).Memory.Length);
        }

        [Fact]
        public void WriteAcessToArray()
        {
            var array = new[] {1, 2, 3};
            var buffer = new ReadOnlySequence<int>(array);
            Assert.Equal(array.GetType().Name, buffer.Start.GetObject().GetType().Name);

            ((int[])buffer.Start.GetObject())[1] = 4;
            Assert.Equal(4, array[1]);
        }

    }
}
