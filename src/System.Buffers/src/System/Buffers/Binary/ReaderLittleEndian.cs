// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;

namespace System.Buffers.Binary
{
    public static partial class SpanBinaryExtensions
    {
        #region ReadLittleEndianROSpan
        /// <summary>
        /// Reads an Int16 out of a read-only span of bytes as little endian.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short ReadInt16LittleEndian(this ReadOnlySpan<byte> buffer)
        {
            short result = buffer.Read<short>();
            if (!BitConverter.IsLittleEndian)
            {
                result = result.Reverse();
            }
            return result;
        }

        /// <summary>
        /// Reads an Int32 out of a read-only span of bytes as little endian.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReadInt32LittleEndian(this ReadOnlySpan<byte> buffer)
        {
            int result = buffer.Read<int>();
            if (!BitConverter.IsLittleEndian)
            {
                result = result.Reverse();
            }
            return result;
        }

        /// <summary>
        /// Reads an Int64 out of a read-only span of bytes as little endian.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long ReadInt64LittleEndian(this ReadOnlySpan<byte> buffer)
        {
            long result = buffer.Read<long>();
            if (!BitConverter.IsLittleEndian)
            {
                result = result.Reverse();
            }
            return result;
        }

        /// <summary>
        /// Reads a UInt16 out of a read-only span of bytes as little endian.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort ReadUInt16LittleEndian(this ReadOnlySpan<byte> buffer)
        {
            ushort result = buffer.Read<ushort>();
            if (!BitConverter.IsLittleEndian)
            {
                result = result.Reverse();
            }
            return result;
        }

        /// <summary>
        /// Reads a UInt32 out of a read-only span of bytes as little endian.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ReadUInt32LittleEndian(this ReadOnlySpan<byte> buffer)
        {
            uint result = buffer.Read<uint>();
            if (!BitConverter.IsLittleEndian)
            {
                result = result.Reverse();
            }
            return result;
        }

        /// <summary>
        /// Reads a UInt64 out of a read-only span of bytes as little endian.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong ReadUInt64LittleEndian(this ReadOnlySpan<byte> buffer)
        {
            ulong result = buffer.Read<ulong>();
            if (!BitConverter.IsLittleEndian)
            {
                result = result.Reverse();
            }
            return result;
        }
        #endregion

        #region ReadLittleEndianSpan
        /// <summary>
        /// Reads an Int16 out of a span of bytes as little endian.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short ReadInt16LittleEndian(this Span<byte> buffer)
        {
            short result = buffer.Read<short>();
            if (!BitConverter.IsLittleEndian)
            {
                result = result.Reverse();
            }
            return result;
        }

        /// <summary>
        /// Reads an Int32 out of a span of bytes as little endian.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReadInt32LittleEndian(this Span<byte> buffer)
        {
            int result = buffer.Read<int>();
            if (!BitConverter.IsLittleEndian)
            {
                result = result.Reverse();
            }
            return result;
        }

        /// <summary>
        /// Reads an Int64 out of a span of bytes as little endian.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long ReadInt64LittleEndian(this Span<byte> buffer)
        {
            long result = buffer.Read<long>();
            if (!BitConverter.IsLittleEndian)
            {
                result = result.Reverse();
            }
            return result;
        }

        /// <summary>
        /// Reads a UInt16 out of a span of bytes as little endian.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort ReadUInt16LittleEndian(this Span<byte> buffer)
        {
            ushort result = buffer.Read<ushort>();
            if (!BitConverter.IsLittleEndian)
            {
                result = result.Reverse();
            }
            return result;
        }

        /// <summary>
        /// Reads a UInt32 out of a span of bytes as little endian.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ReadUInt32LittleEndian(this Span<byte> buffer)
        {
            uint result = buffer.Read<uint>();
            if (!BitConverter.IsLittleEndian)
            {
                result = result.Reverse();
            }
            return result;
        }

        /// <summary>
        /// Reads a UInt64 out of a span of bytes as little endian.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong ReadUInt64LittleEndian(this Span<byte> buffer)
        {
            ulong result = buffer.Read<ulong>();
            if (!BitConverter.IsLittleEndian)
            {
                result = result.Reverse();
            }
            return result;
        }
        #endregion

        #region TryReadLittleEndianSpan
        /// <summary>
        /// Reads an Int16 out of a span of bytes as little endian.
        /// <returns>If the span is too small to contain an Int16, return false.</returns>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryReadInt16LittleEndian(this Span<byte> buffer, out short value)
        {
            bool success = buffer.TryRead(out value);
            if (!BitConverter.IsLittleEndian)
            {
                value = value.Reverse();
            }
            return success;
        }

        /// <summary>
        /// Reads an Int32 out of a span of bytes as little endian.
        /// <returns>If the span is too small to contain an Int32, return false.</returns>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryReadInt32LittleEndian(this Span<byte> buffer, out int value)
        {
            bool success = buffer.TryRead(out value);
            if (!BitConverter.IsLittleEndian)
            {
                value = value.Reverse();
            }
            return success;
        }

        /// <summary>
        /// Reads an Int64 out of a span of bytes as little endian.
        /// <returns>If the span is too small to contain an Int64, return false.</returns>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryReadInt64LittleEndian(this Span<byte> buffer, out long value)
        {
            bool success = buffer.TryRead(out value);
            if (!BitConverter.IsLittleEndian)
            {
                value = value.Reverse();
            }
            return success;
        }

        /// <summary>
        /// Reads a UInt16 out of a span of bytes as little endian.
        /// <returns>If the span is too small to contain a UInt16, return false.</returns>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryReadUInt16LittleEndian(this Span<byte> buffer, out ushort value)
        {
            bool success = buffer.TryRead(out value);
            if (!BitConverter.IsLittleEndian)
            {
                value = value.Reverse();
            }
            return success;
        }

        /// <summary>
        /// Reads a UInt32 out of a span of bytes as little endian.
        /// <returns>If the span is too small to contain a UInt32, return false.</returns>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryReadUInt32LittleEndian(this Span<byte> buffer, out uint value)
        {
            bool success = buffer.TryRead(out value);
            if (!BitConverter.IsLittleEndian)
            {
                value = value.Reverse();
            }
            return success;
        }

        /// <summary>
        /// Reads a UInt64 out of a span of bytes as little endian.
        /// <returns>If the span is too small to contain a UInt64, return false.</returns>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryReadUInt64LittleEndian(this Span<byte> buffer, out ulong value)
        {
            bool success = buffer.TryRead(out value);
            if (!BitConverter.IsLittleEndian)
            {
                value = value.Reverse();
            }
            return success;
        }
        #endregion

        #region TryReadLittleEndianROSpan
        /// <summary>
        /// Reads an Int16 out of a read-only span of bytes as little endian.
        /// <returns>If the span is too small to contain an Int16, return false.</returns>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryReadInt16LittleEndian(this ReadOnlySpan<byte> buffer, out short value)
        {
            bool success = buffer.TryRead(out value);
            if (!BitConverter.IsLittleEndian)
            {
                value = value.Reverse();
            }
            return success;
        }

        /// <summary>
        /// Reads an Int32 out of a read-only span of bytes as little endian.
        /// <returns>If the span is too small to contain an Int32, return false.</returns>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryReadInt32LittleEndian(this ReadOnlySpan<byte> buffer, out int value)
        {
            bool success = buffer.TryRead(out value);
            if (!BitConverter.IsLittleEndian)
            {
                value = value.Reverse();
            }
            return success;
        }

        /// <summary>
        /// Reads an Int64 out of a read-only span of bytes as little endian.
        /// <returns>If the span is too small to contain an Int64, return false.</returns>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryReadInt64LittleEndian(this ReadOnlySpan<byte> buffer, out long value)
        {
            bool success = buffer.TryRead(out value);
            if (!BitConverter.IsLittleEndian)
            {
                value = value.Reverse();
            }
            return success;
        }

        /// <summary>
        /// Reads a UInt16 out of a read-only span of bytes as little endian.
        /// <returns>If the span is too small to contain a UInt16, return false.</returns>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryReadUInt16LittleEndian(this ReadOnlySpan<byte> buffer, out ushort value)
        {
            bool success = buffer.TryRead(out value);
            if (!BitConverter.IsLittleEndian)
            {
                value = value.Reverse();
            }
            return success;
        }

        /// <summary>
        /// Reads a UInt32 out of a read-only span of bytes as little endian.
        /// <returns>If the span is too small to contain a UInt32, return false.</returns>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryReadUInt32LittleEndian(this ReadOnlySpan<byte> buffer, out uint value)
        {
            bool success = buffer.TryRead(out value);
            if (!BitConverter.IsLittleEndian)
            {
                value = value.Reverse();
            }
            return success;
        }

        /// <summary>
        /// Reads a UInt64 out of a read-only span of bytes as little endian.
        /// <returns>If the span is too small to contain a UInt64, return false.</returns>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryReadUInt64LittleEndian(this ReadOnlySpan<byte> buffer, out ulong value)
        {
            bool success = buffer.TryRead(out value);
            if (!BitConverter.IsLittleEndian)
            {
                value = value.Reverse();
            }
            return success;
        }
        #endregion
    }
}
