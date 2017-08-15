// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Tests
{
    public abstract class BitConverterBase
    {

        [Theory]
        [InlineData(true, 1)]
        [InlineData(false, 0)]
        public abstract void ConvertFromBool(bool boolean, int expected);

        [Theory]
        [InlineData((short)0, new byte[] { 0x00, 0x00 })]
        [InlineData((short)-15, new byte[] { 0xF1, 0xFF })]
        [InlineData((short)15, new byte[] { 0x0F, 0x00 })]
        [InlineData((short)10000, new byte[] { 0x10, 0x27 })]
        [InlineData((short)-10000, new byte[] { 0xF0, 0xD8 })]
        [InlineData(short.MinValue, new byte[] { 0x00, 0x80 })]
        [InlineData(short.MaxValue, new byte[] { 0xFF, 0x7F })]
        public abstract void ConvertFromShort(short num, byte[] byteArr);

        [Theory]
        [InlineData('A', new byte[] { 0x41, 0x00 })]
        [InlineData('*', new byte[] { 0x2A, 0x00 })]
        [InlineData('3', new byte[] { 0x33, 0x00 })]
        [InlineData('[', new byte[] { 0x5B, 0x00 })]
        [InlineData('a', new byte[] { 0x61, 0x00 })]
        [InlineData('{', new byte[] { 0x7B, 0x00 })]
        [InlineData('\0', new byte[] { 0x00, 0x00 })]
        [InlineData(' ', new byte[] { 0x20, 0x00 })]
        [InlineData('\u263a', new byte[] { 0x3A, 0x26 })]
        public abstract void ConvertFromChar(char character, byte[] byteArr);

        [Theory]
        [InlineData(0, new byte[] { 0x00, 0x00, 0x00, 0x00 })]
        [InlineData(15, new byte[] { 0x0F, 0x00, 0x00, 0x00 })]
        [InlineData(-15, new byte[] { 0xF1, 0xFF, 0xFF, 0xFF })]
        [InlineData(1048576, new byte[] { 0x00, 0x00, 0x10, 0x00 })]
        [InlineData(-1048576, new byte[] { 0x00, 0x00, 0xF0, 0xFF })]
        [InlineData(1000000000, new byte[] { 0x00, 0xCA, 0x9A, 0x3B })]
        [InlineData(-1000000000, new byte[] { 0x00, 0x36, 0x65, 0xC4 })]
        [InlineData(int.MaxValue, new byte[] { 0xFF, 0xFF, 0xFF, 0x7F })]
        [InlineData(int.MinValue, new byte[] { 0x00, 0x00, 0x00, 0x80 })]
        public abstract void ConvertFromInt(int num, byte[] byteArr);

        [Theory]
        [InlineData(0, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 })]
        [InlineData(16777215, new byte[] { 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00 })]
        [InlineData(-16777215, new byte[] { 0x01, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF })]
        [InlineData(1000000000, new byte[] { 0x00, 0xCA, 0x9A, 0x3B, 0x00, 0x00, 0x00, 0x00 })]
        [InlineData(-1000000000, new byte[] { 0x00, 0x36, 0x65, 0xC4, 0xFF, 0xFF, 0xFF, 0xFF })]
        [InlineData(4294967296, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00 })]
        [InlineData(-4294967296, new byte[] { 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF })]
        [InlineData(187649984473770, new byte[] { 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0x00, 0x00 })]
        [InlineData(-187649984473770, new byte[] { 0x56, 0x55, 0x55, 0x55, 0x55, 0x55, 0xFF, 0xFF })]
        [InlineData(1000000000000000000, new byte[] { 0x00, 0x00, 0x64, 0xA7, 0xB3, 0xB6, 0xE0, 0x0D })]
        [InlineData(-1000000000000000000, new byte[] { 0x00, 0x00, 0x9C, 0x58, 0x4C, 0x49, 0x1F, 0xF2 })]
        [InlineData(long.MinValue, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x80 })]
        [InlineData(long.MaxValue, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x7F })]
        public abstract void ConvertFromLong(long num, byte[] byteArr);

        [Theory]
        [InlineData((ushort)0, new byte[] { 0x00, 0x00 })]
        [InlineData((ushort)15, new byte[] { 0x0F, 0x00 })]
        [InlineData((ushort)1023, new byte[] { 0xFF, 0x03 })]
        [InlineData((ushort)10000, new byte[] { 0x10, 0x27 })]
        [InlineData((ushort)short.MaxValue, new byte[] { 0xFF, 0x7F })]
        [InlineData(ushort.MaxValue, new byte[] { 0xFF, 0xFF })]
        public abstract void ConvertFromUShort(ushort num, byte[] byteArr);

        [Theory]
        [InlineData((uint)0, new byte[] { 0x00, 0x00, 0x00, 0x00 })]
        [InlineData((uint)15, new byte[] { 0x0F, 0x00, 0x00, 0x00 })]
        [InlineData((uint)1023, new byte[] { 0xFF, 0x03, 0x00, 0x00 })]
        [InlineData((uint)1048576, new byte[] { 0x00, 0x00, 0x10, 0x00 })]
        [InlineData((uint)1000000000, new byte[] { 0x00, 0xCA, 0x9A, 0x3B })]
        [InlineData(int.MaxValue, new byte[] { 0xFF, 0xFF, 0xFF, 0x7F })]
        [InlineData(uint.MaxValue, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF })]
        public abstract void ConvertFromUInt(uint num, byte[] byteArr);

        [Theory]
        [InlineData((ulong)0xFFFFFF, new byte[] { 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00 })]
        [InlineData((ulong)1000000000, new byte[] { 0x00, 0xCA, 0x9A, 0x3B, 0x00, 0x00, 0x00, 0x00 })]
        [InlineData((ulong)0x100000000, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00 })]
        [InlineData((ulong)0xAAAAAAAAAAAA, new byte[] { 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0x00, 0x00 })]
        [InlineData((ulong)1000000000000000000, new byte[] { 0x00, 0x00, 0x64, 0xA7, 0xB3, 0xB6, 0xE0, 0x0D })]
        [InlineData(10000000000000000000, new byte[] { 0x00, 0x00, 0xE8, 0x89, 0x04, 0x23, 0xC7, 0x8A })]
        [InlineData((ulong)0, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 })]
        [InlineData(long.MaxValue, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x7F })]
        [InlineData(ulong.MaxValue, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF })]
        public abstract void ConvertFromULong(ulong num, byte[] byteArr);

        [Theory]
        [InlineData(0.0F, new byte[] { 0x00, 0x00, 0x00, 0x00 })]
        [InlineData(1.0F, new byte[] { 0x00, 0x00, 0x80, 0x3F })]
        [InlineData(15.0F, new byte[] { 0x00, 0x00, 0x70, 0x41 })]
        [InlineData(65535.0F, new byte[] { 0x00, 0xFF, 0x7F, 0x47 })]
        [InlineData(0.00390625F, new byte[] { 0x00, 0x00, 0x80, 0x3B })]
        [InlineData(0.00000000023283064365386962890625F, new byte[] { 0x00, 0x00, 0x80, 0x2F })]
        [InlineData(1.2345E-35F, new byte[] { 0x49, 0x46, 0x83, 0x05 })]
        [InlineData(1.2345671F, new byte[] { 0x4B, 0x06, 0x9E, 0x3F })]
        [InlineData(1.2345673F, new byte[] { 0x4D, 0x06, 0x9E, 0x3F })]
        [InlineData(1.2345677F, new byte[] { 0x50, 0x06, 0x9E, 0x3F })]
        [InlineData(1.23456789E+35F, new byte[] { 0x1E, 0x37, 0xBE, 0x79 })]
        [InlineData(float.MinValue, new byte[] { 0xFF, 0xFF, 0x7F, 0xFF })]
        [InlineData(float.MaxValue, new byte[] { 0xFF, 0xFF, 0x7F, 0x7F })]
        [InlineData(float.Epsilon, new byte[] { 0x01, 0x00, 0x00, 0x00 })]
        [InlineData(float.NaN, new byte[] { 0x00, 0x00, 0xC0, 0xFF })]
        [InlineData(float.NegativeInfinity, new byte[] { 0x00, 0x00, 0x80, 0xFF })]
        [InlineData(float.PositiveInfinity, new byte[] { 0x00, 0x00, 0x80, 0x7F })]
        public abstract void ConvertFromFloat(float num, byte[] byteArr);

        [Theory]
        [InlineData(0.0, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 })]
        [InlineData(1.0, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF0, 0x3F })]
        [InlineData(255.0, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0xE0, 0x6F, 0x40 })]
        [InlineData(4294967295.0, new byte[] { 0x00, 0x00, 0xE0, 0xFF, 0xFF, 0xFF, 0xEF, 0x41 })]
        [InlineData(0.00390625, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x70, 0x3F })]
        [InlineData(0.00000000023283064365386962890625, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF0, 0x3D })]
        [InlineData(1.23456789012345E-300, new byte[] { 0xDF, 0x88, 0x1E, 0x1C, 0xFE, 0x74, 0xAA, 0x01 })]
        [InlineData(1.2345678901234565, new byte[] { 0xFA, 0x59, 0x8C, 0x42, 0xCA, 0xC0, 0xF3, 0x3F })]
        [InlineData(1.2345678901234567, new byte[] { 0xFB, 0x59, 0x8C, 0x42, 0xCA, 0xC0, 0xF3, 0x3F })]
        [InlineData(1.2345678901234569, new byte[] { 0xFC, 0x59, 0x8C, 0x42, 0xCA, 0xC0, 0xF3, 0x3F })]
        [InlineData(1.23456789012345678E+300, new byte[] { 0x52, 0xD3, 0xBB, 0xBC, 0xE8, 0x7E, 0x3D, 0x7E })]
        [InlineData(double.MinValue, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xEF, 0xFF })]
        [InlineData(double.MaxValue, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xEF, 0x7F })]
        [InlineData(double.Epsilon, new byte[] { 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 })]
        [InlineData(double.NaN, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF8, 0xFF })]
        [InlineData(double.NegativeInfinity, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF0, 0xFF })]
        [InlineData(double.PositiveInfinity, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF0, 0x7F })]
        public abstract void ConvertFromDouble(double num, byte[] byteArr);

        [Theory]
        [InlineData(0, ' ')]
        [InlineData(1, '\0')]
        [InlineData(3, '*')]
        [InlineData(5, 'A')]
        [InlineData(7, '}')]
        [InlineData(9, '\u00C5')] // Latin capital letter A with ring above (Å)
        [InlineData(11, '\u03A8')] // Greek capital letter Psi (Ψ)
        [InlineData(13, '\u0429')] // Cyrillic capital letter Shcha (Щ)
        [InlineData(15, '\u20AC')] // Euro sign (€)
        public abstract void ToChar(int index, char character);

        [Theory]
        [InlineData(1, (short)0)]
        [InlineData(0, (short)15)]
        [InlineData(8, (short)-15)]
        [InlineData(4, (short)10000)]
        [InlineData(6, (short)-10000)]
        [InlineData(9, (short)32767)]
        [InlineData(2, (short)-32768)]
        public abstract void ToInt16(int index, short expected);

        [Theory]
        [InlineData(0x00EC, new byte[] { 0xEC, 0x00, 0x00, 0x00 })]
        [InlineData(0x3FFFFFFF, new byte[] { 0xFF, 0xFF, 0xFF, 0x3F })]
        [InlineData(0x1000, new byte[] { 0x00, 0x10, 0x00, 0x00 })]
        public abstract void ToInt32(int expected, byte[] byteArray);

        [Theory]
        [InlineData(8, (long)0)]
        [InlineData(5, (long)16777215)]
        [InlineData(34, (long)-16777215)]
        [InlineData(17, (long)1000000000)]
        [InlineData(0, (long)-1000000000)]
        //[InlineData(21, 4294967296)]
        //[InlineData(26, -4294967296)]
        //[InlineData(53, 187649984473770)]
        //[InlineData(45, -187649984473770)]
        //[InlineData(59, 1000000000000000000)]
        //[InlineData(67, -1000000000000000000)]
        //[InlineData(37, 9223372036854775807)]
        //[InlineData(9, -9223372036854775808)]
        public abstract void ToInt64(int index, long expected);

        /* TODO:
         * ToUInt16
         * ToUInt32
         * ToUInt64
         * ToSingle
         * ToDouble
         * ToBoolean
         * FIX: ToInt64 (overflow?)
         * ADD: Chars over unicode +255 to ToChar
         */
    }
}
