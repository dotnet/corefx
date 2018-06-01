// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Linq;
using System.Text;
using Xunit;

namespace System.Memory.Tests
{
    public abstract class ReadOnlySequenceTests<T>
    {
        internal ReadOnlySequenceFactory<T> Factory { get; }

        internal ReadOnlySequenceTests(ReadOnlySequenceFactory<T> factory)
        {
            Factory = factory;
        }

        [Fact]
        public void EmptyIsCorrect()
        {
            ReadOnlySequence<T> buffer = Factory.CreateOfSize(0);
            Assert.Equal(0, buffer.Length);
            Assert.True(buffer.IsEmpty);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(8)]
        public void LengthIsCorrect(int length)
        {
            ReadOnlySequence<T> buffer = Factory.CreateOfSize(length);
            Assert.Equal(length, buffer.Length);
        }

        [Theory]
        [MemberData(nameof(OutOfRangeSliceCases))]
        public void ReadOnlyBufferDoesNotAllowSlicingOutOfRange(Action<ReadOnlySequence<T>> fail)
        {
            ReadOnlySequence<T> buffer = Factory.CreateOfSize(100);
            Assert.Throws<ArgumentOutOfRangeException>(() => fail(buffer));
        }

        [Fact]
        public void ReadOnlyBufferGetPosition_MovesPosition()
        {
            ReadOnlySequence<T> buffer = Factory.CreateOfSize(100);

            SequencePosition position = buffer.GetPosition(65);
            Assert.Equal(buffer.Slice(position).Length, 35);

            position = buffer.GetPosition(65, buffer.Start);
            Assert.Equal(buffer.Slice(position).Length, 35);
        }

        [Fact]
        public void ReadOnlyBufferGetPosition_ChecksBounds()
        {
            ReadOnlySequence<T> buffer = Factory.CreateOfSize(100);
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.GetPosition(101));
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.GetPosition(101, buffer.Start));
        }

        [Fact]
        public void ReadOnlyBufferGetPosition_DoesNotAlowNegative()
        {
            ReadOnlySequence<T> buffer = Factory.CreateOfSize(20);
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.GetPosition(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.GetPosition(-1, buffer.Start));
        }

        [Fact]
        public void ReadOnlyBufferSlice_ChecksEnd()
        {
            ReadOnlySequence<T> buffer = Factory.CreateOfSize(100);
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.Slice(70, buffer.Start));
        }

        [Fact]
        public void SliceToTheEndWorks()
        {
            ReadOnlySequence<T> buffer = Factory.CreateOfSize(10);
            Assert.True(buffer.Slice(buffer.End).IsEmpty);
        }

        [Fact]
        public void CopyTo_ThrowsWhenSourceLargerThenDestination()
        {
            ReadOnlySequence<T> buffer = Factory.CreateOfSize(10);

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                Span<T> span = new T[5];
                buffer.CopyTo(span);
            });
        }

        [Theory]
        [InlineData(1)]
        [InlineData(8)]
        public void GetPositionAndSliceAreEqual(int length)
        {
            ReadOnlySequence<T> buffer = Factory.CreateOfSize(length);

            ReadOnlySequence<T> sliceOffsetToStrart = buffer.Slice(0, buffer.Start);
            TestSlice(buffer.Start, buffer.Start, sliceOffsetToStrart);

            ReadOnlySequence<T> sliceOffsetToStrartÄ = buffer.Slice(0L, buffer.Start);
            TestSlice(buffer.Start, buffer.Start, sliceOffsetToStrart);

            ReadOnlySequence<T> sliceStartToStart = buffer.Slice(buffer.Start, buffer.Start);
            TestSlice(buffer.Start, buffer.Start, sliceStartToStart);

            for (int i = 0; i <= length; i++)
            {
                SequencePosition start = buffer.GetPosition(i);
                if (i == length)
                    TestPosition(buffer.End, start);
                TestPosition(start, buffer.GetPosition(i, buffer.Start));

                ReadOnlySequence<T> sliceOffset = buffer.Slice(i);
                TestSlice(start, buffer.End, sliceOffset);

                ReadOnlySequence<T> sliceStart = buffer.Slice(start);
                TestSlice(start, buffer.End, sliceStart);

                ReadOnlySequence<T> sliceBufStartLen = buffer.Slice(buffer.Start, i);
                TestSlice(buffer.Start, start, sliceBufStartLen);

                ReadOnlySequence<T> sliceBufStartLenL = buffer.Slice(buffer.Start, (long)i);
                TestSlice(buffer.Start, start, sliceBufStartLenL);

                ReadOnlySequence<T> sliceBufStartEnd = buffer.Slice(buffer.Start, start);
                TestSlice(buffer.Start, start, sliceBufStartEnd);

                for (int j = i; j <= length; j++)
                {
                    int len = j - i;
                    SequencePosition end = buffer.GetPosition(j);
                    TestPosition(end, buffer.GetPosition(len, start));

                    ReadOnlySequence<T> sliceOffsetLen = buffer.Slice(i, len);
                    TestSlice(start, end, sliceOffsetLen);

                    ReadOnlySequence<T> sliceOffsetLenL = buffer.Slice((long)i, len);
                    TestSlice(start, end, sliceOffsetLenL);

                    ReadOnlySequence<T> sliceOffsetEnd = buffer.Slice(i, end);
                    TestSlice(start, end, sliceOffsetEnd);

                    ReadOnlySequence<T> sliceOffsetEndL = buffer.Slice((long)i, end);
                    TestSlice(start, end, sliceOffsetEndL);

                    ReadOnlySequence<T> sliceStartLen = buffer.Slice(start, len);
                    TestSlice(start, end, sliceStartLen);

                    ReadOnlySequence<T> sliceStartLenL = buffer.Slice(start, (long)len);
                    TestSlice(start, end, sliceStartLenL);

                    ReadOnlySequence<T> sliceStartEnd = buffer.Slice(start, end);
                    TestSlice(start, end, sliceStartEnd);
                }

            }
        }

        public static void TestSlice(SequencePosition start, SequencePosition end, ReadOnlySequence<T> slice)
        {
            TestPosition(start, slice.Start);
            TestPosition(end, slice.End);
        }

        public static void TestPosition(SequencePosition expected, SequencePosition actual)
        {
            int GetIndex(SequencePosition pos) => pos.GetInteger() & int.MaxValue;

            Assert.Equal(expected.GetObject(), actual.GetObject());
            Assert.Equal(GetIndex(expected), GetIndex(actual));
        }

        public static TheoryData<Func<ReadOnlySequence<T>, ReadOnlySequence<T>>> ValidSliceCases => new TheoryData<Func<ReadOnlySequence<T>, ReadOnlySequence<T>>>
        {
            b => b.Slice(5),
            b => b.Slice(0).Slice(5),
            b => b.Slice(5, 5),
            b => b.Slice(b.GetPosition(5), 5),
            b => b.Slice(5, b.GetPosition(10)),
            b => b.Slice(b.GetPosition(5), b.GetPosition(10)),
            b => b.Slice(b.GetPosition(5, b.Start), 5),
            b => b.Slice(5, b.GetPosition(10, b.Start)),
            b => b.Slice(b.GetPosition(5, b.Start), b.GetPosition(10, b.Start)),

            b => b.Slice((long)5),
            b => b.Slice((long)5, 5),
            b => b.Slice(b.GetPosition(5), (long)5),
            b => b.Slice((long)5, b.GetPosition(10)),
            b => b.Slice(b.GetPosition(5, b.Start), (long)5),
            b => b.Slice((long)5, b.GetPosition(10, b.Start)),
        };

        public static TheoryData<Action<ReadOnlySequence<T>>> OutOfRangeSliceCases => new TheoryData<Action<ReadOnlySequence<T>>>
        {
            // negative start	
            b => b.Slice(-1), // no length
            b => b.Slice(-1, -1), // negative length
            b => b.Slice(-1, 0), // zero length
            b => b.Slice(-1, 1), // positive length
            b => b.Slice(-1, 101), // after end length
            b => b.Slice(-1, b.Start), // to start
            b => b.Slice(-1, b.End), // to end

            // zero start
            b => b.Slice(0, -1), // negative length
            b => b.Slice(0, 101), // after end length

            // end start
            b => b.Slice(100, -1), // negative length
            b => b.Slice(100, 1), // after end length
            b => b.Slice(100, b.Start), // to start

            // After end start
            b => b.Slice(101), // no length
            b => b.Slice(101, -1), // negative length
            b => b.Slice(101, 0), // zero length
            b => b.Slice(101, 1), // after end length
            b => b.Slice(101, b.Start), // to start
            b => b.Slice(101, b.End), // to end

            // At Start start
            b => b.Slice(b.Start, -1), // negative length
            b => b.Slice(b.Start, 101), // after end length

            // At End start
            b => b.Slice(b.End, -1), // negative length
            b => b.Slice(b.End, 1), // after end length
            b => b.Slice(b.End, b.Start), // to start

            // Slice at begin
            b => b.Slice(0, 70).Slice(0, b.End), // to after end
            b => b.Slice(0, 70).Slice(b.Start, b.End), // to after end
            // from after end
            b => b.Slice(0, 70).Slice(b.End),
            b => b.Slice(0, 70).Slice(b.End, -1), // negative length
            b => b.Slice(0, 70).Slice(b.End, 0), // zero length
            b => b.Slice(0, 70).Slice(b.End, 1), // after end length
            b => b.Slice(0, 70).Slice(b.End, b.Start), // to start
            b => b.Slice(0, 70).Slice(b.End, b.End), // to after end

            // Slice at begin
            b => b.Slice(b.Start, 70).Slice(0, b.End), // to after end
            b => b.Slice(b.Start, 70).Slice(b.Start, b.End), // to after end
            // from after end
            b => b.Slice(b.Start, 70).Slice(b.End),
            b => b.Slice(b.Start, 70).Slice(b.End, -1), // negative length
            b => b.Slice(b.Start, 70).Slice(b.End, 0), // zero length
            b => b.Slice(b.Start, 70).Slice(b.End, 1), // after end length
            b => b.Slice(b.Start, 70).Slice(b.End, b.Start), // to start
            b => b.Slice(b.Start, 70).Slice(b.End, b.End), // to after end

            // Slice at middle
            b => b.Slice(30, 40).Slice(0, b.Start), // to before start
            b => b.Slice(30, 40).Slice(0, b.End), // to after end
            // from before start
            b => b.Slice(30, 40).Slice(b.Start),
            b => b.Slice(30, 40).Slice(b.Start, -1), // negative length
            b => b.Slice(30, 40).Slice(b.Start, 0), // zero length
            b => b.Slice(30, 40).Slice(b.Start, 1), // positive length
            b => b.Slice(30, 40).Slice(b.Start, 41), // after end length
            b => b.Slice(30, 40).Slice(b.Start, b.Start), // to before start
            b => b.Slice(30, 40).Slice(b.Start, b.End), // to after end
            // from after end
            b => b.Slice(30, 40).Slice(b.End),
            b => b.Slice(b.Start, 70).Slice(b.End, -1), // negative length
            b => b.Slice(b.Start, 70).Slice(b.End, 0), // zero length
            b => b.Slice(b.Start, 70).Slice(b.End, 1), // after end length
            b => b.Slice(30, 40).Slice(b.End, b.Start), // to before start
            b => b.Slice(30, 40).Slice(b.End, b.End), // to after end

            // Slice at end
            b => b.Slice(70, 30).Slice(0, b.Start), // to before start
            // from before start
            b => b.Slice(30, 40).Slice(b.Start),
            b => b.Slice(30, 40).Slice(b.Start, -1), // negative length
            b => b.Slice(30, 40).Slice(b.Start, 0), // zero length
            b => b.Slice(30, 40).Slice(b.Start, 1), // positive length
            b => b.Slice(30, 40).Slice(b.Start, 31), // after end length
            b => b.Slice(30, 40).Slice(b.Start, b.Start), // to before start
            b => b.Slice(30, 40).Slice(b.Start, b.End), // to end
            // from end
            b => b.Slice(70, 30).Slice(b.End, b.Start), // to before start

            // Slice at end
            b => b.Slice(70, 30).Slice(0, b.Start), // to before start
            // from before start
            b => b.Slice(30, 40).Slice(b.Start),
            b => b.Slice(30, 40).Slice(b.Start, -1), // negative length
            b => b.Slice(30, 40).Slice(b.Start, 0), // zero length
            b => b.Slice(30, 40).Slice(b.Start, 1), // positive length
            b => b.Slice(30, 40).Slice(b.Start, 31), // after end length
            b => b.Slice(30, 40).Slice(b.Start, b.Start), // to before start
            b => b.Slice(30, 40).Slice(b.Start, b.End), // to end
            // from end
            b => b.Slice(70, 30).Slice(b.End, b.Start), // to before start
        };
    }
}
