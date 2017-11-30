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
        private static bool TryFormatUInt64Default(ulong value, Span<byte> buffer, out int bytesWritten)
        {
            if (value < 10)
            {
                if (buffer.Length == 0) goto FalseExit;
                buffer[0] = (byte)('0' + value);
                bytesWritten = 1;
                return true;
            }

#if BIT32
            if (value <= uint.MaxValue)
            {
                int digitCountOnePart = FormattingHelpers.CountDigits((uint)value);
                if (digitCountOnePart > buffer.Length) goto FalseExit;
                bytesWritten = digitCountOnePart;
                // WriteDigits does not do bounds checks
                FormattingHelpers.WriteDigits((uint)value, buffer.Slice(0, digitCountOnePart));
                return true;
            }
            else
            {
                if (value < 1000000000000000000)
                {
                    ulong part1 = value / 1000000000;
                    ulong part2 = value - (part1 * 1000000000);

                    int digitCount1 = FormattingHelpers.CountDigits((uint)part1);
                    int digitCount2 = 9;
                    int digitCountTwoParts = digitCount1 + digitCount2;
                    if (digitCountTwoParts > buffer.Length) goto FalseExit;
                    bytesWritten = digitCountTwoParts;
                    // WriteDigits does not do bounds checks
                    FormattingHelpers.WriteDigits((uint)part1, buffer.Slice(0, digitCount1));
                    FormattingHelpers.WriteDigits((uint)part2, buffer.Slice(digitCount1, digitCount2));
                    return true;

                }
                else
                {
                    ulong part1 = value / 1000000000000000000;
                    ulong partial = value - (part1 * 1000000000000000000);
                    ulong part2 = partial / 1000000000;
                    ulong part3 = partial - (part2 * 1000000000);

                    int digitCount1 = FormattingHelpers.CountDigits((uint)part1);
                    int digitCount2 = 9;
                    int digitCount3 = 9;
                    int digitCountThreeParts = digitCount1 + digitCount2 + digitCount3;
                    if (digitCountThreeParts > buffer.Length) goto FalseExit;
                    bytesWritten = digitCountThreeParts;
                    // WriteDigits does not do bounds checks
                    FormattingHelpers.WriteDigits((uint)part1, buffer.Slice(0, digitCount1));
                    FormattingHelpers.WriteDigits((uint)part2, buffer.Slice(digitCount1, digitCount2));
                    FormattingHelpers.WriteDigits((uint)part3, buffer.Slice(digitCount1 + digitCount2, digitCount3));
                    return true;
                }
            }
#else
            int digitCount = FormattingHelpers.CountDigits(value);
            if (digitCount > buffer.Length) goto FalseExit;
            bytesWritten = digitCount;
            // WriteDigits does not do bounds checks
            FormattingHelpers.WriteDigits(value, buffer.Slice(0, digitCount));
            return true;
#endif

        FalseExit:
            bytesWritten = 0;
            return false;
        }

        // TODO: Use this instead of TryFormatUInt64Default to format numbers less than uint.MaxValue for BIT32
        private static bool TryFormatUInt32Default(uint value, Span<byte> buffer, out int bytesWritten)
        {
            if (value < 10)
            {
                if (buffer.Length == 0) goto FalseExit;
                buffer[0] = (byte)('0' + value);
                bytesWritten = 1;
                return true;
            }
            
            int digitCount = FormattingHelpers.CountDigits(value);
            if (digitCount > buffer.Length) goto FalseExit;
            bytesWritten = digitCount;
            // WriteDigits does not do bounds checks
            FormattingHelpers.WriteDigits(value, buffer.Slice(0, digitCount));
            return true;

        FalseExit:
            bytesWritten = 0;
            return false;
        }
    }
}
