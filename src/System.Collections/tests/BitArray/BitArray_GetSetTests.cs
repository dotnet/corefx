// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace System.Collections.Tests
{
    public static class BitArray_GetSetTests
    {
        private const int BitsPerByte = 8;
        private const int BitsPerInt32 = 32;

        public static IEnumerable<object[]> Get_Set_Data()
        {
            foreach (int size in new[] { 0, 1, BitsPerByte, BitsPerByte * 2, BitsPerInt32, BitsPerInt32 * 2 })
            {
                foreach (bool def in new[] { true, false })
                {
                    yield return new object[] { def, Enumerable.Repeat(true, size).ToArray() };
                    yield return new object[] { def, Enumerable.Repeat(false, size).ToArray() };
                    yield return new object[] { def, Enumerable.Range(0, size).Select(i => i % 2 == 1).ToArray() };
                }
            }
        }

        [Theory]
        [MemberData(nameof(Get_Set_Data))]
        public static void Get_Set(bool def, bool[] newValues)
        {
            BitArray bitArray = new BitArray(newValues.Length, def);
            for (int i = 0; i < newValues.Length; i++)
            {
                bitArray.Set(i, newValues[i]);
                Assert.Equal(newValues[i], bitArray[i]);
                Assert.Equal(newValues[i], bitArray.Get(i));
            }
        }

        [Fact]
        public static void Get_InvalidIndex_ThrowsArgumentOutOfRangeException()
        {
            BitArray bitArray = new BitArray(4);
            Assert.Throws<ArgumentOutOfRangeException>("index", () => bitArray.Get(-1));
            Assert.Throws<ArgumentOutOfRangeException>("index", () => bitArray.Get(bitArray.Length));

            Assert.Throws<ArgumentOutOfRangeException>("index", () => bitArray[-1]);
            Assert.Throws<ArgumentOutOfRangeException>("index", () => bitArray[bitArray.Length]);
        }

        [Fact]
        public static void Set_InvalidIndex_ThrowsArgumentOutOfRangeException()
        {
            BitArray bitArray = new BitArray(4);
            Assert.Throws<ArgumentOutOfRangeException>("index", () => bitArray.Set(-1, true));
            Assert.Throws<ArgumentOutOfRangeException>("index", () => bitArray.Set(bitArray.Length, true));

            Assert.Throws<ArgumentOutOfRangeException>("index", () => bitArray[-1] = true);
            Assert.Throws<ArgumentOutOfRangeException>("index", () => bitArray[bitArray.Length] = true);
        }

        [Theory]
        [InlineData(0, true)]
        [InlineData(0, false)]
        [InlineData(1, true)]
        [InlineData(1, false)]
        [InlineData(BitsPerByte, true)]
        [InlineData(BitsPerByte, false)]
        [InlineData(BitsPerByte + 1, true)]
        [InlineData(BitsPerByte + 1, false)]
        [InlineData(BitsPerInt32, true)]
        [InlineData(BitsPerInt32, false)]
        [InlineData(BitsPerInt32 + 1, true)]
        [InlineData(BitsPerInt32 + 1, false)]
        public static void SetAll(int size, bool defaultValue)
        {
            BitArray bitArray = new BitArray(size, defaultValue);
            bitArray.SetAll(!defaultValue);
            for (int i = 0; i < bitArray.Length; i++)
            {
                Assert.Equal(!defaultValue, bitArray[i]);
                Assert.Equal(!defaultValue, bitArray.Get(i));
            }

            bitArray.SetAll(defaultValue);
            for (int i = 0; i < bitArray.Length; i++)
            {
                Assert.Equal(defaultValue, bitArray[i]);
                Assert.Equal(defaultValue, bitArray.Get(i));
            }
        }

        public static IEnumerable<object[]> GetEnumerator_Data()
        {
            foreach (int size in new[] { 0, 1, BitsPerByte, BitsPerByte + 1, BitsPerInt32, BitsPerInt32 + 1 })
            {
                foreach (bool lead in new[] { true, false })
                {
                    yield return new object[] { Enumerable.Range(0, size).Select(i => lead ^ (i % 2 == 0)).ToArray() };
                }
            }
        }

        [Theory]
        [MemberData(nameof(GetEnumerator_Data))]
        public static void GetEnumerator(bool[] values)
        {
            BitArray bitArray = new BitArray(values);
            Assert.NotSame(bitArray.GetEnumerator(), bitArray.GetEnumerator());
            IEnumerator enumerator = bitArray.GetEnumerator();
            for (int i = 0; i < 2; i++)
            {
                int counter = 0;
                while (enumerator.MoveNext())
                {
                    Assert.Equal(bitArray[counter], enumerator.Current);
                    counter++;
                }
                Assert.Equal(bitArray.Length, counter);
                enumerator.Reset();
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(BitsPerByte)]
        [InlineData(BitsPerByte + 1)]
        [InlineData(BitsPerInt32)]
        [InlineData(BitsPerInt32 + 1)]
        public static void GetEnumerator_Invalid(int size)
        {
            BitArray bitArray = new BitArray(size, true);
            IEnumerator enumerator = bitArray.GetEnumerator();

            // Has not started enumerating
            Assert.Throws<InvalidOperationException>(() => enumerator.Current);

            // Has finished enumerating
            while (enumerator.MoveNext()) ;
            Assert.Throws<InvalidOperationException>(() => enumerator.Current);

            // Has resetted enumerating
            enumerator.Reset();
            Assert.Throws<InvalidOperationException>(() => enumerator.Current);

            // Has modified underlying collection
            if (size > 0)
            {
                enumerator.MoveNext();
                bitArray[0] = false;
                Assert.True((bool)enumerator.Current);
                Assert.Throws<InvalidOperationException>(() => enumerator.MoveNext());
                Assert.Throws<InvalidOperationException>(() => enumerator.Reset());
            }
        }

        public static IEnumerable<object[]> Length_Set_Data()
        {
            int[] sizes = { 1, BitsPerByte, BitsPerByte + 1, BitsPerInt32, BitsPerInt32 + 1 };
            foreach (int original in sizes.Concat(new[] { 16384 }))
            {
                foreach (int n in sizes)
                {
                    yield return new object[] { original, n };
                }
            }
        }

        [Theory]
        [MemberData(nameof(Length_Set_Data))]
        public static void Length_Set(int originalSize, int newSize)
        {
            BitArray bitArray = new BitArray(originalSize, true);
            bitArray.Length = newSize;
            Assert.Equal(newSize, bitArray.Length);
            for (int i = 0; i < Math.Min(originalSize, bitArray.Length); i++)
            {
                Assert.True(bitArray[i]);
                Assert.True(bitArray.Get(i));
            }
            for (int i = originalSize; i < newSize; i++)
            {
                Assert.False(bitArray[i]);
                Assert.False(bitArray.Get(i));
            }
            Assert.Throws<ArgumentOutOfRangeException>("index", () => bitArray[newSize]);
            Assert.Throws<ArgumentOutOfRangeException>("index", () => bitArray.Get(newSize));

            // Decrease then increase size
            bitArray.Length = 0;
            Assert.Equal(0, bitArray.Length);

            bitArray.Length = newSize;
            Assert.Equal(newSize, bitArray.Length);
            Assert.False(bitArray.Get(0));
            Assert.False(bitArray.Get(newSize - 1));
        }

        [Fact]
        public static void Length_Set_InvalidLength_ThrowsArgumentOutOfRangeException()
        {
            BitArray bitArray = new BitArray(1);
            Assert.Throws<ArgumentOutOfRangeException>(() => bitArray.Length = -1);
        }

        public static IEnumerable<object[]> CopyTo_Array_TestData()
        {
            yield return new object[] { new BitArray(0), 0, 0, new bool[0], default(bool) };
            yield return new object[] { new BitArray(0), 0, 0, new byte[0], default(byte) };
            yield return new object[] { new BitArray(0), 0, 0, new int[0], default(int) };

            foreach (int bitArraySize in new[] { 0, 1, BitsPerByte, BitsPerByte * 2, BitsPerInt32, BitsPerInt32 * 2 })
            {
                BitArray allTrue = new BitArray(Enumerable.Repeat(true, bitArraySize).ToArray());
                BitArray allFalse = new BitArray(Enumerable.Repeat(false, bitArraySize).ToArray());
                BitArray alternating = new BitArray(Enumerable.Range(0, bitArraySize).Select(i => i % 2 == 1).ToArray());

                foreach (var d in new[] { Tuple.Create(bitArraySize, 0),
                    Tuple.Create(bitArraySize * 2 + 1, 0),
                    Tuple.Create(bitArraySize * 2 + 1, bitArraySize + 1),
                    Tuple.Create(bitArraySize * 2 + 1, bitArraySize / 2 + 1) })
                {
                    int arraySize = d.Item1;
                    int index = d.Item2;

                    yield return new object[] { allTrue, arraySize, index, Enumerable.Repeat(true, bitArraySize).ToArray(), default(bool) };
                    yield return new object[] { allFalse, arraySize, index, Enumerable.Repeat(false, bitArraySize).ToArray(), default(bool) };
                    yield return new object[] { alternating, arraySize, index, Enumerable.Range(0, bitArraySize).Select(i => i % 2 == 1).ToArray(), default(bool) };

                    if (bitArraySize >= BitsPerByte)
                    {
                        yield return new object[] { allTrue, arraySize / BitsPerByte, index / BitsPerByte, Enumerable.Repeat((byte)0xff, bitArraySize / BitsPerByte).ToArray(), default(byte) };
                        yield return new object[] { allFalse, arraySize / BitsPerByte, index / BitsPerByte, Enumerable.Repeat((byte)0x00, bitArraySize / BitsPerByte).ToArray(), default(byte) };
                        yield return new object[] { alternating, arraySize / BitsPerByte, index / BitsPerByte, Enumerable.Repeat((byte)0xaa, bitArraySize / BitsPerByte).ToArray(), default(byte) };
                    }

                    if (bitArraySize >= BitsPerInt32)
                    {
                        yield return new object[] { allTrue, arraySize / BitsPerInt32, index / BitsPerInt32, Enumerable.Repeat(unchecked((int)0xffffffff), bitArraySize / BitsPerInt32).ToArray(), default(int) };
                        yield return new object[] { allFalse, arraySize / BitsPerInt32, index / BitsPerInt32, Enumerable.Repeat(0x00000000, bitArraySize / BitsPerInt32).ToArray(), default(int) };
                        yield return new object[] { alternating, arraySize / BitsPerInt32, index / BitsPerInt32, Enumerable.Repeat(unchecked((int)0xaaaaaaaa), bitArraySize / BitsPerInt32).ToArray(), default(int) };
                    }
                }
            }
        }

        [Theory]
        [MemberData(nameof(CopyTo_Array_TestData))]
        public static void CopyTo<T>(BitArray bitArray, int length, int index, T[] expected, T def)
        {
            T[] array = (T[])Array.CreateInstance(typeof(T), length);
            ICollection collection = bitArray;
            collection.CopyTo(array, index);
            for (int i = 0; i < index; i++)
            {
                Assert.Equal(def, array[i]);
            }
            for (int i = 0; i < expected.Length; i++)
            {
                Assert.Equal(expected[i], array[i + index]);
            }
            for (int i = index + expected.Length; i < array.Length; i++)
            {
                Assert.Equal(def, array[i]);
            }
        }

        [Fact]
        public static void CopyTo_Type_Invalid()
        {
            ICollection bitArray = new BitArray(10);
            Assert.Throws<ArgumentNullException>("array", () => bitArray.CopyTo(null, 0));
            Assert.Throws<ArgumentException>(() => bitArray.CopyTo(new long[10], 0));
            Assert.Throws<ArgumentException>(() => bitArray.CopyTo(new int[10, 10], 0));
        }

        [Theory]
        [InlineData(default(bool), 1, 0, 0)]
        [InlineData(default(bool), 1, 1, 1)]
        [InlineData(default(bool), BitsPerByte, BitsPerByte - 1, 0)]
        [InlineData(default(bool), BitsPerByte, BitsPerByte, 1)]
        [InlineData(default(bool), BitsPerInt32, BitsPerInt32 - 1, 0)]
        [InlineData(default(bool), BitsPerInt32, BitsPerInt32, 1)]
        [InlineData(default(byte), BitsPerByte, 0, 0)]
        [InlineData(default(byte), BitsPerByte, 1, 1)]
        [InlineData(default(byte), BitsPerByte * 4, 4 - 1, 0)]
        [InlineData(default(byte), BitsPerByte * 4, 4, 1)]
        [InlineData(default(int), BitsPerInt32, 0, 0)]
        [InlineData(default(int), BitsPerInt32, 1, 1)]
        [InlineData(default(int), BitsPerInt32 * 4, 4 - 1, 0)]
        [InlineData(default(int), BitsPerInt32 * 4, 4, 1)]
        public static void CopyTo_Size_Invalid<T>(T def, int bits, int arraySize, int index)
        {
            ICollection bitArray = new BitArray(bits);
            T[] array = (T[])Array.CreateInstance(typeof(T), arraySize);
            Assert.Throws<ArgumentOutOfRangeException>("index", () => bitArray.CopyTo(array, -1));
            Assert.Throws<ArgumentException>(def is int ? string.Empty : null, () => bitArray.CopyTo(array, index));
        }

        [Fact]
        public static void SyncRoot()
        {
            ICollection bitArray = new BitArray(10);
            Assert.Same(bitArray.SyncRoot, bitArray.SyncRoot);
            Assert.NotSame(bitArray.SyncRoot, ((ICollection)new BitArray(10)).SyncRoot);
        }
    }
}
