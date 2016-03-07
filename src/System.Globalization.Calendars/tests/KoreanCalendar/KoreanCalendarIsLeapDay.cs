// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using Xunit;

namespace System.Globalization.CalendarsTests
{
    //System.Globalization.KoreanCalendar.IsLeapDay(System.Int32,System.Int32,System.Int32,System.Int32)
    public class KoreanCalendarIsLeapDay
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
            int month = dateTime.Month;
            int day = dateTime.Day;
            int era = gC.GetEra(dateTime);
            bool expectedValue = gC.IsLeapDay(year, month, day, era);
            bool actualValue;
            actualValue = kC.IsLeapDay(year + 2333, month, day, kC.GetEra(dateTime));
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
            int month = dateTime.Month;
            int day = dateTime.Day;
            int era = gC.GetEra(dateTime);
            bool expectedValue = gC.IsLeapDay(year, month, day, era);
            bool actualValue;
            actualValue = kC.IsLeapDay(year + 2333, month, day, kC.GetEra(dateTime));
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
            int month = dateTime.Month;
            int day = dateTime.Day;
            int era = gC.GetEra(dateTime);
            bool expectedValue = gC.IsLeapDay(year, month, day, era);
            bool actualValue;
            actualValue = kC.IsLeapDay(year + 2333, month, day, kC.GetEra(dateTime));
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
            int month = dateTime.Month;
            int day = dateTime.Day;
            int era = gC.GetEra(dateTime);
            bool expectedValue = gC.IsLeapDay(year, month, day, era);
            bool actualValue;
            actualValue = kC.IsLeapDay(year + 2333, month, day, kC.GetEra(dateTime));
            Assert.Equal(expectedValue, actualValue);
        }
        #endregion

        [Fact]
        public void IsLeapDay_Invalid()
        {
            // Year is outside supported range
            Assert.Throws<ArgumentOutOfRangeException>(() => new KoreanCalendar().IsLeapDay(1, 10, 10, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new KoreanCalendar().IsLeapDay(-1, 10, 10, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new KoreanCalendar().IsLeapDay(2333, 10, 10, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new KoreanCalendar().IsLeapDay(12333, 10, 10, 1));

            // Month is outside supported range
            Assert.Throws<ArgumentOutOfRangeException>(() => new KoreanCalendar().IsLeapDay(2334, -1, 10, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new KoreanCalendar().IsLeapDay(2334, 0, 10, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new KoreanCalendar().IsLeapDay(2334, 13, 10, 1));

            // Day is outside supported range
            Assert.Throws<ArgumentOutOfRangeException>(() => new KoreanCalendar().IsLeapDay(2334, 10, 10, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new KoreanCalendar().IsLeapDay(2334, 10, 0, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new KoreanCalendar().IsLeapDay(2334, 10, 10, 32));

            // Era is outside supported range
            Assert.Throws<ArgumentOutOfRangeException>(() => new KoreanCalendar().IsLeapDay(2334, 10, 10, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new KoreanCalendar().IsLeapDay(2334, 10, 10, 2));
        }
    }
}
