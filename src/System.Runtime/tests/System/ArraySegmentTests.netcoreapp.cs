// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace System.Tests
{
    public abstract partial class ArraySegment_Tests<T>
    {
        [Fact]
        public void CopyTo_Default_ThrowsInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => default(ArraySegment<T>).CopyTo(new T[0]));
            Assert.Throws<InvalidOperationException>(() => default(ArraySegment<T>).CopyTo(new T[0], 0));
            Assert.Throws<InvalidOperationException>(() => ((ICollection<T>)default(ArraySegment<T>)).CopyTo(new T[0], 0));
            Assert.Throws<InvalidOperationException>(() => default(ArraySegment<T>).CopyTo(new ArraySegment<T>(new T[0])));
        }

        [Fact]
        public void Empty()
        {
            var empty = ArraySegment<T>.Empty;

            Assert.NotEqual(default(ArraySegment<T>), empty);
            // Check that two Empty invocations return equal ArraySegments.
            Assert.Equal(empty, ArraySegment<T>.Empty);
            // Check that two Empty invocations return ArraySegments with a cached empty array.
            // An empty array is necessary to ensure that someone doesn't use the indexer to store data in the array Empty refers to.
            Assert.Same(empty.Array, ArraySegment<T>.Empty.Array);
            Assert.Equal(0, empty.Array.Length);
            Assert.Equal(0, empty.Offset);
            Assert.Equal(0, empty.Count);
        }

        [Fact]
        public void GetEnumerator_TypeProperties()
        {
            var arraySegment = new ArraySegment<T>(new T[1], 0, 1);
            var ienumerableoft = (IEnumerable<T>)arraySegment;
            var ienumerable = (IEnumerable)arraySegment;

            var enumerator = arraySegment.GetEnumerator();
            Assert.IsType<ArraySegment<T>.Enumerator>(enumerator);
            Assert.IsAssignableFrom<IEnumerator<T>>(enumerator);
            Assert.IsAssignableFrom<IEnumerator>(enumerator);

            var ienumeratoroft = ienumerableoft.GetEnumerator();
            var ienumerator = ienumerable.GetEnumerator();

            Assert.Equal(enumerator.GetType(), ienumeratoroft.GetType());
            Assert.Equal(ienumeratoroft.GetType(), ienumerator.GetType());
        }

        [Fact]
        public void GetEnumerator_Default_ThrowsInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => default(ArraySegment<T>).GetEnumerator());
        }

        [Fact]
        public void Slice_Default_ThrowsInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => default(ArraySegment<T>).Slice(0));
            Assert.Throws<InvalidOperationException>(() => default(ArraySegment<T>).Slice(0, 0));
        }

        [Fact]
        public void ToArray_Default_ThrowsInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => default(ArraySegment<T>).ToArray());
        }
    }

    public static partial class ArraySegment_Tests
    {
        [Theory]
        [MemberData(nameof(ArraySegment_TestData))]
        public static void CopyTo(int[] array, int index, int count)
        {
            const int CopyLining = 5;
            const int DestinationSegmentLining = 3;

            var arraySegment = new ArraySegment<int>(array, index, count);
            var destinationModel = new int[count + 2 * CopyLining];

            // CopyTo(T[])
            CopyAndInvoke(destinationModel, destination =>
            {
                arraySegment.CopyTo(destination);
                
                Assert.Equal(Enumerable.Repeat(default(int), 2 * CopyLining), destination.Skip(count));

                Assert.Equal(arraySegment, destination.Take(count));
            });

            // CopyTo(T[], int)
            CopyAndInvoke(destinationModel, destination =>
            {
                arraySegment.CopyTo(destination, CopyLining);
                
                Assert.Equal(Enumerable.Repeat(default(int), CopyLining), destination.Take(CopyLining));
                Assert.Equal(Enumerable.Repeat(default(int), CopyLining), destination.Skip(CopyLining + count));

                Assert.Equal(arraySegment, destination.Skip(CopyLining).Take(count));
            });

            // ICollection<T>.CopyTo(T[], int)
            CopyAndInvoke(destinationModel, destination =>
            {
                ((ICollection<int>)arraySegment).CopyTo(destination, CopyLining);
                
                Assert.Equal(Enumerable.Repeat(default(int), CopyLining), destination.Take(CopyLining));
                Assert.Equal(Enumerable.Repeat(default(int), CopyLining), destination.Skip(CopyLining + count));

                Assert.Equal(arraySegment, destination.Skip(CopyLining).Take(count));
            });

            // CopyTo(ArraySegment<T>)
            CopyAndInvoke(destinationModel, destination =>
            {
                // We want to make sure this overload is handling edge cases correctly, like ArraySegments that
                // do not begin at the array's start, do not end at the array's end, or have a bigger count than
                // the source ArraySegment. Construct an ArraySegment that will test all of these conditions.
                int destinationIndex = DestinationSegmentLining;
                int destinationCount = destination.Length - 2 * DestinationSegmentLining;
                var destinationSegment = new ArraySegment<int>(destination, destinationIndex, destinationCount);

                arraySegment.CopyTo(destinationSegment);

                Assert.Equal(Enumerable.Repeat(default(int), destinationIndex), destination.Take(destinationIndex));
                int remainder = destination.Length - destinationIndex - count;
                Assert.Equal(Enumerable.Repeat(default(int), remainder), destination.Skip(destinationIndex + count));

                Assert.Equal(arraySegment, destination.Skip(destinationIndex).Take(count));
            });
        }

        private static void CopyAndInvoke<T>(T[] array, Action<T[]> action) => action(array.ToArray());

        [Theory]
        [MemberData(nameof(ArraySegment_TestData))]
        public static void CopyTo_Invalid(int[] array, int index, int count)
        {
            // ArraySegment.CopyTo calls Array.Copy internally, so the exception parameter names come from there.
            var arraySegment = new ArraySegment<int>(array, index, count);

            // Destination is null
            Assert.Throws<ArgumentNullException>("destinationArray", () => arraySegment.CopyTo(null));
            Assert.Throws<ArgumentNullException>("destinationArray", () => arraySegment.CopyTo(null, 0));

            // Destination index not within range
            Assert.Throws<ArgumentOutOfRangeException>("destinationIndex", () => arraySegment.CopyTo(new int[0], -1));
            Assert.Throws<ArgumentOutOfRangeException>("destinationIndex", () => arraySegment.CopyTo(new int[0], 1));

            if (arraySegment.Any())
            {
                // Destination not large enough
                Assert.Throws<ArgumentOutOfRangeException>("destinationArray", () => arraySegment.CopyTo(new int[count - 1]));
                Assert.Throws<ArgumentOutOfRangeException>("destinationArray", () => arraySegment.CopyTo(new int[count - 1], 0));
                Assert.Throws<ArgumentOutOfRangeException>("destinationArray", () => arraySegment.CopyTo(new ArraySegment<int>(new int[count - 1])));
            }
        }

        [Theory]
        [MemberData(nameof(ArraySegment_TestData))]
        public static void GetEnumerator(int[] array, int index, int count)
        {
            var arraySegment = new ArraySegment<int>(array, index, count);
            var enumerator = arraySegment.GetEnumerator();

            var actual = new List<int>();

            while (enumerator.MoveNext())
            {
                actual.Add(enumerator.Current);
            }

            // After MoveNext returns false once, it should return false the second time.
            Assert.False(enumerator.MoveNext());

            IEnumerable<int> expected = array.Skip(index).Take(count);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(ArraySegment_TestData))]
        public static void GetEnumerator_Dispose(int[] array, int index, int count)
        {
            var arraySegment = new ArraySegment<int>(array, index, count);
            var enumerator = arraySegment.GetEnumerator();

            bool expected = arraySegment.Count > 0;
            
            // Dispose shouldn't do anything. Call it twice and then assert like nothing happened.
            enumerator.Dispose();
            enumerator.Dispose();

            Assert.Equal(expected, enumerator.MoveNext());
            if (expected)
            {
                Assert.Equal(array[index], enumerator.Current);
            }
        }

        [Theory]
        [MemberData(nameof(ArraySegment_TestData))]
        public static void GetEnumerator_Reset(int[] array, int index, int count)
        {
            var arraySegment = new ArraySegment<int>(array, index, count);
            var enumerator = (IEnumerator<int>)arraySegment.GetEnumerator();

            int[] expected = array.Skip(index).Take(count).ToArray();
            
            // Reset at a variety of different positions to ensure the implementation
            // isn't something like `position -= CONSTANT`.
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    if (enumerator.MoveNext())
                    {
                        Assert.Equal(expected[j], enumerator.Current);
                    }
                }

                enumerator.Reset();
            }
        }

        [Theory]
        [MemberData(nameof(ArraySegment_TestData))]
        public static void GetEnumerator_Invalid(int[] array, int index, int count)
        {
            var arraySegment = new ArraySegment<int>(array, index, count);
            var enumerator = arraySegment.GetEnumerator();

            // Before beginning
            Assert.Throws<InvalidOperationException>(() => enumerator.Current);
            Assert.Throws<InvalidOperationException>(() => ((IEnumerator<int>)enumerator).Current);
            Assert.Throws<InvalidOperationException>(() => ((IEnumerator)enumerator).Current);

            while (enumerator.MoveNext()) ;
            
            // After end
            Assert.Throws<InvalidOperationException>(() => enumerator.Current);
            Assert.Throws<InvalidOperationException>(() => ((IEnumerator<int>)enumerator).Current);
            Assert.Throws<InvalidOperationException>(() => ((IEnumerator)enumerator).Current);
        }

        [Theory]
        [MemberData(nameof(ArraySegment_TestData))]
        public static void GetSetItem_InRange(int[] array, int index, int count)
        {
            var arraySegment = new ArraySegment<int>(array, index, count);
            int[] expected = array.Skip(index).Take(count).ToArray();

            for (int i = 0; i < count; i++)
            {
                Assert.Equal(expected[i], arraySegment[i]);
                Assert.Equal(expected[i], ((IList<int>)arraySegment)[i]);
                Assert.Equal(expected[i], ((IReadOnlyList<int>)arraySegment)[i]);
            }

            var r = new Random(0);

            for (int i = 0; i < count; i++)
            {
                int next = r.Next(int.MinValue, int.MaxValue);

                // When we modify the underlying array, the indexer should return the updated values.
                array[arraySegment.Offset + i] ^= next;
                Assert.Equal(expected[i] ^ next, arraySegment[i]);

                // When the indexer's set method is called, the underlying array should be modified.
                arraySegment[i] ^= next;
                Assert.Equal(expected[i], array[arraySegment.Offset + i]);
            }
        }

        [Theory]
        [MemberData(nameof(ArraySegment_TestData))]
        public static void GetSetItem_NotInRange(int[] array, int index, int count)
        {
            var arraySegment = new ArraySegment<int>(array, index, count);
            TestGetSetItem_NotInRange(arraySegment, start: -arraySegment.Offset, end: 0); // Check values before start
            TestGetSetItem_NotInRange(arraySegment, start: arraySegment.Count, end: arraySegment.Offset + array.Length); // Check values after end
        }
        
        private static void TestGetSetItem_NotInRange(ArraySegment<int> arraySegment, int start, int end)
        {
            int[] array = arraySegment.Array;
            var r = new Random(1);

            for (int i = start; i < end; i++)
            {
                Assert.Equal(array[arraySegment.Offset + i], arraySegment[i]);

                int next = r.Next(int.MinValue, int.MaxValue);
                int oldValue = arraySegment[i];

                array[arraySegment.Offset + i] ^= next;
                Assert.Equal(oldValue ^ next, arraySegment[i]);

                arraySegment[i] ^= next;
                Assert.Equal(oldValue, array[arraySegment.Offset + i]);
            }
        }

        [Theory]
        [MemberData(nameof(ArraySegment_TestData))]
        public static void GetSetItem_Invalid(int[] array, int index, int count)
        {
            var arraySegment = new ArraySegment<int>(array, index, count);

            // Before start of array
            Assert.Throws<IndexOutOfRangeException>(() => arraySegment[-arraySegment.Offset - 1]);
            Assert.Throws<IndexOutOfRangeException>(() => arraySegment[-arraySegment.Offset - 1] = default(int));

            // After end of array
            Assert.Throws<IndexOutOfRangeException>(() => arraySegment[arraySegment.Offset + array.Length]);
            Assert.Throws<IndexOutOfRangeException>(() => arraySegment[arraySegment.Offset + array.Length] = default(int));
        }

        public static IEnumerable<object[]> ArraySegment_TestData() =>
            new[]
            {
                new object[] { new int[0], 0, 0 }, // Empty array
                new object[] { new[] { 3, 4, 5, 6 }, 0, 4 }, // Full span of non-empty array
                new object[] { new[] { 3, 4, 5, 6 }, 0, 3 }, // Starts at beginning, ends in middle
                new object[] { new[] { 3, 4, 5, 6 }, 1, 3 }, // Starts in middle, ends at end
                new object[] { new[] { 3, 4, 5, 6 }, 1, 2 }, // Starts in middle, ends in middle
                new object[] { new[] { 3, 4, 5, 6 }, 1, 0 } // Non-empty array, count == 0
            };
    }
}
