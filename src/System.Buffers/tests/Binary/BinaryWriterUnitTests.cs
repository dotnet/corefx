// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xunit;

namespace System.Buffers.Binary.Tests
{
    public class BinaryWriterUnitTests
    {
        

        [Fact]
        public void SpanWrite()
        {
            Assert.True(BitConverter.IsLittleEndian);

            Span<byte> span = new byte[8];

            span.Write<byte>(0x11);
            TestHelpers.Validate<byte>(span, 0x11);
            Assert.True(span.TryWrite<byte>(0x11));
            TestHelpers.Validate<byte>(span, 0x11);

            span.Write<sbyte>(0x11);
            TestHelpers.Validate<sbyte>(span, 0x11);
            Assert.True(span.TryWrite<sbyte>(0x11));
            TestHelpers.Validate<sbyte>(span, 0x11);

            span.Write<ushort>(0x1122);
            TestHelpers.Validate<ushort>(span, 0x1122);
            Assert.True(span.TryWrite<ushort>(0x1122));
            TestHelpers.Validate<ushort>(span, 0x1122);

            span.Write<uint>(0x11223344);
            TestHelpers.Validate<uint>(span, 0x11223344);
            Assert.True(span.TryWrite<uint>(0x11223344));
            TestHelpers.Validate<uint>(span, 0x11223344);

            span.Write<ulong>(0x1122334455667788);
            TestHelpers.Validate<ulong>(span, 0x1122334455667788);
            Assert.True(span.TryWrite<ulong>(0x1122334455667788));
            TestHelpers.Validate<ulong>(span, 0x1122334455667788);

            span.Write<short>(0x1122);
            TestHelpers.Validate<short>(span, 0x1122);
            Assert.True(span.TryWrite<short>(0x1122));
            TestHelpers.Validate<short>(span, 0x1122);

            span.Write<int>(0x11223344);
            TestHelpers.Validate<int>(span, 0x11223344);
            Assert.True(span.TryWrite<int>(0x11223344));
            TestHelpers.Validate<int>(span, 0x11223344);

            span.Write<long>(0x1122334455667788);
            TestHelpers.Validate<long>(span, 0x1122334455667788);
            Assert.True(span.TryWrite<long>(0x1122334455667788));
            TestHelpers.Validate<long>(span, 0x1122334455667788);
        }

        [Theory]
        [InlineData(short.MaxValue)]
        [InlineData(short.MinValue)]
        [InlineData(0x7F00)]
        [InlineData(0x00FF)]
        public void SpanWriteInt16(short value)
        {
            Assert.True(BitConverter.IsLittleEndian);
            var span = new Span<byte>(new byte[2]);
            span.WriteInt16BigEndian(value);
            short read = span.ReadInt16BigEndian();
            Assert.Equal(value, read);

            span.Clear();
            Assert.True(span.TryWriteInt16BigEndian(value));
            read = span.ReadInt16BigEndian();
            Assert.Equal(value, read);

            span.Clear();
            span.WriteInt16LittleEndian(value);
            read = span.ReadInt16LittleEndian();
            Assert.Equal(value, read);

            span.Clear();
            Assert.True(span.TryWriteInt16LittleEndian(value));
            read = span.ReadInt16LittleEndian();
            Assert.Equal(value, read);
        }

        [Theory]
        [InlineData(ushort.MaxValue)]
        [InlineData(ushort.MinValue)]
        [InlineData(0xFF00)]
        [InlineData(0x00FF)]
        public void SpanWriteUInt16(ushort value)
        {
            Assert.True(BitConverter.IsLittleEndian);
            var span = new Span<byte>(new byte[2]);
            span.WriteUInt16BigEndian(value);
            ushort read = span.ReadUInt16BigEndian();
            Assert.Equal(value, read);

            span.Clear();
            Assert.True(span.TryWriteUInt16BigEndian(value));
            read = span.ReadUInt16BigEndian();
            Assert.Equal(value, read);

            span.Clear();
            span.WriteUInt16LittleEndian(value);
            read = span.ReadUInt16LittleEndian();
            Assert.Equal(value, read);

            span.Clear();
            Assert.True(span.TryWriteUInt16LittleEndian(value));
            read = span.ReadUInt16LittleEndian();
            Assert.Equal(value, read);
        }

        [Theory]
        [InlineData(int.MaxValue)]
        [InlineData(int.MinValue)]
        [InlineData(0x7F000000)]
        [InlineData(0x00FF0000)]
        [InlineData(0x0000FF00)]
        [InlineData(0x000000FF)]
        public void SpanWriteInt32(int value)
        {
            Assert.True(BitConverter.IsLittleEndian);
            var span = new Span<byte>(new byte[4]);
            span.WriteInt32BigEndian(value);
            int read = span.ReadInt32BigEndian();
            Assert.Equal(value, read);

            span.Clear();
            Assert.True(span.TryWriteInt32BigEndian(value));
            read = span.ReadInt32BigEndian();
            Assert.Equal(value, read);

            span.Clear();
            span.WriteInt32LittleEndian(value);
            read = span.ReadInt32LittleEndian();
            Assert.Equal(value, read);

            span.Clear();
            Assert.True(span.TryWriteInt32LittleEndian(value));
            read = span.ReadInt32LittleEndian();
            Assert.Equal(value, read);
        }

        [Theory]
        [InlineData(uint.MaxValue)]
        [InlineData(uint.MinValue)]
        [InlineData(0xFF000000)]
        [InlineData(0x00FF0000)]
        [InlineData(0x0000FF00)]
        [InlineData(0x000000FF)]
        public void SpanWriteUInt32(uint value)
        {
            Assert.True(BitConverter.IsLittleEndian);
            var span = new Span<byte>(new byte[4]);
            span.WriteUInt32BigEndian(value);
            uint read = span.ReadUInt32BigEndian();
            Assert.Equal(value, read);

            span.Clear();
            Assert.True(span.TryWriteUInt32BigEndian(value));
            read = span.ReadUInt32BigEndian();
            Assert.Equal(value, read);

            span.Clear();
            span.WriteUInt32LittleEndian(value);
            read = span.ReadUInt32LittleEndian();
            Assert.Equal(value, read);

            span.Clear();
            Assert.True(span.TryWriteUInt32LittleEndian(value));
            read = span.ReadUInt32LittleEndian();
            Assert.Equal(value, read);
        }

        [Theory]
        [InlineData(long.MaxValue)]
        [InlineData(long.MinValue)]
        [InlineData(0x7F00000000000000)]
        [InlineData(0x00FF000000000000)]
        [InlineData(0x0000FF0000000000)]
        [InlineData(0x000000FF00000000)]
        [InlineData(0x00000000FF000000)]
        [InlineData(0x0000000000FF0000)]
        [InlineData(0x000000000000FF00)]
        [InlineData(0x00000000000000FF)]
        public void SpanWriteInt64(long value)
        {
            Assert.True(BitConverter.IsLittleEndian);
            var span = new Span<byte>(new byte[8]);
            span.WriteInt64BigEndian(value);
            long read = span.ReadInt64BigEndian();
            Assert.Equal(value, read);

            span.Clear();
            Assert.True(span.TryWriteInt64BigEndian(value));
            read = span.ReadInt64BigEndian();
            Assert.Equal(value, read);

            span.Clear();
            span.WriteInt64LittleEndian(value);
            read = span.ReadInt64LittleEndian();
            Assert.Equal(value, read);

            span.Clear();
            Assert.True(span.TryWriteInt64LittleEndian(value));
            read = span.ReadInt64LittleEndian();
            Assert.Equal(value, read);
        }

        [Theory]
        [InlineData(ulong.MaxValue)]
        [InlineData(ulong.MinValue)]
        [InlineData(0xFF00000000000000)]
        [InlineData(0x00FF000000000000)]
        [InlineData(0x0000FF0000000000)]
        [InlineData(0x000000FF00000000)]
        [InlineData(0x00000000FF000000)]
        [InlineData(0x0000000000FF0000)]
        [InlineData(0x000000000000FF00)]
        [InlineData(0x00000000000000FF)]
        public void SpanWriteUInt64(ulong value)
        {
            Assert.True(BitConverter.IsLittleEndian);
            var span = new Span<byte>(new byte[8]);
            span.WriteUInt64BigEndian(value);
            ulong read = span.ReadUInt64BigEndian();
            Assert.Equal(value, read);

            span.Clear();
            Assert.True(span.TryWriteUInt64BigEndian(value));
            read = span.ReadUInt64BigEndian();
            Assert.Equal(value, read);

            span.Clear();
            span.WriteUInt64LittleEndian(value);
            read = span.ReadUInt64LittleEndian();
            Assert.Equal(value, read);

            span.Clear();
            Assert.True(span.TryWriteUInt64LittleEndian(value));
            read = span.ReadUInt64LittleEndian();
            Assert.Equal(value, read);
        }

        [Fact]
        public void SpanWriteFail()
        {
            byte value = 1;
            Span<byte> span = new byte[1];
            
            span.Write<byte>(value);
            byte read = span.Read<byte>();
            Assert.Equal<byte>(value, read);

            span.Clear();
            Assert.True(span.TryWrite<byte>(value));
            read = span.Read<byte>();
            Assert.Equal<byte>(value, read);

            TestHelpers.AssertThrows<ArgumentOutOfRangeException, byte>(span, (_span) => _span.Write<short>(value));
            Assert.False(span.TryWrite<short>(value));
            TestHelpers.AssertThrows<ArgumentOutOfRangeException, byte>(span, (_span) => _span.Write<int>(value));
            Assert.False(span.TryWrite<int>(value));
            TestHelpers.AssertThrows<ArgumentOutOfRangeException, byte>(span, (_span) => _span.Write<long>(value));
            Assert.False(span.TryWrite<long>(value));

            TestHelpers.AssertThrows<ArgumentOutOfRangeException, byte>(span, (_span) => _span.Write<ushort>(value));
            Assert.False(span.TryWrite<ushort>(value));
            TestHelpers.AssertThrows<ArgumentOutOfRangeException, byte>(span, (_span) => _span.Write<uint>(value));
            Assert.False(span.TryWrite<uint>(value));
            TestHelpers.AssertThrows<ArgumentOutOfRangeException, byte>(span, (_span) => _span.Write<ulong>(value));
            Assert.False(span.TryWrite<ulong>(value));
        }
    }
}
