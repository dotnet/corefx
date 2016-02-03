// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using Xunit;

// System.Globalization.ThaiBuddhistCalendar.ToDateTime(Int32,Int32,Int32,Int32,Int32,Int32,Int32,Int32)
public class ThaiBuddhistCalendarToDateTime2
{
    #region Positive Tests
    // PosTest1: Verify the year is a random year
    [Fact]
    public void PosTest1()
    {
        System.Globalization.Calendar tbc = new ThaiBuddhistCalendar();
        Random rand = new Random(-55);
        int year = rand.Next(tbc.MinSupportedDateTime.Year + 543, tbc.MaxSupportedDateTime.Year + 543);
        int month = rand.Next(1, 13);
        int day = rand.Next(1, 29);
        int hour = rand.Next(0, 24);
        int minute = rand.Next(0, 60);
        int second = rand.Next(0, 60);
        int milliSecond = rand.Next(0, 1000);
        int era = 0;
        for (int i = 0; i < tbc.Eras.Length; i++)
        {
            era = tbc.Eras[i];
            DateTime dt = tbc.ToDateTime(year, month, day, hour, minute, second, milliSecond);
            DateTime desiredDT = new DateTime(year - 543, month, day, hour, minute, second, milliSecond);
            Assert.Equal(dt, desiredDT);
        }
    }

    // PosTest2: Verify the DateTime is 9099-12-31 23:59:29:999
    [Fact]
    public void PosTest2()
    {
        System.Globalization.Calendar tbc = new ThaiBuddhistCalendar();
        int year = 9999 + 543;
        int month = 12;
        int day = 31;
        int hour = 23;
        int minute = 59;
        int second = 59;
        int milliSecond = 999;
        int era;
        for (int i = 0; i < tbc.Eras.Length; i++)
        {
            era = tbc.Eras[i];
            DateTime dt = tbc.ToDateTime(year, month, day, hour, minute, second, milliSecond);
            DateTime desireDT = new DateTime(year - 543, month, day, hour, minute, second, milliSecond);
            Assert.Equal(dt, desireDT);
        }
    }

    // PosTest3: Verify the DateTime is ThaiBuddhistCalendar MinSupportedDateTime
    [Fact]
    public void PosTest3()
    {
        System.Globalization.Calendar tbc = new ThaiBuddhistCalendar();
        DateTime minDT = tbc.MinSupportedDateTime;
        int year = 1 + 543;
        int month = 1;
        int day = 1;
        int hour = 0;
        int minute = 0;
        int second = 0;
        int milliSecond = 0;
        int era;
        for (int i = 0; i < tbc.Eras.Length; i++)
        {
            era = tbc.Eras[i];
            DateTime dt = tbc.ToDateTime(year, month, day, hour, minute, second, milliSecond);
            Assert.Equal(dt, minDT);
        }
    }
    #endregion

    #region Negative Tests
    // NegTest1: The year outside the range supported by the ThaiBuddhistCalendar
    [Fact]
    public void NegTest1()
    {
        System.Globalization.Calendar tbc = new ThaiBuddhistCalendar();
        Random rand = new Random(-55);
        int year = rand.Next(tbc.MaxSupportedDateTime.Year + 543, Int32.MaxValue);
        int month = rand.Next(1, 13);
        int day = rand.Next(1, 29);
        int hour = rand.Next(0, 24);
        int minute = rand.Next(0, 60);
        int second = rand.Next(0, 60);
        int milliSecond = rand.Next(0, 1000);
        int era = tbc.Eras[0];
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            tbc.ToDateTime(year, month, day, hour, minute, second, milliSecond, era);
        });

        year = rand.Next(Int32.MinValue, tbc.MinSupportedDateTime.Year + 543);
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            tbc.ToDateTime(year, month, day, hour, minute, second, milliSecond, era);
        });
    }

    // NegTest2: The month  outside the range supported by the ThaiBuddhistCalendar
    [Fact]
    public void NegTest2()
    {
        System.Globalization.Calendar tbc = new ThaiBuddhistCalendar();
        Random rand = new Random(-55);
        int year = rand.Next(tbc.MinSupportedDateTime.Year + 543, tbc.MaxSupportedDateTime.Year + 543);
        int month = rand.Next(Int32.MinValue, 1);
        int day = rand.Next(1, 29);
        int hour = rand.Next(0, 24);
        int minute = rand.Next(0, 60);
        int second = rand.Next(0, 60);
        int milliSecond = rand.Next(0, 1000);
        int era = tbc.Eras[0];
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            tbc.ToDateTime(year, month, day, hour, minute, second, milliSecond, era);
        });

        month = rand.Next(13, Int32.MaxValue);
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            tbc.ToDateTime(year, month, day, hour, minute, second, milliSecond, era);
        });
    }

    // NegTest3: The day outside the range supported by the ThaiBuddhistCalendar
    [Fact]
    public void NegTest3()
    {
        System.Globalization.Calendar tbc = new ThaiBuddhistCalendar();
        Random rand = new Random(-55);
        int year = rand.Next(tbc.MinSupportedDateTime.Year + 543, tbc.MaxSupportedDateTime.Year + 543);
        int month = rand.Next(1, 13);
        int day = rand.Next(Int32.MinValue, 1);
        int hour = rand.Next(0, 24);
        int minute = rand.Next(0, 60);
        int second = rand.Next(0, 60);
        int milliSecond = rand.Next(0, 1000);
        int era = tbc.Eras[0];
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            tbc.ToDateTime(year, month, day, hour, minute, second, milliSecond, era);
        });

        day = rand.Next(tbc.GetDaysInMonth(year, month, era) + 1, Int32.MaxValue);
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            tbc.ToDateTime(year, month, day, hour, minute, second, milliSecond, era);
        });
    }

    // NegTest4: The hour is less than zero or greater than 23
    [Fact]
    public void NegTest4()
    {
        System.Globalization.Calendar tbc = new ThaiBuddhistCalendar();
        Random rand = new Random(-55);
        int year = rand.Next(tbc.MinSupportedDateTime.Year + 543, tbc.MaxSupportedDateTime.Year + 543);
        int month = rand.Next(1, 13);
        int day = rand.Next(1, 29);
        int hour = rand.Next(Int32.MinValue, 0);
        int minute = rand.Next(0, 60);
        int second = rand.Next(0, 60);
        int milliSecond = rand.Next(0, 1000);
        int era = tbc.Eras[0];
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            tbc.ToDateTime(year, month, day, hour, minute, second, milliSecond, era);
        });

        //hour greater than 23;
        hour = rand.Next(24, Int32.MaxValue);
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            tbc.ToDateTime(year, month, day, hour, minute, second, milliSecond, era);
        });
    }

    // NegTest5: The minute is less than zero or greater than 59
    [Fact]
    public void NegTest5()
    {
        System.Globalization.Calendar tbc = new ThaiBuddhistCalendar();
        Random rand = new Random(-55);
        int year = rand.Next(tbc.MinSupportedDateTime.Year + 543, tbc.MaxSupportedDateTime.Year + 543);
        int month = rand.Next(1, 13);
        int day = rand.Next(1, 29);
        int hour = rand.Next(0, 24);
        int minute = rand.Next(Int32.MinValue, 0);
        int second = rand.Next(0, 60);
        int milliSecond = rand.Next(0, 1000);
        int era = tbc.Eras[0];
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            tbc.ToDateTime(year, month, day, hour, minute, second, milliSecond, era);
        });

        //minute greater than 59;
        minute = rand.Next(60, Int32.MaxValue);
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            tbc.ToDateTime(year, month, day, hour, minute, second, milliSecond, era);
        });
    }

    // NegTest6: The second is less than zero or greater than 59
    [Fact]
    public void NegTest6()
    {
        System.Globalization.Calendar tbc = new ThaiBuddhistCalendar();
        Random rand = new Random(-55);
        int year = rand.Next(tbc.MinSupportedDateTime.Year + 543, tbc.MaxSupportedDateTime.Year + 543);
        int month = rand.Next(1, 13);
        int day = rand.Next(1, 29);
        int hour = rand.Next(0, 24);
        int minute = rand.Next(0, 60);
        int second = rand.Next(Int32.MinValue, 0);
        int milliSecond = rand.Next(0, 1000);
        int era = tbc.Eras[0];
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            tbc.ToDateTime(year, month, day, hour, minute, second, milliSecond, era);
        });

        //second greater than 59;
        second = rand.Next(60, Int32.MaxValue);
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            tbc.ToDateTime(year, month, day, hour, minute, second, milliSecond, era);
        });
    }

    // NegTest7: The milliSecond is less than zero or greater than 999
    [Fact]
    public void NegTest7()
    {
        System.Globalization.Calendar tbc = new ThaiBuddhistCalendar();
        Random rand = new Random(-55);
        int year = rand.Next(tbc.MinSupportedDateTime.Year + 543, tbc.MaxSupportedDateTime.Year + 543);
        int month = rand.Next(1, 13);
        int day = rand.Next(1, 29);
        int hour = rand.Next(0, 24);
        int minute = rand.Next(0, 60);
        int second = rand.Next(0, 60);
        int milliSecond = rand.Next(Int32.MinValue, 0);
        int era = tbc.Eras[0];
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            tbc.ToDateTime(year, month, day, hour, minute, second, milliSecond, era);
        });

        //milliSecond greater than 999;
        second = rand.Next(1000, Int32.MaxValue);
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            tbc.ToDateTime(year, month, day, hour, minute, second, milliSecond, era);
        });
    }

    #endregion
}
