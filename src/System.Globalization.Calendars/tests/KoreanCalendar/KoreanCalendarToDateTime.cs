// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using Xunit;

namespace System.Globalization.CalendarsTests
{
    public class KoreanCalendarToDateTime
    {
        private readonly RandomDataGenerator _generator = new RandomDataGenerator();

        #region Positive Test Logic
        // PosTest1:Invoke the mthod with min datetime
        [Fact]
        public void PosTest1()
        {
            System.Globalization.Calendar kC = new KoreanCalendar();
            DateTime dateTime = DateTime.MinValue;
            DateTime expectedValue = dateTime;
            DateTime actualValue;
            actualValue = kC.ToDateTime(dateTime.Year + 2333, dateTime.Month, dateTime.Day,
                        dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond, kC.GetEra(dateTime));
            Assert.Equal(expectedValue, actualValue);
        }

        // PosTest2:Invoke the mthod with max datetime
        [Fact]
        public void PosTest2()
        {
            System.Globalization.Calendar kC = new KoreanCalendar();
            DateTime dateTime = DateTime.MaxValue;
            DateTime expectedValue = new GregorianCalendar().ToDateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond);
            DateTime actualValue;
            actualValue = kC.ToDateTime(dateTime.Year + 2333, dateTime.Month, dateTime.Day,
                dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond, kC.GetEra(dateTime));
            Assert.Equal(expectedValue, actualValue);
        }

        // PosTest3:Invoke the mthod with leap year datetime
        [Fact]
        public void PosTest3()
        {
            System.Globalization.Calendar kC = new KoreanCalendar();
            DateTime dateTime = new GregorianCalendar().ToDateTime(2004, 2, 29, 1, 1, 1, 0);
            DateTime expectedValue = dateTime;
            DateTime actualValue;
            actualValue = kC.ToDateTime(dateTime.Year + 2333, dateTime.Month, dateTime.Day,
                dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond, kC.GetEra(dateTime));
            Assert.Equal(expectedValue, actualValue);
        }

        // PosTest4:Invoke the mthod with random datetime
        [Fact]
        public void PosTest4()
        {
            System.Globalization.Calendar kC = new KoreanCalendar();
            DateTime dateTime = new DateTime(_generator.GetInt64(-55) % (DateTime.MaxValue.Ticks + 1));
            dateTime = new GregorianCalendar().ToDateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, 0);
            DateTime expectedValue = dateTime;
            DateTime actualValue;
            actualValue = kC.ToDateTime(dateTime.Year + 2333, dateTime.Month, dateTime.Day,
                dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond, kC.GetEra(dateTime));
            Assert.Equal(expectedValue, actualValue);
        }
        #endregion

        [Fact]
        public void ToDateTime_Invalid()
        {
            // Year is outside supported range
            Assert.Throws<ArgumentOutOfRangeException>(() => new KoreanCalendar().ToDateTime(-1, 10, 10, 10, 10, 10, 10, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new KoreanCalendar().ToDateTime(0, 10, 10, 10, 10, 10, 10, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new KoreanCalendar().ToDateTime(2333, 10, 10, 10, 10, 10, 10, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new KoreanCalendar().ToDateTime(12333, 10, 10, 10, 10, 10, 10, 1));

            // Month is outside supported range
            Assert.Throws<ArgumentOutOfRangeException>(() => new KoreanCalendar().ToDateTime(2334, -1, 10, 10, 10, 10, 10, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new KoreanCalendar().ToDateTime(2334, 0, 10, 10, 10, 10, 10, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new KoreanCalendar().ToDateTime(2334, 13, 10, 10, 10, 10, 10, 1));

            // Day is outside supported range
            Assert.Throws<ArgumentOutOfRangeException>(() => new KoreanCalendar().ToDateTime(2334, 10, -1, 10, 10, 10, 10, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new KoreanCalendar().ToDateTime(2334, 10, 0, 10, 10, 10, 10, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new KoreanCalendar().ToDateTime(2334, 10, 32, 10, 10, 10, 10, 1));

            // Hour is outside supported range
            Assert.Throws<ArgumentOutOfRangeException>(() => new KoreanCalendar().ToDateTime(2334, 10, 10, -1, 10, 10, 10, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new KoreanCalendar().ToDateTime(2334, 10, 10, 24, 10, 10, 10, 1));

            // Minute is outside supported range
            Assert.Throws<ArgumentOutOfRangeException>(() => new KoreanCalendar().ToDateTime(2334, 10, 10, -1, 10, 10, 10, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new KoreanCalendar().ToDateTime(2334, 10, 10, 60, 10, 10, 10, 1));

            // Second is outside supported range
            Assert.Throws<ArgumentOutOfRangeException>(() => new KoreanCalendar().ToDateTime(2334, 10, 10, 10, -1, 10, 10, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new KoreanCalendar().ToDateTime(2334, 10, 10, 10, 60, 10, 10, 1));

            // Millisecond is outside supported range
            Assert.Throws<ArgumentOutOfRangeException>(() => new KoreanCalendar().ToDateTime(2334, 10, 10, 10, 10, 10, 10, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new KoreanCalendar().ToDateTime(2334, 10, 10, 10, 10, 10, 10, 1000));

            // Era is outside supported range
            Assert.Throws<ArgumentOutOfRangeException>(() => new KoreanCalendar().ToDateTime(2334, 10, 10, 10, 10, 10, 10, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new KoreanCalendar().ToDateTime(2334, 10, 10, 10, 10, 10, 10, 2));
        }
    }
}
