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
    public static class SpanBinaryExtensions
    {
        public static sbyte Reverse(this sbyte value) { throw null; }
        public static byte Reverse(this byte value) { throw null; }
        public static short Reverse(this short value) { throw null; }
        public static ushort Reverse(this ushort value) { throw null; }
        public static int Reverse(this int value) { throw null; }
        public static uint Reverse(this uint value) { throw null; }
        public static long Reverse(this long value) { throw null; }
        public static ulong Reverse(this ulong value) { throw null; }

        public static T Read<T>(this Span<byte> buffer) where T : struct { throw null; }
        public static T Read<T>(this ReadOnlySpan<byte> buffer) where T : struct { throw null; }
        public static bool TryRead<T>(this Span<byte> buffer, out T value) where T : struct { throw null; }
        public static bool TryRead<T>(this ReadOnlySpan<byte> buffer, out T value) where T : struct { throw null; }

        public static short ReadInt16LittleEndian(this ReadOnlySpan<byte> buffer) { throw null; }
        public static int ReadInt32LittleEndian(this ReadOnlySpan<byte> buffer) { throw null; }
        public static long ReadInt64LittleEndian(this ReadOnlySpan<byte> buffer) { throw null; }
        public static ushort ReadUInt16LittleEndian(this ReadOnlySpan<byte> buffer) { throw null; }
        public static uint ReadUInt32LittleEndian(this ReadOnlySpan<byte> buffer) { throw null; }
        public static ulong ReadUInt64LittleEndian(this ReadOnlySpan<byte> buffer) { throw null; }

        public static short ReadInt16LittleEndian(this Span<byte> buffer) { throw null; }
        public static int ReadInt32LittleEndian(this Span<byte> buffer) { throw null; }
        public static long ReadInt64LittleEndian(this Span<byte> buffer) { throw null; }
        public static ushort ReadUInt16LittleEndian(this Span<byte> buffer) { throw null; }
        public static uint ReadUInt32LittleEndian(this Span<byte> buffer) { throw null; }
        public static ulong ReadUInt64LittleEndian(this Span<byte> buffer) { throw null; }

        public static bool TryReadInt16LittleEndian(this ReadOnlySpan<byte> buffer, out short value) { throw null; }
        public static bool TryReadInt32LittleEndian(this ReadOnlySpan<byte> buffer, out int value) { throw null; }
        public static bool TryReadInt64LittleEndian(this ReadOnlySpan<byte> buffer, out long value) { throw null; }
        public static bool TryReadUInt16LittleEndian(this ReadOnlySpan<byte> buffer, out ushort value) { throw null; }
        public static bool TryReadUInt32LittleEndian(this ReadOnlySpan<byte> buffer, out uint value) { throw null; }
        public static bool TryReadUInt64LittleEndian(this ReadOnlySpan<byte> buffer, out ulong value) { throw null; }

        public static bool TryReadInt16LittleEndian(this Span<byte> buffer, out short value) { throw null; }
        public static bool TryReadInt32LittleEndian(this Span<byte> buffer, out int value) { throw null; }
        public static bool TryReadInt64LittleEndian(this Span<byte> buffer, out long value) { throw null; }
        public static bool TryReadUInt16LittleEndian(this Span<byte> buffer, out ushort value) { throw null; }
        public static bool TryReadUInt32LittleEndian(this Span<byte> buffer, out uint value) { throw null; }
        public static bool TryReadUInt64LittleEndian(this Span<byte> buffer, out ulong value) { throw null; }

        public static short ReadInt16BigEndian(this ReadOnlySpan<byte> buffer) { throw null; }
        public static int ReadInt32BigEndian(this ReadOnlySpan<byte> buffer) { throw null; }
        public static long ReadInt64BigEndian(this ReadOnlySpan<byte> buffer) { throw null; }
        public static ushort ReadUInt16BigEndian(this ReadOnlySpan<byte> buffer) { throw null; }
        public static uint ReadUInt32BigEndian(this ReadOnlySpan<byte> buffer) { throw null; }
        public static ulong ReadUInt64BigEndian(this ReadOnlySpan<byte> buffer) { throw null; }

        public static short ReadInt16BigEndian(this Span<byte> buffer) { throw null; }
        public static int ReadInt32BigEndian(this Span<byte> buffer) { throw null; }
        public static long ReadInt64BigEndian(this Span<byte> buffer) { throw null; }
        public static ushort ReadUInt16BigEndian(this Span<byte> buffer) { throw null; }
        public static uint ReadUInt32BigEndian(this Span<byte> buffer) { throw null; }
        public static ulong ReadUInt64BigEndian(this Span<byte> buffer) { throw null; }

        public static bool TryReadInt16BigEndian(this ReadOnlySpan<byte> buffer, out short value) { throw null; }
        public static bool TryReadInt32BigEndian(this ReadOnlySpan<byte> buffer, out int value) { throw null; }
        public static bool TryReadInt64BigEndian(this ReadOnlySpan<byte> buffer, out long value) { throw null; }
        public static bool TryReadUInt16BigEndian(this ReadOnlySpan<byte> buffer, out ushort value) { throw null; }
        public static bool TryReadUInt32BigEndian(this ReadOnlySpan<byte> buffer, out uint value) { throw null; }
        public static bool TryReadUInt64BigEndian(this ReadOnlySpan<byte> buffer, out ulong value) { throw null; }

        public static bool TryReadInt16BigEndian(this Span<byte> buffer, out short value) { throw null; }
        public static bool TryReadInt32BigEndian(this Span<byte> buffer, out int value) { throw null; }
        public static bool TryReadInt64BigEndian(this Span<byte> buffer, out long value) { throw null; }
        public static bool TryReadUInt16BigEndian(this Span<byte> buffer, out ushort value) { throw null; }
        public static bool TryReadUInt32BigEndian(this Span<byte> buffer, out uint value) { throw null; }
        public static bool TryReadUInt64BigEndian(this Span<byte> buffer, out ulong value) { throw null; }

        public static void Write<T>(this Span<byte> buffer, T value) where T : struct { throw null; }
        public static bool TryWrite<T>(this Span<byte> buffer, T value) where T : struct { throw null; }

        public static void WriteInt16LittleEndian(this Span<byte> buffer, short value) { throw null; }
        public static void WriteInt32LittleEndian(this Span<byte> buffer, int value) { throw null; }
        public static void WriteInt64LittleEndian(this Span<byte> buffer, long value) { throw null; }
        public static void WriteUInt16LittleEndian(this Span<byte> buffer, ushort value) { throw null; }
        public static void WriteUInt32LittleEndian(this Span<byte> buffer, uint value) { throw null; }
        public static void WriteUInt64LittleEndian(this Span<byte> buffer, ulong value) { throw null; }

        public static bool TryWriteInt16LittleEndian(this Span<byte> buffer, short value) { throw null; }
        public static bool TryWriteInt32LittleEndian(this Span<byte> buffer, int value) { throw null; }
        public static bool TryWriteInt64LittleEndian(this Span<byte> buffer, long value) { throw null; }
        public static bool TryWriteUInt16LittleEndian(this Span<byte> buffer, ushort value) { throw null; }
        public static bool TryWriteUInt32LittleEndian(this Span<byte> buffer, uint value) { throw null; }
        public static bool TryWriteUInt64LittleEndian(this Span<byte> buffer, ulong value) { throw null; }

        public static void WriteInt16BigEndian(this Span<byte> buffer, short value) { throw null; }
        public static void WriteInt32BigEndian(this Span<byte> buffer, int value) { throw null; }
        public static void WriteInt64BigEndian(this Span<byte> buffer, long value) { throw null; }
        public static void WriteUInt16BigEndian(this Span<byte> buffer, ushort value) { throw null; }
        public static void WriteUInt32BigEndian(this Span<byte> buffer, uint value) { throw null; }
        public static void WriteUInt64BigEndian(this Span<byte> buffer, ulong value) { throw null; }

        public static bool TryWriteInt16BigEndian(this Span<byte> buffer, short value) { throw null; }
        public static bool TryWriteInt32BigEndian(this Span<byte> buffer, int value) { throw null; }
        public static bool TryWriteInt64BigEndian(this Span<byte> buffer, long value) { throw null; }
        public static bool TryWriteUInt16BigEndian(this Span<byte> buffer, ushort value) { throw null; }
        public static bool TryWriteUInt32BigEndian(this Span<byte> buffer, uint value) { throw null; }
        public static bool TryWriteUInt64BigEndian(this Span<byte> buffer, ulong value) { throw null; }
    }
}
