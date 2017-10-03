// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;

namespace System.Buffers.Binary
{
    public static partial class SpanBinaryExtensions
    {
        /// <summary>
        /// Writes an Int16 into a span of bytes as little endian.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteInt16LittleEndian(this Span<byte> buffer, short value)
        {
            buffer.Write(BitConverter.IsLittleEndian ? value : value.Reverse());
        }

        /// <summary>
        /// Writes an Int32 into a span of bytes as little endian.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteInt32LittleEndian(this Span<byte> buffer, int value)
        {
            buffer.Write(BitConverter.IsLittleEndian ? value : value.Reverse());
        }

        /// <summary>
        /// Writes an Int64 into a span of bytes as little endian.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteInt64LittleEndian(this Span<byte> buffer, long value)
        {
            buffer.Write(BitConverter.IsLittleEndian ? value : value.Reverse());
        }

        /// <summary>
        /// Write a UInt16 into a span of bytes as little endian.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteUInt16LittleEndian(this Span<byte> buffer, ushort value)
        {
            buffer.Write(BitConverter.IsLittleEndian ? value : value.Reverse());
        }

        /// <summary>
        /// Write a UInt32 into a span of bytes as little endian.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteUInt32LittleEndian(this Span<byte> buffer, uint value)
        {
            buffer.Write(BitConverter.IsLittleEndian ? value : value.Reverse());
        }

        /// <summary>
        /// Write a UInt64 into a span of bytes as little endian.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteUInt64LittleEndian(this Span<byte> buffer, ulong value)
        {
            buffer.Write(BitConverter.IsLittleEndian ? value : value.Reverse());
        }

        /// <summary>
        /// Writes an Int16 into a span of bytes as little endian.
        /// <returns>If the span is too small to contain the value, return false.</returns>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryWriteInt16LittleEndian(this Span<byte> buffer, short value)
        {
            return buffer.TryWrite(BitConverter.IsLittleEndian ? value : value.Reverse());
        }

        /// <summary>
        /// Writes an Int32 into a span of bytes as little endian.
        /// <returns>If the span is too small to contain the value, return false.</returns>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryWriteInt32LittleEndian(this Span<byte> buffer, int value)
        {
            return buffer.TryWrite(BitConverter.IsLittleEndian ? value : value.Reverse());
        }

        /// <summary>
        /// Writes an Int64 into a span of bytes as little endian.
        /// <returns>If the span is too small to contain the value, return false.</returns>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryWriteInt64LittleEndian(this Span<byte> buffer, long value)
        {
            return buffer.TryWrite(BitConverter.IsLittleEndian ? value : value.Reverse());
        }

        /// <summary>
        /// Write a UInt16 into a span of bytes as little endian.
        /// <returns>If the span is too small to contain the value, return false.</returns>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryWriteUInt16LittleEndian(this Span<byte> buffer, ushort value)
        {
            return buffer.TryWrite(BitConverter.IsLittleEndian ? value : value.Reverse());
        }

        /// <summary>
        /// Write a UInt32 into a span of bytes as little endian.
        /// <returns>If the span is too small to contain the value, return false.</returns>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryWriteUInt32LittleEndian(this Span<byte> buffer, uint value)
        {
            return buffer.TryWrite(BitConverter.IsLittleEndian ? value : value.Reverse());
        }

        /// <summary>
        /// Write a UInt64 into a span of bytes as little endian.
        /// <returns>If the span is too small to contain the value, return false.</returns>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryWriteUInt64LittleEndian(this Span<byte> buffer, ulong value)
        {
            return buffer.TryWrite(BitConverter.IsLittleEndian ? value : value.Reverse());
        }
    }
}
