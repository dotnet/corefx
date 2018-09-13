using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace System // No idea what namespace
{
    /// <summary>
    /// Represents additional blit methods.
    /// </summary>
    public static partial class BitOps // .Primitive
    {
        #region ExtractBit

        /// <summary>
        /// Reads whether the specified bit in a mask is set.
        /// </summary>
        /// <param name="value">The mask.</param>
        /// <param name="offset">The ordinal position of the bit to read.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ExtractBit(in byte value, in byte offset)
        {
            var shft = offset & 7; // mod 8: design choice ignores out-of-range values
            var mask = 1U << shft;

            return (value & mask) != 0;
        }

        /// <summary>
        /// Reads whether the specified bit in a mask is set.
        /// </summary>
        /// <param name="value">The mask.</param>
        /// <param name="offset">The ordinal position of the bit to read.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ExtractBit(in ushort value, in byte offset)
        {
            var shft = offset & 15; // mod 16: design choice ignores out-of-range values
            var mask = 1U << shft;

            return (value & mask) != 0;
        }

        /// <summary>
        /// Reads whether the specified bit in a mask is set.
        /// </summary>
        /// <param name="value">The mask.</param>
        /// <param name="offset">The ordinal position of the bit to read.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ExtractBit(in uint value, in byte offset)
        {
            var shft = offset & 31; // mod 32: design choice ignores out-of-range values
            var mask = 1U << shft;

            return (value & mask) != 0;
        }

        /// <summary>
        /// Reads whether the specified bit in a mask is set.
        /// </summary>
        /// <param name="value">The mask.</param>
        /// <param name="offset">The ordinal position of the bit to read.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ExtractBit(in ulong value, in byte offset)
        {
            var shft = offset & 63; // mod 64: design choice ignores out-of-range values
            var mask = 1UL << shft;

            return (value & mask) != 0;
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
        public static bool InsertBit(ref byte value, in byte offset, in bool on)
        {
            var shft = offset & 7; // mod 8: design choice ignores out-of-range values
            var mask = 1U << shft;
            var rsp = value & mask;
            
            value = (byte)(on ?
                value | mask :
                value & ~mask);

            return rsp != 0; // BTS/BTR (inlining should prune if unused)
        }

        /// <summary>
        /// Sets the specified bit in a mask and returns whether it was originally set.
        /// </summary>
        /// <param name="value">The mask.</param>
        /// <param name="offset">The ordinal position of the bit to write.</param>
        /// <param name="on">True to set the bit to 1, or false to set it to 0.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool InsertBit(ref ushort value, in byte offset, in bool on)
        {
            var shft = offset & 15; // mod 16: design choice ignores out-of-range values
            var mask = 1U << shft;
            var rsp = value & mask;

            value = (ushort)(on ?
                value | mask :
                value & ~mask);

            return rsp != 0; // BTS/BTR (inlining should prune if unused)
        }

        /// <summary>
        /// Sets the specified bit in a mask and returns whether it was originally set.
        /// </summary>
        /// <param name="value">The mask.</param>
        /// <param name="offset">The ordinal position of the bit to write.</param>
        /// <param name="on">True to set the bit to 1, or false to set it to 0.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool InsertBit(ref uint value, in byte offset, in bool on)
        {
            var shft = offset & 31; // mod 32: design choice ignores out-of-range values
            var mask = 1U << shft;
            var rsp = value & mask;

            value = on ? 
                value | mask : 
                value & ~mask;

            return rsp != 0; // BTS/BTR (inlining should prune if unused)
        }

        /// <summary>
        /// Sets the specified bit in a mask and returns whether it was originally set.
        /// </summary>
        /// <param name="value">The mask.</param>
        /// <param name="offset">The ordinal position of the bit to write.</param>
        /// <param name="on">True to set the bit to 1, or false to set it to 0.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool InsertBit(ref ulong value, in byte offset, in bool on)
        {
            var shft = offset & 63; // mod 64: design choice ignores out-of-range values
            var mask = 1UL << shft;
            var rsp = value & mask;

            value = on ? 
                value | mask : 
                value & ~mask;

            return rsp != 0; // BTS/BTR (inlining should prune if unused)
        }

        #endregion

        #region FlipBit

        // Truth table (2):
        // v   m  | ~m  ^v  ~
        // 00  01 | 10  10  01
        // 01  01 | 10  11  00
        // 10  01 | 10  00  11
        // 11  01 | 10  01  10
        //                      
        // 00  10 | 01  01  10
        // 01  10 | 01  00  11
        // 10  10 | 01  11  00
        // 11  10 | 01  10  01

        /// <summary>
        /// Negates the specified bit in a mask and returns whether it was originally set.
        /// </summary>
        /// <param name="value">The mask.</param>
        /// <param name="offset">The ordinal position of the bit to flip.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool FlipBit(ref byte value, in byte offset)
        {
            var shft = offset & 7; // mod 8: design choice ignores out-of-range values
            var mask = 1U << shft;
            var rsp = value & mask;

            // See Truth table (2) above
            value = (byte)~(~mask ^ value);

            return rsp != 0; // BTC (inlining should prune if unused)
        }

        /// <summary>
        /// Negates the specified bit in a mask and returns whether it was originally set.
        /// </summary>
        /// <param name="value">The mask.</param>
        /// <param name="offset">The ordinal position of the bit to flip.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool FlipBit(ref ushort value, in byte offset)
        {
            var shft = offset & 15; // mod 16: design choice ignores out-of-range values
            var mask = 1U << shft;
            var rsp = value & mask;

            // See Truth table (2) above
            value = (ushort)~(~mask ^ value);

            return rsp != 0; // BTC (inlining should prune if unused)
        }

        /// <summary>
        /// Negates the specified bit in a mask and returns whether it was originally set.
        /// </summary>
        /// <param name="value">The mask.</param>
        /// <param name="offset">The ordinal position of the bit to flip.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool FlipBit(ref uint value, in byte offset)
        {
            var shft = offset & 31; // mod 32: design choice ignores out-of-range values
            var mask = 1U << shft;
            var rsp = value & mask;

            // See Truth table (2) above
            value = ~(~mask ^ value);

            return rsp != 0; // BTC (inlining should prune if unused)
        }

        /// <summary>
        /// Negates the specified bit in a mask and returns whether it was originally set.
        /// </summary>
        /// <param name="value">The mask.</param>
        /// <param name="offset">The ordinal position of the bit to flip.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool FlipBit(ref ulong value, in byte offset)
        {
            var shft = offset & 63; // mod 64: design choice ignores out-of-range values
            var mask = 1UL << shft;
            var rsp = value & mask;

            // See Truth table (2) above
            value = ~(~mask ^ value);

            return rsp != 0; // BTC (inlining should prune if unused)
        }

        #endregion

        #region Rotate

        /// <summary>
        /// Rotates the specified value left by the specified number of bits.
        /// </summary>
        /// <param name="value">The value to rotate.</param>
        /// <param name="offset">The number of bits to rotate by.</param>
        /// <returns>The rotated value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte RotateLeft(in byte value, in byte offset)
        {
            var shft = offset & 7; // mod 8 safely ignores boundary checks
            var val = (uint)value;

            // Intrinsic not available for byte/ushort
            return (byte)((val << shft) | (val >> (8 - shft)));
        }

        /// <summary>
        /// Rotates the specified value right by the specified number of bits.
        /// </summary>
        /// <param name="value">The value to rotate.</param>
        /// <param name="offset">The number of bits to rotate by.</param>
        /// <returns>The rotated value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte RotateRight(in byte value, in byte offset)
        {
            var shft = offset & 7; // mod 8 safely ignores boundary checks
            var val = (uint)value;

            // Intrinsic not available for byte/ushort
            return (byte)((val >> shft) | (val << (8 - shft)));
        }

        /// <summary>
        /// Rotates the specified value left by the specified number of bits.
        /// </summary>
        /// <param name="value">The value to rotate.</param>
        /// <param name="offset">The number of bits to rotate by.</param>
        /// <returns>The rotated value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort RotateLeft(in ushort value, in byte offset)
        {
            var shft = offset & 15; // mod 16 safely ignores boundary checks
            var val = (uint)value;

            // Intrinsic not available for byte/ushort
            return (ushort)((val << shft) | (val >> (16 - shft)));
        }

        /// <summary>
        /// Rotates the specified value right by the specified number of bits.
        /// </summary>
        /// <param name="value">The value to rotate.</param>
        /// <param name="offset">The number of bits to rotate by.</param>
        /// <returns>The rotated value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort RotateRight(in ushort value, in byte offset)
        {
            var shft = offset & 15; // mod 16 safely ignores boundary checks
            var val = (uint)value;

            // Intrinsic not available for byte/ushort
            return (ushort)((val >> shft) | (val << (16 - shft)));
        }

        /// <summary>
        /// Rotates the specified value left by the specified number of bits.
        /// </summary>
        /// <param name="value">The value to rotate.</param>
        /// <param name="offset">The number of bits to rotate by.</param>
        /// <returns>The rotated value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint RotateLeft(in uint value, in byte offset)
        {
            var shft = offset & 31; // mod 32 safely ignores boundary checks

            // Will compile to instrinsic if pattern complies (uint/ulong):
            // https://github.com/dotnet/coreclr/pull/1830
            return (value << shft) | (value >> (32 - shft));
        }

        /// <summary>
        /// Rotates the specified value right by the specified number of bits.
        /// </summary>
        /// <param name="value">The value to rotate.</param>
        /// <param name="offset">The number of bits to rotate by.</param>
        /// <returns>The rotated value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint RotateRight(in uint value, in byte offset)
        {
            var shft = offset & 31; // mod 32 safely ignores boundary checks

            // Will compile to instrinsic if pattern complies (uint/ulong):
            // https://github.com/dotnet/coreclr/pull/1830
            return (value >> shft) | (value << (32 - shft));
        }

        /// <summary>
        /// Rotates the specified value left by the specified number of bits.
        /// </summary>
        /// <param name="value">The value to rotate.</param>
        /// <param name="offset">The number of bits to rotate by.</param>
        /// <returns>The rotated value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong RotateLeft(in ulong value, in byte offset)
        {
            var shft = offset & 63; // mod 64 safely ignores boundary checks

            // Will compile to instrinsic if pattern complies (uint/ulong):
            // https://github.com/dotnet/coreclr/pull/1830
            return (value << shft) | (value >> (64 - shft));
        }

        /// <summary>
        /// Rotates the specified value right by the specified number of bits.
        /// </summary>
        /// <param name="value">The value to rotate.</param>
        /// <param name="offset">The number of bits to rotate by.</param>
        /// <returns>The rotated value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong RotateRight(in ulong value, in byte offset)
        {
            var shft = offset & 63; // mod 64 safely ignores boundary checks

            // Will compile to instrinsic if pattern complies (uint/ulong):
            // https://github.com/dotnet/coreclr/pull/1830
            return (value >> shft) | (value << (64 - shft));
        }

        #endregion

        #region PopCount

        // Truth table (1):
        // Short-circuit lower boundary using optimization trick (n+1 >> 1)
        // 0 (000) -> 1 (001) -> 0 (000) ✔
        // 1 (001) -> 2 (010) -> 1 (001) ✔
        // 2 (010) -> 3 (011) -> 1 (001) ✔
        // 3 (011) -> 4 (100) -> 2 (010) ✔
        // 4 (100) -> 5 (101) -> 2 (010) ✖ (trick fails)

        /// <summary>
        /// Returns the population count (number of bits set) of a mask.
        /// </summary>
        /// <param name="value">The mask.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int PopCount(in byte value)
        {
            // 22 ops
            // TODO: Benchmark whether other algo is faster
            var val
                = (value & 1)
                + (value >> 1 & 1)
                + (value >> 2 & 1)
                + (value >> 3 & 1)
                + (value >> 4 & 1)
                + (value >> 5 & 1)
                + (value >> 6 & 1)
                + (value >> 7 & 1);

            return val;
        }

        /// <summary>
        /// Returns the population count (number of bits set) of a mask.
        /// </summary>
        /// <param name="value">The mask.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int PopCount(in ushort value)
            => PopCount((uint)value);

        /// <summary>
        /// Returns the population count (number of bits set) of a mask.
        /// </summary>
        /// <param name="value">The mask.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int PopCount(in uint value)
        {
            // See truth table (1) above
            if (value <= 3)
                return (int)((value + 1) >> 1);

            // Uses a SWAR (SIMD Within A Register) approach

            const uint c0 = 0x_5555_5555;
            const uint c1 = 0x_3333_3333;
            const uint c2 = 0x_0F0F_0F0F;
            const uint c3 = 0x_0101_0101;

            var val = value;

            val -= (val >> 1) & c0;
            val = (val & c1) + ((val >> 2) & c1);
            val = (val + (val >> 4)) & c2;
            val *= c3;
            val >>= 24;

            return (int)val;
        }

        /// <summary>
        /// Returns the population count (number of bits set) of a mask.
        /// </summary>
        /// <param name="value">The mask.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int PopCount(in ulong value)
        {
            // See truth table (1) above
            if (value <= 3)
                return (int)((value + 1) >> 1);

            // Use a SWAR (SIMD Within A Register) approach

            const ulong c0 = 0x_5555_5555_5555_5555;
            const ulong c1 = 0x_3333_3333_3333_3333;
            const ulong c2 = 0x_0F0F_0F0F_0F0F_0F0F;
            const ulong c3 = 0x_0101_0101_0101_0101;

            var val = value;

            val -= (value >> 1) & c0;
            val = (val & c1) + ((val >> 2) & c1);
            val = (val + (val >> 4)) & c2;
            val *= c3;
            val >>= 56;

            return (int)val;
        }

        #endregion

        #region LeadingCount        

        /// <summary>
        /// Count the number of leading bits in a mask.
        /// </summary>
        /// <param name="value">The mask.</param>
        /// <param name="ones">True to count ones, or false to count zeros.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int LeadingCount(in byte value, in bool ones)
        {
            if (value == 0)
                return ones ? 0 : 8;

            if (value == byte.MaxValue)
                return ones ? 8 : 0;

            // If a leading-ones operation, negate mask but remember to truncate carry-bits
            var val = ones ? (uint)(byte)~(uint)value : value;

            return 7 - FloorLog2Impl(val);
        }

        /// <summary>
        /// Count the number of leading bits in a mask.
        /// </summary>
        /// <param name="value">The mask.</param>
        /// <param name="ones">True to count ones, or false to count zeros.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int LeadingCount(in ushort value, in bool ones)
        {
            if (value == 0)
                return ones ? 0 : 16;

            if (value == ushort.MaxValue)
                return ones ? 16 : 0;

            // If a leading-ones operation, negate mask but remember to truncate carry-bits
            var val = ones ? (uint)(ushort)~(uint)value : value;

            return 15 - FloorLog2Impl(val);
        }

        /// <summary>
        /// Count the number of leading bits in a mask.
        /// </summary>
        /// <param name="value">The mask.</param>
        /// <param name="ones">True to count ones, or false to count zeros.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int LeadingCount(in uint value, in bool ones)
        {
            if (value == 0)
                return ones ? 0 : 32;

            if (value == uint.MaxValue)
                return ones ? 32 : 0;

            // If a leading-ones operation, negate mask but remember to truncate carry-bits
            var val = ones ? ~value : value;

            return 31 - FloorLog2Impl(val);
        }

        /// <summary>
        /// Count the number of leading bits in a mask.
        /// </summary>
        /// <param name="value">The mask.</param>
        /// <param name="ones">True to count ones, or false to count zeros.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int LeadingCount(in ulong value, in bool ones)
        {
            if (value == 0)
                return ones ? 0 : 64;

            if (value == ulong.MaxValue)
                return ones ? 64 : 0;

            // If a leading-ones operation, negate mask but remember to truncate carry-bits
            var val = ones ? ~value : value;
            
            return 63 - FloorLog2(val);
        }

        #endregion

        #region TrailingCount

        // Build this table by taking n = 0,1,2,4,...,512
        // [2^n % 11] = tz(n)
        private static readonly byte[] s_trail8u = new byte[11] // mod 11
        {
            //    2^n  % 11     b=bin(n)   z=tz(b)
            8, //   0  [ 0]     0000_0000  8
            0, //   1  [ 1]     0000_0001  0 
            1, //   2  [ 2]     0000_0010  1
            8, // 256  [ 3]  01_0000_0000  8 (n/a) 1u << 8
               
            2, //   4  [ 4]     0000_0100  2
            4, //  16  [ 5]     0001_0000  4
            9, // 512  [ 6]  10_0000_0000  9 (n/a) 1u << 9
            7, // 128  [ 7]     1000_0000  7
               
            3, //   8  [ 8]     0000_1000  3
            6, //  64  [ 9]     0100_0000  6
            5, //  32  [10]     0010_0000  5
        };

        /// <summary>
        /// Count the number of trailing bits in a mask.
        /// </summary>
        /// <param name="value">The mask.</param>
        /// <param name="ones">True to count ones, or false to count zeros.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int TrailingCount(in byte value, in bool ones)
        {
            // If a trailing-ones operation, negate mask but remember to truncate carry-bits
            var val = ones ? (uint)(byte)~(uint)value : value;

            // The expression (n & -n) returns lsb(n).
            // Only possible values are therefore [0,1,2,4,...,128]
            var lsb = val & -val; // eg 44==0010 1100 -> (44 & -44) -> 4. 4==0100, which is the lsb of 44.

            // Mod-11 is a simple perfect-hashing scheme over [0,1,2,4,...,128]
            // in order to derive a contiguous range [0..10] to use as a jmp table.
            lsb = lsb % 11; // eg 44 -> 4 % 11 -> 4

            // NoOp: Hashing scheme has unused outputs (inputs 256 and higher do not fit a byte)
            Debug.Assert(!(lsb == 3 || lsb == 6), $"{nameof(TrailingCount)}({value}, {ones}) resulted in unexpected {typeof(byte)} hash {lsb}");

            var cnt = s_trail8u[lsb]; // eg 44 -> 4 -> 2 (44==0010 1100 has 2 trailing zeros)
            return cnt;
        }

        // See algorithm notes in TrailingCount(byte)
        private static readonly byte[] s_trail16u = new byte[19] // mod 19
        {
            //        2^n  % 19     b=bin(n)             z=tz(b)
            16, //      0  [ 0]     0000_0000_0000_0000  16
            00, //      1  [ 1]     0000_0000_0000_0001   0
            01, //      2  [ 2]     0000_0000_0000_0010   1
            13, //   8192  [ 3]     0010_0000_0000_0000  13

            02, //      4  [ 4]     0000_0000_0000_0100   2
            16, //  65536  [ 5]  01_0000_0000_0000_0000  16 (n/a) 1u << 16
            14, //  16384  [ 6]     0100_0000_0000_0000  14
            06, //     64  [ 7]     0000_0000_0100_0000   6

            03, //      8  [ 8]     0000_0000_0000_1000   3
            08, //    256  [ 9]     0000_0001_0000_0000   8
            17, // 131072  [10]  10_0000_0000_0000_0000  17 (n/a) 1u << 17
            12, //   4096  [11]     0001_0000_0000_0000  12

            15, //  32768  [12]     1000_0000_0000_0000  15
            05, //     32  [13]     0000_0000_0010_0000   5
            07, //    128  [14]     0000_0000_1000_0000   7
            11, //   2048  [15]     0000_1000_0000_0000  11

            04, //     16  [16]     0000_0000_0001_0000   4
            10, //   1024  [17]     0000_0100_0000_0000  10
            09  //    512  [18]     0000_0010_0000_0000   9
        };

        /// <summary>
        /// Count the number of trailing bits in a mask.
        /// </summary>
        /// <param name="value">The mask.</param>
        /// <param name="ones">True to count ones, or false to count zeros.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int TrailingCount(in ushort value, in bool ones)
        {
            // If a trailing-ones operation, negate mask but remember to truncate carry-bits
            var val = ones ? (uint)(ushort)~(uint)value : value;

            // See algorithm notes in TrailingCount(byte)
            var lsb = val & -val;
            lsb = lsb % 19; // mod 19

            // NoOp: Hashing scheme has unused outputs (inputs 65536 and higher do not fit a ushort)
            Debug.Assert(!(lsb == 5 || lsb == 10), $"{nameof(TrailingCount)}({value}, {ones}) resulted in unexpected {typeof(ushort)} hash {lsb}");

            var cnt = s_trail16u[lsb];
            return cnt;
        }

        // See algorithm notes in TrailingCount(byte)
        private static readonly byte[] s_trail32u = new byte[37] // mod 37
        {
            //                2^n  % 37       b=bin(n)                                 z=tz(b)
            32, //              0  [ 0]       0000_0000_0000_0000_0000_0000_0000_0000  32
            00, //              1  [ 1]       0000_0000_0000_0000_0000_0000_0000_0001   0
            01, //              2  [ 2]       0000_0000_0000_0000_0000_0000_0000_0010   1
            26,

            02, //              4  [ 4]       0000_0000_0000_0000_0000_0000_0000_0100   2
            23,
            27,                  
            32, //  4,294,967,296  [ 7]  0001_0000_0000_0000_0000_0000_0000_0000_0000  32 (n/a) 1ul << 32

            03, //              8  [ 8]       0000_0000_0000_0000_0000_0000_0000_1000   3
            16,
            24,
            30,

            28,
            11, //           2048  [13]       0000_0000_0000_0000_0000_1000_0000_0000  11
            33, //  8,589,934,592  [14]  0010_0000_0000_0000_0000_0000_0000_0000_0000  33 (n/a) 1ul << 33
            13,

            04, //             16  [16]       0000_0000_0000_0000_0000_0000_0001_0000   4
            07, //            128  [17]       0000_0000_0000_0000_0000_0000_1000_0000   7
            17,
            35, // 34,359,738,368  [19]  1000_0000_0000_0000_0000_0000_0000_0000_0000  35 (n/a) 1ul << 35

            25,
            22,
            31,
            15, //           8192  [15]       0000_0000_0000_0000_0010_0000_0000_0000  13

            29,
            10, //           1024  [25]       0000_0000_0000_0000_0000_0100_0000_0000  10
            12, //           4096  [26]       0000_0000_0000_0000_0001_0000_0000_0000  12
            06, //             64  [27]       0000_0000_0000_0000_0000_0000_0100_0000   6

            34, // 17,179,869,184  [28]  0100_0000_0000_0000_0000_0000_0000_0000_0000  34 (n/a) 1ul << 34
            21,
            14,
            09, //            512  [31]       0000_0000_0000_0000_0000_0010_0000_0000   9

            05, //             32  [32]       0000_0000_0000_0000_0000_0000_0010_0000   5
            20,
            08, //            256  [34]       0000_0000_0000_0000_0000_0001_0000_0000   8
            19,

            18  //        262,144  [36]
        };

        /// <summary>
        /// Count the number of trailing bits in a mask.
        /// </summary>
        /// <param name="value">The mask.</param>
        /// <param name="ones">True to count ones, or false to count zeros.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int TrailingCount(in uint value, in bool ones)
        {
            // If a trailing-ones operation, negate mask
            var val = ones ? ~value : value;

            // See algorithm notes in TrailingCount(byte)
            var lsb = val & -val;
            lsb = lsb % 37; // mod 37

            // NoOp: Hashing scheme has unused outputs (inputs 4,294,967,296 and higher do not fit a uint)
            Debug.Assert(!(lsb == 7 || lsb == 14 || lsb == 19 || lsb == 28), $"{nameof(TrailingCount)}({value}, {ones}) resulted in unexpected {typeof(uint)} hash {lsb}");

            var cnt = s_trail32u[lsb];
            return cnt;
        }

        /// <summary>
        /// Count the number of trailing bits in a mask.
        /// </summary>
        /// <param name="value">The mask.</param>
        /// <param name="ones">True to count ones, or false to count zeros.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int TrailingCount(in ulong value, in bool ones)
        {
            if (value == 0)
                return ones ? 0 : 64;

            var val = (uint)value; // Grab low uint
            var inc = 0;

            if (value > uint.MaxValue)
            {
                if (value == ulong.MaxValue)
                    return ones ? 64 : 0;

                // TrailingOnes 
                if (ones)
                {
                    if (val == uint.MaxValue)
                    {
                        val = (uint)(value >> 32); // Grab high uint
                        inc = 32;
                    }
                }

                // TrailingZeros
                else if (val == 0)
                {
                    val = (uint)(value >> 32); // Grab high uint
                    inc = 32;
                }
            }

            return inc + TrailingCount(val, ones);
        }

        #endregion

        #region FloorLog2        

        private static readonly byte[] s_deBruijn32 = new byte[32]
        {
            00, 09, 01, 10, 13, 21, 02, 29,
            11, 14, 16, 18, 22, 25, 03, 30,
            08, 12, 20, 28, 15, 17, 24, 07,
            19, 27, 23, 06, 26, 05, 04, 31
        };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int FloorLog2Impl(in uint value)
        {
            // Perf: Do not use guard clauses; callers must be trusted

            // Short-circuit lower boundary using optimization trick (n >> 1)
            // 0 (000) => 0 (000) ✖ (n/a, 0 trapped @ callsite)
            // 1 (001) => 0 (000) ✔
            // 2 (010) => 1 (001) ✔
            // 3 (011) => 1 (001) ✔
            // 4 (100) => 2 (010) ✔
            // 5 (101) => 2 (010) ✔
            // 6 (110) => 3 (011) ✖ (trick fails)

            if (value <= 5)
                return (int)(value >> 1);

            var val = value;
            val |= val >> 01;
            val |= val >> 02;
            val |= val >> 04;
            val |= val >> 08;
            val |= val >> 16;

            const uint c32 = 0x07C4_ACDDu;
            var ix = (val * c32) >> 27;

            return s_deBruijn32[ix];
        }

        /// <summary>
        /// Finds the floor of the base-2 log of the specified value.
        /// It is a fast equivalent of <code>Math.Floor(Math.Log(<paramref name="value"/>, 2))</code>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The floor(log2) of the value.</returns>
        public static int FloorLog2(in byte value)
        {
            if (value == 0) throw new ArgumentOutOfRangeException(nameof(value));

            // Short-circuit upper boundary
            // 2^7              = 128
            // byte.MaxValue    = 255
            // 2^8              = 256

            const uint hi = 1U << 7;
            if (value >= hi) return 7;

            return FloorLog2Impl(value);
        }

        /// <summary>
        /// Finds the floor of the base-2 log of the specified value.
        /// It is a fast equivalent of <code>Math.Floor(Math.Log(<paramref name="value"/>, 2))</code>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The floor(log2) of the value.</returns>
        public static int FloorLog2(in sbyte value)
        {
            if (value == 0) throw new ArgumentOutOfRangeException(nameof(value));

            // Short-circuit upper boundary
            // 2^6              = 63
            // sbyte.MaxValue   = 127
            // 2^7              = 128

            const uint hi = 1U << 6;
            if (value >= hi) return 6;

            return FloorLog2Impl((uint)value);
        }

        /// <summary>
        /// Finds the floor of the base-2 log of the specified value.
        /// It is a fast equivalent of <code>Math.Floor(Math.Log(<paramref name="value"/>, 2))</code>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The floor(log2) of the value.</returns>
        public static int FloorLog2(in ushort value)
        {
            if (value == 0) throw new ArgumentOutOfRangeException(nameof(value));

            // Short-circuit upper boundary
            // 2^15             = 32,768
            // byte.MaxValue    = 65,535
            // 2^16             = 65,536

            const uint hi = 1U << 15;
            if (value >= hi) return 15;

            return FloorLog2Impl(value);
        }

        /// <summary>
        /// Finds the floor of the base-2 log of the specified value.
        /// It is a fast equivalent of <code>Math.Floor(Math.Log(<paramref name="value"/>, 2))</code>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The floor(log2) of the value.</returns>
        public static int FloorLog2(in short value)
        {
            if (value == 0) throw new ArgumentOutOfRangeException(nameof(value));

            // Short-circuit upper boundary
            // 2^14             = 16,384
            // short.MaxValue   = 32,767
            // 2^15             = 32,768

            const uint hi = 1U << 14;
            if (value >= hi) return 14;

            return FloorLog2Impl((uint)value);
        }

        /// <summary>
        /// Finds the floor of the base-2 log of the specified value.
        /// It is a fast equivalent of <code>Math.Floor(Math.Log(<paramref name="value"/>, 2))</code>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The floor(log2) of the value.</returns>
        public static int FloorLog2(in uint value)
        {
            if (value == 0) throw new ArgumentOutOfRangeException(nameof(value));

            // Short-circuit upper boundary
            // 2^31             = 2,147,483,648
            // uint.MaxValue    = 4,294,967,295
            // 2^32             = 4,294,967,296

            const uint hi = 1U << 31;
            if (value >= hi) return 31;

            return FloorLog2Impl(value);
        }

        /// <summary>
        /// Finds the floor of the base-2 log of the specified value.
        /// It is a fast equivalent of <code>Math.Floor(Math.Log(<paramref name="value"/>, 2))</code>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The floor(log2) of the value.</returns>
        public static int FloorLog2(in int value)
        {
            if (value <= 0) throw new ArgumentOutOfRangeException(nameof(value));

            // Short-circuit upper boundary
            // 2^30             = 1,073,741,824
            // int.MaxValue     = 2,147,483,647
            // 2^31             = 2,147,483,648

            const int hi = 1 << 30;
            if (value >= hi) return 30;

            return FloorLog2Impl((uint)value);
        }

        /// <summary>
        /// Finds the floor of the base-2 log of the specified value.
        /// It is a fast equivalent of <code>Math.Floor(Math.Log(<paramref name="value"/>, 2))</code>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The floor(log2) of the value.</returns>
        public static int FloorLog2(in ulong value)
        {
            if (value == 0) throw new ArgumentOutOfRangeException(nameof(value));

            // Short-circuit upper boundary
            // 2^63             = 9,223,372,036,854,775,808
            // ulong.MaxValue   = 18,446,744,073,709,551,615
            // 2^64             = 18,446,744,073,709,551,616

            const ulong hi = 1UL << 63;

            // Heuristic: hot path assumes small numbers more likely
            var val = (uint)value;
            var inc = 0;

            if (value > uint.MaxValue) // 0xFFFF_FFFF
            {
                if (value >= hi) return 63;

                val = (uint)(value >> 32);
                inc = 32;
            }

            return inc + FloorLog2Impl(val);
        }

        /// <summary>
        /// Finds the floor of the base-2 log of the specified value.
        /// It is a fast equivalent of <code>Math.Floor(Math.Log(<paramref name="value"/>, 2))</code>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The floor(log2) of the value.</returns>
        public static int FloorLog2(in long value)
        {
            if (value <= 0) throw new ArgumentOutOfRangeException(nameof(value));

            // Short-circuit upper boundary
            // 2^62             = 4,611,686,018,427,387,904
            // long.MaxValue    = 9,223,372,036,854,775,807
            // 2^63             = 9,223,372,036,854,775,808

            const long hi = 1L << 62;
            
            // Heuristic: hot path assumes small numbers more likely
            var val = (uint)value;
            var inc = 0;

            if (value > uint.MaxValue) // 0xFFFF_FFFF
            {
                if (value >= hi) return 62;

                val = (uint)(value >> 32);
                inc = 32;
            }

            return inc + FloorLog2Impl(val);
        }

        #endregion
    }
}
