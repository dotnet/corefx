// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace System.Buffers.Text
{
    /// <summary>
    /// Methods to format common data types as Utf8 strings.
    /// </summary>
    public static partial class Utf8Formatter
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool TryFormatUInt64Default(ulong value, Span<byte> buffer, out int bytesWritten)
        {
            if (value < 10)
            {
                return TryFormatUInt32SingleDigit((uint)value, buffer, out bytesWritten);
            }
#if BIT32
            if (value <= uint.MaxValue)
            {
                return TryFormatUInt32MultipleDigits((uint)value, buffer, out bytesWritten);
            }
            else
            {
                if (value < Utf8Constants.Quintillion)
                {
                    return TryFormatUInt64LessThanQuintillion(value, buffer, out bytesWritten);
                }
                else
                {
                    return TryFormatUInt64MoreThanQuintillion(value, buffer, out bytesWritten);
                }
            }
#else
            return TryFormatUInt64MultipleDigits(value, buffer, out bytesWritten);
#endif // BIT32
        }

        // TODO: Use this instead of TryFormatUInt64Default to format numbers less than uint.MaxValue for BIT32
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool TryFormatUInt32Default(uint value, Span<byte> buffer, out int bytesWritten)
        {
            if (value < 10)
            {
                return TryFormatUInt32SingleDigit(value, buffer, out bytesWritten);
            }
            return TryFormatUInt32MultipleDigits(value, buffer, out bytesWritten);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool TryFormatUInt32SingleDigit(uint value, Span<byte> buffer, out int bytesWritten)
        {
            if (buffer.Length == 0)
            {
                bytesWritten = 0;
                return false;
            }
            buffer[0] = (byte)('0' + value);
            bytesWritten = 1;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool TryFormatUInt32MultipleDigits(uint value, Span<byte> buffer, out int bytesWritten)
        {
            int digitCount = FormattingHelpers.CountDigits(value);
            // WriteDigits does not do bounds checks
            if (digitCount > buffer.Length)
            {
                bytesWritten = 0;
                return false;
            }
            FormattingHelpers.WriteDigits(value, buffer.Slice(0, digitCount));
            bytesWritten = digitCount;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool TryFormatUInt64SingleDigit(ulong value, Span<byte> buffer, out int bytesWritten)
        {
            if (buffer.Length == 0)
            {
                bytesWritten = 0;
                return false;
            }
            buffer[0] = (byte)('0' + value);
            bytesWritten = 1;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool TryFormatUInt64MultipleDigits(ulong value, Span<byte> buffer, out int bytesWritten)
        {
            int digitCount = FormattingHelpers.CountDigits(value);
            // WriteDigits does not do bounds checks
            if (digitCount > buffer.Length)
            {
                bytesWritten = 0;
                return false;
            }
            FormattingHelpers.WriteDigits(value, buffer.Slice(0, digitCount));
            bytesWritten = digitCount;
            return true;
        }

        // Split ulong into two parts that can each fit in a uint - {1-9 digits}{9 digits}
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool TryFormatUInt64LessThanQuintillion(ulong value, Span<byte> buffer, out int bytesWritten)
        {
            ulong OverNineDigits = value / Utf8Constants.Billion;
            ulong LastNineDigits = value - (OverNineDigits * Utf8Constants.Billion);

            int digitCountOverNineDigits = FormattingHelpers.CountDigits((uint)OverNineDigits);
            Debug.Assert(digitCountOverNineDigits >= 1 && digitCountOverNineDigits <= 9);
            int digitCount = digitCountOverNineDigits + 9;
            // WriteDigits does not do bounds checks
            if (digitCount > buffer.Length)
            {
                bytesWritten = 0;
                return false;
            }
            bytesWritten = digitCount;
            FormattingHelpers.WriteDigits((uint)OverNineDigits, buffer.Slice(0, digitCountOverNineDigits));
            FormattingHelpers.WriteDigits((uint)LastNineDigits, buffer.Slice(digitCountOverNineDigits, 9));
            return true;
        }

        // Split ulong into three parts that can each fit in a uint - {1-2 digits}{9 digits}{9 digits}
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool TryFormatUInt64MoreThanQuintillion(ulong value, Span<byte> buffer, out int bytesWritten)
        {
            ulong OverEighteenDigits = value / Utf8Constants.Quintillion;
            ulong LastEighteenDigits = value - (OverEighteenDigits * Utf8Constants.Quintillion);
            ulong MiddleNineDigits = LastEighteenDigits / Utf8Constants.Billion;
            ulong LastNineDigits = LastEighteenDigits - (MiddleNineDigits * Utf8Constants.Billion);

            int digitCountOverEighteenDigits = FormattingHelpers.CountDigits((uint)OverEighteenDigits);
            Debug.Assert(digitCountOverEighteenDigits >= 1 && digitCountOverEighteenDigits <= 2);
            int digitCount = digitCountOverEighteenDigits + 18;
            // WriteDigits does not do bounds checks
            if (digitCount > buffer.Length)
            {
                bytesWritten = 0;
                return false;
            }
            bytesWritten = digitCount;
            FormattingHelpers.WriteDigits((uint)OverEighteenDigits, buffer.Slice(0, digitCountOverEighteenDigits));
            FormattingHelpers.WriteDigits((uint)MiddleNineDigits, buffer.Slice(digitCountOverEighteenDigits, 9));
            FormattingHelpers.WriteDigits((uint)LastNineDigits, buffer.Slice(digitCountOverEighteenDigits + 9, 9));
            return true;
        }
    }
}
