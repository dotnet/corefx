// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Tests
{
    public class BitConverterSpan : BitConverterBase
    {
        [Fact]
        public void TryWriteBytes_DestinationSpanNotLargeEnough()
        {
            Assert.False(BitConverter.TryWriteBytes(Span<byte>.Empty, false));
            Assert.False(BitConverter.TryWriteBytes(Span<byte>.Empty, 'a'));
            Assert.False(BitConverter.TryWriteBytes(Span<byte>.Empty, (short)2));
            Assert.False(BitConverter.TryWriteBytes(Span<byte>.Empty, 2));
            Assert.False(BitConverter.TryWriteBytes(Span<byte>.Empty, (long)2));
            Assert.False(BitConverter.TryWriteBytes(Span<byte>.Empty, (ushort)2));
            Assert.False(BitConverter.TryWriteBytes(Span<byte>.Empty, (uint)2));
            Assert.False(BitConverter.TryWriteBytes(Span<byte>.Empty, (ulong)2));
            Assert.False(BitConverter.TryWriteBytes(Span<byte>.Empty, (float)2));
            Assert.False(BitConverter.TryWriteBytes(Span<byte>.Empty, 2.0));
        }

        [Fact]
        public void ToMethods_DestinationSpanNotLargeEnough()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => { BitConverter.ToChar(Span<byte>.Empty); });
            Assert.Throws<ArgumentOutOfRangeException>(() => { BitConverter.ToInt16(Span<byte>.Empty); });
            Assert.Throws<ArgumentOutOfRangeException>(() => { BitConverter.ToInt32(Span<byte>.Empty); });
            Assert.Throws<ArgumentOutOfRangeException>(() => { BitConverter.ToInt64(Span<byte>.Empty); });
            Assert.Throws<ArgumentOutOfRangeException>(() => { BitConverter.ToUInt16(Span<byte>.Empty); });
            Assert.Throws<ArgumentOutOfRangeException>(() => { BitConverter.ToUInt32(Span<byte>.Empty); });
            Assert.Throws<ArgumentOutOfRangeException>(() => { BitConverter.ToUInt64(Span<byte>.Empty); });
            Assert.Throws<ArgumentOutOfRangeException>(() => { BitConverter.ToSingle(Span<byte>.Empty); });
            Assert.Throws<ArgumentOutOfRangeException>(() => { BitConverter.ToDouble(Span<byte>.Empty); });
            Assert.Throws<ArgumentOutOfRangeException>(() => { BitConverter.ToBoolean(Span<byte>.Empty); });
        }

        public override void ConvertFromBool(bool boolean, int expected)
        {
            Span<byte> span = new Span<byte>(new byte[1]);
            Assert.True(BitConverter.TryWriteBytes(span, boolean));
            Assert.Equal(expected, span[0]);
        }

        public override void ConvertFromShort(short num, byte[] byteArr)
        {
            Span<byte> span = new Span<byte>(new byte[2]);
            Assert.True(BitConverter.TryWriteBytes(span, num));
            Assert.Equal(byteArr, span.ToArray());
        }

        public override void ConvertFromChar(char character, byte[] byteArr)
        {
            Span<byte> span = new Span<byte>(new byte[2]);
            Assert.True(BitConverter.TryWriteBytes(span, character));
            Assert.Equal(byteArr, span.ToArray());
        }

        public override void ConvertFromInt(int num, byte[] byteArr)
        {
            Span<byte> span = new Span<byte>(new byte[4]);
            Assert.True(BitConverter.TryWriteBytes(span, num));
            Assert.Equal(byteArr, span.ToArray());
        }

        public override void ConvertFromLong(long num, byte[] byteArr)
        {
            Span<byte> span = new Span<byte>(new byte[8]);
            Assert.True(BitConverter.TryWriteBytes(span, num));
            Assert.Equal(byteArr, span.ToArray());
        }

        public override void ConvertFromUShort(ushort num, byte[] byteArr)
        {
            Span<byte> span = new Span<byte>(new byte[2]);
            Assert.True(BitConverter.TryWriteBytes(span, num));
            Assert.Equal(byteArr, span.ToArray());
        }

        public override void ConvertFromUInt(uint num, byte[] byteArr)
        {
            Span<byte> span = new Span<byte>(new byte[4]);
            Assert.True(BitConverter.TryWriteBytes(span, num));
            Assert.Equal(byteArr, span.ToArray());
        }

        public override void ConvertFromULong(ulong num, byte[] byteArr)
        {
            Span<byte> span = new Span<byte>(new byte[8]);
            Assert.True(BitConverter.TryWriteBytes(span, num));
            Assert.Equal(byteArr, span.ToArray());
        }

        public override void ConvertFromFloat(float num, byte[] byteArr)
        {
            Span<byte> span = new Span<byte>(new byte[4]);
            Assert.True(BitConverter.TryWriteBytes(span, num));
            Assert.Equal(byteArr, span.ToArray());
        }

        public override void ConvertFromDouble(double num, byte[] byteArr)
        {
            Span<byte> span = new Span<byte>(new byte[8]);
            Assert.True(BitConverter.TryWriteBytes(span, num));
            Assert.Equal(byteArr, span.ToArray());
        }

        public override void ToChar(int index, char character)
        {
            byte[] byteArray = { 0x20, 0x00, 0x00, 0x2A, 0x00, 0x41, 0x00, 0x7D, 0x00, 0xC5, 0x00, 0xA8, 0x03, 0x29, 0x04, 0xAC, 0x20 };
            ReadOnlySpan<byte> span = new ReadOnlySpan<byte>(byteArray);
            BitConverter.ToChar(span);
            Assert.Equal(character, BitConverter.ToChar(span.Slice(index)));
        }

        public override void ToInt16(int index, short expected)
        {
            byte[] byteArray = { 15, 0, 0, 128, 16, 39, 240, 216, 241, 255, 127 };
            ReadOnlySpan<byte> span = new ReadOnlySpan<byte>(byteArray);
            Assert.Equal(expected, BitConverter.ToInt16(span.Slice(index)));
        }

        public override void ToInt32(int expected, byte[] byteArray)
        {
            ReadOnlySpan<byte> span = new ReadOnlySpan<byte>(byteArray);
            Assert.Equal(expected, BitConverter.ToInt32(byteArray));
        }

        public override void ToInt64(int index, long expected)
        {
            byte[] byteArray =
                { 0x00, 0x36, 0x65, 0xC4, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x80, 0x00, 0xCA, 0x9A,
            0x3B, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0x01, 0x00, 0x00, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0xFF, 0xFF, 0x7F, 0x56, 0x55, 0x55, 0x55, 0x55, 0x55, 0xFF, 0xFF, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0x00,
            0x00, 0x64, 0xA7, 0xB3, 0xB6, 0xE0, 0x0D, 0x00, 0x00, 0x9C, 0x58, 0x4C, 0x49, 0x1F, 0xF2 };
            ReadOnlySpan<byte> span = new ReadOnlySpan<byte>(byteArray);
            Assert.Equal(expected, BitConverter.ToInt32(span.Slice(index)));
        }
    }
}
