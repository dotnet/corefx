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

            WriteMachineEndian<byte>(ref span, 0x11);
            TestHelpers.Validate<byte>(span, 0x11);
            Assert.True(TryWriteMachineEndian<byte>(ref span, 0x11));
            TestHelpers.Validate<byte>(span, 0x11);

            WriteMachineEndian<sbyte>(ref span, 0x11);
            TestHelpers.Validate<sbyte>(span, 0x11);
            Assert.True(TryWriteMachineEndian<sbyte>(ref span, 0x11));
            TestHelpers.Validate<sbyte>(span, 0x11);

            WriteMachineEndian<ushort>(ref span, 0x1122);
            TestHelpers.Validate<ushort>(span, 0x1122);
            Assert.True(TryWriteMachineEndian<ushort>(ref span, 0x1122));
            TestHelpers.Validate<ushort>(span, 0x1122);

            WriteMachineEndian<uint>(ref span, 0x11223344);
            TestHelpers.Validate<uint>(span, 0x11223344);
            Assert.True(TryWriteMachineEndian<uint>(ref span, 0x11223344));
            TestHelpers.Validate<uint>(span, 0x11223344);

            WriteMachineEndian<ulong>(ref span, 0x1122334455667788);
            TestHelpers.Validate<ulong>(span, 0x1122334455667788);
            Assert.True(TryWriteMachineEndian<ulong>(ref span, 0x1122334455667788));
            TestHelpers.Validate<ulong>(span, 0x1122334455667788);

            WriteMachineEndian<short>(ref span, 0x1122);
            TestHelpers.Validate<short>(span, 0x1122);
            Assert.True(TryWriteMachineEndian<short>(ref span, 0x1122));
            TestHelpers.Validate<short>(span, 0x1122);

            WriteMachineEndian<int>(ref span, 0x11223344);
            TestHelpers.Validate<int>(span, 0x11223344);
            Assert.True(TryWriteMachineEndian<int>(ref span, 0x11223344));
            TestHelpers.Validate<int>(span, 0x11223344);

            WriteMachineEndian<long>(ref span, 0x1122334455667788);
            TestHelpers.Validate<long>(span, 0x1122334455667788);
            Assert.True(TryWriteMachineEndian<long>(ref span, 0x1122334455667788));
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
            
            WriteMachineEndian<byte>(ref span, value);
            byte read = ReadMachineEndian<byte>(span);
            Assert.Equal<byte>(value, read);

            span.Clear();
            Assert.True(TryWriteMachineEndian<byte>(ref span, value));
            read = ReadMachineEndian<byte>(span);
            Assert.Equal<byte>(value, read);

            TestHelpers.AssertThrows<ArgumentOutOfRangeException, byte>(span, (_span) => WriteMachineEndian<short>(ref _span, value));
            Assert.False(TryWriteMachineEndian<short>(ref span, value));
            TestHelpers.AssertThrows<ArgumentOutOfRangeException, byte>(span, (_span) => WriteMachineEndian<int>(ref _span, value));
            Assert.False(TryWriteMachineEndian<int>(ref span, value));
            TestHelpers.AssertThrows<ArgumentOutOfRangeException, byte>(span, (_span) => WriteMachineEndian<long>(ref _span, value));
            Assert.False(TryWriteMachineEndian<long>(ref span, value));

            TestHelpers.AssertThrows<ArgumentOutOfRangeException, byte>(span, (_span) => WriteMachineEndian<ushort>(ref _span, value));
            Assert.False(TryWriteMachineEndian<ushort>(ref span, value));
            TestHelpers.AssertThrows<ArgumentOutOfRangeException, byte>(span, (_span) => WriteMachineEndian<uint>(ref _span, value));
            Assert.False(TryWriteMachineEndian<uint>(ref span, value));
            TestHelpers.AssertThrows<ArgumentOutOfRangeException, byte>(span, (_span) => WriteMachineEndian<ulong>(ref _span, value));
            Assert.False(TryWriteMachineEndian<ulong>(ref span, value));
        }
    }
}
