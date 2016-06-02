// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace System.Collections.Tests
{
    public static class BitArray_CtorTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(40)]
        [InlineData(200)]
        [InlineData(65551)]
        public static void Ctor_Int(int length)
        {
            BitArray bitArray = new BitArray(length);
            Assert.Equal(length, bitArray.Length);
            for (int i = 0; i < bitArray.Length; i++)
            {
                Assert.False(bitArray[i]);
                Assert.False(bitArray.Get(i));
            }
            ICollection collection = bitArray;
            Assert.Equal(length, collection.Count);
            Assert.False(collection.IsSynchronized);
        }

        [Theory]
        [InlineData(0, true)]
        [InlineData(0, false)]
        [InlineData(40, true)]
        [InlineData(40, false)]
        [InlineData(200, true)]
        [InlineData(200, false)]
        [InlineData(65551, true)]
        [InlineData(65551, false)]
        public static void Ctor_Int_Bool(int length, bool defaultValue)
        {
            BitArray bitArray = new BitArray(length, defaultValue);
            Assert.Equal(length, bitArray.Length);
            for (int i = 0; i < bitArray.Length; i++)
            {
                Assert.Equal(defaultValue, bitArray[i]);
                Assert.Equal(defaultValue, bitArray.Get(i));
            }
            ICollection collection = bitArray;
            Assert.Equal(length, collection.Count);
            Assert.False(collection.IsSynchronized);
        }

        [Fact]
        public static void Ctor_Int_NegativeLength_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>("length", () => new BitArray(-1));
            Assert.Throws<ArgumentOutOfRangeException>("length", () => new BitArray(-1, false));
        }

        public static IEnumerable<object[]> Ctor_BoolArray_TestData()
        {
            yield return new object[] { new bool[0] };
            yield return new object[] { new bool[] { false } };
            yield return new object[] { new bool[] { true } };
            yield return new object[] { new bool[] { false, false, true, false, false, false, true, false, false, true } };
            yield return new object[] { Enumerable.Repeat(true, 32).ToArray() };
            yield return new object[] { Enumerable.Repeat(false, 32).ToArray() };
            yield return new object[] { Enumerable.Range(0, 32).Select(x => x % 2 == 0).ToArray() };
            yield return new object[] { Enumerable.Repeat(true, 64).ToArray() };
            yield return new object[] { Enumerable.Repeat(false, 64).ToArray() };
            yield return new object[] { Enumerable.Range(0, 64).Select(x => x % 2 == 0).ToArray() };
        }

        [Theory]
        [MemberData(nameof(Ctor_BoolArray_TestData))]
        public static void Ctor_BoolArray(bool[] values)
        {
            BitArray bitArray = new BitArray(values);
            Assert.Equal(values.Length, bitArray.Length);
            for (int i = 0; i < bitArray.Length; i++)
            {
                Assert.Equal(values[i], bitArray[i]);
                Assert.Equal(values[i], bitArray.Get(i));
            }
            ICollection collection = bitArray;
            Assert.Equal(values.Length, collection.Count);
            Assert.False(collection.IsSynchronized);
        }

        public static IEnumerable<object[]> Ctor_BitArray_TestData()
        {
            foreach (int size in new[] { 0, 1, 8, 32, 64 })
            {
                yield return new object[] { "length", new BitArray(size) };
                yield return new object[] { "length|default(true)", new BitArray(size, true) };
                yield return new object[] { "length|default(false)", new BitArray(size, false) };
                yield return new object[] { "bool[](all)", new BitArray(Enumerable.Repeat(true, size).ToArray()) };
                yield return new object[] { "bool[](none)", new BitArray(Enumerable.Repeat(false, size).ToArray()) };
                yield return new object[] { "bool[](alternating)", new BitArray(Enumerable.Range(0, size).Select(x => x % 2 == 0).ToArray()) };
                yield return new object[] { "byte[](all)", new BitArray(Enumerable.Repeat((byte)255, size / 8).ToArray()) };
                yield return new object[] { "byte[](none)", new BitArray(Enumerable.Repeat((byte)0, size / 8).ToArray()) };
                yield return new object[] { "byte[](alternating)", new BitArray(Enumerable.Repeat((byte)0xaa, size / 8).ToArray()) };
                yield return new object[] { "int[](all)", new BitArray(Enumerable.Repeat(unchecked((int)0xffffffff), size / 32).ToArray()) };
                yield return new object[] { "int[](none)", new BitArray(Enumerable.Repeat(0, size / 32).ToArray()) };
                yield return new object[] { "int[](alternating)", new BitArray(Enumerable.Repeat(unchecked((int)0xaaaaaaaa), size / 32).ToArray()) };
            }
        }

        [Theory]
        [MemberData(nameof(Ctor_BitArray_TestData))]
        public static void Ctor_BitArray(string label, BitArray bits)
        {
            BitArray bitArray = new BitArray(bits);
            Assert.Equal(bits.Length, bitArray.Length);
            for (int i = 0; i < bitArray.Length; i++)
            {
                Assert.Equal(bits[i], bitArray[i]);
                Assert.Equal(bits[i], bitArray.Get(i));
            }
            ICollection collection = bitArray;
            Assert.Equal(bits.Length, collection.Count);
            Assert.False(collection.IsSynchronized);
        }

        public static IEnumerable<object[]> Ctor_IntArray_TestData()
        {
            yield return new object[] { new int[0], new bool[0] };
            yield return new object[] { Enumerable.Repeat(unchecked((int)0xffffffff), 10).ToArray(), Enumerable.Repeat(true, 320).ToArray() };
            yield return new object[] { Enumerable.Repeat(0, 10).ToArray(), Enumerable.Repeat(false, 320).ToArray() };
            yield return new object[] { Enumerable.Repeat(unchecked((int)0xaaaaaaaa), 10).ToArray(), Enumerable.Range(0, 320).Select(i => i % 2 == 1).ToArray() };
        }

        [Theory]
        [MemberData(nameof(Ctor_IntArray_TestData))]
        public static void Ctor_IntArray(int[] array, bool[] expected)
        {
            BitArray bitArray = new BitArray(array);
            Assert.Equal(expected.Length, bitArray.Length);
            for (int i = 0; i < expected.Length; i++)
            {
                Assert.Equal(expected[i], bitArray[i]);
                Assert.Equal(expected[i], bitArray.Get(i));
            }
            ICollection collection = bitArray;
            Assert.Equal(expected.Length, collection.Count);
            Assert.False(collection.IsSynchronized);
        }

        [Fact]
        public static void Ctor_BitArray_Null_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("bits", () => new BitArray((BitArray)null));
        }

        [Fact]
        public static void Ctor_BoolArray_Null_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("values", () => new BitArray((bool[])null));
        }

        [Fact]
        public static void Ctor_IntArray_Null_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("values", () => new BitArray((int[])null));
        }

        public static IEnumerable<object[]> Ctor_ByteArray_TestData()
        {
            yield return new object[] { new byte[0], new bool[0] };
            yield return new object[] { new byte[] { 255, 255 }, Enumerable.Repeat(true, 16).ToArray() };
            yield return new object[] { new byte[] { 0, 0 }, Enumerable.Repeat(false, 16).ToArray() };
            yield return new object[] { new byte[] { 255, 255, 255, 255 }, Enumerable.Repeat(true, 32).ToArray() };
            yield return new object[] { new byte[] { 0, 0, 0, 0 }, Enumerable.Repeat(false, 32).ToArray() };
            yield return new object[] { Enumerable.Repeat((byte)0xaa, 4).ToArray(), Enumerable.Range(0, 32).Select(i => i % 2 == 1).ToArray() };
            yield return new object[] { new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 }, Enumerable.Repeat(true, 64).ToArray() };
            yield return new object[] { new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }, Enumerable.Repeat(false, 64).ToArray() };
            yield return new object[] { Enumerable.Repeat((byte)0xaa, 8).ToArray(), Enumerable.Range(0, 64).Select(i => i % 2 == 1).ToArray() };
        }

        [Theory]
        [MemberData(nameof(Ctor_ByteArray_TestData))]
        public static void Ctor_ByteArray(byte[] bytes, bool[] expected)
        {
            BitArray bitArray = new BitArray(bytes);
            Assert.Equal(expected.Length, bitArray.Length);
            for (int i = 0; i < bitArray.Length; i++)
            {
                Assert.Equal(expected[i], bitArray[i]);
                Assert.Equal(expected[i], bitArray.Get(i));
            }
            ICollection collection = bitArray;
            Assert.Equal(expected.Length, collection.Count);
            Assert.False(collection.IsSynchronized);
        }

        [Fact]
        public static void Ctor_ByteArray_Null_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("bytes", () => new BitArray((byte[])null));
        }
    }
}
