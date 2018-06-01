// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Linq;
using System.Text;
using Xunit;

namespace System.Memory.Tests
{
    public abstract class ReadOnlySequenceTestsChar: ReadOnlySequenceTests<char>
    {
        public class Array : ReadOnlySequenceTestsChar
        {
            public Array() : base(ReadOnlySequenceFactory<char>.ArrayFactory) { }
        }

        public class String : ReadOnlySequenceTestsChar
        {
            public String() : base(ReadOnlySequenceFactoryChar.StringFactory) { }
        }

        public class Memory : ReadOnlySequenceTestsChar
        {
            public Memory() : base(ReadOnlySequenceFactory<char>.MemoryFactory) { }
        }

        public class SingleSegment : ReadOnlySequenceTestsChar
        {
            public SingleSegment() : base(ReadOnlySequenceFactory<char>.SingleSegmentFactory) { }
        }

        public class SegmentPerChar : ReadOnlySequenceTestsChar
        {
            public SegmentPerChar() : base(ReadOnlySequenceFactory<char>.SegmentPerItemFactory) { }
        }

        public class SplitInThreeSegments : ReadOnlySequenceTestsChar
        {
            public SplitInThreeSegments() : base(ReadOnlySequenceFactory<char>.SplitInThree) { }
        }

        internal ReadOnlySequenceTestsChar(ReadOnlySequenceFactory<char> factory): base(factory) { }

        [Theory]
        [InlineData(1)]
        [InlineData(8)]
        public void ToArrayIsCorrect(int length)
        {
            char[] data = Enumerable.Range(0, length).Select(i => (char)i).ToArray();
            ReadOnlySequence<char> buffer = Factory.CreateWithContent(data);
            Assert.Equal(length, buffer.Length);
            Assert.Equal(data, buffer.ToArray());
        }

        [Fact]
        public void ToStringIsCorrect()
        {
            char[] array = Enumerable.Range(0, 255).Select(i => (char)i).ToArray();
            ReadOnlySequence<char> buffer = Factory.CreateWithContent(array);
            Assert.Equal(array, buffer.ToString());
        }

        [Theory]
        [MemberData(nameof(ValidSliceCases))]
        public void Slice_Works(Func<ReadOnlySequence<char>, ReadOnlySequence<char>> func)
        {
            ReadOnlySequence<char> buffer = Factory.CreateWithContent(new char[] { (char)0, (char)1, (char)2, (char)3, (char)4, (char)5, (char)6, (char)7, (char)8, (char)9 });
            ReadOnlySequence<char> slice = func(buffer);
            Assert.Equal(new char[] { (char)5, (char)6, (char)7, (char)8, (char)9 }, slice.ToArray());
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
            ReadOnlySequence<char> buffer = Factory.CreateWithContent(raw.ToCharArray());
            SequencePosition? result = buffer.PositionOf((char)searchFor);

            Assert.NotNull(result);
            Assert.Equal(buffer.Slice(result.Value).ToArray(), raw.Substring(expectIndex));
        }

        [Fact]
        public void PositionOf_ReturnsNullIfNotFound()
        {
            ReadOnlySequence<char> buffer = Factory.CreateWithContent(new char[] { (char)1, (char)2, (char)3 });
            SequencePosition? result = buffer.PositionOf((char)4);

            Assert.Null(result);
        }
    }
}
