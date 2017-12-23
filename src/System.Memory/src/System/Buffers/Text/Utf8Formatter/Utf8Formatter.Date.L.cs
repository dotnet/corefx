// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#if !netstandard
using Internal.Runtime.CompilerServices;
#endif

namespace System.Buffers.Text
{
    public static partial class Utf8Formatter
    {
        // Rfc1123 - lowercase
        //
        //   01234567890123456789012345678
        //   -----------------------------
        //   tue, 03 jan 2017 08:08:05 gmt
        //
        private static bool TryFormatDateTimeL(DateTime value, Span<byte> buffer, out int bytesWritten)
        {
            bytesWritten = 29;
            if (buffer.Length < bytesWritten)
            {
                bytesWritten = 0;
                return false;
            }

            ref byte utf8Bytes = ref MemoryMarshal.GetReference(buffer);

            byte[] dayAbbrev = DayAbbreviationsLowercase[(int)value.DayOfWeek];
            Unsafe.Add(ref utf8Bytes, 0) = dayAbbrev[0];
            Unsafe.Add(ref utf8Bytes, 1) = dayAbbrev[1];
            Unsafe.Add(ref utf8Bytes, 2) = dayAbbrev[2];
            Unsafe.Add(ref utf8Bytes, 3) = Utf8Constants.Comma;
            Unsafe.Add(ref utf8Bytes, 4) = Utf8Constants.Space;

            FormattingHelpers.WriteDigits(value.Day, 2, ref utf8Bytes, 5);
            Unsafe.Add(ref utf8Bytes, 7) = (byte)' ';

            byte[] monthAbbrev = MonthAbbreviationsLowercase[value.Month - 1];
            Unsafe.Add(ref utf8Bytes, 8) = monthAbbrev[0];
            Unsafe.Add(ref utf8Bytes, 9) = monthAbbrev[1];
            Unsafe.Add(ref utf8Bytes, 10) = monthAbbrev[2];
            Unsafe.Add(ref utf8Bytes, 11) = Utf8Constants.Space;

            FormattingHelpers.WriteDigits(value.Year, 4, ref utf8Bytes, 12);
            Unsafe.Add(ref utf8Bytes, 16) = Utf8Constants.Space;

            FormattingHelpers.WriteDigits(value.Hour, 2, ref utf8Bytes, 17);
            Unsafe.Add(ref utf8Bytes, 19) = Utf8Constants.Colon;

            FormattingHelpers.WriteDigits(value.Minute, 2, ref utf8Bytes, 20);
            Unsafe.Add(ref utf8Bytes, 22) = Utf8Constants.Colon;

            FormattingHelpers.WriteDigits(value.Second, 2, ref utf8Bytes, 23);
            Unsafe.Add(ref utf8Bytes, 25) = Utf8Constants.Space;

            Unsafe.Add(ref utf8Bytes, 26) = GMT1Lowercase;
            Unsafe.Add(ref utf8Bytes, 27) = GMT2Lowercase;
            Unsafe.Add(ref utf8Bytes, 28) = GMT3Lowercase;

            return true;
        }
    }
}
