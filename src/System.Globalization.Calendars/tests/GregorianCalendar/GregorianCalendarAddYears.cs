// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using Xunit;

namespace System.Globalization.CalendarsTests
{
    // GregorianCalendar.AddYears(DateTime, int)
    public class GregorianCalendarAddYears
    {
        private readonly RandomDataGenerator _generator = new RandomDataGenerator();

        #region Positive tests
        // PosTest1: Add zero year to the specified date time
        [Fact]
        public void PosTest1()
        {
            DateTime initialTime;
            int years;
            DateTime resultingTime;
            System.Globalization.Calendar myCalendar = new GregorianCalendar(GregorianCalendarTypes.USEnglish);
            years = 0;
            initialTime = DateTime.Now;
            resultingTime = myCalendar.AddYears(initialTime, years);
            Assert.Equal(initialTime, resultingTime);
        }

        // PosTest2: the specified time is MinSupportedDateTime and the number of years added is a normal value
        [Fact]
        public void PosTest2()
        {
            DateTime initialTime;
            int years;
            DateTime resultingTime;
            System.Globalization.Calendar myCalendar = new GregorianCalendar(GregorianCalendarTypes.USEnglish);
            years = 100;
            initialTime = myCalendar.MinSupportedDateTime;
            resultingTime = myCalendar.AddYears(initialTime, years);
            VerifyAddyearsResult(myCalendar, initialTime, resultingTime, years);
        }

        // PosTest3: the specified time is MaxSupportedDateTime and the number of years added is a normal value
        [Fact]
        public void PosTest3()
        {
            DateTime initialTime;
            int years;
            DateTime resultingTime;
            System.Globalization.Calendar myCalendar = new GregorianCalendar(GregorianCalendarTypes.USEnglish);
            years = -99;
            initialTime = myCalendar.MaxSupportedDateTime;
            resultingTime = myCalendar.AddYears(initialTime, years);
            VerifyAddyearsResult(myCalendar, initialTime, resultingTime, years);
        }

        // PosTest4: the specified time is random value between 0 and MaxSupportedDateTime - 1, years added is a normal value
        [Fact]
        public void PosTest4()
        {
            DateTime initialTime;
            int years;
            DateTime resultingTime;
            System.Globalization.Calendar myCalendar = new GregorianCalendar(GregorianCalendarTypes.USEnglish);
            years = 1;
            long maxTimeInTicks = myCalendar.MaxSupportedDateTime.Ticks;
            long minTimeInTikcs = myCalendar.MinSupportedDateTime.Ticks;
            initialTime = new DateTime(_generator.GetInt64(-55) % maxTimeInTicks);
            resultingTime = myCalendar.AddYears(initialTime, years);
            VerifyAddyearsResult(myCalendar, initialTime, resultingTime, years);
        }

        // PosTest5: the specified time is February in leap year, years added is a normal value
        [Fact]
        public void PosTest5()
        {
            DateTime initialTime;
            int years;
            DateTime resultingTime;
            System.Globalization.Calendar myCalendar = new GregorianCalendar(GregorianCalendarTypes.USEnglish);
            years = 13;
            initialTime = myCalendar.ToDateTime(2000, 2, 29, 10, 30, 24, 0);
            resultingTime = myCalendar.AddYears(initialTime, years);
            VerifyAddyearsResult(myCalendar, initialTime, resultingTime, years);
        }

        // PosTest6: the specified time is February in leap year, years added is a normal value
        [Fact]
        public void PosTest6()
        {
            DateTime initialTime;
            int years;
            DateTime resultingTime;
            System.Globalization.Calendar myCalendar = new GregorianCalendar(GregorianCalendarTypes.USEnglish);
            years = 4;
            initialTime = myCalendar.ToDateTime(1996, 2, 29, 10, 30, 24, 0);
            resultingTime = myCalendar.AddYears(initialTime, years);
            VerifyAddyearsResult(myCalendar, initialTime, resultingTime, years);
        }

        // PosTest7: the specified time is any month other than February in leap year, years added is a normal value
        [Fact]
        public void PosTest7()
        {
            DateTime initialTime;
            int years;
            DateTime resultingTime;
            System.Globalization.Calendar myCalendar = new GregorianCalendar(GregorianCalendarTypes.USEnglish);
            years = 48;
            initialTime = myCalendar.ToDateTime(1996, 3, 29, 10, 30, 24, 0);
            resultingTime = myCalendar.AddYears(initialTime, years);
            VerifyAddyearsResult(myCalendar, initialTime, resultingTime, years);
        }

        // PosTest8: the specified time is February in common year, years added is a normal value
        [Fact]
        public void PosTest8()
        {
            DateTime initialTime;
            int years;
            DateTime resultingTime;
            System.Globalization.Calendar myCalendar = new GregorianCalendar(GregorianCalendarTypes.USEnglish);
            years = 48;
            initialTime = myCalendar.ToDateTime(1999, 2, 28, 10, 30, 24, 0);
            resultingTime = myCalendar.AddYears(initialTime, years);
            VerifyAddyearsResult(myCalendar, initialTime, resultingTime, years);
        }

        #endregion
        #region Helper methods for positive tests
        private void VerifyAddyearsResult(Calendar calendar, DateTime oldTime, DateTime newTime, int years)
        {
            int oldYear = calendar.GetYear(oldTime);
            int oldMonth = calendar.GetMonth(oldTime);
            int oldDay = calendar.GetDayOfMonth(oldTime);
            long oldTicksOfDay = oldTime.Ticks % TimeSpan.TicksPerDay;
            int newYear = calendar.GetYear(newTime);
            int newMonth = calendar.GetMonth(newTime);
            int newDay = calendar.GetDayOfMonth(newTime);
            long newTicksOfDay = newTime.Ticks % TimeSpan.TicksPerDay;
            Assert.Equal(oldTicksOfDay, newTicksOfDay);
            Assert.False(newDay > oldDay);
            Assert.False(newYear != oldYear + years);
        }

        #endregion
        #region Helper method for all the tests
        private string GetParamesInfo(DateTime time, int years)
        {
            string str = string.Empty;
            str += string.Format("\nThe initial time is {0}, number of years added is {1}.", time, years);
            return str;
        }
        #endregion
    }
}
