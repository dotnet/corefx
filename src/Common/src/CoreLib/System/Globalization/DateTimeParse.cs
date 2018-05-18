// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace System
{
    internal static class DateTimeParse
    {
        internal const Int32 MaxDateTimeNumberDigits = 8;

        internal delegate bool MatchNumberDelegate(ref __DTString str, int digitLen, out int result);

        internal static MatchNumberDelegate m_hebrewNumberParser = new MatchNumberDelegate(DateTimeParse.MatchHebrewDigits);

        internal static DateTime ParseExact(ReadOnlySpan<char> s, ReadOnlySpan<char> format, DateTimeFormatInfo dtfi, DateTimeStyles style)
        {
            DateTimeResult result = new DateTimeResult();       // The buffer to store the parsing result.
            result.Init(s);
            if (TryParseExact(s, format, dtfi, style, ref result))
            {
                return result.parsedDate;
            }
            else
            {
                throw GetDateTimeParseException(ref result);
            }
        }

        internal static DateTime ParseExact(ReadOnlySpan<char> s, ReadOnlySpan<char> format, DateTimeFormatInfo dtfi, DateTimeStyles style, out TimeSpan offset)
        {
            DateTimeResult result = new DateTimeResult();       // The buffer to store the parsing result.
            offset = TimeSpan.Zero;
            result.Init(s);
            result.flags |= ParseFlags.CaptureOffset;
            if (TryParseExact(s, format, dtfi, style, ref result))
            {
                offset = result.timeZoneOffset;
                return result.parsedDate;
            }
            else
            {
                throw GetDateTimeParseException(ref result);
            }
        }

        internal static bool TryParseExact(ReadOnlySpan<char> s, ReadOnlySpan<char> format, DateTimeFormatInfo dtfi, DateTimeStyles style, out DateTime result)
        {
            result = DateTime.MinValue;
            DateTimeResult resultData = new DateTimeResult();       // The buffer to store the parsing result.
            resultData.Init(s);
            if (TryParseExact(s, format, dtfi, style, ref resultData))
            {
                result = resultData.parsedDate;
                return true;
            }
            return false;
        }

        internal static bool TryParseExact(ReadOnlySpan<char> s, ReadOnlySpan<char> format, DateTimeFormatInfo dtfi, DateTimeStyles style, out DateTime result, out TimeSpan offset)
        {
            result = DateTime.MinValue;
            offset = TimeSpan.Zero;
            DateTimeResult resultData = new DateTimeResult();       // The buffer to store the parsing result.
            resultData.Init(s);
            resultData.flags |= ParseFlags.CaptureOffset;
            if (TryParseExact(s, format, dtfi, style, ref resultData))
            {
                result = resultData.parsedDate;
                offset = resultData.timeZoneOffset;
                return true;
            }
            return false;
        }

        internal static bool TryParseExact(ReadOnlySpan<char> s, ReadOnlySpan<char> format, DateTimeFormatInfo dtfi, DateTimeStyles style, ref DateTimeResult result)
        {
            if (s.Length == 0)
            {
                result.SetFailure(ParseFailureKind.FormatWithParameter, nameof(SR.Format_BadDateTime));
                return false;
            }

            if (format.Length == 0)
            {
                result.SetBadFormatSpecifierFailure();
                return false;
            }

            Debug.Assert(dtfi != null, "dtfi == null");

            return DoStrictParse(s, format, style, dtfi, ref result);
        }

        internal static DateTime ParseExactMultiple(ReadOnlySpan<char> s, String[] formats,
                                                DateTimeFormatInfo dtfi, DateTimeStyles style)
        {
            DateTimeResult result = new DateTimeResult();       // The buffer to store the parsing result.
            result.Init(s);
            if (TryParseExactMultiple(s, formats, dtfi, style, ref result))
            {
                return result.parsedDate;
            }
            else
            {
                throw GetDateTimeParseException(ref result);
            }
        }


        internal static DateTime ParseExactMultiple(ReadOnlySpan<char> s, String[] formats,
                                                DateTimeFormatInfo dtfi, DateTimeStyles style, out TimeSpan offset)
        {
            DateTimeResult result = new DateTimeResult();       // The buffer to store the parsing result.
            offset = TimeSpan.Zero;
            result.Init(s);
            result.flags |= ParseFlags.CaptureOffset;
            if (TryParseExactMultiple(s, formats, dtfi, style, ref result))
            {
                offset = result.timeZoneOffset;
                return result.parsedDate;
            }
            else
            {
                throw GetDateTimeParseException(ref result);
            }
        }

        internal static bool TryParseExactMultiple(ReadOnlySpan<char> s, String[] formats,
                                                   DateTimeFormatInfo dtfi, DateTimeStyles style, out DateTime result, out TimeSpan offset)
        {
            result = DateTime.MinValue;
            offset = TimeSpan.Zero;
            DateTimeResult resultData = new DateTimeResult();       // The buffer to store the parsing result.
            resultData.Init(s);
            resultData.flags |= ParseFlags.CaptureOffset;
            if (TryParseExactMultiple(s, formats, dtfi, style, ref resultData))
            {
                result = resultData.parsedDate;
                offset = resultData.timeZoneOffset;
                return true;
            }
            return false;
        }


        internal static bool TryParseExactMultiple(ReadOnlySpan<char> s, String[] formats,
                                                   DateTimeFormatInfo dtfi, DateTimeStyles style, out DateTime result)
        {
            result = DateTime.MinValue;
            DateTimeResult resultData = new DateTimeResult();       // The buffer to store the parsing result.
            resultData.Init(s);
            if (TryParseExactMultiple(s, formats, dtfi, style, ref resultData))
            {
                result = resultData.parsedDate;
                return true;
            }
            return false;
        }

        internal static bool TryParseExactMultiple(ReadOnlySpan<char> s, String[] formats,
                                                DateTimeFormatInfo dtfi, DateTimeStyles style, ref DateTimeResult result)
        {
            if (formats == null)
            {
                result.SetFailure(ParseFailureKind.ArgumentNull, nameof(SR.ArgumentNull_String), null, nameof(formats));
                return false;
            }

            if (s.Length == 0)
            {
                result.SetFailure(ParseFailureKind.FormatWithParameter, nameof(SR.Format_BadDateTime));
                return false;
            }

            if (formats.Length == 0)
            {
                result.SetFailure(ParseFailureKind.Format, nameof(SR.Format_NoFormatSpecifier));
                return false;
            }

            Debug.Assert(dtfi != null, "dtfi == null");

            //
            // Do a loop through the provided formats and see if we can parse succesfully in
            // one of the formats.
            //
            for (int i = 0; i < formats.Length; i++)
            {
                if (formats[i] == null || formats[i].Length == 0)
                {
                    result.SetBadFormatSpecifierFailure();
                    return false;
                }
                // Create a new result each time to ensure the runs are independent. Carry through
                // flags from the caller and return the result.
                DateTimeResult innerResult = new DateTimeResult();       // The buffer to store the parsing result.
                innerResult.Init(s);
                innerResult.flags = result.flags;
                if (TryParseExact(s, formats[i], dtfi, style, ref innerResult))
                {
                    result.parsedDate = innerResult.parsedDate;
                    result.timeZoneOffset = innerResult.timeZoneOffset;
                    return (true);
                }
            }
            result.SetBadDateTimeFailure();
            return (false);
        }

        ////////////////////////////////////////////////////////////////////////////
        // Date Token Types
        //
        // Following is the set of tokens that can be generated from a date
        // string. Notice that the legal set of trailing separators have been
        // folded in with the date number, and month name tokens. This set
        // of tokens is chosen to reduce the number of date parse states.
        //
        ////////////////////////////////////////////////////////////////////////////

        internal enum DTT : int
        {
            End = 0,    // '\0'
            NumEnd = 1,    // Num[ ]*[\0]
            NumAmpm = 2,    // Num[ ]+AmPm
            NumSpace = 3,    // Num[ ]+^[Dsep|Tsep|'0\']
            NumDatesep = 4,    // Num[ ]*Dsep
            NumTimesep = 5,    // Num[ ]*Tsep
            MonthEnd = 6,    // Month[ ]*'\0'
            MonthSpace = 7,    // Month[ ]+^[Dsep|Tsep|'\0']
            MonthDatesep = 8,    // Month[ ]*Dsep
            NumDatesuff = 9,    // Month[ ]*DSuff
            NumTimesuff = 10,   // Month[ ]*TSuff
            DayOfWeek = 11,   // Day of week name
            YearSpace = 12,   // Year+^[Dsep|Tsep|'0\']
            YearDateSep = 13,  // Year+Dsep
            YearEnd = 14,  // Year+['\0']
            TimeZone = 15,  // timezone name
            Era = 16,  // era name
            NumUTCTimeMark = 17,      // Num + 'Z'
            // When you add a new token which will be in the
            // state table, add it after NumLocalTimeMark.
            Unk = 18,   // unknown
            NumLocalTimeMark = 19,    // Num + 'T'
            Max = 20,   // marker
        }

        internal enum TM
        {
            NotSet = -1,
            AM = 0,
            PM = 1,
        }


        ////////////////////////////////////////////////////////////////////////////
        //
        // DateTime parsing state enumeration (DS.*)
        //
        ////////////////////////////////////////////////////////////////////////////

        internal enum DS
        {
            BEGIN = 0,
            N = 1,        // have one number
            NN = 2,        // have two numbers

            // The following are known to be part of a date

            D_Nd = 3,        // date string: have number followed by date separator
            D_NN = 4,        // date string: have two numbers
            D_NNd = 5,        // date string: have two numbers followed by date separator

            D_M = 6,        // date string: have a month
            D_MN = 7,        // date string: have a month and a number
            D_NM = 8,        // date string: have a number and a month
            D_MNd = 9,        // date string: have a month and number followed by date separator
            D_NDS = 10,       // date string: have one number followed a date suffix.

            D_Y = 11,        // date string: have a year.
            D_YN = 12,        // date string: have a year and a number
            D_YNd = 13,        // date string: have a year and a number and a date separator
            D_YM = 14,        // date string: have a year and a month
            D_YMd = 15,        // date string: have a year and a month and a date separator
            D_S = 16,       // have numbers followed by a date suffix.
            T_S = 17,       // have numbers followed by a time suffix.

            // The following are known to be part of a time

            T_Nt = 18,          // have num followed by time separator
            T_NNt = 19,       // have two numbers followed by time separator


            ERROR = 20,

            // The following are terminal states. These all have an action
            // associated with them; and transition back to BEGIN.

            DX_NN = 21,       // day from two numbers
            DX_NNN = 22,       // day from three numbers
            DX_MN = 23,       // day from month and one number
            DX_NM = 24,       // day from month and one number
            DX_MNN = 25,       // day from month and two numbers
            DX_DS = 26,       // a set of date suffixed numbers.
            DX_DSN = 27,       // day from date suffixes and one number.
            DX_NDS = 28,       // day from one number and date suffixes .
            DX_NNDS = 29,       // day from one number and date suffixes .

            DX_YNN = 30,       // date string: have a year and two number
            DX_YMN = 31,       // date string: have a year, a month, and a number.
            DX_YN = 32,       // date string: have a year and one number
            DX_YM = 33,       // date string: have a year, a month.
            TX_N = 34,       // time from one number (must have ampm)
            TX_NN = 35,       // time from two numbers
            TX_NNN = 36,       // time from three numbers
            TX_TS = 37,       // a set of time suffixed numbers.
            DX_NNY = 38,
        }

        ////////////////////////////////////////////////////////////////////////////
        //
        // NOTE: The following state machine table is dependent on the order of the
        // DS and DTT enumerations.
        //
        // For each non terminal state, the following table defines the next state
        // for each given date token type.
        //
        ////////////////////////////////////////////////////////////////////////////

        //          End       NumEnd      NumAmPm     NumSpace    NumDaySep   NumTimesep  MonthEnd    MonthSpace  MonthDSep   NumDateSuff NumTimeSuff     DayOfWeek     YearSpace   YearDateSep YearEnd     TimeZone   Era         UTCTimeMark
        private static DS[][] dateParsingStates = {
// DS.BEGIN                                                                             // DS.BEGIN
new DS[] { DS.BEGIN, DS.ERROR,   DS.TX_N,    DS.N,       DS.D_Nd,    DS.T_Nt,    DS.ERROR,   DS.D_M,     DS.D_M,     DS.D_S,     DS.T_S,         DS.BEGIN,     DS.D_Y,     DS.D_Y,     DS.ERROR,   DS.BEGIN,  DS.BEGIN,    DS.ERROR},

// DS.N                                                                                 // DS.N
new DS[] { DS.ERROR, DS.DX_NN,   DS.ERROR,   DS.NN,      DS.D_NNd,   DS.ERROR,   DS.DX_NM,   DS.D_NM,    DS.D_MNd,   DS.D_NDS,   DS.ERROR,       DS.N,         DS.D_YN,    DS.D_YNd,   DS.DX_YN,   DS.N,      DS.N,        DS.ERROR},

// DS.NN                                                                                // DS.NN
new DS[] { DS.DX_NN, DS.DX_NNN,  DS.TX_N,    DS.DX_NNN,  DS.ERROR,   DS.T_Nt,    DS.DX_MNN,  DS.DX_MNN,  DS.ERROR,   DS.ERROR,   DS.T_S,         DS.NN,        DS.DX_NNY,  DS.ERROR,   DS.DX_NNY,  DS.NN,     DS.NN,       DS.ERROR},

// DS.D_Nd                                                                              // DS.D_Nd
new DS[] { DS.ERROR, DS.DX_NN,   DS.ERROR,   DS.D_NN,    DS.D_NNd,   DS.ERROR,   DS.DX_NM,   DS.D_MN,    DS.D_MNd,   DS.ERROR,   DS.ERROR,       DS.D_Nd,      DS.D_YN,    DS.D_YNd,   DS.DX_YN,   DS.ERROR,  DS.D_Nd,     DS.ERROR},

// DS.D_NN                                                                              // DS.D_NN
new DS[] { DS.DX_NN, DS.DX_NNN,  DS.TX_N,    DS.DX_NNN,  DS.ERROR,   DS.T_Nt,    DS.DX_MNN,  DS.DX_MNN,  DS.ERROR,   DS.DX_DS,   DS.T_S,         DS.D_NN,     DS.DX_NNY,   DS.ERROR,   DS.DX_NNY,  DS.ERROR,  DS.D_NN,     DS.ERROR},

// DS.D_NNd                                                                             // DS.D_NNd
new DS[] { DS.ERROR, DS.DX_NNN,  DS.DX_NNN,  DS.DX_NNN,  DS.ERROR,   DS.ERROR,   DS.DX_MNN,  DS.DX_MNN,  DS.ERROR,   DS.DX_DS,   DS.ERROR,       DS.D_NNd,     DS.DX_NNY,  DS.ERROR,   DS.DX_NNY,  DS.ERROR,  DS.D_NNd,    DS.ERROR},

// DS.D_M                                                                               // DS.D_M
new DS[] { DS.ERROR, DS.DX_MN,   DS.ERROR,   DS.D_MN,    DS.D_MNd,   DS.ERROR,   DS.ERROR,   DS.ERROR,   DS.ERROR,   DS.ERROR,   DS.ERROR,       DS.D_M,       DS.D_YM,    DS.D_YMd,   DS.DX_YM,   DS.ERROR,  DS.D_M,      DS.ERROR},

// DS.D_MN                                                                              // DS.D_MN
new DS[] { DS.DX_MN, DS.DX_MNN,  DS.DX_MNN,  DS.DX_MNN,  DS.ERROR,   DS.T_Nt,    DS.ERROR,   DS.ERROR,   DS.ERROR,   DS.DX_DS,   DS.T_S,         DS.D_MN,      DS.DX_YMN,  DS.ERROR,   DS.DX_YMN,  DS.ERROR,  DS.D_MN,     DS.ERROR},

// DS.D_NM                                                                              // DS.D_NM
new DS[] { DS.DX_NM, DS.DX_MNN,  DS.DX_MNN,  DS.DX_MNN,  DS.ERROR,   DS.T_Nt,    DS.ERROR,   DS.ERROR,   DS.ERROR,   DS.DX_DS,   DS.T_S,         DS.D_NM,      DS.DX_YMN,  DS.ERROR,   DS.DX_YMN,  DS.ERROR,   DS.D_NM,    DS.ERROR},

// DS.D_MNd                                                                             // DS.D_MNd
new DS[] { DS.ERROR, DS.DX_MNN,  DS.ERROR,   DS.DX_MNN,  DS.ERROR,   DS.ERROR,   DS.ERROR,   DS.ERROR,   DS.ERROR,   DS.ERROR,   DS.ERROR,       DS.D_MNd,     DS.DX_YMN,  DS.ERROR,   DS.DX_YMN,  DS.ERROR,   DS.D_MNd,   DS.ERROR},

// DS.D_NDS,                                                                            // DS.D_NDS,
new DS[] { DS.DX_NDS,DS.DX_NNDS, DS.DX_NNDS, DS.DX_NNDS, DS.ERROR,   DS.T_Nt,    DS.ERROR,   DS.ERROR,   DS.ERROR,   DS.D_NDS,   DS.T_S,         DS.D_NDS,     DS.ERROR,   DS.ERROR,   DS.ERROR,   DS.ERROR,   DS.D_NDS,   DS.ERROR},

// DS.D_Y                                                                               // DS.D_Y
new DS[] { DS.ERROR, DS.DX_YN,   DS.ERROR,   DS.D_YN,    DS.D_YNd,   DS.ERROR,   DS.DX_YM,   DS.D_YM,    DS.D_YMd,   DS.D_YM,    DS.ERROR,       DS.D_Y,       DS.ERROR,   DS.ERROR,   DS.ERROR,   DS.ERROR,   DS.D_Y,     DS.ERROR},

// DS.D_YN                                                                              // DS.D_YN
new DS[] { DS.DX_YN, DS.DX_YNN,  DS.DX_YNN,  DS.DX_YNN,  DS.ERROR,   DS.ERROR,   DS.DX_YMN,  DS.DX_YMN,  DS.ERROR,   DS.ERROR,   DS.ERROR,       DS.D_YN,      DS.ERROR,   DS.ERROR,   DS.ERROR,   DS.ERROR,   DS.D_YN,    DS.ERROR},

// DS.D_YNd                                                                             // DS.D_YNd
new DS[] { DS.ERROR, DS.DX_YNN,  DS.DX_YNN,  DS.DX_YNN,  DS.ERROR,   DS.ERROR,   DS.DX_YMN,  DS.DX_YMN,  DS.ERROR,   DS.ERROR,   DS.ERROR,       DS.D_YN,      DS.ERROR,   DS.ERROR,   DS.ERROR,   DS.ERROR,   DS.D_YN,    DS.ERROR},

// DS.D_YM                                                                              // DS.D_YM
new DS[] { DS.DX_YM, DS.DX_YMN,  DS.DX_YMN,  DS.DX_YMN,  DS.ERROR,   DS.ERROR,   DS.ERROR,   DS.ERROR,   DS.ERROR,   DS.ERROR,   DS.ERROR,       DS.D_YM,      DS.ERROR,   DS.ERROR,   DS.ERROR,   DS.ERROR,   DS.D_YM,    DS.ERROR},

// DS.D_YMd                                                                             // DS.D_YMd
new DS[] { DS.ERROR, DS.DX_YMN,  DS.DX_YMN,  DS.DX_YMN,  DS.ERROR,   DS.ERROR,   DS.ERROR,   DS.ERROR,   DS.ERROR,   DS.ERROR,   DS.ERROR,       DS.D_YM,      DS.ERROR,   DS.ERROR,   DS.ERROR,   DS.ERROR,   DS.D_YM,    DS.ERROR},

// DS.D_S                                                                               // DS.D_S
new DS[] { DS.DX_DS, DS.DX_DSN,  DS.TX_N,    DS.T_Nt,    DS.ERROR,   DS.T_Nt,    DS.ERROR,   DS.ERROR,   DS.ERROR,   DS.D_S,     DS.T_S,         DS.D_S,       DS.ERROR,   DS.ERROR,   DS.ERROR,   DS.ERROR,   DS.D_S,     DS.ERROR},

// DS.T_S                                                                               // DS.T_S
new DS[] { DS.TX_TS, DS.TX_TS,   DS.TX_TS,   DS.T_Nt,    DS.D_Nd,    DS.ERROR,   DS.ERROR,   DS.ERROR,   DS.ERROR,   DS.D_S,     DS.T_S,         DS.T_S,       DS.ERROR,   DS.ERROR,   DS.ERROR,   DS.T_S,     DS.T_S,     DS.ERROR},

// DS.T_Nt                                                                              // DS.T_Nt
new DS[] { DS.ERROR, DS.TX_NN,   DS.TX_NN,   DS.TX_NN,   DS.ERROR,   DS.T_NNt,   DS.DX_NM,   DS.D_NM,    DS.ERROR,   DS.ERROR,   DS.T_S,         DS.ERROR,     DS.ERROR,   DS.ERROR,   DS.ERROR,   DS.T_Nt,    DS.T_Nt,    DS.TX_NN},

// DS.T_NNt                                                                             // DS.T_NNt
new DS[] { DS.ERROR, DS.TX_NNN,  DS.TX_NNN,  DS.TX_NNN,  DS.ERROR,   DS.ERROR,   DS.ERROR,   DS.ERROR,   DS.ERROR,   DS.ERROR,   DS.T_S,         DS.T_NNt,     DS.ERROR,   DS.ERROR,   DS.ERROR,   DS.T_NNt,   DS.T_NNt,   DS.TX_NNN},
};
        //          End       NumEnd      NumAmPm     NumSpace    NumDaySep   NumTimesep  MonthEnd    MonthSpace  MonthDSep   NumDateSuff NumTimeSuff     DayOfWeek     YearSpace   YearDateSep YearEnd     TimeZone    Era        UTCMark

        internal const String GMTName = "GMT";
        internal const String ZuluName = "Z";

        //
        // Search from the index of str at str.Index to see if the target string exists in the str.
        //
        private static bool MatchWord(ref __DTString str, String target)
        {
            if (target.Length > (str.Value.Length - str.Index))
            {
                return false;
            }

            if (str.CompareInfo.Compare(str.Value.Slice(str.Index, target.Length), target, CompareOptions.IgnoreCase) != 0)
            {
                return (false);
            }

            int nextCharIndex = str.Index + target.Length;

            if (nextCharIndex < str.Value.Length)
            {
                char nextCh = str.Value[nextCharIndex];
                if (Char.IsLetter(nextCh))
                {
                    return (false);
                }
            }
            str.Index = nextCharIndex;
            if (str.Index < str.Length)
            {
                str.m_current = str.Value[str.Index];
            }

            return (true);
        }


        //
        // Check the word at the current index to see if it matches GMT name or Zulu name.
        //
        private static bool GetTimeZoneName(ref __DTString str)
        {
            if (MatchWord(ref str, GMTName))
            {
                return (true);
            }

            if (MatchWord(ref str, ZuluName))
            {
                return (true);
            }

            return (false);
        }

        internal static bool IsDigit(char ch) => (uint)(ch - '0') <= 9;

        /*=================================ParseFraction==========================
        **Action: Starting at the str.Index, which should be a decimal symbol.
        ** if the current character is a digit, parse the remaining
        **      numbers as fraction.  For example, if the sub-string starting at str.Index is "123", then
        **      the method will return 0.123
        **Returns:      The fraction number.
        **Arguments:
        **      str the parsing string
        **Exceptions:
        ============================================================================*/

        private static bool ParseFraction(ref __DTString str, out double result)
        {
            result = 0;
            double decimalBase = 0.1;
            int digits = 0;
            char ch;
            while (str.GetNext()
                   && IsDigit(ch = str.m_current))
            {
                result += (ch - '0') * decimalBase;
                decimalBase *= 0.1;
                digits++;
            }
            return (digits > 0);
        }

        /*=================================ParseTimeZone==========================
        **Action: Parse the timezone offset in the following format:
        **          "+8", "+08", "+0800", "+0800"
        **        This method is used by DateTime.Parse().
        **Returns:      The TimeZone offset.
        **Arguments:
        **      str the parsing string
        **Exceptions:
        **      FormatException if invalid timezone format is found.
        ============================================================================*/

        private static bool ParseTimeZone(ref __DTString str, ref TimeSpan result)
        {
            // The hour/minute offset for timezone.
            int hourOffset = 0;
            int minuteOffset = 0;
            DTSubString sub;

            // Consume the +/- character that has already been read
            sub = str.GetSubString();
            if (sub.length != 1)
            {
                return false;
            }
            char offsetChar = sub[0];
            if (offsetChar != '+' && offsetChar != '-')
            {
                return false;
            }
            str.ConsumeSubString(sub);

            sub = str.GetSubString();
            if (sub.type != DTSubStringType.Number)
            {
                return false;
            }
            int value = sub.value;
            int length = sub.length;
            if (length == 1 || length == 2)
            {
                // Parsing "+8" or "+08"
                hourOffset = value;
                str.ConsumeSubString(sub);
                // See if we have minutes
                sub = str.GetSubString();
                if (sub.length == 1 && sub[0] == ':')
                {
                    // Parsing "+8:00" or "+08:00"
                    str.ConsumeSubString(sub);
                    sub = str.GetSubString();
                    if (sub.type != DTSubStringType.Number || sub.length < 1 || sub.length > 2)
                    {
                        return false;
                    }
                    minuteOffset = sub.value;
                    str.ConsumeSubString(sub);
                }
            }
            else if (length == 3 || length == 4)
            {
                // Parsing "+800" or "+0800"
                hourOffset = value / 100;
                minuteOffset = value % 100;
                str.ConsumeSubString(sub);
            }
            else
            {
                // Wrong number of digits
                return false;
            }
            Debug.Assert(hourOffset >= 0 && hourOffset <= 99, "hourOffset >= 0 && hourOffset <= 99");
            Debug.Assert(minuteOffset >= 0 && minuteOffset <= 99, "minuteOffset >= 0 && minuteOffset <= 99");
            if (minuteOffset < 0 || minuteOffset >= 60)
            {
                return false;
            }

            result = new TimeSpan(hourOffset, minuteOffset, 0);
            if (offsetChar == '-')
            {
                result = result.Negate();
            }
            return true;
        }

        // This is the helper function to handle timezone in string in the format like +/-0800
        private static bool HandleTimeZone(ref __DTString str, ref DateTimeResult result)
        {
            if ((str.Index < str.Length - 1))
            {
                char nextCh = str.Value[str.Index];
                // Skip whitespace, but don't update the index unless we find a time zone marker
                int whitespaceCount = 0;
                while (Char.IsWhiteSpace(nextCh) && str.Index + whitespaceCount < str.Length - 1)
                {
                    whitespaceCount++;
                    nextCh = str.Value[str.Index + whitespaceCount];
                }
                if (nextCh == '+' || nextCh == '-')
                {
                    str.Index += whitespaceCount;
                    if ((result.flags & ParseFlags.TimeZoneUsed) != 0)
                    {
                        // Should not have two timezone offsets.
                        result.SetBadDateTimeFailure();
                        return false;
                    }
                    result.flags |= ParseFlags.TimeZoneUsed;
                    if (!ParseTimeZone(ref str, ref result.timeZoneOffset))
                    {
                        result.SetBadDateTimeFailure();
                        return false;
                    }
                }
            }
            return true;
        }

        //
        // This is the lexer. Check the character at the current index, and put the found token in dtok and
        // some raw date/time information in raw.
        //
        private static Boolean Lex(DS dps, ref __DTString str, ref DateTimeToken dtok, ref DateTimeRawInfo raw, ref DateTimeResult result, ref DateTimeFormatInfo dtfi, DateTimeStyles styles)
        {
            TokenType tokenType;
            int tokenValue;
            int indexBeforeSeparator;
            char charBeforeSeparator;

            TokenType sep;
            dtok.dtt = DTT.Unk;     // Assume the token is unkown.

            str.GetRegularToken(out tokenType, out tokenValue, dtfi);

#if _LOGGING
            if (_tracingEnabled)
            {
                Trace($"Lex({Hex(str.Value)})\tpos:{str.Index}({Hex(str.m_current)}), {tokenType}, DS.{dps}");
            }
#endif // _LOGGING

            // Look at the regular token.
            switch (tokenType)
            {
                case TokenType.NumberToken:
                case TokenType.YearNumberToken:
                    if (raw.numCount == 3 || tokenValue == -1)
                    {
                        result.SetBadDateTimeFailure();
                        LexTraceExit("0010", dps);
                        return false;
                    }
                    //
                    // This is a digit.
                    //
                    // If the previous parsing state is DS.T_NNt (like 12:01), and we got another number,
                    // so we will have a terminal state DS.TX_NNN (like 12:01:02).
                    // If the previous parsing state is DS.T_Nt (like 12:), and we got another number,
                    // so we will have a terminal state DS.TX_NN (like 12:01).
                    //
                    // Look ahead to see if the following character is a decimal point or timezone offset.
                    // This enables us to parse time in the forms of:
                    //  "11:22:33.1234" or "11:22:33-08".
                    if (dps == DS.T_NNt)
                    {
                        if ((str.Index < str.Length - 1))
                        {
                            char nextCh = str.Value[str.Index];
                            if (nextCh == '.')
                            {
                                // While ParseFraction can fail, it just means that there were no digits after
                                // the dot. In this case ParseFraction just removes the dot. This is actually
                                // valid for cultures like Albanian, that join the time marker to the time with
                                // with a dot: e.g. "9:03.MD"
                                ParseFraction(ref str, out raw.fraction);
                            }
                        }
                    }
                    if (dps == DS.T_NNt || dps == DS.T_Nt)
                    {
                        if ((str.Index < str.Length - 1))
                        {
                            if (false == HandleTimeZone(ref str, ref result))
                            {
                                LexTraceExit("0020 (value like \"12:01\" or \"12:\" followed by a non-TZ number", dps);
                                return false;
                            }
                        }
                    }

                    dtok.num = tokenValue;
                    if (tokenType == TokenType.YearNumberToken)
                    {
                        if (raw.year == -1)
                        {
                            raw.year = tokenValue;
                            //
                            // If we have number which has 3 or more digits (like "001" or "0001"),
                            // we assume this number is a year. Save the currnet raw.numCount in
                            // raw.year.
                            //
                            switch (sep = str.GetSeparatorToken(dtfi, out indexBeforeSeparator, out charBeforeSeparator))
                            {
                                case TokenType.SEP_End:
                                    dtok.dtt = DTT.YearEnd;
                                    break;
                                case TokenType.SEP_Am:
                                case TokenType.SEP_Pm:
                                    if (raw.timeMark == TM.NotSet)
                                    {
                                        raw.timeMark = (sep == TokenType.SEP_Am ? TM.AM : TM.PM);
                                        dtok.dtt = DTT.YearSpace;
                                    }
                                    else
                                    {
                                        result.SetBadDateTimeFailure();
                                        LexTraceExit("0030 (TM.AM/TM.PM Happened more than 1x)", dps);
                                    }
                                    break;
                                case TokenType.SEP_Space:
                                    dtok.dtt = DTT.YearSpace;
                                    break;
                                case TokenType.SEP_Date:
                                    dtok.dtt = DTT.YearDateSep;
                                    break;
                                case TokenType.SEP_Time:
                                    if (!raw.hasSameDateAndTimeSeparators)
                                    {
                                        result.SetBadDateTimeFailure();
                                        LexTraceExit("0040 (Invalid separator after number)", dps);
                                        return false;
                                    }

                                    // we have the date and time separators are same and getting a year number, then change the token to YearDateSep as
                                    // we are sure we are not parsing time.
                                    dtok.dtt = DTT.YearDateSep;
                                    break;
                                case TokenType.SEP_DateOrOffset:
                                    // The separator is either a date separator or the start of a time zone offset. If the token will complete the date then
                                    // process just the number and roll back the index so that the outer loop can attempt to parse the time zone offset.
                                    if ((dateParsingStates[(int)dps][(int)DTT.YearDateSep] == DS.ERROR)
                                        && (dateParsingStates[(int)dps][(int)DTT.YearSpace] > DS.ERROR))
                                    {
                                        str.Index = indexBeforeSeparator;
                                        str.m_current = charBeforeSeparator;
                                        dtok.dtt = DTT.YearSpace;
                                    }
                                    else
                                    {
                                        dtok.dtt = DTT.YearDateSep;
                                    }
                                    break;
                                case TokenType.SEP_YearSuff:
                                case TokenType.SEP_MonthSuff:
                                case TokenType.SEP_DaySuff:
                                    dtok.dtt = DTT.NumDatesuff;
                                    dtok.suffix = sep;
                                    break;
                                case TokenType.SEP_HourSuff:
                                case TokenType.SEP_MinuteSuff:
                                case TokenType.SEP_SecondSuff:
                                    dtok.dtt = DTT.NumTimesuff;
                                    dtok.suffix = sep;
                                    break;
                                default:
                                    // Invalid separator after number number.
                                    result.SetBadDateTimeFailure();
                                    LexTraceExit("0040 (Invalid separator after number)", dps);
                                    return false;
                            }
                            //
                            // Found the token already. Return now.
                            //
                            LexTraceExit("0050 (success)", dps);
                            return true;
                        }
                        result.SetBadDateTimeFailure();
                        LexTraceExit("0060", dps);
                        return false;
                    }
                    switch (sep = str.GetSeparatorToken(dtfi, out indexBeforeSeparator, out charBeforeSeparator))
                    {
                        //
                        // Note here we check if the numCount is less than three.
                        // When we have more than three numbers, it will be caught as error in the state machine.
                        //
                        case TokenType.SEP_End:
                            dtok.dtt = DTT.NumEnd;
                            raw.AddNumber(dtok.num);
                            break;
                        case TokenType.SEP_Am:
                        case TokenType.SEP_Pm:
                            if (raw.timeMark == TM.NotSet)
                            {
                                raw.timeMark = (sep == TokenType.SEP_Am ? TM.AM : TM.PM);
                                dtok.dtt = DTT.NumAmpm;
                                // Fix AM/PM parsing case, e.g. "1/10 5 AM"
                                if (dps == DS.D_NN)
                                {
                                    if (!ProcessTerminalState(DS.DX_NN, ref str, ref result, ref styles, ref raw, dtfi))
                                    {
                                        return false;
                                    }
                                }

                                raw.AddNumber(dtok.num);
                            }
                            else
                            {
                                result.SetBadDateTimeFailure();
                                break;
                            }
                            if (dps == DS.T_NNt || dps == DS.T_Nt)
                            {
                                if (false == HandleTimeZone(ref str, ref result))
                                {
                                    LexTraceExit("0070 (HandleTimeZone returned false)", dps);
                                    return false;
                                }
                            }
                            break;
                        case TokenType.SEP_Space:
                            dtok.dtt = DTT.NumSpace;
                            raw.AddNumber(dtok.num);
                            break;
                        case TokenType.SEP_Date:
                            dtok.dtt = DTT.NumDatesep;
                            raw.AddNumber(dtok.num);
                            break;
                        case TokenType.SEP_DateOrOffset:
                            // The separator is either a date separator or the start of a time zone offset. If the token will complete the date then
                            // process just the number and roll back the index so that the outer loop can attempt to parse the time zone offset.
                            if ((dateParsingStates[(int)dps][(int)DTT.NumDatesep] == DS.ERROR)
                                && (dateParsingStates[(int)dps][(int)DTT.NumSpace] > DS.ERROR))
                            {
                                str.Index = indexBeforeSeparator;
                                str.m_current = charBeforeSeparator;
                                dtok.dtt = DTT.NumSpace;
                            }
                            else
                            {
                                dtok.dtt = DTT.NumDatesep;
                            }
                            raw.AddNumber(dtok.num);
                            break;
                        case TokenType.SEP_Time:
                            if (raw.hasSameDateAndTimeSeparators &&
                                (dps == DS.D_Y || dps == DS.D_YN || dps == DS.D_YNd || dps == DS.D_YM || dps == DS.D_YMd))
                            {
                                // we are parsing a date and we have the time separator same as date separator, so we mark the token as date separator
                                dtok.dtt = DTT.NumDatesep;
                                raw.AddNumber(dtok.num);
                                break;
                            }
                            dtok.dtt = DTT.NumTimesep;
                            raw.AddNumber(dtok.num);
                            break;
                        case TokenType.SEP_YearSuff:
                            try
                            {
                                dtok.num = dtfi.Calendar.ToFourDigitYear(tokenValue);
                            }
                            catch (ArgumentOutOfRangeException)
                            {
                                result.SetBadDateTimeFailure();
                                LexTraceExit("0075 (Calendar.ToFourDigitYear failed)", dps);
                                return false;
                            }
                            dtok.dtt = DTT.NumDatesuff;
                            dtok.suffix = sep;
                            break;
                        case TokenType.SEP_MonthSuff:
                        case TokenType.SEP_DaySuff:
                            dtok.dtt = DTT.NumDatesuff;
                            dtok.suffix = sep;
                            break;
                        case TokenType.SEP_HourSuff:
                        case TokenType.SEP_MinuteSuff:
                        case TokenType.SEP_SecondSuff:
                            dtok.dtt = DTT.NumTimesuff;
                            dtok.suffix = sep;
                            break;
                        case TokenType.SEP_LocalTimeMark:
                            dtok.dtt = DTT.NumLocalTimeMark;
                            raw.AddNumber(dtok.num);
                            break;
                        default:
                            // Invalid separator after number number.
                            result.SetBadDateTimeFailure();
                            LexTraceExit("0080", dps);
                            return false;
                    }
                    break;
                case TokenType.HebrewNumber:
                    if (tokenValue >= 100)
                    {
                        // This is a year number
                        if (raw.year == -1)
                        {
                            raw.year = tokenValue;
                            //
                            // If we have number which has 3 or more digits (like "001" or "0001"),
                            // we assume this number is a year. Save the currnet raw.numCount in
                            // raw.year.
                            //
                            switch (sep = str.GetSeparatorToken(dtfi, out indexBeforeSeparator, out charBeforeSeparator))
                            {
                                case TokenType.SEP_End:
                                    dtok.dtt = DTT.YearEnd;
                                    break;
                                case TokenType.SEP_Space:
                                    dtok.dtt = DTT.YearSpace;
                                    break;
                                case TokenType.SEP_DateOrOffset:
                                    // The separator is either a date separator or the start of a time zone offset. If the token will complete the date then
                                    // process just the number and roll back the index so that the outer loop can attempt to parse the time zone offset.
                                    if (dateParsingStates[(int)dps][(int)DTT.YearSpace] > DS.ERROR)
                                    {
                                        str.Index = indexBeforeSeparator;
                                        str.m_current = charBeforeSeparator;
                                        dtok.dtt = DTT.YearSpace;
                                        break;
                                    }
                                    goto default;
                                default:
                                    // Invalid separator after number number.
                                    result.SetBadDateTimeFailure();
                                    LexTraceExit("0090", dps);
                                    return false;
                            }
                        }
                        else
                        {
                            // Invalid separator after number number.
                            result.SetBadDateTimeFailure();
                            LexTraceExit("0100", dps);
                            return false;
                        }
                    }
                    else
                    {
                        // This is a day number
                        dtok.num = tokenValue;
                        raw.AddNumber(dtok.num);

                        switch (sep = str.GetSeparatorToken(dtfi, out indexBeforeSeparator, out charBeforeSeparator))
                        {
                            //
                            // Note here we check if the numCount is less than three.
                            // When we have more than three numbers, it will be caught as error in the state machine.
                            //
                            case TokenType.SEP_End:
                                dtok.dtt = DTT.NumEnd;
                                break;
                            case TokenType.SEP_Space:
                            case TokenType.SEP_Date:
                                dtok.dtt = DTT.NumDatesep;
                                break;
                            case TokenType.SEP_DateOrOffset:
                                // The separator is either a date separator or the start of a time zone offset. If the token will complete the date then
                                // process just the number and roll back the index so that the outer loop can attempt to parse the time zone offset.
                                if ((dateParsingStates[(int)dps][(int)DTT.NumDatesep] == DS.ERROR)
                                    && (dateParsingStates[(int)dps][(int)DTT.NumSpace] > DS.ERROR))
                                {
                                    str.Index = indexBeforeSeparator;
                                    str.m_current = charBeforeSeparator;
                                    dtok.dtt = DTT.NumSpace;
                                }
                                else
                                {
                                    dtok.dtt = DTT.NumDatesep;
                                }
                                break;
                            default:
                                // Invalid separator after number number.
                                result.SetBadDateTimeFailure();
                                LexTraceExit("0110", dps);
                                return false;
                        }
                    }
                    break;
                case TokenType.DayOfWeekToken:
                    if (raw.dayOfWeek == -1)
                    {
                        //
                        // This is a day of week name.
                        //
                        raw.dayOfWeek = tokenValue;
                        dtok.dtt = DTT.DayOfWeek;
                    }
                    else
                    {
                        result.SetBadDateTimeFailure();
                        LexTraceExit("0120 (DayOfWeek seen more than 1x)", dps);
                        return false;
                    }
                    break;
                case TokenType.MonthToken:
                    if (raw.month == -1)
                    {
                        //
                        // This is a month name
                        //
                        switch (sep = str.GetSeparatorToken(dtfi, out indexBeforeSeparator, out charBeforeSeparator))
                        {
                            case TokenType.SEP_End:
                                dtok.dtt = DTT.MonthEnd;
                                break;
                            case TokenType.SEP_Space:
                                dtok.dtt = DTT.MonthSpace;
                                break;
                            case TokenType.SEP_Date:
                                dtok.dtt = DTT.MonthDatesep;
                                break;
                            case TokenType.SEP_Time:
                                if (!raw.hasSameDateAndTimeSeparators)
                                {
                                    result.SetBadDateTimeFailure();
                                    LexTraceExit("0130 (Invalid separator after month name)", dps);
                                    return false;
                                }

                                // we have the date and time separators are same and getting a Month name, then change the token to MonthDatesep as
                                // we are sure we are not parsing time.
                                dtok.dtt = DTT.MonthDatesep;
                                break;
                            case TokenType.SEP_DateOrOffset:
                                // The separator is either a date separator or the start of a time zone offset. If the token will complete the date then
                                // process just the number and roll back the index so that the outer loop can attempt to parse the time zone offset.
                                if ((dateParsingStates[(int)dps][(int)DTT.MonthDatesep] == DS.ERROR)
                                    && (dateParsingStates[(int)dps][(int)DTT.MonthSpace] > DS.ERROR))
                                {
                                    str.Index = indexBeforeSeparator;
                                    str.m_current = charBeforeSeparator;
                                    dtok.dtt = DTT.MonthSpace;
                                }
                                else
                                {
                                    dtok.dtt = DTT.MonthDatesep;
                                }
                                break;
                            default:
                                //Invalid separator after month name
                                result.SetBadDateTimeFailure();
                                LexTraceExit("0130 (Invalid separator after month name)", dps);
                                return false;
                        }
                        raw.month = tokenValue;
                    }
                    else
                    {
                        result.SetBadDateTimeFailure();
                        LexTraceExit("0140 (MonthToken seen more than 1x)", dps);
                        return false;
                    }
                    break;
                case TokenType.EraToken:
                    if (result.era != -1)
                    {
                        result.era = tokenValue;
                        dtok.dtt = DTT.Era;
                    }
                    else
                    {
                        result.SetBadDateTimeFailure();
                        LexTraceExit("0150 (EraToken seen when result.era already set)", dps);
                        return false;
                    }
                    break;
                case TokenType.JapaneseEraToken:
                    // Special case for Japanese.  We allow Japanese era name to be used even if the calendar is not Japanese Calendar.
                    result.calendar = JapaneseCalendar.GetDefaultInstance();
                    dtfi = DateTimeFormatInfo.GetJapaneseCalendarDTFI();
                    if (result.era != -1)
                    {
                        result.era = tokenValue;
                        dtok.dtt = DTT.Era;
                    }
                    else
                    {
                        result.SetBadDateTimeFailure();
                        LexTraceExit("0160 (JapaneseEraToken seen when result.era already set)", dps);
                        return false;
                    }
                    break;
                case TokenType.TEraToken:
                    result.calendar = TaiwanCalendar.GetDefaultInstance();
                    dtfi = DateTimeFormatInfo.GetTaiwanCalendarDTFI();
                    if (result.era != -1)
                    {
                        result.era = tokenValue;
                        dtok.dtt = DTT.Era;
                    }
                    else
                    {
                        result.SetBadDateTimeFailure();
                        LexTraceExit("0170 (TEraToken seen when result.era already set)", dps);
                        return false;
                    }
                    break;
                case TokenType.TimeZoneToken:
                    //
                    // This is a timezone designator
                    //
                    // NOTENOTE : for now, we only support "GMT" and "Z" (for Zulu time).
                    //
                    if ((result.flags & ParseFlags.TimeZoneUsed) != 0)
                    {
                        // Should not have two timezone offsets.
                        result.SetBadDateTimeFailure();
                        LexTraceExit("0180 (seen GMT or Z more than 1x)", dps);
                        return false;
                    }
                    dtok.dtt = DTT.TimeZone;
                    result.flags |= ParseFlags.TimeZoneUsed;
                    result.timeZoneOffset = new TimeSpan(0);
                    result.flags |= ParseFlags.TimeZoneUtc;
                    break;
                case TokenType.EndOfString:
                    dtok.dtt = DTT.End;
                    break;
                case TokenType.DateWordToken:
                case TokenType.IgnorableSymbol:
                    // Date words and ignorable symbols can just be skipped over
                    break;
                case TokenType.Am:
                case TokenType.Pm:
                    if (raw.timeMark == TM.NotSet)
                    {
                        raw.timeMark = (TM)tokenValue;
                    }
                    else
                    {
                        result.SetBadDateTimeFailure();
                        LexTraceExit("0190 (AM/PM timeMark already set)", dps);
                        return false;
                    }
                    break;
                case TokenType.UnknownToken:
                    if (Char.IsLetter(str.m_current))
                    {
                        result.SetFailure(ParseFailureKind.FormatWithOriginalDateTimeAndParameter, nameof(SR.Format_UnknownDateTimeWord), str.Index);
                        LexTraceExit("0200", dps);
                        return (false);
                    }

                    if ((str.m_current == '-' || str.m_current == '+') && ((result.flags & ParseFlags.TimeZoneUsed) == 0))
                    {
                        Int32 originalIndex = str.Index;
                        if (ParseTimeZone(ref str, ref result.timeZoneOffset))
                        {
                            result.flags |= ParseFlags.TimeZoneUsed;
                            LexTraceExit("0220 (success)", dps);
                            return true;
                        }
                        else
                        {
                            // Time zone parse attempt failed. Fall through to punctuation handling.
                            str.Index = originalIndex;
                        }
                    }

                    // Visual Basic implements string to date conversions on top of DateTime.Parse:
                    //   CDate("#10/10/95#")
                    //
                    if (VerifyValidPunctuation(ref str))
                    {
                        LexTraceExit("0230 (success)", dps);
                        return true;
                    }

                    result.SetBadDateTimeFailure();
                    LexTraceExit("0240", dps);
                    return false;
            }

            LexTraceExit("0250 (success)", dps);
            return true;
        }

        private static Boolean VerifyValidPunctuation(ref __DTString str)
        {
            // Compatability Behavior. Allow trailing nulls and surrounding hashes
            Char ch = str.Value[str.Index];
            if (ch == '#')
            {
                bool foundStart = false;
                bool foundEnd = false;
                for (int i = 0; i < str.Length; i++)
                {
                    ch = str.Value[i];
                    if (ch == '#')
                    {
                        if (foundStart)
                        {
                            if (foundEnd)
                            {
                                // Having more than two hashes is invalid
                                return false;
                            }
                            else
                            {
                                foundEnd = true;
                            }
                        }
                        else
                        {
                            foundStart = true;
                        }
                    }
                    else if (ch == '\0')
                    {
                        // Allow nulls only at the end
                        if (!foundEnd)
                        {
                            return false;
                        }
                    }
                    else if ((!Char.IsWhiteSpace(ch)))
                    {
                        // Anything other than whitespace outside hashes is invalid
                        if (!foundStart || foundEnd)
                        {
                            return false;
                        }
                    }
                }
                if (!foundEnd)
                {
                    // The has was un-paired
                    return false;
                }
                // Valid Hash usage: eat the hash and continue.
                str.GetNext();
                return true;
            }
            else if (ch == '\0')
            {
                for (int i = str.Index; i < str.Length; i++)
                {
                    if (str.Value[i] != '\0')
                    {
                        // Nulls are only valid if they are the only trailing character
                        return false;
                    }
                }
                // Move to the end of the string
                str.Index = str.Length;
                return true;
            }
            return false;
        }

        private const int ORDER_YMD = 0;     // The order of date is Year/Month/Day.
        private const int ORDER_MDY = 1;     // The order of date is Month/Day/Year.
        private const int ORDER_DMY = 2;     // The order of date is Day/Month/Year.
        private const int ORDER_YDM = 3;     // The order of date is Year/Day/Month
        private const int ORDER_YM = 4;     // Year/Month order.
        private const int ORDER_MY = 5;     // Month/Year order.
        private const int ORDER_MD = 6;     // Month/Day order.
        private const int ORDER_DM = 7;     // Day/Month order.

        //
        // Decide the year/month/day order from the datePattern.
        //
        // Return 0 for YMD, 1 for MDY, 2 for DMY, otherwise -1.
        //
        private static Boolean GetYearMonthDayOrder(String datePattern, DateTimeFormatInfo dtfi, out int order)
        {
            int yearOrder = -1;
            int monthOrder = -1;
            int dayOrder = -1;
            int orderCount = 0;

            bool inQuote = false;

            for (int i = 0; i < datePattern.Length && orderCount < 3; i++)
            {
                char ch = datePattern[i];
                if (ch == '\\' || ch == '%')
                {
                    i++;
                    continue;  // Skip next character that is escaped by this backslash
                }

                if (ch == '\'' || ch == '"')
                {
                    inQuote = !inQuote;
                }

                if (!inQuote)
                {
                    if (ch == 'y')
                    {
                        yearOrder = orderCount++;

                        //
                        // Skip all year pattern charaters.
                        //
                        for (; i + 1 < datePattern.Length && datePattern[i + 1] == 'y'; i++)
                        {
                            // Do nothing here.
                        }
                    }
                    else if (ch == 'M')
                    {
                        monthOrder = orderCount++;
                        //
                        // Skip all month pattern characters.
                        //
                        for (; i + 1 < datePattern.Length && datePattern[i + 1] == 'M'; i++)
                        {
                            // Do nothing here.
                        }
                    }
                    else if (ch == 'd')
                    {
                        int patternCount = 1;
                        //
                        // Skip all day pattern characters.
                        //
                        for (; i + 1 < datePattern.Length && datePattern[i + 1] == 'd'; i++)
                        {
                            patternCount++;
                        }
                        //
                        // Make sure this is not "ddd" or "dddd", which means day of week.
                        //
                        if (patternCount <= 2)
                        {
                            dayOrder = orderCount++;
                        }
                    }
                }
            }

            if (yearOrder == 0 && monthOrder == 1 && dayOrder == 2)
            {
                order = ORDER_YMD;
                return true;
            }
            if (monthOrder == 0 && dayOrder == 1 && yearOrder == 2)
            {
                order = ORDER_MDY;
                return true;
            }
            if (dayOrder == 0 && monthOrder == 1 && yearOrder == 2)
            {
                order = ORDER_DMY;
                return true;
            }
            if (yearOrder == 0 && dayOrder == 1 && monthOrder == 2)
            {
                order = ORDER_YDM;
                return true;
            }
            order = -1;
            return false;
        }

        //
        // Decide the year/month order from the pattern.
        //
        // Return 0 for YM, 1 for MY, otherwise -1.
        //
        private static Boolean GetYearMonthOrder(String pattern, DateTimeFormatInfo dtfi, out int order)
        {
            int yearOrder = -1;
            int monthOrder = -1;
            int orderCount = 0;

            bool inQuote = false;
            for (int i = 0; i < pattern.Length && orderCount < 2; i++)
            {
                char ch = pattern[i];
                if (ch == '\\' || ch == '%')
                {
                    i++;
                    continue;  // Skip next character that is escaped by this backslash
                }

                if (ch == '\'' || ch == '"')
                {
                    inQuote = !inQuote;
                }

                if (!inQuote)
                {
                    if (ch == 'y')
                    {
                        yearOrder = orderCount++;

                        //
                        // Skip all year pattern charaters.
                        //
                        for (; i + 1 < pattern.Length && pattern[i + 1] == 'y'; i++)
                        {
                        }
                    }
                    else if (ch == 'M')
                    {
                        monthOrder = orderCount++;
                        //
                        // Skip all month pattern characters.
                        //
                        for (; i + 1 < pattern.Length && pattern[i + 1] == 'M'; i++)
                        {
                        }
                    }
                }
            }

            if (yearOrder == 0 && monthOrder == 1)
            {
                order = ORDER_YM;
                return true;
            }
            if (monthOrder == 0 && yearOrder == 1)
            {
                order = ORDER_MY;
                return true;
            }
            order = -1;
            return false;
        }

        //
        // Decide the month/day order from the pattern.
        //
        // Return 0 for MD, 1 for DM, otherwise -1.
        //
        private static Boolean GetMonthDayOrder(String pattern, DateTimeFormatInfo dtfi, out int order)
        {
            int monthOrder = -1;
            int dayOrder = -1;
            int orderCount = 0;

            bool inQuote = false;
            for (int i = 0; i < pattern.Length && orderCount < 2; i++)
            {
                char ch = pattern[i];
                if (ch == '\\' || ch == '%')
                {
                    i++;
                    continue;  // Skip next character that is escaped by this backslash
                }

                if (ch == '\'' || ch == '"')
                {
                    inQuote = !inQuote;
                }

                if (!inQuote)
                {
                    if (ch == 'd')
                    {
                        int patternCount = 1;
                        //
                        // Skip all day pattern charaters.
                        //
                        for (; i + 1 < pattern.Length && pattern[i + 1] == 'd'; i++)
                        {
                            patternCount++;
                        }

                        //
                        // Make sure this is not "ddd" or "dddd", which means day of week.
                        //
                        if (patternCount <= 2)
                        {
                            dayOrder = orderCount++;
                        }
                    }
                    else if (ch == 'M')
                    {
                        monthOrder = orderCount++;
                        //
                        // Skip all month pattern characters.
                        //
                        for (; i + 1 < pattern.Length && pattern[i + 1] == 'M'; i++)
                        {
                        }
                    }
                }
            }

            if (monthOrder == 0 && dayOrder == 1)
            {
                order = ORDER_MD;
                return true;
            }
            if (dayOrder == 0 && monthOrder == 1)
            {
                order = ORDER_DM;
                return true;
            }
            order = -1;
            return false;
        }

        //
        // Adjust the two-digit year if necessary.
        //
        private static bool TryAdjustYear(ref DateTimeResult result, int year, out int adjustedYear)
        {
            if (year < 100)
            {
                try
                {
                    // the Calendar classes need some real work.  Many of the calendars that throw
                    // don't implement a fast/non-allocating (and non-throwing) IsValid{Year|Day|Month} method.
                    // we are making a targeted try/catch fix in the in-place release but will revisit this code
                    // in the next side-by-side release.
                    year = result.calendar.ToFourDigitYear(year);
                }
                catch (ArgumentOutOfRangeException)
                {
                    adjustedYear = -1;
                    return false;
                }
            }
            adjustedYear = year;
            return true;
        }

        private static bool SetDateYMD(ref DateTimeResult result, int year, int month, int day)
        {
            // Note, longer term these checks should be done at the end of the parse. This current
            // way of checking creates order dependence with parsing the era name.
            if (result.calendar.IsValidDay(year, month, day, result.era))
            {
                result.SetDate(year, month, day);                           // YMD
                return (true);
            }
            return (false);
        }

        private static bool SetDateMDY(ref DateTimeResult result, int month, int day, int year)
        {
            return (SetDateYMD(ref result, year, month, day));
        }

        private static bool SetDateDMY(ref DateTimeResult result, int day, int month, int year)
        {
            return (SetDateYMD(ref result, year, month, day));
        }

        private static bool SetDateYDM(ref DateTimeResult result, int year, int day, int month)
        {
            return (SetDateYMD(ref result, year, month, day));
        }

        private static void GetDefaultYear(ref DateTimeResult result, ref DateTimeStyles styles)
        {
            result.Year = result.calendar.GetYear(GetDateTimeNow(ref result, ref styles));
            result.flags |= ParseFlags.YearDefault;
        }

        // Processing teriminal case: DS.DX_NN
        private static Boolean GetDayOfNN(ref DateTimeResult result, ref DateTimeStyles styles, ref DateTimeRawInfo raw, DateTimeFormatInfo dtfi)
        {
            if ((result.flags & ParseFlags.HaveDate) != 0)
            {
                // Multiple dates in the input string
                result.SetBadDateTimeFailure();
                return false;
            }

            int n1 = raw.GetNumber(0);
            int n2 = raw.GetNumber(1);

            GetDefaultYear(ref result, ref styles);

            int order;
            if (!GetMonthDayOrder(dtfi.MonthDayPattern, dtfi, out order))
            {
                result.SetFailure(ParseFailureKind.FormatWithParameter, nameof(SR.Format_BadDatePattern), dtfi.MonthDayPattern);
                return false;
            }

            if (order == ORDER_MD)
            {
                if (SetDateYMD(ref result, result.Year, n1, n2))                           // MD
                {
                    result.flags |= ParseFlags.HaveDate;
                    return true;
                }
            }
            else
            {
                // ORDER_DM
                if (SetDateYMD(ref result, result.Year, n2, n1))                           // DM
                {
                    result.flags |= ParseFlags.HaveDate;
                    return true;
                }
            }
            result.SetBadDateTimeFailure();
            return false;
        }

        // Processing teriminal case: DS.DX_NNN
        private static Boolean GetDayOfNNN(ref DateTimeResult result, ref DateTimeRawInfo raw, DateTimeFormatInfo dtfi)
        {
            if ((result.flags & ParseFlags.HaveDate) != 0)
            {
                // Multiple dates in the input string
                result.SetBadDateTimeFailure();
                return false;
            }

            int n1 = raw.GetNumber(0);
            int n2 = raw.GetNumber(1); ;
            int n3 = raw.GetNumber(2);

            int order;
            if (!GetYearMonthDayOrder(dtfi.ShortDatePattern, dtfi, out order))
            {
                result.SetFailure(ParseFailureKind.FormatWithParameter, nameof(SR.Format_BadDatePattern), dtfi.ShortDatePattern);
                return false;
            }
            int year;

            if (order == ORDER_YMD)
            {
                if (TryAdjustYear(ref result, n1, out year) && SetDateYMD(ref result, year, n2, n3))         // YMD
                {
                    result.flags |= ParseFlags.HaveDate;
                    return true;
                }
            }
            else if (order == ORDER_MDY)
            {
                if (TryAdjustYear(ref result, n3, out year) && SetDateMDY(ref result, n1, n2, year))         // MDY
                {
                    result.flags |= ParseFlags.HaveDate;
                    return true;
                }
            }
            else if (order == ORDER_DMY)
            {
                if (TryAdjustYear(ref result, n3, out year) && SetDateDMY(ref result, n1, n2, year))         // DMY
                {
                    result.flags |= ParseFlags.HaveDate;
                    return true;
                }
            }
            else if (order == ORDER_YDM)
            {
                if (TryAdjustYear(ref result, n1, out year) && SetDateYDM(ref result, year, n2, n3))         // YDM
                {
                    result.flags |= ParseFlags.HaveDate;
                    return true;
                }
            }
            result.SetBadDateTimeFailure();
            return false;
        }

        private static Boolean GetDayOfMN(ref DateTimeResult result, ref DateTimeStyles styles, ref DateTimeRawInfo raw, DateTimeFormatInfo dtfi)
        {
            if ((result.flags & ParseFlags.HaveDate) != 0)
            {
                // Multiple dates in the input string
                result.SetBadDateTimeFailure();
                return false;
            }

            // The interpretation is based on the MonthDayPattern and YearMonthPattern
            //
            //    MonthDayPattern   YearMonthPattern  Interpretation
            //    ---------------   ----------------  ---------------
            //    MMMM dd           MMMM yyyy         Day
            //    MMMM dd           yyyy MMMM         Day
            //    dd MMMM           MMMM yyyy         Year
            //    dd MMMM           yyyy MMMM         Day
            //
            // In the first and last cases, it could be either or neither, but a day is a better default interpretation
            // than a 2 digit year.

            int monthDayOrder;
            if (!GetMonthDayOrder(dtfi.MonthDayPattern, dtfi, out monthDayOrder))
            {
                result.SetFailure(ParseFailureKind.FormatWithParameter, nameof(SR.Format_BadDatePattern), dtfi.MonthDayPattern);
                return false;
            }
            if (monthDayOrder == ORDER_DM)
            {
                int yearMonthOrder;
                if (!GetYearMonthOrder(dtfi.YearMonthPattern, dtfi, out yearMonthOrder))
                {
                    result.SetFailure(ParseFailureKind.FormatWithParameter, nameof(SR.Format_BadDatePattern), dtfi.YearMonthPattern);
                    return false;
                }
                if (yearMonthOrder == ORDER_MY)
                {
                    int year;
                    if (!TryAdjustYear(ref result, raw.GetNumber(0), out year) || !SetDateYMD(ref result, year, raw.month, 1))
                    {
                        result.SetBadDateTimeFailure();
                        return false;
                    }
                    return true;
                }
            }

            GetDefaultYear(ref result, ref styles);
            if (!SetDateYMD(ref result, result.Year, raw.month, raw.GetNumber(0)))
            {
                result.SetBadDateTimeFailure();
                return false;
            }
            return true;
        }

        ////////////////////////////////////////////////////////////////////////
        //  Actions:
        //  Deal with the terminal state for Hebrew Month/Day pattern
        //
        ////////////////////////////////////////////////////////////////////////

        private static Boolean GetHebrewDayOfNM(ref DateTimeResult result, ref DateTimeRawInfo raw, DateTimeFormatInfo dtfi)
        {
            int monthDayOrder;
            if (!GetMonthDayOrder(dtfi.MonthDayPattern, dtfi, out monthDayOrder))
            {
                result.SetFailure(ParseFailureKind.FormatWithParameter, nameof(SR.Format_BadDatePattern), dtfi.MonthDayPattern);
                return false;
            }
            result.Month = raw.month;
            if (monthDayOrder == ORDER_DM || monthDayOrder == ORDER_MD)
            {
                if (result.calendar.IsValidDay(result.Year, result.Month, raw.GetNumber(0), result.era))
                {
                    result.Day = raw.GetNumber(0);
                    return true;
                }
            }
            result.SetBadDateTimeFailure();
            return false;
        }

        private static Boolean GetDayOfNM(ref DateTimeResult result, ref DateTimeStyles styles, ref DateTimeRawInfo raw, DateTimeFormatInfo dtfi)
        {
            if ((result.flags & ParseFlags.HaveDate) != 0)
            {
                // Multiple dates in the input string
                result.SetBadDateTimeFailure();
                return false;
            }

            // The interpretation is based on the MonthDayPattern and YearMonthPattern
            //
            //    MonthDayPattern   YearMonthPattern  Interpretation
            //    ---------------   ----------------  ---------------
            //    MMMM dd           MMMM yyyy         Day
            //    MMMM dd           yyyy MMMM         Year
            //    dd MMMM           MMMM yyyy         Day
            //    dd MMMM           yyyy MMMM         Day
            //
            // In the first and last cases, it could be either or neither, but a day is a better default interpretation
            // than a 2 digit year.

            int monthDayOrder;
            if (!GetMonthDayOrder(dtfi.MonthDayPattern, dtfi, out monthDayOrder))
            {
                result.SetFailure(ParseFailureKind.FormatWithParameter, nameof(SR.Format_BadDatePattern), dtfi.MonthDayPattern);
                return false;
            }
            if (monthDayOrder == ORDER_MD)
            {
                int yearMonthOrder;
                if (!GetYearMonthOrder(dtfi.YearMonthPattern, dtfi, out yearMonthOrder))
                {
                    result.SetFailure(ParseFailureKind.FormatWithParameter, nameof(SR.Format_BadDatePattern), dtfi.YearMonthPattern);
                    return false;
                }
                if (yearMonthOrder == ORDER_YM)
                {
                    int year;
                    if (!TryAdjustYear(ref result, raw.GetNumber(0), out year) || !SetDateYMD(ref result, year, raw.month, 1))
                    {
                        result.SetBadDateTimeFailure();
                        return false;
                    }
                    return true;
                }
            }

            GetDefaultYear(ref result, ref styles);
            if (!SetDateYMD(ref result, result.Year, raw.month, raw.GetNumber(0)))
            {
                result.SetBadDateTimeFailure();
                return false;
            }
            return true;
        }

        private static Boolean GetDayOfMNN(ref DateTimeResult result, ref DateTimeRawInfo raw, DateTimeFormatInfo dtfi)
        {
            if ((result.flags & ParseFlags.HaveDate) != 0)
            {
                // Multiple dates in the input string
                result.SetBadDateTimeFailure();
                return false;
            }

            int n1 = raw.GetNumber(0);
            int n2 = raw.GetNumber(1);

            int order;
            if (!GetYearMonthDayOrder(dtfi.ShortDatePattern, dtfi, out order))
            {
                result.SetFailure(ParseFailureKind.FormatWithParameter, nameof(SR.Format_BadDatePattern), dtfi.ShortDatePattern);
                return false;
            }
            int year;

            if (order == ORDER_MDY)
            {
                if (TryAdjustYear(ref result, n2, out year) && result.calendar.IsValidDay(year, raw.month, n1, result.era))
                {
                    result.SetDate(year, raw.month, n1);      // MDY
                    result.flags |= ParseFlags.HaveDate;
                    return true;
                }
                else if (TryAdjustYear(ref result, n1, out year) && result.calendar.IsValidDay(year, raw.month, n2, result.era))
                {
                    result.SetDate(year, raw.month, n2);      // YMD
                    result.flags |= ParseFlags.HaveDate;
                    return true;
                }
            }
            else if (order == ORDER_YMD)
            {
                if (TryAdjustYear(ref result, n1, out year) && result.calendar.IsValidDay(year, raw.month, n2, result.era))
                {
                    result.SetDate(year, raw.month, n2);      // YMD
                    result.flags |= ParseFlags.HaveDate;
                    return true;
                }
                else if (TryAdjustYear(ref result, n2, out year) && result.calendar.IsValidDay(year, raw.month, n1, result.era))
                {
                    result.SetDate(year, raw.month, n1);      // DMY
                    result.flags |= ParseFlags.HaveDate;
                    return true;
                }
            }
            else if (order == ORDER_DMY)
            {
                if (TryAdjustYear(ref result, n2, out year) && result.calendar.IsValidDay(year, raw.month, n1, result.era))
                {
                    result.SetDate(year, raw.month, n1);      // DMY
                    result.flags |= ParseFlags.HaveDate;
                    return true;
                }
                else if (TryAdjustYear(ref result, n1, out year) && result.calendar.IsValidDay(year, raw.month, n2, result.era))
                {
                    result.SetDate(year, raw.month, n2);      // YMD
                    result.flags |= ParseFlags.HaveDate;
                    return true;
                }
            }

            result.SetBadDateTimeFailure();
            return false;
        }

        private static Boolean GetDayOfYNN(ref DateTimeResult result, ref DateTimeRawInfo raw, DateTimeFormatInfo dtfi)
        {
            if ((result.flags & ParseFlags.HaveDate) != 0)
            {
                // Multiple dates in the input string
                result.SetBadDateTimeFailure();
                return false;
            }

            int n1 = raw.GetNumber(0);
            int n2 = raw.GetNumber(1);
            String pattern = dtfi.ShortDatePattern;

            // For compatibility, don't throw if we can't determine the order, but default to YMD instead
            int order;
            if (GetYearMonthDayOrder(pattern, dtfi, out order) && order == ORDER_YDM)
            {
                if (SetDateYMD(ref result, raw.year, n2, n1))
                {
                    result.flags |= ParseFlags.HaveDate;
                    return true; // Year + DM
                }
            }
            else
            {
                if (SetDateYMD(ref result, raw.year, n1, n2))
                {
                    result.flags |= ParseFlags.HaveDate;
                    return true; // Year + MD
                }
            }
            result.SetBadDateTimeFailure();
            return false;
        }

        private static Boolean GetDayOfNNY(ref DateTimeResult result, ref DateTimeRawInfo raw, DateTimeFormatInfo dtfi)
        {
            if ((result.flags & ParseFlags.HaveDate) != 0)
            {
                // Multiple dates in the input string
                result.SetBadDateTimeFailure();
                return false;
            }

            int n1 = raw.GetNumber(0);
            int n2 = raw.GetNumber(1);

            int order;
            if (!GetYearMonthDayOrder(dtfi.ShortDatePattern, dtfi, out order))
            {
                result.SetFailure(ParseFailureKind.FormatWithParameter, nameof(SR.Format_BadDatePattern), dtfi.ShortDatePattern);
                return false;
            }

            if (order == ORDER_MDY || order == ORDER_YMD)
            {
                if (SetDateYMD(ref result, raw.year, n1, n2))
                {
                    result.flags |= ParseFlags.HaveDate;
                    return true; // MD + Year
                }
            }
            else
            {
                if (SetDateYMD(ref result, raw.year, n2, n1))
                {
                    result.flags |= ParseFlags.HaveDate;
                    return true; // DM + Year
                }
            }
            result.SetBadDateTimeFailure();
            return false;
        }


        private static Boolean GetDayOfYMN(ref DateTimeResult result, ref DateTimeRawInfo raw)
        {
            if ((result.flags & ParseFlags.HaveDate) != 0)
            {
                // Multiple dates in the input string
                result.SetBadDateTimeFailure();
                return false;
            }

            if (SetDateYMD(ref result, raw.year, raw.month, raw.GetNumber(0)))
            {
                result.flags |= ParseFlags.HaveDate;
                return true;
            }
            result.SetBadDateTimeFailure();
            return false;
        }

        private static Boolean GetDayOfYN(ref DateTimeResult result, ref DateTimeRawInfo raw)
        {
            if ((result.flags & ParseFlags.HaveDate) != 0)
            {
                // Multiple dates in the input string
                result.SetBadDateTimeFailure();
                return false;
            }

            if (SetDateYMD(ref result, raw.year, raw.GetNumber(0), 1))
            {
                result.flags |= ParseFlags.HaveDate;
                return true;
            }
            result.SetBadDateTimeFailure();
            return false;
        }

        private static Boolean GetDayOfYM(ref DateTimeResult result, ref DateTimeRawInfo raw)
        {
            if ((result.flags & ParseFlags.HaveDate) != 0)
            {
                // Multiple dates in the input string
                result.SetBadDateTimeFailure();
                return false;
            }

            if (SetDateYMD(ref result, raw.year, raw.month, 1))
            {
                result.flags |= ParseFlags.HaveDate;
                return true;
            }
            result.SetBadDateTimeFailure();
            return false;
        }

        private static void AdjustTimeMark(DateTimeFormatInfo dtfi, ref DateTimeRawInfo raw)
        {
            // Specail case for culture which uses AM as empty string.
            // E.g. af-ZA (0x0436)
            //    S1159                  \x0000
            //    S2359                  nm
            // In this case, if we are parsing a string like "2005/09/14 12:23", we will assume this is in AM.

            if (raw.timeMark == TM.NotSet)
            {
                if (dtfi.AMDesignator != null && dtfi.PMDesignator != null)
                {
                    if (dtfi.AMDesignator.Length == 0 && dtfi.PMDesignator.Length != 0)
                    {
                        raw.timeMark = TM.AM;
                    }
                    if (dtfi.PMDesignator.Length == 0 && dtfi.AMDesignator.Length != 0)
                    {
                        raw.timeMark = TM.PM;
                    }
                }
            }
        }

        //
        // Adjust hour according to the time mark.
        //
        private static Boolean AdjustHour(ref int hour, TM timeMark)
        {
            if (timeMark != TM.NotSet)
            {
                if (timeMark == TM.AM)
                {
                    if (hour < 0 || hour > 12)
                    {
                        return false;
                    }
                    hour = (hour == 12) ? 0 : hour;
                }
                else
                {
                    if (hour < 0 || hour > 23)
                    {
                        return false;
                    }
                    if (hour < 12)
                    {
                        hour += 12;
                    }
                }
            }
            return true;
        }

        private static Boolean GetTimeOfN(ref DateTimeResult result, ref DateTimeRawInfo raw)
        {
            if ((result.flags & ParseFlags.HaveTime) != 0)
            {
                // Multiple times in the input string
                result.SetBadDateTimeFailure();
                return false;
            }
            //
            // In this case, we need a time mark. Check if so.
            //
            if (raw.timeMark == TM.NotSet)
            {
                result.SetBadDateTimeFailure();
                return false;
            }
            result.Hour = raw.GetNumber(0);
            result.flags |= ParseFlags.HaveTime;
            return true;
        }

        private static Boolean GetTimeOfNN(ref DateTimeResult result, ref DateTimeRawInfo raw)
        {
            Debug.Assert(raw.numCount >= 2, "raw.numCount >= 2");
            if ((result.flags & ParseFlags.HaveTime) != 0)
            {
                // Multiple times in the input string
                result.SetBadDateTimeFailure();
                return false;
            }

            result.Hour = raw.GetNumber(0);
            result.Minute = raw.GetNumber(1);
            result.flags |= ParseFlags.HaveTime;
            return true;
        }

        private static Boolean GetTimeOfNNN(ref DateTimeResult result, ref DateTimeRawInfo raw)
        {
            if ((result.flags & ParseFlags.HaveTime) != 0)
            {
                // Multiple times in the input string
                result.SetBadDateTimeFailure();
                return false;
            }
            Debug.Assert(raw.numCount >= 3, "raw.numCount >= 3");
            result.Hour = raw.GetNumber(0);
            result.Minute = raw.GetNumber(1);
            result.Second = raw.GetNumber(2);
            result.flags |= ParseFlags.HaveTime;
            return true;
        }

        //
        // Processing terminal state: A Date suffix followed by one number.
        //
        private static Boolean GetDateOfDSN(ref DateTimeResult result, ref DateTimeRawInfo raw)
        {
            if (raw.numCount != 1 || result.Day != -1)
            {
                result.SetBadDateTimeFailure();
                return false;
            }
            result.Day = raw.GetNumber(0);
            return true;
        }

        private static Boolean GetDateOfNDS(ref DateTimeResult result, ref DateTimeRawInfo raw)
        {
            if (result.Month == -1)
            {
                //Should have a month suffix
                result.SetBadDateTimeFailure();
                return false;
            }
            if (result.Year != -1)
            {
                // Already has a year suffix
                result.SetBadDateTimeFailure();
                return false;
            }
            if (!TryAdjustYear(ref result, raw.GetNumber(0), out result.Year))
            {
                // the year value is out of range
                result.SetBadDateTimeFailure();
                return false;
            }
            result.Day = 1;
            return true;
        }

        private static Boolean GetDateOfNNDS(ref DateTimeResult result, ref DateTimeRawInfo raw, DateTimeFormatInfo dtfi)
        {
            // For partial CJK Dates, the only valid formats are with a specified year, followed by two numbers, which
            // will be the Month and Day, and with a specified Month, when the numbers are either the year and day or
            // day and year, depending on the short date pattern.

            if ((result.flags & ParseFlags.HaveYear) != 0)
            {
                if (((result.flags & ParseFlags.HaveMonth) == 0) && ((result.flags & ParseFlags.HaveDay) == 0))
                {
                    if (TryAdjustYear(ref result, raw.year, out result.Year) && SetDateYMD(ref result, result.Year, raw.GetNumber(0), raw.GetNumber(1)))
                    {
                        return true;
                    }
                }
            }
            else if ((result.flags & ParseFlags.HaveMonth) != 0)
            {
                if (((result.flags & ParseFlags.HaveYear) == 0) && ((result.flags & ParseFlags.HaveDay) == 0))
                {
                    int order;
                    if (!GetYearMonthDayOrder(dtfi.ShortDatePattern, dtfi, out order))
                    {
                        result.SetFailure(ParseFailureKind.FormatWithParameter, nameof(SR.Format_BadDatePattern), dtfi.ShortDatePattern);
                        return false;
                    }
                    int year;
                    if (order == ORDER_YMD)
                    {
                        if (TryAdjustYear(ref result, raw.GetNumber(0), out year) && SetDateYMD(ref result, year, result.Month, raw.GetNumber(1)))
                        {
                            return true;
                        }
                    }
                    else
                    {
                        if (TryAdjustYear(ref result, raw.GetNumber(1), out year) && SetDateYMD(ref result, year, result.Month, raw.GetNumber(0)))
                        {
                            return true;
                        }
                    }
                }
            }
            result.SetBadDateTimeFailure();
            return false;
        }

        //
        // A date suffix is found, use this method to put the number into the result.
        //
        private static bool ProcessDateTimeSuffix(ref DateTimeResult result, ref DateTimeRawInfo raw, ref DateTimeToken dtok)
        {
            switch (dtok.suffix)
            {
                case TokenType.SEP_YearSuff:
                    if ((result.flags & ParseFlags.HaveYear) != 0)
                    {
                        return false;
                    }
                    result.flags |= ParseFlags.HaveYear;
                    result.Year = raw.year = dtok.num;
                    break;
                case TokenType.SEP_MonthSuff:
                    if ((result.flags & ParseFlags.HaveMonth) != 0)
                    {
                        return false;
                    }
                    result.flags |= ParseFlags.HaveMonth;
                    result.Month = raw.month = dtok.num;
                    break;
                case TokenType.SEP_DaySuff:
                    if ((result.flags & ParseFlags.HaveDay) != 0)
                    {
                        return false;
                    }
                    result.flags |= ParseFlags.HaveDay;
                    result.Day = dtok.num;
                    break;
                case TokenType.SEP_HourSuff:
                    if ((result.flags & ParseFlags.HaveHour) != 0)
                    {
                        return false;
                    }
                    result.flags |= ParseFlags.HaveHour;
                    result.Hour = dtok.num;
                    break;
                case TokenType.SEP_MinuteSuff:
                    if ((result.flags & ParseFlags.HaveMinute) != 0)
                    {
                        return false;
                    }
                    result.flags |= ParseFlags.HaveMinute;
                    result.Minute = dtok.num;
                    break;
                case TokenType.SEP_SecondSuff:
                    if ((result.flags & ParseFlags.HaveSecond) != 0)
                    {
                        return false;
                    }
                    result.flags |= ParseFlags.HaveSecond;
                    result.Second = dtok.num;
                    break;
            }
            return true;
        }

        ////////////////////////////////////////////////////////////////////////
        //
        // Actions:
        // This is used by DateTime.Parse().
        // Process the terminal state for the Hebrew calendar parsing.
        //
        ////////////////////////////////////////////////////////////////////////

        internal static Boolean ProcessHebrewTerminalState(DS dps, ref __DTString str, ref DateTimeResult result, ref DateTimeStyles styles, ref DateTimeRawInfo raw, DateTimeFormatInfo dtfi)
        {
            // The following are accepted terminal state for Hebrew date.
            switch (dps)
            {
                case DS.DX_MNN:
                    // Deal with the default long/short date format when the year number is ambigous (i.e. year < 100).
                    raw.year = raw.GetNumber(1);
                    if (!dtfi.YearMonthAdjustment(ref raw.year, ref raw.month, true))
                    {
                        result.SetFailure(ParseFailureKind.FormatBadDateTimeCalendar, nameof(SR.Format_BadDateTimeCalendar));
                        return false;
                    }
                    if (!GetDayOfMNN(ref result, ref raw, dtfi))
                    {
                        return false;
                    }
                    break;
                case DS.DX_YMN:
                    // Deal with the default long/short date format when the year number is NOT ambigous (i.e. year >= 100).
                    if (!dtfi.YearMonthAdjustment(ref raw.year, ref raw.month, true))
                    {
                        result.SetFailure(ParseFailureKind.FormatBadDateTimeCalendar, nameof(SR.Format_BadDateTimeCalendar));
                        return false;
                    }
                    if (!GetDayOfYMN(ref result, ref raw))
                    {
                        return false;
                    }
                    break;
                case DS.DX_NNY:
                    // When formatting, we only format up to the hundred digit of the Hebrew year, although Hebrew year is now over 5000.
                    // E.g. if the year is 5763, we only format as 763. so we do the reverse when parsing.
                    if (raw.year < 1000)
                    {
                        raw.year += 5000;
                    }
                    if (!GetDayOfNNY(ref result, ref raw, dtfi))
                    {
                        return false;
                    }
                    if (!dtfi.YearMonthAdjustment(ref result.Year, ref raw.month, true))
                    {
                        result.SetFailure(ParseFailureKind.FormatBadDateTimeCalendar, nameof(SR.Format_BadDateTimeCalendar));
                        return false;
                    }
                    break;
                case DS.DX_NM:
                case DS.DX_MN:
                    // Deal with Month/Day pattern.
                    GetDefaultYear(ref result, ref styles);
                    if (!dtfi.YearMonthAdjustment(ref result.Year, ref raw.month, true))
                    {
                        result.SetFailure(ParseFailureKind.FormatBadDateTimeCalendar, nameof(SR.Format_BadDateTimeCalendar));
                        return false;
                    }
                    if (!GetHebrewDayOfNM(ref result, ref raw, dtfi))
                    {
                        return false;
                    }
                    break;
                case DS.DX_YM:
                    // Deal with Year/Month pattern.
                    if (!dtfi.YearMonthAdjustment(ref raw.year, ref raw.month, true))
                    {
                        result.SetFailure(ParseFailureKind.FormatBadDateTimeCalendar, nameof(SR.Format_BadDateTimeCalendar));
                        return false;
                    }
                    if (!GetDayOfYM(ref result, ref raw))
                    {
                        return false;
                    }
                    break;
                case DS.TX_N:
                    // Deal hour + AM/PM
                    if (!GetTimeOfN(ref result, ref raw))
                    {
                        return false;
                    }
                    break;
                case DS.TX_NN:
                    if (!GetTimeOfNN(ref result, ref raw))
                    {
                        return false;
                    }
                    break;
                case DS.TX_NNN:
                    if (!GetTimeOfNNN(ref result, ref raw))
                    {
                        return false;
                    }
                    break;
                default:
                    result.SetBadDateTimeFailure();
                    return false;
            }
            if (dps > DS.ERROR)
            {
                //
                // We have reached a terminal state. Reset the raw num count.
                //
                raw.numCount = 0;
            }
            return true;
        }

        //
        // A terminal state has been reached, call the appropriate function to fill in the parsing result.
        // Return true if the state is a terminal state.
        //
        internal static Boolean ProcessTerminalState(DS dps, ref __DTString str, ref DateTimeResult result, ref DateTimeStyles styles, ref DateTimeRawInfo raw, DateTimeFormatInfo dtfi)
        {
            bool passed = true;
            switch (dps)
            {
                case DS.DX_NN:
                    passed = GetDayOfNN(ref result, ref styles, ref raw, dtfi);
                    break;
                case DS.DX_NNN:
                    passed = GetDayOfNNN(ref result, ref raw, dtfi);
                    break;
                case DS.DX_MN:
                    passed = GetDayOfMN(ref result, ref styles, ref raw, dtfi);
                    break;
                case DS.DX_NM:
                    passed = GetDayOfNM(ref result, ref styles, ref raw, dtfi);
                    break;
                case DS.DX_MNN:
                    passed = GetDayOfMNN(ref result, ref raw, dtfi);
                    break;
                case DS.DX_DS:
                    // The result has got the correct value. No need to process.
                    passed = true;
                    break;
                case DS.DX_YNN:
                    passed = GetDayOfYNN(ref result, ref raw, dtfi);
                    break;
                case DS.DX_NNY:
                    passed = GetDayOfNNY(ref result, ref raw, dtfi);
                    break;
                case DS.DX_YMN:
                    passed = GetDayOfYMN(ref result, ref raw);
                    break;
                case DS.DX_YN:
                    passed = GetDayOfYN(ref result, ref raw);
                    break;
                case DS.DX_YM:
                    passed = GetDayOfYM(ref result, ref raw);
                    break;
                case DS.TX_N:
                    passed = GetTimeOfN(ref result, ref raw);
                    break;
                case DS.TX_NN:
                    passed = GetTimeOfNN(ref result, ref raw);
                    break;
                case DS.TX_NNN:
                    passed = GetTimeOfNNN(ref result, ref raw);
                    break;
                case DS.TX_TS:
                    // The result has got the correct value. Nothing to do.
                    passed = true;
                    break;
                case DS.DX_DSN:
                    passed = GetDateOfDSN(ref result, ref raw);
                    break;
                case DS.DX_NDS:
                    passed = GetDateOfNDS(ref result, ref raw);
                    break;
                case DS.DX_NNDS:
                    passed = GetDateOfNNDS(ref result, ref raw, dtfi);
                    break;
            }

            PTSTraceExit(dps, passed);
            if (!passed)
            {
                return false;
            }

            if (dps > DS.ERROR)
            {
                //
                // We have reached a terminal state. Reset the raw num count.
                //
                raw.numCount = 0;
            }
            return true;
        }

        internal static DateTime Parse(ReadOnlySpan<char> s, DateTimeFormatInfo dtfi, DateTimeStyles styles)
        {
            DateTimeResult result = new DateTimeResult();       // The buffer to store the parsing result.
            result.Init(s);
            if (TryParse(s, dtfi, styles, ref result))
            {
                return result.parsedDate;
            }
            else
            {
                throw GetDateTimeParseException(ref result);
            }
        }

        internal static DateTime Parse(ReadOnlySpan<char> s, DateTimeFormatInfo dtfi, DateTimeStyles styles, out TimeSpan offset)
        {
            DateTimeResult result = new DateTimeResult();       // The buffer to store the parsing result.
            result.Init(s);
            result.flags |= ParseFlags.CaptureOffset;
            if (TryParse(s, dtfi, styles, ref result))
            {
                offset = result.timeZoneOffset;
                return result.parsedDate;
            }
            else
            {
                throw GetDateTimeParseException(ref result);
            }
        }


        internal static bool TryParse(ReadOnlySpan<char> s, DateTimeFormatInfo dtfi, DateTimeStyles styles, out DateTime result)
        {
            result = DateTime.MinValue;
            DateTimeResult resultData = new DateTimeResult();       // The buffer to store the parsing result.
            resultData.Init(s);
            if (TryParse(s, dtfi, styles, ref resultData))
            {
                result = resultData.parsedDate;
                return true;
            }
            return false;
        }

        internal static bool TryParse(ReadOnlySpan<char> s, DateTimeFormatInfo dtfi, DateTimeStyles styles, out DateTime result, out TimeSpan offset)
        {
            result = DateTime.MinValue;
            offset = TimeSpan.Zero;
            DateTimeResult parseResult = new DateTimeResult();       // The buffer to store the parsing result.
            parseResult.Init(s);
            parseResult.flags |= ParseFlags.CaptureOffset;
            if (TryParse(s, dtfi, styles, ref parseResult))
            {
                result = parseResult.parsedDate;
                offset = parseResult.timeZoneOffset;
                return true;
            }
            return false;
        }


        //
        // This is the real method to do the parsing work.
        //
        internal static bool TryParse(ReadOnlySpan<char> s, DateTimeFormatInfo dtfi, DateTimeStyles styles, ref DateTimeResult result)
        {
            if (s.Length == 0)
            {
                result.SetFailure(ParseFailureKind.FormatWithParameter, nameof(SR.Format_BadDateTime));
                return false;
            }

            Debug.Assert(dtfi != null, "dtfi == null");

#if _LOGGING
            DTFITrace(dtfi);
#endif

            DateTime time;
            //
            // First try the predefined format.
            //

            DS dps = DS.BEGIN;     // Date Parsing State.
            bool reachTerminalState = false;

            DateTimeToken dtok = new DateTimeToken();      // The buffer to store the parsing token.
            dtok.suffix = TokenType.SEP_Unk;
            DateTimeRawInfo raw = new DateTimeRawInfo();    // The buffer to store temporary parsing information.
            unsafe
            {
                Int32* numberPointer = stackalloc Int32[3];
                raw.Init(numberPointer);
            }
            raw.hasSameDateAndTimeSeparators = dtfi.DateSeparator.Equals(dtfi.TimeSeparator, StringComparison.Ordinal);

            result.calendar = dtfi.Calendar;
            result.era = Calendar.CurrentEra;

            //
            // The string to be parsed. Use a __DTString wrapper so that we can trace the index which
            // indicates the begining of next token.
            //
            __DTString str = new __DTString(s, dtfi);

            str.GetNext();

            //
            // The following loop will break out when we reach the end of the str.
            //
            do
            {
                //
                // Call the lexer to get the next token.
                //
                // If we find a era in Lex(), the era value will be in raw.era.
                if (!Lex(dps, ref str, ref dtok, ref raw, ref result, ref dtfi, styles))
                {
                    TPTraceExit("0000", dps);
                    return false;
                }

                //
                // If the token is not unknown, process it.
                // Otherwise, just discard it.
                //
                if (dtok.dtt != DTT.Unk)
                {
                    //
                    // Check if we got any CJK Date/Time suffix.
                    // Since the Date/Time suffix tells us the number belongs to year/month/day/hour/minute/second,
                    // store the number in the appropriate field in the result.
                    //
                    if (dtok.suffix != TokenType.SEP_Unk)
                    {
                        if (!ProcessDateTimeSuffix(ref result, ref raw, ref dtok))
                        {
                            result.SetBadDateTimeFailure();
                            TPTraceExit("0010", dps);
                            return false;
                        }

                        dtok.suffix = TokenType.SEP_Unk;  // Reset suffix to SEP_Unk;
                    }

                    if (dtok.dtt == DTT.NumLocalTimeMark)
                    {
                        if (dps == DS.D_YNd || dps == DS.D_YN)
                        {
                            // Consider this as ISO 8601 format:
                            // "yyyy-MM-dd'T'HH:mm:ss"                 1999-10-31T02:00:00
                            TPTraceExit("0020", dps);
                            return (ParseISO8601(ref raw, ref str, styles, ref result));
                        }
                        else
                        {
                            result.SetBadDateTimeFailure();
                            TPTraceExit("0030", dps);
                            return false;
                        }
                    }

                    if (raw.hasSameDateAndTimeSeparators)
                    {
                        if (dtok.dtt == DTT.YearEnd || dtok.dtt == DTT.YearSpace || dtok.dtt == DTT.YearDateSep)
                        {
                            // When time and date separators are same and we are hitting a year number while the first parsed part of the string was recognized
                            // as part of time (and not a date) DS.T_Nt, DS.T_NNt then change the state to be a date so we try to parse it as a date instead
                            if (dps == DS.T_Nt)
                            {
                                dps = DS.D_Nd;
                            }
                            if (dps == DS.T_NNt)
                            {
                                dps = DS.D_NNd;
                            }
                        }

                        bool atEnd = str.AtEnd();
                        if (dateParsingStates[(int)dps][(int)dtok.dtt] == DS.ERROR || atEnd)
                        {
                            switch (dtok.dtt)
                            {
                                // we have the case of Serbia have dates in forms 'd.M.yyyy.' so we can expect '.' after the date parts.
                                // changing the token to end with space instead of Date Separator will avoid failing the parsing.

                                case DTT.YearDateSep: dtok.dtt = atEnd ? DTT.YearEnd : DTT.YearSpace; break;
                                case DTT.NumDatesep: dtok.dtt = atEnd ? DTT.NumEnd : DTT.NumSpace; break;
                                case DTT.NumTimesep: dtok.dtt = atEnd ? DTT.NumEnd : DTT.NumSpace; break;
                                case DTT.MonthDatesep: dtok.dtt = atEnd ? DTT.MonthEnd : DTT.MonthSpace; break;
                            }
                        }
                    }

                    //
                    // Advance to the next state, and continue
                    //
                    dps = dateParsingStates[(int)dps][(int)dtok.dtt];

                    if (dps == DS.ERROR)
                    {
                        result.SetBadDateTimeFailure();
                        TPTraceExit("0040 (invalid state transition)", dps);
                        return false;
                    }
                    else if (dps > DS.ERROR)
                    {
                        if ((dtfi.FormatFlags & DateTimeFormatFlags.UseHebrewRule) != 0)
                        {
                            if (!ProcessHebrewTerminalState(dps, ref str, ref result, ref styles, ref raw, dtfi))
                            {
                                TPTraceExit("0050 (ProcessHebrewTerminalState)", dps);
                                return false;
                            }
                        }
                        else
                        {
                            if (!ProcessTerminalState(dps, ref str, ref result, ref styles, ref raw, dtfi))
                            {
                                TPTraceExit("0060 (ProcessTerminaltState)", dps);
                                return false;
                            }
                        }
                        reachTerminalState = true;

                        //
                        // If we have reached a terminal state, start over from DS.BEGIN again.
                        // For example, when we parsed "1999-12-23 13:30", we will reach a terminal state at "1999-12-23",
                        // and we start over so we can continue to parse "12:30".
                        //
                        dps = DS.BEGIN;
                    }
                }
            } while (dtok.dtt != DTT.End && dtok.dtt != DTT.NumEnd && dtok.dtt != DTT.MonthEnd);

            if (!reachTerminalState)
            {
                result.SetBadDateTimeFailure();
                TPTraceExit("0070 (did not reach terminal state)", dps);
                return false;
            }

            AdjustTimeMark(dtfi, ref raw);
            if (!AdjustHour(ref result.Hour, raw.timeMark))
            {
                result.SetBadDateTimeFailure();
                TPTraceExit("0080 (AdjustHour)", dps);
                return false;
            }

            // Check if the parased string only contains hour/minute/second values.
            bool bTimeOnly = (result.Year == -1 && result.Month == -1 && result.Day == -1);

            //
            // Check if any year/month/day is missing in the parsing string.
            // If yes, get the default value from today's date.
            //
            if (!CheckDefaultDateTime(ref result, ref result.calendar, styles))
            {
                TPTraceExit("0090 (failed to fill in missing year/month/day defaults)", dps);
                return false;
            }

            if (!result.calendar.TryToDateTime(result.Year, result.Month, result.Day,
                    result.Hour, result.Minute, result.Second, 0, result.era, out time))
            {
                result.SetFailure(ParseFailureKind.FormatBadDateTimeCalendar, nameof(SR.Format_BadDateTimeCalendar));
                TPTraceExit("0100 (result.calendar.TryToDateTime)", dps);
                return false;
            }
            if (raw.fraction > 0)
            {
                time = time.AddTicks((long)Math.Round(raw.fraction * Calendar.TicksPerSecond));
            }

            //
            // We have to check day of week before we adjust to the time zone.
            // Otherwise, the value of day of week may change after adjusting to the time zone.
            //
            if (raw.dayOfWeek != -1)
            {
                //
                // Check if day of week is correct.
                //
                if (raw.dayOfWeek != (int)result.calendar.GetDayOfWeek(time))
                {
                    result.SetFailure(ParseFailureKind.FormatWithOriginalDateTime, nameof(SR.Format_BadDayOfWeek));
                    TPTraceExit("0110 (dayOfWeek check)", dps);
                    return false;
                }
            }

            result.parsedDate = time;

            if (!DetermineTimeZoneAdjustments(ref str, ref result, styles, bTimeOnly))
            {
                TPTraceExit("0120 (DetermineTimeZoneAdjustments)", dps);
                return false;
            }
            TPTraceExit("0130 (success)", dps);
            return true;
        }


        // Handles time zone adjustments and sets DateTimeKind values as required by the styles
        private static Boolean DetermineTimeZoneAdjustments(ref __DTString str, ref DateTimeResult result, DateTimeStyles styles, Boolean bTimeOnly)
        {
            if ((result.flags & ParseFlags.CaptureOffset) != 0)
            {
                // This is a DateTimeOffset parse, so the offset will actually be captured directly, and
                // no adjustment is required in most cases
                return DateTimeOffsetTimeZonePostProcessing(ref str, ref result, styles);
            }
            else
            {
                Int64 offsetTicks = result.timeZoneOffset.Ticks;

                // the DateTime offset must be within +- 14:00 hours.
                if (offsetTicks < DateTimeOffset.MinOffset || offsetTicks > DateTimeOffset.MaxOffset)
                {
                    result.SetFailure(ParseFailureKind.FormatWithOriginalDateTime, nameof(SR.Format_OffsetOutOfRange));
                    return false;
                }
            }

            // The flags AssumeUniveral and AssumeLocal only apply when the input does not have a time zone
            if ((result.flags & ParseFlags.TimeZoneUsed) == 0)
            {
                // If AssumeLocal or AssumeLocal is used, there will always be a kind specified. As in the
                // case when a time zone is present, it will default to being local unless AdjustToUniversal
                // is present. These comparisons determine whether setting the kind is sufficient, or if a
                // time zone adjustment is required. For consistentcy with the rest of parsing, it is desirable
                // to fall through to the Adjust methods below, so that there is consist handling of boundary
                // cases like wrapping around on time-only dates and temporarily allowing an adjusted date
                // to exceed DateTime.MaxValue
                if ((styles & DateTimeStyles.AssumeLocal) != 0)
                {
                    if ((styles & DateTimeStyles.AdjustToUniversal) != 0)
                    {
                        result.flags |= ParseFlags.TimeZoneUsed;
                        result.timeZoneOffset = TimeZoneInfo.GetLocalUtcOffset(result.parsedDate, TimeZoneInfoOptions.NoThrowOnInvalidTime);
                    }
                    else
                    {
                        result.parsedDate = DateTime.SpecifyKind(result.parsedDate, DateTimeKind.Local);
                        return true;
                    }
                }
                else if ((styles & DateTimeStyles.AssumeUniversal) != 0)
                {
                    if ((styles & DateTimeStyles.AdjustToUniversal) != 0)
                    {
                        result.parsedDate = DateTime.SpecifyKind(result.parsedDate, DateTimeKind.Utc);
                        return true;
                    }
                    else
                    {
                        result.flags |= ParseFlags.TimeZoneUsed;
                        result.timeZoneOffset = TimeSpan.Zero;
                    }
                }
                else
                {
                    // No time zone and no Assume flags, so DateTimeKind.Unspecified is fine
                    Debug.Assert(result.parsedDate.Kind == DateTimeKind.Unspecified, "result.parsedDate.Kind == DateTimeKind.Unspecified");
                    return true;
                }
            }

            if (((styles & DateTimeStyles.RoundtripKind) != 0) && ((result.flags & ParseFlags.TimeZoneUtc) != 0))
            {
                result.parsedDate = DateTime.SpecifyKind(result.parsedDate, DateTimeKind.Utc);
                return true;
            }

            if ((styles & DateTimeStyles.AdjustToUniversal) != 0)
            {
                return (AdjustTimeZoneToUniversal(ref result));
            }
            return (AdjustTimeZoneToLocal(ref result, bTimeOnly));
        }

        // Apply validation and adjustments specific to DateTimeOffset
        private static Boolean DateTimeOffsetTimeZonePostProcessing(ref __DTString str, ref DateTimeResult result, DateTimeStyles styles)
        {
            // For DateTimeOffset, default to the Utc or Local offset when an offset was not specified by
            // the input string.
            if ((result.flags & ParseFlags.TimeZoneUsed) == 0)
            {
                if ((styles & DateTimeStyles.AssumeUniversal) != 0)
                {
                    // AssumeUniversal causes the offset to default to zero (0)
                    result.timeZoneOffset = TimeSpan.Zero;
                }
                else
                {
                    // AssumeLocal causes the offset to default to Local.  This flag is on by default for DateTimeOffset.
                    result.timeZoneOffset = TimeZoneInfo.GetLocalUtcOffset(result.parsedDate, TimeZoneInfoOptions.NoThrowOnInvalidTime);
                }
            }

            Int64 offsetTicks = result.timeZoneOffset.Ticks;

            // there should be no overflow, because the offset can be no more than -+100 hours and the date already
            // fits within a DateTime.
            Int64 utcTicks = result.parsedDate.Ticks - offsetTicks;

            // For DateTimeOffset, both the parsed time and the corresponding UTC value must be within the boundaries
            // of a DateTime instance.
            if (utcTicks < DateTime.MinTicks || utcTicks > DateTime.MaxTicks)
            {
                result.SetFailure(ParseFailureKind.FormatWithOriginalDateTime, nameof(SR.Format_UTCOutOfRange));
                return false;
            }

            // the offset must be within +- 14:00 hours.
            if (offsetTicks < DateTimeOffset.MinOffset || offsetTicks > DateTimeOffset.MaxOffset)
            {
                result.SetFailure(ParseFailureKind.FormatWithOriginalDateTime, nameof(SR.Format_OffsetOutOfRange));
                return false;
            }

            // DateTimeOffset should still honor the AdjustToUniversal flag for consistency with DateTime. It means you
            // want to return an adjusted UTC value, so store the utcTicks in the DateTime and set the offset to zero
            if ((styles & DateTimeStyles.AdjustToUniversal) != 0)
            {
                if (((result.flags & ParseFlags.TimeZoneUsed) == 0) && ((styles & DateTimeStyles.AssumeUniversal) == 0))
                {
                    // Handle the special case where the timeZoneOffset was defaulted to Local
                    Boolean toUtcResult = AdjustTimeZoneToUniversal(ref result);
                    result.timeZoneOffset = TimeSpan.Zero;
                    return toUtcResult;
                }

                // The constructor should always succeed because of the range check earlier in the function
                // Although it is UTC, internally DateTimeOffset does not use this flag
                result.parsedDate = new DateTime(utcTicks, DateTimeKind.Utc);
                result.timeZoneOffset = TimeSpan.Zero;
            }

            return true;
        }


        //
        // Adjust the specified time to universal time based on the supplied timezone.
        // E.g. when parsing "2001/06/08 14:00-07:00",
        // the time is 2001/06/08 14:00, and timeZoneOffset = -07:00.
        // The result will be "2001/06/08 21:00"
        //
        private static Boolean AdjustTimeZoneToUniversal(ref DateTimeResult result)
        {
            long resultTicks = result.parsedDate.Ticks;
            resultTicks -= result.timeZoneOffset.Ticks;
            if (resultTicks < 0)
            {
                resultTicks += Calendar.TicksPerDay;
            }

            if (resultTicks < DateTime.MinTicks || resultTicks > DateTime.MaxTicks)
            {
                result.SetFailure(ParseFailureKind.FormatWithOriginalDateTime, nameof(SR.Format_DateOutOfRange));
                return false;
            }
            result.parsedDate = new DateTime(resultTicks, DateTimeKind.Utc);
            return true;
        }

        //
        // Adjust the specified time to universal time based on the supplied timezone,
        // and then convert to local time.
        // E.g. when parsing "2001/06/08 14:00-04:00", and local timezone is GMT-7.
        // the time is 2001/06/08 14:00, and timeZoneOffset = -05:00.
        // The result will be "2001/06/08 11:00"
        //
        private static Boolean AdjustTimeZoneToLocal(ref DateTimeResult result, bool bTimeOnly)
        {
            long resultTicks = result.parsedDate.Ticks;
            // Convert to local ticks
            TimeZoneInfo tz = TimeZoneInfo.Local;
            Boolean isAmbiguousLocalDst = false;
            if (resultTicks < Calendar.TicksPerDay)
            {
                //
                // This is time of day.
                //

                // Adjust timezone.
                resultTicks -= result.timeZoneOffset.Ticks;
                // If the time is time of day, use the current timezone offset.
                resultTicks += tz.GetUtcOffset(bTimeOnly ? DateTime.Now : result.parsedDate, TimeZoneInfoOptions.NoThrowOnInvalidTime).Ticks;

                if (resultTicks < 0)
                {
                    resultTicks += Calendar.TicksPerDay;
                }
            }
            else
            {
                // Adjust timezone to GMT.
                resultTicks -= result.timeZoneOffset.Ticks;
                if (resultTicks < DateTime.MinTicks || resultTicks > DateTime.MaxTicks)
                {
                    // If the result ticks is greater than DateTime.MaxValue, we can not create a DateTime from this ticks.
                    // In this case, keep using the old code.
                    resultTicks += tz.GetUtcOffset(result.parsedDate, TimeZoneInfoOptions.NoThrowOnInvalidTime).Ticks;
                }
                else
                {
                    // Convert the GMT time to local time.
                    DateTime utcDt = new DateTime(resultTicks, DateTimeKind.Utc);
                    Boolean isDaylightSavings = false;
                    resultTicks += TimeZoneInfo.GetUtcOffsetFromUtc(utcDt, TimeZoneInfo.Local, out isDaylightSavings, out isAmbiguousLocalDst).Ticks;
                }
            }
            if (resultTicks < DateTime.MinTicks || resultTicks > DateTime.MaxTicks)
            {
                result.parsedDate = DateTime.MinValue;
                result.SetFailure(ParseFailureKind.FormatWithOriginalDateTime, nameof(SR.Format_DateOutOfRange));
                return false;
            }
            result.parsedDate = new DateTime(resultTicks, DateTimeKind.Local, isAmbiguousLocalDst);
            return true;
        }

        //
        // Parse the ISO8601 format string found during Parse();
        //
        //
        private static bool ParseISO8601(ref DateTimeRawInfo raw, ref __DTString str, DateTimeStyles styles, ref DateTimeResult result)
        {
            if (raw.year < 0 || raw.GetNumber(0) < 0 || raw.GetNumber(1) < 0)
            {
            }
            str.Index--;
            int hour, minute;
            int second = 0;
            double partSecond = 0;

            str.SkipWhiteSpaces();
            if (!ParseDigits(ref str, 2, out hour))
            {
                result.SetBadDateTimeFailure();
                return false;
            }
            str.SkipWhiteSpaces();
            if (!str.Match(':'))
            {
                result.SetBadDateTimeFailure();
                return false;
            }
            str.SkipWhiteSpaces();
            if (!ParseDigits(ref str, 2, out minute))
            {
                result.SetBadDateTimeFailure();
                return false;
            }
            str.SkipWhiteSpaces();
            if (str.Match(':'))
            {
                str.SkipWhiteSpaces();
                if (!ParseDigits(ref str, 2, out second))
                {
                    result.SetBadDateTimeFailure();
                    return false;
                }
                if (str.Match('.'))
                {
                    if (!ParseFraction(ref str, out partSecond))
                    {
                        result.SetBadDateTimeFailure();
                        return false;
                    }
                    str.Index--;
                }
                str.SkipWhiteSpaces();
            }
            if (str.GetNext())
            {
                char ch = str.GetChar();
                if (ch == '+' || ch == '-')
                {
                    result.flags |= ParseFlags.TimeZoneUsed;
                    if (!ParseTimeZone(ref str, ref result.timeZoneOffset))
                    {
                        result.SetBadDateTimeFailure();
                        return false;
                    }
                }
                else if (ch == 'Z' || ch == 'z')
                {
                    result.flags |= ParseFlags.TimeZoneUsed;
                    result.timeZoneOffset = TimeSpan.Zero;
                    result.flags |= ParseFlags.TimeZoneUtc;
                }
                else
                {
                    str.Index--;
                }
                str.SkipWhiteSpaces();
                if (str.Match('#'))
                {
                    if (!VerifyValidPunctuation(ref str))
                    {
                        result.SetBadDateTimeFailure();
                        return false;
                    }
                    str.SkipWhiteSpaces();
                }
                if (str.Match('\0'))
                {
                    if (!VerifyValidPunctuation(ref str))
                    {
                        result.SetBadDateTimeFailure();
                        return false;
                    }
                }
                if (str.GetNext())
                {
                    // If this is true, there were non-white space characters remaining in the DateTime
                    result.SetBadDateTimeFailure();
                    return false;
                }
            }

            DateTime time;
            Calendar calendar = GregorianCalendar.GetDefaultInstance();
            if (!calendar.TryToDateTime(raw.year, raw.GetNumber(0), raw.GetNumber(1),
                    hour, minute, second, 0, result.era, out time))
            {
                result.SetFailure(ParseFailureKind.FormatBadDateTimeCalendar, nameof(SR.Format_BadDateTimeCalendar));
                return false;
            }

            time = time.AddTicks((long)Math.Round(partSecond * Calendar.TicksPerSecond));
            result.parsedDate = time;
            if (!DetermineTimeZoneAdjustments(ref str, ref result, styles, false))
            {
                return false;
            }
            return true;
        }


        ////////////////////////////////////////////////////////////////////////
        //
        // Actions:
        //    Parse the current word as a Hebrew number.
        //      This is used by DateTime.ParseExact().
        //
        ////////////////////////////////////////////////////////////////////////

        internal static bool MatchHebrewDigits(ref __DTString str, int digitLen, out int number)
        {
            number = 0;

            // Create a context object so that we can parse the Hebrew number text character by character.
            HebrewNumberParsingContext context = new HebrewNumberParsingContext(0);

            // Set this to ContinueParsing so that we will run the following while loop in the first time.
            HebrewNumberParsingState state = HebrewNumberParsingState.ContinueParsing;

            while (state == HebrewNumberParsingState.ContinueParsing && str.GetNext())
            {
                state = HebrewNumber.ParseByChar(str.GetChar(), ref context);
            }

            if (state == HebrewNumberParsingState.FoundEndOfHebrewNumber)
            {
                // If we have reached a terminal state, update the result and returns.
                number = context.result;
                return (true);
            }

            // If we run out of the character before reaching FoundEndOfHebrewNumber, or
            // the state is InvalidHebrewNumber or ContinueParsing, we fail to match a Hebrew number.
            // Return an error.
            return false;
        }

        /*=================================ParseDigits==================================
        **Action: Parse the number string in __DTString that are formatted using
        **        the following patterns:
        **        "0", "00", and "000..0"
        **Returns: the integer value
        **Arguments:    str: a __DTString.  The parsing will start from the
        **              next character after str.Index.
        **Exceptions: FormatException if error in parsing number.
        ==============================================================================*/

        internal static bool ParseDigits(ref __DTString str, int digitLen, out int result)
        {
            if (digitLen == 1)
            {
                // 1 really means 1 or 2 for this call
                return ParseDigits(ref str, 1, 2, out result);
            }
            else
            {
                return ParseDigits(ref str, digitLen, digitLen, out result);
            }
        }

        internal static bool ParseDigits(ref __DTString str, int minDigitLen, int maxDigitLen, out int result)
        {
            Debug.Assert(minDigitLen > 0, "minDigitLen > 0");
            Debug.Assert(maxDigitLen < 9, "maxDigitLen < 9");
            Debug.Assert(minDigitLen <= maxDigitLen, "minDigitLen <= maxDigitLen");
            int localResult = 0;
            int startingIndex = str.Index;
            int tokenLength = 0;
            while (tokenLength < maxDigitLen)
            {
                if (!str.GetNextDigit())
                {
                    str.Index--;
                    break;
                }
                localResult = localResult * 10 + str.GetDigit();
                tokenLength++;
            }
            result = localResult;
            if (tokenLength < minDigitLen)
            {
                str.Index = startingIndex;
                return false;
            }
            return true;
        }

        /*=================================ParseFractionExact==================================
        **Action: Parse the number string in __DTString that are formatted using
        **        the following patterns:
        **        "0", "00", and "000..0"
        **Returns: the fraction value
        **Arguments:    str: a __DTString.  The parsing will start from the
        **              next character after str.Index.
        **Exceptions: FormatException if error in parsing number.
        ==============================================================================*/

        private static bool ParseFractionExact(ref __DTString str, int maxDigitLen, ref double result)
        {
            if (!str.GetNextDigit())
            {
                str.Index--;
                return false;
            }
            result = str.GetDigit();

            int digitLen = 1;
            for (; digitLen < maxDigitLen; digitLen++)
            {
                if (!str.GetNextDigit())
                {
                    str.Index--;
                    break;
                }
                result = result * 10 + str.GetDigit();
            }

            result /= TimeSpanParse.Pow10(digitLen);
            return (digitLen == maxDigitLen);
        }

        /*=================================ParseSign==================================
        **Action: Parse a positive or a negative sign.
        **Returns:      true if postive sign.  flase if negative sign.
        **Arguments:    str: a __DTString.  The parsing will start from the
        **              next character after str.Index.
        **Exceptions:   FormatException if end of string is encountered or a sign
        **              symbol is not found.
        ==============================================================================*/

        private static bool ParseSign(ref __DTString str, ref bool result)
        {
            if (!str.GetNext())
            {
                // A sign symbol ('+' or '-') is expected. However, end of string is encountered.
                return false;
            }
            char ch = str.GetChar();
            if (ch == '+')
            {
                result = true;
                return (true);
            }
            else if (ch == '-')
            {
                result = false;
                return (true);
            }
            // A sign symbol ('+' or '-') is expected.
            return false;
        }

        /*=================================ParseTimeZoneOffset==================================
        **Action: Parse the string formatted using "z", "zz", "zzz" in DateTime.Format().
        **Returns: the TimeSpan for the parsed timezone offset.
        **Arguments:    str: a __DTString.  The parsing will start from the
        **              next character after str.Index.
        **              len: the repeated number of the "z"
        **Exceptions: FormatException if errors in parsing.
        ==============================================================================*/

        private static bool ParseTimeZoneOffset(ref __DTString str, int len, ref TimeSpan result)
        {
            bool isPositive = true;
            int hourOffset;
            int minuteOffset = 0;

            switch (len)
            {
                case 1:
                case 2:
                    if (!ParseSign(ref str, ref isPositive))
                    {
                        return (false);
                    }
                    if (!ParseDigits(ref str, len, out hourOffset))
                    {
                        return (false);
                    }
                    break;
                default:
                    if (!ParseSign(ref str, ref isPositive))
                    {
                        return (false);
                    }

                    // Parsing 1 digit will actually parse 1 or 2.
                    if (!ParseDigits(ref str, 1, out hourOffset))
                    {
                        return (false);
                    }
                    // ':' is optional.
                    if (str.Match(":"))
                    {
                        // Found ':'
                        if (!ParseDigits(ref str, 2, out minuteOffset))
                        {
                            return (false);
                        }
                    }
                    else
                    {
                        // Since we can not match ':', put the char back.
                        str.Index--;
                        if (!ParseDigits(ref str, 2, out minuteOffset))
                        {
                            return (false);
                        }
                    }
                    break;
            }
            if (minuteOffset < 0 || minuteOffset >= 60)
            {
                return false;
            }

            result = (new TimeSpan(hourOffset, minuteOffset, 0));
            if (!isPositive)
            {
                result = result.Negate();
            }
            return (true);
        }

        /*=================================MatchAbbreviatedMonthName==================================
        **Action: Parse the abbreviated month name from string starting at str.Index.
        **Returns: A value from 1 to 12 for the first month to the twelveth month.
        **Arguments:    str: a __DTString.  The parsing will start from the
        **              next character after str.Index.
        **Exceptions: FormatException if an abbreviated month name can not be found.
        ==============================================================================*/

        private static bool MatchAbbreviatedMonthName(ref __DTString str, DateTimeFormatInfo dtfi, ref int result)
        {
            int maxMatchStrLen = 0;
            result = -1;
            if (str.GetNext())
            {
                //
                // Scan the month names (note that some calendars has 13 months) and find
                // the matching month name which has the max string length.
                // We need to do this because some cultures (e.g. "cs-CZ") which have
                // abbreviated month names with the same prefix.
                //
                int monthsInYear = (dtfi.GetMonthName(13).Length == 0 ? 12 : 13);
                for (int i = 1; i <= monthsInYear; i++)
                {
                    String searchStr = dtfi.GetAbbreviatedMonthName(i);
                    int matchStrLen = searchStr.Length;
                    if (dtfi.HasSpacesInMonthNames
                            ? str.MatchSpecifiedWords(searchStr, false, ref matchStrLen)
                            : str.MatchSpecifiedWord(searchStr))
                    {
                        if (matchStrLen > maxMatchStrLen)
                        {
                            maxMatchStrLen = matchStrLen;
                            result = i;
                        }
                    }
                }

                // Search leap year form.
                if ((dtfi.FormatFlags & DateTimeFormatFlags.UseLeapYearMonth) != 0)
                {
                    int tempResult = str.MatchLongestWords(dtfi.internalGetLeapYearMonthNames(), ref maxMatchStrLen);
                    // We found a longer match in the leap year month name.  Use this as the result.
                    // The result from MatchLongestWords is 0 ~ length of word array.
                    // So we increment the result by one to become the month value.
                    if (tempResult >= 0)
                    {
                        result = tempResult + 1;
                    }
                }
            }
            if (result > 0)
            {
                str.Index += (maxMatchStrLen - 1);
                return (true);
            }
            return false;
        }

        /*=================================MatchMonthName==================================
        **Action: Parse the month name from string starting at str.Index.
        **Returns: A value from 1 to 12 indicating the first month to the twelveth month.
        **Arguments:    str: a __DTString.  The parsing will start from the
        **              next character after str.Index.
        **Exceptions: FormatException if a month name can not be found.
        ==============================================================================*/

        private static bool MatchMonthName(ref __DTString str, DateTimeFormatInfo dtfi, ref int result)
        {
            int maxMatchStrLen = 0;
            result = -1;
            if (str.GetNext())
            {
                //
                // Scan the month names (note that some calendars has 13 months) and find
                // the matching month name which has the max string length.
                // We need to do this because some cultures (e.g. "vi-VN") which have
                // month names with the same prefix.
                //
                int monthsInYear = (dtfi.GetMonthName(13).Length == 0 ? 12 : 13);
                for (int i = 1; i <= monthsInYear; i++)
                {
                    String searchStr = dtfi.GetMonthName(i);
                    int matchStrLen = searchStr.Length;
                    if (dtfi.HasSpacesInMonthNames
                            ? str.MatchSpecifiedWords(searchStr, false, ref matchStrLen)
                            : str.MatchSpecifiedWord(searchStr))
                    {
                        if (matchStrLen > maxMatchStrLen)
                        {
                            maxMatchStrLen = matchStrLen;
                            result = i;
                        }
                    }
                }

                // Search genitive form.
                if ((dtfi.FormatFlags & DateTimeFormatFlags.UseGenitiveMonth) != 0)
                {
                    int tempResult = str.MatchLongestWords(dtfi.MonthGenitiveNames, ref maxMatchStrLen);
                    // We found a longer match in the genitive month name.  Use this as the result.
                    // The result from MatchLongestWords is 0 ~ length of word array.
                    // So we increment the result by one to become the month value.
                    if (tempResult >= 0)
                    {
                        result = tempResult + 1;
                    }
                }

                // Search leap year form.
                if ((dtfi.FormatFlags & DateTimeFormatFlags.UseLeapYearMonth) != 0)
                {
                    int tempResult = str.MatchLongestWords(dtfi.internalGetLeapYearMonthNames(), ref maxMatchStrLen);
                    // We found a longer match in the leap year month name.  Use this as the result.
                    // The result from MatchLongestWords is 0 ~ length of word array.
                    // So we increment the result by one to become the month value.
                    if (tempResult >= 0)
                    {
                        result = tempResult + 1;
                    }
                }
            }

            if (result > 0)
            {
                str.Index += (maxMatchStrLen - 1);
                return (true);
            }
            return false;
        }

        /*=================================MatchAbbreviatedDayName==================================
        **Action: Parse the abbreviated day of week name from string starting at str.Index.
        **Returns: A value from 0 to 6 indicating Sunday to Saturday.
        **Arguments:    str: a __DTString.  The parsing will start from the
        **              next character after str.Index.
        **Exceptions: FormatException if a abbreviated day of week name can not be found.
        ==============================================================================*/

        private static bool MatchAbbreviatedDayName(ref __DTString str, DateTimeFormatInfo dtfi, ref int result)
        {
            int maxMatchStrLen = 0;
            result = -1;
            if (str.GetNext())
            {
                for (DayOfWeek i = DayOfWeek.Sunday; i <= DayOfWeek.Saturday; i++)
                {
                    String searchStr = dtfi.GetAbbreviatedDayName(i);
                    int matchStrLen = searchStr.Length;
                    if (dtfi.HasSpacesInDayNames
                            ? str.MatchSpecifiedWords(searchStr, false, ref matchStrLen)
                            : str.MatchSpecifiedWord(searchStr))
                    {
                        if (matchStrLen > maxMatchStrLen)
                        {
                            maxMatchStrLen = matchStrLen;
                            result = (int)i;
                        }
                    }
                }
            }
            if (result >= 0)
            {
                str.Index += maxMatchStrLen - 1;
                return (true);
            }
            return false;
        }

        /*=================================MatchDayName==================================
        **Action: Parse the day of week name from string starting at str.Index.
        **Returns: A value from 0 to 6 indicating Sunday to Saturday.
        **Arguments:    str: a __DTString.  The parsing will start from the
        **              next character after str.Index.
        **Exceptions: FormatException if a day of week name can not be found.
        ==============================================================================*/

        private static bool MatchDayName(ref __DTString str, DateTimeFormatInfo dtfi, ref int result)
        {
            // Turkish (tr-TR) got day names with the same prefix.
            int maxMatchStrLen = 0;
            result = -1;
            if (str.GetNext())
            {
                for (DayOfWeek i = DayOfWeek.Sunday; i <= DayOfWeek.Saturday; i++)
                {
                    String searchStr = dtfi.GetDayName(i);
                    int matchStrLen = searchStr.Length;
                    if (dtfi.HasSpacesInDayNames
                            ? str.MatchSpecifiedWords(searchStr, false, ref matchStrLen)
                            : str.MatchSpecifiedWord(searchStr))
                    {
                        if (matchStrLen > maxMatchStrLen)
                        {
                            maxMatchStrLen = matchStrLen;
                            result = (int)i;
                        }
                    }
                }
            }
            if (result >= 0)
            {
                str.Index += maxMatchStrLen - 1;
                return (true);
            }
            return false;
        }

        /*=================================MatchEraName==================================
        **Action: Parse era name from string starting at str.Index.
        **Returns: An era value.
        **Arguments:    str: a __DTString.  The parsing will start from the
        **              next character after str.Index.
        **Exceptions: FormatException if an era name can not be found.
        ==============================================================================*/

        private static bool MatchEraName(ref __DTString str, DateTimeFormatInfo dtfi, ref int result)
        {
            if (str.GetNext())
            {
                int[] eras = dtfi.Calendar.Eras;

                if (eras != null)
                {
                    for (int i = 0; i < eras.Length; i++)
                    {
                        String searchStr = dtfi.GetEraName(eras[i]);
                        if (str.MatchSpecifiedWord(searchStr))
                        {
                            str.Index += (searchStr.Length - 1);
                            result = eras[i];
                            return (true);
                        }
                        searchStr = dtfi.GetAbbreviatedEraName(eras[i]);
                        if (str.MatchSpecifiedWord(searchStr))
                        {
                            str.Index += (searchStr.Length - 1);
                            result = eras[i];
                            return (true);
                        }
                    }
                }
            }
            return false;
        }

        /*=================================MatchTimeMark==================================
        **Action: Parse the time mark (AM/PM) from string starting at str.Index.
        **Returns: TM_AM or TM_PM.
        **Arguments:    str: a __DTString.  The parsing will start from the
        **              next character after str.Index.
        **Exceptions: FormatException if a time mark can not be found.
        ==============================================================================*/

        private static bool MatchTimeMark(ref __DTString str, DateTimeFormatInfo dtfi, ref TM result)
        {
            result = TM.NotSet;
            // In some cultures have empty strings in AM/PM mark. E.g. af-ZA (0x0436), the AM mark is "", and PM mark is "nm".
            if (dtfi.AMDesignator.Length == 0)
            {
                result = TM.AM;
            }
            if (dtfi.PMDesignator.Length == 0)
            {
                result = TM.PM;
            }

            if (str.GetNext())
            {
                String searchStr = dtfi.AMDesignator;
                if (searchStr.Length > 0)
                {
                    if (str.MatchSpecifiedWord(searchStr))
                    {
                        // Found an AM timemark with length > 0.
                        str.Index += (searchStr.Length - 1);
                        result = TM.AM;
                        return (true);
                    }
                }
                searchStr = dtfi.PMDesignator;
                if (searchStr.Length > 0)
                {
                    if (str.MatchSpecifiedWord(searchStr))
                    {
                        // Found a PM timemark with length > 0.
                        str.Index += (searchStr.Length - 1);
                        result = TM.PM;
                        return (true);
                    }
                }
                str.Index--; // Undo the GetNext call.
            }
            if (result != TM.NotSet)
            {
                // If one of the AM/PM marks is empty string, return the result.
                return (true);
            }
            return false;
        }

        /*=================================MatchAbbreviatedTimeMark==================================
        **Action: Parse the abbreviated time mark (AM/PM) from string starting at str.Index.
        **Returns: TM_AM or TM_PM.
        **Arguments:    str: a __DTString.  The parsing will start from the
        **              next character after str.Index.
        **Exceptions: FormatException if a abbreviated time mark can not be found.
        ==============================================================================*/

        private static bool MatchAbbreviatedTimeMark(ref __DTString str, DateTimeFormatInfo dtfi, ref TM result)
        {
            // NOTENOTE : the assumption here is that abbreviated time mark is the first
            // character of the AM/PM designator.  If this invariant changes, we have to
            // change the code below.
            if (str.GetNext())
            {
                string amDesignator = dtfi.AMDesignator;
                if (amDesignator.Length > 0 && str.GetChar() == amDesignator[0])
                {
                    result = TM.AM;
                    return true;
                }

                string pmDesignator = dtfi.PMDesignator;
                if (pmDesignator.Length > 0 && str.GetChar() == pmDesignator[0])
                {
                    result = TM.PM;
                    return true;
                }
            }
            return false;
        }

        /*=================================CheckNewValue==================================
        **Action: Check if currentValue is initialized.  If not, return the newValue.
        **        If yes, check if the current value is equal to newValue.  Return false
        **        if they are not equal.  This is used to check the case like "d" and "dd" are both
        **        used to format a string.
        **Returns: the correct value for currentValue.
        **Arguments:
        **Exceptions:
        ==============================================================================*/

        private static bool CheckNewValue(ref int currentValue, int newValue, char patternChar, ref DateTimeResult result)
        {
            if (currentValue == -1)
            {
                currentValue = newValue;
                return (true);
            }
            else
            {
                if (newValue != currentValue)
                {
                    result.SetFailure(ParseFailureKind.FormatWithParameter, nameof(SR.Format_RepeatDateTimePattern), patternChar);
                    return (false);
                }
            }
            return (true);
        }

        private static DateTime GetDateTimeNow(ref DateTimeResult result, ref DateTimeStyles styles)
        {
            if ((result.flags & ParseFlags.CaptureOffset) != 0)
            {
                if ((result.flags & ParseFlags.TimeZoneUsed) != 0)
                {
                    // use the supplied offset to calculate 'Now'
                    return new DateTime(DateTime.UtcNow.Ticks + result.timeZoneOffset.Ticks, DateTimeKind.Unspecified);
                }
                else if ((styles & DateTimeStyles.AssumeUniversal) != 0)
                {
                    // assume the offset is Utc
                    return DateTime.UtcNow;
                }
            }

            // assume the offset is Local
            return DateTime.Now;
        }

        private static bool CheckDefaultDateTime(ref DateTimeResult result, ref Calendar cal, DateTimeStyles styles)
        {
            if ((result.flags & ParseFlags.CaptureOffset) != 0)
            {
                // DateTimeOffset.Parse should allow dates without a year, but only if there is also no time zone marker;
                // e.g. "May 1 5pm" is OK, but "May 1 5pm -08:30" is not.  This is somewhat pragmatic, since we would
                // have to rearchitect parsing completely to allow this one case to correctly handle things like leap
                // years and leap months.  Is is an extremely corner case, and DateTime is basically incorrect in that
                // case today.
                //
                // values like "11:00Z" or "11:00 -3:00" are also acceptable
                //
                // if ((month or day is set) and (year is not set and time zone is set))
                //
                if (((result.Month != -1) || (result.Day != -1))
                    && ((result.Year == -1 || ((result.flags & ParseFlags.YearDefault) != 0)) && (result.flags & ParseFlags.TimeZoneUsed) != 0))
                {
                    result.SetFailure(ParseFailureKind.FormatWithOriginalDateTime, nameof(SR.Format_MissingIncompleteDate));
                    return false;
                }
            }


            if ((result.Year == -1) || (result.Month == -1) || (result.Day == -1))
            {
                /*
                The following table describes the behaviors of getting the default value
                when a certain year/month/day values are missing.

                An "X" means that the value exists.  And "--" means that value is missing.

                Year    Month   Day =>  ResultYear  ResultMonth     ResultDay       Note

                X       X       X       Parsed year Parsed month    Parsed day
                X       X       --      Parsed Year Parsed month    First day       If we have year and month, assume the first day of that month.
                X       --      X       Parsed year First month     Parsed day      If the month is missing, assume first month of that year.
                X       --      --      Parsed year First month     First day       If we have only the year, assume the first day of that year.

                --      X       X       CurrentYear Parsed month    Parsed day      If the year is missing, assume the current year.
                --      X       --      CurrentYear Parsed month    First day       If we have only a month value, assume the current year and current day.
                --      --      X       CurrentYear First month     Parsed day      If we have only a day value, assume current year and first month.
                --      --      --      CurrentYear Current month   Current day     So this means that if the date string only contains time, you will get current date.

                */

                DateTime now = GetDateTimeNow(ref result, ref styles);
                if (result.Month == -1 && result.Day == -1)
                {
                    if (result.Year == -1)
                    {
                        if ((styles & DateTimeStyles.NoCurrentDateDefault) != 0)
                        {
                            // If there is no year/month/day values, and NoCurrentDateDefault flag is used,
                            // set the year/month/day value to the beginning year/month/day of DateTime().
                            // Note we should be using Gregorian for the year/month/day.
                            cal = GregorianCalendar.GetDefaultInstance();
                            result.Year = result.Month = result.Day = 1;
                        }
                        else
                        {
                            // Year/Month/Day are all missing.
                            result.Year = cal.GetYear(now);
                            result.Month = cal.GetMonth(now);
                            result.Day = cal.GetDayOfMonth(now);
                        }
                    }
                    else
                    {
                        // Month/Day are both missing.
                        result.Month = 1;
                        result.Day = 1;
                    }
                }
                else
                {
                    if (result.Year == -1)
                    {
                        result.Year = cal.GetYear(now);
                    }
                    if (result.Month == -1)
                    {
                        result.Month = 1;
                    }
                    if (result.Day == -1)
                    {
                        result.Day = 1;
                    }
                }
            }
            // Set Hour/Minute/Second to zero if these value are not in str.
            if (result.Hour == -1) result.Hour = 0;
            if (result.Minute == -1) result.Minute = 0;
            if (result.Second == -1) result.Second = 0;
            if (result.era == -1) result.era = Calendar.CurrentEra;
            return true;
        }

        // Expand a pre-defined format string (like "D" for long date) to the real format that
        // we are going to use in the date time parsing.
        // This method also set the dtfi according/parseInfo to some special pre-defined
        // formats.
        //
        private static String ExpandPredefinedFormat(ReadOnlySpan<char> format, ref DateTimeFormatInfo dtfi, ref ParsingInfo parseInfo, ref DateTimeResult result)
        {
            //
            // Check the format to see if we need to override the dtfi to be InvariantInfo,
            // and see if we need to set up the userUniversalTime flag.
            //
            switch (format[0])
            {
                case 'o':
                case 'O':       // Round Trip Format
                    parseInfo.calendar = GregorianCalendar.GetDefaultInstance();
                    dtfi = DateTimeFormatInfo.InvariantInfo;
                    break;
                case 'r':
                case 'R':       // RFC 1123 Standard.  (in Universal time)
                    parseInfo.calendar = GregorianCalendar.GetDefaultInstance();
                    dtfi = DateTimeFormatInfo.InvariantInfo;

                    if ((result.flags & ParseFlags.CaptureOffset) != 0)
                    {
                        result.flags |= ParseFlags.Rfc1123Pattern;
                    }
                    break;
                case 's':       // Sortable format (in local time)
                    dtfi = DateTimeFormatInfo.InvariantInfo;
                    parseInfo.calendar = GregorianCalendar.GetDefaultInstance();
                    break;
                case 'u':       // Universal time format in sortable format.
                    parseInfo.calendar = GregorianCalendar.GetDefaultInstance();
                    dtfi = DateTimeFormatInfo.InvariantInfo;

                    if ((result.flags & ParseFlags.CaptureOffset) != 0)
                    {
                        result.flags |= ParseFlags.UtcSortPattern;
                    }
                    break;
                case 'U':       // Universal time format with culture-dependent format.
                    parseInfo.calendar = GregorianCalendar.GetDefaultInstance();
                    result.flags |= ParseFlags.TimeZoneUsed;
                    result.timeZoneOffset = new TimeSpan(0);
                    result.flags |= ParseFlags.TimeZoneUtc;
                    if (dtfi.Calendar.GetType() != typeof(GregorianCalendar))
                    {
                        dtfi = (DateTimeFormatInfo)dtfi.Clone();
                        dtfi.Calendar = GregorianCalendar.GetDefaultInstance();
                    }
                    break;
            }

            //
            // Expand the pre-defined format character to the real format from DateTimeFormatInfo.
            //
            return (DateTimeFormat.GetRealFormat(format, dtfi));
        }





        // Given a specified format character, parse and update the parsing result.
        //
        private static bool ParseByFormat(
            ref __DTString str,
            ref __DTString format,
            ref ParsingInfo parseInfo,
            DateTimeFormatInfo dtfi,
            ref DateTimeResult result)
        {
            int tokenLen = 0;
            int tempYear = 0, tempMonth = 0, tempDay = 0, tempDayOfWeek = 0, tempHour = 0, tempMinute = 0, tempSecond = 0;
            double tempFraction = 0;
            TM tempTimeMark = 0;

            char ch = format.GetChar();

            switch (ch)
            {
                case 'y':
                    tokenLen = format.GetRepeatCount();
                    bool parseResult;
                    if (dtfi.HasForceTwoDigitYears)
                    {
                        parseResult = ParseDigits(ref str, 1, 4, out tempYear);
                    }
                    else
                    {
                        if (tokenLen <= 2)
                        {
                            parseInfo.fUseTwoDigitYear = true;
                        }
                        parseResult = ParseDigits(ref str, tokenLen, out tempYear);
                    }
                    if (!parseResult && parseInfo.fCustomNumberParser)
                    {
                        parseResult = parseInfo.parseNumberDelegate(ref str, tokenLen, out tempYear);
                    }
                    if (!parseResult)
                    {
                        result.SetBadDateTimeFailure();
                        return (false);
                    }
                    if (!CheckNewValue(ref result.Year, tempYear, ch, ref result))
                    {
                        return (false);
                    }
                    break;
                case 'M':
                    tokenLen = format.GetRepeatCount();
                    if (tokenLen <= 2)
                    {
                        if (!ParseDigits(ref str, tokenLen, out tempMonth))
                        {
                            if (!parseInfo.fCustomNumberParser ||
                                !parseInfo.parseNumberDelegate(ref str, tokenLen, out tempMonth))
                            {
                                result.SetBadDateTimeFailure();
                                return (false);
                            }
                        }
                    }
                    else
                    {
                        if (tokenLen == 3)
                        {
                            if (!MatchAbbreviatedMonthName(ref str, dtfi, ref tempMonth))
                            {
                                result.SetBadDateTimeFailure();
                                return (false);
                            }
                        }
                        else
                        {
                            if (!MatchMonthName(ref str, dtfi, ref tempMonth))
                            {
                                result.SetBadDateTimeFailure();
                                return (false);
                            }
                        }
                        result.flags |= ParseFlags.ParsedMonthName;
                    }
                    if (!CheckNewValue(ref result.Month, tempMonth, ch, ref result))
                    {
                        return (false);
                    }
                    break;
                case 'd':
                    // Day & Day of week
                    tokenLen = format.GetRepeatCount();
                    if (tokenLen <= 2)
                    {
                        // "d" & "dd"

                        if (!ParseDigits(ref str, tokenLen, out tempDay))
                        {
                            if (!parseInfo.fCustomNumberParser ||
                                !parseInfo.parseNumberDelegate(ref str, tokenLen, out tempDay))
                            {
                                result.SetBadDateTimeFailure();
                                return (false);
                            }
                        }
                        if (!CheckNewValue(ref result.Day, tempDay, ch, ref result))
                        {
                            return (false);
                        }
                    }
                    else
                    {
                        if (tokenLen == 3)
                        {
                            // "ddd"
                            if (!MatchAbbreviatedDayName(ref str, dtfi, ref tempDayOfWeek))
                            {
                                result.SetBadDateTimeFailure();
                                return (false);
                            }
                        }
                        else
                        {
                            // "dddd*"
                            if (!MatchDayName(ref str, dtfi, ref tempDayOfWeek))
                            {
                                result.SetBadDateTimeFailure();
                                return (false);
                            }
                        }
                        if (!CheckNewValue(ref parseInfo.dayOfWeek, tempDayOfWeek, ch, ref result))
                        {
                            return (false);
                        }
                    }
                    break;
                case 'g':
                    tokenLen = format.GetRepeatCount();
                    // Put the era value in result.era.
                    if (!MatchEraName(ref str, dtfi, ref result.era))
                    {
                        result.SetBadDateTimeFailure();
                        return (false);
                    }
                    break;
                case 'h':
                    parseInfo.fUseHour12 = true;
                    tokenLen = format.GetRepeatCount();
                    if (!ParseDigits(ref str, (tokenLen < 2 ? 1 : 2), out tempHour))
                    {
                        result.SetBadDateTimeFailure();
                        return (false);
                    }
                    if (!CheckNewValue(ref result.Hour, tempHour, ch, ref result))
                    {
                        return (false);
                    }
                    break;
                case 'H':
                    tokenLen = format.GetRepeatCount();
                    if (!ParseDigits(ref str, (tokenLen < 2 ? 1 : 2), out tempHour))
                    {
                        result.SetBadDateTimeFailure();
                        return (false);
                    }
                    if (!CheckNewValue(ref result.Hour, tempHour, ch, ref result))
                    {
                        return (false);
                    }
                    break;
                case 'm':
                    tokenLen = format.GetRepeatCount();
                    if (!ParseDigits(ref str, (tokenLen < 2 ? 1 : 2), out tempMinute))
                    {
                        result.SetBadDateTimeFailure();
                        return (false);
                    }
                    if (!CheckNewValue(ref result.Minute, tempMinute, ch, ref result))
                    {
                        return (false);
                    }
                    break;
                case 's':
                    tokenLen = format.GetRepeatCount();
                    if (!ParseDigits(ref str, (tokenLen < 2 ? 1 : 2), out tempSecond))
                    {
                        result.SetBadDateTimeFailure();
                        return (false);
                    }
                    if (!CheckNewValue(ref result.Second, tempSecond, ch, ref result))
                    {
                        return (false);
                    }
                    break;
                case 'f':
                case 'F':
                    tokenLen = format.GetRepeatCount();
                    if (tokenLen <= DateTimeFormat.MaxSecondsFractionDigits)
                    {
                        if (!ParseFractionExact(ref str, tokenLen, ref tempFraction))
                        {
                            if (ch == 'f')
                            {
                                result.SetBadDateTimeFailure();
                                return (false);
                            }
                        }
                        if (result.fraction < 0)
                        {
                            result.fraction = tempFraction;
                        }
                        else
                        {
                            if (tempFraction != result.fraction)
                            {
                                result.SetFailure(ParseFailureKind.FormatWithParameter, nameof(SR.Format_RepeatDateTimePattern), ch);
                                return (false);
                            }
                        }
                    }
                    else
                    {
                        result.SetBadDateTimeFailure();
                        return (false);
                    }
                    break;
                case 't':
                    // AM/PM designator
                    tokenLen = format.GetRepeatCount();
                    if (tokenLen == 1)
                    {
                        if (!MatchAbbreviatedTimeMark(ref str, dtfi, ref tempTimeMark))
                        {
                            result.SetBadDateTimeFailure();
                            return (false);
                        }
                    }
                    else
                    {
                        if (!MatchTimeMark(ref str, dtfi, ref tempTimeMark))
                        {
                            result.SetBadDateTimeFailure();
                            return (false);
                        }
                    }

                    if (parseInfo.timeMark == TM.NotSet)
                    {
                        parseInfo.timeMark = tempTimeMark;
                    }
                    else
                    {
                        if (parseInfo.timeMark != tempTimeMark)
                        {
                            result.SetFailure(ParseFailureKind.FormatWithParameter, nameof(SR.Format_RepeatDateTimePattern), ch);
                            return (false);
                        }
                    }
                    break;
                case 'z':
                    // timezone offset
                    tokenLen = format.GetRepeatCount();
                    {
                        TimeSpan tempTimeZoneOffset = new TimeSpan(0);
                        if (!ParseTimeZoneOffset(ref str, tokenLen, ref tempTimeZoneOffset))
                        {
                            result.SetBadDateTimeFailure();
                            return (false);
                        }
                        if ((result.flags & ParseFlags.TimeZoneUsed) != 0 && tempTimeZoneOffset != result.timeZoneOffset)
                        {
                            result.SetFailure(ParseFailureKind.FormatWithParameter, nameof(SR.Format_RepeatDateTimePattern), 'z');
                            return (false);
                        }
                        result.timeZoneOffset = tempTimeZoneOffset;
                        result.flags |= ParseFlags.TimeZoneUsed;
                    }
                    break;
                case 'Z':
                    if ((result.flags & ParseFlags.TimeZoneUsed) != 0 && result.timeZoneOffset != TimeSpan.Zero)
                    {
                        result.SetFailure(ParseFailureKind.FormatWithParameter, nameof(SR.Format_RepeatDateTimePattern), 'Z');
                        return (false);
                    }

                    result.flags |= ParseFlags.TimeZoneUsed;
                    result.timeZoneOffset = new TimeSpan(0);
                    result.flags |= ParseFlags.TimeZoneUtc;

                    // The updating of the indexes is to reflect that ParseExact MatchXXX methods assume that
                    // they need to increment the index and Parse GetXXX do not. Since we are calling a Parse
                    // method from inside ParseExact we need to adjust this. Long term, we should try to
                    // eliminate this discrepancy.
                    str.Index++;
                    if (!GetTimeZoneName(ref str))
                    {
                        result.SetBadDateTimeFailure();
                        return false;
                    }
                    str.Index--;
                    break;
                case 'K':
                    // This should parse either as a blank, the 'Z' character or a local offset like "-07:00"
                    if (str.Match('Z'))
                    {
                        if ((result.flags & ParseFlags.TimeZoneUsed) != 0 && result.timeZoneOffset != TimeSpan.Zero)
                        {
                            result.SetFailure(ParseFailureKind.FormatWithParameter, nameof(SR.Format_RepeatDateTimePattern), 'K');
                            return (false);
                        }

                        result.flags |= ParseFlags.TimeZoneUsed;
                        result.timeZoneOffset = new TimeSpan(0);
                        result.flags |= ParseFlags.TimeZoneUtc;
                    }
                    else if (str.Match('+') || str.Match('-'))
                    {
                        str.Index--; // Put the character back for the parser
                        TimeSpan tempTimeZoneOffset = new TimeSpan(0);
                        if (!ParseTimeZoneOffset(ref str, 3, ref tempTimeZoneOffset))
                        {
                            result.SetBadDateTimeFailure();
                            return (false);
                        }
                        if ((result.flags & ParseFlags.TimeZoneUsed) != 0 && tempTimeZoneOffset != result.timeZoneOffset)
                        {
                            result.SetFailure(ParseFailureKind.FormatWithParameter, nameof(SR.Format_RepeatDateTimePattern), 'K');
                            return (false);
                        }
                        result.timeZoneOffset = tempTimeZoneOffset;
                        result.flags |= ParseFlags.TimeZoneUsed;
                    }
                    // Otherwise it is unspecified and we consume no characters
                    break;
                case ':':
                    // We match the separator in time pattern with the character in the time string if both equal to ':' or the date separator is matching the characters in the date string
                    // We have to exclude the case when the time separator is more than one character and starts with ':' something like "::" for instance.
                    if (((dtfi.TimeSeparator.Length > 1 && dtfi.TimeSeparator[0] == ':') || !str.Match(':')) &&
                        !str.Match(dtfi.TimeSeparator))
                    {
                        // A time separator is expected.
                        result.SetBadDateTimeFailure();
                        return false;
                    }
                    break;
                case '/':
                    // We match the separator in date pattern with the character in the date string if both equal to '/' or the date separator is matching the characters in the date string
                    // We have to exclude the case when the date separator is more than one character and starts with '/' something like "//" for instance.
                    if (((dtfi.DateSeparator.Length > 1 && dtfi.DateSeparator[0] == '/') || !str.Match('/')) &&
                        !str.Match(dtfi.DateSeparator))
                    {
                        // A date separator is expected.
                        result.SetBadDateTimeFailure();
                        return false;
                    }
                    break;
                case '\"':
                case '\'':
                    StringBuilder enquotedString = StringBuilderCache.Acquire();
                    // Use ParseQuoteString so that we can handle escape characters within the quoted string.
                    if (!TryParseQuoteString(format.Value, format.Index, enquotedString, out tokenLen))
                    {
                        result.SetFailure(ParseFailureKind.FormatWithParameter, nameof(SR.Format_BadQuote), ch);
                        StringBuilderCache.Release(enquotedString);
                        return (false);
                    }
                    format.Index += tokenLen - 1;

                    // Some cultures uses space in the quoted string.  E.g. Spanish has long date format as:
                    // "dddd, dd' de 'MMMM' de 'yyyy".  When inner spaces flag is set, we should skip whitespaces if there is space
                    // in the quoted string.
                    String quotedStr = StringBuilderCache.GetStringAndRelease(enquotedString);

                    for (int i = 0; i < quotedStr.Length; i++)
                    {
                        if (quotedStr[i] == ' ' && parseInfo.fAllowInnerWhite)
                        {
                            str.SkipWhiteSpaces();
                        }
                        else if (!str.Match(quotedStr[i]))
                        {
                            // Can not find the matching quoted string.
                            result.SetBadDateTimeFailure();
                            return false;
                        }
                    }

                    // The "r" and "u" formats incorrectly quoted 'GMT' and 'Z', respectively.  We cannot
                    // correct this mistake for DateTime.ParseExact for compatibility reasons, but we can
                    // fix it for DateTimeOffset.ParseExact as DateTimeOffset has not been publically released
                    // with this issue.
                    if ((result.flags & ParseFlags.CaptureOffset) != 0)
                    {
                        if ((result.flags & ParseFlags.Rfc1123Pattern) != 0 && quotedStr == GMTName)
                        {
                            result.flags |= ParseFlags.TimeZoneUsed;
                            result.timeZoneOffset = TimeSpan.Zero;
                        }
                        else if ((result.flags & ParseFlags.UtcSortPattern) != 0 && quotedStr == ZuluName)
                        {
                            result.flags |= ParseFlags.TimeZoneUsed;
                            result.timeZoneOffset = TimeSpan.Zero;
                        }
                    }

                    break;
                case '%':
                    // Skip this so we can get to the next pattern character.
                    // Used in case like "%d", "%y"

                    // Make sure the next character is not a '%' again.
                    if (format.Index >= format.Value.Length - 1 || format.Value[format.Index + 1] == '%')
                    {
                        result.SetBadFormatSpecifierFailure(format.Value);
                        return false;
                    }
                    break;
                case '\\':
                    // Escape character. For example, "\d".
                    // Get the next character in format, and see if we can
                    // find a match in str.
                    if (format.GetNext())
                    {
                        if (!str.Match(format.GetChar()))
                        {
                            // Can not find a match for the escaped character.
                            result.SetBadDateTimeFailure();
                            return false;
                        }
                    }
                    else
                    {
                        result.SetBadFormatSpecifierFailure(format.Value);
                        return false;
                    }
                    break;
                case '.':
                    if (!str.Match(ch))
                    {
                        if (format.GetNext())
                        {
                            // If we encounter the pattern ".F", and the dot is not present, it is an optional
                            // second fraction and we can skip this format.
                            if (format.Match('F'))
                            {
                                format.GetRepeatCount();
                                break;
                            }
                        }
                        result.SetBadDateTimeFailure();
                        return false;
                    }
                    break;
                default:
                    if (ch == ' ')
                    {
                        if (parseInfo.fAllowInnerWhite)
                        {
                            // Skip whitespaces if AllowInnerWhite.
                            // Do nothing here.
                        }
                        else
                        {
                            if (!str.Match(ch))
                            {
                                // If the space does not match, and trailing space is allowed, we do
                                // one more step to see if the next format character can lead to
                                // successful parsing.
                                // This is used to deal with special case that a empty string can match
                                // a specific pattern.
                                // The example here is af-ZA, which has a time format like "hh:mm:ss tt".  However,
                                // its AM symbol is "" (empty string).  If fAllowTrailingWhite is used, and time is in
                                // the AM, we will trim the whitespaces at the end, which will lead to a failure
                                // when we are trying to match the space before "tt".
                                if (parseInfo.fAllowTrailingWhite)
                                {
                                    if (format.GetNext())
                                    {
                                        if (ParseByFormat(ref str, ref format, ref parseInfo, dtfi, ref result))
                                        {
                                            return (true);
                                        }
                                    }
                                }
                                result.SetBadDateTimeFailure();
                                return false;
                            }
                            // Found a macth.
                        }
                    }
                    else
                    {
                        if (format.MatchSpecifiedWord(GMTName))
                        {
                            format.Index += (GMTName.Length - 1);
                            // Found GMT string in format.  This means the DateTime string
                            // is in GMT timezone.
                            result.flags |= ParseFlags.TimeZoneUsed;
                            result.timeZoneOffset = TimeSpan.Zero;
                            if (!str.Match(GMTName))
                            {
                                result.SetBadDateTimeFailure();
                                return false;
                            }
                        }
                        else if (!str.Match(ch))
                        {
                            // ch is expected.
                            result.SetBadDateTimeFailure();
                            return false;
                        }
                    }
                    break;
            } // switch
            return (true);
        }

        //
        // The pos should point to a quote character. This method will
        // get the string enclosed by the quote character.
        //
        internal static bool TryParseQuoteString(ReadOnlySpan<char> format, int pos, StringBuilder result, out int returnValue)
        {
            //
            // NOTE : pos will be the index of the quote character in the 'format' string.
            //
            returnValue = 0;
            int formatLen = format.Length;
            int beginPos = pos;
            char quoteChar = format[pos++]; // Get the character used to quote the following string.

            bool foundQuote = false;
            while (pos < formatLen)
            {
                char ch = format[pos++];
                if (ch == quoteChar)
                {
                    foundQuote = true;
                    break;
                }
                else if (ch == '\\')
                {
                    // The following are used to support escaped character.
                    // Escaped character is also supported in the quoted string.
                    // Therefore, someone can use a format like "'minute:' mm\"" to display:
                    //  minute: 45"
                    // because the second double quote is escaped.
                    if (pos < formatLen)
                    {
                        result.Append(format[pos++]);
                    }
                    else
                    {
                        //
                        // This means that '\' is at the end of the formatting string.
                        //
                        return false;
                    }
                }
                else
                {
                    result.Append(ch);
                }
            }

            if (!foundQuote)
            {
                // Here we can't find the matching quote.
                return false;
            }

            //
            // Return the character count including the begin/end quote characters and enclosed string.
            //
            returnValue = (pos - beginPos);
            return true;
        }




        /*=================================DoStrictParse==================================
        **Action: Do DateTime parsing using the format in formatParam.
        **Returns: The parsed DateTime.
        **Arguments:
        **Exceptions:
        **
        **Notes:
        **  When the following general formats are used, InvariantInfo is used in dtfi:
        **      'r', 'R', 's'.
        **  When the following general formats are used, the time is assumed to be in Universal time.
        **
        **Limitations:
        **  Only GregarianCalendar is supported for now.
        **  Only support GMT timezone.
        ==============================================================================*/

        private static bool DoStrictParse(
            ReadOnlySpan<char> s,
            ReadOnlySpan<char> formatParam,
            DateTimeStyles styles,
            DateTimeFormatInfo dtfi,
            ref DateTimeResult result)
        {
            ParsingInfo parseInfo = new ParsingInfo();
            parseInfo.Init();

            parseInfo.calendar = dtfi.Calendar;
            parseInfo.fAllowInnerWhite = ((styles & DateTimeStyles.AllowInnerWhite) != 0);
            parseInfo.fAllowTrailingWhite = ((styles & DateTimeStyles.AllowTrailingWhite) != 0);

            if (formatParam.Length == 1)
            {
                if (((result.flags & ParseFlags.CaptureOffset) != 0) && formatParam[0] == 'U')
                {
                    // The 'U' format is not allowed for DateTimeOffset
                    result.SetBadFormatSpecifierFailure(formatParam);
                    return false;
                }
                formatParam = ExpandPredefinedFormat(formatParam, ref dtfi, ref parseInfo, ref result);
            }

            bool bTimeOnly = false;
            result.calendar = parseInfo.calendar;

            if (parseInfo.calendar.ID == CalendarId.HEBREW)
            {
                parseInfo.parseNumberDelegate = m_hebrewNumberParser;
                parseInfo.fCustomNumberParser = true;
            }

            // Reset these values to negative one so that we could throw exception
            // if we have parsed every item twice.
            result.Hour = result.Minute = result.Second = -1;

            __DTString format = new __DTString(formatParam, dtfi, false);
            __DTString str = new __DTString(s, dtfi, false);

            if (parseInfo.fAllowTrailingWhite)
            {
                // Trim trailing spaces if AllowTrailingWhite.
                format.TrimTail();
                format.RemoveTrailingInQuoteSpaces();
                str.TrimTail();
            }

            if ((styles & DateTimeStyles.AllowLeadingWhite) != 0)
            {
                format.SkipWhiteSpaces();
                format.RemoveLeadingInQuoteSpaces();
                str.SkipWhiteSpaces();
            }

            //
            // Scan every character in format and match the pattern in str.
            //
            while (format.GetNext())
            {
                // We trim inner spaces here, so that we will not eat trailing spaces when
                // AllowTrailingWhite is not used.
                if (parseInfo.fAllowInnerWhite)
                {
                    str.SkipWhiteSpaces();
                }
                if (!ParseByFormat(ref str, ref format, ref parseInfo, dtfi, ref result))
                {
                    return (false);
                }
            }

            if (str.Index < str.Value.Length - 1)
            {
                // There are still remaining character in str.
                result.SetBadDateTimeFailure();
                return false;
            }

            if (parseInfo.fUseTwoDigitYear && ((dtfi.FormatFlags & DateTimeFormatFlags.UseHebrewRule) == 0))
            {
                // A two digit year value is expected. Check if the parsed year value is valid.
                if (result.Year >= 100)
                {
                    result.SetBadDateTimeFailure();
                    return false;
                }
                try
                {
                    result.Year = parseInfo.calendar.ToFourDigitYear(result.Year);
                }
                catch (ArgumentOutOfRangeException)
                {
                    result.SetBadDateTimeFailure();
                    return false;
                }
            }

            if (parseInfo.fUseHour12)
            {
                if (parseInfo.timeMark == TM.NotSet)
                {
                    // hh is used, but no AM/PM designator is specified.
                    // Assume the time is AM.
                    // Don't throw exceptions in here becasue it is very confusing for the caller.
                    // I always got confused myself when I use "hh:mm:ss" to parse a time string,
                    // and ParseExact() throws on me (because I didn't use the 24-hour clock 'HH').
                    parseInfo.timeMark = TM.AM;
                }
                if (result.Hour > 12)
                {
                    // AM/PM is used, but the value for HH is too big.
                    result.SetBadDateTimeFailure();
                    return false;
                }
                if (parseInfo.timeMark == TM.AM)
                {
                    if (result.Hour == 12)
                    {
                        result.Hour = 0;
                    }
                }
                else
                {
                    result.Hour = (result.Hour == 12) ? 12 : result.Hour + 12;
                }
            }
            else
            {
                // Military (24-hour time) mode
                //
                // AM cannot be set with a 24-hour time like 17:15.
                // PM cannot be set with a 24-hour time like 03:15.
                if ((parseInfo.timeMark == TM.AM && result.Hour >= 12)
                    || (parseInfo.timeMark == TM.PM && result.Hour < 12))
                {
                    result.SetBadDateTimeFailure();
                    return false;
                }
            }


            // Check if the parased string only contains hour/minute/second values.
            bTimeOnly = (result.Year == -1 && result.Month == -1 && result.Day == -1);
            if (!CheckDefaultDateTime(ref result, ref parseInfo.calendar, styles))
            {
                return false;
            }

            if (!bTimeOnly && dtfi.HasYearMonthAdjustment)
            {
                if (!dtfi.YearMonthAdjustment(ref result.Year, ref result.Month, ((result.flags & ParseFlags.ParsedMonthName) != 0)))
                {
                    result.SetFailure(ParseFailureKind.FormatBadDateTimeCalendar, nameof(SR.Format_BadDateTimeCalendar));
                    return false;
                }
            }
            if (!parseInfo.calendar.TryToDateTime(result.Year, result.Month, result.Day,
                    result.Hour, result.Minute, result.Second, 0, result.era, out result.parsedDate))
            {
                result.SetFailure(ParseFailureKind.FormatBadDateTimeCalendar, nameof(SR.Format_BadDateTimeCalendar));
                return false;
            }
            if (result.fraction > 0)
            {
                result.parsedDate = result.parsedDate.AddTicks((long)Math.Round(result.fraction * Calendar.TicksPerSecond));
            }

            //
            // We have to check day of week before we adjust to the time zone.
            // It is because the value of day of week may change after adjusting
            // to the time zone.
            //
            if (parseInfo.dayOfWeek != -1)
            {
                //
                // Check if day of week is correct.
                //
                if (parseInfo.dayOfWeek != (int)parseInfo.calendar.GetDayOfWeek(result.parsedDate))
                {
                    result.SetFailure(ParseFailureKind.FormatWithOriginalDateTime, nameof(SR.Format_BadDayOfWeek));
                    return false;
                }
            }


            if (!DetermineTimeZoneAdjustments(ref str, ref result, styles, bTimeOnly))
            {
                return false;
            }
            return true;
        }

        private static Exception GetDateTimeParseException(ref DateTimeResult result)
        {
            switch (result.failure)
            {
                case ParseFailureKind.ArgumentNull:
                    return new ArgumentNullException(result.failureArgumentName, SR.GetResourceString(result.failureMessageID));
                case ParseFailureKind.Format:
                    return new FormatException(SR.GetResourceString(result.failureMessageID));
                case ParseFailureKind.FormatWithParameter:
                    return new FormatException(SR.Format(SR.GetResourceString(result.failureMessageID), result.failureMessageFormatArgument));
                case ParseFailureKind.FormatBadDateTimeCalendar:
                    return new FormatException(SR.Format(SR.GetResourceString(result.failureMessageID), new string(result.originalDateTimeString), result.calendar));
                case ParseFailureKind.FormatWithOriginalDateTime:
                    return new FormatException(SR.Format(SR.GetResourceString(result.failureMessageID), new string(result.originalDateTimeString)));
                case ParseFailureKind.FormatWithFormatSpecifier:
                    return new FormatException(SR.Format(SR.GetResourceString(result.failureMessageID), new string(result.failedFormatSpecifier)));
                case ParseFailureKind.FormatWithOriginalDateTimeAndParameter:
                    return new FormatException(SR.Format(SR.GetResourceString(result.failureMessageID), new string(result.originalDateTimeString), result.failureMessageFormatArgument));
                default:
                    Debug.Fail("Unknown DateTimeParseFailure: " + result.failure.ToString());
                    return null;
            }
        }

        [Conditional("_LOGGING")]
        private static void LexTraceExit(string message, DS dps)
        {
#if _LOGGING
            if (!_tracingEnabled)
                return;
            Trace($"Lex return {message}, DS.{dps}");
#endif // _LOGGING
        }
        [Conditional("_LOGGING")]
        private static void PTSTraceExit(DS dps, bool passed)
        {
#if _LOGGING
            if (!_tracingEnabled)
                return;
            Trace($"ProcessTerminalState {(passed ? "passed" : "failed")} @ DS.{dps}");
#endif // _LOGGING
        }
        [Conditional("_LOGGING")]
        private static void TPTraceExit(string message, DS dps)
        {
#if _LOGGING
            if (!_tracingEnabled)
                return;
            Trace($"TryParse return {message}, DS.{dps}");
#endif // _LOGGING
        }
        [Conditional("_LOGGING")]
        private static void DTFITrace(DateTimeFormatInfo dtfi)
        {
#if _LOGGING
            if (!_tracingEnabled)
                return;

            Trace("DateTimeFormatInfo Properties");
#if !FEATURE_COREFX_GLOBALIZATION
            Trace($" NativeCalendarName {Hex(dtfi.NativeCalendarName)}");
#endif
            Trace($"       AMDesignator {Hex(dtfi.AMDesignator)}");
            Trace($"       PMDesignator {Hex(dtfi.PMDesignator)}");
            Trace($"      TimeSeparator {Hex(dtfi.TimeSeparator)}");
            Trace($"      AbbrvDayNames {Hex(dtfi.AbbreviatedDayNames)}");
            Trace($"   ShortestDayNames {Hex(dtfi.ShortestDayNames)}");
            Trace($"           DayNames {Hex(dtfi.DayNames)}");
            Trace($"    AbbrvMonthNames {Hex(dtfi.AbbreviatedMonthNames)}");
            Trace($"         MonthNames {Hex(dtfi.MonthNames)}");
            Trace($" AbbrvMonthGenNames {Hex(dtfi.AbbreviatedMonthGenitiveNames)}");
            Trace($"      MonthGenNames {Hex(dtfi.MonthGenitiveNames)}");
#endif // _LOGGING
        }
#if _LOGGING
        // return a string in the form: "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"
        private static string Hex(string[] strs)
        {
            if (strs == null || strs.Length == 0)
                return String.Empty;
            if (strs.Length == 1)
                return Hex(strs[0]);

            int curLineLength = 0;
            int maxLineLength = 55;
            int newLinePadding = 20;


            //invariant: strs.Length >= 2
            StringBuilder buffer = new StringBuilder();
            buffer.Append(Hex(strs[0]));
            curLineLength = buffer.Length;
            String s;

            for (int i = 1; i < strs.Length - 1; i++)
            {
                s = Hex(strs[i]);

                if (s.Length > maxLineLength || (curLineLength + s.Length + 2) > maxLineLength)
                {
                    buffer.Append(',');
                    buffer.Append(Environment.NewLine);
                    buffer.Append(' ', newLinePadding);
                    curLineLength = 0;
                }
                else
                {
                    buffer.Append(", ");
                    curLineLength += 2;
                }
                buffer.Append(s);
                curLineLength += s.Length;
            }

            buffer.Append(',');
            s = Hex(strs[strs.Length - 1]);
            if (s.Length > maxLineLength || (curLineLength + s.Length + 6) > maxLineLength)
            {
                buffer.Append(Environment.NewLine);
                buffer.Append(' ', newLinePadding);
            }
            else
            {
                buffer.Append(' ');
            }
            buffer.Append(s);
            return buffer.ToString();
        }
        // return a string in the form: "Sun"
        private static string Hex(string str) => Hex((ReadOnlySpan<char>)str);
        private static string Hex(ReadOnlySpan<char> str)
        {
            StringBuilder buffer = new StringBuilder();
            buffer.Append("\"");
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] <= '\x007f')
                    buffer.Append(str[i]);
                else
                    buffer.Append("\\u" + ((int)str[i]).ToString("x4", CultureInfo.InvariantCulture));
            }
            buffer.Append("\"");
            return buffer.ToString();
        }
        // return an unicode escaped string form of char c
        private static String Hex(char c)
        {
            if (c <= '\x007f')
                return c.ToString(CultureInfo.InvariantCulture);
            else
                return "\\u" + ((int)c).ToString("x4", CultureInfo.InvariantCulture);
        }

        private static void Trace(string s)
        {
            // Internal.Console.WriteLine(s);
        }

        private static bool _tracingEnabled = false;
#endif // _LOGGING
    }


    //
    // This is a string parsing helper which wraps a String object.
    // It has a Index property which tracks
    // the current parsing pointer of the string.
    //
    internal ref struct __DTString
    {
        //
        // Value propery: stores the real string to be parsed.
        //
        internal ReadOnlySpan<char> Value;

        //
        // Index property: points to the character that we are currently parsing.
        //
        internal int Index;

        // The length of Value string.
        internal int Length => Value.Length;

        // The current chracter to be looked at.
        internal char m_current;

        private CompareInfo m_info;
        // Flag to indicate if we encouter an digit, we should check for token or not.
        // In some cultures, such as mn-MN, it uses "\x0031\x00a0\x0434\x04af\x0433\x044d\x044d\x0440\x00a0\x0441\x0430\x0440" in month names.
        private bool m_checkDigitToken;

        internal __DTString(ReadOnlySpan<char> str, DateTimeFormatInfo dtfi, bool checkDigitToken) : this(str, dtfi)
        {
            m_checkDigitToken = checkDigitToken;
        }

        internal __DTString(ReadOnlySpan<char> str, DateTimeFormatInfo dtfi)
        {
            Index = -1;
            Value = str;

            m_current = '\0';
            if (dtfi != null)
            {
                m_info = dtfi.CompareInfo;
                m_checkDigitToken = ((dtfi.FormatFlags & DateTimeFormatFlags.UseDigitPrefixInTokens) != 0);
            }
            else
            {
                m_info = CultureInfo.CurrentCulture.CompareInfo;
                m_checkDigitToken = false;
            }
        }

        internal CompareInfo CompareInfo
        {
            get { return m_info; }
        }

        //
        // Advance the Index.
        // Return true if Index is NOT at the end of the string.
        //
        // Typical usage:
        // while (str.GetNext())
        // {
        //     char ch = str.GetChar()
        // }
        internal bool GetNext()
        {
            Index++;
            if (Index < Length)
            {
                m_current = Value[Index];
                return (true);
            }
            return (false);
        }

        internal bool AtEnd()
        {
            return Index < Length ? false : true;
        }

        internal bool Advance(int count)
        {
            Debug.Assert(Index + count <= Length, "__DTString::Advance: Index + count <= len");
            Index += count;
            if (Index < Length)
            {
                m_current = Value[Index];
                return (true);
            }
            return (false);
        }


        // Used by DateTime.Parse() to get the next token.
        internal void GetRegularToken(out TokenType tokenType, out int tokenValue, DateTimeFormatInfo dtfi)
        {
            tokenValue = 0;
            if (Index >= Length)
            {
                tokenType = TokenType.EndOfString;
                return;
            }

            tokenType = TokenType.UnknownToken;

        Start:
            if (DateTimeParse.IsDigit(m_current))
            {
                // This is a digit.
                tokenValue = m_current - '0';
                int value;
                int start = Index;

                //
                // Collect other digits.
                //
                while (++Index < Length)
                {
                    m_current = Value[Index];
                    value = m_current - '0';
                    if (value >= 0 && value <= 9)
                    {
                        tokenValue = tokenValue * 10 + value;
                    }
                    else
                    {
                        break;
                    }
                }
                if (Index - start > DateTimeParse.MaxDateTimeNumberDigits)
                {
                    tokenType = TokenType.NumberToken;
                    tokenValue = -1;
                }
                else if (Index - start < 3)
                {
                    tokenType = TokenType.NumberToken;
                }
                else
                {
                    // If there are more than 3 digits, assume that it's a year value.
                    tokenType = TokenType.YearNumberToken;
                }
                if (m_checkDigitToken)
                {
                    int save = Index;
                    char saveCh = m_current;
                    // Re-scan using the staring Index to see if this is a token.
                    Index = start;  // To include the first digit.
                    m_current = Value[Index];
                    TokenType tempType;
                    int tempValue;
                    // This DTFI has tokens starting with digits.
                    // E.g. mn-MN has month name like "\x0031\x00a0\x0434\x04af\x0433\x044d\x044d\x0440\x00a0\x0441\x0430\x0440"
                    if (dtfi.Tokenize(TokenType.RegularTokenMask, out tempType, out tempValue, ref this))
                    {
                        tokenType = tempType;
                        tokenValue = tempValue;
                        // This is a token, so the Index has been advanced propertly in DTFI.Tokenizer().
                    }
                    else
                    {
                        // Use the number token value.
                        // Restore the index.
                        Index = save;
                        m_current = saveCh;
                    }
                }
            }
            else if (Char.IsWhiteSpace(m_current))
            {
                // Just skip to the next character.
                while (++Index < Length)
                {
                    m_current = Value[Index];
                    if (!(Char.IsWhiteSpace(m_current)))
                    {
                        goto Start;
                    }
                }
                // We have reached the end of string.
                tokenType = TokenType.EndOfString;
            }
            else
            {
                dtfi.Tokenize(TokenType.RegularTokenMask, out tokenType, out tokenValue, ref this);
            }
        }

        internal TokenType GetSeparatorToken(DateTimeFormatInfo dtfi, out int indexBeforeSeparator, out char charBeforeSeparator)
        {
            indexBeforeSeparator = Index;
            charBeforeSeparator = m_current;
            TokenType tokenType;
            if (!SkipWhiteSpaceCurrent())
            {
                // Reach the end of the string.
                return (TokenType.SEP_End);
            }
            if (!DateTimeParse.IsDigit(m_current))
            {
                // Not a digit.  Tokenize it.
                int tokenValue;
                bool found = dtfi.Tokenize(TokenType.SeparatorTokenMask, out tokenType, out tokenValue, ref this);
                if (!found)
                {
                    tokenType = TokenType.SEP_Space;
                }
            }
            else
            {
                // Do nothing here.  If we see a number, it will not be a separator. There is no need wasting time trying to find the
                // separator token.
                tokenType = TokenType.SEP_Space;
            }
            return (tokenType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool MatchSpecifiedWord(String target) =>
            Index + target.Length <= Length &&
            m_info.Compare(Value.Slice(Index, target.Length), target, CompareOptions.IgnoreCase) == 0;

        private static readonly Char[] WhiteSpaceChecks = new Char[] { ' ', '\u00A0' };

        internal bool MatchSpecifiedWords(String target, bool checkWordBoundary, ref int matchLength)
        {
            int valueRemaining = Value.Length - Index;
            matchLength = target.Length;

            if (matchLength > valueRemaining || m_info.Compare(Value.Slice(Index, matchLength), target, CompareOptions.IgnoreCase) != 0)
            {
                // Check word by word
                int targetPosition = 0;                 // Where we are in the target string
                int thisPosition = Index;         // Where we are in this string
                int wsIndex = target.IndexOfAny(WhiteSpaceChecks, targetPosition);
                if (wsIndex == -1)
                {
                    return false;
                }
                do
                {
                    int segmentLength = wsIndex - targetPosition;
                    if (thisPosition >= Value.Length - segmentLength)
                    { // Subtraction to prevent overflow.
                        return false;
                    }
                    if (segmentLength == 0)
                    {
                        // If segmentLength == 0, it means that we have leading space in the target string.
                        // In that case, skip the leading spaces in the target and this string.
                        matchLength--;
                    }
                    else
                    {
                        // Make sure we also have whitespace in the input string
                        if (!Char.IsWhiteSpace(Value[thisPosition + segmentLength]))
                        {
                            return false;
                        }
                        if (m_info.CompareOptionIgnoreCase(Value.Slice(thisPosition, segmentLength), target.AsSpan(targetPosition, segmentLength)) != 0)
                        {
                            return false;
                        }
                        // Advance the input string
                        thisPosition = thisPosition + segmentLength + 1;
                    }
                    // Advance our target string
                    targetPosition = wsIndex + 1;


                    // Skip past multiple whitespace
                    while (thisPosition < Value.Length && Char.IsWhiteSpace(Value[thisPosition]))
                    {
                        thisPosition++;
                        matchLength++;
                    }
                } while ((wsIndex = target.IndexOfAny(WhiteSpaceChecks, targetPosition)) >= 0);
                // now check the last segment;
                if (targetPosition < target.Length)
                {
                    int segmentLength = target.Length - targetPosition;
                    if (thisPosition > Value.Length - segmentLength)
                    {
                        return false;
                    }
                    if (m_info.CompareOptionIgnoreCase(Value.Slice(thisPosition, segmentLength), target.AsSpan(targetPosition, segmentLength)) != 0)
                    {
                        return false;
                    }
                }
            }

            if (checkWordBoundary)
            {
                int nextCharIndex = Index + matchLength;
                if (nextCharIndex < Value.Length)
                {
                    if (Char.IsLetter(Value[nextCharIndex]))
                    {
                        return (false);
                    }
                }
            }
            return (true);
        }

        //
        // Check to see if the string starting from Index is a prefix of
        // str.
        // If a match is found, true value is returned and Index is updated to the next character to be parsed.
        // Otherwise, Index is unchanged.
        //
        internal bool Match(String str)
        {
            if (++Index >= Length)
            {
                return (false);
            }

            if (str.Length > (Value.Length - Index))
            {
                return false;
            }

            if (m_info.Compare(Value.Slice(Index, str.Length), str, CompareOptions.Ordinal) == 0)
            {
                // Update the Index to the end of the matching string.
                // So the following GetNext()/Match() opeartion will get
                // the next character to be parsed.
                Index += (str.Length - 1);
                return (true);
            }
            return (false);
        }

        internal bool Match(char ch)
        {
            if (++Index >= Length)
            {
                return (false);
            }
            if (Value[Index] == ch)
            {
                m_current = ch;
                return (true);
            }
            Index--;
            return (false);
        }

        //
        //  Actions: From the current position, try matching the longest word in the specified string array.
        //      E.g. words[] = {"AB", "ABC", "ABCD"}, if the current position points to a substring like "ABC DEF",
        //          MatchLongestWords(words, ref MaxMatchStrLen) will return 1 (the index), and maxMatchLen will be 3.
        //  Returns:
        //      The index that contains the longest word to match
        //  Arguments:
        //      words   The string array that contains words to search.
        //      maxMatchStrLen  [in/out] the initailized maximum length.  This parameter can be used to
        //          find the longest match in two string arrays.
        //
        internal int MatchLongestWords(String[] words, ref int maxMatchStrLen)
        {
            int result = -1;
            for (int i = 0; i < words.Length; i++)
            {
                String word = words[i];
                int matchLength = word.Length;
                if (MatchSpecifiedWords(word, false, ref matchLength))
                {
                    if (matchLength > maxMatchStrLen)
                    {
                        maxMatchStrLen = matchLength;
                        result = i;
                    }
                }
            }

            return (result);
        }

        //
        // Get the number of repeat character after the current character.
        // For a string "hh:mm:ss" at Index of 3. GetRepeatCount() = 2, and Index
        // will point to the second ':'.
        //
        internal int GetRepeatCount()
        {
            char repeatChar = Value[Index];
            int pos = Index + 1;
            while ((pos < Length) && (Value[pos] == repeatChar))
            {
                pos++;
            }
            int repeatCount = (pos - Index);
            // Update the Index to the end of the repeated characters.
            // So the following GetNext() opeartion will get
            // the next character to be parsed.
            Index = pos - 1;
            return (repeatCount);
        }

        // Return false when end of string is encountered or a non-digit character is found.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool GetNextDigit() =>
            ++Index < Length &&
            DateTimeParse.IsDigit(Value[Index]);

        //
        // Get the current character.
        //
        internal char GetChar()
        {
            Debug.Assert(Index >= 0 && Index < Length, "Index >= 0 && Index < len");
            return (Value[Index]);
        }

        //
        // Convert the current character to a digit, and return it.
        //
        internal int GetDigit()
        {
            Debug.Assert(Index >= 0 && Index < Length, "Index >= 0 && Index < len");
            Debug.Assert(DateTimeParse.IsDigit(Value[Index]), "IsDigit(Value[Index])");
            return (Value[Index] - '0');
        }

        //
        // Eat White Space ahead of the current position
        //
        // Return false if end of string is encountered.
        //
        internal void SkipWhiteSpaces()
        {
            // Look ahead to see if the next character
            // is a whitespace.
            while (Index + 1 < Length)
            {
                char ch = Value[Index + 1];
                if (!Char.IsWhiteSpace(ch))
                {
                    return;
                }
                Index++;
            }
            return;
        }

        //
        // Skip white spaces from the current position
        //
        // Return false if end of string is encountered.
        //
        internal bool SkipWhiteSpaceCurrent()
        {
            if (Index >= Length)
            {
                return (false);
            }

            if (!Char.IsWhiteSpace(m_current))
            {
                return (true);
            }

            while (++Index < Length)
            {
                m_current = Value[Index];
                if (!Char.IsWhiteSpace(m_current))
                {
                    return (true);
                }
                // Nothing here.
            }
            return (false);
        }

        internal void TrimTail()
        {
            int i = Length - 1;
            while (i >= 0 && Char.IsWhiteSpace(Value[i]))
            {
                i--;
            }
            Value = Value.Slice(0, i + 1);
        }

        // Trim the trailing spaces within a quoted string.
        // Call this after TrimTail() is done.
        internal void RemoveTrailingInQuoteSpaces()
        {
            int i = Length - 1;
            if (i <= 1)
            {
                return;
            }
            char ch = Value[i];
            // Check if the last character is a quote.
            if (ch == '\'' || ch == '\"')
            {
                if (char.IsWhiteSpace(Value[i - 1]))
                {
                    i--;
                    while (i >= 1 && char.IsWhiteSpace(Value[i - 1]))
                    {
                        i--;
                    }
                    Span<char> result = new char[i + 1];
                    result[i] = ch;
                    Value.Slice(0, i).CopyTo(result);
                    Value = result;
                }
            }
        }

        // Trim the leading spaces within a quoted string.
        // Call this after the leading spaces before quoted string are trimmed.
        internal void RemoveLeadingInQuoteSpaces()
        {
            if (Length <= 2)
            {
                return;
            }
            int i = 0;
            char ch = Value[i];
            // Check if the last character is a quote.
            if (ch == '\'' || ch == '\"')
            {
                while ((i + 1) < Length && char.IsWhiteSpace(Value[i + 1]))
                {
                    i++;
                }
                if (i != 0)
                {
                    Span<char> result = new char[Value.Length - i];
                    result[0] = ch;
                    Value.Slice(i + 1).CopyTo(result.Slice(1));
                    Value = result;
                }
            }
        }

        internal DTSubString GetSubString()
        {
            DTSubString sub = new DTSubString();
            sub.index = Index;
            sub.s = Value;
            while (Index + sub.length < Length)
            {
                DTSubStringType currentType;
                Char ch = Value[Index + sub.length];
                if (ch >= '0' && ch <= '9')
                {
                    currentType = DTSubStringType.Number;
                }
                else
                {
                    currentType = DTSubStringType.Other;
                }

                if (sub.length == 0)
                {
                    sub.type = currentType;
                }
                else
                {
                    if (sub.type != currentType)
                    {
                        break;
                    }
                }
                sub.length++;
                if (currentType == DTSubStringType.Number)
                {
                    // Incorporate the number into the value
                    // Limit the digits to prevent overflow
                    if (sub.length > DateTimeParse.MaxDateTimeNumberDigits)
                    {
                        sub.type = DTSubStringType.Invalid;
                        return sub;
                    }
                    int number = ch - '0';
                    Debug.Assert(number >= 0 && number <= 9, "number >= 0 && number <= 9");
                    sub.value = sub.value * 10 + number;
                }
                else
                {
                    // For non numbers, just return this length 1 token. This should be expanded
                    // to more types of thing if this parsing approach is used for things other
                    // than numbers and single characters
                    break;
                }
            }
            if (sub.length == 0)
            {
                sub.type = DTSubStringType.End;
                return sub;
            }

            return sub;
        }

        internal void ConsumeSubString(DTSubString sub)
        {
            Debug.Assert(sub.index == Index, "sub.index == Index");
            Debug.Assert(sub.index + sub.length <= Length, "sub.index + sub.length <= len");
            Index = sub.index + sub.length;
            if (Index < Length)
            {
                m_current = Value[Index];
            }
        }
    }

    internal enum DTSubStringType
    {
        Unknown = 0,
        Invalid = 1,
        Number = 2,
        End = 3,
        Other = 4,
    }

    internal ref struct DTSubString
    {
        internal ReadOnlySpan<char> s;
        internal Int32 index;
        internal Int32 length;
        internal DTSubStringType type;
        internal Int32 value;

        internal Char this[Int32 relativeIndex]
        {
            get
            {
                return s[index + relativeIndex];
            }
        }
    }

    //
    // The buffer to store the parsing token.
    //
    internal
    struct DateTimeToken
    {
        internal DateTimeParse.DTT dtt;    // Store the token
        internal TokenType suffix; // Store the CJK Year/Month/Day suffix (if any)
        internal int num;    // Store the number that we are parsing (if any)
    }

    //
    // The buffer to store temporary parsing information.
    //
    internal
    unsafe struct DateTimeRawInfo
    {
        private int* num;
        internal int numCount;
        internal int month;
        internal int year;
        internal int dayOfWeek;
        internal int era;
        internal DateTimeParse.TM timeMark;
        internal double fraction;
        internal bool hasSameDateAndTimeSeparators;

        internal void Init(int* numberBuffer)
        {
            month = -1;
            year = -1;
            dayOfWeek = -1;
            era = -1;
            timeMark = DateTimeParse.TM.NotSet;
            fraction = -1;
            num = numberBuffer;
        }
        internal unsafe void AddNumber(int value)
        {
            num[numCount++] = value;
        }
        internal unsafe int GetNumber(int index)
        {
            return num[index];
        }
    }

    internal enum ParseFailureKind
    {
        None = 0,
        ArgumentNull = 1,
        Format = 2,
        FormatWithParameter = 3,
        FormatWithOriginalDateTime = 4,
        FormatWithFormatSpecifier = 5,
        FormatWithOriginalDateTimeAndParameter = 6,
        FormatBadDateTimeCalendar = 7,  // FormatException when ArgumentOutOfRange is thrown by a Calendar.TryToDateTime().
    };

    [Flags]
    internal enum ParseFlags
    {
        HaveYear = 0x00000001,
        HaveMonth = 0x00000002,
        HaveDay = 0x00000004,
        HaveHour = 0x00000008,
        HaveMinute = 0x00000010,
        HaveSecond = 0x00000020,
        HaveTime = 0x00000040,
        HaveDate = 0x00000080,
        TimeZoneUsed = 0x00000100,
        TimeZoneUtc = 0x00000200,
        ParsedMonthName = 0x00000400,
        CaptureOffset = 0x00000800,
        YearDefault = 0x00001000,
        Rfc1123Pattern = 0x00002000,
        UtcSortPattern = 0x00004000,
    }

    //
    // This will store the result of the parsing.  And it will be eventually
    // used to construct a DateTime instance.
    //
    internal
    ref struct DateTimeResult
    {
        internal int Year;
        internal int Month;
        internal int Day;
        //
        // Set time defualt to 00:00:00.
        //
        internal int Hour;
        internal int Minute;
        internal int Second;
        internal double fraction;

        internal int era;

        internal ParseFlags flags;

        internal TimeSpan timeZoneOffset;

        internal Calendar calendar;

        internal DateTime parsedDate;

        internal ParseFailureKind failure;
        internal string failureMessageID;
        internal object failureMessageFormatArgument;
        internal string failureArgumentName;
        internal ReadOnlySpan<char> originalDateTimeString;
        internal ReadOnlySpan<char> failedFormatSpecifier;

        internal void Init(ReadOnlySpan<char> originalDateTimeString)
        {
            this.originalDateTimeString = originalDateTimeString;
            Year = -1;
            Month = -1;
            Day = -1;
            fraction = -1;
            era = -1;
        }

        internal void SetDate(int year, int month, int day)
        {
            Year = year;
            Month = month;
            Day = day;
        }

        internal void SetBadFormatSpecifierFailure()
        {
            SetBadFormatSpecifierFailure(ReadOnlySpan<char>.Empty);
        }

        internal void SetBadFormatSpecifierFailure(ReadOnlySpan<char> failedFormatSpecifier)
        {
            this.failure = ParseFailureKind.FormatWithFormatSpecifier;
            this.failureMessageID = nameof(SR.Format_BadFormatSpecifier);
            this.failedFormatSpecifier = failedFormatSpecifier;
        }

        internal void SetBadDateTimeFailure()
        {
            this.failure = ParseFailureKind.FormatWithOriginalDateTime;
            this.failureMessageID = nameof(SR.Format_BadDateTime);
            this.failureMessageFormatArgument = null;
        }

        internal void SetFailure(ParseFailureKind failure, string failureMessageID)
        {
            this.failure = failure;
            this.failureMessageID = failureMessageID;
            this.failureMessageFormatArgument = null;
        }

        internal void SetFailure(ParseFailureKind failure, string failureMessageID, object failureMessageFormatArgument)
        {
            this.failure = failure;
            this.failureMessageID = failureMessageID;
            this.failureMessageFormatArgument = failureMessageFormatArgument;
        }

        internal void SetFailure(ParseFailureKind failure, string failureMessageID, object failureMessageFormatArgument, string failureArgumentName)
        {
            this.failure = failure;
            this.failureMessageID = failureMessageID;
            this.failureMessageFormatArgument = failureMessageFormatArgument;
            this.failureArgumentName = failureArgumentName;
        }
    }

    // This is the helper data structure used in ParseExact().
    internal struct ParsingInfo
    {
        internal Calendar calendar;
        internal int dayOfWeek;
        internal DateTimeParse.TM timeMark;

        internal bool fUseHour12;
        internal bool fUseTwoDigitYear;
        internal bool fAllowInnerWhite;
        internal bool fAllowTrailingWhite;
        internal bool fCustomNumberParser;
        internal DateTimeParse.MatchNumberDelegate parseNumberDelegate;

        internal void Init()
        {
            dayOfWeek = -1;
            timeMark = DateTimeParse.TM.NotSet;
        }
    }

    //
    // The type of token that will be returned by DateTimeFormatInfo.Tokenize().
    //
    internal enum TokenType
    {
        // The valid token should start from 1.

        // Regular tokens. The range is from 0x00 ~ 0xff.
        NumberToken = 1,    // The number.  E.g. "12"
        YearNumberToken = 2,    // The number which is considered as year number, which has 3 or more digits.  E.g. "2003"
        Am = 3,    // AM timemark. E.g. "AM"
        Pm = 4,    // PM timemark. E.g. "PM"
        MonthToken = 5,    // A word (or words) that represents a month name.  E.g. "March"
        EndOfString = 6,    // End of string
        DayOfWeekToken = 7,    // A word (or words) that represents a day of week name.  E.g. "Monday" or "Mon"
        TimeZoneToken = 8,    // A word that represents a timezone name. E.g. "GMT"
        EraToken = 9,    // A word that represents a era name. E.g. "A.D."
        DateWordToken = 10,   // A word that can appear in a DateTime string, but serves no parsing semantics.  E.g. "de" in Spanish culture.
        UnknownToken = 11,   // An unknown word, which signals an error in parsing.
        HebrewNumber = 12,   // A number that is composed of Hebrew text.  Hebrew calendar uses Hebrew digits for year values, month values, and day values.
        JapaneseEraToken = 13,   // Era name for JapaneseCalendar
        TEraToken = 14,   // Era name for TaiwanCalendar
        IgnorableSymbol = 15,   // A separator like "," that is equivalent to whitespace


        // Separator tokens.
        SEP_Unk = 0x100,         // Unknown separator.
        SEP_End = 0x200,    // The end of the parsing string.
        SEP_Space = 0x300,    // Whitespace (including comma).
        SEP_Am = 0x400,    // AM timemark. E.g. "AM"
        SEP_Pm = 0x500,    // PM timemark. E.g. "PM"
        SEP_Date = 0x600,    // date separator. E.g. "/"
        SEP_Time = 0x700,    // time separator. E.g. ":"
        SEP_YearSuff = 0x800,    // Chinese/Japanese/Korean year suffix.
        SEP_MonthSuff = 0x900,    // Chinese/Japanese/Korean month suffix.
        SEP_DaySuff = 0xa00,    // Chinese/Japanese/Korean day suffix.
        SEP_HourSuff = 0xb00,   // Chinese/Japanese/Korean hour suffix.
        SEP_MinuteSuff = 0xc00,   // Chinese/Japanese/Korean minute suffix.
        SEP_SecondSuff = 0xd00,   // Chinese/Japanese/Korean second suffix.
        SEP_LocalTimeMark = 0xe00,   // 'T', used in ISO 8601 format.
        SEP_DateOrOffset = 0xf00,   // '-' which could be a date separator or start of a time zone offset

        RegularTokenMask = 0x00ff,
        SeparatorTokenMask = 0xff00,
    }
}
