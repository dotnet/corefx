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
            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => bitArray.Get(-1));
            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => bitArray.Get(bitArray.Length));

            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => bitArray[-1]);
            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => bitArray[bitArray.Length]);
        }

        [Fact]
        public static void Set_InvalidIndex_ThrowsArgumentOutOfRangeException()
        {
            BitArray bitArray = new BitArray(4);
            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => bitArray.Set(-1, true));
            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => bitArray.Set(bitArray.Length, true));

            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => bitArray[-1] = true);
            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => bitArray[bitArray.Length] = true);
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
            while (enumerator.MoveNext())
                ;
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

        [Fact]
        public static void GetEnumerator_CloneEnumerator_ReturnsUniqueEnumerator()
        {
            BitArray bitArray = new BitArray(1);
            IEnumerator enumerator = bitArray.GetEnumerator();
            ICloneable cloneableEnumerator = enumerator as ICloneable;
            Assert.NotNull(cloneableEnumerator);

            IEnumerator clonedEnumerator = (IEnumerator)cloneableEnumerator.Clone();
            Assert.NotSame(enumerator, clonedEnumerator);

            Assert.True(clonedEnumerator.MoveNext());
            Assert.False(clonedEnumerator.MoveNext());

            Assert.True(enumerator.MoveNext());
            Assert.False(enumerator.MoveNext());
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
            BitArray bitArray = new BitArray(originalSize, true)
            {
                Length = newSize
            };
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
            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => bitArray[newSize]);
            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => bitArray.Get(newSize));

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

                foreach (Tuple<int, int> d in new[] { Tuple.Create(bitArraySize, 0),
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

            foreach (int bitArraySize in new[] { BitsPerInt32 - 1, BitsPerInt32 * 2 - 1 })
            {
                BitArray allTrue = new BitArray(Enumerable.Repeat(true, bitArraySize).ToArray());
                BitArray allFalse = new BitArray(Enumerable.Repeat(false, bitArraySize).ToArray());
                BitArray alternating = new BitArray(Enumerable.Range(0, bitArraySize).Select(i => i % 2 == 1).ToArray());

                foreach (Tuple<int, int> d in new[] { Tuple.Create(bitArraySize, 0),
                    Tuple.Create(bitArraySize * 2 + 1, 0),
                    Tuple.Create(bitArraySize * 2 + 1, bitArraySize + 1),
                    Tuple.Create(bitArraySize * 2 + 1, bitArraySize / 2 + 1)})
                {
                    int arraySize = d.Item1;
                    int index = d.Item2;

                    if (bitArraySize >= BitsPerInt32)
                    {
                        yield return new object[] { allTrue, (arraySize - 1) / BitsPerInt32 + 1, index / BitsPerInt32, Enumerable.Repeat(unchecked((int)0xffffffff), bitArraySize / BitsPerInt32).Concat(new[] { unchecked((int)(0xffffffffu >> 1)) }).ToArray(), default(int) };
                        yield return new object[] { allFalse, (arraySize - 1) / BitsPerInt32 + 1, index / BitsPerInt32, Enumerable.Repeat(0x00000000, bitArraySize / BitsPerInt32 + 1).ToArray(), default(int) };
                        yield return new object[] { alternating, (arraySize - 1) / BitsPerInt32 + 1, index / BitsPerInt32, Enumerable.Repeat(unchecked((int)0xaaaaaaaa), bitArraySize / BitsPerInt32).Concat(new[] { unchecked((int)(0xaaaaaaaau >> 2)) }).ToArray(), default(int) };
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
            AssertExtensions.Throws<ArgumentNullException>("array", () => bitArray.CopyTo(null, 0));
            AssertExtensions.Throws<ArgumentException>("array", null, () => bitArray.CopyTo(new long[10], 0));
            AssertExtensions.Throws<ArgumentException>("array", null, () => bitArray.CopyTo(new int[10, 10], 0));
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
            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => bitArray.CopyTo(array, -1));
            if (def is int)
            {
                AssertExtensions.Throws<ArgumentException>("destinationArray", string.Empty, () => bitArray.CopyTo(array, index));
            }
            else
            {
                AssertExtensions.Throws<ArgumentException>(null, () => bitArray.CopyTo(array, index));
            }
        }

        [Fact]
        public static void SyncRoot()
        {
            ICollection bitArray = new BitArray(10);
            Assert.Same(bitArray.SyncRoot, bitArray.SyncRoot);
            Assert.NotSame(bitArray.SyncRoot, ((ICollection)new BitArray(10)).SyncRoot);
        }

        public static IEnumerable<object> CopyTo_Hidden_Data()
        {
            yield return new object[] { "ZeroLength", new BitArray(0) };
            yield return new object[] { "Constructor", new BitArray(BitsPerInt32 / 2 - 3, true) };
            yield return new object[] { "Not", new BitArray(BitsPerInt32 / 2 - 3, false).Not() };
            BitArray setAll = new BitArray(BitsPerInt32 / 2 - 3, false);
            setAll.SetAll(true);
            yield return new object[] { "SetAll", setAll };
            BitArray lengthShort = new BitArray(BitsPerInt32, true)
            {
                Length = BitsPerInt32 / 2 - 3
            };
            yield return new object[] { "Length-Short", lengthShort };
            BitArray lengthLong = new BitArray(2 * BitsPerInt32, true)
            {
                Length = BitsPerInt32 - 3
            };
            yield return new object[] { "Length-Long < 32", lengthLong };
            BitArray lengthLong2 = new BitArray(2 * BitsPerInt32, true)
            {
                Length = BitsPerInt32 + 3
            };
            yield return new object[] { "Length-Long > 32", lengthLong2 };
            // alligned test cases
            yield return new object[] { "Aligned-Constructor", new BitArray(BitsPerInt32, true) };
            yield return new object[] { "Aligned-Not", new BitArray(BitsPerInt32, false).Not() };
            BitArray alignedSetAll = new BitArray(BitsPerInt32, false);
            alignedSetAll.SetAll(true);
            yield return new object[] { "Aligned-SetAll", alignedSetAll };
            BitArray alignedLengthLong = new BitArray(2 * BitsPerInt32, true);
            yield return new object[] { "Aligned-Length-Long", alignedLengthLong };
        }

        [Theory]
        [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework, "Desktop Framework hasn't received the fix for #9838 yet.")]
        [MemberData(nameof(CopyTo_Hidden_Data))]
        public static void CopyTo_Int_Hidden(string label, BitArray bits)
        {
            int allBitsSet = unchecked((int)0xffffffff); // 32 bits set to 1 = -1
            int fullInts = bits.Length / BitsPerInt32;
            int remainder = bits.Length % BitsPerInt32;
            int arrayLength = fullInts + (remainder > 0 ? 1 : 0);

            int[] data = new int[arrayLength];
            ((ICollection)bits).CopyTo(data, 0);

            Assert.All(data.Take(fullInts), d => Assert.Equal(allBitsSet, d));

            if (remainder > 0)
            {
                Assert.Equal((1 << remainder) - 1, data[fullInts]);
            }
        }

        [Theory]
        [SkipOnTargetFramework(~TargetFrameworkMonikers.NetFramework, "Desktop Framework hasn't received the fix for #9838 yet.")]
        [MemberData(nameof(CopyTo_Hidden_Data))]
        public static void CopyTo_Int_Hidden_Desktop(string label, BitArray bits)
        {
            int allBitsSet = unchecked((int)0xffffffff); // 32 bits set to 1 = -1
            int fullInts = bits.Length / BitsPerInt32;
            int remainder = bits.Length % BitsPerInt32;
            int arrayLength = fullInts + (remainder > 0 ? 1 : 0);

            int[] data = new int[arrayLength];
            ((ICollection)bits).CopyTo(data, 0);

            Assert.All(data, d => Assert.Equal(allBitsSet, d));

        }

        [Theory]
        [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework, "Desktop Framework hasn't received the fix for #9838 yet.")]
        [MemberData(nameof(CopyTo_Hidden_Data))]
        public static void CopyTo_Byte_Hidden(string label, BitArray bits)
        {
            byte allBitsSet = (1 << BitsPerByte) - 1; // 8 bits set to 1 = 255

            int fullBytes = bits.Length / BitsPerByte;
            int remainder = bits.Length % BitsPerByte;
            int arrayLength = fullBytes + (remainder > 0 ? 1 : 0);

            byte[] data = new byte[arrayLength];
            ((ICollection)bits).CopyTo(data, 0);

            Assert.All(data.Take(fullBytes), d => Assert.Equal(allBitsSet, d));

            if (remainder > 0)
            {
                Assert.Equal((byte)((1 << remainder) - 1), data[fullBytes]);
            }
        }

        [Theory]
        [SkipOnTargetFramework(~TargetFrameworkMonikers.NetFramework, "Desktop Framework hasn't received the fix for #9838 yet.")]
        [MemberData(nameof(CopyTo_Hidden_Data))]
        public static void CopyTo_Byte_Hidden_Desktop(string label, BitArray bits)
        {
            byte allBitsSet = (1 << BitsPerByte) - 1; // 8 bits set to 1 = 255

            int fullBytes = bits.Length / BitsPerByte;
            int remainder = bits.Length % BitsPerByte;
            int arrayLength = fullBytes + (remainder > 0 ? 1 : 0);

            byte[] data = new byte[arrayLength];
            ((ICollection)bits).CopyTo(data, 0);

            Assert.All(data, d => Assert.Equal(allBitsSet, d));

        }
    }
}
