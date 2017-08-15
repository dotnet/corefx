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

        public override void ToChar(int index, char character)
        {
            byte[] byteArray = { 32, 0, 0, 42, 0, 65, 0, 125, 0, 197, 0, 168, 3, 41, 4, 172, 32 };
            Assert.Equal(character, BitConverter.ToChar(byteArray, index));
        }

        public override void ToInt16(int index, short expected)
        {
            byte[] byteArray = { 15, 0, 0, 128, 16, 39, 240, 216, 241, 255, 127 };
            Assert.Equal(expected, BitConverter.ToInt16(byteArray, index));
        }

        public override void ToInt32(int expected, byte[] byteArray)
        {
            Assert.Equal(expected, BitConverter.ToInt32(byteArray));
        }

        public override void ToInt64(int index, long expected)
        {
            byte[] byteArray =
                { 0x00, 0x36, 0x65, 0xC4, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x80, 0x00, 0xCA, 0x9A,
            0x3B, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0x01, 0x00, 0x00, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0xFF, 0xFF, 0x7F, 0x56, 0x55, 0x55, 0x55, 0x55, 0x55, 0xFF, 0xFF, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0x00,
            0x00, 0x64, 0xA7, 0xB3, 0xB6, 0xE0, 0x0D, 0x00, 0x00, 0x9C, 0x58, 0x4C, 0x49, 0x1F, 0xF2 };
            Assert.Equal(expected, BitConverter.ToInt32(byteArray, index));
        }
    }
}
