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

        public override void ToChar(int index, char expected)
        {
            ReadOnlySpan<byte> span = new ReadOnlySpan<byte>(s_toCharByteArray);
            BitConverter.ToChar(span);
            Assert.Equal(expected, BitConverter.ToChar(span.Slice(index)));
        }

        public override void ToInt16(int index, short expected)
        {
            ReadOnlySpan<byte> span = new ReadOnlySpan<byte>(s_toInt16ByteArray);
            Assert.Equal(expected, BitConverter.ToInt16(span.Slice(index)));
        }

        public override void ToInt32(int expected, byte[] byteArray)
        {
            ReadOnlySpan<byte> span = new ReadOnlySpan<byte>(byteArray);
            Assert.Equal(expected, BitConverter.ToInt32(byteArray));
        }

        public override void ToInt64(int index, long expected)
        {
            ReadOnlySpan<byte> span = new ReadOnlySpan<byte>(s_toInt64ByteArray);
            Assert.Equal(expected, BitConverter.ToInt64(span.Slice(index)));
        }

        public override void ToUInt16(int index, ushort expected)
        {
            ReadOnlySpan<byte> span = new ReadOnlySpan<byte>(s_toUInt16ByteArray);
            Assert.Equal(expected, BitConverter.ToUInt16(span.Slice(index)));
        }

        public override void ToUInt32(int index, uint expected)
        {
            ReadOnlySpan<byte> span = new ReadOnlySpan<byte>(s_toUInt32ByteArray);
            Assert.Equal(expected, BitConverter.ToUInt32(span.Slice(index)));
        }

        public override void ToUInt64(int index, ulong expected)
        {
            ReadOnlySpan<byte> span = new ReadOnlySpan<byte>(s_toUInt64ByteArray);
            Assert.Equal(expected, BitConverter.ToUInt64(span.Slice(index)));
        }

        public override void ToSingle(int index, float expected)
        {
            ReadOnlySpan<byte> span = new ReadOnlySpan<byte>(s_toSingleByteArray);
            Assert.Equal(expected, BitConverter.ToSingle(span.Slice(index)));
        }

        public override void ToDouble(int index, double expected)
        {
            ReadOnlySpan<byte> span = new ReadOnlySpan<byte>(s_toDoubleByteArray);
            Assert.Equal(expected, BitConverter.ToDouble(span.Slice(index)));
        }

        public override void ToBoolean(int index, bool expected)
        {
            ReadOnlySpan<byte> span = new ReadOnlySpan<byte>(s_toBooleanByteArray);
            Assert.Equal(expected, BitConverter.ToBoolean(span.Slice(index)));
        }
    }
}
