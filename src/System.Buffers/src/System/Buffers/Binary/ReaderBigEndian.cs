// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;

namespace System.Buffers.Binary
{
    public static partial class SpanBinaryExtensions
    {
        #region ReadBigEndianROSpan
        /// <summary>
        /// Reads an Int16 out of a read-only span of bytes as big endian.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short ReadInt16BigEndian(this ReadOnlySpan<byte> buffer)
        {
            short result = buffer.Read<short>();
            if (BitConverter.IsLittleEndian)
            {
                result = result.Reverse();
            }
            return result;
        }

        /// <summary>
        /// Reads an Int32 out of a read-only span of bytes as big endian.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReadInt32BigEndian(this ReadOnlySpan<byte> buffer)
        {
            int result = buffer.Read<int>();
            if (BitConverter.IsLittleEndian)
            {
                result = result.Reverse();
            }
            return result;
        }

        /// <summary>
        /// Reads an Int64 out of a read-only span of bytes as big endian.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long ReadInt64BigEndian(this ReadOnlySpan<byte> buffer)
        {
            long result = buffer.Read<long>();
            if (BitConverter.IsLittleEndian)
            {
                result = result.Reverse();
            }
            return result;
        }

        /// <summary>
        /// Reads a UInt16 out of a read-only span of bytes as big endian.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort ReadUInt16BigEndian(this ReadOnlySpan<byte> buffer)
        {
            ushort result = buffer.Read<ushort>();
            if (BitConverter.IsLittleEndian)
            {
                result = result.Reverse();
            }
            return result;
        }

        /// <summary>
        /// Reads a UInt32 out of a read-only span of bytes as big endian.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ReadUInt32BigEndian(this ReadOnlySpan<byte> buffer)
        {
            uint result = buffer.Read<uint>();
            if (BitConverter.IsLittleEndian)
            {
                result = result.Reverse();
            }
            return result;
        }

        /// <summary>
        /// Reads a UInt64 out of a read-only span of bytes as big endian.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong ReadUInt64BigEndian(this ReadOnlySpan<byte> buffer)
        {
            ulong result = buffer.Read<ulong>();
            if (BitConverter.IsLittleEndian)
            {
                result = result.Reverse();
            }
            return result;
        }
        #endregion

        #region ReadBigEndianSpan
        /// <summary>
        /// Reads an Int16 out of a span of bytes as big endian.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short ReadInt16BigEndian(this Span<byte> buffer)
        {
            short result = buffer.Read<short>();
            if (BitConverter.IsLittleEndian)
            {
                result = result.Reverse();
            }
            return result;
        }

        /// <summary>
        /// Reads an Int32 out of a span of bytes as big endian.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReadInt32BigEndian(this Span<byte> buffer)
        {
            int result = buffer.Read<int>();
            if (BitConverter.IsLittleEndian)
            {
                result = result.Reverse();
            }
            return result;
        }

        /// <summary>
        /// Reads an Int64 out of a span of bytes as big endian.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long ReadInt64BigEndian(this Span<byte> buffer)
        {
            long result = buffer.Read<long>();
            if (BitConverter.IsLittleEndian)
            {
                result = result.Reverse();
            }
            return result;
        }

        /// <summary>
        /// Reads a UInt16 out of a span of bytes as big endian.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort ReadUInt16BigEndian(this Span<byte> buffer)
        {
            ushort result = buffer.Read<ushort>();
            if (BitConverter.IsLittleEndian)
            {
                result = result.Reverse();
            }
            return result;
        }

        /// <summary>
        /// Reads a UInt32 out of a span of bytes as big endian.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ReadUInt32BigEndian(this Span<byte> buffer)
        {
            uint result = buffer.Read<uint>();
            if (BitConverter.IsLittleEndian)
            {
                result = result.Reverse();
            }
            return result;
        }

        /// <summary>
        /// Reads a UInt64 out of a span of bytes as big endian.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong ReadUInt64BigEndian(this Span<byte> buffer)
        {
            ulong result = buffer.Read<ulong>();
            if (BitConverter.IsLittleEndian)
            {
                result = result.Reverse();
            }
            return result;
        }
        #endregion

        #region TryReadBigEndianSpan
        /// <summary>
        /// Reads an Int16 out of a span of bytes as big endian.
        /// <returns>If the span is too small to contain an Int16, return false.</returns>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryReadInt16BigEndian(this Span<byte> buffer, out short value)
        {
            bool success = buffer.TryRead(out value);
            if (BitConverter.IsLittleEndian)
            {
                value = value.Reverse();
            }
            return success;
        }

        /// <summary>
        /// Reads an Int32 out of a span of bytes as big endian.
        /// <returns>If the span is too small to contain an Int32, return false.</returns>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryReadInt32BigEndian(this Span<byte> buffer, out int value)
        {
            bool success = buffer.TryRead(out value);
            if (BitConverter.IsLittleEndian)
            {
                value = value.Reverse();
            }
            return success;
        }

        /// <summary>
        /// Reads an Int64 out of a span of bytes as big endian.
        /// <returns>If the span is too small to contain an Int64, return false.</returns>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryReadInt64BigEndian(this Span<byte> buffer, out long value)
        {
            bool success = buffer.TryRead(out value);
            if (BitConverter.IsLittleEndian)
            {
                value = value.Reverse();
            }
            return success;
        }

        /// <summary>
        /// Reads a UInt16 out of a span of bytes as big endian.
        /// <returns>If the span is too small to contain a UInt16, return false.</returns>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryReadUInt16BigEndian(this Span<byte> buffer, out ushort value)
        {
            bool success = buffer.TryRead(out value);
            if (BitConverter.IsLittleEndian)
            {
                value = value.Reverse();
            }
            return success;
        }

        /// <summary>
        /// Reads a UInt32 out of a span of bytes as big endian.
        /// <returns>If the span is too small to contain a UInt32, return false.</returns>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryReadUInt32BigEndian(this Span<byte> buffer, out uint value)
        {
            bool success = buffer.TryRead(out value);
            if (BitConverter.IsLittleEndian)
            {
                value = value.Reverse();
            }
            return success;
        }

        /// <summary>
        /// Reads a UInt64 out of a span of bytes as big endian.
        /// <returns>If the span is too small to contain a UInt64, return false.</returns>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryReadUInt64BigEndian(this Span<byte> buffer, out ulong value)
        {
            bool success = buffer.TryRead(out value);
            if (BitConverter.IsLittleEndian)
            {
                value = value.Reverse();
            }
            return success;
        }
        #endregion

        #region TryReadBigEndianROSpan
        /// <summary>
        /// Reads an Int16 out of a read-only span of bytes as big endian.
        /// <returns>If the span is too small to contain an Int16, return false.</returns>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryReadInt16BigEndian(this ReadOnlySpan<byte> buffer, out short value)
        {
            bool success = buffer.TryRead(out value);
            if (BitConverter.IsLittleEndian)
            {
                value = value.Reverse();
            }
            return success;
        }

        /// <summary>
        /// Reads an Int32 out of a read-only span of bytes as big endian.
        /// <returns>If the span is too small to contain an Int32, return false.</returns>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryReadInt32BigEndian(this ReadOnlySpan<byte> buffer, out int value)
        {
            bool success = buffer.TryRead(out value);
            if (BitConverter.IsLittleEndian)
            {
                value = value.Reverse();
            }
            return success;
        }

        /// <summary>
        /// Reads an Int64 out of a read-only span of bytes as big endian.
        /// <returns>If the span is too small to contain an Int64, return false.</returns>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryReadInt64BigEndian(this ReadOnlySpan<byte> buffer, out long value)
        {
            bool success = buffer.TryRead(out value);
            if (BitConverter.IsLittleEndian)
            {
                value = value.Reverse();
            }
            return success;
        }

        /// <summary>
        /// Reads a UInt16 out of a read-only span of bytes as big endian.
        /// <returns>If the span is too small to contain a UInt16, return false.</returns>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryReadUInt16BigEndian(this ReadOnlySpan<byte> buffer, out ushort value)
        {
            bool success = buffer.TryRead(out value);
            if (BitConverter.IsLittleEndian)
            {
                value = value.Reverse();
            }
            return success;
        }

        /// <summary>
        /// Reads a UInt32 out of a read-only span of bytes as big endian.
        /// <returns>If the span is too small to contain a UInt32, return false.</returns>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryReadUInt32BigEndian(this ReadOnlySpan<byte> buffer, out uint value)
        {
            bool success = buffer.TryRead(out value);
            if (BitConverter.IsLittleEndian)
            {
                value = value.Reverse();
            }
            return success;
        }

        /// <summary>
        /// Reads a UInt64 out of a read-only span of bytes as big endian.
        /// <returns>If the span is too small to contain a UInt64, return false.</returns>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryReadUInt64BigEndian(this ReadOnlySpan<byte> buffer, out ulong value)
        {
            bool success = buffer.TryRead(out value);
            if (BitConverter.IsLittleEndian)
            {
                value = value.Reverse();
            }
            return success;
        }
        #endregion
    }
}

