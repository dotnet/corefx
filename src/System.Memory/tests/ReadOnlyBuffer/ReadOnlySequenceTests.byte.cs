// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Linq;
using System.Text;
using Xunit;

namespace System.Memory.Tests
{
    public abstract class ReadOnlySequenceTestsByte: ReadOnlySequenceTests<byte>
    {
        public class Array : ReadOnlySequenceTestsByte
        {
            public Array() : base(ReadOnlySequenceFactory<byte>.ArrayFactory) { }
        }

        public class Memory : ReadOnlySequenceTestsByte
        {
            public Memory() : base(ReadOnlySequenceFactory<byte>.MemoryFactory) { }
        }

        public class SingleSegment : ReadOnlySequenceTestsByte
        {
            public SingleSegment() : base(ReadOnlySequenceFactory<byte>.SingleSegmentFactory) { }
        }

        public class SegmentPerByte : ReadOnlySequenceTestsByte
        {
            public SegmentPerByte() : base(ReadOnlySequenceFactory<byte>.SegmentPerItemFactory) { }
        }

        public class SplitInThreeSegments : ReadOnlySequenceTestsByte
        {
            public SplitInThreeSegments() : base(ReadOnlySequenceFactory<byte>.SplitInThree) { }
        }

        internal ReadOnlySequenceTestsByte(ReadOnlySequenceFactory<byte> factory): base(factory) { }

        [Theory]
        [InlineData(1)]
        [InlineData(8)]
        public void ToArrayIsCorrect(int length)
        {
            byte[] data = Enumerable.Range(0, length).Select(i => (byte)i).ToArray();
            ReadOnlySequence<byte> buffer = Factory.CreateWithContent(data);
            Assert.Equal(length, buffer.Length);
            Assert.Equal(data, buffer.ToArray());
        }

        [Fact]
        public void ToStringIsCorrect()
        {
            ReadOnlySequence<byte> buffer = Factory.CreateWithContent(Enumerable.Range(0, 255).Select(i => (byte)i).ToArray());
            Assert.Equal("System.Buffers.ReadOnlySequence<Byte>[255]", buffer.ToString());
        }

        [Theory]
        [MemberData(nameof(ValidSliceCases))]
        public void Slice_Works(Func<ReadOnlySequence<byte>, ReadOnlySequence<byte>> func)
        {
            ReadOnlySequence<byte> buffer = Factory.CreateWithContent(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            ReadOnlySequence<byte> slice = func(buffer);
            Assert.Equal(new byte[] { 5, 6, 7, 8, 9 }, slice.ToArray());
        }

        [Theory]
        [InlineData("a", 'a', 0)]
        [InlineData("ab", 'a', 0)]
        [InlineData("aab", 'a', 0)]
        [InlineData("acab", 'a', 0)]
        [InlineData("acab", 'c', 1)]
        [InlineData("abcdefghijklmnopqrstuvwxyz", 'l', 11)]
        [InlineData("abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz", 'l', 11)]
        [InlineData("aaaaaaaaaaacmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz", 'm', 12)]
        [InlineData("aaaaaaaaaaarmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz", 'r', 11)]
        [InlineData("/localhost:5000/PATH/%2FPATH2/ HTTP/1.1", '%', 21)]
        [InlineData("/localhost:5000/PATH/%2FPATH2/?key=value HTTP/1.1", '%', 21)]
        [InlineData("/localhost:5000/PATH/PATH2/?key=value HTTP/1.1", '?', 27)]
        [InlineData("/localhost:5000/PATH/PATH2/ HTTP/1.1", ' ', 27)]
        public void PositionOf_ReturnsPosition(string raw, char searchFor, int expectIndex)
        {
            ReadOnlySequence<byte> buffer = Factory.CreateWithContent(Encoding.ASCII.GetBytes(raw));
            SequencePosition? result = buffer.PositionOf((byte)searchFor);

            Assert.NotNull(result);
            Assert.Equal(buffer.Slice(result.Value).ToArray(), Encoding.ASCII.GetBytes(raw.Substring(expectIndex)));
        }

        [Fact]
        public void PositionOf_ReturnsNullIfNotFound()
        {
            ReadOnlySequence<byte> buffer = Factory.CreateWithContent(new byte[] { 1, 2, 3 });
            SequencePosition? result = buffer.PositionOf((byte)4);

            Assert.Null(result);
        }
    }
}
