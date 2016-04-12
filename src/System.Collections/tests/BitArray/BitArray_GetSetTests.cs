// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Collections.Tests
{
    public class BitArray_GetSetTests
    {
        [Theory]
        [InlineData(new bool[] { true })]
        [InlineData(new bool[] { false })]
        [InlineData(new bool[] { true, false, true, true, false, true })]
        public static void Get_Set(bool[] newValues)
        {
            BitArray bitArray = new BitArray(newValues.Length, false);
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
        [InlineData(6, true)]
        [InlineData(6, false)]
        [InlineData(0x1000F, true)]
        public static void SetAll(int size, bool defaultValue)
        {
            BitArray bitArray = new BitArray(6, defaultValue);
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
            
        [Theory]
        [InlineData(new bool[0])]
        [InlineData(new bool[] { true, false, true, false, true, false, true, false, true, false })]
        public static void GetEnumerator(bool[] values)
        {
            BitArray bitArray = new BitArray(values);
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

        [Fact]
        public static void GetEnumerator_Invalid()
        {
            BitArray bitArray = new BitArray(10, true);
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
            enumerator.MoveNext();
            bitArray[0] = false;
            Assert.True((bool)enumerator.Current);
            Assert.Throws<InvalidOperationException>(() => enumerator.MoveNext());
            Assert.Throws<InvalidOperationException>(() => enumerator.Reset());
        }

        [Theory]
        [InlineData(16, 48)]
        [InlineData(48, 24)]
        [InlineData(16384, 256)]
        [InlineData(48, 0)]
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
        }

        [Fact]
        public static void Length_Set_InvalidLength_ThrowsArgumentOutOfRangeException()
        {
            BitArray bitArray = new BitArray(1);
            Assert.Throws<ArgumentOutOfRangeException>(() => bitArray.Length = -11);
        }
    }
}
