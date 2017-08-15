// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Tests
{
    public class BitConverterArray : BitConverterBase
    {
        public override void ConvertFromBool(bool boolean, int expected)
        {
            Assert.Equal(expected, BitConverter.GetBytes(boolean)[0]);
        }

        public override void ConvertFromShort(short num, byte[] byteArr)
        {
            Assert.Equal(byteArr, BitConverter.GetBytes(num));
        }

        public override void ConvertFromChar(char character, byte[] byteArr)
        {
            Assert.Equal(byteArr, BitConverter.GetBytes(character));
        }

        public override void ConvertFromInt(int num, byte[] byteArr)
        {
            Assert.Equal(byteArr, BitConverter.GetBytes(num));
        }

        public override void ConvertFromLong(long num, byte[] byteArr)
        {
            Assert.Equal(byteArr, BitConverter.GetBytes(num));
        }

        public override void ConvertFromUShort(ushort num, byte[] byteArr)
        {
            Assert.Equal(byteArr, BitConverter.GetBytes(num));
        }

        public override void ConvertFromUInt(uint num, byte[] byteArr)
        {
            Assert.Equal(byteArr, BitConverter.GetBytes(num));
        }

        public override void ConvertFromULong(ulong num, byte[] byteArr)
        {
            Assert.Equal(byteArr, BitConverter.GetBytes(num));
        }

        public override void ConvertFromFloat(float num, byte[] byteArr)
        {
            Assert.Equal(byteArr, BitConverter.GetBytes(num));
        }

        public override void ConvertFromDouble(double num, byte[] byteArr)
        {
            Assert.Equal(byteArr, BitConverter.GetBytes(num));
        }

        public override void ToChar(int index, char expected)
        {
            Assert.Equal(expected, BitConverter.ToChar(s_toCharByteArray, index));
        }

        public override void ToInt16(int index, short expected)
        {
            Assert.Equal(expected, BitConverter.ToInt16(s_toInt16ByteArray, index));
        }

        public override void ToInt32(int expected, byte[] byteArray)
        {
            Assert.Equal(expected, BitConverter.ToInt32(byteArray, 0));
        }

        public override void ToInt64(int index, long expected)
        {
            Assert.Equal(expected, BitConverter.ToInt64(s_toInt64ByteArray, index));
        }

        public override void ToUInt16(int index, ushort expected)
        {
            Assert.Equal(expected, BitConverter.ToUInt16(s_toUInt16ByteArray, index));
        }

        public override void ToUInt32(int index, uint expected)
        {
            Assert.Equal(expected, BitConverter.ToUInt32(s_toUInt32ByteArray, index));
        }

        public override void ToUInt64(int index, ulong expected)
        {
            Assert.Equal(expected, BitConverter.ToUInt64(s_toUInt64ByteArray, index));
        }

        public override void ToSingle(int index, float expected)
        {
            Assert.Equal(expected, BitConverter.ToSingle(s_toSingleByteArray, index));
        }

        public override void ToDouble(int index, double expected)
        {
            Assert.Equal(expected, BitConverter.ToDouble(s_toDoubleByteArray, index));
        }

        public override void ToBoolean(int index, bool expected)
        {
            Assert.Equal(expected, BitConverter.ToBoolean(s_toBooleanByteArray, index));
        }
    }
}
