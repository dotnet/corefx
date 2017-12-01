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

#if BIT32
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool TryFormatInt64Default(long value, Span<byte> buffer, out int bytesWritten)
        {
            if ((ulong)value < 10)
            {
                return TryFormatUInt32SingleDigit((uint)value, buffer, out bytesWritten);
            }

            if (value <= int.MaxValue && value >= int.MinValue)
            {
                return TryFormatInt32MultipleDigits((int)value, buffer, out bytesWritten);
            }
            else
            {
                if (value <= Utf8Constants.Quintillion && value >= -Utf8Constants.Quintillion)
                {
                    return TryFormatInt64LessThanQuintillion(value, buffer, out bytesWritten);
                }
                else
                {
                    return TryFormatInt64MoreThanQuintillion(value, buffer, out bytesWritten);
                }
            }
        }
#else
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool TryFormatInt64Default(long value, Span<byte> buffer, out int bytesWritten)
        {
            if ((ulong)value < 10)
            {
                return TryFormatUInt32SingleDigit((uint)value, buffer, out bytesWritten);
            }
            return TryFormatInt64MultipleDigits(value, buffer, out bytesWritten);
        }
#endif // BIT32


        // TODO: Use this instead of TryFormatInt64Default to format numbers less than int.MaxValue for BIT32
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool TryFormatInt32Default(int value, Span<byte> buffer, out int bytesWritten)
        {
            if ((uint)value < 10)
            {
                return TryFormatUInt32SingleDigit((uint)value, buffer, out bytesWritten);
            }
            return TryFormatInt32MultipleDigits(value, buffer, out bytesWritten);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool TryFormatInt32MultipleDigits(int value, Span<byte> buffer, out int bytesWritten)
        {
            if (value < 0)
            {
                value = -value;
                int digitCount = FormattingHelpers.CountDigits((uint)value);
                // WriteDigits does not do bounds checks
                if (digitCount >= buffer.Length)
                {
                    bytesWritten = 0;
                    return false;
                }
                buffer[0] = Utf8Constants.Minus;
                FormattingHelpers.WriteDigits((uint)value, buffer.Slice(1, digitCount));
                bytesWritten = digitCount + 1;
                return true;
            }
            else
            {
                return TryFormatUInt32MultipleDigits((uint)value, buffer, out bytesWritten);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool TryFormatInt64MultipleDigits(long value, Span<byte> buffer, out int bytesWritten)
        {
            if (value < 0)
            {
                value = -value;
                int digitCount = FormattingHelpers.CountDigits((ulong)value);
                // WriteDigits does not do bounds checks
                if (digitCount >= buffer.Length)
                {
                    bytesWritten = 0;
                    return false;
                }
                buffer[0] = Utf8Constants.Minus;
                FormattingHelpers.WriteDigits((ulong)value, buffer.Slice(1, digitCount));
                bytesWritten = digitCount + 1;
                return true;
            }
            else
            {
                return TryFormatUInt64MultipleDigits((ulong)value, buffer, out bytesWritten);
            }
        }

        // Split long into two parts that can each fit in a uint - {1-9 digits}{9 digits}
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool TryFormatInt64LessThanQuintillion(long value, Span<byte> buffer, out int bytesWritten)
        {
            if (value < 0)
            {
                value = -value;
                ulong OverNineDigits = (ulong)value / Utf8Constants.Billion;
                ulong LastNineDigits = (ulong)value - (OverNineDigits * Utf8Constants.Billion);

                int digitCountOverNineDigits = FormattingHelpers.CountDigits((uint)OverNineDigits);
                Debug.Assert(digitCountOverNineDigits >= 1 && digitCountOverNineDigits <= 9);
                int digitCount = digitCountOverNineDigits + 9;
                // WriteDigits does not do bounds checks
                if (digitCount >= buffer.Length)
                {
                    bytesWritten = 0;
                    return false;
                }
                buffer[0] = Utf8Constants.Minus;
                FormattingHelpers.WriteDigits((uint)OverNineDigits, buffer.Slice(1, digitCountOverNineDigits));
                FormattingHelpers.WriteDigits((uint)LastNineDigits, buffer.Slice(digitCountOverNineDigits + 1, 9));
                bytesWritten = digitCount + 1;
                return true;
            }
            else
            {
                return TryFormatUInt64LessThanQuintillion((ulong)value, buffer, out bytesWritten); 
            }
        }

        // Split long into three parts that can each fit in a uint - {1 digit}{9 digits}{9 digits}
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool TryFormatInt64MoreThanQuintillion(long value, Span<byte> buffer, out int bytesWritten)
        {
            if (value < 0)
            {
                value = -value;
                ulong OverEighteenDigits = (ulong)value / Utf8Constants.Quintillion;
                ulong LastEighteenDigits = (ulong)value - (OverEighteenDigits * Utf8Constants.Quintillion);
                ulong MiddleNineDigits = LastEighteenDigits / Utf8Constants.Billion;
                ulong LastNineDigits = LastEighteenDigits - (MiddleNineDigits * Utf8Constants.Billion);

                int digitCountOverEighteenDigits = FormattingHelpers.CountDigits((uint)OverEighteenDigits);
                Debug.Assert(digitCountOverEighteenDigits == 1);
                int digitCount = digitCountOverEighteenDigits + 18;
                // WriteDigits does not do bounds checks
                if (digitCount >= buffer.Length)
                {
                    bytesWritten = 0;
                    return false;
                }
                buffer[0] = Utf8Constants.Minus;
                FormattingHelpers.WriteDigits((uint)OverEighteenDigits, buffer.Slice(1, digitCountOverEighteenDigits));
                FormattingHelpers.WriteDigits((uint)MiddleNineDigits, buffer.Slice(digitCountOverEighteenDigits + 1, 9));
                FormattingHelpers.WriteDigits((uint)LastNineDigits, buffer.Slice(digitCountOverEighteenDigits + 1 + 9, 9));
                bytesWritten = digitCount + 1;
                return true;
            }
            else
            {
                return TryFormatUInt64MoreThanQuintillion((ulong)value, buffer, out bytesWritten);
            }
        }
    }
}
