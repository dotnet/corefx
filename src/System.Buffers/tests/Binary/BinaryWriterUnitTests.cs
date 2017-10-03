// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xunit;

using static System.Buffers.Binary.BinaryPrimitives;

namespace System.Buffers.Binary.Tests
{
    public class BinaryWriterUnitTests
    {
        

        [Fact]
        public void SpanWrite()
        {
            Assert.True(BitConverter.IsLittleEndian);

            Span<byte> span = new byte[8];

            WriteCurrentEndianness<byte>(ref span, 0x11);
            TestHelpers.Validate<byte>(span, 0x11);
            Assert.True(TryWriteCurrentEndianness<byte>(ref span, 0x11));
            TestHelpers.Validate<byte>(span, 0x11);

            WriteCurrentEndianness<sbyte>(ref span, 0x11);
            TestHelpers.Validate<sbyte>(span, 0x11);
            Assert.True(TryWriteCurrentEndianness<sbyte>(ref span, 0x11));
            TestHelpers.Validate<sbyte>(span, 0x11);

            WriteCurrentEndianness<ushort>(ref span, 0x1122);
            TestHelpers.Validate<ushort>(span, 0x1122);
            Assert.True(TryWriteCurrentEndianness<ushort>(ref span, 0x1122));
            TestHelpers.Validate<ushort>(span, 0x1122);

            WriteCurrentEndianness<uint>(ref span, 0x11223344);
            TestHelpers.Validate<uint>(span, 0x11223344);
            Assert.True(TryWriteCurrentEndianness<uint>(ref span, 0x11223344));
            TestHelpers.Validate<uint>(span, 0x11223344);

            WriteCurrentEndianness<ulong>(ref span, 0x1122334455667788);
            TestHelpers.Validate<ulong>(span, 0x1122334455667788);
            Assert.True(TryWriteCurrentEndianness<ulong>(ref span, 0x1122334455667788));
            TestHelpers.Validate<ulong>(span, 0x1122334455667788);

            WriteCurrentEndianness<short>(ref span, 0x1122);
            TestHelpers.Validate<short>(span, 0x1122);
            Assert.True(TryWriteCurrentEndianness<short>(ref span, 0x1122));
            TestHelpers.Validate<short>(span, 0x1122);

            WriteCurrentEndianness<int>(ref span, 0x11223344);
            TestHelpers.Validate<int>(span, 0x11223344);
            Assert.True(TryWriteCurrentEndianness<int>(ref span, 0x11223344));
            TestHelpers.Validate<int>(span, 0x11223344);

            WriteCurrentEndianness<long>(ref span, 0x1122334455667788);
            TestHelpers.Validate<long>(span, 0x1122334455667788);
            Assert.True(TryWriteCurrentEndianness<long>(ref span, 0x1122334455667788));
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
            WriteInt16BigEndian(span, value);
            short read = ReadInt16BigEndian(span);
            Assert.Equal(value, read);

            span.Clear();
            Assert.True(TryWriteInt16BigEndian(span, value));
            read = ReadInt16BigEndian(span);
            Assert.Equal(value, read);

            span.Clear();
            WriteInt16LittleEndian(span, value);
            read = ReadInt16LittleEndian(span);
            Assert.Equal(value, read);

            span.Clear();
            Assert.True(TryWriteInt16LittleEndian(span, value));
            read = ReadInt16LittleEndian(span);
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
            WriteUInt16BigEndian(span, value);
            ushort read = ReadUInt16BigEndian(span);
            Assert.Equal(value, read);

            span.Clear();
            Assert.True(TryWriteUInt16BigEndian(span, value));
            read = ReadUInt16BigEndian(span);
            Assert.Equal(value, read);

            span.Clear();
            WriteUInt16LittleEndian(span, value);
            read = ReadUInt16LittleEndian(span);
            Assert.Equal(value, read);

            span.Clear();
            Assert.True(TryWriteUInt16LittleEndian(span, value));
            read = ReadUInt16LittleEndian(span);
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
            WriteInt32BigEndian(span, value);
            int read = ReadInt32BigEndian(span);
            Assert.Equal(value, read);

            span.Clear();
            Assert.True(TryWriteInt32BigEndian(span, value));
            read = ReadInt32BigEndian(span);
            Assert.Equal(value, read);

            span.Clear();
            WriteInt32LittleEndian(span, value);
            read = ReadInt32LittleEndian(span);
            Assert.Equal(value, read);

            span.Clear();
            Assert.True(TryWriteInt32LittleEndian(span, value));
            read = ReadInt32LittleEndian(span);
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
            WriteUInt32BigEndian(span, value);
            uint read = ReadUInt32BigEndian(span);
            Assert.Equal(value, read);

            span.Clear();
            Assert.True(TryWriteUInt32BigEndian(span, value));
            read = ReadUInt32BigEndian(span);
            Assert.Equal(value, read);

            span.Clear();
            WriteUInt32LittleEndian(span, value);
            read = ReadUInt32LittleEndian(span);
            Assert.Equal(value, read);

            span.Clear();
            Assert.True(TryWriteUInt32LittleEndian(span, value));
            read = ReadUInt32LittleEndian(span);
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
            WriteInt64BigEndian(span, value);
            long read = ReadInt64BigEndian(span);
            Assert.Equal(value, read);

            span.Clear();
            Assert.True(TryWriteInt64BigEndian(span, value));
            read = ReadInt64BigEndian(span);
            Assert.Equal(value, read);

            span.Clear();
            WriteInt64LittleEndian(span, value);
            read = ReadInt64LittleEndian(span);
            Assert.Equal(value, read);

            span.Clear();
            Assert.True(TryWriteInt64LittleEndian(span, value));
            read = ReadInt64LittleEndian(span);
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
            WriteUInt64BigEndian(span, value);
            ulong read = ReadUInt64BigEndian(span);
            Assert.Equal(value, read);

            span.Clear();
            Assert.True(TryWriteUInt64BigEndian(span, value));
            read = ReadUInt64BigEndian(span);
            Assert.Equal(value, read);

            span.Clear();
            WriteUInt64LittleEndian(span, value);
            read = ReadUInt64LittleEndian(span);
            Assert.Equal(value, read);

            span.Clear();
            Assert.True(TryWriteUInt64LittleEndian(span, value));
            read = ReadUInt64LittleEndian(span);
            Assert.Equal(value, read);
        }

        [Fact]
        public void SpanWriteFail()
        {
            byte value = 1;
            Span<byte> span = new byte[1];
            
            WriteCurrentEndianness<byte>(ref span, value);
            byte read = ReadCurrentEndianness<byte>(span);
            Assert.Equal<byte>(value, read);

            span.Clear();
            Assert.True(TryWriteCurrentEndianness<byte>(ref span, value));
            read = ReadCurrentEndianness<byte>(span);
            Assert.Equal<byte>(value, read);

            TestHelpers.AssertThrows<ArgumentOutOfRangeException, byte>(span, (_span) => WriteCurrentEndianness<short>(ref _span, value));
            Assert.False(TryWriteCurrentEndianness<short>(ref span, value));
            TestHelpers.AssertThrows<ArgumentOutOfRangeException, byte>(span, (_span) => WriteCurrentEndianness<int>(ref _span, value));
            Assert.False(TryWriteCurrentEndianness<int>(ref span, value));
            TestHelpers.AssertThrows<ArgumentOutOfRangeException, byte>(span, (_span) => WriteCurrentEndianness<long>(ref _span, value));
            Assert.False(TryWriteCurrentEndianness<long>(ref span, value));

            TestHelpers.AssertThrows<ArgumentOutOfRangeException, byte>(span, (_span) => WriteCurrentEndianness<ushort>(ref _span, value));
            Assert.False(TryWriteCurrentEndianness<ushort>(ref span, value));
            TestHelpers.AssertThrows<ArgumentOutOfRangeException, byte>(span, (_span) => WriteCurrentEndianness<uint>(ref _span, value));
            Assert.False(TryWriteCurrentEndianness<uint>(ref span, value));
            TestHelpers.AssertThrows<ArgumentOutOfRangeException, byte>(span, (_span) => WriteCurrentEndianness<ulong>(ref _span, value));
            Assert.False(TryWriteCurrentEndianness<ulong>(ref span, value));
        }
    }
}
