// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;

namespace System.Buffers.Text
{
    public static partial class Utf8Parser
    {
        //
        // Parse an RFC1123 date string.
        //
        // 01234567890123456789012345678
        // -----------------------------
        // Tue, 03 Jan 2017 08:08:05 GMT
        //
        private static bool TryParseDateTimeOffsetR(ReadOnlySpan<byte> text, uint caseFlipXorMask, out DateTimeOffset dateTimeOffset, out int bytesConsumed)
        {
            if (text.Length < Utf8Constants.Rfc1123DateStringLength)
            {
                bytesConsumed = 0;
                dateTimeOffset = default;
                return false;
            }

            DayOfWeek dayOfWeek;
            {
                uint dow0 = text[0] ^ caseFlipXorMask;
                uint dow1 = text[1];
                uint dow2 = text[2];
                uint comma = text[3];
                uint dowString = (dow0 << 24) | (dow1 << 16) | (dow2 << 8) | comma;
                switch (dowString)
                {
                    case 0x53756E2c /* 'Sun,' */: dayOfWeek = DayOfWeek.Sunday; break;
                    case 0x4d6f6e2c /* 'Mon,' */: dayOfWeek = DayOfWeek.Monday; break;
                    case 0x5475652c /* 'Tue,' */: dayOfWeek = DayOfWeek.Tuesday; break;
                    case 0x5765642c /* 'Wed,' */: dayOfWeek = DayOfWeek.Wednesday; break;
                    case 0x5468752c /* 'Thu,' */: dayOfWeek = DayOfWeek.Thursday; break;
                    case 0x4672692c /* 'Fri,' */: dayOfWeek = DayOfWeek.Friday; break;
                    case 0x5361742c /* 'Sat,' */: dayOfWeek = DayOfWeek.Saturday; break;
                    default:
                        bytesConsumed = 0;
                        dateTimeOffset = default;
                        return false;
                }
            }

            if (text[4] != Utf8Constants.Space)
            {
                bytesConsumed = 0;
                dateTimeOffset = default;
                return false;
            }

            int day;
            {
                uint digit1 = text[5] - 48u; // '0'
                uint digit2 = text[6] - 48u; // '0'

                if (digit1 > 9 || digit2 > 9)
                {
                    bytesConsumed = 0;
                    dateTimeOffset = default;
                    return false;
                }

                day = (int)(digit1 * 10 + digit2);
            }

            if (text[7] != Utf8Constants.Space)
            {
                bytesConsumed = 0;
                dateTimeOffset = default;
                return false;
            }

            int month;
            {
                uint mon0 = text[8] ^ caseFlipXorMask;
                uint mon1 = text[9];
                uint mon2 = text[10];
                uint space = text[11];
                uint monthString = (mon0 << 24) | (mon1 << 16) | (mon2 << 8) | space;
                switch (monthString)
                {
                    case 0x4a616e20 /* 'Jan ' */ : month = 1; break;
                    case 0x46656220 /* 'Feb ' */ : month = 2; break;
                    case 0x4d617220 /* 'Mar ' */ : month = 3; break;
                    case 0x41707220 /* 'Apr ' */ : month = 4; break;
                    case 0x4d617920 /* 'May ' */ : month = 5; break;
                    case 0x4a756e20 /* 'Jun ' */ : month = 6; break;
                    case 0x4a756c20 /* 'Jul ' */ : month = 7; break;
                    case 0x41756720 /* 'Aug ' */ : month = 8; break;
                    case 0x53657020 /* 'Sep ' */ : month = 9; break;
                    case 0x4f637420 /* 'Oct ' */ : month = 10; break;
                    case 0x4e6f7620 /* 'Nov ' */ : month = 11; break;
                    case 0x44656320 /* 'Dec ' */ : month = 12; break;
                    default:
                        bytesConsumed = 0;
                        dateTimeOffset = default;
                        return false;
                }
            }

            int year;
            {
                uint digit1 = text[12] - 48u; // '0'
                uint digit2 = text[13] - 48u; // '0'
                uint digit3 = text[14] - 48u; // '0'
                uint digit4 = text[15] - 48u; // '0'

                if (digit1 > 9 || digit2 > 9 || digit3 > 9 || digit4 > 9)
                {
                    bytesConsumed = 0;
                    dateTimeOffset = default;
                    return false;
                }

                year = (int)(digit1 * 1000 + digit2 * 100 + digit3 * 10 + digit4);
            }

            if (text[16] != Utf8Constants.Space)
            {
                bytesConsumed = 0;
                dateTimeOffset = default;
                return false;
            }

            int hour;
            {
                uint digit1 = text[17] - 48u; // '0'
                uint digit2 = text[18] - 48u; // '0'

                if (digit1 > 9 || digit2 > 9)
                {
                    bytesConsumed = 0;
                    dateTimeOffset = default;
                    return false;
                }

                hour = (int)(digit1 * 10 + digit2);
            }

            if (text[19] != Utf8Constants.Colon)
            {
                bytesConsumed = 0;
                dateTimeOffset = default;
                return false;
            }

            int minute;
            {
                uint digit1 = text[20] - 48u; // '0'
                uint digit2 = text[21] - 48u; // '0'

                if (digit1 > 9 || digit2 > 9)
                {
                    bytesConsumed = 0;
                    dateTimeOffset = default;
                    return false;
                }

                minute = (int)(digit1 * 10 + digit2);
            }

            if (text[22] != Utf8Constants.Colon)
            {
                bytesConsumed = 0;
                dateTimeOffset = default;
                return false;
            }

            int second;
            {
                uint digit1 = text[23] - 48u; // '0'
                uint digit2 = text[24] - 48u; // '0'

                if (digit1 > 9 || digit2 > 9)
                {
                    bytesConsumed = 0;
                    dateTimeOffset = default;
                    return false;
                }

                second = (int)(digit1 * 10 + digit2);
            }

            {
                uint space = text[25];
                uint g = text[26] ^ caseFlipXorMask;
                uint m = text[27] ^ caseFlipXorMask;
                uint t = text[28] ^ caseFlipXorMask;
                uint gmtString = (space << 24) | (g << 16) | (m << 8) | t;
                if (gmtString != 0x20474d54 /* ' GMT' */)
                {
                    bytesConsumed = 0;
                    dateTimeOffset = default;
                    return false;
                }
            }

            if (!TryCreateDateTimeOffset(year: year, month: month, day: day, hour: hour, minute: minute, second: second, fraction: 0, offsetNegative: false, offsetHours: 0, offsetMinutes: 0, out dateTimeOffset))
            {
                bytesConsumed = 0;
                dateTimeOffset = default;
                return false;
            }

            if (dayOfWeek != dateTimeOffset.DayOfWeek)
            {
                // If we got here, the day of week did not match the actual date.
                bytesConsumed = 0;
                dateTimeOffset = default;
                return false;
            }

            bytesConsumed = Utf8Constants.Rfc1123DateStringLength;
            return true;
        }
    }
}
