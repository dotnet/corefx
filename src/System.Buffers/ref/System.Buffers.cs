// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------

namespace System.Buffers
{
    public abstract class ArrayPool<T>
    {
        public static ArrayPool<T> Shared { get { throw null; } }
        public static ArrayPool<T> Create() { throw null; }
        public static ArrayPool<T> Create(int maxArrayLength, int maxArraysPerBucket) { throw null; }
        public abstract T[] Rent(int minimumLength);
        public abstract void Return(T[] array, bool clearArray = false);
    }
}

namespace System.Buffers.Binary
{
    [System.CLSCompliantAttribute(false)]
    public static class BinaryPrimitives
    {
        public static sbyte ReverseEndianness(sbyte value) { throw null; }
        public static byte ReverseEndianness(byte value) { throw null; }
        public static short ReverseEndianness(short value) { throw null; }
        public static ushort ReverseEndianness(ushort value) { throw null; }
        public static int ReverseEndianness(int value) { throw null; }
        public static uint ReverseEndianness(uint value) { throw null; }
        public static long ReverseEndianness(long value) { throw null; }
        public static ulong ReverseEndianness(ulong value) { throw null; }

        public static T ReadMachineEndian<T>(ReadOnlySpan<byte> buffer) where T : struct { throw null; }
        public static bool TryReadMachineEndian<T>(ReadOnlySpan<byte> buffer, out T value) where T : struct { throw null; }

        public static short ReadInt16LittleEndian(ReadOnlySpan<byte> buffer) { throw null; }
        public static int ReadInt32LittleEndian(ReadOnlySpan<byte> buffer) { throw null; }
        public static long ReadInt64LittleEndian(ReadOnlySpan<byte> buffer) { throw null; }
        public static ushort ReadUInt16LittleEndian(ReadOnlySpan<byte> buffer) { throw null; }
        public static uint ReadUInt32LittleEndian(ReadOnlySpan<byte> buffer) { throw null; }
        public static ulong ReadUInt64LittleEndian(ReadOnlySpan<byte> buffer) { throw null; }

        public static bool TryReadInt16LittleEndian(ReadOnlySpan<byte> buffer, out short value) { throw null; }
        public static bool TryReadInt32LittleEndian(ReadOnlySpan<byte> buffer, out int value) { throw null; }
        public static bool TryReadInt64LittleEndian(ReadOnlySpan<byte> buffer, out long value) { throw null; }
        public static bool TryReadUInt16LittleEndian(ReadOnlySpan<byte> buffer, out ushort value) { throw null; }
        public static bool TryReadUInt32LittleEndian(ReadOnlySpan<byte> buffer, out uint value) { throw null; }
        public static bool TryReadUInt64LittleEndian(ReadOnlySpan<byte> buffer, out ulong value) { throw null; }

        public static short ReadInt16BigEndian(ReadOnlySpan<byte> buffer) { throw null; }
        public static int ReadInt32BigEndian(ReadOnlySpan<byte> buffer) { throw null; }
        public static long ReadInt64BigEndian(ReadOnlySpan<byte> buffer) { throw null; }
        public static ushort ReadUInt16BigEndian(ReadOnlySpan<byte> buffer) { throw null; }
        public static uint ReadUInt32BigEndian(ReadOnlySpan<byte> buffer) { throw null; }
        public static ulong ReadUInt64BigEndian(ReadOnlySpan<byte> buffer) { throw null; }

        public static bool TryReadInt16BigEndian(ReadOnlySpan<byte> buffer, out short value) { throw null; }
        public static bool TryReadInt32BigEndian(ReadOnlySpan<byte> buffer, out int value) { throw null; }
        public static bool TryReadInt64BigEndian(ReadOnlySpan<byte> buffer, out long value) { throw null; }
        public static bool TryReadUInt16BigEndian(ReadOnlySpan<byte> buffer, out ushort value) { throw null; }
        public static bool TryReadUInt32BigEndian(ReadOnlySpan<byte> buffer, out uint value) { throw null; }
        public static bool TryReadUInt64BigEndian(ReadOnlySpan<byte> buffer, out ulong value) { throw null; }

        public static void WriteMachineEndian<T>(ref Span<byte> buffer, T value) where T : struct { throw null; }
        public static bool TryWriteMachineEndian<T>(ref Span<byte> buffer, T value) where T : struct { throw null; }

        public static void WriteInt16LittleEndian(Span<byte> buffer, short value) { throw null; }
        public static void WriteInt32LittleEndian(Span<byte> buffer, int value) { throw null; }
        public static void WriteInt64LittleEndian(Span<byte> buffer, long value) { throw null; }
        public static void WriteUInt16LittleEndian(Span<byte> buffer, ushort value) { throw null; }
        public static void WriteUInt32LittleEndian(Span<byte> buffer, uint value) { throw null; }
        public static void WriteUInt64LittleEndian(Span<byte> buffer, ulong value) { throw null; }

        public static bool TryWriteInt16LittleEndian(Span<byte> buffer, short value) { throw null; }
        public static bool TryWriteInt32LittleEndian(Span<byte> buffer, int value) { throw null; }
        public static bool TryWriteInt64LittleEndian(Span<byte> buffer, long value) { throw null; }
        public static bool TryWriteUInt16LittleEndian(Span<byte> buffer, ushort value) { throw null; }
        public static bool TryWriteUInt32LittleEndian(Span<byte> buffer, uint value) { throw null; }
        public static bool TryWriteUInt64LittleEndian(Span<byte> buffer, ulong value) { throw null; }

        public static void WriteInt16BigEndian(Span<byte> buffer, short value) { throw null; }
        public static void WriteInt32BigEndian(Span<byte> buffer, int value) { throw null; }
        public static void WriteInt64BigEndian(Span<byte> buffer, long value) { throw null; }
        public static void WriteUInt16BigEndian(Span<byte> buffer, ushort value) { throw null; }
        public static void WriteUInt32BigEndian(Span<byte> buffer, uint value) { throw null; }
        public static void WriteUInt64BigEndian(Span<byte> buffer, ulong value) { throw null; }

        public static bool TryWriteInt16BigEndian(Span<byte> buffer, short value) { throw null; }
        public static bool TryWriteInt32BigEndian(Span<byte> buffer, int value) { throw null; }
        public static bool TryWriteInt64BigEndian(Span<byte> buffer, long value) { throw null; }
        public static bool TryWriteUInt16BigEndian(Span<byte> buffer, ushort value) { throw null; }
        public static bool TryWriteUInt32BigEndian(Span<byte> buffer, uint value) { throw null; }
        public static bool TryWriteUInt64BigEndian(Span<byte> buffer, ulong value) { throw null; }
    }
}
