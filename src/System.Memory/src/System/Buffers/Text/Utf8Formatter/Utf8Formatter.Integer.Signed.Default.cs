// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;

namespace System.Buffers.Text
{
    /// <summary>
    /// Methods to format common data types as Utf8 strings.
    /// </summary>
    public static partial class Utf8Formatter
    {
        private static bool TryFormatInt64Default(long value, Span<byte> buffer, out int bytesWritten)
        {
            ref byte utf8Bytes = ref buffer.DangerousGetPinnableReference();
            int idx = 0;

            if (value < 0)
            {
                if (buffer.Length < 2) goto FalseExit;  // Buffer of length 1 won't have space for the digits after the minus sign
                Unsafe.Add(ref utf8Bytes, idx++) = Utf8Constants.Minus;

                // Abs(long.MinValue) == long.MaxValue + 1, so we need to handle this specially.
                if (value == long.MinValue)
                {
                    if (buffer.Length < 20) goto FalseExit; // WriteDigits does not do bounds checks
                    Unsafe.Add(ref utf8Bytes, 1) = (byte)'9';
                    FormattingHelpers.WriteDigits(223372036854775808L, 18, ref utf8Bytes, 2);
                    bytesWritten = 20;
                    return true;
                }

                value = -value;
            }

            long left = value;
            for (int i = idx; i < buffer.Length; i++)
            {
                left = FormattingHelpers.DivMod(left, 10, out long num);
                Unsafe.Add(ref utf8Bytes, i) = (byte)('0' + num);
                if (left == 0)
                {
                    i++;
                    // Reverse the bytes
                    for (int j = 0; j < ((i - idx) >> 1); j++)
                    {
                        byte temp = Unsafe.Add(ref utf8Bytes, j + idx);
                        Unsafe.Add(ref utf8Bytes, j + idx) = Unsafe.Add(ref utf8Bytes, i - j - 1);
                        Unsafe.Add(ref utf8Bytes, i - j - 1) = temp;
                    }
                    bytesWritten = i;
                    return true;
                }
            }

            // Buffer too small, clean up what has been written
            buffer.Clear();
        FalseExit:
            bytesWritten = 0;
            return false;
        }
    }
}
