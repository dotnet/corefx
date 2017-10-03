// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;

namespace System.Buffers.Binary
{
    public static partial class SpanBinaryExtensions
    {
        /// <summary>
        /// Writes an Int16 into a span of bytes as big endian.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteInt16BigEndian(this Span<byte> buffer, short value)
        {
            buffer.Write(BitConverter.IsLittleEndian ? value.Reverse() : value);
        }

        /// <summary>
        /// Writes an Int32 into a span of bytes as big endian.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteInt32BigEndian(this Span<byte> buffer, int value)
        {
            buffer.Write(BitConverter.IsLittleEndian ? value.Reverse() : value);
        }

        /// <summary>
        /// Writes an Int64 into a span of bytes as big endian.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteInt64BigEndian(this Span<byte> buffer, long value)
        {
            buffer.Write(BitConverter.IsLittleEndian ? value.Reverse() : value);
        }

        /// <summary>
        /// Write a UInt16 into a span of bytes as big endian.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteUInt16BigEndian(this Span<byte> buffer, ushort value)
        {
            buffer.Write(BitConverter.IsLittleEndian ? value.Reverse() : value);
        }

        /// <summary>
        /// Write a UInt32 into a span of bytes as big endian.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteUInt32BigEndian(this Span<byte> buffer, uint value)
        {
            buffer.Write(BitConverter.IsLittleEndian ? value.Reverse() : value);
        }

        /// <summary>
        /// Write a UInt64 into a span of bytes as big endian.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteUInt64BigEndian(this Span<byte> buffer, ulong value)
        {
            buffer.Write(BitConverter.IsLittleEndian ? value.Reverse() : value);
        }

        /// <summary>
        /// Writes an Int16 into a span of bytes as big endian.
        /// <returns>If the span is too small to contain the value, return false.</returns>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryWriteInt16BigEndian(this Span<byte> buffer, short value)
        {
            return buffer.TryWrite(BitConverter.IsLittleEndian ? value.Reverse() : value);
        }

        /// <summary>
        /// Writes an Int32 into a span of bytes as big endian.
        /// <returns>If the span is too small to contain the value, return false.</returns>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryWriteInt32BigEndian(this Span<byte> buffer, int value)
        {
            return buffer.TryWrite(BitConverter.IsLittleEndian ? value.Reverse() : value);
        }

        /// <summary>
        /// Writes an Int64 into a span of bytes as big endian.
        /// <returns>If the span is too small to contain the value, return false.</returns>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryWriteInt64BigEndian(this Span<byte> buffer, long value)
        {
            return buffer.TryWrite(BitConverter.IsLittleEndian ? value.Reverse() : value);
        }

        /// <summary>
        /// Write a UInt16 into a span of bytes as big endian.
        /// <returns>If the span is too small to contain the value, return false.</returns>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryWriteUInt16BigEndian(this Span<byte> buffer, ushort value)
        {
            return buffer.TryWrite(BitConverter.IsLittleEndian ? value.Reverse() : value);
        }

        /// <summary>
        /// Write a UInt32 into a span of bytes as big endian.
        /// <returns>If the span is too small to contain the value, return false.</returns>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryWriteUInt32BigEndian(this Span<byte> buffer, uint value)
        {
            return buffer.TryWrite(BitConverter.IsLittleEndian ? value.Reverse() : value);
        }

        /// <summary>
        /// Write a UInt64 into a span of bytes as big endian.
        /// <returns>If the span is too small to contain the value, return false.</returns>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryWriteUInt64BigEndian(this Span<byte> buffer, ulong value)
        {
            return buffer.TryWrite(BitConverter.IsLittleEndian ? value.Reverse() : value);
        }
    }
}
