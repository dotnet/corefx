// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace System.Globalization
{
    public static class ISOWeekTests
    {
        // The following 71 years in a 400-year cycle have 53 weeks (371 days).
        // Years not listed have 52 weeks (364 days). Add 2000 for current years.
        private static readonly HashSet<int> s_YearsWith53Weeks = new HashSet<int>
        {
            004, 009, 015, 020, 026,
            032, 037, 043, 048, 054,
            060, 065, 071, 076, 082,
            088, 093, 099, 105, 111, 116, 122,
            128, 133, 139, 144, 150,
            156, 161, 167, 172, 178,
            184, 189, 195, 201, 207, 212, 218,
            224, 229, 235, 240, 246,
            252, 257, 263, 268, 274,
            280, 285, 291, 296, 303, 308, 314,
            320, 325, 331, 336, 342,
            348, 353, 359, 364, 370,
            376, 381, 387, 392, 398
        };

        // From https://en.wikipedia.org/wiki/ISO_week_date#Relation_with_the_Gregorian_calendar.
        private static readonly DateData[] s_DateData =
        {
            new DateData("0001-01-01", "0001-W01-1"),
            new DateData("2005-01-01", "2004-W53-6"),
            new DateData("2005-01-02", "2004-W53-7"),
            new DateData("2005-12-31", "2005-W52-6"),
            new DateData("2006-01-01", "2005-W52-7"),
            new DateData("2006-01-02", "2006-W01-1"),
            new DateData("2006-12-31", "2006-W52-7"),
            new DateData("2007-01-01", "2007-W01-1"),
            new DateData("2007-12-30", "2007-W52-7"),
            new DateData("2007-12-31", "2008-W01-1"),
            new DateData("2008-01-01", "2008-W01-2"),
            new DateData("2008-12-28", "2008-W52-7"),
            new DateData("2008-12-29", "2009-W01-1"),
            new DateData("2008-12-30", "2009-W01-2"),
            new DateData("2008-12-31", "2009-W01-3"),
            new DateData("2009-01-01", "2009-W01-4"),
            new DateData("2009-12-31", "2009-W53-4"),
            new DateData("2010-01-01", "2009-W53-5"),
            new DateData("2010-01-02", "2009-W53-6"),
            new DateData("2010-01-03", "2009-W53-7"),
            new DateData("9999-12-31", "9999-W52-5"),
        };

        private static readonly YearData[] s_YearData =
        {
            new YearData(2005, "2005-01-03", "2006-01-01"),
            new YearData(2006, "2006-01-02", "2006-12-31"),
            new YearData(2007, "2007-01-01", "2007-12-30"),
            new YearData(2008, "2007-12-31", "2008-12-28"),
            new YearData(2009, "2008-12-29", "2010-01-03"),
        };

        public static IEnumerable<object[]> WeeksInYear_TestData()
        {
            for (int year = 0; year <= 400; year++)
            {
                bool isLong = s_YearsWith53Weeks.Contains(year);
                yield return new object[] { 2000 + year, isLong };
            }
        }

        public static IEnumerable<object[]> GetWeekOfYear_TestData()
        {
            return s_DateData.Select(x => new object[] {x.Date, x.Week});
        }

        public static IEnumerable<object[]> GetYear_TestData()
        {
            return s_DateData.Select(x => new object[] {x.Date, x.Year});
        }

        public static IEnumerable<object[]> ToDateTime_TestData()
        {
            return s_DateData.Select(x => new object[] {x.Year, x.Week, x.DayOfWeek, x.Date});
        }

        public static IEnumerable<object[]> GetYearStart_TestData()
        {
            return s_YearData.Select(x => new object[] {x.Year, x.StartDate});
        }

        public static IEnumerable<object[]> GetYearEnd_TestData()
        {
            return s_YearData.Select(x => new object[] {x.Year, x.EndDate});
        }

        [Theory, MemberData(nameof(GetWeekOfYear_TestData))]
        public static void GetWeekOfYear(DateTime date, int expected)
        {
            Assert.Equal(expected, ISOWeek.GetWeekOfYear(date));
        }

        [Theory, MemberData(nameof(GetYear_TestData))]
        public static void GetYear(DateTime date, int expected)
        {
            Assert.Equal(expected, ISOWeek.GetYear(date));
        }

        [Theory, MemberData(nameof(ToDateTime_TestData))]
        public static void ToDateTime(int year, int week, DayOfWeek dayOfWeek, DateTime expected)
        {
            Assert.Equal(expected, ISOWeek.ToDateTime(year, week, dayOfWeek));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(10000)]
        public static void ToDateTime_WithInvalidYear_Throws(int year)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => ISOWeek.ToDateTime(year, 1, DayOfWeek.Friday));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(54)]
        public static void ToDateTime_WithInvalidWeek_Throws(int week)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => ISOWeek.ToDateTime(2018, week, DayOfWeek.Friday));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(8)]
        public static void ToDateTime_WithInvalidDayOfWeek_Throws(int dayOfWeek)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => ISOWeek.ToDateTime(2018, 1, (DayOfWeek)dayOfWeek));
        }

        [Theory, MemberData(nameof(WeeksInYear_TestData))]
        public static void GetWeeksInYear(int year, bool isLong)
        {
            Assert.Equal(isLong ? 53 : 52, ISOWeek.GetWeeksInYear(year));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(10000)]
        public static void GetWeeksInYear_WithInvalidYear_Throws(int year)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => ISOWeek.GetWeeksInYear(year));
        }

        [Theory, MemberData(nameof(GetYearStart_TestData))]
        public static void GetYearStart(int year, DateTime expected)
        {
            Assert.Equal(expected, ISOWeek.GetYearStart(year));
        }

        [Theory, MemberData(nameof(GetYearEnd_TestData))]
        public static void GetYearEnd(int year, DateTime expected)
        {
            Assert.Equal(expected, ISOWeek.GetYearEnd(year));
        }

        private static DateTime ParseIsoDate(string date)
        {
            return DateTime.ParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        }

        private struct DateData
        {
            public readonly DateTime Date;

            public readonly int Year;

            public readonly int Week;

            public readonly DayOfWeek DayOfWeek;

            public DateData(string date, string week)
            {
                Date = ParseIsoDate(date);
                ParseWeekString(week, out Year, out Week, out DayOfWeek);
            }

            private static void ParseWeekString(string value, out int year, out int week, out DayOfWeek day)
            {
                var parts = value.Split('-');
                year = int.Parse(parts[0]);
                week = int.Parse(parts[1].Substring(1));
                day = (DayOfWeek) ((int.Parse(parts[2]) + 7) % 7);
            }
        }

        private struct YearData
        {
            public readonly int Year;

            public readonly DateTime StartDate;

            public readonly DateTime EndDate;

            public YearData(int year, string startDate, string endDate)
            {
                Year = year;
                StartDate = ParseIsoDate(startDate);
                EndDate = ParseIsoDate(endDate);
            }
        }
    }
}
