// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace System.Text.Json
{
    internal static partial class JsonWriterHelper
    {
        public static bool TryFormatToISO(DateTimeOffset value, Span<byte> destination, out int bytesWritten)
        {
            return TryFormatDateTime(value.DateTime, value.Offset, destination, out bytesWritten);
        }

        public static bool TryFormatToISO(DateTime value, Span<byte> destination, out int bytesWritten)
        {
            return TryFormatDateTime(value, JsonConstants.NullUtcOffset, destination, out bytesWritten);
        }

        //
        // ISO 8601 format
        // If the milliseconds part of the date is zero, we omit the fraction part of the output string,
        // else we write the fraction up to 7 decimal places with no trailing zeros. i.e. the format is
        // YYYY-MM-DDThh:mm:ss[.s]TZD where TZD = Z or +-hh:mm.
        // e.g.
        //   ---------------------------------
        //   2017-06-12T05:30:45.768-07:00
        //   2017-06-12T05:30:45.00768Z           (Z is short for "+00:00" but also distinguishes DateTimeKind.Utc from DateTimeKind.Local)
        //   2017-06-12T05:30:45                  (interpreted as local time wrt to current time zone)
        private static bool TryFormatDateTime(DateTime value, TimeSpan offset, Span<byte> destination, out int bytesWritten)
        {
            // Must write YYYY-MM-DDThh:mm:ss
            const int MinimumBytesNeeded = 19;

            int bytesRequired = MinimumBytesNeeded;
            DateTimeKind kind = DateTimeKind.Local;

            if (offset == JsonConstants.NullUtcOffset)
            {
                kind = value.Kind;
                if (kind == DateTimeKind.Local)
                {
                    offset = TimeZoneInfo.Local.GetUtcOffset(value);
                    bytesRequired += 6;
                }
                else if (kind == DateTimeKind.Utc)
                {
                    bytesRequired += 1;
                }
            }
            else
            {
                bytesRequired += 6;
            }

            if (destination.Length < bytesRequired)
            {
                bytesWritten = 0;
                return false;
            }

            bytesWritten = bytesRequired;

            uint fraction = (uint)(value.Ticks % TimeSpan.TicksPerSecond);
            int numFractionDigits = 0;

            if (fraction > 0)
            {
                // Remove trailing zeros
                while (fraction % 10 == 0)
                {
                    fraction /= 10;
                }

                numFractionDigits = (int)Math.Floor(Math.Log10(fraction) + 1);
                bytesWritten += numFractionDigits + 1 /* Account for fraction period */;
            }

            // Hoist most of the bounds checks on buffer.
            { var unused = destination[MinimumBytesNeeded - 1]; }

            WriteFourDecimalDigits((uint)value.Year, destination, 0);
            destination[4] = JsonConstants.Hyphen;

            WriteTwoDecimalDigits((uint)value.Month, destination, 5);
            destination[7] = JsonConstants.Hyphen;

            WriteTwoDecimalDigits((uint)value.Day, destination, 8);
            destination[10] = JsonConstants.TimePrefix;

            WriteTwoDecimalDigits((uint)value.Hour, destination, 11);
            destination[13] = JsonConstants.Colon;

            WriteTwoDecimalDigits((uint)value.Minute, destination, 14);
            destination[16] = JsonConstants.Colon;

            WriteTwoDecimalDigits((uint)value.Second, destination, 17);

            int offsetIndex = 19;
            if (fraction > 0)
            {
                destination[19] = JsonConstants.Period;
                WriteDigits(fraction, destination.Slice(20, numFractionDigits));

                offsetIndex += numFractionDigits + 1 /* Account for fraction period */;
            }

            if (kind == DateTimeKind.Local)
            {
                byte sign;

                if (offset < default(TimeSpan) /* a "const" version of TimeSpan.Zero */)
                {
                    sign = JsonConstants.Hyphen;
                    offset = TimeSpan.FromTicks(-offset.Ticks);
                }
                else
                {
                    sign = JsonConstants.Plus;
                }

                // Writing the value backward allows the JIT to optimize by
                // performing a single bounds check against buffer.
                WriteTwoDecimalDigits((uint)offset.Minutes, destination, offsetIndex + 4);
                destination[offsetIndex + 3] = JsonConstants.Colon;
                WriteTwoDecimalDigits((uint)offset.Hours, destination, offsetIndex + 1);
                destination[offsetIndex] = sign;

            }
            else if (kind == DateTimeKind.Utc)
            {
                destination[offsetIndex] = JsonConstants.UtcOffsetToken;
            }

            return true;
        }

        // The following methods are borrowed verbatim from src/Common/src/CoreLib/System/Buffers/Text/Utf8Formatter/FormattingHelpers.cs

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void WriteDigits(uint value, Span<byte> buffer)
        {
            // We can mutate the 'value' parameter since it's a copy-by-value local.
            // It'll be used to represent the value left over after each division by 10.

            for (int i = buffer.Length - 1; i >= 1; i--)
            {
                uint temp = '0' + value;
                value /= 10;
                buffer[i] = (byte)(temp - (value * 10));
            }

            Debug.Assert(value < 10);
            buffer[0] = (byte)('0' + value);
        }

        /// <summary>
        /// Writes a value [ 0000 .. 9999 ] to the buffer starting at the specified offset.
        /// This method performs best when the starting index is a constant literal.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void WriteFourDecimalDigits(uint value, Span<byte> buffer, int startingIndex = 0)
        {
            Debug.Assert(0 <= value && value <= 9999);

            uint temp = '0' + value;
            value /= 10;
            buffer[startingIndex + 3] = (byte)(temp - (value * 10));

            temp = '0' + value;
            value /= 10;
            buffer[startingIndex + 2] = (byte)(temp - (value * 10));

            temp = '0' + value;
            value /= 10;
            buffer[startingIndex + 1] = (byte)(temp - (value * 10));

            buffer[startingIndex] = (byte)('0' + value);
        }

        /// <summary>
        /// Writes a value [ 00 .. 99 ] to the buffer starting at the specified offset.
        /// This method performs best when the starting index is a constant literal.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void WriteTwoDecimalDigits(uint value, Span<byte> buffer, int startingIndex = 0)
        {
            Debug.Assert(0 <= value && value <= 99);

            uint temp = '0' + value;
            value /= 10;
            buffer[startingIndex + 1] = (byte)(temp - (value * 10));

            buffer[startingIndex] = (byte)('0' + value);
        }
    }
}
