// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xunit;

namespace System.Buffers.Binary.Tests
{
    public class BinaryReaderUnitTests
    {

        [Fact]
        public void SpanRead()
        {
            Assert.True(BitConverter.IsLittleEndian);

            ulong value = 0x8877665544332211; // [11 22 33 44 55 66 77 88]
            Span<byte> span;
            unsafe {
                span = new Span<byte>(&value, 8);
            }

            Assert.Equal<byte>(0x11, span.Read<byte>());
            Assert.True(span.TryRead(out byte byteValue));
            Assert.Equal(0x11, byteValue);

            Assert.Equal<sbyte>(0x11, span.Read<sbyte>());
            Assert.True(span.TryRead(out byte sbyteValue));
            Assert.Equal(0x11, byteValue);

            Assert.Equal<ushort>(0x1122, span.ReadUInt16BigEndian());
            Assert.True(span.TryReadUInt16BigEndian(out ushort ushortValue));
            Assert.Equal(0x1122, ushortValue);

            Assert.Equal<ushort>(0x2211, span.ReadUInt16LittleEndian());
            Assert.True(span.TryReadUInt16LittleEndian(out ushortValue));
            Assert.Equal(0x2211, ushortValue);

            Assert.Equal<short>(0x1122, span.ReadInt16BigEndian());
            Assert.True(span.TryReadInt16BigEndian(out short shortValue));
            Assert.Equal(0x1122, shortValue);

            Assert.Equal<short>(0x2211, span.ReadInt16LittleEndian());
            Assert.True(span.TryReadInt16LittleEndian(out shortValue));
            Assert.Equal(0x2211, ushortValue);

            Assert.Equal<uint>(0x11223344, span.ReadUInt32BigEndian());
            Assert.True(span.TryReadUInt32BigEndian(out uint uintValue));
            Assert.Equal<uint>(0x11223344, uintValue);

            Assert.Equal<uint>(0x44332211, span.ReadUInt32LittleEndian());
            Assert.True(span.TryReadUInt32LittleEndian(out uintValue));
            Assert.Equal<uint>(0x44332211, uintValue);

            Assert.Equal<int>(0x11223344, span.ReadInt32BigEndian());
            Assert.True(span.TryReadInt32BigEndian(out int intValue));
            Assert.Equal<int>(0x11223344, intValue);

            Assert.Equal<int>(0x44332211, span.ReadInt32LittleEndian());
            Assert.True(span.TryReadInt32LittleEndian(out intValue));
            Assert.Equal<int>(0x44332211, intValue);

            Assert.Equal<ulong>(0x1122334455667788, span.ReadUInt64BigEndian());
            Assert.True(span.TryReadUInt64BigEndian(out ulong ulongValue));
            Assert.Equal<ulong>(0x1122334455667788, ulongValue);

            Assert.Equal<ulong>(0x8877665544332211, span.ReadUInt64LittleEndian());
            Assert.True(span.TryReadUInt64LittleEndian(out ulongValue));
            Assert.Equal<ulong>(0x8877665544332211, ulongValue);

            Assert.Equal<long>(0x1122334455667788, span.ReadInt64BigEndian());
            Assert.True(span.TryReadInt64BigEndian(out long longValue));
            Assert.Equal<long>(0x1122334455667788, longValue);

            Assert.Equal<long>(unchecked((long)0x8877665544332211), span.ReadInt64LittleEndian());
            Assert.True(span.TryReadInt64LittleEndian(out longValue));
            Assert.Equal<long>(unchecked((long)0x8877665544332211), longValue);
        }

        [Fact]
        public void ReadOnlySpanRead()
        {
            Assert.True(BitConverter.IsLittleEndian);

            ulong value = 0x8877665544332211; // [11 22 33 44 55 66 77 88]
            ReadOnlySpan<byte> span;
            unsafe {
                span = new ReadOnlySpan<byte>(&value, 8);
            }

            Assert.Equal<byte>(0x11, span.Read<byte>());
            Assert.True(span.TryRead(out byte byteValue));
            Assert.Equal(0x11, byteValue);

            Assert.Equal<sbyte>(0x11, span.Read<sbyte>());
            Assert.True(span.TryRead(out byte sbyteValue));
            Assert.Equal(0x11, byteValue);

            Assert.Equal<ushort>(0x1122, span.ReadUInt16BigEndian());
            Assert.True(span.TryReadUInt16BigEndian(out ushort ushortValue));
            Assert.Equal(0x1122, ushortValue);

            Assert.Equal<ushort>(0x2211, span.ReadUInt16LittleEndian());
            Assert.True(span.TryReadUInt16LittleEndian(out ushortValue));
            Assert.Equal(0x2211, ushortValue);

            Assert.Equal<short>(0x1122, span.ReadInt16BigEndian());
            Assert.True(span.TryReadInt16BigEndian(out short shortValue));
            Assert.Equal(0x1122, shortValue);

            Assert.Equal<short>(0x2211, span.ReadInt16LittleEndian());
            Assert.True(span.TryReadInt16LittleEndian(out shortValue));
            Assert.Equal(0x2211, ushortValue);

            Assert.Equal<uint>(0x11223344, span.ReadUInt32BigEndian());
            Assert.True(span.TryReadUInt32BigEndian(out uint uintValue));
            Assert.Equal<uint>(0x11223344, uintValue);

            Assert.Equal<uint>(0x44332211, span.ReadUInt32LittleEndian());
            Assert.True(span.TryReadUInt32LittleEndian(out uintValue));
            Assert.Equal<uint>(0x44332211, uintValue);

            Assert.Equal<int>(0x11223344, span.ReadInt32BigEndian());
            Assert.True(span.TryReadInt32BigEndian(out int intValue));
            Assert.Equal<int>(0x11223344, intValue);

            Assert.Equal<int>(0x44332211, span.ReadInt32LittleEndian());
            Assert.True(span.TryReadInt32LittleEndian(out intValue));
            Assert.Equal<int>(0x44332211, intValue);

            Assert.Equal<ulong>(0x1122334455667788, span.ReadUInt64BigEndian());
            Assert.True(span.TryReadUInt64BigEndian(out ulong ulongValue));
            Assert.Equal<ulong>(0x1122334455667788, ulongValue);

            Assert.Equal<ulong>(0x8877665544332211, span.ReadUInt64LittleEndian());
            Assert.True(span.TryReadUInt64LittleEndian(out ulongValue));
            Assert.Equal<ulong>(0x8877665544332211, ulongValue);

            Assert.Equal<long>(0x1122334455667788, span.ReadInt64BigEndian());
            Assert.True(span.TryReadInt64BigEndian(out long longValue));
            Assert.Equal<long>(0x1122334455667788, longValue);

            Assert.Equal<long>(unchecked((long)0x8877665544332211), span.ReadInt64LittleEndian());
            Assert.True(span.TryReadInt64LittleEndian(out longValue));
            Assert.Equal<long>(unchecked((long)0x8877665544332211), longValue);
        }

        [Fact]
        public void SpanReadFail()
        {
            Span<byte> span = new byte[] { 1 };

            Assert.Equal<byte>(1, span.Read<byte>());
            Assert.True(span.TryRead(out byte byteValue));
            Assert.Equal(1, byteValue);

            TestHelpers.AssertThrows<ArgumentOutOfRangeException, byte>(span, (_span) => _span.Read<short>());
            Assert.False(span.TryRead(out short shortValue));
            TestHelpers.AssertThrows<ArgumentOutOfRangeException, byte>(span, (_span) => _span.Read<int>());
            Assert.False(span.TryRead(out int intValue));
            TestHelpers.AssertThrows<ArgumentOutOfRangeException, byte>(span, (_span) => _span.Read<long>());
            Assert.False(span.TryRead(out long longValue));

            TestHelpers.AssertThrows<ArgumentOutOfRangeException, byte>(span, (_span) => _span.Read<ushort>());
            Assert.False(span.TryRead(out ushort ushortValue));
            TestHelpers.AssertThrows<ArgumentOutOfRangeException, byte>(span, (_span) => _span.Read<uint>());
            Assert.False(span.TryRead(out uint uintValue));
            TestHelpers.AssertThrows<ArgumentOutOfRangeException, byte>(span, (_span) => _span.Read<ulong>());
            Assert.False(span.TryRead(out ulong ulongValue));
        }

        [Fact]
        public void ReadOnlySpanReadFail()
        {
            ReadOnlySpan<byte> span = new byte[] { 1 };

            Assert.Equal<byte>(1, span.Read<byte>());
            Assert.True(span.TryRead(out byte byteValue));
            Assert.Equal(1, byteValue);

            TestHelpers.AssertThrows<ArgumentOutOfRangeException, byte>(span, (_span) => _span.Read<short>());
            Assert.False(span.TryRead(out short shortValue));
            TestHelpers.AssertThrows<ArgumentOutOfRangeException, byte>(span, (_span) => _span.Read<int>());
            Assert.False(span.TryRead(out int intValue));
            TestHelpers.AssertThrows<ArgumentOutOfRangeException, byte>(span, (_span) => _span.Read<long>());
            Assert.False(span.TryRead(out long longValue));

            TestHelpers.AssertThrows<ArgumentOutOfRangeException, byte>(span, (_span) => _span.Read<ushort>());
            Assert.False(span.TryRead(out ushort ushortValue));
            TestHelpers.AssertThrows<ArgumentOutOfRangeException, byte>(span, (_span) => _span.Read<uint>());
            Assert.False(span.TryRead(out uint uintValue));
            TestHelpers.AssertThrows<ArgumentOutOfRangeException, byte>(span, (_span) => _span.Read<ulong>());
            Assert.False(span.TryRead(out ulong ulongValue));
        }

        [Fact]
        public void ReverseByteDoesNothing()
        {
            byte valueMax = byte.MaxValue;
            byte valueMin = byte.MinValue;
            sbyte signedValueMax = sbyte.MaxValue;
            sbyte signedValueMin = sbyte.MinValue;

            Assert.Equal(valueMax, valueMax.Reverse());
            Assert.Equal(valueMin, valueMin.Reverse());
            Assert.Equal(signedValueMax, signedValueMax.Reverse());
            Assert.Equal(signedValueMin, signedValueMin.Reverse());
        }

        [Fact]
        public void SpanWriteAndReadBigEndianHeterogeneousStruct()
        {
            Assert.True(BitConverter.IsLittleEndian);

            Span<byte> spanBE = new byte[Unsafe.SizeOf<TestStruct>()];

            spanBE.WriteInt16BigEndian(testStruct.S0);
            spanBE.Slice(2).WriteInt32BigEndian(testStruct.I0);
            spanBE.Slice(6).WriteInt64BigEndian(testStruct.L0);
            spanBE.Slice(14).WriteUInt16BigEndian(testStruct.US0);
            spanBE.Slice(16).WriteUInt32BigEndian(testStruct.UI0);
            spanBE.Slice(20).WriteUInt64BigEndian(testStruct.UL0);
            spanBE.Slice(28).WriteInt16BigEndian(testStruct.S1);
            spanBE.Slice(30).WriteInt32BigEndian(testStruct.I1);
            spanBE.Slice(34).WriteInt64BigEndian(testStruct.L1);
            spanBE.Slice(42).WriteUInt16BigEndian(testStruct.US1);
            spanBE.Slice(44).WriteUInt32BigEndian(testStruct.UI1);
            spanBE.Slice(48).WriteUInt64BigEndian(testStruct.UL1);

            ReadOnlySpan<byte> readOnlySpanBE = new ReadOnlySpan<byte>(spanBE.ToArray());

            var readStruct = new TestStruct
            {
                S0 = spanBE.ReadInt16BigEndian(),
                I0 = spanBE.Slice(2).ReadInt32BigEndian(),
                L0 = spanBE.Slice(6).ReadInt64BigEndian(),
                US0 = spanBE.Slice(14).ReadUInt16BigEndian(),
                UI0 = spanBE.Slice(16).ReadUInt32BigEndian(),
                UL0 = spanBE.Slice(20).ReadUInt64BigEndian(),
                S1 = spanBE.Slice(28).ReadInt16BigEndian(),
                I1 = spanBE.Slice(30).ReadInt32BigEndian(),
                L1 = spanBE.Slice(34).ReadInt64BigEndian(),
                US1 = spanBE.Slice(42).ReadUInt16BigEndian(),
                UI1 = spanBE.Slice(44).ReadUInt32BigEndian(),
                UL1 = spanBE.Slice(48).ReadUInt64BigEndian()
            };

            var readStructFromReadOnlySpan = new TestStruct
            {
                S0 = readOnlySpanBE.ReadInt16BigEndian(),
                I0 = readOnlySpanBE.Slice(2).ReadInt32BigEndian(),
                L0 = readOnlySpanBE.Slice(6).ReadInt64BigEndian(),
                US0 = readOnlySpanBE.Slice(14).ReadUInt16BigEndian(),
                UI0 = readOnlySpanBE.Slice(16).ReadUInt32BigEndian(),
                UL0 = readOnlySpanBE.Slice(20).ReadUInt64BigEndian(),
                S1 = readOnlySpanBE.Slice(28).ReadInt16BigEndian(),
                I1 = readOnlySpanBE.Slice(30).ReadInt32BigEndian(),
                L1 = readOnlySpanBE.Slice(34).ReadInt64BigEndian(),
                US1 = readOnlySpanBE.Slice(42).ReadUInt16BigEndian(),
                UI1 = readOnlySpanBE.Slice(44).ReadUInt32BigEndian(),
                UL1 = readOnlySpanBE.Slice(48).ReadUInt64BigEndian()
            };

            Assert.Equal(testStruct, readStruct);
            Assert.Equal(testStruct, readStructFromReadOnlySpan);
        }

        [Fact]
        public void SpanWriteAndReadLittleEndianHeterogeneousStruct()
        {
            Assert.True(BitConverter.IsLittleEndian);

            Span<byte> spanLE = new byte[Unsafe.SizeOf<TestStruct>()];

            spanLE.WriteInt16LittleEndian(testStruct.S0);
            spanLE.Slice(2).WriteInt32LittleEndian(testStruct.I0);
            spanLE.Slice(6).WriteInt64LittleEndian(testStruct.L0);
            spanLE.Slice(14).WriteUInt16LittleEndian(testStruct.US0);
            spanLE.Slice(16).WriteUInt32LittleEndian(testStruct.UI0);
            spanLE.Slice(20).WriteUInt64LittleEndian(testStruct.UL0);
            spanLE.Slice(28).WriteInt16LittleEndian(testStruct.S1);
            spanLE.Slice(30).WriteInt32LittleEndian(testStruct.I1);
            spanLE.Slice(34).WriteInt64LittleEndian(testStruct.L1);
            spanLE.Slice(42).WriteUInt16LittleEndian(testStruct.US1);
            spanLE.Slice(44).WriteUInt32LittleEndian(testStruct.UI1);
            spanLE.Slice(48).WriteUInt64LittleEndian(testStruct.UL1);

            ReadOnlySpan<byte> readOnlySpanLE = new ReadOnlySpan<byte>(spanLE.ToArray());

            var readStruct = new TestStruct
            {
                S0 = spanLE.ReadInt16LittleEndian(),
                I0 = spanLE.Slice(2).ReadInt32LittleEndian(),
                L0 = spanLE.Slice(6).ReadInt64LittleEndian(),
                US0 = spanLE.Slice(14).ReadUInt16LittleEndian(),
                UI0 = spanLE.Slice(16).ReadUInt32LittleEndian(),
                UL0 = spanLE.Slice(20).ReadUInt64LittleEndian(),
                S1 = spanLE.Slice(28).ReadInt16LittleEndian(),
                I1 = spanLE.Slice(30).ReadInt32LittleEndian(),
                L1 = spanLE.Slice(34).ReadInt64LittleEndian(),
                US1 = spanLE.Slice(42).ReadUInt16LittleEndian(),
                UI1 = spanLE.Slice(44).ReadUInt32LittleEndian(),
                UL1 = spanLE.Slice(48).ReadUInt64LittleEndian()
            };

            var readStructFromReadOnlySpan = new TestStruct
            {
                S0 = readOnlySpanLE.ReadInt16LittleEndian(),
                I0 = readOnlySpanLE.Slice(2).ReadInt32LittleEndian(),
                L0 = readOnlySpanLE.Slice(6).ReadInt64LittleEndian(),
                US0 = readOnlySpanLE.Slice(14).ReadUInt16LittleEndian(),
                UI0 = readOnlySpanLE.Slice(16).ReadUInt32LittleEndian(),
                UL0 = readOnlySpanLE.Slice(20).ReadUInt64LittleEndian(),
                S1 = readOnlySpanLE.Slice(28).ReadInt16LittleEndian(),
                I1 = readOnlySpanLE.Slice(30).ReadInt32LittleEndian(),
                L1 = readOnlySpanLE.Slice(34).ReadInt64LittleEndian(),
                US1 = readOnlySpanLE.Slice(42).ReadUInt16LittleEndian(),
                UI1 = readOnlySpanLE.Slice(44).ReadUInt32LittleEndian(),
                UL1 = readOnlySpanLE.Slice(48).ReadUInt64LittleEndian()
            };

            Assert.Equal(testStruct, readStruct);
            Assert.Equal(testStruct, readStructFromReadOnlySpan);
        }

        [Fact]
        public void ReadingStructFieldByFieldOrReadAndReverse()
        {
            Assert.True(BitConverter.IsLittleEndian);
            Span<byte> spanBE = new byte[Unsafe.SizeOf<TestHelpers.TestStructExplicit>()];

            var testExplicitStruct = new TestHelpers.TestStructExplicit
            {
                S0 = short.MaxValue,
                I0 = int.MaxValue,
                L0 = long.MaxValue,
                US0 = ushort.MaxValue,
                UI0 = uint.MaxValue,
                UL0 = ulong.MaxValue,
                S1 = short.MinValue,
                I1 = int.MinValue,
                L1 = long.MinValue,
                US1 = ushort.MinValue,
                UI1 = uint.MinValue,
                UL1 = ulong.MinValue
            };

            spanBE.WriteInt16BigEndian(testExplicitStruct.S0);
            spanBE.Slice(2).WriteInt32BigEndian(testExplicitStruct.I0);
            spanBE.Slice(6).WriteInt64BigEndian(testExplicitStruct.L0);
            spanBE.Slice(14).WriteUInt16BigEndian(testExplicitStruct.US0);
            spanBE.Slice(16).WriteUInt32BigEndian(testExplicitStruct.UI0);
            spanBE.Slice(20).WriteUInt64BigEndian(testExplicitStruct.UL0);
            spanBE.Slice(28).WriteInt16BigEndian(testExplicitStruct.S1);
            spanBE.Slice(30).WriteInt32BigEndian(testExplicitStruct.I1);
            spanBE.Slice(34).WriteInt64BigEndian(testExplicitStruct.L1);
            spanBE.Slice(42).WriteUInt16BigEndian(testExplicitStruct.US1);
            spanBE.Slice(44).WriteUInt32BigEndian(testExplicitStruct.UI1);
            spanBE.Slice(48).WriteUInt64BigEndian(testExplicitStruct.UL1);

            Assert.Equal(56, spanBE.Length);

            ReadOnlySpan<byte> readOnlySpanBE = new ReadOnlySpan<byte>(spanBE.ToArray());

            var readStructAndReverse = spanBE.Read<TestHelpers.TestStructExplicit>();
            if (BitConverter.IsLittleEndian)
            {
                readStructAndReverse.S0 = readStructAndReverse.S0.Reverse();
                readStructAndReverse.I0 = readStructAndReverse.I0.Reverse();
                readStructAndReverse.L0 = readStructAndReverse.L0.Reverse();
                readStructAndReverse.US0 = readStructAndReverse.US0.Reverse();
                readStructAndReverse.UI0 = readStructAndReverse.UI0.Reverse();
                readStructAndReverse.UL0 = readStructAndReverse.UL0.Reverse();
                readStructAndReverse.S1 = readStructAndReverse.S1.Reverse();
                readStructAndReverse.I1 = readStructAndReverse.I1.Reverse();
                readStructAndReverse.L1 = readStructAndReverse.L1.Reverse();
                readStructAndReverse.US1 = readStructAndReverse.US1.Reverse();
                readStructAndReverse.UI1 = readStructAndReverse.UI1.Reverse();
                readStructAndReverse.UL1 = readStructAndReverse.UL1.Reverse();
            }

            var readStructFieldByField = new TestHelpers.TestStructExplicit
            {
                S0 = spanBE.ReadInt16BigEndian(),
                I0 = spanBE.Slice(2).ReadInt32BigEndian(),
                L0 = spanBE.Slice(6).ReadInt64BigEndian(),
                US0 = spanBE.Slice(14).ReadUInt16BigEndian(),
                UI0 = spanBE.Slice(16).ReadUInt32BigEndian(),
                UL0 = spanBE.Slice(20).ReadUInt64BigEndian(),
                S1 = spanBE.Slice(28).ReadInt16BigEndian(),
                I1 = spanBE.Slice(30).ReadInt32BigEndian(),
                L1 = spanBE.Slice(34).ReadInt64BigEndian(),
                US1 = spanBE.Slice(42).ReadUInt16BigEndian(),
                UI1 = spanBE.Slice(44).ReadUInt32BigEndian(),
                UL1 = spanBE.Slice(48).ReadUInt64BigEndian()
            };

            var readStructAndReverseFromReadOnlySpan = readOnlySpanBE.Read<TestHelpers.TestStructExplicit>();
            if (BitConverter.IsLittleEndian)
            {
                readStructAndReverseFromReadOnlySpan.S0 = readStructAndReverseFromReadOnlySpan.S0.Reverse();
                readStructAndReverseFromReadOnlySpan.I0 = readStructAndReverseFromReadOnlySpan.I0.Reverse();
                readStructAndReverseFromReadOnlySpan.L0 = readStructAndReverseFromReadOnlySpan.L0.Reverse();
                readStructAndReverseFromReadOnlySpan.US0 = readStructAndReverseFromReadOnlySpan.US0.Reverse();
                readStructAndReverseFromReadOnlySpan.UI0 = readStructAndReverseFromReadOnlySpan.UI0.Reverse();
                readStructAndReverseFromReadOnlySpan.UL0 = readStructAndReverseFromReadOnlySpan.UL0.Reverse();
                readStructAndReverseFromReadOnlySpan.S1 = readStructAndReverseFromReadOnlySpan.S1.Reverse();
                readStructAndReverseFromReadOnlySpan.I1 = readStructAndReverseFromReadOnlySpan.I1.Reverse();
                readStructAndReverseFromReadOnlySpan.L1 = readStructAndReverseFromReadOnlySpan.L1.Reverse();
                readStructAndReverseFromReadOnlySpan.US1 = readStructAndReverseFromReadOnlySpan.US1.Reverse();
                readStructAndReverseFromReadOnlySpan.UI1 = readStructAndReverseFromReadOnlySpan.UI1.Reverse();
                readStructAndReverseFromReadOnlySpan.UL1 = readStructAndReverseFromReadOnlySpan.UL1.Reverse();
            }

            var readStructFieldByFieldFromReadOnlySpan = new TestHelpers.TestStructExplicit
            {
                S0 = readOnlySpanBE.ReadInt16BigEndian(),
                I0 = readOnlySpanBE.Slice(2).ReadInt32BigEndian(),
                L0 = readOnlySpanBE.Slice(6).ReadInt64BigEndian(),
                US0 = readOnlySpanBE.Slice(14).ReadUInt16BigEndian(),
                UI0 = readOnlySpanBE.Slice(16).ReadUInt32BigEndian(),
                UL0 = readOnlySpanBE.Slice(20).ReadUInt64BigEndian(),
                S1 = readOnlySpanBE.Slice(28).ReadInt16BigEndian(),
                I1 = readOnlySpanBE.Slice(30).ReadInt32BigEndian(),
                L1 = readOnlySpanBE.Slice(34).ReadInt64BigEndian(),
                US1 = readOnlySpanBE.Slice(42).ReadUInt16BigEndian(),
                UI1 = readOnlySpanBE.Slice(44).ReadUInt32BigEndian(),
                UL1 = readOnlySpanBE.Slice(48).ReadUInt64BigEndian()
            };

            Assert.Equal(testExplicitStruct, readStructAndReverse);
            Assert.Equal(testExplicitStruct, readStructFieldByField);

            Assert.Equal(testExplicitStruct, readStructAndReverseFromReadOnlySpan);
            Assert.Equal(testExplicitStruct, readStructFieldByFieldFromReadOnlySpan);
        }

        private static TestStruct testStruct = new TestStruct
        {
            S0 = short.MaxValue,
            I0 = int.MaxValue,
            L0 = long.MaxValue,
            US0 = ushort.MaxValue,
            UI0 = uint.MaxValue,
            UL0 = ulong.MaxValue,
            S1 = short.MinValue,
            I1 = int.MinValue,
            L1 = long.MinValue,
            US1 = ushort.MinValue,
            UI1 = uint.MinValue,
            UL1 = ulong.MinValue
        };

        [StructLayout(LayoutKind.Sequential)]
        private struct TestStruct
        {
            public short S0;
            public int I0;
            public long L0;
            public ushort US0;
            public uint UI0;
            public ulong UL0;
            public short S1;
            public int I1;
            public long L1;
            public ushort US1;
            public uint UI1;
            public ulong UL1;
        }
    }
}
