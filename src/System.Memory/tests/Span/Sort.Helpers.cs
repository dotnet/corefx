// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace System.SpanTests
{
    public static partial class SortSpanTests
    {
        internal static void Test_Keys_Int8(ISortCases sortCases) =>
            Test(sortCases, i => (sbyte)i, sbyte.MinValue);
        internal static void Test_Keys_UInt8(ISortCases sortCases) =>
            Test(sortCases, i => (byte)i, byte.MaxValue);
        internal static void Test_Keys_Int16(ISortCases sortCases) =>
            Test(sortCases, i => (short)i, short.MinValue);
        internal static void Test_Keys_UInt16(ISortCases sortCases) =>
            Test(sortCases, i => (ushort)i, ushort.MaxValue);
        internal static void Test_Keys_Int32(ISortCases sortCases) =>
            Test(sortCases, i => (int)i, int.MinValue);
        internal static void Test_Keys_UInt32(ISortCases sortCases) =>
            Test(sortCases, i => (uint)i, uint.MaxValue);
        internal static void Test_Keys_Int64(ISortCases sortCases) =>
            Test(sortCases, i => (long)i, long.MinValue);
        internal static void Test_Keys_UInt64(ISortCases sortCases) =>
            Test(sortCases, i => (ulong)i, ulong.MaxValue);
        internal static void Test_Keys_Single(ISortCases sortCases) =>
            Test(sortCases, i => (float)i, float.NaN);
        internal static void Test_Keys_Double(ISortCases sortCases) =>
            Test(sortCases, i => (double)i, double.NaN);
        internal static void Test_Keys_Boolean(ISortCases sortCases) =>
            Test(sortCases, i => i % 2 == 0, false);
        internal static void Test_Keys_Char(ISortCases sortCases) =>
            Test(sortCases, i => (char)i, char.MaxValue);
        internal static void Test_Keys_String(ISortCases sortCases) =>
            Test(sortCases, i => i.ToString("D9"), null);
        internal static void Test_Keys_ComparableStructInt32(ISortCases sortCases) =>
            Test(sortCases, i => new ComparableStructInt32(i), new ComparableStructInt32(int.MinValue));
        internal static void Test_Keys_ComparableClassInt32(ISortCases sortCases) =>
            Test(sortCases, i => new ComparableClassInt32(i), null);
        internal static void Test_Keys_BogusComparable(ISortCases sortCases) =>
            Test(sortCases, i => new BogusComparable(i), null);

        internal static void Test<TKey>(ISortCases sortCase, Func<int, TKey> toKey, TKey specialKey)
            where TKey : IComparable<TKey>
        {
            foreach (var unsorted in sortCase.EnumerateTests(toKey, specialKey))
            {
                TestSortOverloads(unsorted);
            }
        }
        internal static void TestSortOverloads<TKey>(ArraySegment<TKey> keys)
            where TKey : IComparable<TKey>
        {
            var copy = (TKey[])keys.Array.Clone();

            TestSort(keys);
            TestSort(keys, Comparer<TKey>.Default);
            TestSort(keys, Comparer<TKey>.Default.Compare);
            TestSort(keys, new CustomComparer<TKey>());
            TestSort(keys, (IComparer<TKey>)null);
            TestSort(keys, new BogusComparer<TKey>());
        }
        internal static void TestSort<TKey>(
            ArraySegment<TKey> keysToSort)
            where TKey : IComparable<TKey>
        {
            var expected = new ArraySegment<TKey>((TKey[])keysToSort.Array.Clone(),
                keysToSort.Offset, keysToSort.Count);

            var expectedException = RunAndCatchException(() =>
                Array.Sort(expected.Array, expected.Offset, expected.Count));

            var actualException = RunAndCatchException(() =>
            {
                Span<TKey> keysSpan = keysToSort;
                keysSpan.Sort();
            });

            AssertExceptionEquals(expectedException, actualException);
            // We assert the full arrays are as expected, to check for possible under/overflow
            Assert.Equal(expected.Array, keysToSort.Array);
        }

        internal static void TestSort<TKey, TComparer>(
            ArraySegment<TKey> keysToSort,
            TComparer comparer)
            where TComparer : IComparer<TKey>
        {
            var expected = new ArraySegment<TKey>((TKey[])keysToSort.Array.Clone(),
                keysToSort.Offset, keysToSort.Count);

            var expectedException = RunAndCatchException(() =>
                Array.Sort(expected.Array, expected.Offset, expected.Count, comparer));

            var actualException = RunAndCatchException(() =>
            {
                Span<TKey> keysSpan = keysToSort;
                keysSpan.Sort(comparer);
            });

            AssertExceptionEquals(expectedException, actualException);

            // We assert the full arrays are as expected, to check for possible under/overflow
            Assert.Equal(expected.Array, keysToSort.Array);
        }
        internal static void TestSort<TKey>(
            ArraySegment<TKey> keysToSort,
            Comparison<TKey> comparison)
        {
            var expected = new ArraySegment<TKey>((TKey[])keysToSort.Array.Clone(),
                keysToSort.Offset, keysToSort.Count);
            // Array.Sort doesn't have a comparison version for segments
            Exception expectedException = null;
            if (expected.Offset == 0 && expected.Count == expected.Array.Length)
            {
                expectedException = RunAndCatchException(() =>
                    Array.Sort(expected.Array, comparison));
            }
            else
            {
                expectedException = RunAndCatchException(() =>
                    Array.Sort(expected.Array, expected.Offset, expected.Count,
                    new ComparisonComparer<TKey>(comparison)));
            }

            var actualException = RunAndCatchException(() =>
            {
                Span<TKey> keysSpan = keysToSort;
                keysSpan.Sort(comparison);
            });

            AssertExceptionEquals(expectedException, actualException);
            // We assert the full arrays are as expected, to check for possible under/overflow
            Assert.Equal(expected.Array, keysToSort.Array);
        }

        internal static void Test_KeysValues_Int8_Int32(ISortCases sortCases) =>
            Test(sortCases, i => (sbyte)i, sbyte.MinValue, i => i);
        internal static void Test_KeysValues_UInt8_Int32(ISortCases sortCases) =>
            Test(sortCases, i => (byte)i, byte.MaxValue, i => i);
        internal static void Test_KeysValues_Int16_Int32(ISortCases sortCases) =>
            Test(sortCases, i => (short)i, short.MinValue, i => i);
        internal static void Test_KeysValues_UInt16_Int32(ISortCases sortCases) =>
            Test(sortCases, i => (ushort)i, ushort.MaxValue, i => i);
        internal static void Test_KeysValues_Int32_Int32(ISortCases sortCases) =>
            Test(sortCases, i => (int)i, int.MinValue, i => i);
        internal static void Test_KeysValues_UInt32_Int32(ISortCases sortCases) =>
            Test(sortCases, i => (uint)i, uint.MaxValue, i => i);
        internal static void Test_KeysValues_Int64_Int32(ISortCases sortCases) =>
            Test(sortCases, i => (long)i, long.MinValue, i => i);
        internal static void Test_KeysValues_UInt64_Int32(ISortCases sortCases) =>
            Test(sortCases, i => (ulong)i, ulong.MaxValue, i => i);
        internal static void Test_KeysValues_Single_Int32(ISortCases sortCases) =>
            Test(sortCases, i => (float)i, float.NaN, i => i);
        internal static void Test_KeysValues_Double_Int32(ISortCases sortCases) =>
            Test(sortCases, i => (double)i, double.NaN, i => i);
        internal static void Test_KeysValues_Boolean_Int32(ISortCases sortCases) =>
            Test(sortCases, i => i % 2 == 0, false, i => i);
        internal static void Test_KeysValues_Char_Int32(ISortCases sortCases) =>
            Test(sortCases, i => (char)i, char.MaxValue, i => i);
        internal static void Test_KeysValues_String_Int32(ISortCases sortCases) =>
            Test(sortCases, i => i.ToString("D9"), null, i => i);
        internal static void Test_KeysValues_ComparableStructInt32_Int32(ISortCases sortCases) =>
            Test(sortCases, i => new ComparableStructInt32(i), new ComparableStructInt32(int.MinValue), i => i);
        internal static void Test_KeysValues_ComparableClassInt32_Int32(ISortCases sortCases) =>
            Test(sortCases, i => new ComparableClassInt32(i), null, i => i);
        internal static void Test_KeysValues_BogusComparable_Int32(ISortCases sortCases) =>
            Test(sortCases, i => new BogusComparable(i), null, i => i);

        internal static void Test<TKey, TValue>(ISortCases sortCase,
            Func<int, TKey> toKey, TKey specialKey, Func<int, TValue> toValue)
            where TKey : IComparable<TKey>
        {
            foreach (var unsortedKeys in sortCase.EnumerateTests(toKey, specialKey))
            {
                var length = unsortedKeys.Array.Length;
                var values = new TValue[length];
                // Items are always based on "unique" int values
                new IncrementingSpanFiller().Fill(values, length, toValue);
                var unsortedValues = new ArraySegment<TValue>(values, unsortedKeys.Offset, unsortedKeys.Count);
                TestSortOverloads(unsortedKeys, unsortedValues);
            }
        }
        internal static void TestSortOverloads<TKey, TValue>(ArraySegment<TKey> keys, ArraySegment<TValue> values)
            where TKey : IComparable<TKey>
        {
            var copy = (TKey[])keys.Array.Clone();

            TestSort(keys, values);
            TestSort(keys, values, Comparer<TKey>.Default);
            TestSort(keys, values, Comparer<TKey>.Default.Compare);
            TestSort(keys, values, new CustomComparer<TKey>());
            TestSort(keys, values, (IComparer<TKey>)null);
            TestSort(keys, values, new BogusComparer<TKey>());
        }
        internal static void TestSort<TKey, TValue>(
            ArraySegment<TKey> keysToSort, ArraySegment<TValue> valuesToSort)
            where TKey : IComparable<TKey>
        {
            var expectedKeys = new ArraySegment<TKey>((TKey[])keysToSort.Array.Clone(),
                keysToSort.Offset, keysToSort.Count);
            var expectedValues = new ArraySegment<TValue>((TValue[])valuesToSort.Array.Clone(),
                valuesToSort.Offset, valuesToSort.Count);
            Assert.Equal(expectedKeys.Offset, expectedValues.Offset);
            Assert.Equal(expectedKeys.Count, expectedValues.Count);

            var expectedException = RunAndCatchException(() =>
            {
                if (expectedKeys.Offset == 0 && expectedKeys.Count == expectedKeys.Array.Length)
                {
                    Array.Sort(expectedKeys.Array, expectedValues.Array, expectedKeys.Offset, expectedKeys.Count);
                }
                else
                {
                    // HACK: To avoid the fact that .net core Array.Sort still computes
                    //       the depth limit incorrectly, see https://github.com/dotnet/coreclr/pull/16002
                    //       This can result in Array.Sort NOT calling HeapSort when Span does.
                    //       And then values for identical keys may be sorted differently.
                    Span<TKey> ks = expectedKeys;
                    Span<TValue> vs = expectedValues;
                    var noSegmentKeys = ks.ToArray();
                    var noSegmentValues = vs.ToArray();
                    try
                    {
                        Array.Sort(noSegmentKeys, noSegmentValues);
                    }
                    finally
                    {
                        new Span<TKey>(noSegmentKeys).CopyTo(ks);
                        new Span<TValue>(noSegmentValues).CopyTo(vs);
                    }
                }
            });

            var actualException = RunAndCatchException(() =>
            {
                Span<TKey> keysSpan = keysToSort;
                Span<TValue> valuesSpan = valuesToSort;
                keysSpan.Sort(valuesSpan);
            });

            AssertExceptionEquals(expectedException, actualException);
            // We assert the full arrays are as expected, to check for possible under/overflow
            Assert.Equal(expectedKeys.Array, keysToSort.Array);
            Assert.Equal(expectedValues.Array, valuesToSort.Array);
        }
        internal static void TestSort<TKey, TValue, TComparer>(
            ArraySegment<TKey> keysToSort, ArraySegment<TValue> valuesToSort,
            TComparer comparer)
            where TComparer : IComparer<TKey>
        {
            var expectedKeys = new ArraySegment<TKey>((TKey[])keysToSort.Array.Clone(),
                keysToSort.Offset, keysToSort.Count);
            var expectedValues = new ArraySegment<TValue>((TValue[])valuesToSort.Array.Clone(),
                valuesToSort.Offset, valuesToSort.Count);

            var expectedException = RunAndCatchException(() =>
                Array.Sort(expectedKeys.Array, expectedValues.Array, expectedKeys.Offset, expectedKeys.Count, comparer));

            var actualException = RunAndCatchException(() =>
            {
                Span<TKey> keysSpan = keysToSort;
                Span<TValue> valuesSpan = valuesToSort;
                keysSpan.Sort(valuesSpan, comparer);
            });

            AssertExceptionEquals(expectedException, actualException);
            // We assert the full arrays are as expected, to check for possible under/overflow
            Assert.Equal(expectedKeys.Array, keysToSort.Array);
            Assert.Equal(expectedValues.Array, valuesToSort.Array);
        }
        internal static void TestSort<TKey, TValue>(
            ArraySegment<TKey> keysToSort, ArraySegment<TValue> valuesToSort,
            Comparison<TKey> comparison)
        {
            var expectedKeys = new ArraySegment<TKey>((TKey[])keysToSort.Array.Clone(),
                keysToSort.Offset, keysToSort.Count);
            var expectedValues = new ArraySegment<TValue>((TValue[])valuesToSort.Array.Clone(),
                valuesToSort.Offset, valuesToSort.Count);
            // Array.Sort doesn't have a comparison version for segments
            var expectedException = RunAndCatchException(() =>
                Array.Sort(expectedKeys.Array, expectedValues.Array, expectedKeys.Offset, expectedKeys.Count, new ComparisonComparer<TKey>(comparison)));

            var actualException = RunAndCatchException(() =>
            {
                Span<TKey> keysSpan = keysToSort;
                Span<TValue> valuesSpan = valuesToSort;
                keysSpan.Sort(valuesSpan, comparison);
            });

            AssertExceptionEquals(expectedException, actualException);
            // We assert the full arrays are as expected, to check for possible under/overflow
            Assert.Equal(expectedKeys.Array, keysToSort.Array);
            Assert.Equal(expectedValues.Array, valuesToSort.Array);
        }

        public interface ISortCases
        {
            IEnumerable<ArraySegment<TKey>> EnumerateTests<TKey>(Func<int, TKey> toKey, TKey specialKey);
        }
        public class FillerSortCases : ISortCases
        {
            public FillerSortCases(int maxLength, ISpanFiller filler)
            {
                MaxLength = maxLength;
                Filler = filler ?? throw new ArgumentNullException(nameof(filler));
            }

            public int MinLength => 2;
            public int MaxLength { get; }
            public ISpanFiller Filler { get; }

            public IEnumerable<ArraySegment<TKey>> EnumerateTests<TKey>(Func<int, TKey> toKey, TKey specialKey)
            {
                for (int length = MinLength; length <= MaxLength; length++)
                {
                    var unsorted = new TKey[length];
                    Filler.Fill(unsorted, length, toKey);
                    yield return new ArraySegment<TKey>(unsorted);
                }
            }

            public override string ToString()
            {
                return $"Lengths [{MinLength}, {MaxLength,4}] {nameof(Filler)}={Filler.GetType().Name.Replace("SpanFiller", "")} ";
            }
        }
        public class LengthZeroSortCases : ISortCases
        {
            public IEnumerable<ArraySegment<TKey>> EnumerateTests<TKey>(Func<int, TKey> toKey, TKey specialKey)
            {
                yield return new ArraySegment<TKey>(Array.Empty<TKey>());
            }

            public override string ToString()
                => GetType().Name.Replace(nameof(ISortCases).Remove(0, 1), string.Empty);
        }
        public class LengthOneSortCases : ISortCases
        {
            public IEnumerable<ArraySegment<TKey>> EnumerateTests<TKey>(Func<int, TKey> toKey, TKey specialKey)
            {
                yield return new ArraySegment<TKey>(new[] { toKey(-1) });
            }

            public override string ToString()
                => GetType().Name.Replace(nameof(ISortCases).Remove(0, 1), string.Empty);
        }
        public class AllLengthTwoSortCases : ISortCases
        {
            public IEnumerable<ArraySegment<TKey>> EnumerateTests<TKey>(Func<int, TKey> toKey, TKey specialKey)
            {
                const int length = 2;
                for (int i = 0; i < length; i++)
                {
                    for (int j = 0; j < length; j++)
                    {
                        yield return new ArraySegment<TKey>(new[] { toKey(i), toKey(j) });
                    }
                }
            }

            public override string ToString()
                => GetType().Name.Replace(nameof(ISortCases).Remove(0, 1), string.Empty);
        }
        public class AllLengthThreeSortCases : ISortCases
        {
            public IEnumerable<ArraySegment<TKey>> EnumerateTests<TKey>(Func<int, TKey> toKey, TKey specialKey)
            {
                const int length = 3;
                for (int i = 0; i < length; i++)
                {
                    for (int j = 0; j < length; j++)
                    {
                        for (int k = 0; k < length; k++)
                        {
                            yield return new ArraySegment<TKey>(new[] { toKey(i), toKey(j), toKey(k) });
                        }
                    }
                }
            }

            public override string ToString()
                => GetType().Name.Replace(nameof(ISortCases).Remove(0, 1), string.Empty);
        }
        public class AllLengthFourSortCases : ISortCases
        {
            public IEnumerable<ArraySegment<TKey>> EnumerateTests<TKey>(Func<int, TKey> toKey, TKey specialKey)
            {
                const int length = 4;
                for (int i = 0; i < length; i++)
                {
                    for (int j = 0; j < length; j++)
                    {
                        for (int k = 0; k < length; k++)
                        {
                            for (int l = 0; l < length; l++)
                            {
                                yield return new ArraySegment<TKey>(new[] { toKey(i), toKey(j), toKey(k), toKey(l) });
                            }
                        }
                    }
                }
            }

            public override string ToString()
                => GetType().Name.Replace(nameof(ISortCases).Remove(0, 1), string.Empty);
        }
        public class PadAndSliceSortCases : ISortCases
        {
            readonly ISortCases _sortCases;
            readonly int _slicePadding;

            public PadAndSliceSortCases(ISortCases sortCases, int slicePadding)
            {
                _sortCases = sortCases ?? throw new ArgumentNullException(nameof(sortCases));
                _slicePadding = slicePadding;
            }

            public IEnumerable<ArraySegment<TKey>> EnumerateTests<TKey>(Func<int, TKey> toKey, TKey specialKey)
            {
                return _sortCases.EnumerateTests(toKey, specialKey).Select(ks =>
                {
                    var newKeys = new TKey[ks.Count + 2 * _slicePadding];
                    Array.Copy(ks.Array, ks.Offset, newKeys, _slicePadding, ks.Count);
                    var padKey = toKey(unchecked((int)0xCECECECE));
                    for (int i = 0; i < _slicePadding; i++)
                    {
                        newKeys[i] = padKey;
                        newKeys[newKeys.Length - i - 1] = padKey;
                    }
                    return new ArraySegment<TKey>(newKeys, _slicePadding, ks.Count);
                });
            }

            public override string ToString()
                => GetType().Name.Replace(nameof(ISortCases).Remove(0, 1), string.Empty) +
                $":{_slicePadding} " + _sortCases.ToString();
        }
        public class StepwiseSpecialSortCases : ISortCases
        {
            readonly ISortCases _sortCases;
            readonly int _step;

            public StepwiseSpecialSortCases(ISortCases sortCases, int step)
            {
                _sortCases = sortCases ?? throw new ArgumentNullException(nameof(sortCases));
                _step = step;
            }

            public IEnumerable<ArraySegment<TKey>> EnumerateTests<TKey>(Func<int, TKey> toKey, TKey specialKey)
            {
                return _sortCases.EnumerateTests(toKey, specialKey).Select(ks =>
                {
                    for (int i = 0; i < ks.Count; i += _step)
                    {
                        ks.Array[i + ks.Offset] = specialKey;
                    }
                    return ks;
                });
            }

            public override string ToString()
                => GetType().Name.Replace(nameof(ISortCases).Remove(0, 1), string.Empty) +
                $":{_step} " + _sortCases.ToString();
        }


        internal struct CustomComparer<TKey> : IComparer<TKey>
            where TKey : IComparable<TKey>
        {
            public int Compare(TKey x, TKey y) => object.ReferenceEquals(x, y) ? 0 : (x != null ? x.CompareTo(y) : -1);
        }

        internal struct StructCustomComparer<TKey> : IComparer<TKey>
            where TKey : struct, IComparable<TKey>
        {
            public int Compare(TKey x, TKey y) => x.CompareTo(y);
        }

        internal struct BogusComparer<TKey> : IComparer<TKey>
            where TKey : IComparable<TKey>
        {
            public int Compare(TKey x, TKey y) => 1; // Always greater
        }

        public struct ComparableStructInt32 : IComparable<ComparableStructInt32>
        {
            public readonly int Value;

            public ComparableStructInt32(int value)
            {
                Value = value;
            }

            public int CompareTo(ComparableStructInt32 other)
            {
                return this.Value.CompareTo(other.Value);
            }

            public override int GetHashCode() => Value.GetHashCode();

            public override string ToString() => $"ComparableStruct {Value}";
        }

        public class ComparableClassInt32 : IComparable<ComparableClassInt32>
        {
            public readonly int Value;

            public ComparableClassInt32(int value)
            {
                Value = value;
            }

            public int CompareTo(ComparableClassInt32 other)
            {
                return other != null ? Value.CompareTo(other.Value) : 1;
            }

            public override int GetHashCode() => Value.GetHashCode();

            public override string ToString() => $"ComparableClass {Value}";
        }

        public class BogusComparable
            : IComparable<BogusComparable>
            , IEquatable<BogusComparable>
        {
            public readonly int Value;

            public BogusComparable(int value)
            {
                Value = value;
            }

            public int CompareTo(BogusComparable other) => 1;

            public bool Equals(BogusComparable other)
            {
                if (other == null)
                    return false;
                return Value.Equals(other.Value);
            }

            public override int GetHashCode() => Value.GetHashCode();

            public override string ToString() => $"Bogus {Value}";
        }

        public struct ValueIdStruct : IComparable<ValueIdStruct>, IEquatable<ValueIdStruct>
        {
            public ValueIdStruct(int value, int identity)
            {
                Value = value;
                Id = identity;
            }

            public int Value { get; }
            public int Id { get; }

            // Sort by value
            public int CompareTo(ValueIdStruct other) =>
                Value.CompareTo(other.Value);

            // Check equality by both
            public bool Equals(ValueIdStruct other) =>
                Value.Equals(other.Value) && Id.Equals(other.Id);

            public override bool Equals(object obj)
            {
                if (obj is ValueIdStruct)
                {
                    return Equals((ValueIdStruct)obj);
                }
                return false;
            }

            public override int GetHashCode() => Value.GetHashCode();

            public override string ToString() => $"{Value} Id:{Id}";
        }

        public class ValueIdClass : IComparable<ValueIdClass>, IEquatable<ValueIdClass>
        {
            public ValueIdClass(int value, int identity)
            {
                Value = value;
                Id = identity;
            }

            public int Value { get; }
            public int Id { get; }

            // Sort by value
            public int CompareTo(ValueIdClass other) =>
                Value.CompareTo(other.Value);

            // Check equality by both
            public bool Equals(ValueIdClass other) =>
                other != null && Value.Equals(other.Value) && Id.Equals(other.Id);

            public override bool Equals(object obj)
            {
                return Equals(obj as ValueIdClass);
            }

            public override int GetHashCode() => Value.GetHashCode();

            public override string ToString() => $"{Value} Id:{Id}";
        }

        // Used for array sort
        internal class ComparisonComparer<TKey> : IComparer<TKey>
        {
            readonly Comparison<TKey> _comparison;

            public ComparisonComparer(Comparison<TKey> comparison)
            {
                _comparison = comparison;
            }

            public int Compare(TKey x, TKey y) => _comparison(x, y);
        }


        internal static Exception RunAndCatchException(Action sort)
        {
            try
            {
                sort();
            }
            catch (Exception e)
            {
                return e;
            }
            return null;
        }

        internal static void AssertExceptionEquals(Exception expectedException, Exception actualException)
        {
            if (expectedException != null)
            {
                Assert.IsType(expectedException.GetType(), actualException);
                if (expectedException.Message != actualException.Message)
                {
                    Assert.StartsWith("Unable to sort because the IComparable.CompareTo() method returns inconsistent results. Either a value does not compare equal to itself, or one value repeatedly compared to another value yields different results. IComparable: '", actualException.Message);
                    Assert.EndsWith("'.", actualException.Message);
                }
            }
            else
            {
                Assert.Null(actualException);
            }
        }
    }
}
