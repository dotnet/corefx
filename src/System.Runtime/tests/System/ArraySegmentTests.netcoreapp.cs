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
    }

    public static partial class ArraySegment_Tests
    {
        [Theory]
        [MemberData(nameof(GetEnumerator_TestData))]
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

            var expected = array.Skip(index).Take(count);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetEnumerator_TestData))]
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
        [MemberData(nameof(GetEnumerator_TestData))]
        public static void GetEnumerator_Reset(int[] array, int index, int count)
        {
            var arraySegment = new ArraySegment<int>(array, index, count);
            var enumerator = (IEnumerator<int>)arraySegment.GetEnumerator();

            var expected = array.Skip(index).Take(count).ToArray();
            
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
        [MemberData(nameof(GetEnumerator_TestData))]
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

        public static IEnumerable<object[]> GetEnumerator_TestData() =>
            new[]
            {
                new object[] { new int[0], 0, 0 }, // Empty array
                new object[] { new[] { 3, 4, 5 }, 0, 3 }, // Full span of non-empty array
                new object[] { new[] { 3, 4, 5 }, 0, 2 }, // Starts at beginning, ends in middle
                new object[] { new[] { 3, 4, 5 }, 1, 2 }, // Starts in middle, ends at end
                new object[] { new[] { 3, 4, 5 }, 1, 1 }, // Starts in middle, ends in middle
                new object[] { new[] { 3, 4, 5 }, 1, 0 } // Non-empty array, count == 0
            };
    }
}
