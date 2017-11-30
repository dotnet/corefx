// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Buffers.Text
{
    /// <summary>
    /// Methods to format common data types as Utf8 strings.
    /// </summary>
    public static partial class Utf8Formatter
    {
        private static bool TryFormatInt64Default(long value, Span<byte> buffer, out int bytesWritten)
        {
            if ((ulong)value < 10)
            {
                if (buffer.Length == 0) goto FalseExit;
                buffer[0] = (byte)('0' + value);
                bytesWritten = 1;
                return true;
            }

#if BIT32
            if (value <= int.MaxValue && value >= int.MinValue)
            {
                if (value < 0)
                {
                    value = -value;
                    int digitCount = FormattingHelpers.CountDigits((uint)value);
                    if (digitCount >= buffer.Length) goto FalseExit;
                    bytesWritten = digitCount + 1;
                    buffer[0] = Utf8Constants.Minus;
                    buffer = buffer.Slice(1, digitCount);
                }
                else
                {
                    int digitCount = FormattingHelpers.CountDigits((uint)value);
                    if (digitCount > buffer.Length) goto FalseExit;
                    bytesWritten = digitCount;
                    buffer = buffer.Slice(0, digitCount);
                }

                // WriteDigits does not do bounds checks
                FormattingHelpers.WriteDigits((uint)value, buffer);
            }
            else
            {
                if (value < 1000000000000000000 && value > -1000000000000000000)
                {
                    if (value < 0)
                    {
                        value = -value;
                        ulong part1 = (ulong)value / 1000000000;
                        ulong part2 = (ulong)value - (part1 * 1000000000);
                        int digitCount1 = FormattingHelpers.CountDigits((uint)part1);
                        int digitCount2 = 9;
                        int digitCountTwoParts = digitCount1 + digitCount2;
                        if (digitCountTwoParts >= buffer.Length) goto FalseExit;
                        bytesWritten = digitCountTwoParts + 1;
                        buffer[0] = Utf8Constants.Minus;
                        buffer = buffer.Slice(1, digitCountTwoParts);
                        
                        // WriteDigits does not do bounds checks
                        FormattingHelpers.WriteDigits((uint)part1, buffer.Slice(0, digitCount1));
                        FormattingHelpers.WriteDigits((uint)part2, buffer.Slice(digitCount1, digitCount2));
                    }
                    else
                    {
                        ulong part1 = (ulong)value / 1000000000;
                        ulong part2 = (ulong)value - (part1 * 1000000000);
                        int digitCount1 = FormattingHelpers.CountDigits((uint)part1);
                        int digitCount2 = 9;
                        int digitCountTwoParts = digitCount1 + digitCount2;
                        if (digitCountTwoParts > buffer.Length) goto FalseExit;
                        bytesWritten = digitCountTwoParts;
                        buffer = buffer.Slice(0, digitCountTwoParts);
                        
                        // WriteDigits does not do bounds checks
                        FormattingHelpers.WriteDigits((uint)part1, buffer.Slice(0, digitCount1));
                        FormattingHelpers.WriteDigits((uint)part2, buffer.Slice(digitCount1, digitCount2));
                    }
                }
                else
                {
                    if (value < 0)
                    {
                        value = -value;
                        ulong part1 = (ulong)value / 1000000000000000000;
                        ulong partial = (ulong)value - (part1 * 1000000000000000000);
                        ulong part2 = partial / 1000000000;
                        ulong part3 = partial - (part2 * 1000000000);
                        int digitCount1 = FormattingHelpers.CountDigits((uint)part1);
                        int digitCount2 = 9;
                        int digitCount3 = 9;
                        int digitCountThreeParts = digitCount1 + digitCount2 + digitCount3;
                        if (digitCountThreeParts >= buffer.Length) goto FalseExit;
                        bytesWritten = digitCountThreeParts + 1;
                        buffer[0] = Utf8Constants.Minus;
                        buffer = buffer.Slice(1, digitCountThreeParts);
                        
                        // WriteDigits does not do bounds checks
                        FormattingHelpers.WriteDigits((uint)part1, buffer.Slice(0, digitCount1));
                        FormattingHelpers.WriteDigits((uint)part2, buffer.Slice(digitCount1, digitCount2));
                        FormattingHelpers.WriteDigits((uint)part3, buffer.Slice(digitCount1 + digitCount2, digitCount3));
                    }
                    else
                    {
                        ulong part1 = (ulong)value / 1000000000000000000;
                        ulong partial = (ulong)value - (part1 * 1000000000000000000);
                        ulong part2 = partial / 1000000000;
                        ulong part3 = partial - (part2 * 1000000000);
                        int digitCount1 = FormattingHelpers.CountDigits((uint)part1);
                        int digitCount2 = 9;
                        int digitCount3 = 9;
                        int digitCountThreeParts = digitCount1 + digitCount2 + digitCount3;
                        if (digitCountThreeParts > buffer.Length) goto FalseExit;
                        bytesWritten = digitCountThreeParts;
                        buffer = buffer.Slice(0, digitCountThreeParts);
                        // WriteDigits does not do bounds checks
                        FormattingHelpers.WriteDigits((uint)part1, buffer.Slice(0, digitCount1));
                        FormattingHelpers.WriteDigits((uint)part2, buffer.Slice(digitCount1, digitCount2));
                        FormattingHelpers.WriteDigits((uint)part3, buffer.Slice(digitCount1 + digitCount2, digitCount3));
                    }
                }
            }
            return true;
#else
            if (value < 0)
            {
                value = -value;
                int digitCount = FormattingHelpers.CountDigits((ulong)value);
                if (digitCount >= buffer.Length) goto FalseExit;
                bytesWritten = digitCount + 1;
                buffer[0] = Utf8Constants.Minus;
                buffer = buffer.Slice(1, digitCount);
            }
            else
            {
                int digitCount = FormattingHelpers.CountDigits((ulong)value);
                if (digitCount > buffer.Length) goto FalseExit;
                bytesWritten = digitCount;
                buffer = buffer.Slice(0, digitCount);
            }

            // WriteDigits does not do bounds checks
            FormattingHelpers.WriteDigits((ulong)value, buffer);
            return true;
#endif

        FalseExit:
            bytesWritten = 0;
            return false;
        }

        // TODO: Use this instead of TryFormatInt64Default to format numbers less than int.MaxValue for BIT32
        private static bool TryFormatInt32Default(int value, Span<byte> buffer, out int bytesWritten)
        {
            if ((uint)value < 10)
            {
                if (buffer.Length == 0) goto FalseExit;
                buffer[0] = (byte)('0' + value);
                bytesWritten = 1;
                return true;
            }
            
            if (value < 0)
            {
                value = -value;
                int digitCount = FormattingHelpers.CountDigits((uint)value);
                if (digitCount >= buffer.Length) goto FalseExit;
                bytesWritten = digitCount + 1;
                buffer[0] = Utf8Constants.Minus;
                buffer = buffer.Slice(1, digitCount);
            }
            else
            {
                int digitCount = FormattingHelpers.CountDigits((uint)value);
                if (digitCount > buffer.Length) goto FalseExit;
                bytesWritten = digitCount;
                buffer = buffer.Slice(0, digitCount);
            }

            // WriteDigits does not do bounds checks
            FormattingHelpers.WriteDigits((uint)value, buffer);
            return true;

        FalseExit:
            bytesWritten = 0;
            return false;
        }
    }
}
