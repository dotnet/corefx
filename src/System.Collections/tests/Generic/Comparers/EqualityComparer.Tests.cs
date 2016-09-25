// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Tests;
using System.Diagnostics;
using System.Linq;
using Xunit;

namespace System.Collections.Generic.Tests
{
    public class EqualityComparerTests
    {
        public class EqualsData<T> : TheoryData<T, T, bool>
        {
            public IEnumerable<T> Items
            {
                get
                {
                    return this.Select(array => array[0])
                        .Concat(this.Select(array => array[1]))
                        .Cast<T>();
                }
            }
        }

        public class HashData<T> : TheoryData<T, int> { }

        [Theory]
        [MemberData(nameof(ByteData))]
        [MemberData(nameof(StringData))]
        [MemberData(nameof(IEquatableData))]
        [MemberData(nameof(Int16EnumData))]
        [MemberData(nameof(SByteEnumData))]
        [MemberData(nameof(Int32EnumData))]
        [MemberData(nameof(Int64EnumData))]
        [MemberData(nameof(NonEquatableValueTypeData))]
        [MemberData(nameof(ObjectData))]
        public void Equals<T>(T left, T right, bool expected)
        {
            var comparer = EqualityComparer<T>.Default;
            IEqualityComparer nonGenericComparer = comparer;

            Assert.Equal(expected, comparer.Equals(left, right));
            Assert.Equal(expected, comparer.Equals(right, left)); // Should be commutative.
            
            Assert.True(comparer.Equals(left, left)); // Should be reflexive.
            Assert.True(comparer.Equals(right, right));

            // If both sides are Ts then the explicit implementation of
            // IEqualityComparer.Equals should also succeed, with the same results
            Assert.Equal(expected, nonGenericComparer.Equals(left, right));
            Assert.Equal(expected, nonGenericComparer.Equals(right, left));

            Assert.True(nonGenericComparer.Equals(left, left));
            Assert.True(nonGenericComparer.Equals(right, right));

            // All comparers returned by EqualityComparer<T>.Default should be
            // able to handle nulls before dispatching to IEquatable<T>.Equals()
            if (default(T) == null)
            {
                T nil = default(T);

                Assert.True(comparer.Equals(nil, nil));

                Assert.Equal(left == null, comparer.Equals(left, nil));
                Assert.Equal(left == null, comparer.Equals(nil, left));

                Assert.Equal(right == null, comparer.Equals(right, nil));
                Assert.Equal(right == null, comparer.Equals(nil, right));

                // IEqualityComparer.Equals explicit implementation
                Assert.True(nonGenericComparer.Equals(nil, nil));

                Assert.Equal(left == null, nonGenericComparer.Equals(left, nil));
                Assert.Equal(left == null, nonGenericComparer.Equals(nil, left));

                Assert.Equal(right == null, nonGenericComparer.Equals(right, nil));
                Assert.Equal(right == null, nonGenericComparer.Equals(nil, right));
            }

            // GetHashCode: If 2 objects are equal, then their hash code should be the same.

            if (expected)
            {
                int hash = comparer.GetHashCode(left);

                Assert.Equal(hash, comparer.GetHashCode(left)); // Should return the same result across multiple invocations
                Assert.Equal(hash, comparer.GetHashCode(right));

                Assert.Equal(hash, nonGenericComparer.GetHashCode(left));
                Assert.Equal(hash, nonGenericComparer.GetHashCode(right));
            }
        }

        public static EqualsData<byte> ByteData()
        {
            return new EqualsData<byte>
            {
                { 3, 3, true },
                { 3, 4, false },
                { 0, 255, false },
                { 0, 128, false }
            };
        }

        public static EqualsData<string> StringData()
        {
            return new EqualsData<string>
            {
                { "foo", "foo", true },
                { string.Empty, null, false },
                { "bar", new string("bar".ToCharArray()), true },
                { "foo", "bar", false }
            };
        }
        
        public static EqualsData<Equatable> IEquatableData()
        {
            var one = new Equatable(1);

            return new EqualsData<Equatable>
            {
                { one, one, true },
                { one, new Equatable(1), true },
                { new Equatable(int.MinValue + 1), new Equatable(1), false },
                { new Equatable(-1), new Equatable(int.MaxValue), false }
            };
        }

        public static EqualsData<Int16Enum> Int16EnumData()
        {
            return new EqualsData<Int16Enum>
            {
                { (Int16Enum)(-2), (Int16Enum)(-4), false }, // Negative shorts hash specially.
                { Int16Enum.Two, Int16Enum.Two, true },
                { Int16Enum.Min, Int16Enum.Max, false },
                { Int16Enum.Min, Int16Enum.Min, true },
                { Int16Enum.One, Int16Enum.Min + 1, false }
            };
        }

        public static EqualsData<SByteEnum> SByteEnumData()
        {
            return new EqualsData<SByteEnum>
            {
                { (SByteEnum)(-2), (SByteEnum)(-4), false }, // Negative sbytes hash specially.
                { SByteEnum.Two, SByteEnum.Two, true },
                { SByteEnum.Min, SByteEnum.Max, false },
                { SByteEnum.Min, SByteEnum.Min, true },
                { SByteEnum.One, SByteEnum.Min + 1, false }
            };
        }

        public static EqualsData<Int32Enum> Int32EnumData()
        {
            return new EqualsData<Int32Enum>
            {
                { (Int32Enum)(-2), (Int32Enum)(-4), false },
                { Int32Enum.Two, Int32Enum.Two, true },
                { Int32Enum.Min, Int32Enum.Max, false },
                { Int32Enum.Min, Int32Enum.Min, true },
                { Int32Enum.One, Int32Enum.Min + 1, false }
            };
        }

        public static EqualsData<Int64Enum> Int64EnumData()
        {
            return new EqualsData<Int64Enum>
            {
                { (Int64Enum)(-2), (Int64Enum)(-4), false },
                { Int64Enum.Two, Int64Enum.Two, true },
                { Int64Enum.Min, Int64Enum.Max, false },
                { Int64Enum.Min, Int64Enum.Min, true },
                { Int64Enum.One, Int64Enum.Min + 1, false }
            };
        }
        
        public static EqualsData<NonEquatableValueType> NonEquatableValueTypeData()
        {
            // Comparisons for structs that do not override ValueType.Equals or
            // ValueType.GetHashCode should still work as expected.

            var one = new NonEquatableValueType { Value = 1 };

            return new EqualsData<NonEquatableValueType>
            {
                { new NonEquatableValueType(), new NonEquatableValueType(), true },
                { one, one, true },
                { new NonEquatableValueType(-1), new NonEquatableValueType(), false },
                { new NonEquatableValueType(2), new NonEquatableValueType(2), true }
            };
        }

        public static EqualsData<object> ObjectData()
        {
            var obj = new object();

            return new EqualsData<object>
            {
                { obj, obj, true },
                { obj, new object(), false },
                { obj, null, false }
            };
        }

        [Theory]
        [MemberData(nameof(ByteHashData))]
        [MemberData(nameof(StringHashData))]
        [MemberData(nameof(IEquatableHashData))]
        [MemberData(nameof(Int16EnumHashData))]
        [MemberData(nameof(SByteEnumHashData))]
        [MemberData(nameof(Int32EnumHashData))]
        [MemberData(nameof(Int64EnumHashData))]
        [MemberData(nameof(NonEquatableValueTypeHashData))]
        [MemberData(nameof(ObjectHashData))]
        public void GetHashCode<T>(T value, int expected)
        {
            var comparer = EqualityComparer<T>.Default;
            IEqualityComparer nonGenericComparer = comparer;

            Assert.Equal(expected, comparer.GetHashCode(value));
            Assert.Equal(expected, comparer.GetHashCode(value)); // Should return the same result across multiple invocations

            Assert.Equal(expected, nonGenericComparer.GetHashCode(value));
            Assert.Equal(expected, nonGenericComparer.GetHashCode(value));

            // We should deal with nulls before dispatching to object.GetHashCode.
            if (default(T) == null)
            {
                T nil = default(T);

                Assert.Equal(0, comparer.GetHashCode(nil));

                Assert.Equal(0, nonGenericComparer.GetHashCode(nil));
            }
        }

        public static HashData<byte> ByteHashData() => GenerateHashData(ByteData());

        public static HashData<string> StringHashData() => GenerateHashData(StringData());

        public static HashData<Equatable> IEquatableHashData() => GenerateHashData(IEquatableData());

        public static HashData<Int16Enum> Int16EnumHashData() => GenerateHashData(Int16EnumData());

        public static HashData<SByteEnum> SByteEnumHashData() => GenerateHashData(SByteEnumData());

        public static HashData<Int32Enum> Int32EnumHashData() => GenerateHashData(Int32EnumData());

        public static HashData<Int64Enum> Int64EnumHashData() => GenerateHashData(Int64EnumData());

        public static HashData<NonEquatableValueType> NonEquatableValueTypeHashData() => GenerateHashData(NonEquatableValueTypeData());

        public static HashData<object> ObjectHashData() => GenerateHashData(ObjectData());

        private static HashData<T> GenerateHashData<T>(EqualsData<T> input)
        {
            Debug.Assert(input != null);

            var result = new HashData<T>();
            
            foreach (T item in input.Items)
            {
                result.Add(item, item?.GetHashCode() ?? 0);
            }

            return result;
        }
    }
}
