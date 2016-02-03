// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using Xunit;

namespace System.Globalization.CalendarsTests
{
    // System.Globalization.ThaiBuddhistCalendar.GetDayOfMonth(DateTime)
    public class ThaiBuddhistCalendarGetDayOfMonth
    {
        private readonly int[] _DAYS_PER_MONTHS_IN_LEAP_YEAR = new int[13]
        {
            0, 31, 29, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31
        };
        private readonly int[] _DAYS_PER_MONTHS_IN_NO_LEAP_YEAR = new int[13]
        {
            0, 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31
        };

        #region Positive Tests
        // PosTest1: Verify the day is a random Date
        [Fact]
        public void PosTest1()
        {
            System.Globalization.Calendar tbc = new ThaiBuddhistCalendar();
            Random rand = new Random(-55);
            int year = rand.Next(tbc.MinSupportedDateTime.Year + 543, tbc.MaxSupportedDateTime.Year + 544);
            int month = rand.Next(1, 12);
            int day;
            if (IsLeapYear(year))
            {
                day = rand.Next(1, _DAYS_PER_MONTHS_IN_LEAP_YEAR[month] + 1);
            }
            else
            {
                day = rand.Next(1, _DAYS_PER_MONTHS_IN_NO_LEAP_YEAR[month] + 1);
            }

            DateTime dt = tbc.ToDateTime(year, month, day, 0, 0, 0, 0);
            int actualDays = dt.Day;
            Assert.Equal(actualDays, tbc.GetDayOfMonth(dt));
        }

        // PosTest2: Verify the DateTime is a leap year
        [Fact]
        public void PosTest2()
        {
            System.Globalization.Calendar tbc = new ThaiBuddhistCalendar();
            Random rand = new Random(-55);
            int year = 2000;
            int month = 2;
            int day;
            DateTime dt;
            for (day = 1; day < 30; day++)
            {
                dt = new DateTime(year, month, day);
                Assert.Equal(day, tbc.GetDayOfMonth(dt));
            }
        }

        // PosTest3: Verify the DateTime is ThaiBuddhistCalendar MaxSupportDateTime
        [Fact]
        public void PosTest3()
        {
            System.Globalization.Calendar tbc = new ThaiBuddhistCalendar();
            DateTime dt = tbc.MaxSupportedDateTime;
            Assert.Equal(31, tbc.GetDayOfMonth(dt));
        }

        // PosTest4: Verify the DateTime is ThaiBuddhistCalendar MinSupportedDateTime
        [Fact]
        public void PosTest4()
        {
            System.Globalization.Calendar tbc = new ThaiBuddhistCalendar();
            DateTime dt = tbc.MinSupportedDateTime;
            Assert.Equal(1, tbc.GetDayOfMonth(dt));
        }
        #endregion

        #region Helper Method
        private bool IsLeapYear(int year)
        {
            return ((year % 4) == 0) && !(((year % 100) == 0) || ((year % 400) == 0));
        }
        #endregion
    }
}
