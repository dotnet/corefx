// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Linq;
using Xunit;

namespace System.Memory.Tests
{
    public abstract class ReadOnlyBufferFacts
    {
        public class Array: ReadOnlyBufferFacts
        {
            public Array() : base(ReadOnlyBufferFactory.Array) { }
        }

        public class OwnedMemory: ReadOnlyBufferFacts
        {
            public OwnedMemory() : base(ReadOnlyBufferFactory.OwnedMemory) { }
        }

        public class SingleSegment: ReadOnlyBufferFacts
        {
            public SingleSegment() : base(ReadOnlyBufferFactory.SingleSegment) { }
        }

        public class SegmentPerByte: ReadOnlyBufferFacts
        {
            public SegmentPerByte() : base(ReadOnlyBufferFactory.SegmentPerByte) { }
        }

        internal ReadOnlyBufferFactory Factory { get; }

        internal ReadOnlyBufferFacts(ReadOnlyBufferFactory factory)
        {
            Factory = factory;
        }

        [Fact]
        public void EmptyIsCorrect()
        {
            var buffer = Factory.CreateOfSize(0);
            Assert.Equal(0, buffer.Length);
            Assert.True(buffer.IsEmpty);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(8)]
        public void LengthIsCorrect(int length)
        {
            var buffer = Factory.CreateOfSize(length);
            Assert.Equal(length, buffer.Length);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(8)]
        public void ToArrayIsCorrect(int length)
        {
            var data = Enumerable.Range(0, length).Select(i => (byte)i).ToArray();
            var buffer = Factory.CreateWithContent(data);
            Assert.Equal(length, buffer.Length);
            Assert.Equal(data, buffer.ToArray());
        }

        [Theory]
        [MemberData(nameof(OutOfRangeSliceCases))]
        public void ReadOnlyBufferDoesNotAllowSlicingOutOfRange(Action<ReadOnlyBuffer<byte>> fail)
        {
            var buffer = Factory.CreateOfSize(100);
            Assert.Throws<ArgumentOutOfRangeException>(() => fail(buffer));
        }

        [Fact]
        public void ReadOnlyBufferGetPosition_MovesPosition()
        {
            var buffer = Factory.CreateOfSize(100);
            var position = buffer.GetPosition(buffer.Start, 65);
            Assert.Equal(buffer.Slice(65).Start, position);
        }

        [Fact]
        public void ReadOnlyBufferGetPosition_ChecksBounds()
        {
            var buffer = Factory.CreateOfSize(100);
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.GetPosition(buffer.Start, 101));
        }

        [Fact]
        public void ReadOnlyBufferGetPosition_DoesNotAlowNegative()
        {
            var buffer = Factory.CreateOfSize(20);
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.GetPosition(buffer.Start, -1));
        }

        public void ReadOnlyBufferSlice_ChecksEnd()
        {
            var buffer = Factory.CreateOfSize(100);
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.Slice(70, buffer.Start));
        }

        [Fact]
        public void SegmentStartIsConsideredInBoundsCheck()
        {
            // 0               50           100    0             50             100
            // [                ##############] -> [##############                ]
            //                         ^c1            ^c2
            var bufferSegment1 = new BufferSegment();
            bufferSegment1.Memory = new byte[49];

            var bufferSegment2 = new BufferSegment();
            bufferSegment2.Memory = new byte[50];
            bufferSegment1.SetNext(bufferSegment2);

            var buffer = new ReadOnlyBuffer<byte>(bufferSegment1, 0, bufferSegment2, 50);

            var c1 = buffer.GetPosition(buffer.Start, 25); // segment 1 index 75
            var c2 = buffer.GetPosition(buffer.Start, 55); // segment 2 index 5

            var sliced = buffer.Slice(c1, c2);

            Assert.Equal(30, sliced.Length);
        }

        [Fact]
        public void GetPositionPrefersNextSegment()
        {
            var bufferSegment1 = new BufferSegment
            {
                Memory = new byte[50]
            };

            var bufferSegment2 = new BufferSegment
            {
                Memory = new byte[0]
            };

            bufferSegment1.SetNext(bufferSegment2);

            var buffer = new ReadOnlyBuffer<byte>(bufferSegment1, 0, bufferSegment2, 0);

            var c1 = buffer.GetPosition(buffer.Start, 50);

            Assert.Equal(0, c1.Index);
            Assert.Equal(bufferSegment2, c1.Segment);
        }

        [Fact]
        public void GetPositionDoesNotCrossOutsideBuffer()
        {
            var bufferSegment1 = new BufferSegment
            {
                Memory = new byte[100]
            };

            var bufferSegment2 = new BufferSegment
            {
                Memory = new byte[100]
            };

            var bufferSegment3 = new BufferSegment
            {
                Memory = new byte[0]
            };

            bufferSegment1.SetNext(bufferSegment2);
            bufferSegment2.SetNext(bufferSegment3);

            var buffer = new ReadOnlyBuffer<byte>(bufferSegment1, 0, bufferSegment2, 100);

            var c1 = buffer.GetPosition(buffer.Start, 200);

            Assert.Equal(100, c1.Index);
            Assert.Equal(bufferSegment2, c1.Segment);
        }

        [Fact]
        public void Create_WorksWithArray()
        {
            var buffer = new ReadOnlyBuffer<byte>(new byte[] {1, 2, 3, 4, 5}, 2, 3);
            Assert.Equal(buffer.ToArray(), new byte[] {3, 4, 5});
        }

        [Fact]
        public void Create_WorksWithMemory()
        {
            var memory = new Memory<byte>(new byte[] {1, 2, 3, 4, 5});
            var buffer = new ReadOnlyBuffer<byte>(memory.Slice(2, 3));
            Assert.Equal(new byte[] {3, 4, 5}, buffer.ToArray());
        }

        [Fact]
        public void Create_WorksWithIEnumerableOfMemory()
        {
            var memories = new Memory<byte>[] { new byte[] {1, 2, 3}, new byte[] {4, 5, 6}};
            var buffer = new ReadOnlyBuffer<byte>(memories);

            Assert.Equal(new byte[] {1, 2, 3, 4, 5, 6}, buffer.ToArray());
        }

        [Fact]
        public void SliceToTheEndWorks()
        {
            var buffer = Factory.CreateOfSize(10);
            Assert.True(buffer.Slice(buffer.End).IsEmpty);
        }

        public static TheoryData<Action<ReadOnlyBuffer<byte>>> OutOfRangeSliceCases => new TheoryData<Action<ReadOnlyBuffer<byte>>>
        {
            b => b.Slice(101),
            b => b.Slice(0, 101),
            b => b.Slice(b.Start, 101),
            b => b.Slice(0, 70).Slice(b.End, b.End),
            b => b.Slice(0, 70).Slice(b.Start, b.End),
            b => b.Slice(0, 70).Slice(0, b.End)
        };
    }
}
