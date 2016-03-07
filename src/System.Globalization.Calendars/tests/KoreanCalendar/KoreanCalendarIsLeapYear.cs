// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using Xunit;

namespace System.Globalization.CalendarsTests
{
    //System.Globalization.KoreanCalendar.IsLeapYear(System.Int32,System.Int32)
    public class KoreanCalendarIsLeapYear
    {
        private readonly RandomDataGenerator _generator = new RandomDataGenerator();

        #region Positive Test Logic
        // PosTest1:Invoke the method with min date time
        [Fact]
        public void PosTest1()
        {
            System.Globalization.Calendar kC = new KoreanCalendar();
            System.Globalization.Calendar gC = new GregorianCalendar();
            DateTime dateTime = gC.ToDateTime(1, 1, 1, 0, 0, 0, 0);
            int year = dateTime.Year;
            int era = gC.GetEra(dateTime);
            bool expectedValue = gC.IsLeapYear(year, era);
            bool actualValue;
            actualValue = kC.IsLeapYear(year + 2333, kC.GetEra(dateTime));
            Assert.Equal(expectedValue, actualValue);
        }

        // PosTest2:Invoke the method with max date time
        [Fact]
        public void PosTest2()
        {
            System.Globalization.Calendar kC = new KoreanCalendar();
            System.Globalization.Calendar gC = new GregorianCalendar();
            DateTime dateTime = gC.ToDateTime(9999, 12, 31, 0, 0, 0, 0);
            int year = dateTime.Year;
            int era = gC.GetEra(dateTime);
            bool expectedValue = gC.IsLeapYear(year, era);
            bool actualValue;
            actualValue = kC.IsLeapYear(year + 2333, kC.GetEra(dateTime));
            Assert.Equal(expectedValue, actualValue);
        }

        // PosTest3:Invoke the method with normal date time
        [Fact]
        public void PosTest3()
        {
            System.Globalization.Calendar kC = new KoreanCalendar();
            System.Globalization.Calendar gC = new GregorianCalendar();
            DateTime dateTime = gC.ToDateTime(1900, 2, 28, 0, 0, 0, 0);
            int year = dateTime.Year;
            int era = gC.GetEra(dateTime);
            bool expectedValue = gC.IsLeapYear(year, era);
            bool actualValue;
            actualValue = kC.IsLeapYear(year + 2333, kC.GetEra(dateTime));
            Assert.Equal(expectedValue, actualValue);
        }

        // PosTest4:Invoke the method with leap day date time
        [Fact]
        public void PosTest4()
        {
            System.Globalization.Calendar kC = new KoreanCalendar();
            System.Globalization.Calendar gC = new GregorianCalendar();
            DateTime dateTime = gC.ToDateTime(1200, 2, 29, 0, 0, 0, 0);
            int year = dateTime.Year;
            int era = gC.GetEra(dateTime);
            bool expectedValue = gC.IsLeapYear(year, era);
            bool actualValue;
            actualValue = kC.IsLeapYear(year + 2333, kC.GetEra(dateTime));
            Assert.Equal(expectedValue, actualValue);
        }
        #endregion

        [Fact]
        public void IsLeapYear_Invalid()
        {
            // Year is outside supported range
            Assert.Throws<ArgumentOutOfRangeException>(() => new KoreanCalendar().IsLeapYear(1, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new KoreanCalendar().IsLeapYear(-1, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new KoreanCalendar().IsLeapYear(2333, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new KoreanCalendar().IsLeapYear(12333, 1));

            // Era is outside supported range
            Assert.Throws<ArgumentOutOfRangeException>(() => new KoreanCalendar().IsLeapYear(2334, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new KoreanCalendar().IsLeapYear(2334, 2));
        }
    }
}
