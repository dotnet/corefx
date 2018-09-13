using System;
using System.Runtime.CompilerServices;

namespace System // No idea what namespace
{
    partial class BitOps // .Span
    {
        #region ExtractBit

        /// <summary>
        /// Reads whether the specified bit in a mask is set.
        /// </summary>
        /// <param name="value">The mask.</param>
        /// <param name="offset">The ordinal position of the bit to read.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ExtractBit(ReadOnlySpan<byte> value, uint offset)
        {
            var ix = (int)(offset >> 3); // div 8
            if (ix >= value.Length) throw new ArgumentOutOfRangeException(nameof(offset));

            var shft = (byte)(offset & 7); // mod 8: design choice ignores out-of-range values

            return ExtractBit(value[ix], shft);
        }

        /// <summary>
        /// Reads whether the specified bit in a mask is set.
        /// </summary>
        /// <param name="value">The mask.</param>
        /// <param name="offset">The ordinal position of the bit to read.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ExtractBit(ReadOnlySpan<ushort> value, uint offset)
        {
            var ix = (int)(offset >> 4); // div 16
            if (ix >= value.Length) throw new ArgumentOutOfRangeException(nameof(offset));
            
            var shft = (byte)(offset & 15); // mod 16: design choice ignores out-of-range values

            return ExtractBit(value[ix], shft);
        }

        /// <summary>
        /// Reads whether the specified bit in a mask is set.
        /// </summary>
        /// <param name="value">The mask.</param>
        /// <param name="offset">The ordinal position of the bit to read.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ExtractBit(ReadOnlySpan<uint> value, uint offset)
        {
            var ix = (int)(offset >> 5); // div 32
            if (ix >= value.Length) throw new ArgumentOutOfRangeException(nameof(offset));
            
            var shft = (byte)(offset & 31); // mod 32: design choice ignores out-of-range values

            return ExtractBit(value[ix], shft);
        }

        /// <summary>
        /// Reads whether the specified bit in a mask is set.
        /// </summary>
        /// <param name="value">The mask.</param>
        /// <param name="offset">The ordinal position of the bit to read.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ExtractBit(ReadOnlySpan<ulong> value, uint offset)
        {
            var ix = (int)(offset >> 6); // div 64
            if (ix >= value.Length) throw new ArgumentOutOfRangeException(nameof(offset));

            var shft = (byte)(offset & 63); // mod 64: design choice ignores out-of-range values

            return ExtractBit(value[ix], shft);
        }

        #endregion

        #region InsertBit

        /// <summary>
        /// Sets the specified bit in a mask and returns whether it was originally set.
        /// </summary>
        /// <param name="value">The mask.</param>
        /// <param name="offset">The ordinal position of the bit to write.</param>
        /// <param name="on">True to set the bit to 1, or false to set it to 0.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool InsertBit(Span<byte> value, uint offset, bool on)
        {
            var ix = (int)(offset >> 3); // div 8
            if (ix >= value.Length) throw new ArgumentOutOfRangeException(nameof(offset));
            
            var shft = (byte)(offset & 7); // mod 8: design choice ignores out-of-range values

            return InsertBit(ref value[ix], shft, on);
        }

        /// <summary>
        /// Sets the specified bit in a mask and returns whether it was originally set.
        /// </summary>
        /// <param name="value">The mask.</param>
        /// <param name="offset">The ordinal position of the bit to write.</param>
        /// <param name="on">True to set the bit to 1, or false to set it to 0.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool InsertBit(Span<ushort> value, uint offset, bool on)
        {
            var ix = (int)(offset >> 4); // div 16
            if (ix >= value.Length) throw new ArgumentOutOfRangeException(nameof(offset));

            var shft = (byte)(offset & 15); // mod 16: design choice ignores out-of-range values

            return InsertBit(ref value[ix], shft, on);
        }

        /// <summary>
        /// Sets the specified bit in a mask and returns whether it was originally set.
        /// </summary>
        /// <param name="value">The mask.</param>
        /// <param name="offset">The ordinal position of the bit to write.</param>
        /// <param name="on">True to set the bit to 1, or false to set it to 0.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool InsertBit(Span<uint> value, uint offset, bool on)
        {
            var ix = (int)(offset >> 5); // div 32
            if (ix >= value.Length) throw new ArgumentOutOfRangeException(nameof(offset));

            var shft = (byte)(offset & 31); // mod 32: design choice ignores out-of-range values

            return InsertBit(ref value[ix], shft, on);
        }

        /// <summary>
        /// Sets the specified bit in a mask and returns whether it was originally set.
        /// </summary>
        /// <param name="value">The mask.</param>
        /// <param name="offset">The ordinal position of the bit to write.</param>
        /// <param name="on">True to set the bit to 1, or false to set it to 0.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool InsertBit(Span<ulong> value, uint offset, bool on)
        {
            var ix = (int)(offset >> 6); // div 64
            if (ix >= value.Length) throw new ArgumentOutOfRangeException(nameof(offset));

            var shft = (byte)(offset & 63); // mod 64: design choice ignores out-of-range values

            return InsertBit(ref value[ix], shft, on);
        }

        #endregion

        #region FlipBit

        /// <summary>
        /// Negates the specified bit in a mask and returns whether it was originally set.
        /// </summary>
        /// <param name="value">The mask.</param>
        /// <param name="offset">The ordinal position of the bit to flip.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool FlipBit(Span<byte> value, uint offset)
        {
            var ix = (int)(offset >> 3); // div 8
            if (ix >= value.Length) throw new ArgumentOutOfRangeException(nameof(offset));

            var shft = (byte)(offset & 7); // mod 8: design choice ignores out-of-range values

            return FlipBit(ref value[ix], shft);
        }

        /// <summary>
        /// Negates the specified bit in a mask and returns whether it was originally set.
        /// </summary>
        /// <param name="value">The mask.</param>
        /// <param name="offset">The ordinal position of the bit to flip.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool FlipBit(Span<ushort> value, uint offset)
        {
            var ix = (int)(offset >> 4); // div 16
            if (ix >= value.Length) throw new ArgumentOutOfRangeException(nameof(offset));

            var shft = (byte)(offset & 15); // mod 16: design choice ignores out-of-range values

            return FlipBit(ref value[ix], shft);
        }

        /// <summary>
        /// Negates the specified bit in a mask and returns whether it was originally set.
        /// </summary>
        /// <param name="value">The mask.</param>
        /// <param name="offset">The ordinal position of the bit to flip.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool FlipBit(Span<uint> value, uint offset)
        {
            var ix = (int)(offset >> 5); // div 32
            if (ix >= value.Length) throw new ArgumentOutOfRangeException(nameof(offset));

            var shft = (byte)(offset & 31); // mod 32: design choice ignores out-of-range values

            return FlipBit(ref value[ix], shft);
        }

        /// <summary>
        /// Negates the specified bit in a mask and returns whether it was originally set.
        /// </summary>
        /// <param name="value">The mask.</param>
        /// <param name="offset">The ordinal position of the bit to flip.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool FlipBit(Span<ulong> value, uint offset)
        {
            var ix = (int)(offset >> 6); // div 64
            if (ix >= value.Length) throw new ArgumentOutOfRangeException(nameof(offset));

            var shft = (byte)(offset & 63); // mod 64: design choice ignores out-of-range values

            return FlipBit(ref value[ix], shft);
        }

        #endregion

        #region PopCount

        /// <summary>
        /// Returns the population count (number of bits set) of a mask.
        /// </summary>
        /// <param name="value">The mask.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long PopCount(ReadOnlySpan<byte> value)
        {
            if (value.Length == 0)
                return 0;

            // TODO: Vectorize

            var sum = 0L;

            for (var i = 0; i < value.Length; i++)
            {
                sum += PopCount(value[i]);
            }

            return sum;
        }

        /// <summary>
        /// Returns the population count (number of bits set) of a mask.
        /// </summary>
        /// <param name="value">The mask.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long PopCount(ReadOnlySpan<ushort> value)
        {
            if (value.Length == 0)
                return 0;

            // TODO: Vectorize

            var sum = 0L;

            for (var i = 0; i < value.Length; i++)
            {
                sum += PopCount(value[i]);
            }

            return sum;
        }

        /// <summary>
        /// Returns the population count (number of bits set) of a mask.
        /// </summary>
        /// <param name="value">The mask.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long PopCount(ReadOnlySpan<uint> value)
        {
            if (value.Length == 0)
                return 0;

            // TODO: Vectorize

            var sum = 0L;

            for (var i = 0; i < value.Length; i++)
            {
                sum += PopCount(value[i]);
            }

            return sum;
        }

        /// <summary>
        /// Returns the population count (number of bits set) of a mask.
        /// </summary>
        /// <param name="value">The mask.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long PopCount(ReadOnlySpan<ulong> value)
        {
            if (value.Length == 0)
                return 0;

            // TODO: Vectorize

            var sum = 0L;

            for (var i = 0; i < value.Length; i++)
            {
                sum += PopCount(value[i]);
            }

            return sum;
        }

        #endregion

        #region LeadingCount

        /// <summary>
        /// Count the number of leading bits in a mask.
        /// </summary>
        /// <param name="value">The mask.</param>
        /// <param name="ones">True to count ones, or false to count zeros.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long LeadingCount(ReadOnlySpan<byte> value, bool ones)
        {
            if (value.Length == 0)
                return 0;

            // TODO: Vectorize

            var ix = 0;
            while (value[ix] == 0) ix++;
        
            return LeadingCount(value[ix], ones) + (ix << 3); // mul 8
        }

        /// <summary>
        /// Count the number of leading bits in a mask.
        /// </summary>
        /// <param name="value">The mask.</param>
        /// <param name="ones">True to count ones, or false to count zeros.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long LeadingCount(ReadOnlySpan<ushort> value, bool ones)
        {
            if (value.Length == 0)
                return 0;

            // TODO: Vectorize

            var ix = 0;
            while (value[ix] == 0) ix++;

            return LeadingCount(value[ix], ones) + (ix << 4); // mul 16
        }

        /// <summary>
        /// Count the number of leading bits in a mask.
        /// </summary>
        /// <param name="value">The mask.</param>
        /// <param name="ones">True to count ones, or false to count zeros.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long LeadingCount(ReadOnlySpan<uint> value, bool ones)
        {
            if (value.Length == 0)
                return 0;

            // TODO: Vectorize

            var ix = 0;
            while (value[ix] == 0) ix++;

            return LeadingCount(value[ix], ones) + (ix << 5); // mul 32
        }

        /// <summary>
        /// Count the number of leading bits in a mask.
        /// </summary>
        /// <param name="value">The mask.</param>
        /// <param name="ones">True to count ones, or false to count zeros.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long LeadingCount(ReadOnlySpan<ulong> value, bool ones)
        {
            if (value.Length == 0)
                return 0;

            // TODO: Vectorize

            var ix = 0;
            while (value[ix] == 0) ix++;

            return LeadingCount(value[ix], ones) + (ix << 6); // mul 64
        }

        #endregion

        #region TrailingCount

        /// <summary>
        /// Count the number of trailing bits in a mask.
        /// </summary>
        /// <param name="value">The mask.</param>
        /// <param name="ones">True to count ones, or false to count zeros.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long TrailingCount(ReadOnlySpan<byte> value, bool ones)
        {
            if (value.Length == 0)
                return 0;

            // TODO: Vectorize

            var ix = 0;
            var last = value.Length - 1;
            while (value[last - ix] == 0) ix++;

            return TrailingCount(value[last - ix], ones) + (ix << 3); // mul 8
        }

        /// <summary>
        /// Count the number of trailing bits in a mask.
        /// </summary>
        /// <param name="value">The mask.</param>
        /// <param name="ones">True to count ones, or false to count zeros.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long TrailingCount(ReadOnlySpan<ushort> value, bool ones)
        {
            if (value.Length == 0)
                return 0;

            // TODO: Vectorize

            var ix = 0;
            var last = value.Length - 1;
            while (value[last - ix] == 0) ix++;

            return TrailingCount(value[last - ix], ones) + (ix << 4); // mul 16
        }

        /// <summary>
        /// Count the number of trailing bits in a mask.
        /// </summary>
        /// <param name="value">The mask.</param>
        /// <param name="ones">True to count ones, or false to count zeros.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long TrailingCount(ReadOnlySpan<uint> value, bool ones)
        {
            if (value.Length == 0)
                return 0;

            // TODO: Vectorize

            var ix = 0;
            var last = value.Length - 1;
            while (value[last - ix] == 0) ix++;

            return TrailingCount(value[last - ix], ones) + (ix << 5); // mul 32
        }

        /// <summary>
        /// Count the number of trailing bits in a mask.
        /// </summary>
        /// <param name="value">The mask.</param>
        /// <param name="ones">True to count ones, or false to count zeros.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long TrailingCount(ReadOnlySpan<ulong> value, bool ones)
        {
            if (value.Length == 0)
                return 0;

            // TODO: Vectorize

            var ix = 0;
            var last = value.Length - 1;
            while (value[last - ix] == 0) ix++;

            return TrailingCount(value[last - ix], ones) + (ix << 6); // mul 64
        }

        #endregion
    }
}
