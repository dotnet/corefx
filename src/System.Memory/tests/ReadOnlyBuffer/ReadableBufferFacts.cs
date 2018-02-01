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
        public class Array : ReadOnlyBufferFacts
        {
            public Array() : base(ReadOnlyBufferFactory.ArrayFactory) { }
        }

        public class OwnedMemory : ReadOnlyBufferFacts
        {
            public OwnedMemory() : base(ReadOnlyBufferFactory.MemoryFactory) { }
        }

        public class SingleSegment : ReadOnlyBufferFacts
        {
            public SingleSegment() : base(ReadOnlyBufferFactory.SingleSegmentFactory) { }
        }

        public class SegmentPerByte : ReadOnlyBufferFacts
        {
            public SegmentPerByte() : base(ReadOnlyBufferFactory.SegmentPerByteFactory) { }
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

        [Fact]
        public void ToStringIsCorrect()
        {
            var buffer = Factory.CreateWithContent(Enumerable.Range(0, 255).Select(i => (byte)i).ToArray());
            Assert.Equal(
                "\\0\\u0001\\u0002\\u0003\\u0004\\u0005\\u0006\\a\\b\\t\\n\\v\\f\\r\\u000e\\u000f\\u0010\\u0011\\u0012\\u0013\\u0014\\u0015\\u0016\\u0017\\u0018\\u0019\\u001a\\u001b\\u001c\\u001d\\u001e" +
                "\\u001f !\\\"#$%&\\\'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\\\]^_`abcdefghijklmnopqrstuvwxyz{|}~\\u007f\\u0080\\u0081\\u0082\\u0083\\u0084\\u0085\\u0086\\u0087\\u0088\\u0089" +
                "\\u008a\\u008b\\u008c\\u008d\\u008e\\u008f\\u0090\\u0091\\u0092\\u0093\\u0094\\u0095\\u0096\\u0097\\u0098\\u0099\\u009a\\u009b\\u009c\\u009d\\u009e\\u009f\\u00a0\\u00a1\\u00a2\\u00a3\\u00a4" +
                "\\u00a5\\u00a6\\u00a7\\u00a8\\u00a9\\u00aa\\u00ab\\u00ac\\u00ad\\u00ae\\u00af\\u00b0\\u00b1\\u00b2\\u00b3\\u00b4\\u00b5\\u00b6\\u00b7\\u00b8\\u00b9\\u00ba\\u00bb\\u00bc\\u00bd\\u00be\\u00bf" +
                "\\u00c0\\u00c1\\u00c2\\u00c3\\u00c4\\u00c5\\u00c6\\u00c7\\u00c8\\u00c9\\u00ca\\u00cb\\u00cc\\u00cd\\u00ce\\u00cf\\u00d0\\u00d1\\u00d2\\u00d3\\u00d4\\u00d5\\u00d6\\u00d7\\u00d8\\u00d9\\u00da" +
                "\\u00db\\u00dc\\u00dd\\u00de\\u00df\\u00e0\\u00e1\\u00e2\\u00e3\\u00e4\\u00e5\\u00e6\\u00e7\\u00e8\\u00e9\\u00ea\\u00eb\\u00ec\\u00ed\\u00ee\\u00ef\\u00f0\\u00f1\\u00f2\\u00f3\\u00f4\\u00f5" +
                "\\u00f6\\u00f7\\u00f8\\u00f9\\u00fa\\u00fb\\u00fc\\u00fd\\u00fe",
                buffer.ToString());
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
            var bufferSegment1 = new BufferSegment(new byte[49]);
            var bufferSegment2 = bufferSegment1.Append(new byte[50]);

            var buffer = new ReadOnlyBuffer<byte>(bufferSegment1, 0, bufferSegment2, 50);

            var c1 = buffer.GetPosition(buffer.Start, 25); // segment 1 index 75
            var c2 = buffer.GetPosition(buffer.Start, 55); // segment 2 index 5

            var sliced = buffer.Slice(c1, c2);

            Assert.Equal(30, sliced.Length);
        }

        [Fact]
        public void GetPositionPrefersNextSegment()
        {
            var bufferSegment1 = new BufferSegment(new byte[50]);
            var bufferSegment2 = bufferSegment1.Append(new byte[0]);

            var buffer = new ReadOnlyBuffer<byte>(bufferSegment1, 0, bufferSegment2, 0);

            var c1 = buffer.GetPosition(buffer.Start, 50);

            Assert.Equal(0, c1.Index);
            Assert.Equal(bufferSegment2, c1.Segment);
        }

        [Fact]
        public void GetPositionDoesNotCrossOutsideBuffer()
        {
            var bufferSegment1 = new BufferSegment(new byte[100]);
            var bufferSegment2 = bufferSegment1.Append(new byte[100]);
            var bufferSegment3 = bufferSegment2.Append(new byte[0]);

            var buffer = new ReadOnlyBuffer<byte>(bufferSegment1, 0, bufferSegment2, 100);

            var c1 = buffer.GetPosition(buffer.Start, 200);

            Assert.Equal(100, c1.Index);
            Assert.Equal(bufferSegment2, c1.Segment);
        }

        [Fact]
        public void Create_WorksWithArray()
        {
            var buffer = new ReadOnlyBuffer<byte>(new byte[] { 1, 2, 3, 4, 5 }, 2, 3);
            Assert.Equal(buffer.ToArray(), new byte[] { 3, 4, 5 });
        }

        [Fact]
        public void Create_WorksWithMemory()
        {
            var memory = new Memory<byte>(new byte[] { 1, 2, 3, 4, 5 });
            var buffer = new ReadOnlyBuffer<byte>(memory.Slice(2, 3));
            Assert.Equal(new byte[] { 3, 4, 5 }, buffer.ToArray());
        }

        [Fact]
        public void Create_WorksWithIEnumerableOfMemory()
        {
            var memories = new Memory<byte>[] { new byte[] { 1, 2, 3 }, new byte[] { 4, 5, 6 } };
            var buffer = new ReadOnlyBuffer<byte>(memories);

            Assert.Equal(new byte[] { 1, 2, 3, 4, 5, 6 }, buffer.ToArray());
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
