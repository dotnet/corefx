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
            int idx = 0;

            if (value < 0)
            {
                if (buffer.Length < 2) goto FalseExit;  // Buffer of length 1 won't have space for the digits after the minus sign
                buffer[idx++] = Utf8Constants.Minus;

                // Abs(long.MinValue) == long.MaxValue + 1, so we need to handle this specially.
                if (value == long.MinValue)
                {
                    // digitCount + idx = 19 + 1 = 20
                    if (buffer.Length < 19 + idx) goto FalseExit;  // WriteDigits does not do bounds checks
                    buffer[idx] = (byte)'9';
                    // Already wrote '9', 19 - 1 = 18 digits left
                    FormattingHelpers.WriteDigits(223372036854775808L, 18, ref buffer.DangerousGetPinnableReference(), idx + 1);
                    bytesWritten = 19 + idx;
                    return true;
                }

                value = -value;
            }

            if (value < 10)
            {
                if (buffer.Length == 0) goto FalseExit;
                buffer[idx] = (byte)('0' + value);
                bytesWritten = 1 + idx;
                return true;
            }

            int digitCount = FormattingHelpers.CountDigits((ulong)value);
            if (buffer.Length < digitCount + idx) goto FalseExit;  // WriteDigits does not do bounds checks
            FormattingHelpers.WriteDigits(value, digitCount, ref buffer.DangerousGetPinnableReference(), idx);
            bytesWritten = digitCount + idx;
            return true;

        FalseExit:
            // Buffer too small, clean up what has been written
            buffer.Clear();
            bytesWritten = 0;
            return false;
        }
    }
}
