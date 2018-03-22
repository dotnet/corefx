// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Threading;

namespace System.Globalization
{
    // This calendar recognizes two era values:
    // 0 CurrentEra (AD)
    // 1 BeforeCurrentEra (BC)
    public class GregorianCalendar : Calendar
    {
        /*
            A.D. = anno Domini
         */

        public const int ADEra = 1;

        //
        // This is the max Gregorian year can be represented by DateTime class.  The limitation
        // is derived from DateTime class.
        //
        internal const int MaxYear = 9999;

        internal GregorianCalendarTypes m_type;

        internal static readonly int[] DaysToMonth365 =
        {
            0, 31, 59, 90, 120, 151, 181, 212, 243, 273, 304, 334, 365
        };

        internal static readonly int[] DaysToMonth366 =
        {
            0, 31, 60, 91, 121, 152, 182, 213, 244, 274, 305, 335, 366
        };

        private static volatile Calendar s_defaultInstance;


        public override DateTime MinSupportedDateTime
        {
            get
            {
                return (DateTime.MinValue);
            }
        }

        public override DateTime MaxSupportedDateTime
        {
            get
            {
                return (DateTime.MaxValue);
            }
        }

        public override CalendarAlgorithmType AlgorithmType
        {
            get
            {
                return CalendarAlgorithmType.SolarCalendar;
            }
        }

        /*=================================GetDefaultInstance==========================
        **Action: Internal method to provide a default intance of GregorianCalendar.  Used by NLS+ implementation
        **       and other calendars.
        **Returns:
        **Arguments:
        **Exceptions:
        ============================================================================*/

        internal static Calendar GetDefaultInstance()
        {
            if (s_defaultInstance == null)
            {
                s_defaultInstance = new GregorianCalendar();
            }
            return (s_defaultInstance);
        }

        // Construct an instance of gregorian calendar.

        public GregorianCalendar() :
            this(GregorianCalendarTypes.Localized)
        {
        }


        public GregorianCalendar(GregorianCalendarTypes type)
        {
            if ((int)type < (int)GregorianCalendarTypes.Localized || (int)type > (int)GregorianCalendarTypes.TransliteratedFrench)
            {
                throw new ArgumentOutOfRangeException(
                            nameof(type),
                            SR.Format(SR.ArgumentOutOfRange_Range,
                    GregorianCalendarTypes.Localized, GregorianCalendarTypes.TransliteratedFrench));
            }
            this.m_type = type;
        }

        public virtual GregorianCalendarTypes CalendarType
        {
            get
            {
                return (m_type);
            }

            set
            {
                VerifyWritable();

                switch (value)
                {
                    case GregorianCalendarTypes.Localized:
                    case GregorianCalendarTypes.USEnglish:
                    case GregorianCalendarTypes.MiddleEastFrench:
                    case GregorianCalendarTypes.Arabic:
                    case GregorianCalendarTypes.TransliteratedEnglish:
                    case GregorianCalendarTypes.TransliteratedFrench:
                        m_type = value;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException("m_type", SR.ArgumentOutOfRange_Enum);
                }
            }
        }

        internal override CalendarId ID
        {
            get
            {
                // By returning different ID for different variations of GregorianCalendar,
                // we can support the Transliterated Gregorian calendar.
                // DateTimeFormatInfo will use this ID to get formatting information about
                // the calendar.
                return ((CalendarId)m_type);
            }
        }


        /*=================================GetAbsoluteDate==========================
        **Action: Gets the absolute date for the given Gregorian date.  The absolute date means
        **       the number of days from January 1st, 1 A.D.
        **Returns:  the absolute date
        **Arguments:
        **      year    the Gregorian year
        **      month   the Gregorian month
        **      day     the day
        **Exceptions:
        **      ArgumentOutOfRangException  if year, month, day value is valid.
        **Note:
        **      This is an internal method used by DateToTicks() and the calculations of Hijri and Hebrew calendars.
        **      Number of Days in Prior Years (both common and leap years) +
        **      Number of Days in Prior Months of Current Year +
        **      Number of Days in Current Month
        **
        ============================================================================*/

        internal static long GetAbsoluteDate(int year, int month, int day)
        {
            if (year >= 1 && year <= MaxYear && month >= 1 && month <= 12)
            {
                int[] days = ((year % 4 == 0 && (year % 100 != 0 || year % 400 == 0))) ? DaysToMonth366 : DaysToMonth365;
                if (day >= 1 && (day <= days[month] - days[month - 1]))
                {
                    int y = year - 1;
                    int absoluteDate = y * 365 + y / 4 - y / 100 + y / 400 + days[month - 1] + day - 1;
                    return (absoluteDate);
                }
            }
            throw new ArgumentOutOfRangeException(null, SR.ArgumentOutOfRange_BadYearMonthDay);
        }

        // Returns the tick count corresponding to the given year, month, and day.
        // Will check the if the parameters are valid.
        internal virtual long DateToTicks(int year, int month, int day)
        {
            return (GetAbsoluteDate(year, month, day) * TicksPerDay);
        }

        // Returns the DateTime resulting from adding the given number of
        // months to the specified DateTime. The result is computed by incrementing
        // (or decrementing) the year and month parts of the specified DateTime by
        // value months, and, if required, adjusting the day part of the
        // resulting date downwards to the last day of the resulting month in the
        // resulting year. The time-of-day part of the result is the same as the
        // time-of-day part of the specified DateTime.
        //
        // In more precise terms, considering the specified DateTime to be of the
        // form y / m / d + t, where y is the
        // year, m is the month, d is the day, and t is the
        // time-of-day, the result is y1 / m1 / d1 + t,
        // where y1 and m1 are computed by adding value months
        // to y and m, and d1 is the largest value less than
        // or equal to d that denotes a valid day in month m1 of year
        // y1.
        //

        public override DateTime AddMonths(DateTime time, int months)
        {
            if (months < -120000 || months > 120000)
            {
                throw new ArgumentOutOfRangeException(
                            nameof(months),
                            String.Format(
                                CultureInfo.CurrentCulture,
                                SR.ArgumentOutOfRange_Range,
                                -120000,
                                120000));
            }
            time.GetDatePart(out int y, out int m, out int d);
            int i = m - 1 + months;
            if (i >= 0)
            {
                m = i % 12 + 1;
                y = y + i / 12;
            }
            else
            {
                m = 12 + (i + 1) % 12;
                y = y + (i - 11) / 12;
            }
            int[] daysArray = (y % 4 == 0 && (y % 100 != 0 || y % 400 == 0)) ? DaysToMonth366 : DaysToMonth365;
            int days = (daysArray[m] - daysArray[m - 1]);

            if (d > days)
            {
                d = days;
            }
            long ticks = DateToTicks(y, m, d) + time.Ticks % TicksPerDay;
            Calendar.CheckAddResult(ticks, MinSupportedDateTime, MaxSupportedDateTime);

            return (new DateTime(ticks));
        }


        // Returns the DateTime resulting from adding the given number of
        // years to the specified DateTime. The result is computed by incrementing
        // (or decrementing) the year part of the specified DateTime by value
        // years. If the month and day of the specified DateTime is 2/29, and if the
        // resulting year is not a leap year, the month and day of the resulting
        // DateTime becomes 2/28. Otherwise, the month, day, and time-of-day
        // parts of the result are the same as those of the specified DateTime.
        //

        public override DateTime AddYears(DateTime time, int years)
        {
            return (AddMonths(time, years * 12));
        }

        // Returns the day-of-month part of the specified DateTime. The returned
        // value is an integer between 1 and 31.
        //

        public override int GetDayOfMonth(DateTime time)
        {
            return time.Day;
        }

        // Returns the day-of-week part of the specified DateTime. The returned value
        // is an integer between 0 and 6, where 0 indicates Sunday, 1 indicates
        // Monday, 2 indicates Tuesday, 3 indicates Wednesday, 4 indicates
        // Thursday, 5 indicates Friday, and 6 indicates Saturday.
        //

        public override DayOfWeek GetDayOfWeek(DateTime time)
        {
            return ((DayOfWeek)((int)(time.Ticks / TicksPerDay + 1) % 7));
        }

        // Returns the day-of-year part of the specified DateTime. The returned value
        // is an integer between 1 and 366.
        //

        public override int GetDayOfYear(DateTime time)
        {
            return time.DayOfYear;
        }

        // Returns the number of days in the month given by the year and
        // month arguments.
        //

        public override int GetDaysInMonth(int year, int month, int era)
        {
            if (era == CurrentEra || era == ADEra)
            {
                if (year < 1 || year > MaxYear)
                {
                    throw new ArgumentOutOfRangeException(nameof(year), SR.Format(SR.ArgumentOutOfRange_Range,
                        1, MaxYear));
                }
                if (month < 1 || month > 12)
                {
                    throw new ArgumentOutOfRangeException(nameof(month), SR.ArgumentOutOfRange_Month);
                }
                int[] days = ((year % 4 == 0 && (year % 100 != 0 || year % 400 == 0)) ? DaysToMonth366 : DaysToMonth365);
                return (days[month] - days[month - 1]);
            }
            throw new ArgumentOutOfRangeException(nameof(era), SR.ArgumentOutOfRange_InvalidEraValue);
        }

        // Returns the number of days in the year given by the year argument for the current era.
        //

        public override int GetDaysInYear(int year, int era)
        {
            if (era == CurrentEra || era == ADEra)
            {
                if (year >= 1 && year <= MaxYear)
                {
                    return ((year % 4 == 0 && (year % 100 != 0 || year % 400 == 0)) ? 366 : 365);
                }
                throw new ArgumentOutOfRangeException(
                            nameof(year),
                            String.Format(
                                CultureInfo.CurrentCulture,
                                SR.ArgumentOutOfRange_Range,
                                1,
                                MaxYear));
            }
            throw new ArgumentOutOfRangeException(nameof(era), SR.ArgumentOutOfRange_InvalidEraValue);
        }

        // Returns the era for the specified DateTime value.

        public override int GetEra(DateTime time)
        {
            return (ADEra);
        }


        public override int[] Eras
        {
            get
            {
                return (new int[] { ADEra });
            }
        }


        // Returns the month part of the specified DateTime. The returned value is an
        // integer between 1 and 12.
        //

        public override int GetMonth(DateTime time)
        {
            return time.Month;
        }

        // Returns the number of months in the specified year and era.

        public override int GetMonthsInYear(int year, int era)
        {
            if (era == CurrentEra || era == ADEra)
            {
                if (year >= 1 && year <= MaxYear)
                {
                    return (12);
                }
                throw new ArgumentOutOfRangeException(
                            nameof(year),
                            String.Format(
                                CultureInfo.CurrentCulture,
                                SR.ArgumentOutOfRange_Range,
                                1,
                                MaxYear));
            }
            throw new ArgumentOutOfRangeException(nameof(era), SR.ArgumentOutOfRange_InvalidEraValue);
        }

        // Returns the year part of the specified DateTime. The returned value is an
        // integer between 1 and 9999.
        //

        public override int GetYear(DateTime time)
        {
            return time.Year;
        }

        internal override bool IsValidYear(int year, int era) => year >= 1 && year <= MaxYear;

        internal override bool IsValidDay(int year, int month, int day, int era)
        {
            if ((era != CurrentEra && era != ADEra) || 
                year < 1 || year > MaxYear ||
                month < 1 || month > 12 ||
                day < 1)
            {
                return false;
            }

            int[] days = (year % 4 == 0 && (year % 100 != 0 || year % 400 == 0)) ? DaysToMonth366 : DaysToMonth365;
            return day <= (days[month] - days[month - 1]);
        }

        // Checks whether a given day in the specified era is a leap day. This method returns true if
        // the date is a leap day, or false if not.
        //

        public override bool IsLeapDay(int year, int month, int day, int era)
        {
            if (month < 1 || month > 12)
            {
                throw new ArgumentOutOfRangeException(nameof(month), SR.Format(SR.ArgumentOutOfRange_Range,
                    1, 12));
            }

            if (era != CurrentEra && era != ADEra)
            {
                throw new ArgumentOutOfRangeException(nameof(era), SR.ArgumentOutOfRange_InvalidEraValue);
            }
            if (year < 1 || year > MaxYear)
            {
                throw new ArgumentOutOfRangeException(
                                nameof(year),
                                SR.Format(SR.ArgumentOutOfRange_Range, 1, MaxYear));
            }

            if (day < 1 || day > GetDaysInMonth(year, month))
            {
                throw new ArgumentOutOfRangeException(nameof(day), SR.Format(SR.ArgumentOutOfRange_Range,
                    1, GetDaysInMonth(year, month)));
            }
            if (!IsLeapYear(year))
            {
                return (false);
            }
            if (month == 2 && day == 29)
            {
                return (true);
            }
            return (false);
        }

        // Returns  the leap month in a calendar year of the specified era. This method returns 0
        // if this calendar does not have leap month, or this year is not a leap year.
        //

        public override int GetLeapMonth(int year, int era)
        {
            if (era != CurrentEra && era != ADEra)
            {
                throw new ArgumentOutOfRangeException(nameof(era), SR.ArgumentOutOfRange_InvalidEraValue);
            }
            if (year < 1 || year > MaxYear)
            {
                throw new ArgumentOutOfRangeException(
                            nameof(year),
                            String.Format(
                                CultureInfo.CurrentCulture,
                                SR.ArgumentOutOfRange_Range, 1, MaxYear));
            }
            return (0);
        }

        // Checks whether a given month in the specified era is a leap month. This method returns true if
        // month is a leap month, or false if not.
        //

        public override bool IsLeapMonth(int year, int month, int era)
        {
            if (era != CurrentEra && era != ADEra)
            {
                throw new ArgumentOutOfRangeException(nameof(era), SR.ArgumentOutOfRange_InvalidEraValue);
            }

            if (year < 1 || year > MaxYear)
            {
                throw new ArgumentOutOfRangeException(
                            nameof(year),
                            String.Format(
                                CultureInfo.CurrentCulture,
                                SR.ArgumentOutOfRange_Range, 1, MaxYear));
            }

            if (month < 1 || month > 12)
            {
                throw new ArgumentOutOfRangeException(nameof(month), SR.Format(SR.ArgumentOutOfRange_Range,
                    1, 12));
            }
            return (false);
        }

        // Checks whether a given year in the specified era is a leap year. This method returns true if
        // year is a leap year, or false if not.
        //

        public override bool IsLeapYear(int year, int era)
        {
            if (era == CurrentEra || era == ADEra)
            {
                if (year >= 1 && year <= MaxYear)
                {
                    return (year % 4 == 0 && (year % 100 != 0 || year % 400 == 0));
                }

                throw new ArgumentOutOfRangeException(
                            nameof(year),
                            String.Format(
                                CultureInfo.CurrentCulture,
                                SR.ArgumentOutOfRange_Range, 1, MaxYear));
            }
            throw new ArgumentOutOfRangeException(nameof(era), SR.ArgumentOutOfRange_InvalidEraValue);
        }

        // Returns the date and time converted to a DateTime value.  Throws an exception if the n-tuple is invalid.
        //

        public override DateTime ToDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond, int era)
        {
            if (era == CurrentEra || era == ADEra)
            {
                return new DateTime(year, month, day, hour, minute, second, millisecond);
            }
            throw new ArgumentOutOfRangeException(nameof(era), SR.ArgumentOutOfRange_InvalidEraValue);
        }

        internal override bool TryToDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond, int era, out DateTime result)
        {
            if (era == CurrentEra || era == ADEra)
            {
                return DateTime.TryCreate(year, month, day, hour, minute, second, millisecond, out result);
            }
            result = DateTime.MinValue;
            return false;
        }

        private const int DEFAULT_TWO_DIGIT_YEAR_MAX = 2029;


        public override int TwoDigitYearMax
        {
            get
            {
                if (twoDigitYearMax == -1)
                {
                    twoDigitYearMax = GetSystemTwoDigitYearSetting(ID, DEFAULT_TWO_DIGIT_YEAR_MAX);
                }
                return (twoDigitYearMax);
            }

            set
            {
                VerifyWritable();
                if (value < 99 || value > MaxYear)
                {
                    throw new ArgumentOutOfRangeException(
                                "year",
                                String.Format(
                                    CultureInfo.CurrentCulture,
                                    SR.ArgumentOutOfRange_Range,
                                    99,
                                    MaxYear));
                }
                twoDigitYearMax = value;
            }
        }


        public override int ToFourDigitYear(int year)
        {
            if (year < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(year),
                    SR.ArgumentOutOfRange_NeedNonNegNum);
            }

            if (year > MaxYear)
            {
                throw new ArgumentOutOfRangeException(
                            nameof(year),
                            String.Format(
                                CultureInfo.CurrentCulture,
                                SR.ArgumentOutOfRange_Range, 1, MaxYear));
            }
            return (base.ToFourDigitYear(year));
        }
    }
}
