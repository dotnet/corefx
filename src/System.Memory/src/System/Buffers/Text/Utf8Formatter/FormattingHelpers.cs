// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Runtime.CompilerServices;

#if !netstandard
using Internal.Runtime.CompilerServices;
#endif

namespace System.Buffers.Text
{
    // All the helper methods in this class assume that the by-ref is valid and that there is
    // enough space to fit the items that will be written into the underlying memory. The calling
    // code must have already done all the necessary validation.
    internal static class FormattingHelpers
    {
        // For the purpose of formatting time, the format specifier contains room for
        // exactly 7 digits in the fraction portion. See "Round-trip format specifier"
        // at the following URL for more information.
        // https://msdn.microsoft.com/en-us/library/az4se3k1(v=vs.110).aspx#Roundtrip
        private const int FractionDigits = 7;

        // A simple lookup table for converting numbers to hex.
        internal const string HexTableLower = "0123456789abcdef";

        internal const string HexTableUpper = "0123456789ABCDEF";

        // TODO: Where should the below method live? Ideally it should be publicly exposed
        // since having blittable access to structs would be convenient.
        // Should it be on BinaryPrimitives?

        /// <summary>
        /// Given a reference to a blittable value type, returns a Span that allows
        /// access to the binary representation of the value.
        /// </summary>
        /// <remarks>
        /// No copy is performed as part of this method's logic.
        /// This is intended to be a "safe" API even though it's implemented in terms of unsafe casts.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span<byte> GetSpanForBlittable<T>(ref T value) where T : struct
        {
#if netstandard
            if (SpanHelpers.IsReferenceOrContainsReferences<T>())
            {
                ThrowHelper.ThrowArgumentException_InvalidTypeWithPointersNotSupported(typeof(T));
            }
#else
            if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
            {
                ThrowHelper.ThrowArgumentException_InvalidTypeWithPointersNotSupported(typeof(T));
            }
#endif
            return Span<byte>.DangerousCreate(null, ref Unsafe.As<T, byte>(ref value), Unsafe.SizeOf<T>());
        }

        /// <summary>
        /// Returns the symbol contained within the standard format. If the standard format
        /// has not been initialized, returns the provided fallback symbol.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char GetSymbolOrDefault(in StandardFormat format, char defaultSymbol)
        {
            // This is equivalent to the line below, but it is written in such a way
            // that the JIT is able to perform more optimizations.
            //
            // return (format.IsDefault) ? defaultSymbol : format.Symbol;

            var symbol = format.Symbol;
            if (symbol == default && format.Precision == default)
            {
                symbol = defaultSymbol;
            }
            return symbol;
        }

        #region UTF-8 Helper methods

        public enum HexCasing : uint
        {
            // Output [ '0' .. '9' ] and [ 'A' .. 'F' ].
            Uppercase = 0,

            // Output [ '0' .. '9' ] and [ 'a' .. 'f' ].
            // This works because values in the range [ 0x30 .. 0x39 ] ([ '0' .. '9' ])
            // already have the 0x20 bit set, so ORing them with 0x20 is a no-op,
            // while outputs in the range [ 0x41 .. 0x46 ] ([ 'A' .. 'F' ])
            // don't have the 0x20 bit set, so ORing them maps to
            // [ 0x61 .. 0x66 ] ([ 'a' .. 'f' ]), which is what we want.
            Lowercase = 0x2020U,
        }

        // The JIT can elide bounds checks if 'startingIndex' is constant and if the caller is
        // writing to a span of known length (or the caller has already checked the bounds of the
        // furthest access).
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteHexByte(byte value, Span<byte> buffer, int startingIndex = 0, HexCasing casing = HexCasing.Uppercase)
        {
            // We want to pack the incoming byte into a single integer [ 0000 HHHH 0000 LLLL ],
            // where HHHH and LLLL are the high and low nibbles of the incoming byte. Then
            // subtract this integer from a constant minuend as shown below.
            //
            //   [ 1000 1001 1000 1001 ]
            // - [ 0000 HHHH 0000 LLLL ]
            // =========================
            //   [ *YYY **** *ZZZ **** ]
            //
            // The end result of this is that YYY is 0b000 if HHHH <= 9, and YYY is 0b111 if HHHH >= 10.
            // Similarly, ZZZ is 0b000 if LLLL <= 9, and ZZZ is 0b111 if LLLL >= 10.
            // (We don't care about the value of asterisked bits.)
            //
            // To turn a nibble in the range [ 0 .. 9 ] into hex, we calculate hex := nibble + 48 (ascii '0').
            // To turn a nibble in the range [ 10 .. 15 ] into hex, we calculate hex := nibble - 10 + 65 (ascii 'A').
            //                                                                => hex := nibble + 55.
            // The difference in the starting ASCII offset is (55 - 48) = 7, depending on whether the nibble is <= 9 or >= 10.
            // Since 7 is 0b111, this conveniently matches the YYY or ZZZ value computed during the earlier subtraction.

            // The commented out code below is code that directly implements the logic described above.

            //uint packedOriginalValues = (((uint)value & 0xF0U) << 4) + ((uint)value & 0x0FU);
            //uint difference = 0x8989U - packedOriginalValues;
            //uint add7Mask = (difference & 0x7070U) >> 4; // line YYY and ZZZ back up with the packed values
            //uint packedResult = packedOriginalValues + add7Mask + 0x3030U /* ascii '0' */;

            // The code below is equivalent to the commented out code above but has been tweaked
            // to allow codegen to make some extra optimizations.

            uint difference = (((uint)value & 0xF0U) << 4) + ((uint)value & 0x0FU) - 0x8989U;
            uint packedResult = ((((uint)(-(int)difference) & 0x7070U) >> 4) + difference + 0xB9B9U) | (uint)casing;

            // The low byte of the packed result contains the hex representation of the incoming byte's low nibble.
            // The adjacent byte of the packed result contains the hex representation of the incoming byte's high nibble.

            // Finally, write to the output buffer starting with the *highest* index so that codegen can
            // elide all but the first bounds check. (This only works if 'startingIndex' is a compile-time constant.)

            buffer[startingIndex + 1] = (byte)(packedResult);
            buffer[startingIndex] = (byte)(packedResult >> 8);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteFractionDigits(long value, int digitCount, ref byte buffer, int index)
        {
            for (int i = FractionDigits; i > digitCount; i--)
                value /= 10;

            WriteDigits(value, digitCount, ref buffer, index);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteDigits(ulong value, Span<byte> buffer)
        {
            ulong left = value;

            for (int i = buffer.Length - 1; i >= 1; i--)
            {
                left = DivMod(left, 10, out ulong num);
                buffer[i] = (byte)('0' + num);
            }

            Debug.Assert(left < 10);
            buffer[0] = (byte)('0' + left);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteDigits(uint value, Span<byte> buffer)
        {
            uint left = value;

            for (int i = buffer.Length - 1; i >= 1; i--)
            {
                left = DivMod(left, 10, out uint num);
                buffer[i] = (byte)('0' + num);
            }

            Debug.Assert(left < 10);
            buffer[0] = (byte)('0' + left);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteDigits(long value, int digitCount, ref byte buffer, int index)
        {
            long left = value;

            for (int i = digitCount - 1; i >= 0; i--)
            {
                left = DivMod(left, 10, out long num);
                Unsafe.Add(ref buffer, index + i) = (byte)('0' + num);
            }

            Debug.Assert(left == 0);
        }

        /// <summary>
        /// The unsigned long implementation of this method is much slower than the signed version above
        /// due to optimization tricks that happen at the IL to ASM stage. Use the signed version unless
        /// you definitely need to deal with numbers larger than long.MaxValue.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteDigits(ulong value, int digitCount, ref byte buffer, int index)
        {
            ulong left = value;

            for (int i = digitCount - 1; i >= 0; i--)
            {
                left = DivMod(left, 10, out ulong num);
                Unsafe.Add(ref buffer, index + i) = (byte)('0' + num);
            }

            Debug.Assert(left == 0);
        }

        #endregion UTF-8 Helper methods

        #region Math Helper methods

        /// <summary>
        /// We don't have access to Math.DivRem, so this is a copy of the implementation.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long DivMod(long numerator, long denominator, out long modulo)
        {
            long div = numerator / denominator;
            modulo = numerator - (div * denominator);
            return div;
        }

        /// <summary>
        /// We don't have access to Math.DivRem, so this is a copy of the implementation.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong DivMod(ulong numerator, ulong denominator, out ulong modulo)
        {
            ulong div = numerator / denominator;
            modulo = numerator - (div * denominator);
            return div;
        }

        /// <summary>
        /// We don't have access to Math.DivRem, so this is a copy of the implementation.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint DivMod(uint numerator, uint denominator, out uint modulo)
        {
            uint div = numerator / denominator;
            modulo = numerator - (div * denominator);
            return div;
        }

        #endregion Math Helper methods

        #region Character counting helper methods

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CountDigits(long n)
        {
            if (n == 0)
                return 1;

            int digits = 0;
            while (n != 0)
            {
                n /= 10;
                digits++;
            }

            return digits;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CountDigits(ulong value)
        {
            int digits = 1;
            uint part;
            if (value >= 10000000)
            {
                if (value >= 100000000000000)
                {
                    part = (uint)(value / 100000000000000);
                    digits += 14;
                }
                else
                {
                    part = (uint)(value / 10000000);
                    digits += 7;
                }
            }
            else
            {
                part = (uint)value;
            }

            if (part < 10)
            {
                // no-op
            }
            else if (part < 100)
            {
                digits += 1;
            }
            else if (part < 1000)
            {
                digits += 2;
            }
            else if (part < 10000)
            {
                digits += 3;
            }
            else if (part < 100000)
            {
                digits += 4;
            }
            else if (part < 1000000)
            {
                digits += 5;
            }
            else
            {
                Debug.Assert(part < 10000000);
                digits += 6;
            }

            return digits;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CountDigits(uint value)
        {
            int digits = 1;
            if (value >= 100000)
            {
                value = value / 100000;
                digits += 5;
            }

            if (value < 10)
            {
                // no-op
            }
            else if (value < 100)
            {
                digits += 1;
            }
            else if (value < 1000)
            {
                digits += 2;
            }
            else if (value < 10000)
            {
                digits += 3;
            }
            else
            {
                Debug.Assert(value < 100000);
                digits += 4;
            }

            return digits;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CountFractionDigits(long n)
        {
            Debug.Assert(n >= 0);

            long left = n;
            long m = 0;
            int count = FractionDigits;

            // Remove all the 0 (zero) values from the right.
            while (left > 0 && m == 0 && count > 0)
            {
                left = DivMod(left, 10, out m);
                count--;
            }

            return count + 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CountHexDigits(ulong value)
        {
            // TODO: When x86 intrinsic support comes online, experiment with implementing this using lzcnt.
            // return 16 - (int)((uint)Lzcnt.LeadingZeroCount(value | 1) >> 3);

            int digits = 1;

            if (value > 0xFFFFFFFF)
            {
                digits += 8;
                value >>= 0x20;
            }
            if (value > 0xFFFF)
            {
                digits += 4;
                value >>= 0x10;
            }
            if (value > 0xFF)
            {
                digits += 2;
                value >>= 0x8;
            }
            if (value > 0xF)
                digits++;

            return digits;
        }

        #endregion Character counting helper methods
    }
}
