// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xunit;

namespace System.Buffers.Binary.Tests
{
    public static class TestHelpers
    {

        public static void Validate<T>(Span<byte> span, T value) where T : struct
        {
            T read = span.Read<T>();
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
            return spanBE;
        }

        public static Span<byte> GetSpanLE()
        {
            Span<byte> spanLE = new byte[Unsafe.SizeOf<TestStructExplicit>()];

            spanLE.WriteInt16LittleEndian(testExplicitStruct.S0);
            spanLE.Slice(2).WriteInt32LittleEndian(testExplicitStruct.I0);
            spanLE.Slice(6).WriteInt64LittleEndian(testExplicitStruct.L0);
            spanLE.Slice(14).WriteUInt16LittleEndian(testExplicitStruct.US0);
            spanLE.Slice(16).WriteUInt32LittleEndian(testExplicitStruct.UI0);
            spanLE.Slice(20).WriteUInt64LittleEndian(testExplicitStruct.UL0);
            spanLE.Slice(28).WriteInt16LittleEndian(testExplicitStruct.S1);
            spanLE.Slice(30).WriteInt32LittleEndian(testExplicitStruct.I1);
            spanLE.Slice(34).WriteInt64LittleEndian(testExplicitStruct.L1);
            spanLE.Slice(42).WriteUInt16LittleEndian(testExplicitStruct.US1);
            spanLE.Slice(44).WriteUInt32LittleEndian(testExplicitStruct.UI1);
            spanLE.Slice(48).WriteUInt64LittleEndian(testExplicitStruct.UL1);

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

