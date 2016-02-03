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

        #region Negative Test Logic
        // NegTest1:Invoke the method with the year outside the lower supported range
        [Fact]
        public void NegTest1()
        {
            System.Globalization.Calendar kC = new KoreanCalendar();
            int year = 2333;
            int month = _generator.GetInt16(-55) % 12 + 1;
            int day = _generator.GetInt16(-55) % 28 + 1;
            int era = 1;
            bool actualValue;
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                actualValue = kC.IsLeapDay(year, month, day, era);
            });
        }

        // NegTest2:Invoke the method with the year outside the lower supported range
        [Fact]
        public void NegTest2()
        {
            System.Globalization.Calendar kC = new KoreanCalendar();
            int year = 0;
            int month = _generator.GetInt16(-55) % 12 + 1;
            int day = _generator.GetInt16(-55) % 28 + 1;
            int era = 1;
            bool actualValue;
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                actualValue = kC.IsLeapDay(year, month, day, era);
            });
        }

        // NegTest3:Invoke the method with the year outside the upper supported range
        [Fact]
        public void NegTest3()
        {
            System.Globalization.Calendar kC = new KoreanCalendar();
            int year = 2333;
            int month = _generator.GetInt16(-55) % 12 + 1;
            int day = _generator.GetInt16(-55) % 28 + 1;
            int era = 1;
            bool actualValue;
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                actualValue = kC.IsLeapDay(year, month, day, era);
            });
        }

        // NegTest4:Invoke the method with the month outside the lower supported range
        [Fact]
        public void NegTest4()
        {
            System.Globalization.Calendar kC = new KoreanCalendar();
            int year = _generator.GetInt16(-55) % 9999 + 2334;
            int month = 0;
            int day = _generator.GetInt16(-55) % 28 + 1;
            int era = 1;
            bool actualValue;
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                actualValue = kC.IsLeapDay(year, month, day, era);
            });
        }

        // NegTest5:Invoke the method with the month outside the upper supported range
        [Fact]
        public void NegTest5()
        {
            System.Globalization.Calendar kC = new KoreanCalendar();
            int year = _generator.GetInt16(-55) % 9999 + 2334;
            int month = 13;
            int day = _generator.GetInt16(-55) % 28 + 1;
            int era = 1;
            bool actualValue;
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                actualValue = kC.IsLeapDay(year, month, day, era);
            });
        }

        // NegTest6:Invoke the method with the day outside the lower supported range
        [Fact]
        public void NegTest6()
        {
            System.Globalization.Calendar kC = new KoreanCalendar();
            int year = _generator.GetInt16(-55) % 9999 + 2334;
            int month = _generator.GetInt16(-55) % 12 + 1;
            int day = 0;
            int era = 1;
            bool actualValue;
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                actualValue = kC.IsLeapDay(year, month, day, era);
            });
        }

        // NegTest7:Invoke the method with the wrong leap day
        [Fact]
        public void NegTest7()
        {
            System.Globalization.Calendar kC = new KoreanCalendar();
            int year = 4004;
            int month = 2;
            int day = 29;
            int era = 1;
            bool actualValue;
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                actualValue = kC.IsLeapDay(year, month, day, era);
            });
        }

        // NegTest8:Invoke the method with the day outside the upper supported range
        [Fact]
        public void NegTest8()
        {
            System.Globalization.Calendar kC = new KoreanCalendar();
            int year = _generator.GetInt16(-55) % 9999 + 2334;
            int month = _generator.GetInt16(-55) % 12 + 1;
            int day = 32;
            int era = 1;
            bool actualValue;
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                actualValue = kC.IsLeapDay(year, month, day, era);
            });
        }

        // NegTest9:Invoke the method with the era outside the lower supported range
        [Fact]
        public void NegTest9()
        {
            System.Globalization.Calendar kC = new KoreanCalendar();
            int year = _generator.GetInt16(-55) % 9999 + 2334;
            int month = _generator.GetInt16(-55) % 12 + 1;
            int day = _generator.GetInt16(-55) % 28 + 1;
            // The KoreanEra is 1, however using an Era value of 0 defaults to "current era" for the calendar being used. In order to force
            // the ArgumentOutOfRangeException the era must not be 0 or 1
            int era = -1;
            bool actualValue;
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                actualValue = kC.IsLeapDay(year, month, day, era);
            });
        }

        // NegTest10:Invoke the method with the era outside the upper supported range
        [Fact]
        public void NegTest10()
        {
            System.Globalization.Calendar kC = new KoreanCalendar();
            int year = _generator.GetInt16(-55) % 9999 + 2334;
            int month = _generator.GetInt16(-55) % 12 + 1;
            int day = _generator.GetInt16(-55) % 28 + 1;
            int era = 2;
            bool actualValue;
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                actualValue = kC.IsLeapDay(year, month, day, era);
            });
        }
        #endregion
    }
}
