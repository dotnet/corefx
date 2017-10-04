// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xunit;

using static System.Buffers.Binary.BinaryPrimitives;

namespace System.Buffers.Binary.Tests
{
    public static class TestHelpers
    {

        public static void Validate<T>(Span<byte> span, T value) where T : struct
        {
            T read = ReadMachineEndian<T>(span);
            Assert.Equal(value, read);
            span.Clear();
        }

        public delegate void AssertThrowsAction<T>(Span<T> span);

        // Cannot use standard Assert.Throws() when testing Span - Span and closures don't get along.
        public static void AssertThrows<E, T>(Span<T> span, AssertThrowsAction<T> action) where E:Exception
        {
            try
            {
                action(span);
                Assert.False(true, "Expected exception: " + typeof(E).GetType());
            }
            catch (E)
            {
            }
            catch (Exception wrongException)
            {
                Assert.False(true, "Wrong exception thrown: Expected " + typeof(E).GetType() + ": Actual: " + wrongException.GetType());
            }
        }

        public delegate void AssertThrowsActionReadOnly<T>(ReadOnlySpan<T> span);

        // Cannot use standard Assert.Throws() when testing Span - Span and closures don't get along.
        public static void AssertThrows<E, T>(ReadOnlySpan<T> span, AssertThrowsActionReadOnly<T> action) where E:Exception
        {
            try
            {
                action(span);
                Assert.False(true, "Expected exception: " + typeof(E).GetType());
            }
            catch (E)
            {
            }
            catch (Exception wrongException)
            {
                Assert.False(true, "Wrong exception thrown: Expected " + typeof(E).GetType() + ": Actual: " + wrongException.GetType());
            }
        }

        public static TestStructExplicit testExplicitStruct = new TestStructExplicit
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

        public static Span<byte> GetSpanBE()
        {
            Span<byte> spanBE = new byte[Unsafe.SizeOf<TestStructExplicit>()];

            WriteInt16BigEndian(spanBE, testExplicitStruct.S0);
            WriteInt32BigEndian(spanBE.Slice(2), testExplicitStruct.I0);
            WriteInt64BigEndian(spanBE.Slice(6), testExplicitStruct.L0);
            WriteUInt16BigEndian(spanBE.Slice(14), testExplicitStruct.US0);
            WriteUInt32BigEndian(spanBE.Slice(16), testExplicitStruct.UI0);
            WriteUInt64BigEndian(spanBE.Slice(20), testExplicitStruct.UL0);
            WriteInt16BigEndian(spanBE.Slice(28), testExplicitStruct.S1);
            WriteInt32BigEndian(spanBE.Slice(30), testExplicitStruct.I1);
            WriteInt64BigEndian(spanBE.Slice(34), testExplicitStruct.L1);
            WriteUInt16BigEndian(spanBE.Slice(42), testExplicitStruct.US1);
            WriteUInt32BigEndian(spanBE.Slice(44), testExplicitStruct.UI1);
            WriteUInt64BigEndian(spanBE.Slice(48), testExplicitStruct.UL1);

            Assert.Equal(56, spanBE.Length);
            return spanBE;
        }

        public static Span<byte> GetSpanLE()
        {
            Span<byte> spanLE = new byte[Unsafe.SizeOf<TestStructExplicit>()];

            WriteInt16LittleEndian(spanLE, testExplicitStruct.S0);
            WriteInt32LittleEndian(spanLE.Slice(2), testExplicitStruct.I0);
            WriteInt64LittleEndian( spanLE.Slice(6), testExplicitStruct.L0);
            WriteUInt16LittleEndian(spanLE.Slice(14), testExplicitStruct.US0);
            WriteUInt32LittleEndian(spanLE.Slice(16), testExplicitStruct.UI0);
            WriteUInt64LittleEndian(spanLE.Slice(20), testExplicitStruct.UL0);
            WriteInt16LittleEndian(spanLE.Slice(28), testExplicitStruct.S1);
            WriteInt32LittleEndian(spanLE.Slice(30), testExplicitStruct.I1);
            WriteInt64LittleEndian(spanLE.Slice(34), testExplicitStruct.L1);
            WriteUInt16LittleEndian(spanLE.Slice(42), testExplicitStruct.US1);
            WriteUInt32LittleEndian(spanLE.Slice(44), testExplicitStruct.UI1);
            WriteUInt64LittleEndian(spanLE.Slice(48), testExplicitStruct.UL1);

            Assert.Equal(56, spanLE.Length);
            return spanLE;
        }
        
        [StructLayout(LayoutKind.Explicit)]
        public struct TestStructExplicit
        {
            [FieldOffset(0)]
            public short S0;
            [FieldOffset(2)]
            public int I0;
            [FieldOffset(6)]
            public long L0;
            [FieldOffset(14)]
            public ushort US0;
            [FieldOffset(16)]
            public uint UI0;
            [FieldOffset(20)]
            public ulong UL0;
            [FieldOffset(28)]
            public short S1;
            [FieldOffset(30)]
            public int I1;
            [FieldOffset(34)]
            public long L1;
            [FieldOffset(42)]
            public ushort US1;
            [FieldOffset(44)]
            public uint UI1;
            [FieldOffset(48)]
            public ulong UL1;
        }
    }
}

