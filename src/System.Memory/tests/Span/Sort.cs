// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace System.SpanTests
{
    public static partial class SpanTests
    {
        // Existing coreclr tests seem to be in here:
        // https://github.com/dotnet/coreclr/tree/master/tests/src/CoreMangLib/cti/system/array
        // E.g. arraysort1.cs etc.

        //public static readonly TheoryData<uint[]> s_sortCasesUInt =
        //new TheoryData<uint[]> {
        //    ,
        //};

        // To run just these tests append to command line:
        // -trait "MyTrait=MyTraitValue"

        // How do we create a not comparable? I.e. something Comparer<T>.Default fails on?
        //struct NotComparable { int i; string s; IntPtr p; }
        //[Fact]
        //[Trait("MyTrait", "MyTraitValue")]
        //public static void Sort_NotComparableThrows()
        //{
        //    var comparer = Comparer<NotComparable>.Default;
        //    Assert.Throws<ArgumentNullException>(() => new Span<NotComparable>(new NotComparable[16])
        //        .Sort());
        //    Assert.Throws<ArgumentNullException>(() => new Span<NotComparable>(new NotComparable[16])
        //        .Sort(comparer));
        //}

        [Fact]
        [Trait("MyTrait", "MyTraitValue")]
        public static void Sort_NullComparerDoesNotThrow()
        {
            new Span<int>(new int[] { 3 }).Sort((IComparer<int>)null);
        }

        [Fact]
        [Trait("MyTrait", "MyTraitValue")]
        public static void Sort_NullComparisonThrows()
        {
            Assert.Throws<ArgumentNullException>(() => new Span<int>(new int[] { }).Sort((Comparison<int>)null));
            Assert.Throws<ArgumentNullException>(() => new Span<string>(new string[] { }).Sort((Comparison<string>)null));
        }

        [Theory]
        [Trait("MyTrait", "MyTraitValue")]
        [InlineData(new uint[] { })]
        [InlineData(new uint[] { 1 })]
        [InlineData(new uint[] { 2, 1 })]
        [InlineData(new uint[] { 3, 1, 2 })]
        [InlineData(new uint[] { 3, 2, 1 })]
        [InlineData(new uint[] { 18, 17, 16, 15, 14, 13, 12, 11, 10, 9, 8, 1, 2, 3, 4, 7, 6, 5 })]
        public static void Sort_UInt(uint[] unsorted)
        {
            TestSortOverloads(unsorted);
        }

        // TODO: OuterLoop
        [Theory]
        [Trait("MyTrait", "MyTraitValue")]
        [InlineData(17, 1024)]
        [InlineData(42, 1024)]
        [InlineData(1873318, 1024)]
        public static void Sort_Random_Int(int seed, int maxCount)
        {
            var random = new Random(seed);
            for (int count = 0; count < maxCount; count++)
            {
                var unsorted = Enumerable.Range(0, count).Select(i => random.Next()).ToArray();
                TestSortOverloads(unsorted);
            }
        }

        [Theory]
        [Trait("MyTrait", "MyTraitValue")]
        [InlineData(17, 1024)]
        [InlineData(42, 1024)]
        [InlineData(1873318, 1024)]
        public static void Sort_Random_Float(int seed, int maxCount)
        {
            var random = new Random(seed);
            for (int count = 0; count < maxCount; count++)
            {
                var unsorted = Enumerable.Range(0, count).Select(i => (float)random.Next()).ToArray();
                TestSortOverloads(unsorted);
            }
        }

        [Theory]
        [Trait("MyTrait", "MyTraitValue")]
        [InlineData(17, 512)]
        [InlineData(42, 512)]
        [InlineData(1873318, 512)]
        public static void Sort_Random_String(int seed, int maxCount)
        {
            var random = new Random(seed);
            for (int count = 0; count < maxCount; count++)
            {
                var unsorted = Enumerable.Range(0, count).Select(i => random.Next().ToString("D9")).ToArray();
                TestSortOverloads(unsorted);
            }
        }

        [Theory]
        [Trait("MyTrait", "MyTraitValue")]
        [InlineData(1024)]
        public static void Sort_MedianOfThreeKiller_Int(int maxCount)
        {
            var filler = new MedianOfThreeKillerSpanFiller();
            for (int count = 0; count < maxCount; count++)
            {
                var unsorted = new int[count];
                filler.Fill(unsorted, count, i => i);
                TestSortOverloads(unsorted);
            }
        }

        [Theory]
        [Trait("MyTrait", "MyTraitValue")]
        [InlineData(64)]
        public static void Sort_MedianOfThreeKiller_String(int maxCount)
        {
            var filler = new MedianOfThreeKillerSpanFiller();
            for (int count = 0; count < maxCount; count++)
            {
                var unsorted = new string[count];
                filler.Fill(unsorted, count, i => i.ToString("D9"));
                TestSortOverloads(unsorted);
            }
        }

        // TODO: OuterLoop
        [Fact]
        [Trait("MyTrait", "MyTraitValue")]
        public static void Sort_Reverse_Int()
        {
            for (int count = 1; count <= 256 * 1024; count <<= 1)
            {
                var unsorted = Enumerable.Range(0, count).Reverse().ToArray();
                TestSortOverloads(unsorted);
            }
        }

        // TODO: OuterLoop
        [Fact]
        [Trait("MyTrait", "MyTraitValue")]
        public static void Sort_AlreadySorted_Int()
        {
            for (int count = 1; count <= 256 * 1024; count <<= 1)
            {
                var unsorted = Enumerable.Range(0, count).ToArray();
                TestSortOverloads(unsorted);
            }
        }

        // TODO: OuterLoop
        [Fact]
        [Trait("MyTrait", "MyTraitValue")]
        public static void SortWithItems_Reverse_Int()
        {
            for (int count = 1; count <= 256 * 1024; count <<= 1)
            {
                var unsorted = Enumerable.Range(0, count).Reverse().ToArray();
                var unsortedItems = Enumerable.Range(0, count).ToArray();
                TestSortOverloads(unsorted, unsortedItems);
            }
        }

        [Theory]
        [Trait("MyTrait", "MyTraitValue")]
        [InlineData(17, 1024)]
        [InlineData(42, 1024)]
        [InlineData(1873318, 1024)]
        public static void SortWithItems_Random_Int(int seed, int maxCount)
        {
            var random = new Random(seed);
            for (int count = 0; count < maxCount; count++)
            {
                var unsorted = Enumerable.Range(0, count).Select(i => random.Next()).ToArray();
                var unsortedItems = Enumerable.Range(0, count).ToArray();
                TestSortOverloads(unsorted, unsortedItems);
            }
        }

        [Theory]
        [Trait("MyTrait", "MyTraitValue")]
        [InlineData(1024)]
        public static void SortWithItems_MedianOfThreeKiller_Int(int maxCount)
        {
            var filler = new MedianOfThreeKillerSpanFiller();
            for (int count = 0; count < maxCount; count++)
            {
                var unsorted = new int[count];
                filler.Fill(unsorted, count, i => i);
                var unsortedItems = Enumerable.Range(0, count).ToArray();
                TestSortOverloads(unsorted, unsortedItems);
            }
        }

        [Theory]
        [Trait("MyTrait", "MyTraitValue")]
        [InlineData(new uint[] { })]
        [InlineData(new uint[] { 1 })]
        [InlineData(new uint[] { 2, 1 })]
        [InlineData(new uint[] { 3, 1, 2 })]
        [InlineData(new uint[] { 3, 2, 1 })]
        [InlineData(new uint[] { 3, 2, 4, 1 })]
        [InlineData(new uint[] { 18, 17, 16, 15, 14, 13, 12, 11, 10, 9, 8, 1, 2, 3, 4, 7, 6, 5 })]
        public static void SortWithItems_UInt(uint[] unsorted)
        {
            var unsortedItems = Enumerable.Range(0, unsorted.Length).ToArray();
            TestSortOverloads(unsorted, unsortedItems);
        }


        // NOTE: Sort_MaxLength_NoOverflow test is constrained to run on Windows and MacOSX because it causes
        //       problems on Linux due to the way deferred memory allocation works. On Linux, the allocation can
        //       succeed even if there is not enough memory but then the test may get killed by the OOM killer at the
        //       time the memory is accessed which triggers the full memory allocation.
        [Fact]
        [Trait("MyTrait", "MyTraitValue")]
        [OuterLoop]
        [PlatformSpecific(TestPlatforms.Windows | TestPlatforms.OSX)]
        public unsafe static void Sort_MaxLength_NoOverflow()
        {
            if (sizeof(IntPtr) == sizeof(long))
            {
                // Allocate maximum length span native memory
                var length = int.MaxValue;
                if (!AllocationHelper.TryAllocNative(new IntPtr(length), out IntPtr memory))
                {
                    Console.WriteLine($"Span.Sort test {nameof(Sort_MaxLength_NoOverflow)} skipped (could not alloc memory).");
                    return;
                }
                try
                {
                    var span = new Span<byte>(memory.ToPointer(), length);
                    var filler = new DecrementingSpanFiller();
                    const byte fill = 42;
                    span.Fill(fill);
                    span[0] = 255;
                    span[1] = 254;
                    span[span.Length - 2] = 1;
                    span[span.Length - 1] = 0;

                    span.Sort();

                    Assert.Equal(span[0], (byte)0);
                    Assert.Equal(span[1], (byte)1);
                    Assert.Equal(span[span.Length - 2], (byte)254);
                    Assert.Equal(span[span.Length - 1], (byte)255);
                    for (int i = 2; i < length - 2; i++)
                    {
                        Assert.Equal(span[i], fill);
                    }
                }
                finally
                {
                    AllocationHelper.ReleaseNative(ref memory);
                }
            }
        }


        private static void TestSortOverloads<T>(T[] array)
            where T : IComparable<T>
        {
            TestSpan(array);
            TestComparerSpan(array);
            TestComparisonSpan(array);
            TestCustomComparerSpan(array);
            TestNullComparerSpan(array);
        }

        private static void TestSpan<T>(T[] array)
            where T : IComparable<T>
        {
            var span = new Span<T>(array);
            var expected = (T[])array.Clone();
            Array.Sort(expected);

            span.Sort();

            Assert.Equal(expected, array);
        }
        private static void TestComparerSpan<T>(T[] array)
            where T : IComparable<T>
        {
            var span = new Span<T>(array);
            var expected = (T[])array.Clone();
            Array.Sort(expected);

            span.Sort(Comparer<T>.Default);

            Assert.Equal(expected, array);
        }
        private static void TestComparisonSpan<T>(T[] array)
            where T : IComparable<T>
        {
            var span = new Span<T>(array);
            var expected = (T[])array.Clone();
            Array.Sort(expected);

            span.Sort(Comparer<T>.Default.Compare);

            Assert.Equal(expected, array);
        }
        private static void TestCustomComparerSpan<T>(T[] array)
            where T : IComparable<T>
        {
            var span = new Span<T>(array);
            var expected = (T[])array.Clone();
            Array.Sort(expected);

            span.Sort(new CustomComparer<T>());

            Assert.Equal(expected, array);
        }
        private static void TestNullComparerSpan<T>(T[] array)
            where T : IComparable<T>
        {
            var span = new Span<T>(array);
            var expected = (T[])array.Clone();
            Array.Sort(expected);

            span.Sort((IComparer<T>)null);

            Assert.Equal(expected, array);
        }


        private static void TestSortOverloads<TKey, TValue>(TKey[] keys, TValue[] values)
            where TKey : IComparable<TKey>
        {
            TestSpan(keys, values);
            TestComparerSpan(keys, values);
            TestComparisonSpan(keys, values);
            TestCustomComparerSpan(keys, values);
            TestNullComparerSpan(keys, values);
        }

        private static void TestSpan<TKey, TValue>(TKey[] keys, TValue[] values)
            where TKey : IComparable<TKey>
        {
            var expectedKeys = (TKey[])keys.Clone();
            var expectedValues = (TValue[])values.Clone();
            Array.Sort(expectedKeys, expectedValues);

            var spanKeys = new Span<TKey>(keys);
            var spanValues = new Span<TValue>(values);
            spanKeys.Sort(spanValues);

            Assert.Equal(expectedKeys, keys);
            Assert.Equal(expectedValues, values);
        }
        private static void TestComparerSpan<TKey, TValue>(TKey[] keys, TValue[] values)
            where TKey : IComparable<TKey>
        {
            var expectedKeys = (TKey[])keys.Clone();
            var expectedValues = (TValue[])values.Clone();
            Array.Sort(expectedKeys, expectedValues);

            var spanKeys = new Span<TKey>(keys);
            var spanValues = new Span<TValue>(values);
            spanKeys.Sort(spanValues, Comparer<TKey>.Default);

            Assert.Equal(expectedKeys, keys);
            Assert.Equal(expectedValues, values);
        }
        private static void TestComparisonSpan<TKey, TValue>(TKey[] keys, TValue[] values)
            where TKey : IComparable<TKey>
        {
            var expectedKeys = (TKey[])keys.Clone();
            var expectedValues = (TValue[])values.Clone();
            Array.Sort(expectedKeys, expectedValues);

            var spanKeys = new Span<TKey>(keys);
            var spanValues = new Span<TValue>(values);
            spanKeys.Sort(spanValues, Comparer<TKey>.Default.Compare);

            Assert.Equal(expectedKeys, keys);
            Assert.Equal(expectedValues, values);
        }
        private static void TestCustomComparerSpan<TKey, TValue>(TKey[] keys, TValue[] values)
            where TKey : IComparable<TKey>
        {
            var expectedKeys = (TKey[])keys.Clone();
            var expectedValues = (TValue[])values.Clone();
            Array.Sort(expectedKeys, expectedValues);

            var spanKeys = new Span<TKey>(keys);
            var spanValues = new Span<TValue>(values);
            spanKeys.Sort(spanValues, new CustomComparer<TKey>());

            Assert.Equal(expectedKeys, keys);
            Assert.Equal(expectedValues, values);
        }
        private static void TestNullComparerSpan<TKey, TValue>(TKey[] keys, TValue[] values)
            where TKey : IComparable<TKey>
        {
            var expectedKeys = (TKey[])keys.Clone();
            var expectedValues = (TValue[])values.Clone();
            Array.Sort(expectedKeys, expectedValues);

            var spanKeys = new Span<TKey>(keys);
            var spanValues = new Span<TValue>(values);
            spanKeys.Sort(spanValues, (IComparer<TKey>)null);

            Assert.Equal(expectedKeys, keys);
            Assert.Equal(expectedValues, values);
        }

        internal struct CustomComparer<T> : IComparer<T>
            where T : IComparable<T>
        {
            public int Compare(T x, T y) => x.CompareTo(y);
        }

        public interface ISpanFiller
        {
            void Fill<T>(Span<T> span, int sliceLength, Func<int, T> toValue);
        }
        public class ConstantSpanFiller : ISpanFiller
        {
            readonly int _fill;

            public ConstantSpanFiller(int fill)
            {
                _fill = fill;
            }

            public void Fill<T>(Span<T> span, int sliceLength, Func<int, T> toValue)
            {
                span.Fill(toValue(_fill));
            }
        }
        public class DecrementingSpanFiller : ISpanFiller
        {
            public void Fill<T>(Span<T> span, int sliceLength, Func<int, T> toValue)
            {
                DecrementingFill(span, toValue);
            }

            public static void DecrementingFill<T>(Span<T> span, Func<int, T> toValue)
            {
                for (int i = 0; i < span.Length; i++)
                {
                    span[i] = toValue(span.Length - i - 1);
                }
            }
        }
        public class IncrementingSpanFiller : ISpanFiller
        {
            public void Fill<T>(Span<T> span, int sliceLength, Func<int, T> toValue)
            {
                IncrementingFill(span, toValue);
            }

            public static void IncrementingFill<T>(Span<T> span, Func<int, T> toValue)
            {
                for (int i = 0; i < span.Length; i++)
                {
                    span[i] = toValue(i);
                }
            }
        }
        public class MedianOfThreeKillerSpanFiller : ISpanFiller
        {
            public void Fill<T>(Span<T> span, int sliceLength, Func<int, T> toValue)
            {
                // Each slice must be median of three!
                int i = 0;
                for (; i < span.Length - sliceLength; i += sliceLength)
                {
                    InitializeMedianOfThreeKiller(span.Slice(i, sliceLength), toValue);
                }
                // Fill remainder just to be sure
                InitializeMedianOfThreeKiller(span.Slice(i, span.Length - i), toValue);
            }

            public static void InitializeMedianOfThreeKiller<T>(Span<T> span, Func<int, T> toValue)
            {
                var length = span.Length;
                // if n is odd, set the last element to n-1, and proceed
                // with n decremented by 1
                if (length % 2 != 0)
                {
                    span[length - 1] = toValue(length);
                    --length;
                }
                var m = length / 2;
                for (int i = 0; i < m; ++i)
                {
                    // first half of array (even indices)
                    if (i % 2 == 0)
                        span[i] = toValue(i + 1);
                    // first half of array (odd indices)
                    else
                        span[i] = toValue(m + i + (m % 2 != 0 ? 1 : 0));
                    // second half of array
                    span[m + i] = toValue((i + 1) * 2);
                }
            }
        }
        public class PartialRandomShuffleSpanFiller : ISpanFiller
        {
            readonly ISpanFiller _spanFiller;
            readonly double _fractionRandomShuffles;
            readonly int _seed;

            public PartialRandomShuffleSpanFiller(ISpanFiller spanFiller, double fractionRandomShuffles, int seed)
            {
                _spanFiller = spanFiller;
                _fractionRandomShuffles = fractionRandomShuffles;
                _seed = seed;
            }

            public void Fill<T>(Span<T> span, int sliceLength, Func<int, T> toValue)
            {
                _spanFiller.Fill(span, sliceLength, toValue);

                RandomShuffle(span, _fractionRandomShuffles);
            }

            private void RandomShuffle<T>(Span<T> span, double fractionRandomShuffles)
            {
                var random = new Random(_seed);
                int shuffleCount = Math.Max(0, (int)(span.Length * fractionRandomShuffles));
                for (int i = 0; i < shuffleCount; i++)
                {
                    var a = random.Next(span.Length);
                    var b = random.Next(span.Length);
                    var temp = span[a];
                    span[a] = span[b];
                    span[b] = temp;
                }
            }
        }
        public class RandomSpanFiller : ISpanFiller
        {
            readonly int _seed;

            public RandomSpanFiller(int seed)
            {
                _seed = seed;
            }

            public void Fill<T>(Span<T> span, int sliceLength, Func<int, T> toValue)
            {
                var random = new Random(_seed);
                RandomFill(random, span, toValue);
            }

            public static void RandomFill<T>(Random random, Span<T> span, Func<int, T> toValue)
            {
                for (int i = 0; i < span.Length; i++)
                {
                    span[i] = toValue(random.Next());
                }
            }
        }
    }
}
