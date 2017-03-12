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
            // Source is default
            Assert.Throws<InvalidOperationException>(() => default(ArraySegment<T>).CopyTo(new T[0]));
            Assert.Throws<InvalidOperationException>(() => default(ArraySegment<T>).CopyTo(new T[0], 0));
            Assert.Throws<InvalidOperationException>(() => ((ICollection<T>)default(ArraySegment<T>)).CopyTo(new T[0], 0));
            Assert.Throws<InvalidOperationException>(() => default(ArraySegment<T>).CopyTo(new ArraySegment<T>(new T[0])));

            // Destination is default
            Assert.Throws<InvalidOperationException>(() => new ArraySegment<T>(new T[0]).CopyTo(default(ArraySegment<T>)));
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

        [Fact]
        public void ToArray_Empty_ReturnsSameArray()
        {
            T[] cachedArray = ArraySegment<T>.Empty.ToArray();
            Assert.Same(cachedArray, ArraySegment<T>.Empty.ToArray());
            Assert.Same(cachedArray, new ArraySegment<T>(new T[0]).ToArray());
            Assert.Same(cachedArray, new ArraySegment<T>(new T[1], 0, 0).ToArray());
        }

        [Fact]
        public void ToArray_NonEmptyArray_DoesNotReturnSameArray()
        {
            // Prevent a faulty implementation like `if (Count == 0) { return Array; }`
            var emptyArraySegment = new ArraySegment<T>(new T[1], 0, 0);
            Assert.NotSame(emptyArraySegment.Array, emptyArraySegment.ToArray());
        }
    }

    public static partial class ArraySegment_Tests
    {
        [Theory]
        [MemberData(nameof(Conversion_FromArray_TestData))]
        public static void Conversion_FromArray(int[] array)
        {
            ArraySegment<int> implicitlyConverted = array;
            ArraySegment<int> explicitlyConverted = (ArraySegment<int>)array;

            var expected = new ArraySegment<int>(array);
            Assert.Equal(expected, implicitlyConverted);
            Assert.Equal(expected, explicitlyConverted);
        }

        public static IEnumerable<object[]> Conversion_FromArray_TestData()
        {
            yield return new object[] { new int[0] };
            yield return new object[] { new int[1] };
        }

        [Theory]
        [MemberData(nameof(ArraySegment_TestData))]
        public static void CopyTo(ArraySegment<int> arraySegment)
        {
            const int CopyLining = 5;
            const int DestinationSegmentLining = 3;

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
        public static void CopyTo_Invalid(ArraySegment<int> arraySegment)
        {
            // ArraySegment.CopyTo calls Array.Copy internally, so the exception parameter names come from there.

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
        public static void GetEnumerator(ArraySegment<int> arraySegment)
        {
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
        public static void GetEnumerator_Dispose(ArraySegment<int> arraySegment)
        {
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
        public static void GetEnumerator_Reset(ArraySegment<int> arraySegment)
        {
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
        public static void GetEnumerator_Invalid(ArraySegment<int> arraySegment)
        {
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
        public static void GetSetItem_InRange(ArraySegment<int> arraySegment)
        {
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
        public static void GetSetItem_NotInRange(ArraySegment<int> arraySegment)
        {
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
        public static void GetSetItem_Invalid(ArraySegment<int> arraySegment)
        {
            // Before start of array
            Assert.Throws<IndexOutOfRangeException>(() => arraySegment[-arraySegment.Offset - 1]);
            Assert.Throws<IndexOutOfRangeException>(() => arraySegment[-arraySegment.Offset - 1] = default(int));

            // After end of array
            Assert.Throws<IndexOutOfRangeException>(() => arraySegment[arraySegment.Offset + array.Length]);
            Assert.Throws<IndexOutOfRangeException>(() => arraySegment[arraySegment.Offset + array.Length] = default(int));
        }

        [Theory]
        [MemberData(nameof(Slice_TestData))]
        public static void Slice(ArraySegment<int> arraySegment, int index, int count)
        {
            var expected = new ArraySegment<int>(arraySegment.Array, arraySegment.Offset + index, count);

            if (index + count == arraySegment.Count)
            {
                Assert.Equal(expected, arraySegment.Slice(index));
            }

            Assert.Equal(expected, arraySegment.Slice(index, count));
        }

        public static IEnumerable<object[]> Slice_TestData()
        {
            var arraySegments = ArraySegment_TestData().Select(array => array.Single()).Cast<ArraySegment<int>>();

            foreach (ArraySegment<int> arraySegment in arraySegments)
            {
                yield return new object[] { arraySegment, 0, 0 }; // Preserve start, no items
                yield return new object[] { arraySegment, 0, arraySegment.Count }; // Preserve start, preserve count (noop)
                yield return new object[] { arraySegment, arraySegment.Count, 0 }; // Start at end, no items
                
                if (arraySegment.Any())
                {
                    yield return new object[] { arraySegment, 1, 0 }; // Start at middle or end, no items
                    yield return new object[] { arraySegment, 1, arraySegment.Count - 1 }; // Start at middle or end, rest of items
                    yield return new object[] { arraySegment, arraySegment.Count - 1, 1 }; // Preserve start or start at middle, one item
                }

                yield return new object[] { arraySegment, 0, arraySegment.Count / 2 }; // Preserve start, multiple items, end at middle
                yield return new object[] { arraySegment, arraySegment.Count / 2, arraySegment.Count / 2 }; // Start at middle, multiple items, end at middle (due to integer division truncation) or preserve end
                yield return new object[] { arraySegment, arraySegment.Count / 4, arraySegment.Count / 2 }; // Start at middle, multiple items, end at middle

                // ArraySegment.Slice permits negative indices. This allows the user to backtrack the start of the ArraySegment within the array.
                // It also allows users to pass in counts larger than the count of the ArraySegment, provided that it will be able to fit within
                // the ArraySegment's array.

                yield return new object[] { arraySegment, -arraySegment.Offset, arraySegment.Offset }; // Previous segment
                yield return new object[] { arraySegment, -arraySegment.Offset, arraySegment.Offset + arraySegment.Count }; // Previous + This segment
                yield return new object[] { arraySegment, -arraySegment.Offset, arraySegment.Array.Length }; // Previous + This + Next segment
                yield return new object[] { arraySegment, 0, arraySegment.Array.Length - arraySegment.Offset }; // This + Next segment
                yield return new object[] { arraySegment, arraySegment.Count, arraySegment.Array.Length - arraySegment.Offset - arraySegment.Count }; // Next segment
            }
        }

        [Theory]
        [MemberData(nameof(ArraySegment_TestData))]
        public static void ToArray(ArraySegment<int> arraySegment)
        {
            // ToList is called here so we copy the data and raise an assert if ToArray modifies the underlying array.
            var expected = arraySegment.Array.Skip(arraySegment.Offset).Take(arraySegment.Count).ToList();
            Assert.Equal(expected, arraySegment.ToArray());
        }

        public static IEnumerable<object[]> ArraySegment_TestData()
        {
            var arraySegments = new (int[] array, int index, int count)[]
            {
                (array: new int[0], index: 0, 0), // Empty array
                (array: new[] { 3, 4, 5, 6 }, index: 0, count: 4), // Full span of non-empty array
                (array: new[] { 3, 4, 5, 6 }, index: 0, count: 3), // Starts at beginning, ends in middle
                (array: new[] { 3, 4, 5, 6 }, index: 1, count: 3), // Starts in middle, ends at end
                (array: new[] { 3, 4, 5, 6 }, index: 1, count: 2), // Starts in middle, ends in middle
                (array: new[] { 3, 4, 5, 6 }, index: 1, count: 0) // Non-empty array, count == 0
            };
            
            return arraySegments.Select(as => new object[] { new ArraySegment<int>(as.array, as.index, as.count) });
        }
    }
}
