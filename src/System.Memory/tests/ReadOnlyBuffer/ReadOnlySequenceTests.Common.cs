// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.MemoryTests;
using System.Text;
using Xunit;

namespace System.Memory.Tests
{
    public class CommonReadOnlySequenceTests
    {
        #region region Position

        [Theory]
        [InlineData(0)]
        [InlineData(long.MaxValue - 100)]
        [InlineData(-100)]
        public void SegmentStartIsConsideredInBoundsCheck(long startIndex)
        {
            // 0               50           100    0             50             100 
            // [                ##############] -> [##############                ]
            //                         ^c1            ^c2
            BufferSegment<byte> bufferSegment1 = new BufferSegment<byte>(new byte[100], startIndex);
            BufferSegment<byte> bufferSegment2 = bufferSegment1.Append(new byte[100]);

            ReadOnlySequence<byte> buffer = new ReadOnlySequence<byte>(bufferSegment1, 50, bufferSegment2, 50);

            SequencePosition c1 = buffer.GetPosition(25); // segment 1 index 75
            SequencePosition c2 = buffer.GetPosition(55); // segment 2 index 5
            Assert.Equal(5, c2.GetInteger());
            Assert.Equal(bufferSegment2, c2.GetObject());
            Assert.Equal(c2, buffer.GetPosition(55, buffer.Start));

            ReadOnlySequence<byte> sliced = buffer.Slice(c1, c2);
            Assert.Equal(30, sliced.Length);

            Assert.Equal(c1, buffer.GetPosition(25, buffer.Start)); // segment 1 index 75
            Assert.Equal(c2, buffer.GetPosition(55, buffer.Start)); // segment 2 index 5
        }

        [Theory]
        [InlineData(0)]
        [InlineData(long.MaxValue - 25)]
        [InlineData(-25)]
        public void GetPositionPrefersNextSegment(long startIndex)
        {
            BufferSegment<byte> bufferSegment1 = new BufferSegment<byte>(new byte[50], startIndex);
            BufferSegment<byte> bufferSegment2 = bufferSegment1.Append(new byte[0]);

            ReadOnlySequence<byte> buffer = new ReadOnlySequence<byte>(bufferSegment1, 0, bufferSegment2, 0);

            SequencePosition c1 = buffer.GetPosition(50);
            Assert.Equal(0, c1.GetInteger());
            Assert.Equal(bufferSegment2, c1.GetObject());

            c1 = buffer.GetPosition(50, buffer.Start);
            Assert.Equal(0, c1.GetInteger());
            Assert.Equal(bufferSegment2, c1.GetObject());
        }

        [Theory]
        [InlineData(0)]
        [InlineData(long.MaxValue - 100)]
        [InlineData(-100)]
        public void GetPositionDoesNotCrossOutsideBuffer(long startIndex)
        {
            BufferSegment<byte> bufferSegment1 = new BufferSegment<byte>(new byte[100], startIndex);
            BufferSegment<byte> bufferSegment2 = bufferSegment1.Append(new byte[100]);
            BufferSegment<byte> bufferSegment3 = bufferSegment2.Append(new byte[0]);

            ReadOnlySequence<byte> buffer = new ReadOnlySequence<byte>(bufferSegment1, 0, bufferSegment2, 100);

            SequencePosition c1 = buffer.GetPosition(200);
            Assert.Equal(100, c1.GetInteger());
            Assert.Equal(bufferSegment2, c1.GetObject());

            c1 = buffer.GetPosition(200, buffer.Start);
            Assert.Equal(100, c1.GetInteger());
            Assert.Equal(bufferSegment2, c1.GetObject());
        }

        [Theory]
        [InlineData(0)]
        [InlineData(long.MaxValue - 200)]
        [InlineData(-200)]
        public void CheckEndReachableDoesNotCrossPastEnd(long startIndex)
        {
            BufferSegment<byte> bufferSegment1 = new BufferSegment<byte>(new byte[100], startIndex);
            BufferSegment<byte> bufferSegment2 = bufferSegment1.Append(new byte[100]);
            BufferSegment<byte> bufferSegment3 = bufferSegment2.Append(new byte[100]);
            BufferSegment<byte> bufferSegment4 = bufferSegment3.Append(new byte[100]);

            ReadOnlySequence<byte> buffer = new ReadOnlySequence<byte>(bufferSegment1, 0, bufferSegment4, 100);

            SequencePosition c1 = buffer.GetPosition(200);
            Assert.Equal(0, c1.GetInteger());
            Assert.Equal(bufferSegment3, c1.GetObject());

            ReadOnlySequence<byte> seq = buffer.Slice(0, c1);
            Assert.Equal(200, seq.Length);

            c1 = buffer.GetPosition(200, buffer.Start);
            Assert.Equal(0, c1.GetInteger());
            Assert.Equal(bufferSegment3, c1.GetObject());

            seq = buffer.Slice(0, c1);
            Assert.Equal(200, seq.Length);
        }

        #endregion

        #region First

        [Theory]
        [InlineData(0)]
        [InlineData(long.MaxValue - 250)]
        [InlineData(-250)]
        public void CanGetFirst(long startIndex)
        {
            BufferSegment<byte> bufferSegment1 = new BufferSegment<byte>(new byte[100], startIndex);
            BufferSegment<byte> bufferSegment2 = bufferSegment1.Append(new byte[100]);
            BufferSegment<byte> bufferSegment3 = bufferSegment2.Append(new byte[100]);
            BufferSegment<byte> bufferSegment4 = bufferSegment3.Append(new byte[200]);

            ReadOnlySequence<byte> buffer = new ReadOnlySequence<byte>(bufferSegment1, 0, bufferSegment4, 200);

            Assert.Equal(500, buffer.Length);
            int length = 500;

            for (int s = 0; s < 3; s++)
            {
                for (int i = 100; i > 0; i--)
                {
                    Assert.Equal(i, buffer.First.Length);
                    buffer = buffer.Slice(1);
                    length--;
                    Assert.Equal(length, buffer.Length);
                }
            }

            Assert.Equal(200, buffer.Length);

            for (int i = 200; i > 0; i--)
            {
                Assert.Equal(i, buffer.First.Length);
                buffer = buffer.Slice(1);
                length--;
                Assert.Equal(length, buffer.Length);
            }

            Assert.Equal(0, buffer.Length);
            Assert.Equal(0, buffer.First.Length);
        }

        #endregion

        #region EmptySegments

        [Theory]
        [InlineData(0)]
        [InlineData(long.MaxValue - 100)]
        [InlineData(-100)]
        public void SeekSkipsEmptySegments(long startIndex)
        {
            BufferSegment<byte> bufferSegment1 = new BufferSegment<byte>(new byte[100], startIndex);
            BufferSegment<byte> bufferSegment2 = bufferSegment1.Append(new byte[0]);
            BufferSegment<byte> bufferSegment3 = bufferSegment2.Append(new byte[0]);
            BufferSegment<byte> bufferSegment4 = bufferSegment3.Append(new byte[100]);

            ReadOnlySequence<byte> buffer = new ReadOnlySequence<byte>(bufferSegment1, 0, bufferSegment4, 100);

            SequencePosition c1 = buffer.GetPosition(100);

            Assert.Equal(0, c1.GetInteger());
            Assert.Equal(bufferSegment4, c1.GetObject());

            c1 = buffer.GetPosition(100, buffer.Start);

            Assert.Equal(0, c1.GetInteger());
            Assert.Equal(bufferSegment4, c1.GetObject());
        }

        [Theory]
        [InlineData(0)]
        [InlineData(long.MaxValue)]
        [InlineData(long.MinValue)]
        [InlineData(-1)]
        public void TryGetReturnsEmptySegments(long startIndex)
        {
            BufferSegment<byte> bufferSegment1 = new BufferSegment<byte>(new byte[0], startIndex);
            BufferSegment<byte> bufferSegment2 = bufferSegment1.Append(new byte[0]);
            BufferSegment<byte> bufferSegment3 = bufferSegment2.Append(new byte[0]);
            BufferSegment<byte> bufferSegment4 = bufferSegment3.Append(new byte[0]);

            ReadOnlySequence<byte> buffer = new ReadOnlySequence<byte>(bufferSegment1, 0, bufferSegment3, 0);

            ReadOnlyMemory<byte> memory;
            SequencePosition start = buffer.Start;
            Assert.True(buffer.TryGet(ref start, out memory));
            Assert.Equal(0, memory.Length);
            Assert.True(buffer.TryGet(ref start, out memory));
            Assert.Equal(0, memory.Length);
            Assert.True(buffer.TryGet(ref start, out memory));
            Assert.Equal(0, memory.Length);
            Assert.False(buffer.TryGet(ref start, out memory));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(long.MaxValue - 50)]
        [InlineData(-50)]
        public void SeekEmptySkipDoesNotCrossPastEnd(long startIndex)
        {
            BufferSegment<byte> bufferSegment1 = new BufferSegment<byte>(new byte[100], startIndex);
            BufferSegment<byte> bufferSegment2 = bufferSegment1.Append(new byte[0]);
            BufferSegment<byte> bufferSegment3 = bufferSegment2.Append(new byte[0]);
            BufferSegment<byte> bufferSegment4 = bufferSegment3.Append(new byte[100]);

            ReadOnlySequence<byte> buffer = new ReadOnlySequence<byte>(bufferSegment1, 0, bufferSegment2, 0);

            SequencePosition c1 = buffer.GetPosition(100);
            Assert.Equal(0, c1.GetInteger());
            Assert.Equal(bufferSegment2, c1.GetObject());

            c1 = buffer.GetPosition(100, buffer.Start);
            Assert.Equal(0, c1.GetInteger());
            Assert.Equal(bufferSegment2, c1.GetObject());

            // Go out of bounds for segment
            Assert.Throws<ArgumentOutOfRangeException>("offset", () => buffer.GetPosition(150, buffer.Start));
            Assert.Throws<ArgumentOutOfRangeException>("offset", () => buffer.GetPosition(250, buffer.Start));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(long.MaxValue - 50)]
        [InlineData(-50)]
        public void SeekEmptySkipDoesNotCrossPastEndWithExtraChainedBlocks(long startIndex)
        {
            BufferSegment<byte> bufferSegment1 = new BufferSegment<byte>(new byte[100], startIndex);
            BufferSegment<byte> bufferSegment2 = bufferSegment1.Append(new byte[0]);
            BufferSegment<byte> bufferSegment3 = bufferSegment2.Append(new byte[0]);
            BufferSegment<byte> bufferSegment4 = bufferSegment3.Append(new byte[100]);
            BufferSegment<byte> bufferSegment5 = bufferSegment4.Append(new byte[0]);
            BufferSegment<byte> bufferSegment6 = bufferSegment5.Append(new byte[100]);

            ReadOnlySequence<byte> buffer = new ReadOnlySequence<byte>(bufferSegment1, 0, bufferSegment2, 0);

            SequencePosition c1 = buffer.GetPosition(100);
            Assert.Equal(0, c1.GetInteger());
            Assert.Equal(bufferSegment2, c1.GetObject());

            c1 = buffer.GetPosition(100, buffer.Start);
            Assert.Equal(0, c1.GetInteger());
            Assert.Equal(bufferSegment2, c1.GetObject());

            // Go out of bounds for segment
            Assert.Throws<ArgumentOutOfRangeException>("offset", () => c1 = buffer.GetPosition(150, buffer.Start));
            Assert.Throws<ArgumentOutOfRangeException>("offset", () => c1 = buffer.GetPosition(250, buffer.Start));
        }

        #endregion

        #region TryGet

        [Theory]
        [InlineData(0)]
        [InlineData(long.MaxValue - 150)]
        [InlineData(-150)]
        public void TryGetStopsAtEnd(long startIndex)
        {
            BufferSegment<byte> bufferSegment1 = new BufferSegment<byte>(new byte[100], startIndex);
            BufferSegment<byte> bufferSegment2 = bufferSegment1.Append(new byte[100]);
            BufferSegment<byte> bufferSegment3 = bufferSegment2.Append(new byte[100]);
            BufferSegment<byte> bufferSegment4 = bufferSegment3.Append(new byte[100]);
            BufferSegment<byte> bufferSegment5 = bufferSegment4.Append(new byte[100]);

            ReadOnlySequence<byte> buffer = new ReadOnlySequence<byte>(bufferSegment1, 0, bufferSegment3, 100);

            ReadOnlyMemory<byte> memory;
            SequencePosition start = buffer.Start;
            Assert.True(buffer.TryGet(ref start, out memory));
            Assert.Equal(100, memory.Length);
            Assert.True(buffer.TryGet(ref start, out memory));
            Assert.Equal(100, memory.Length);
            Assert.True(buffer.TryGet(ref start, out memory));
            Assert.Equal(100, memory.Length);
            Assert.False(buffer.TryGet(ref start, out memory));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(long.MaxValue - 50)]
        [InlineData(-50)]
        public void TryGetStopsAtEndWhenEndIsLastByteOfFull(long startIndex)
        {
            BufferSegment<byte> bufferSegment1 = new BufferSegment<byte>(new byte[100], startIndex);
            BufferSegment<byte> bufferSegment2 = bufferSegment1.Append(new byte[100]);

            ReadOnlySequence<byte> buffer = new ReadOnlySequence<byte>(bufferSegment1, 0, bufferSegment1, 100);

            ReadOnlyMemory<byte> memory;
            SequencePosition start = buffer.Start;
            Assert.True(buffer.TryGet(ref start, out memory));
            Assert.Equal(100, memory.Length);
            Assert.False(buffer.TryGet(ref start, out memory));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(long.MaxValue - 50)]
        [InlineData(-100)]
        public void TryGetStopsAtEndWhenEndIsFirstByteOfFull(long startIndex)
        {
            BufferSegment<byte> bufferSegment1 = new BufferSegment<byte>(new byte[100], startIndex);
            BufferSegment<byte> bufferSegment2 = bufferSegment1.Append(new byte[100]);

            ReadOnlySequence<byte> buffer = new ReadOnlySequence<byte>(bufferSegment1, 0, bufferSegment2, 0);

            ReadOnlyMemory<byte> memory;
            SequencePosition start = buffer.Start;
            Assert.True(buffer.TryGet(ref start, out memory));
            Assert.Equal(100, memory.Length);
            Assert.True(buffer.TryGet(ref start, out memory));
            Assert.Equal(0, memory.Length);
            Assert.False(buffer.TryGet(ref start, out memory));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(long.MaxValue - 50)]
        [InlineData(-50)]
        public void TryGetStopsAtEndWhenEndIsFirstByteOfEmpty(long startIndex)
        {
            BufferSegment<byte> bufferSegment1 = new BufferSegment<byte>(new byte[100], startIndex);
            BufferSegment<byte> bufferSegment2 = bufferSegment1.Append(new byte[0]);

            ReadOnlySequence<byte> buffer = new ReadOnlySequence<byte>(bufferSegment1, 0, bufferSegment2, 0);

            ReadOnlyMemory<byte> memory;
            SequencePosition start = buffer.Start;
            Assert.True(buffer.TryGet(ref start, out memory));
            Assert.Equal(100, memory.Length);
            Assert.True(buffer.TryGet(ref start, out memory));
            Assert.Equal(0, memory.Length);
            Assert.False(buffer.TryGet(ref start, out memory));
        }

        #endregion

        #region Enumerable

        [Theory]
        [InlineData(0)]
        [InlineData(long.MaxValue - 50)]
        [InlineData(-50)]
        public void EnumerableStopsAtEndWhenEndIsLastByteOfFull(long startIndex)
        {
            BufferSegment<byte> bufferSegment1 = new BufferSegment<byte>(new byte[100], startIndex);
            BufferSegment<byte> bufferSegment2 = bufferSegment1.Append(new byte[100]);

            ReadOnlySequence<byte> buffer = new ReadOnlySequence<byte>(bufferSegment1, 0, bufferSegment1, 100);

            List<int> sizes = new List<int>();
            foreach (ReadOnlyMemory<byte> memory in buffer)
            {
                sizes.Add(memory.Length);
            }

            Assert.Equal(1, sizes.Count);
            Assert.Equal(new [] { 100 }, sizes);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(long.MaxValue - 50)]
        [InlineData(-50)]
        public void EnumerableStopsAtEndWhenEndIsFirstByteOfFull(long startIndex)
        {
            BufferSegment<byte> bufferSegment1 = new BufferSegment<byte>(new byte[100], startIndex);
            BufferSegment<byte> bufferSegment2 = bufferSegment1.Append(new byte[100]);

            ReadOnlySequence<byte> buffer = new ReadOnlySequence<byte>(bufferSegment1, 0, bufferSegment2, 0);

            List<int> sizes = new List<int>();
            foreach (ReadOnlyMemory<byte> memory in buffer)
            {
                sizes.Add(memory.Length);
            }

            Assert.Equal(2, sizes.Count);
            Assert.Equal(new [] { 100, 0 }, sizes);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(long.MaxValue - 50)]
        [InlineData(-50)]
        public void EnumerableStopsAtEndWhenEndIsFirstByteOfEmpty(long startIndex)
        {
            BufferSegment<byte> bufferSegment1 = new BufferSegment<byte>(new byte[100], startIndex);
            BufferSegment<byte> bufferSegment2 = bufferSegment1.Append(new byte[0]);

            ReadOnlySequence<byte> buffer = new ReadOnlySequence<byte>(bufferSegment1, 0, bufferSegment2, 0);

            List<int> sizes = new List<int>();
            foreach (ReadOnlyMemory<byte> memory in buffer)
            {
                sizes.Add(memory.Length);
            }

            Assert.Equal(2, sizes.Count);
            Assert.Equal(new [] { 100, 0 }, sizes);
        }

        #endregion

        #region Empty

        [Fact]
        public void Empty_ReturnsLengthZeroBuffer()
        {
            ReadOnlySequence<byte> buffer = ReadOnlySequence<byte>.Empty;
            Assert.Equal(0, buffer.Length);
            Assert.True(buffer.IsEmpty);
            Assert.True(buffer.IsSingleSegment);

            // Assert.Equal(buffer.Start, buffer.End);
            Assert.Equal(buffer.Start.GetObject(), buffer.End.GetObject());
            Assert.Equal(0, buffer.Start.GetInteger() & int.MaxValue);
            Assert.Equal(0, buffer.End.GetInteger() & int.MaxValue);

            Assert.True(buffer.First.IsEmpty);
            Assert.Equal($"System.Buffers.ReadOnlySequence<{typeof(byte).Name}>[0]", buffer.ToString());
        }

        [Fact]
        public void Empty_Slice()
        {
            ReadOnlySequence<byte> buffer = ReadOnlySequence<byte>.Empty;
            Assert.Equal(buffer, buffer.Slice(0, 0));
            Assert.Equal(buffer, buffer.Slice(0, buffer.End));
            Assert.Equal(buffer, buffer.Slice(0));
            Assert.Equal(buffer, buffer.Slice(0L, 0L));
            Assert.Equal(buffer, buffer.Slice(0L, buffer.End));
            Assert.Equal(buffer, buffer.Slice(buffer.Start));
            Assert.Equal(buffer, buffer.Slice(buffer.Start, 0));
            Assert.Equal(buffer, buffer.Slice(buffer.Start, 0L));
            Assert.Equal(buffer, buffer.Slice(buffer.Start, buffer.End));
        }

        [Fact]
        public void Empty_SlicePositiveStart()
        {
            ReadOnlySequence<byte> buffer = ReadOnlySequence<byte>.Empty;
            Assert.Throws<ArgumentOutOfRangeException>("offset", () => buffer.Slice(1, 0));
            Assert.Throws<ArgumentOutOfRangeException>("offset", () => buffer.Slice(1, 0));
            Assert.Throws<ArgumentOutOfRangeException>("offset", () => buffer.Slice(1, buffer.End));
            Assert.Throws<ArgumentOutOfRangeException>("offset", () => buffer.Slice(1));
            Assert.Throws<ArgumentOutOfRangeException>("offset", () => buffer.Slice(1L, 0L));
            Assert.Throws<ArgumentOutOfRangeException>("offset", () => buffer.Slice(1L, buffer.End));
        }

        [Fact]
        public void Empty_SliceNegativeStart()
        {
            ReadOnlySequence<byte> buffer = ReadOnlySequence<byte>.Empty;
            Assert.Throws<ArgumentOutOfRangeException>("start", () => buffer.Slice(-1, 0));
            Assert.Throws<ArgumentOutOfRangeException>("start", () => buffer.Slice(-1, -1));
            Assert.Throws<ArgumentOutOfRangeException>("start", () => buffer.Slice(-1, buffer.End));
            Assert.Throws<ArgumentOutOfRangeException>("start", () => buffer.Slice(-1));
            Assert.Throws<ArgumentOutOfRangeException>("start", () => buffer.Slice(-1L, 0L));
            Assert.Throws<ArgumentOutOfRangeException>("start", () => buffer.Slice(-1L, -1L));
            Assert.Throws<ArgumentOutOfRangeException>("start", () => buffer.Slice(-1L, buffer.End));
        }

        [Fact]
        public void Empty_SlicePositiveLength()
        {
            ReadOnlySequence<byte> buffer = ReadOnlySequence<byte>.Empty;
            Assert.Throws<ArgumentOutOfRangeException>("offset", () => buffer.Slice(0, 1));
            Assert.Throws<ArgumentOutOfRangeException>("offset", () => buffer.Slice(0L, 1L));
            Assert.Throws<ArgumentOutOfRangeException>("offset", () => buffer.Slice(buffer.Start, 1));
            Assert.Throws<ArgumentOutOfRangeException>("offset", () => buffer.Slice(buffer.Start, 1L));
        }

        [Fact]
        public void Empty_SliceNegativeLength()
        {
            ReadOnlySequence<byte> buffer = ReadOnlySequence<byte>.Empty;
            Assert.Throws<ArgumentOutOfRangeException>("length", () => buffer.Slice(0, -1));
            Assert.Throws<ArgumentOutOfRangeException>("length", () => buffer.Slice(0L, -1L));
            Assert.Throws<ArgumentOutOfRangeException>("length", () => buffer.Slice(buffer.Start, -1));
            Assert.Throws<ArgumentOutOfRangeException>("length", () => buffer.Slice(buffer.Start, -1L));
        }

        [Fact]
        public void Empty_Enumerator()
        {
            ReadOnlySequence<byte> buffer = ReadOnlySequence<byte>.Empty;
            ReadOnlySequence<byte>.Enumerator enumerator = buffer.GetEnumerator();
            {
                Assert.Equal(default, enumerator.Current); 
                Assert.True(enumerator.MoveNext());
                ReadOnlyMemory<byte> memory = enumerator.Current;
                Assert.True(memory.IsEmpty);

                Assert.False(enumerator.MoveNext());
            }
            enumerator = new ReadOnlySequence<byte>.Enumerator(buffer);
            {
                Assert.Equal(default, enumerator.Current); 
                Assert.True(enumerator.MoveNext());
                ReadOnlyMemory<byte> memory = enumerator.Current;
                Assert.True(memory.IsEmpty);

                Assert.False(enumerator.MoveNext());
            }
        }

        [Fact]
        public void Empty_GetPosition()
        {
            ReadOnlySequence<byte> buffer = ReadOnlySequence<byte>.Empty;
            Assert.Equal(buffer.Start, buffer.GetPosition(0));
            Assert.Equal(buffer.Start, buffer.GetPosition(0, buffer.Start));
            Assert.Equal(buffer.Start, buffer.GetPosition(0, buffer.End));
            //Assert.Equal(buffer.End, buffer.GetPosition(0));
            //Assert.Equal(buffer.End, buffer.GetPosition(0, buffer.Start));
            //Assert.Equal(buffer.End, buffer.GetPosition(0, buffer.End));
        }

        [Fact]
        public void Empty_GetPositionPositive()
        {
            ReadOnlySequence<byte> buffer = ReadOnlySequence<byte>.Empty;
            Assert.Throws<ArgumentOutOfRangeException>("offset", () => buffer.GetPosition(1));
            Assert.Throws<ArgumentOutOfRangeException>("offset", () => buffer.GetPosition(1, buffer.Start));
            Assert.Throws<ArgumentOutOfRangeException>("offset", () => buffer.GetPosition(1, buffer.End));
        }

        [Fact]
        public void Empty_GetPositionNegative()
        {
            ReadOnlySequence<byte> buffer = ReadOnlySequence<byte>.Empty;
            Assert.Throws<ArgumentOutOfRangeException>("offset", () => buffer.GetPosition(-1));
            Assert.Throws<ArgumentOutOfRangeException>("offset", () => buffer.GetPosition(-1, buffer.Start));
            Assert.Throws<ArgumentOutOfRangeException>("offset", () => buffer.GetPosition(-1, buffer.End));
        }

        [Fact]
        public void Empty_TryGet()
        {
            ReadOnlySequence<byte> buffer = ReadOnlySequence<byte>.Empty;
            ReadOnlyMemory<byte> memory;

            SequencePosition c1 = buffer.Start;
            Assert.True(buffer.TryGet(ref c1, out memory, false));
            Assert.Equal(buffer.Start, c1);
            Assert.True(memory.IsEmpty);

            Assert.True(buffer.TryGet(ref c1, out memory, true));
            Assert.Equal(null, c1.GetObject());
            Assert.True(memory.IsEmpty);

            Assert.False(buffer.TryGet(ref c1, out memory, false));
            Assert.Equal(null, c1.GetObject());
            Assert.True(memory.IsEmpty);
            
            Assert.False(buffer.TryGet(ref c1, out memory, true));
            Assert.Equal(null, c1.GetObject());
            Assert.True(memory.IsEmpty);

            Assert.False(buffer.TryGet(ref c1, out memory, false));
            Assert.Equal(null, c1.GetObject());
            Assert.True(memory.IsEmpty);

            Assert.False(buffer.TryGet(ref c1, out memory, true));
            Assert.Equal(null, c1.GetObject());
            Assert.True(memory.IsEmpty);

            c1 = buffer.End;
            Assert.True(buffer.TryGet(ref c1, out memory, false));
            Assert.Equal(buffer.End, c1);
            Assert.True(memory.IsEmpty);

            Assert.True(buffer.TryGet(ref c1, out memory, true));
            Assert.Equal(null, c1.GetObject());
            Assert.True(memory.IsEmpty);

            Assert.False(buffer.TryGet(ref c1, out memory, false));
            Assert.Equal(null, c1.GetObject());
            Assert.True(memory.IsEmpty);
            
            Assert.False(buffer.TryGet(ref c1, out memory, true));
            Assert.Equal(null, c1.GetObject());
            Assert.True(memory.IsEmpty);

            Assert.False(buffer.TryGet(ref c1, out memory, false));
            Assert.Equal(null, c1.GetObject());
            Assert.True(memory.IsEmpty);

            Assert.False(buffer.TryGet(ref c1, out memory, true));
            Assert.Equal(null, c1.GetObject());
            Assert.True(memory.IsEmpty);
        }
        
        #endregion

        #region Constructor 

        [Fact]
        public void Ctor_Array_Offset()
        {
            ReadOnlySequence<byte> buffer = new ReadOnlySequence<byte>(new byte[] { 1, 2, 3, 4, 5 }, 2, 3);
            Assert.Equal(buffer.ToArray(), new byte[] { 3, 4, 5 });
        }

        [Fact]
        public void Ctor_Array_NoOffset()
        {
            ReadOnlySequence<byte> buffer = new ReadOnlySequence<byte>(new byte[] { 1, 2, 3, 4, 5 });
            Assert.Equal(buffer.ToArray(), new byte[] { 1, 2, 3, 4, 5 });
        }

        [Fact]
        public void Ctor_Array_ValidatesArguments()
        {
            Assert.Throws<ArgumentOutOfRangeException>("start", () => new ReadOnlySequence<byte>(new byte[] { 1, 2, 3, 4, 5 }, 6, 0));
            Assert.Throws<ArgumentOutOfRangeException>("length", () => new ReadOnlySequence<byte>(new byte[] { 1, 2, 3, 4, 5 }, 4, 2));
            Assert.Throws<ArgumentOutOfRangeException>("start", () => new ReadOnlySequence<byte>(new byte[] { 1, 2, 3, 4, 5 }, -4, 0));
            Assert.Throws<ArgumentOutOfRangeException>("length", () => new ReadOnlySequence<byte>(new byte[] { 1, 2, 3, 4, 5 }, 4, -2));
            Assert.Throws<ArgumentNullException>("array", () => new ReadOnlySequence<byte>((byte[])null, 4, 2));
        }

        [Fact]
        public void Ctor_OwnedMemory_Offset()
        {
            CustomMemoryForTest<byte> ownedMemory = new CustomMemoryForTest<byte>(new byte[] { 1, 2, 3, 4, 5 }, 0, 5);
            ReadOnlySequence<byte> buffer = new ReadOnlySequence<byte>(ownedMemory, 2, 3);
            Assert.Equal(buffer.ToArray(), new byte[] { 3, 4, 5 });
        }

        [Fact]
        public void Ctor_OwnedMemory_NoOffset()
        {
            CustomMemoryForTest<byte> ownedMemory = new CustomMemoryForTest<byte>(new byte[] { 1, 2, 3, 4, 5 }, 0, 5);
            ReadOnlySequence<byte> buffer = new ReadOnlySequence<byte>(ownedMemory);
            Assert.Equal(buffer.ToArray(), new byte[] { 1, 2, 3, 4, 5 });
        }

        [Fact]
        public void Ctor_OwnedMemory_ValidatesArguments()
        {
            CustomMemoryForTest<byte> ownedMemory = new CustomMemoryForTest<byte>(new byte[] { 1, 2, 3, 4, 5 }, 0, 5);
            Assert.Throws<ArgumentOutOfRangeException>("start", () => new ReadOnlySequence<byte>(ownedMemory, 6, 0));
            Assert.Throws<ArgumentOutOfRangeException>("length", () => new ReadOnlySequence<byte>(ownedMemory, 4, 2));
            Assert.Throws<ArgumentOutOfRangeException>("start", () => new ReadOnlySequence<byte>(ownedMemory, -4, 0));
            Assert.Throws<ArgumentOutOfRangeException>("length", () => new ReadOnlySequence<byte>(ownedMemory, 4, -2));
            Assert.Throws<ArgumentNullException>("ownedMemory", () => new ReadOnlySequence<byte>((CustomMemoryForTest<byte>)null, 4, 2));
        }

        [Fact]
        public void Ctor_Memory()
        {
            ReadOnlyMemory<byte> memory = new ReadOnlyMemory<byte>(new byte[] { 1, 2, 3, 4, 5 });
            ReadOnlySequence<byte> buffer = new ReadOnlySequence<byte>(memory.Slice(2, 3));
            Assert.Equal(new byte[] { 3, 4, 5 }, buffer.ToArray());
        }

        [Theory]
        [InlineData(0)]
        [InlineData(long.MaxValue - 5)]
        [InlineData(-5)]
        public void Ctor_ReadOnlySequenceSegment_ValidatesArguments(long startIndex)
        {
            BufferSegment<byte> segment = new BufferSegment<byte>(new byte[] { 1, 2, 3, 4, 5 }, startIndex);
            Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => new ReadOnlySequence<byte>(segment, 6, segment, 0));
            Assert.Throws<ArgumentOutOfRangeException>("endIndex", () => new ReadOnlySequence<byte>(segment, 0, segment, 6));
            Assert.Throws<ArgumentOutOfRangeException>("endIndex", () => new ReadOnlySequence<byte>(segment, 3, segment, 0));

            Assert.Throws<ArgumentOutOfRangeException>("startIndex", () => new ReadOnlySequence<byte>(segment, -5, segment, 0));
            Assert.Throws<ArgumentOutOfRangeException>("endIndex", () => new ReadOnlySequence<byte>(segment, 0, segment, -5));

            Assert.Throws<ArgumentNullException>("startSegment", () => new ReadOnlySequence<byte>(null, 5, segment, 0));
            Assert.Throws<ArgumentNullException>("endSegment", () => new ReadOnlySequence<byte>(segment, 5, null, 0));

            BufferSegment<byte> segment2 = segment.Append(new byte[] {1, 2, 3, 4, 5});
            Assert.Throws<ArgumentOutOfRangeException>("length", () => new ReadOnlySequence<byte>(segment2, 0, segment, 0));
            Assert.Throws<ArgumentOutOfRangeException>("length", () => new ReadOnlySequence<byte>(segment2, 0, segment, 5));
            Assert.Throws<ArgumentOutOfRangeException>("length", () => new ReadOnlySequence<byte>(segment2, 5, segment, 0));
            Assert.Throws<ArgumentOutOfRangeException>("length", () => new ReadOnlySequence<byte>(segment2, 5, segment, 5));
        }

        #endregion

        [Theory]
        [InlineData(0)]
        [InlineData(long.MaxValue - 4096)]
        [InlineData(-4096)]
        public void HelloWorldAcrossTwoBlocks(long startIndex)
        {
            //     block 1       ->    block2
            // [padding..hello]  ->  [  world   ]
            const int blockSize = 4096;

            byte[] bytes = Encoding.ASCII.GetBytes("Hello World");
            byte[] firstBytes = Enumerable.Repeat((byte)'a', blockSize - 5).Concat(bytes.Take(5)).ToArray();
            byte[] secondBytes = bytes.Skip(5).Concat(Enumerable.Repeat((byte)'a', blockSize - (bytes.Length - 5))).ToArray();

            BufferSegment<byte> firstSegement = new BufferSegment<byte>(firstBytes, startIndex);
            BufferSegment<byte> secondSegement = firstSegement.Append(secondBytes);

            ReadOnlySequence<byte> buffer = new ReadOnlySequence<byte>(firstSegement, 0, secondSegement, bytes.Length - 5);
            Assert.False(buffer.IsSingleSegment);
            ReadOnlySequence<byte> helloBuffer = buffer.Slice(blockSize - 5);
            Assert.False(helloBuffer.IsSingleSegment);
            List<ReadOnlyMemory<byte>> memory = new List<ReadOnlyMemory<byte>>();
            foreach (ReadOnlyMemory<byte> m in helloBuffer)
            {
                memory.Add(m);
            }

            List<ReadOnlyMemory<byte>> spans = memory;

            Assert.Equal(2, memory.Count);
            byte[] helloBytes = new byte[spans[0].Length];
            spans[0].Span.CopyTo(helloBytes);
            byte[] worldBytes = new byte[spans[1].Length];
            spans[1].Span.CopyTo(worldBytes);
            Assert.Equal("Hello", Encoding.ASCII.GetString(helloBytes));
            Assert.Equal(" World", Encoding.ASCII.GetString(worldBytes));
        }
    }
}
