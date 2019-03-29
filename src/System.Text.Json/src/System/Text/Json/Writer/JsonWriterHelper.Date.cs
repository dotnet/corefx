﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace System.Text.Json
{
    internal static partial class JsonWriterHelper
    {
        public static void TrimDateTime(Span<byte> buffer, out int bytesWritten)
        {
            Trim(buffer, out bytesWritten);
        }

        public static void TrimDateTimeOffset(Span<byte> buffer, out int bytesWritten)
        {
            Trim(buffer, out bytesWritten);
        }

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
        private static void Trim(Span<byte> buffer, out int bytesWritten)
        {
            // Assert buffer is the right length for:
            // YYYY-MM-DDThh:mm:ss.fffffff (JsonConstants.MaximumFormatDateTimeLength)
            // YYYY-MM-DDThh:mm:ss.fffffffZ (JsonConstants.MaximumFormatDateTimeLength + 1)
            // YYYY-MM-DDThh:mm:ss.fffffff(+|-)hh:mm (JsonConstants.MaximumFormatDateTimeOffsetLength)
            Debug.Assert(buffer.Length == JsonConstants.MaximumFormatDateTimeLength ||
                buffer.Length == (JsonConstants.MaximumFormatDateTimeLength + 1) ||
                buffer.Length == JsonConstants.MaximumFormatDateTimeOffsetLength);

            uint digit7 = buffer[26] - (uint)'0';
            uint digit6 = buffer[25] - (uint)'0';
            uint digit5 = buffer[24] - (uint)'0';
            uint digit4 = buffer[23] - (uint)'0';
            uint digit3 = buffer[22] - (uint)'0';
            uint digit2 = buffer[21] - (uint)'0';
            uint digit1 = buffer[20] - (uint)'0';
            uint fraction = (digit1 * 1_000_000) + (digit2 * 100_000) + (digit3 * 10_000) + (digit4 * 1_000) + (digit5 * 100) + (digit6 * 10) + digit7;

            // The period's index
            int curIndex = 19;

            if (fraction > 0)
            {
                int numFractionDigits = 7;
                // Remove trailing zeros
                while (fraction % 10 == 0)
                {
                    fraction /= 10;
                    numFractionDigits -= 1;
                }

                // The last fraction digit's index will be (the period's index plus one) + (the number of fraction digits minus one)
                int fractionEnd = 19 + numFractionDigits;

                // Write fraction
                for (int i = fractionEnd; i >= curIndex; i--)
                {
                    buffer[i] = (byte)((fraction % 10) + (uint)'0');
                    fraction /= 10;
                }

                curIndex = fractionEnd + 1;
            }

            bytesWritten = curIndex;

            if (buffer.Length > JsonConstants.MaximumFormatDateTimeLength)
            {
                // Write offset

                buffer[curIndex] = buffer[27];

                bytesWritten = curIndex + 1;

                if (buffer.Length == JsonConstants.MaximumFormatDateTimeOffsetLength)
                {
                    int bufferEnd = curIndex + 5;

                    buffer[bufferEnd] = buffer[32];
                    buffer[bufferEnd - 1] = buffer[31];
                    buffer[bufferEnd - 2] = buffer[30];
                    buffer[bufferEnd - 3] = buffer[29];
                    buffer[bufferEnd - 4] = buffer[28];

                    bytesWritten = bufferEnd + 1;
                }
            }
        }
    }
}
