// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;

namespace System.Net
{
    internal static class HttpDateParse
    {
        private static readonly DateTimeFormatInfo s_format = DateTimeFormatInfo.InvariantInfo;
        private static readonly DateTimeStyles s_style = DateTimeStyles.AssumeUniversal | DateTimeStyles.AllowWhiteSpaces;

        private static readonly string[] s_dateFormats =
        {
            // RFC1123
            "R",
            // RFC1123 - UTC
            "ddd, dd MMM yyyy HH:mm:ss \"UTC\"",
            // RFC1123 - Offset
            "ddd, dd MMM yyyy HH:mm:sszzz",
            // RFC850
            "dddd, dd-MMM-yy HH:mm:ss \"GMT\"",
            // RFC850 - UTC
            "dddd, dd-MMM-yy HH:mm:ss \"UTC\"",
            // RFC850 - Offset
            "dddd, dd-MMM-yy HH:mm:sszzz",
            // ANSI
            "ddd MMM d HH:mm:ss yyyy"
        };

        /// <summary>
        /// Parses through an ANSI, RFC850, or RFC1123 date format and converts it to a <see cref="DateTime"/>.
        /// </summary>
        public static bool ParseHttpDate(string dateString, out DateTime result)
        {
            result = DateTime.MinValue;
            // Convert to upper so that UTC/GMT case is ignored (if present).
            // Parsing already ignores casing of day-of-week/month, but the zone is checked as an exact string.
            dateString = dateString.ToUpperInvariant();

            if (DateTimeOffset.TryParseExact(dateString, s_dateFormats, s_format, s_style, out var offset))
            {
                result = offset.LocalDateTime;
                return true;
            }

            return false;
        }
    }
}
