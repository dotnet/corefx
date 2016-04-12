// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Collections.Tests
{
    public class BitArray_CtorTests
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
            Ctor_BitArray(bitArray);
        }

        [Fact]
        public static void Ctor_Int_NegativeLength_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>("length", () => new BitArray(-1));
        }

        [Theory]
        [InlineData(0, true)]
        [InlineData(0, false)]
        [InlineData(40, true)]
        [InlineData(40, true)]
        public static void Ctor_Int_Bool(int length, bool defaultValue)
        {
            BitArray bitArray = new BitArray(length, defaultValue);
            Assert.Equal(length, bitArray.Length);
            for (int i = 0; i < bitArray.Length; i++)
            {
                Assert.Equal(defaultValue, bitArray[i]);
                Assert.Equal(defaultValue, bitArray.Get(i));
            }
            Ctor_BitArray(bitArray);
        }

        [Fact]
        public static void Ctor_Int_Bool_NegativeLength_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>("length", () => new BitArray(-1));
        }

        [Theory]
        [InlineData(new bool[0])]
        [InlineData(new bool[] { false, false, true, false, false, false, true, false, false, true })]
        public static void Ctor_BoolArray(bool[] values)
        {
            BitArray bitArray = new BitArray(values);
            Assert.Equal(values.Length, bitArray.Length);
            for (int i = 0; i < bitArray.Length; i++)
            {
                Assert.Equal(values[i], bitArray[i]);
                Assert.Equal(values[i], bitArray.Get(i));
            }
            Ctor_BitArray(bitArray);
        }

        public static void Ctor_BitArray(BitArray bits)
        {
            BitArray bitArray = new BitArray(bits);
            Assert.Equal(bits.Length, bitArray.Length);
            for (int i = 0; i < bitArray.Length; i++)
            {
                Assert.Equal(bits[i], bitArray[i]);
                Assert.Equal(bits[i], bitArray.Get(i));
            }
        }

        [Fact]
        public static void BitArray_CtorBitArrayTest()
        {
            // Creating bit array with 10 values that have all 1s
            BitArray ba1 = null;
            BitArray bits = null;
            int[] values = null;
            int i;

            values = new int[10];
            for (i = 0; i < 10; i++)
                values[i] = unchecked((int)0xffffffff);

            bits = new BitArray(values);
            ba1 = new BitArray(bits);

            Assert.Equal(ba1.Length, 320); //"Err_7! ba1.Length should be equal to 320"

            for (i = 0; i < 320; i++)
            {
                Assert.True(ba1.Get(i)); //"Err_8! array should contain all 1s"
            }

            // Creating BitArray with 10 values that have all 0s
            values = new int[10];
            for (i = 0; i < 10; i++)
                values[i] = 0;

            bits = new BitArray(values);
            ba1 = new BitArray(bits);

            Assert.Equal(ba1.Length, 320); //"Err_9! ba1.Length should be equal to 320"

            for (i = 0; i < 320; i++)
            {
                Assert.False(ba1.Get(i)); //"Err_10! array should contain all 0s"
            }


            // Creating BitArray with 10 values that have zeros and ones
            values = new int[10];
            for (i = 0; i < 10; i++)
                values[i] = unchecked((int)0xaaaaaaaa);

            bits = new BitArray(values);
            ba1 = new BitArray(bits);

            Assert.Equal(ba1.Length, 320); //"Err_11! ba1.Length should be equal to 320"

            for (i = 0; i < 320; i++)
            {
                if (i % 2 == 1)
                    Assert.True(ba1.Get(i)); //"Err_12! index should contain 1"
                else
                    Assert.False(ba1.Get(i)); //"Err_13! index should contain 0"
            }
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
    }
}
