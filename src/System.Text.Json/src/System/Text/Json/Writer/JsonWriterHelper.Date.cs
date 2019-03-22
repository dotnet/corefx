// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace System.Text.Json
{
    internal static partial class JsonWriterHelper
    {
        //
        // Trims roundtrippable DateTime(Offset) input.
        // If the milliseconds part of the date is zero, we omit the fraction part of the date,
        // else we write the fraction up to 7 decimal places with no trailing zeros. i.e. the output format is
        // YYYY-MM-DDThh:mm:ss[.s]TZD where TZD = Z or +-hh:mm.
        // e.g.
        //   ---------------------------------
        //   2017-06-12T05:30:45.768-07:00
        //   2017-06-12T05:30:45.00768Z           (Z is short for "+00:00" but also distinguishes DateTimeKind.Utc from DateTimeKind.Local)
        //   2017-06-12T05:30:45                  (interpreted as local time wrt to current time zone)
        public static void TrimDateTimeOffsetString(Span<byte> buffer, out int bytesWritten)
        {
            Debug.Assert(buffer.Length == JsonConstants.MaximumFormatDateTimeLength ||
                buffer.Length == (JsonConstants.MaximumFormatDateTimeLength + 1) ||
                buffer.Length == JsonConstants.MaximumFormatDateTimeOffsetLength);

            uint digit1 = buffer[20] - (uint)'0';
            uint digit2 = buffer[21] - (uint)'0';
            uint digit3 = buffer[22] - (uint)'0';
            uint digit4 = buffer[23] - (uint)'0';
            uint digit5 = buffer[24] - (uint)'0';
            uint digit6 = buffer[25] - (uint)'0';
            uint digit7 = buffer[26] - (uint)'0';
            uint fraction = (digit1 * 1_000_000) + (digit2 * 100_000) + (digit3 * 10_000) + (digit4 * 1_000) + (digit5 * 100) + (digit6 * 10) + digit7;

            int curIndex = 19;

            if (fraction > 0)
            {
                // Remove trailing zeros
                while (fraction % 10 == 0)
                {
                    fraction /= 10;
                }

                buffer[curIndex++] = (byte)'.';

                // The last fraction digit's index will be the current index + (the number of fraction digits minus one)
                int fractionEnd = curIndex + ((int)Math.Floor(Math.Log10(fraction) + 1) - 1);

                // Write fraction
                for (int i = fractionEnd; i >= curIndex; i--)
                {
                    buffer[i] = (byte)((fraction % 10) + (uint)'0');
                    fraction /= 10;
                }

                curIndex = fractionEnd + 1;
            }

            if (buffer.Length > JsonConstants.MaximumFormatDateTimeLength)
            {
                // Write offset
                buffer[curIndex++] = buffer[27];

                if (buffer.Length == JsonConstants.MaximumFormatDateTimeOffsetLength)
                {
                    buffer[curIndex++] = buffer[28];
                    buffer[curIndex++] = buffer[29];
                    buffer[curIndex++] = buffer[30];
                    buffer[curIndex++] = buffer[31];
                    buffer[curIndex++] = buffer[32];
                }
            }

            bytesWritten = curIndex;
        }
    }
}
