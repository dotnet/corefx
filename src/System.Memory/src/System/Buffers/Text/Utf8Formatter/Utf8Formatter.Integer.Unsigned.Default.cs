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
        private static bool TryFormatUInt64Default(ulong value, Span<byte> buffer, out int bytesWritten)
        {
            ref byte utf8Bytes = ref buffer.DangerousGetPinnableReference();

            ulong left = value;
            for (int i = 0; i < buffer.Length; i++)
            {
                left = FormattingHelpers.DivMod(left, 10, out ulong num);
                Unsafe.Add(ref utf8Bytes, i) = (byte)('0' + num);
                if (left == 0)
                {
                    i++;
                    // Reverse the bytes
                    for (int j = 0; j < (i >> 1); j++)
                    {
                        byte temp = Unsafe.Add(ref utf8Bytes, j);
                        Unsafe.Add(ref utf8Bytes, j) = Unsafe.Add(ref utf8Bytes, i - j - 1);
                        Unsafe.Add(ref utf8Bytes, i - j - 1) = temp;
                    }
                    bytesWritten = i;
                    return true;
                }
            }

            // Buffer too small, clean up what has been written
            buffer.Clear();
            bytesWritten = 0;
            return false;
        }
    }
}
