// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using Xunit;

namespace System.Globalization.CalendarsTests
{
    // System.Globalization.TaiwanCalendar.GetWeekOfYear(DateTime,CalendarWeekRule,DayOfWeek)
    public class TaiwanCalendarGetWeekOfYear
    {
        private readonly int[] _DAYS_PER_MONTHS_IN_LEAP_YEAR = new int[13] { 0, 31, 29, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
        private readonly int[] _DAYS_PER_MONTHS_IN_NO_LEAP_YEAR = new int[13] { 0, 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

        #region Positive Tests
        // PosTest1: Verify the DateTime is a random Date
        [Fact]
        public void PosTest1()
        {
            System.Globalization.Calendar tc = new TaiwanCalendar();
            Random rand = new Random(-55);
            int year = rand.Next(tc.MinSupportedDateTime.Year, tc.MaxSupportedDateTime.Year - 1911);
            int month = rand.Next(1, 12);
            int day;
            if (tc.IsLeapYear(year))
            {
                day = rand.Next(1, _DAYS_PER_MONTHS_IN_LEAP_YEAR[month] + 1);
            }
            else
            {
                day = rand.Next(1, _DAYS_PER_MONTHS_IN_NO_LEAP_YEAR[month] + 1);
            }
            DateTime dt = new DateTime(year, month, day);

            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    int actualWeek = getWeekOfYear(dt, (CalendarWeekRule)j, (DayOfWeek)i);
                    int resultWeek = tc.GetWeekOfYear(dt, (CalendarWeekRule)j, (DayOfWeek)i);
                    Assert.Equal(resultWeek, actualWeek);
                }
            }
        }

        // PosTest2: Verify the DateTime is TaiwanCalendar MaxSupportDateTime
        [Fact]
        public void PosTest2()
        {
            System.Globalization.Calendar tc = new TaiwanCalendar();
            DateTime dt = tc.MaxSupportedDateTime;

            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    int actualWeek = getWeekOfYear(dt, (CalendarWeekRule)j, (DayOfWeek)i);
                    int resultWeek = tc.GetWeekOfYear(dt, (CalendarWeekRule)j, (DayOfWeek)i);
                    Assert.Equal(resultWeek, actualWeek);
                }
            }
        }

        // PosTest3: Verify the DateTime is TaiwanCalendar MinSupportedDateTime
        [Fact]
        public void PosTest3()
        {
            System.Globalization.Calendar tc = new TaiwanCalendar();
            DateTime dt = tc.MinSupportedDateTime;

            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    int actualWeek = getWeekOfYear(dt, (CalendarWeekRule)j, (DayOfWeek)i);
                    int resultWeek = tc.GetWeekOfYear(dt, (CalendarWeekRule)j, (DayOfWeek)i);
                    Assert.Equal(resultWeek, actualWeek);
                }
            }
        }

        // PosTest4: Verify the DateTime is the last day of the year
        [Fact]
        public void PosTest4()
        {
            System.Globalization.Calendar tc = new TaiwanCalendar();
            System.Globalization.Calendar gc = new GregorianCalendar();
            Random rand = new Random(-55);
            int year = rand.Next(tc.MinSupportedDateTime.Year, tc.MaxSupportedDateTime.Year - 1911);
            int month = 12;
            int day = 31;
            int actualWeek = 53;
            DateTime dt = new DateTime(year, month, day);

            if (DayOfWeek.Saturday == new DateTime(year, 1, 1).DayOfWeek && DateTime.IsLeapYear(year))
            {
                actualWeek = 54;
            }
            int resultWeek = tc.GetWeekOfYear(dt, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);

            Assert.Equal(resultWeek, actualWeek);
        }
        #endregion

        #region Negative Tests
        // NegTest1: The resulting DateTime is less than the supported range
        [Fact]
        public void NegTest1()
        {
            System.Globalization.Calendar tc = new TaiwanCalendar();
            Random rand = new Random(-55);
            DateTime dt = tc.MinSupportedDateTime;
            dt = dt.AddYears(rand.Next(-1911, -1));

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                tc.GetWeekOfYear(dt, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
            });
        }

        // NegTest2: firstDayOfWeek is outside the range supported by the calendar
        [Fact]
        public void NegTest2()
        {
            System.Globalization.Calendar tc = new TaiwanCalendar();
            Random rand = new Random(-55);
            int year = rand.Next(tc.MinSupportedDateTime.Year, tc.MaxSupportedDateTime.Year - 1911);
            int month = rand.Next(1, 12);
            int day;
            if (tc.IsLeapYear(year))
            {
                day = rand.Next(1, _DAYS_PER_MONTHS_IN_LEAP_YEAR[month] + 1);
            }
            else
            {
                day = rand.Next(1, _DAYS_PER_MONTHS_IN_NO_LEAP_YEAR[month] + 1);
            }

            DateTime dt = new DateTime(year, month, day);
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                tc.GetWeekOfYear(dt, CalendarWeekRule.FirstDay, (DayOfWeek)7);
            });

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                tc.GetWeekOfYear(dt, CalendarWeekRule.FirstDay, (DayOfWeek)(-1));
            });
        }

        // NegTest3: CalendarWeekRule is outside the range supported by the calendar
        [Fact]
        public void NegTest3()
        {
            System.Globalization.Calendar tc = new TaiwanCalendar();
            Random rand = new Random(-55);
            int year = rand.Next(tc.MinSupportedDateTime.Year, tc.MaxSupportedDateTime.Year - 1911);
            int month = rand.Next(1, 12);
            int day;
            if (tc.IsLeapYear(year))
            {
                day = rand.Next(1, _DAYS_PER_MONTHS_IN_LEAP_YEAR[month] + 1);
            }
            else
            {
                day = rand.Next(1, _DAYS_PER_MONTHS_IN_NO_LEAP_YEAR[month] + 1);
            }

            DateTime dt = new DateTime(year, month, day);

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                tc.GetWeekOfYear(dt, (CalendarWeekRule)3, DayOfWeek.Sunday);
            });

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                tc.GetWeekOfYear(dt, (CalendarWeekRule)(-1), DayOfWeek.Sunday);
            });
        }



        #endregion
        #region Help Methods
        internal int GetFirstDayWeekOfYear(DateTime time, int firstDayOfWeek)
        {
            System.Globalization.Calendar gc = new GregorianCalendar();
            int dayOfYear = gc.GetDayOfYear(time) - 1;   // Make the day of year to be 0-based, so that 1/1 is day 0.
                                                         // Calculate the day of week for the first day of the year.
                                                         // dayOfWeek - (dayOfYear % 7) is the day of week for the first day of this year.  Note that
                                                         // this value can be less than 0.  It's fine since we are making it positive again in calculating offset.
            int dayForJan1 = (int)gc.GetDayOfWeek(time) - (dayOfYear % 7);
            int offset = (dayForJan1 - firstDayOfWeek + 14) % 7;
            //BCLDebug.Assert(offset >= 0, "Calendar.GetFirstDayWeekOfYear(): offset >= 0");
            return ((dayOfYear + offset) / 7 + 1);
        }

        internal int GetWeekOfYearFullDays(DateTime time, CalendarWeekRule rule, int firstDayOfWeek, int fullDays)
        {
            int dayForJan1;
            int offset;
            int day;
            System.Globalization.Calendar gc = new GregorianCalendar();
            int dayOfYear = gc.GetDayOfYear(time) - 1; // Make the day of year to be 0-based, so that 1/1 is day 0.
                                                       //
                                                       // Calculate the number of days between the first day of year (1/1) and the first day of the week.
                                                       // This value will be a positive value from 0 ~ 6.  We call this value as "offset".
                                                       //
                                                       // If offset is 0, it means that the 1/1 is the start of the first week.
                                                       //     Assume the first day of the week is Monday, it will look like this:
                                                       //     Sun      Mon     Tue     Wed     Thu     Fri     Sat
                                                       //     12/31    1/1     1/2     1/3     1/4     1/5     1/6
                                                       //              +--> First week starts here.
                                                       //
                                                       // If offset is 1, it means that the first day of the week is 1 day ahead of 1/1.
                                                       //     Assume the first day of the week is Monday, it will look like this:
                                                       //     Sun      Mon     Tue     Wed     Thu     Fri     Sat
                                                       //     1/1      1/2     1/3     1/4     1/5     1/6     1/7
                                                       //              +--> First week starts here.
                                                       //
                                                       // If offset is 2, it means that the first day of the week is 2 days ahead of 1/1.
                                                       //     Assume the first day of the week is Monday, it will look like this:
                                                       //     Sat      Sun     Mon     Tue     Wed     Thu     Fri     Sat
                                                       //     1/1      1/2     1/3     1/4     1/5     1/6     1/7     1/8
                                                       //                      +--> First week starts here.



            // Day of week is 0-based.
            // Get the day of week for 1/1.  This can be derived from the day of week of the target day.
            // Note that we can get a negative value.  It's ok since we are going to make it a positive value when calculating the offset.
            dayForJan1 = (int)gc.GetDayOfWeek(time) - (dayOfYear % 7);

            // Now, calucalte the offset.  Substract the first day of week from the dayForJan1.  And make it a positive value.
            offset = (firstDayOfWeek - dayForJan1 + 14) % 7;
            if (offset != 0 && offset >= fullDays)
            {
                //
                // If the offset is greater than the value of fullDays, it means that
                // the first week of the year starts on the week where Jan/1 falls on.
                //
                offset -= 7;
            }
            //
            // Calculate the day of year for specified time by taking offset into account.
            //
            day = dayOfYear - offset;
            if (day >= 0)
            {
                //
                // If the day of year value is greater than zero, get the week of year.
                //
                return (day / 7 + 1);
            }
            //
            // Otherwise, the specified time falls on the week of previous year.
            // Call this method again by passing the last day of previous year.
            //
            return (GetWeekOfYearFullDays(time.AddDays(-(dayOfYear + 1)), rule, firstDayOfWeek, fullDays));
        }

        // Returns the week of year for the specified DateTime. The returned value is an
        // integer between 1 and 53.
        //

        public virtual int getWeekOfYear(DateTime time, CalendarWeekRule rule, DayOfWeek firstDayOfWeek)
        {
            if ((int)firstDayOfWeek < 0 || (int)firstDayOfWeek > 6)
            {
                throw new ArgumentOutOfRangeException();
            }
            switch (rule)
            {
                case CalendarWeekRule.FirstDay:
                    return (GetFirstDayWeekOfYear(time, (int)firstDayOfWeek));
                case CalendarWeekRule.FirstFullWeek:
                    return (GetWeekOfYearFullDays(time, rule, (int)firstDayOfWeek, 7));
                case CalendarWeekRule.FirstFourDayWeek:
                    return (GetWeekOfYearFullDays(time, rule, (int)firstDayOfWeek, 4));
            }
            throw new ArgumentOutOfRangeException();
        }
        #endregion
    }
}

