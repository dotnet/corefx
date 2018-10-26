// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Xunit;

namespace System.Tests
{
    public static partial class DateTimeTests
    {
        [Fact]
        public static void MaxValue()
        {
            VerifyDateTime(DateTime.MaxValue, 9999, 12, 31, 23, 59, 59, 999, DateTimeKind.Unspecified);
        }

        [Fact]
        public static void MinValue()
        {
            VerifyDateTime(DateTime.MinValue, 1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified);
        }

        [Fact]
        public static void Ctor_Long()
        {
            VerifyDateTime(new DateTime(999999999999999999), 3169, 11, 16, 9, 46, 39, 999, DateTimeKind.Unspecified);
        }

        [Fact]
        public static void Ctor_Long_InvalidTicks_ThrowsArgumentOutOfRangeException()
        {
            AssertExtensions.Throws<ArgumentOutOfRangeException>("ticks", () => new DateTime(DateTime.MinValue.Ticks - 1)); // Ticks < DateTime.MinValue.Ticks
            AssertExtensions.Throws<ArgumentOutOfRangeException>("ticks", () => new DateTime(DateTime.MaxValue.Ticks + 1)); // Ticks > DateTime.MaxValue.Ticks
        }

        [Fact]
        public static void Ctor_Long_DateTimeKind()
        {
            VerifyDateTime(new DateTime(999999999999999999, DateTimeKind.Utc), 3169, 11, 16, 9, 46, 39, 999, DateTimeKind.Utc);
        }

        [Fact]
        public static void Ctor_Long_DateTimeKind_Invalid()
        {
            AssertExtensions.Throws<ArgumentOutOfRangeException>("ticks", () => new DateTime(DateTime.MinValue.Ticks - 1, DateTimeKind.Utc)); // Ticks < DateTime.MinValue.Ticks
            AssertExtensions.Throws<ArgumentOutOfRangeException>("ticks", () => new DateTime(DateTime.MaxValue.Ticks + 1, DateTimeKind.Utc)); // Ticks > DateTime.MaxValue.Ticks

            AssertExtensions.Throws<ArgumentException>("kind", () => new DateTime(0, DateTimeKind.Unspecified - 1)); // Invalid date time kind
            AssertExtensions.Throws<ArgumentException>("kind", () => new DateTime(0, DateTimeKind.Local + 1)); // Invalid date time kind
        }

        [Fact]
        public static void Ctor_Int_Int_Int()
        {
            var dateTime = new DateTime(2012, 6, 11);
            VerifyDateTime(dateTime, 2012, 6, 11, 0, 0, 0, 0, DateTimeKind.Unspecified);
        }

        [Fact]
        public static void Ctor_Int_Int_Int_Invalid()
        {
            AssertExtensions.Throws<ArgumentOutOfRangeException>(null, () => new DateTime(0, 1, 1)); // Year < 0
            AssertExtensions.Throws<ArgumentOutOfRangeException>(null, () => new DateTime(10000, 1, 1)); // Year > 9999

            AssertExtensions.Throws<ArgumentOutOfRangeException>(null, () => new DateTime(1, 0, 1)); // Month < 1
            AssertExtensions.Throws<ArgumentOutOfRangeException>(null, () => new DateTime(1, 13, 1)); // Month > 12

            AssertExtensions.Throws<ArgumentOutOfRangeException>(null, () => new DateTime(1, 1, 0)); // Day < 1
            AssertExtensions.Throws<ArgumentOutOfRangeException>(null, () => new DateTime(1, 1, 32)); // Day > days in month
        }

        [Fact]
        public static void Ctor_Int_Int_Int_Int_Int_Int()
        {
            var dateTime = new DateTime(2012, 12, 31, 13, 50, 10);
            VerifyDateTime(dateTime, 2012, 12, 31, 13, 50, 10, 0, DateTimeKind.Unspecified);
        }

        [Fact]
        public static void Ctor_Int_Int_Int_Int_Int_Int_Invalid()
        {
            AssertExtensions.Throws<ArgumentOutOfRangeException>(null, () => new DateTime(0, 1, 1, 1, 1, 1)); // Year < 1
            AssertExtensions.Throws<ArgumentOutOfRangeException>(null, () => new DateTime(10000, 1, 1, 1, 1, 1)); // Year > 9999

            AssertExtensions.Throws<ArgumentOutOfRangeException>(null, () => new DateTime(1, 0, 1, 1, 1, 1)); // Month < 1
            AssertExtensions.Throws<ArgumentOutOfRangeException>(null, () => new DateTime(1, 13, 1, 1, 1, 1)); // Month > 12

            AssertExtensions.Throws<ArgumentOutOfRangeException>(null, () => new DateTime(1, 1, 0, 1, 1, 1)); // Day < 1
            AssertExtensions.Throws<ArgumentOutOfRangeException>(null, () => new DateTime(1, 1, 32, 1, 1, 1)); // Day > days in month

            AssertExtensions.Throws<ArgumentOutOfRangeException>(null, () => new DateTime(1, 1, 1, -1, 1, 1)); // Hour < 0
            AssertExtensions.Throws<ArgumentOutOfRangeException>(null, () => new DateTime(1, 1, 1, 24, 1, 1)); // Hour > 23

            AssertExtensions.Throws<ArgumentOutOfRangeException>(null, () => new DateTime(1, 1, 1, 1, -1, 1)); // Minute < 0
            AssertExtensions.Throws<ArgumentOutOfRangeException>(null, () => new DateTime(1, 1, 1, 1, 60, 1)); // Minute > 59

            AssertExtensions.Throws<ArgumentOutOfRangeException>(null, () => new DateTime(1, 1, 1, 1, 1, -1)); // Second < 0
            AssertExtensions.Throws<ArgumentOutOfRangeException>(null, () => new DateTime(1, 1, 1, 1, 1, 60)); // Second > 59
        }

        [Fact]
        public static void Ctor_Int_Int_Int_Int_Int_Int_Int_DateTimeKind()
        {
            var dateTime = new DateTime(1986, 8, 15, 10, 20, 5, DateTimeKind.Local);
            VerifyDateTime(dateTime, 1986, 8, 15, 10, 20, 5, 0, DateTimeKind.Local);
        }

        [Fact]
        public static void Ctor_Int_Int_Int_Int_Int_Int_DateTimeKind_Invalid()
        {
            AssertExtensions.Throws<ArgumentOutOfRangeException>(null, () => new DateTime(0, 1, 1, 1, 1, 1, DateTimeKind.Utc)); // Year < 1
            AssertExtensions.Throws<ArgumentOutOfRangeException>(null, () => new DateTime(10000, 1, 1, 1, 1, 1, DateTimeKind.Utc)); // Year > 9999

            AssertExtensions.Throws<ArgumentOutOfRangeException>(null, () => new DateTime(1, 0, 1, 1, 1, 1, DateTimeKind.Utc)); // Month < 1
            AssertExtensions.Throws<ArgumentOutOfRangeException>(null, () => new DateTime(1, 13, 1, 1, 1, 1, DateTimeKind.Utc)); // Month > 12

            AssertExtensions.Throws<ArgumentOutOfRangeException>(null, () => new DateTime(1, 1, 0, 1, 1, 1, DateTimeKind.Utc)); // Day < 1
            AssertExtensions.Throws<ArgumentOutOfRangeException>(null, () => new DateTime(1, 1, 32, 1, 1, 1, DateTimeKind.Utc)); // Day > days in month

            AssertExtensions.Throws<ArgumentOutOfRangeException>(null, () => new DateTime(1, 1, 1, -1, 1, 1, DateTimeKind.Utc)); // Hour < 0
            AssertExtensions.Throws<ArgumentOutOfRangeException>(null, () => new DateTime(1, 1, 1, 24, 1, 1, DateTimeKind.Utc)); // Hour > 23

            AssertExtensions.Throws<ArgumentOutOfRangeException>(null, () => new DateTime(1, 1, 1, 1, -1, 1, DateTimeKind.Utc)); // Minute < 0
            AssertExtensions.Throws<ArgumentOutOfRangeException>(null, () => new DateTime(1, 1, 1, 1, 60, 1, DateTimeKind.Utc)); // Minute > 59

            AssertExtensions.Throws<ArgumentOutOfRangeException>(null, () => new DateTime(1, 1, 1, 1, 1, -1, DateTimeKind.Utc)); // Second < 0
            AssertExtensions.Throws<ArgumentOutOfRangeException>(null, () => new DateTime(1, 1, 1, 1, 1, 60, DateTimeKind.Utc)); // Second > 59

            AssertExtensions.Throws<ArgumentException>("kind", () => new DateTime(1, 1, 1, 1, 1, 1, DateTimeKind.Unspecified - 1)); // Invalid date time kind
            AssertExtensions.Throws<ArgumentException>("kind", () => new DateTime(1, 1, 1, 1, 1, 1, DateTimeKind.Local + 1)); // Invalid date time kind
        }

        [Fact]
        public static void Ctor_Int_Int_Int_Int_Int_Int_Int()
        {
            var dateTime = new DateTime(1973, 10, 6, 14, 30, 0, 500);
            VerifyDateTime(dateTime, 1973, 10, 6, 14, 30, 0, 500, DateTimeKind.Unspecified);
        }

        [Fact]
        public static void Ctor_Int_Int_Int_Int_Int_Int_Int_Invalid()
        {
            AssertExtensions.Throws<ArgumentOutOfRangeException>(null, () => new DateTime(0, 1, 1, 1, 1, 1, 1)); // Year < 1
            AssertExtensions.Throws<ArgumentOutOfRangeException>(null, () => new DateTime(10000, 1, 1, 1, 1, 1, 1)); // Year > 9999

            AssertExtensions.Throws<ArgumentOutOfRangeException>(null, () => new DateTime(1, 0, 1, 1, 1, 1, 1)); // Month < 1
            AssertExtensions.Throws<ArgumentOutOfRangeException>(null, () => new DateTime(1, 13, 1, 1, 1, 1, 1)); // Month > 12

            AssertExtensions.Throws<ArgumentOutOfRangeException>(null, () => new DateTime(1, 1, 0, 1, 1, 1, 1)); // Day < 1
            AssertExtensions.Throws<ArgumentOutOfRangeException>(null, () => new DateTime(1, 1, 32, 1, 1, 1, 1)); // Day > days in month

            AssertExtensions.Throws<ArgumentOutOfRangeException>(null, () => new DateTime(1, 1, 1, -1, 1, 1, 1)); // Hour < 0
            AssertExtensions.Throws<ArgumentOutOfRangeException>(null, () => new DateTime(1, 1, 1, 24, 1, 1, 1)); // Hour > 23

            AssertExtensions.Throws<ArgumentOutOfRangeException>(null, () => new DateTime(1, 1, 1, 1, -1, 1, 1)); // Minute < 0
            AssertExtensions.Throws<ArgumentOutOfRangeException>(null, () => new DateTime(1, 1, 1, 1, 60, 1, 1)); // Minute > 59

            AssertExtensions.Throws<ArgumentOutOfRangeException>(null, () => new DateTime(1, 1, 1, 1, 1, -1, 1)); // Second < 0
            AssertExtensions.Throws<ArgumentOutOfRangeException>(null, () => new DateTime(1, 1, 1, 1, 1, 60, 1)); // Second > 59

            AssertExtensions.Throws<ArgumentOutOfRangeException>("millisecond", () => new DateTime(1, 1, 1, 1, 1, 1, -1)); // Milisecond < 0
            AssertExtensions.Throws<ArgumentOutOfRangeException>("millisecond", () => new DateTime(1, 1, 1, 1, 1, 1, 1000)); // Millisecond > 999
        }

        [Fact]
        public static void Ctor_Int_Int_Int_Int_Int_Int_Int_Int_DateTimeKind()
        {
            var dateTime = new DateTime(1986, 8, 15, 10, 20, 5, 600, DateTimeKind.Local);
            VerifyDateTime(dateTime, 1986, 8, 15, 10, 20, 5, 600, DateTimeKind.Local);
        }

        [Fact]
        public static void Ctor_Int_Int_Int_Int_Int_Int_Int_DateTimeKind_Invalid()
        {
            AssertExtensions.Throws<ArgumentOutOfRangeException>(null, () => new DateTime(0, 1, 1, 1, 1, 1, 1, DateTimeKind.Utc)); // Year < 1
            AssertExtensions.Throws<ArgumentOutOfRangeException>(null, () => new DateTime(10000, 1, 1, 1, 1, 1, 1, DateTimeKind.Utc)); // Year > 9999

            AssertExtensions.Throws<ArgumentOutOfRangeException>(null, () => new DateTime(1, 0, 1, 1, 1, 1, 1, DateTimeKind.Utc)); // Month < 0
            AssertExtensions.Throws<ArgumentOutOfRangeException>(null, () => new DateTime(1, 13, 1, 1, 1, 1, 1, DateTimeKind.Utc)); // Month > 12

            AssertExtensions.Throws<ArgumentOutOfRangeException>(null, () => new DateTime(1, 1, 0, 1, 1, 1, 1, DateTimeKind.Utc)); // Day < 0
            AssertExtensions.Throws<ArgumentOutOfRangeException>(null, () => new DateTime(1, 1, 32, 1, 1, 1, 1, DateTimeKind.Utc)); // Day > days in month

            AssertExtensions.Throws<ArgumentOutOfRangeException>(null, () => new DateTime(1, 1, 1, -1, 1, 1, 1, DateTimeKind.Utc)); // Hour < 0
            AssertExtensions.Throws<ArgumentOutOfRangeException>(null, () => new DateTime(1, 1, 1, 24, 1, 1, 1, DateTimeKind.Utc)); // Hour > 23

            AssertExtensions.Throws<ArgumentOutOfRangeException>(null, () => new DateTime(1, 1, 1, 1, -1, 1, 1, DateTimeKind.Utc)); // Minute < 0
            AssertExtensions.Throws<ArgumentOutOfRangeException>(null, () => new DateTime(1, 1, 1, 1, 60, 1, 1, DateTimeKind.Utc)); // Minute > 59

            AssertExtensions.Throws<ArgumentOutOfRangeException>(null, () => new DateTime(1, 1, 1, 1, 1, -1, 1, DateTimeKind.Utc)); // Second < 0
            AssertExtensions.Throws<ArgumentOutOfRangeException>(null, () => new DateTime(1, 1, 1, 1, 1, 60, 1, DateTimeKind.Utc)); // Second > 59

            AssertExtensions.Throws<ArgumentOutOfRangeException>("millisecond", () => new DateTime(1, 1, 1, 1, 1, 1, -1, DateTimeKind.Utc)); // Millisecond < 0
            AssertExtensions.Throws<ArgumentOutOfRangeException>("millisecond", () => new DateTime(1, 1, 1, 1, 1, 1, 1000, DateTimeKind.Utc)); // Millisecond > 999

            AssertExtensions.Throws<ArgumentException>("kind", () => new DateTime(1, 1, 1, 1, 1, 1, 1, DateTimeKind.Unspecified - 1)); // Invalid date time kind
            AssertExtensions.Throws<ArgumentException>("kind", () => new DateTime(1, 1, 1, 1, 1, 1, 1, DateTimeKind.Local + 1)); // Invalid date time kind
        }

        [Theory]
        [InlineData(2004, true)]
        [InlineData(2005, false)]
        public static void IsLeapYear(int year, bool expected)
        {
            Assert.Equal(expected, DateTime.IsLeapYear(year));
        }

        [Fact]
        public static void IsLeapYear_InvalidYear_ThrowsArgumentOutOfRangeException()
        {
            AssertExtensions.Throws<ArgumentOutOfRangeException>("year", () => DateTime.IsLeapYear(0));
            AssertExtensions.Throws<ArgumentOutOfRangeException>("year", () => DateTime.IsLeapYear(10000));
        }

        public static IEnumerable<object[]> Add_TimeSpan_TestData()
        {
            yield return new object[] { new DateTime(1000), new TimeSpan(10), new DateTime(1010) };
            yield return new object[] { new DateTime(1000), TimeSpan.Zero, new DateTime(1000) };
            yield return new object[] { new DateTime(1000), new TimeSpan(-10), new DateTime(990) };
        }

        [Theory]
        [MemberData(nameof(Add_TimeSpan_TestData))]
        public static void Add_TimeSpan(DateTime dateTime, TimeSpan timeSpan, DateTime expected)
        {
            Assert.Equal(expected, dateTime.Add(timeSpan));
        }

        [Fact]
        public static void Add_TimeSpan_NewDateOutOfRange_ThrowsArgumentOutOfRangeException()
        {
            AssertExtensions.Throws<ArgumentOutOfRangeException>("value", () => DateTime.MinValue.Add(TimeSpan.FromTicks(-1)));
            AssertExtensions.Throws<ArgumentOutOfRangeException>("value", () => DateTime.MaxValue.Add(TimeSpan.FromTicks(11)));
        }

        public static IEnumerable<object[]> AddYears_TestData()
        {
            yield return new object[] { new DateTime(1986, 8, 15, 10, 20, 5, 70), 10, new DateTime(1996, 8, 15, 10, 20, 5, 70) };
            yield return new object[] { new DateTime(1986, 8, 15, 10, 20, 5, 70), 0, new DateTime(1986, 8, 15, 10, 20, 5, 70) };
            yield return new object[] { new DateTime(1986, 8, 15, 10, 20, 5, 70), -10, new DateTime(1976, 8, 15, 10, 20, 5, 70) };
        }

        [Theory]
        [MemberData(nameof(AddYears_TestData))]
        public static void AddYears(DateTime dateTime, int years, DateTime expected)
        {
            Assert.Equal(expected, dateTime.AddYears(years));
        }

        [Fact]
        public static void AddYears_NewDateOutOfRange_ThrowsArgumentOutOfRangeException()
        {
            AssertExtensions.Throws<ArgumentOutOfRangeException>("years", () => DateTime.Now.AddYears(10001));
            AssertExtensions.Throws<ArgumentOutOfRangeException>("years", () => DateTime.Now.AddYears(-10001));

            AssertExtensions.Throws<ArgumentOutOfRangeException>("months", () => DateTime.MaxValue.AddYears(1));
            AssertExtensions.Throws<ArgumentOutOfRangeException>("months", () => DateTime.MinValue.AddYears(-1));
        }

        public static IEnumerable<object[]> AddMonths_TestData()
        {
            yield return new object[] { new DateTime(1986, 8, 15, 10, 20, 5, 70), 2, new DateTime(1986, 10, 15, 10, 20, 5, 70) };
            yield return new object[] { new DateTime(1986, 8, 15, 10, 20, 5, 70), 0, new DateTime(1986, 8, 15, 10, 20, 5, 70) };
            yield return new object[] { new DateTime(1986, 8, 15, 10, 20, 5, 70), -2, new DateTime(1986, 6, 15, 10, 20, 5, 70) };
        }

        [Theory]
        [MemberData(nameof(AddMonths_TestData))]
        public static void AddMonths(DateTime dateTime, int months, DateTime expected)
        {
            Assert.Equal(expected, dateTime.AddMonths(months));
        }

        [Fact]
        public static void AddMonths_NewDateOutOfRange_ThrowsArgumentOutOfRangeException()
        {
            AssertExtensions.Throws<ArgumentOutOfRangeException>("months", () => DateTime.Now.AddMonths(120001));
            AssertExtensions.Throws<ArgumentOutOfRangeException>("months", () => DateTime.Now.AddMonths(-120001));

            AssertExtensions.Throws<ArgumentOutOfRangeException>("months", () => DateTime.MaxValue.AddMonths(1));
            AssertExtensions.Throws<ArgumentOutOfRangeException>("months", () => DateTime.MinValue.AddMonths(-1));
        }

        public static IEnumerable<object[]> AddDays_TestData()
        {
            yield return new object[] { new DateTime(1986, 8, 15, 10, 20, 5, 70), 2, new DateTime(1986, 8, 17, 10, 20, 5, 70) };
            yield return new object[] { new DateTime(1986, 8, 15, 10, 20, 5, 70), 0, new DateTime(1986, 8, 15, 10, 20, 5, 70) };
            yield return new object[] { new DateTime(1986, 8, 15, 10, 20, 5, 70), -2, new DateTime(1986, 8, 13, 10, 20, 5, 70) };
        }

        [Theory]
        [MemberData(nameof(AddDays_TestData))]
        public static void AddDays(DateTime dateTime, double days, DateTime expected)
        {
            Assert.Equal(expected, dateTime.AddDays(days));
        }

        [Fact]
        public static void AddDays_NewDateOutOfRange_ThrowsArgumentOutOfRangeException()
        {
            AssertExtensions.Throws<ArgumentOutOfRangeException>("value", () => DateTime.MaxValue.AddDays(1));
            AssertExtensions.Throws<ArgumentOutOfRangeException>("value", () => DateTime.MinValue.AddDays(-1));
        }

        public static IEnumerable<object[]> AddHours_TestData()
        {
            yield return new object[] { new DateTime(1986, 8, 15, 10, 20, 5, 70), 3, new DateTime(1986, 8, 15, 13, 20, 5, 70) };
            yield return new object[] { new DateTime(1986, 8, 15, 10, 20, 5, 70), 0, new DateTime(1986, 8, 15, 10, 20, 5, 70) };
            yield return new object[] { new DateTime(1986, 8, 15, 10, 20, 5, 70), -3, new DateTime(1986, 8, 15, 7, 20, 5, 70) };
        }

        [Theory]
        [MemberData(nameof(AddHours_TestData))]
        public static void AddHours(DateTime dateTime, double hours, DateTime expected)
        {
            Assert.Equal(expected, dateTime.AddHours(hours));
        }

        [Fact]
        public static void AddHours_NewDateOutOfRange_ThrowsArgumentOutOfRangeException()
        {
            AssertExtensions.Throws<ArgumentOutOfRangeException>("value", () => DateTime.MaxValue.AddHours(1));
            AssertExtensions.Throws<ArgumentOutOfRangeException>("value", () => DateTime.MinValue.AddHours(-1));
        }

        public static IEnumerable<object[]> AddMinutes_TestData()
        {
            yield return new object[] { new DateTime(1986, 8, 15, 10, 20, 5, 70), 5, new DateTime(1986, 8, 15, 10, 25, 5, 70) };
            yield return new object[] { new DateTime(1986, 8, 15, 10, 20, 5, 70), 0, new DateTime(1986, 8, 15, 10, 20, 5, 70) };
            yield return new object[] { new DateTime(1986, 8, 15, 10, 20, 5, 70), -5, new DateTime(1986, 8, 15, 10, 15, 5, 70) };
        }

        [Theory]
        [MemberData(nameof(AddMinutes_TestData))]
        public static void AddMinutes(DateTime dateTime, double minutes, DateTime expected)
        {
            Assert.Equal(expected, dateTime.AddMinutes(minutes));
        }

        [Fact]
        public static void AddMinutes_NewDateOutOfRange_ThrowsArgumentOutOfRangeException()
        {
            AssertExtensions.Throws<ArgumentOutOfRangeException>("value", () => DateTime.MaxValue.AddMinutes(1));
            AssertExtensions.Throws<ArgumentOutOfRangeException>("value", () => DateTime.MinValue.AddMinutes(-1));
        }

        public static IEnumerable<object[]> AddSeconds_TestData()
        {
            yield return new object[] { new DateTime(1986, 8, 15, 10, 20, 5, 70), 30, new DateTime(1986, 8, 15, 10, 20, 35, 70) };
            yield return new object[] { new DateTime(1986, 8, 15, 10, 20, 5, 70), 0, new DateTime(1986, 8, 15, 10, 20, 5, 70) };
            yield return new object[] { new DateTime(1986, 8, 15, 10, 20, 5, 70), -3, new DateTime(1986, 8, 15, 10, 20, 2, 70) };
        }

        [Theory]
        [MemberData(nameof(AddSeconds_TestData))]
        public static void AddSeconds(DateTime dateTime, double seconds, DateTime expected)
        {
            Assert.Equal(expected, dateTime.AddSeconds(seconds));
        }

        [Fact]
        public static void AddSeconds_NewDateOutOfRange_ThrowsArgumentOutOfRangeException()
        {
            AssertExtensions.Throws<ArgumentOutOfRangeException>("value", () => DateTime.MaxValue.AddSeconds(1));
            AssertExtensions.Throws<ArgumentOutOfRangeException>("value", () => DateTime.MinValue.AddSeconds(-1));
        }

        public static IEnumerable<object[]> AddMilliseconds_TestData()
        {
            yield return new object[] { new DateTime(1986, 8, 15, 10, 20, 5, 70), 10, new DateTime(1986, 8, 15, 10, 20, 5, 80) };
            yield return new object[] { new DateTime(1986, 8, 15, 10, 20, 5, 70), 0, new DateTime(1986, 8, 15, 10, 20, 5, 70) };
            yield return new object[] { new DateTime(1986, 8, 15, 10, 20, 5, 70), -10, new DateTime(1986, 8, 15, 10, 20, 5, 60) };
        }

        [Theory]
        [MemberData(nameof(AddMilliseconds_TestData))]
        public static void AddMilliseconds(DateTime dateTime, double milliseconds, DateTime expected)
        {
            Assert.Equal(expected, dateTime.AddMilliseconds(milliseconds));
        }

        [Fact]
        public static void AddMilliseconds_NewDateOutOfRange_ThrowsArgumentOutOfRangeException()
        {
            AssertExtensions.Throws<ArgumentOutOfRangeException>("value", () => DateTime.MaxValue.AddMilliseconds(1));
            AssertExtensions.Throws<ArgumentOutOfRangeException>("value", () => DateTime.MinValue.AddMilliseconds(-1));
        }

        public static IEnumerable<object[]> AddTicks_TestData()
        {
            yield return new object[] { new DateTime(1000), 10, new DateTime(1010) };
            yield return new object[] { new DateTime(1000), 0, new DateTime(1000) };
            yield return new object[] { new DateTime(1000), -10, new DateTime(990) };
        }

        [Theory]
        [MemberData(nameof(AddTicks_TestData))]
        public static void AddTicks(DateTime dateTime, long ticks, DateTime expected)
        {
            Assert.Equal(expected, dateTime.AddTicks(ticks));
        }

        [Fact]
        public static void AddTicks_NewDateOutOfRange_ThrowsArgumentOutOfRangeException()
        {
            AssertExtensions.Throws<ArgumentOutOfRangeException>("value", () => DateTime.MaxValue.AddTicks(1));
            AssertExtensions.Throws<ArgumentOutOfRangeException>("value", () => DateTime.MinValue.AddTicks(-1));
        }

        [Fact]
        public static void DayOfWeekTest()
        {
            var dateTime = new DateTime(2012, 6, 18);
            Assert.Equal(DayOfWeek.Monday, dateTime.DayOfWeek);
        }

        [Fact]
        public static void DayOfYear()
        {
            var dateTime = new DateTime(2012, 6, 18);
            Assert.Equal(170, dateTime.DayOfYear);
        }

        [Fact]
        public static void TimeOfDay()
        {
            var dateTime = new DateTime(2012, 6, 18, 10, 5, 1, 0);
            TimeSpan ts = dateTime.TimeOfDay;

            DateTime newDate = dateTime.Subtract(ts);
            Assert.Equal(new DateTime(2012, 6, 18, 0, 0, 0, 0).Ticks, newDate.Ticks);
            Assert.Equal(dateTime.Ticks, newDate.Add(ts).Ticks);
        }

        [Fact]
        public static void Today()
        {
            DateTime today = DateTime.Today;
            DateTime now = DateTime.Now;
            VerifyDateTime(today, now.Year, now.Month, now.Day, 0, 0, 0, 0, DateTimeKind.Local);

            today = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, DateTimeKind.Utc);
            Assert.Equal(DateTimeKind.Utc, today.Kind);
            Assert.False(today.IsDaylightSavingTime());
        }

        [Fact]
        public static void Conversion()
        {
            DateTime today = DateTime.Today;
            long dateTimeRaw = today.ToBinary();
            Assert.Equal(today, DateTime.FromBinary(dateTimeRaw));

            dateTimeRaw = today.ToUniversalTime().ToBinary();
            Assert.Equal(today.ToUniversalTime(), DateTime.FromBinary(dateTimeRaw));

            dateTimeRaw = today.ToFileTime();
            Assert.Equal(today, DateTime.FromFileTime(dateTimeRaw));

            dateTimeRaw = today.ToFileTimeUtc();
            Assert.Equal(today, DateTime.FromFileTimeUtc(dateTimeRaw).ToLocalTime());
        }

        public static IEnumerable<object[]> Subtract_TimeSpan_TestData()
        {
            var dateTime = new DateTime(2012, 6, 18, 10, 5, 1, 0, DateTimeKind.Utc);

            yield return new object[] { dateTime, new TimeSpan(10, 5, 1), new DateTime(2012, 6, 18, 0, 0, 0, 0, DateTimeKind.Utc) };
            yield return new object[] { dateTime, new TimeSpan(-10, -5, -1), new DateTime(2012, 6, 18, 20, 10, 2, 0, DateTimeKind.Utc) };
        }

        [Theory]
        [MemberData(nameof(Subtract_TimeSpan_TestData))]
        public static void Subtract_TimeSpan(DateTime dateTime, TimeSpan timeSpan, DateTime expected)
        {
            Assert.Equal(expected, dateTime - timeSpan);
            Assert.Equal(expected, dateTime.Subtract(timeSpan));
        }

        public static IEnumerable<object[]> Subtract_DateTime_TestData()
        {
            var dateTime1 = new DateTime(1996, 6, 3, 22, 15, 0, DateTimeKind.Utc);
            var dateTime2 = new DateTime(1996, 12, 6, 13, 2, 0, DateTimeKind.Utc);
            var dateTime3 = new DateTime(1996, 10, 12, 8, 42, 0, DateTimeKind.Utc);

            yield return new object[] { dateTime2, dateTime1, new TimeSpan(185, 14, 47, 0) };
            yield return new object[] { dateTime1, dateTime2, new TimeSpan(-185, -14, -47, 0) };
            yield return new object[] { dateTime1, dateTime2, new TimeSpan(-185, -14, -47, 0) };
        }

        [Theory]
        [MemberData(nameof(Subtract_DateTime_TestData))]
        public static void Subtract_DateTime(DateTime dateTime1, DateTime dateTime2, TimeSpan expected)
        {
            Assert.Equal(expected, dateTime1 - dateTime2);
            Assert.Equal(expected, dateTime1.Subtract(dateTime2));
        }

        [Fact]
        public static void Subtract_DateTime_Invalid()
        {
            DateTime date1 = DateTime.MinValue.ToLocalTime();
            Assert.Throws<ArgumentOutOfRangeException>(() => date1.Subtract(new TimeSpan(365, 0, 0, 0)));

            DateTime date2 = DateTime.MaxValue.ToLocalTime();
            Assert.Throws<ArgumentOutOfRangeException>(() => date2.Subtract(new TimeSpan(-365, 0, 0, 0)));
        }

        [Fact]
        public static void Parse_String()
        {
            DateTime expected = DateTime.MaxValue;
            string expectedString = expected.ToString();

            DateTime result = DateTime.Parse(expectedString);
            Assert.Equal(expectedString, result.ToString());
        }

        [Fact]
        public static void Parse_String_FormatProvider()
        {
            DateTime expected = DateTime.MaxValue;
            string expectedString = expected.ToString();

            DateTime result = DateTime.Parse(expectedString, null);
            Assert.Equal(expectedString, result.ToString());
        }

        [Fact]
        public static void Parse_String_FormatProvider_DateTimeStyles()
        {
            DateTime expected = DateTime.MaxValue;
            string expectedString = expected.ToString();

            DateTime result = DateTime.Parse(expectedString, null, DateTimeStyles.None);
            Assert.Equal(expectedString, result.ToString());
        }

        [Fact]
        public static void Parse_Japanese()
        {
            var expected = new DateTime(2012, 12, 21, 10, 8, 6);
            var cultureInfo = new CultureInfo("ja-JP");

            string expectedString = string.Format(cultureInfo, "{0}", expected);
            Assert.Equal(expected, DateTime.Parse(expectedString, cultureInfo));
        }

        [Fact]
        public static void Parse_InvalidArguments_Throws()
        {
            AssertExtensions.Throws<ArgumentNullException>("s", () => DateTime.Parse(null));
            AssertExtensions.Throws<ArgumentNullException>("s", () => DateTime.Parse(null, new MyFormatter()));
            AssertExtensions.Throws<ArgumentNullException>("s", () => DateTime.Parse((string)null, new MyFormatter(), DateTimeStyles.NoCurrentDateDefault));

            Assert.Throws<FormatException>(() => DateTime.Parse(""));
            Assert.Throws<FormatException>(() => DateTime.Parse("", new MyFormatter()));
            Assert.Throws<FormatException>(() => DateTime.Parse("", new MyFormatter(), DateTimeStyles.NoCurrentDateDefault));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public static void TryParse_NullOrEmptyString_ReturnsFalse(string input)
        {
            Assert.False(DateTime.TryParse(input, out DateTime result));
            Assert.False(DateTime.TryParse(input, new MyFormatter(), DateTimeStyles.None, out result));
        }

        [Fact]
        public static void ParseExact_InvalidArguments_Throws()
        {
            AssertExtensions.Throws<ArgumentNullException>("s", () => DateTime.ParseExact(null, "d", new MyFormatter()));
            AssertExtensions.Throws<ArgumentNullException>("s", () => DateTime.ParseExact((string)null, "d", new MyFormatter(), DateTimeStyles.None));
            AssertExtensions.Throws<ArgumentNullException>("s", () => DateTime.ParseExact((string)null, new[] { "d" }, new MyFormatter(), DateTimeStyles.NoCurrentDateDefault));

            Assert.Throws<FormatException>(() => DateTime.ParseExact("", "d", new MyFormatter()));
            Assert.Throws<FormatException>(() => DateTime.ParseExact("", "d", new MyFormatter(), DateTimeStyles.None));
            Assert.Throws<FormatException>(() => DateTime.ParseExact("", new[] { "d" }, new MyFormatter(), DateTimeStyles.NoCurrentDateDefault));

            AssertExtensions.Throws<ArgumentNullException>("format", () => DateTime.ParseExact("123", null, new MyFormatter()));
            AssertExtensions.Throws<ArgumentNullException>("format", () => DateTime.ParseExact("123", (string)null, new MyFormatter(), DateTimeStyles.None));
            AssertExtensions.Throws<ArgumentNullException>("formats", () => DateTime.ParseExact("123", (string[])null, new MyFormatter(), DateTimeStyles.NoCurrentDateDefault));

            Assert.Throws<FormatException>(() => DateTime.ParseExact("123", "", new MyFormatter()));
            Assert.Throws<FormatException>(() => DateTime.ParseExact("123", "", new MyFormatter(), DateTimeStyles.None));
            Assert.Throws<FormatException>(() => DateTime.ParseExact("123", new string[0], new MyFormatter(), DateTimeStyles.NoCurrentDateDefault));
            Assert.Throws<FormatException>(() => DateTime.ParseExact("123", new string[] { null }, new MyFormatter(), DateTimeStyles.NoCurrentDateDefault));
            Assert.Throws<FormatException>(() => DateTime.ParseExact("123", new[] { "" }, new MyFormatter(), DateTimeStyles.NoCurrentDateDefault));
        }

        [Fact]
        public static void TryParseExact_InvalidArguments_ReturnsFalse()
        {
            Assert.False(DateTime.TryParseExact((string)null, "d", new MyFormatter(), DateTimeStyles.None, out DateTime result));
            Assert.False(DateTime.TryParseExact((string)null, new[] { "d" }, new MyFormatter(), DateTimeStyles.None, out result));

            Assert.False(DateTime.TryParseExact("", "d", new MyFormatter(), DateTimeStyles.None, out result));
            Assert.False(DateTime.TryParseExact("", new[] { "d" }, new MyFormatter(), DateTimeStyles.None, out result));

            Assert.False(DateTime.TryParseExact("abc", (string)null, new MyFormatter(), DateTimeStyles.None, out result));
            Assert.False(DateTime.TryParseExact("abc", (string[])null, new MyFormatter(), DateTimeStyles.None, out result));

            Assert.False(DateTime.TryParseExact("abc", "", new MyFormatter(), DateTimeStyles.None, out result));
            Assert.False(DateTime.TryParseExact("abc", new string[0], new MyFormatter(), DateTimeStyles.None, out result));
            Assert.False(DateTime.TryParseExact("abc", new string[] { null }, new MyFormatter(), DateTimeStyles.None, out result));
            Assert.False(DateTime.TryParseExact("abc", new[] { "" }, new MyFormatter(), DateTimeStyles.None, out result));
            Assert.False(DateTime.TryParseExact("abc", new[] { "" }, new MyFormatter(), DateTimeStyles.None, out result));
        }

        [Fact]
        public static void TryParse_String()
        {
            DateTime expected = DateTime.MaxValue;
            string expectedString = expected.ToString("g");

            DateTime result;
            Assert.True(DateTime.TryParse(expectedString, out result));
            Assert.Equal(expectedString, result.ToString("g"));
        }

        [Fact]
        public static void TryParse_String_FormatProvider_DateTimeStyles_U()
        {
            DateTime expected = DateTime.MaxValue;
            string expectedString = expected.ToString("u");

            DateTime result;
            Assert.True(DateTime.TryParse(expectedString, null, DateTimeStyles.AdjustToUniversal, out result));
            Assert.Equal(expectedString, result.ToString("u"));
        }

        [Fact]
        public static void TryParse_String_FormatProvider_DateTimeStyles_G()
        {
            DateTime expected = DateTime.MaxValue;
            string expectedString = expected.ToString("g");

            DateTime result;
            Assert.True(DateTime.TryParse(expectedString, null, DateTimeStyles.AdjustToUniversal, out result));
            Assert.Equal(expectedString, result.ToString("g"));
        }

        [Fact]
        [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework, "The full .NET framework has a bug and incorrectly parses this date")]
        public static void TryParse_TimeDesignators_NetCore()
        {
            DateTime result;
            Assert.True(DateTime.TryParse("4/21 5am", new CultureInfo("en-US"), DateTimeStyles.None, out result));
            Assert.Equal(4, result.Month);
            Assert.Equal(21, result.Day);
            Assert.Equal(5, result.Hour);

            Assert.True(DateTime.TryParse("4/21 5pm", new CultureInfo("en-US"), DateTimeStyles.None, out result));
            Assert.Equal(4, result.Month);
            Assert.Equal(21, result.Day);
            Assert.Equal(17, result.Hour);
        }

        [Fact]
        [SkipOnTargetFramework(~TargetFrameworkMonikers.NetFramework, "The coreclr fixed a bug where the .NET framework incorrectly parses this date")]
        public static void TryParse_TimeDesignators_Netfx()
        {
            DateTime result;
            Assert.True(DateTime.TryParse("4/21 5am", new CultureInfo("en-US"), DateTimeStyles.None, out result));
            Assert.Equal(DateTime.Now.Month, result.Month);
            Assert.Equal(DateTime.Now.Day, result.Day);
            Assert.Equal(4, result.Hour);
            Assert.Equal(0, result.Minute);
            Assert.Equal(0, result.Second);

            Assert.True(DateTime.TryParse("4/21 5pm", new CultureInfo("en-US"), DateTimeStyles.None, out result));
            Assert.Equal(DateTime.Now.Month, result.Month);
            Assert.Equal(DateTime.Now.Day, result.Day);
            Assert.Equal(16, result.Hour);
            Assert.Equal(0, result.Minute);
            Assert.Equal(0, result.Second);
        }

        public static IEnumerable<object[]> StandardFormatSpecifiers()
        {
            yield return new object[] { "d" };
            yield return new object[] { "D" };
            yield return new object[] { "f" };
            yield return new object[] { "F" };
            yield return new object[] { "g" };
            yield return new object[] { "G" };
            yield return new object[] { "m" };
            yield return new object[] { "M" };
            yield return new object[] { "o" };
            yield return new object[] { "O" };
            yield return new object[] { "r" };
            yield return new object[] { "R" };
            yield return new object[] { "s" };
            yield return new object[] { "t" };
            yield return new object[] { "T" };
            yield return new object[] { "u" };
            yield return new object[] { "U" };
            yield return new object[] { "y" };
            yield return new object[] { "Y" };
        }

        [Theory]
        [MemberData(nameof(StandardFormatSpecifiers))]
        public static void ParseExact_ToStringThenParseExactRoundtrip_Success(string standardFormat)
        {
            var r = new Random(42);
            for (int i = 0; i < 200; i++) // test with a bunch of random dates
            {
                DateTime dt = new DateTime(DateTime.MinValue.Ticks + (long)(r.NextDouble() * (DateTime.MaxValue.Ticks - DateTime.MinValue.Ticks)), DateTimeKind.Unspecified);
                string expected = dt.ToString(standardFormat);

                Assert.Equal(expected, DateTime.ParseExact(expected, standardFormat, null).ToString(standardFormat));
                Assert.Equal(expected, DateTime.ParseExact(expected, standardFormat, null, DateTimeStyles.None).ToString(standardFormat));
                Assert.Equal(expected, DateTime.ParseExact(expected, new[] { standardFormat }, null, DateTimeStyles.None).ToString(standardFormat));
                Assert.Equal(expected, DateTime.ParseExact(expected, new[] { standardFormat }, null, DateTimeStyles.AllowWhiteSpaces).ToString(standardFormat));

                Assert.True(DateTime.TryParseExact(expected, standardFormat, null, DateTimeStyles.None, out DateTime actual));
                Assert.Equal(expected, actual.ToString(standardFormat));
                Assert.True(DateTime.TryParseExact(expected, new[] { standardFormat }, null, DateTimeStyles.None, out actual));
                Assert.Equal(expected, actual.ToString(standardFormat));

                // Should also parse with Parse, though may not round trip exactly
                DateTime.Parse(expected);
            }
        }

        public static IEnumerable<object[]> InvalidFormatSpecifierRoundtripPairs()
        {
            yield return new object[] { "d", "f" };
            yield return new object[] { "o", "r" };
            yield return new object[] { "u", "y" };
        }

        [Theory]
        [MemberData(nameof(InvalidFormatSpecifierRoundtripPairs))]
        public static void ParseExact_ToStringThenParseExact_RoundtripWithOtherFormat_Fails(string toStringFormat, string parseFormat)
        {
            DateTime dt = DateTime.Now;
            string expected = dt.ToString(toStringFormat);

            Assert.Throws<FormatException>(() => DateTime.ParseExact(expected, parseFormat, null));
            Assert.Throws<FormatException>(() => DateTime.ParseExact(expected, parseFormat, null, DateTimeStyles.None));
            Assert.Throws<FormatException>(() => DateTime.ParseExact(expected, new[] { parseFormat }, null, DateTimeStyles.None));

            Assert.False(DateTime.TryParseExact(expected, parseFormat, null, DateTimeStyles.None, out DateTime result));
            Assert.False(DateTime.TryParseExact(expected, new[] { parseFormat }, null, DateTimeStyles.None, out result));
        }

        [Theory]
        [MemberData(nameof(ParseExact_TestData_R))]
        public static void ParseExact_String_String_FormatProvider_DateTimeStyles_R(DateTime dt, string input)
        {
            Assert.Equal(DateTimeKind.Unspecified, DateTime.ParseExact(input, "r", null).Kind);

            Assert.Equal(dt.ToString("r"), DateTime.ParseExact(input, "r", null).ToString("r"));
            Assert.Equal(dt.ToString("r"), DateTime.ParseExact(input, "r", null, DateTimeStyles.None).ToString("r"));

            const string Whitespace = " \t\r\n ";
            Assert.Equal(dt.ToString("r"), DateTime.ParseExact(Whitespace + input, "r", null, DateTimeStyles.AllowLeadingWhite).ToString("r"));
            Assert.Equal(dt.ToString("r"), DateTime.ParseExact(input + Whitespace, "r", null, DateTimeStyles.AllowTrailingWhite).ToString("r"));
            Assert.Equal(dt.ToString("r"), DateTime.ParseExact(
                Whitespace +
                input +
                Whitespace, "r", null, DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite).ToString("r"));
            Assert.Equal(dt.ToString("r"), DateTime.ParseExact(
                input.Substring(0, 4) +
                Whitespace +
                input.Substring(4), "r", null, DateTimeStyles.AllowInnerWhite).ToString("r"));
            Assert.Equal(dt.ToString("r"), DateTime.ParseExact(
                Whitespace +
                input.Substring(0, 4) +
                Whitespace +
                input.Substring(4) +
                Whitespace, "r", null, DateTimeStyles.AllowWhiteSpaces).ToString("r"));
        }

        public static IEnumerable<object[]> ParseExact_TestData_R()
        {
            // Lowest, highest, and random DateTime in lower, upper, and normal casing
            var pairs = new(DateTime, string)[]
            {
                (DateTime.MaxValue, "Fri, 31 Dec 9999 23:59:59"),
                (DateTime.MinValue, "Mon, 01 Jan 0001 00:00:00"),
                (new DateTime(1906, 8, 15, 7, 24, 5, 300), "Wed, 15 Aug 1906 07:24:05"),
            };
            foreach ((DateTime, string) pair in pairs)
            {
                yield return new object[] { pair.Item1, pair.Item2 + " GMT" };
                yield return new object[] { pair.Item1, pair.Item2.ToLowerInvariant() + " GMT" };
                yield return new object[] { pair.Item1, pair.Item2.ToUpperInvariant() + " GMT" };
            }

            // All months
            DateTime dt = DateTime.UtcNow;
            for (int i = 0; i < 12; i++)
            {
                dt = dt.AddMonths(1);
                yield return new object[] { dt, dt.ToString("R") };
            }

            // All days
            for (int i = 0; i < 7; i++)
            {
                dt = dt.AddDays(1);
                yield return new object[] { dt, dt.ToString("R") };
            }
        }

        [Theory]
        [MemberData(nameof(ParseExact_TestData_InvalidData_R))]
        public static void ParseExact_InvalidData_R(string invalidString)
        {
            Assert.Throws<FormatException>(() => DateTime.ParseExact(invalidString, "r", null));
            Assert.Throws<FormatException>(() => DateTime.ParseExact(invalidString, "r", null, DateTimeStyles.None));
            Assert.Throws<FormatException>(() => DateTime.ParseExact(invalidString, new string[] { "r" }, null, DateTimeStyles.None));
        }

        public static IEnumerable<object[]> ParseExact_TestData_InvalidData_R()
        {
            yield return new object[] { "Thu, 15 Aug 1906 07:24:05 GMT" }; // invalid day of week
            yield return new object[] { "Ste, 15 Aug 1906 07:24:05 GMT" }; // invalid day of week
            yield return new object[] { "We, 15 Aug 1906 07:24:05 GMT" }; // too short day of week
            yield return new object[] { "Wedn, 15 Aug 1906 07:24:05 GMT" }; // too long day of week

            yield return new object[] { "Wed, 32 Aug 1906 07:24:05 GMT" }; // too large day
            yield return new object[] { "Wed, -1 Aug 1906 07:24:05 GMT" }; // too small day

            yield return new object[] { "Wed, 15 Au 1906 07:24:05 GMT" }; // too small month
            yield return new object[] { "Wed, 15 August 1906 07:24:05 GMT" }; // too large month

            yield return new object[] { "Wed, 15 Aug -1 07:24:05 GMT" }; // too small year
            yield return new object[] { "Wed, 15 Aug 10000 07:24:05 GMT" }; // too large year

            yield return new object[] { "Wed, 15 Aug 1906 24:24:05 GMT" }; // too large hour
            yield return new object[] { "Wed, 15 Aug 1906 07:60:05 GMT" }; // too large minute
            yield return new object[] { "Wed, 15 Aug 1906 07:24:60 GMT" }; // too large second

            yield return new object[] { "Wed, 15 Aug 1906 07:24:05 STE" }; // invalid timezone
            yield return new object[] { "Wed, 15 Aug 1906 07:24:05 GM" }; // too short timezone
            yield return new object[] { "Wed, 15 Aug 1906 07:24:05 GMTT" }; // too long timezone
            yield return new object[] { "Wed, 15 Aug 1906 07:24:05 gmt" }; // wrong casing
            yield return new object[] { "Wed, 15 Aug 1906 07:24:05 Z" }; // zulu invalid
            yield return new object[] { "Wed, 15 Aug 1906 07:24:05 UTC" }; // UTC invalid

            yield return new object[] { " Wed, 15 Aug 1906 07:24:05 GMT" }; // whitespace before
            yield return new object[] { "Wed, 15 Aug 1906 07:24:05 GMT " }; // whitespace after
            yield return new object[] { "Wed, 15 Aug 1906  07:24:05 GMT" }; // extra whitespace middle
            yield return new object[] { "Wed, 15 Aug 1906 07: 24:05 GMT" }; // extra whitespace middle

            yield return new object[] { "Wed,\t15 Aug 1906 07:24:05 GMT" }; // wrong whitespace for first space
            yield return new object[] { "Wed, 15\tAug 1906 07:24:05 GMT" }; // wrong whitespace for second space
            yield return new object[] { "Wed, 15 Aug\t1906 07:24:05 GMT" }; // wrong whitespace for third space
            yield return new object[] { "Wed, 15 Aug 1906\t07:24:05 GMT" }; // wrong whitespace for fourth space
            yield return new object[] { "Wed, 15 Aug 1906 07:24:05\tGMT" }; // wrong whitespace for fifth space
            yield return new object[] { "Wed; 15 Aug 1906 07:24:05 GMT" }; // wrong comma
            yield return new object[] { "Wed\x642C 15 Aug 1906 07:24:05 GMT" }; // wrong comma
            yield return new object[] { "Wed, 15 Aug 1906 07;24:05 GMT" }; // wrong first colon
            yield return new object[] { "Wed, 15 Aug 1906 07:24;05 GMT" }; // wrong second colon

            yield return new object[] { "\x2057ed, 15 Aug 1906 07:24:05 GMT" }; // invalid characters to validate ASCII checks on day of week
            yield return new object[] { "W\x5765d, 15 Aug 1906 07:24:05 GMT" }; // invalid characters to validate ASCII checks on day of week
            yield return new object[] { "We\x6564, 15 Aug 1906 07:24:05 GMT" }; // invalid characters to validate ASCII checks on day of week

            yield return new object[] { "Wed, 15 \x2041ug 1906 07:24:05 GMT" }; // invalid characters to validate ASCII checks on month
            yield return new object[] { "Wed, 15 A\x4175g 1906 07:24:05 GMT" }; // invalid characters to validate ASCII checks on month
            yield return new object[] { "Wed, 15 Au\x7567 1906 07:24:05 GMT" }; // invalid characters to validate ASCII checks on month

            yield return new object[] { "Wed, 15 Aug 1906 07:24:05 \x2047MT" }; // invalid characters to validate ASCII checks on GMT
            yield return new object[] { "Wed, 15 Aug 1906 07:24:05 G\x474DT" }; // invalid characters to validate ASCII checks on GMT
            yield return new object[] { "Wed, 15 Aug 1906 07:24:05 GM\x4D54" }; // invalid characters to validate ASCII checks on GMT

            yield return new object[] { "Wed, A5 Aug 1906 07:24:05 GMT" }; // invalid digits
            yield return new object[] { "Wed, 1A Aug 1906 07:24:05 GMT" }; // invalid digits
            yield return new object[] { "Wed, 15 Aug A906 07:24:05 GMT" }; // invalid digits
            yield return new object[] { "Wed, 15 Aug 1A06 07:24:05 GMT" }; // invalid digits
            yield return new object[] { "Wed, 15 Aug 19A6 07:24:05 GMT" }; // invalid digits
            yield return new object[] { "Wed, 15 Aug 190A 07:24:05 GMT" }; // invalid digits
            yield return new object[] { "Wed, 15 Aug 1906 A7:24:05 GMT" }; // invalid digits
            yield return new object[] { "Wed, 15 Aug 1906 0A:24:05 GMT" }; // invalid digits
            yield return new object[] { "Wed, 15 Aug 1906 07:A4:05 GMT" }; // invalid digits
            yield return new object[] { "Wed, 15 Aug 1906 07:2A:05 GMT" }; // invalid digits
            yield return new object[] { "Wed, 15 Aug 1906 07:24:A5 GMT" }; // invalid digits
            yield return new object[] { "Wed, 15 Aug 1906 07:24:0A GMT" }; // invalid digits
        }

        [Theory]
        [MemberData(nameof(ParseExact_TestData_O))]
        public static void ParseExact_String_String_FormatProvider_DateTimeStyles_O(DateTime dt, string input)
        {
            string expectedString;
            if (input.Length == 27) // no timezone
            {
                Assert.Equal(DateTimeKind.Unspecified, DateTime.ParseExact(input, "o", null).Kind);
                expectedString = dt.ToString("o");
            }
            else // "Z" or +/- offset
            {
                Assert.Equal(DateTimeKind.Local, DateTime.ParseExact(input, "o", null).Kind);
                expectedString = dt.ToLocalTime().ToString("o");
            }

            Assert.Equal(expectedString, DateTime.ParseExact(input, "o", null).ToString("o"));
            Assert.Equal(expectedString, DateTime.ParseExact(input, "o", null, DateTimeStyles.None).ToString("o"));

            const string Whitespace = " \t\r\n ";
            Assert.Equal(expectedString, DateTime.ParseExact(Whitespace + input, "o", null, DateTimeStyles.AllowLeadingWhite).ToString("o"));
            Assert.Equal(expectedString, DateTime.ParseExact(input + Whitespace, "o", null, DateTimeStyles.AllowTrailingWhite).ToString("o"));
            Assert.Equal(expectedString, DateTime.ParseExact(
                Whitespace +
                input +
                Whitespace, "o", null, DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite).ToString("o"));
            Assert.Equal(expectedString, DateTime.ParseExact(
                input.Substring(0, 27) +
                Whitespace +
                input.Substring(27), "o", null, DateTimeStyles.AllowInnerWhite).ToString("o"));
            Assert.Equal(expectedString, DateTime.ParseExact(
                Whitespace +
                input.Substring(0, 27) +
                Whitespace +
                input.Substring(27) +
                Whitespace, "o", null, DateTimeStyles.AllowWhiteSpaces).ToString("o"));
        }

        public static IEnumerable<object[]> ParseExact_TestData_O()
        {
            // Arbitrary DateTime in each of Unspecified, Utc, and Local kinds.
            foreach (DateTimeKind kind in new[] { DateTimeKind.Unspecified, DateTimeKind.Utc, DateTimeKind.Local })
            {
                var dt = new DateTime(1234567891234567891, kind);
                yield return new object[] { dt, dt.ToString("o") };
            }

            // Min and max in each of Unspecified, Utc, and Local kinds.
            foreach (DateTime dt in new[] { DateTime.MinValue, DateTime.MaxValue })
            {
                yield return new object[] { dt, dt.ToString("o") };
                yield return new object[] { dt.ToUniversalTime(), dt.ToUniversalTime().ToString("o") };
                yield return new object[] { dt.ToLocalTime(), dt.ToLocalTime().ToString("o") };
            }

            // 1-digit offset hour is accepted due to legacy/compat
            yield return new object[] { new DateTime(636664076235238523, DateTimeKind.Utc), "2018-07-05T18:36:43.5238523+1:23" };
        }

        [Theory]
        [MemberData(nameof(ParseExact_TestData_InvalidData_O))]
        public static void ParseExact_InvalidData_O(string invalidString)
        {
            Assert.Throws<FormatException>(() => DateTime.ParseExact(invalidString, "o", null));
            Assert.Throws<FormatException>(() => DateTime.ParseExact(invalidString, "o", null, DateTimeStyles.None));
            Assert.Throws<FormatException>(() => DateTime.ParseExact(invalidString, new string[] { "o" }, null, DateTimeStyles.None));
        }

        public static IEnumerable<object[]> ParseExact_TestData_InvalidData_O()
        {
            yield return new object[] { " 2018-07-05T18:36:43.5238523" }; // whitespace before
            yield return new object[] { " 2018-07-05T18:36:43.5238523Z" }; // whitespace before
            yield return new object[] { " 2018-07-05T18:36:43.5238523+00:00" }; // whitespace before
            yield return new object[] { "2018-07-05T18:36:43.5238523 " }; // whitespace after
            yield return new object[] { "2018-07-05T18:36:43.5238523Z " }; // whitespace after
            yield return new object[] { "2018-07-05T18:36:43.5238523+00:00 " }; // whitespace after
            yield return new object[] { "2018-07-05T18:36:43.5238523 Z" }; // whitespace inside
            yield return new object[] { "2018-07-05T18:36:43.5238523 +00:00" }; // whitespace inside

            yield return new object[] { "201-07-05T18:36:43.5238523" }; // too short year
            yield return new object[] { "20181-07-05T18:36:43.5238523" }; // too long year
            yield return new object[] { "2018-7-05T18:36:43.5238523" }; // too short month
            yield return new object[] { "2018-017-05T18:36:43.5238523" }; // too long month
            yield return new object[] { "2018-07-5T18:36:43.5238523" }; // too short day
            yield return new object[] { "2018-07-015T18:36:43.5238523" }; // too long day
            yield return new object[] { "2018-07-05T018:36:43.5238523" }; // too long hour
            yield return new object[] { "2018-07-05T8:36:43.5238523" }; // too short hour
            yield return new object[] { "2018-07-05T18:6:43.5238523" }; // too short minute
            yield return new object[] { "2018-07-05T18:036:43.5238523" }; // too long minute
            yield return new object[] { "2018-07-05T18:06:3.5238523" }; // too short second
            yield return new object[] { "2018-07-05T18:36:043.5238523" }; // too long second
            yield return new object[] { "2018-07-05T18:06:03.238523" }; // too short fraction
            yield return new object[] { "2018-07-05T18:36:43.15238523" }; // too long fraction
            yield return new object[] { "2018-07-05T18:36:43.5238523+001:00" }; // too long offset hour
            yield return new object[] { "2018-07-05T18:36:43.5238523+01:0" }; // too short offset minute
            yield return new object[] { "2018-07-05T18:36:43.5238523+01:000" }; // too long offset minute

            yield return new object[] { "2018=07-05T18:36:43.5238523" }; // invalid first hyphen
            yield return new object[] { "2018-07=05T18:36:43.5238523" }; // invalid second hyphen
            yield return new object[] { "2018-07-05A18:36:43.5238523" }; // invalid T
            yield return new object[] { "2018-07-05T18;36:43.5238523" }; // invalid first colon
            yield return new object[] { "2018-07-05T18:36;43.5238523" }; // invalid second colon
            yield return new object[] { "2018-07-05T18:36:43,5238523" }; // invalid period
            yield return new object[] { "2018-07-05T18:36:43.5238523,00:00" }; // invalid +/-/Z
            yield return new object[] { "2018-07-05T18:36:43.5238523+00;00" }; // invalid third colon
            yield return new object[] { "2018-07-05T18:36:43.5238523+1;00" }; // invalid colon with 1-digit offset hour

            yield return new object[] { "a018-07-05T18:36:43.5238523" }; // invalid digits
            yield return new object[] { "2a18-07-05T18:36:43.5238523" }; // invalid digits
            yield return new object[] { "20a8-07-05T18:36:43.5238523" }; // invalid digits
            yield return new object[] { "201a-07-05T18:36:43.5238523" }; // invalid digits
            yield return new object[] { "2018-a7-05T18:36:43.5238523" }; // invalid digits
            yield return new object[] { "2018-0a-05T18:36:43.5238523" }; // invalid digits
            yield return new object[] { "2018-07-a5T18:36:43.5238523" }; // invalid digits
            yield return new object[] { "2018-07-0aT18:36:43.5238523" }; // invalid digits
            yield return new object[] { "2018-07-05Ta8:36:43.5238523" }; // invalid digits
            yield return new object[] { "2018-07-05T1a:36:43.5238523" }; // invalid digits
            yield return new object[] { "2018-07-05T18:a6:43.5238523" }; // invalid digits
            yield return new object[] { "2018-07-05T18:3a:43.5238523" }; // invalid digits
            yield return new object[] { "2018-07-05T18:36:a3.5238523" }; // invalid digits
            yield return new object[] { "2018-07-05T18:36:4a.5238523" }; // invalid digits
            yield return new object[] { "2018-07-05T18:36:43.a238523" }; // invalid digits
            yield return new object[] { "2018-07-05T18:36:43.5a38523" }; // invalid digits
            yield return new object[] { "2018-07-05T18:36:43.52a8523" }; // invalid digits
            yield return new object[] { "2018-07-05T18:36:43.523a523" }; // invalid digits
            yield return new object[] { "2018-07-05T18:36:43.5238a23" }; // invalid digits
            yield return new object[] { "2018-07-05T18:36:43.52385a3" }; // invalid digits
            yield return new object[] { "2018-07-05T18:36:43.523852a" }; // invalid digits
            yield return new object[] { "2018-07-05T18:36:43.5238523+a0:00" }; // invalid digits
            yield return new object[] { "2018-07-05T18:36:43.5238523+0a:00" }; // invalid digits
            yield return new object[] { "2018-07-05T18:36:43.5238523+00:a0" }; // invalid digits
            yield return new object[] { "2018-07-05T18:36:43.5238523+00:0a" }; // invalid digits
        }

        [Fact]
        public static void ParseExact_String_String_FormatProvider_DateTimeStyles_CustomFormatProvider()
        {
            var formatter = new MyFormatter();
            string dateBefore = DateTime.Now.ToString();

            DateTime dateAfter = DateTime.ParseExact(dateBefore, "G", formatter, DateTimeStyles.AdjustToUniversal);
            Assert.Equal(dateBefore, dateAfter.ToString());
        }

        [Fact]
        public static void ParseExact_String_StringArray_FormatProvider_DateTimeStyles()
        {
            DateTime expected = DateTime.MaxValue;
            string expectedString = expected.ToString("g");

            var formats = new string[] { "g" };
            DateTime result = DateTime.ParseExact(expectedString, formats, null, DateTimeStyles.AdjustToUniversal);
            Assert.Equal(expectedString, result.ToString("g"));
        }

        [Fact]
        public static void TryParseExact_String_String_FormatProvider_DateTimeStyles_NullFormatProvider()
        {
            DateTime expected = DateTime.MaxValue;
            string expectedString = expected.ToString("g");

            DateTime resulted;
            Assert.True(DateTime.TryParseExact(expectedString, "g", null, DateTimeStyles.AdjustToUniversal, out resulted));
            Assert.Equal(expectedString, resulted.ToString("g"));
        }

        [Fact]
        public static void TryParseExact_String_StringArray_FormatProvider_DateTimeStyles()
        {
            DateTime expected = DateTime.MaxValue;
            string expectedString = expected.ToString("g");

            var formats = new string[] { "g" };
            DateTime result;
            Assert.True(DateTime.TryParseExact(expectedString, formats, null, DateTimeStyles.AdjustToUniversal, out result));
            Assert.Equal(expectedString, result.ToString("g"));
        }

        [Fact]
        [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework, "Needs desktop port: https://github.com/dotnet/coreclr/issues/15896")]
        // Regression test for https://github.com/dotnet/coreclr/issues/15896
        public static void TryParseExact_EmptyAMPMDesignator()
        {
            var englishCulture = new CultureInfo("en-US");
            englishCulture.DateTimeFormat.AMDesignator = "";
            englishCulture.DateTimeFormat.PMDesignator = "";
            Assert.False(DateTime.TryParseExact(" ", "%t", englishCulture, DateTimeStyles.None, out _));
        }

        public static void ParseExact_EscapedSingleQuotes()
        {
            var formatInfo = DateTimeFormatInfo.GetInstance(new CultureInfo("mt-MT"));
            const string format = @"dddd, d' ta\' 'MMMM yyyy";

            DateTime expected = new DateTime(1999, 2, 28, 17, 00, 01);
            string formatted = expected.ToString(format, formatInfo);
            DateTime actual = DateTime.ParseExact(formatted, format, formatInfo);

            Assert.Equal(expected.Date, actual.Date);
        }

        [Theory]
        [InlineData("fi-FI")]
        [InlineData("nb-NO")]
        [InlineData("nb-SJ")]
        [InlineData("sr-Cyrl-XK")]
        [InlineData("sr-Latn-ME")]
        [InlineData("sr-Latn-RS")]
        [InlineData("sr-Latn-XK")]
        public static void Parse_SpecialCultures(string cultureName)
        {
            // Test DateTime parsing with cultures which has the date separator and time separator are same
            CultureInfo cultureInfo;
            try
            {
                cultureInfo = new CultureInfo(cultureName);
            }
            catch (CultureNotFoundException)
            {
                // Ignore un-supported culture in current platform
                return;
            }

            var dateTime = new DateTime(2015, 11, 20, 11, 49, 50);
            string dateString = dateTime.ToString(cultureInfo.DateTimeFormat.ShortDatePattern, cultureInfo);

            DateTime parsedDate;
            Assert.True(DateTime.TryParse(dateString, cultureInfo, DateTimeStyles.None, out parsedDate));
            if (cultureInfo.DateTimeFormat.ShortDatePattern.Contains("yyyy") || HasDifferentDateTimeSeparators(cultureInfo.DateTimeFormat))
            {
                Assert.Equal(dateTime.Date, parsedDate);
            }
            else
            {
                // When the date separator and time separator are the same, DateTime.TryParse cannot
                // tell the difference between a short date like dd.MM.yy and a short time
                // like HH.mm.ss. So it assumes that if it gets 03.04.11, that must be a time
                // and uses the current date to construct the date time.
                DateTime now = DateTime.Now;
                Assert.Equal(new DateTime(now.Year, now.Month, now.Day, dateTime.Day, dateTime.Month, dateTime.Year % 100), parsedDate);
            }

            dateString = dateTime.ToString(cultureInfo.DateTimeFormat.LongDatePattern, cultureInfo);
            Assert.True(DateTime.TryParse(dateString, cultureInfo, DateTimeStyles.None, out parsedDate));
            Assert.Equal(dateTime.Date, parsedDate);

            dateString = dateTime.ToString(cultureInfo.DateTimeFormat.FullDateTimePattern, cultureInfo);
            Assert.True(DateTime.TryParse(dateString, cultureInfo, DateTimeStyles.None, out parsedDate));
            Assert.Equal(dateTime, parsedDate);

            dateString = dateTime.ToString(cultureInfo.DateTimeFormat.LongTimePattern, cultureInfo);
            Assert.True(DateTime.TryParse(dateString, cultureInfo, DateTimeStyles.None, out parsedDate));
            Assert.Equal(dateTime.TimeOfDay, parsedDate.TimeOfDay);
        }

        private static bool HasDifferentDateTimeSeparators(DateTimeFormatInfo dateTimeFormat)
        {
            // Since .NET Core doesn't expose DateTimeFormatInfo DateSeparator and TimeSeparator properties,
            // this method gets the separators using DateTime.ToString by passing in the invariant separators.
            // The invariant separators will then get turned into the culture's separators by ToString,
            // which are then compared.

            var dateTime = new DateTime(2015, 11, 24, 17, 57, 29);
            string separators = dateTime.ToString("/@:", dateTimeFormat);

            int delimiterIndex = separators.IndexOf('@');
            string dateSeparator = separators.Substring(0, delimiterIndex);
            string timeSeparator = separators.Substring(delimiterIndex + 1);
            return dateSeparator != timeSeparator;
        }

        [Fact]
        public static void GetDateTimeFormats()
        {
            var allStandardFormats = new char[]
            {
            'd', 'D', 'f', 'F', 'g', 'G',
            'm', 'M', 'o', 'O', 'r', 'R',
            's', 't', 'T', 'u', 'U', 'y', 'Y',
            };

            var dateTime = new DateTime(2009, 7, 28, 5, 23, 15);
            var formats = new List<string>();

            foreach (char format in allStandardFormats)
            {
                string[] dates = dateTime.GetDateTimeFormats(format);

                Assert.True(dates.Length > 0);

                DateTime parsedDate;
                Assert.True(DateTime.TryParseExact(dates[0], format.ToString(), CultureInfo.CurrentCulture, DateTimeStyles.None, out parsedDate));

                formats.AddRange(dates);
            }

            List<string> actualFormats = dateTime.GetDateTimeFormats().ToList();
            Assert.Equal(formats.OrderBy(t => t), actualFormats.OrderBy(t => t));

            actualFormats = dateTime.GetDateTimeFormats(CultureInfo.CurrentCulture).ToList();
            Assert.Equal(formats.OrderBy(t => t), actualFormats.OrderBy(t => t));
        }

        [Fact]
        public static void GetDateTimeFormats_FormatSpecifier_InvalidFormat()
        {
            var dateTime = new DateTime(2009, 7, 28, 5, 23, 15);
            Assert.Throws<FormatException>(() => dateTime.GetDateTimeFormats('x')); // No such format
        }

        private static void VerifyDateTime(DateTime dateTime, int year, int month, int day, int hour, int minute, int second, int millisecond, DateTimeKind kind)
        {
            Assert.Equal(year, dateTime.Year);
            Assert.Equal(month, dateTime.Month);
            Assert.Equal(day, dateTime.Day);
            Assert.Equal(hour, dateTime.Hour);
            Assert.Equal(minute, dateTime.Minute);
            Assert.Equal(second, dateTime.Second);
            Assert.Equal(millisecond, dateTime.Millisecond);

            Assert.Equal(kind, dateTime.Kind);
        }

        private class MyFormatter : IFormatProvider
        {
            public object GetFormat(Type formatType)
            {
                return typeof(IFormatProvider) == formatType ? this : null;
            }
        }

        [Fact]
        public static void InvalidDateTimeStyles()
        {
            string strDateTime = "Thursday, August 31, 2006 1:14";
            string[] formats = new string[] { "f" };
            IFormatProvider provider = new CultureInfo("en-US");
            DateTimeStyles style = DateTimeStyles.AssumeLocal | DateTimeStyles.AssumeUniversal;
            AssertExtensions.Throws<ArgumentException>("style", () => DateTime.ParseExact(strDateTime, formats, provider, style));
        }

        [Fact]
        public static void TestTryParseAtBoundaries()
        {
            Assert.True(DateTime.TryParse("9999-12-31T23:59:59.9999999", out var maxDateTime),
                        "DateTime parsing expected to succeed at the boundary DateTime.MaxValue");
            Assert.Equal(DateTime.MaxValue, maxDateTime);

            if (PlatformDetection.IsFullFramework)
            {
                AssertExtensions.Throws<ArgumentOutOfRangeException>("value", () => DateTime.TryParse("9999-12-31T23:59:59.999999999Z", out var dateTime)); // exceeded DateTime.MaxValue
            }
            else
            {
                Assert.False(DateTime.TryParse("9999-12-31T23:59:59.999999999Z", out var dateTime),
                         "DateTime parsing expected to throw with any dates greater than DateTime.MaxValue");
            }
        }

        public static IEnumerable<object[]> Parse_ValidInput_Succeeds_MemberData()
        {
            yield return new object[] { "1234 12", CultureInfo.InvariantCulture, new DateTime(1234, 12, 1, 0, 0, 0) };
            yield return new object[] { "12 1234", CultureInfo.InvariantCulture, new DateTime(1234, 12, 1, 0, 0, 0) };
            yield return new object[] { "12 1234 11", CultureInfo.InvariantCulture, new DateTime(1234, 12, 11, 0, 0, 0) };
            yield return new object[] { "1234 12 13", CultureInfo.InvariantCulture, new DateTime(1234, 12, 13, 0, 0, 0) };
            yield return new object[] { "12 13 1234", CultureInfo.InvariantCulture, new DateTime(1234, 12, 13, 0, 0, 0) };
            yield return new object[] { "1 1 1", CultureInfo.InvariantCulture, new DateTime(2001, 1, 1, 0, 0, 0) };
            yield return new object[] { "2 2 2Z", CultureInfo.InvariantCulture, TimeZoneInfo.ConvertTimeFromUtc(new DateTime(2002, 2, 2, 0, 0, 0, DateTimeKind.Utc), TimeZoneInfo.Local) };
            yield return new object[] { "#10/10/2095#\0", CultureInfo.InvariantCulture, new DateTime(2095, 10, 10, 0, 0, 0) };

            DateTime today = DateTime.Today;
            var hebrewCulture = new CultureInfo("he-IL");
            hebrewCulture.DateTimeFormat.Calendar = new HebrewCalendar();
            yield return new object[] { today.ToString(hebrewCulture), hebrewCulture, today };

            var mongolianCulture = new CultureInfo("mn-MN");
            yield return new object[] { today.ToString(mongolianCulture), mongolianCulture, today };
        }

        [Theory]
        [MemberData(nameof(Parse_ValidInput_Succeeds_MemberData))]
        public static void Parse_ValidInput_Succeeds(string input, CultureInfo culture, DateTime? expected)
        {
            Assert.Equal(expected, DateTime.Parse(input, culture));
        }

        public static IEnumerable<object[]> ParseExact_ValidInput_Succeeds_MemberData()
        {
            foreach (DateTimeStyles style in new[] { DateTimeStyles.None, DateTimeStyles.AllowWhiteSpaces })
            {
                yield return new object[] { "9", "%d", CultureInfo.InvariantCulture, style, new DateTime(DateTime.Now.Year, 1, 9, 0, 0, 0) };
                yield return new object[] { "15", "dd", CultureInfo.InvariantCulture, style, new DateTime(DateTime.Now.Year, 1, 15, 0, 0, 0) };

                yield return new object[] { "9", "%M", CultureInfo.InvariantCulture, style, new DateTime(DateTime.Now.Year, 9, 1, 0, 0, 0) };
                yield return new object[] { "09", "MM", CultureInfo.InvariantCulture, style, new DateTime(DateTime.Now.Year, 9, 1, 0, 0, 0) };
                yield return new object[] { "Sep", "MMM", CultureInfo.InvariantCulture, style, new DateTime(DateTime.Now.Year, 9, 1, 0, 0, 0) };
                yield return new object[] { "September", "MMMM", CultureInfo.InvariantCulture, style, new DateTime(DateTime.Now.Year, 9, 1, 0, 0, 0) };

                yield return new object[] { "1", "%y", CultureInfo.InvariantCulture, style, new DateTime(2001, 1, 1, 0, 0, 0) };
                yield return new object[] { "01", "yy", CultureInfo.InvariantCulture, style, new DateTime(2001, 1, 1, 0, 0, 0) };
                yield return new object[] { "2001", "yyyy", CultureInfo.InvariantCulture, style, new DateTime(2001, 1, 1, 0, 0, 0) };

                yield return new object[] { "3", "%H", CultureInfo.InvariantCulture, style, DateTime.Today + TimeSpan.FromHours(3) };
                yield return new object[] { "03", "HH", CultureInfo.InvariantCulture, style, DateTime.Today + TimeSpan.FromHours(3) };

                yield return new object[] { "3A", "ht", CultureInfo.InvariantCulture, style, DateTime.Today + TimeSpan.FromHours(3) };
                yield return new object[] { "03A", "hht", CultureInfo.InvariantCulture, style, DateTime.Today + TimeSpan.FromHours(3) };
                yield return new object[] { "3P", "ht", CultureInfo.InvariantCulture, style, DateTime.Today + TimeSpan.FromHours(12 + 3) };
                yield return new object[] { "03P", "hht", CultureInfo.InvariantCulture, style, DateTime.Today + TimeSpan.FromHours(12 + 3) };

                yield return new object[] { "2017-10-11 01:23:45Z", "u", CultureInfo.InvariantCulture, style, new DateTime(2017, 10, 11, 1, 23, 45) };
                yield return new object[] { "9/8/2017 10:11:12 AM", "M/d/yyyy HH':'mm':'ss tt", CultureInfo.InvariantCulture, style, new DateTime(2017, 9, 8, 10, 11, 12) };
                yield return new object[] { "9/8/2017 20:11:12 PM", "M/d/yyyy HH':'mm':'ss tt", CultureInfo.InvariantCulture, style, new DateTime(2017, 9, 8, 20, 11, 12) };
                yield return new object[] { "Fri, 08 Sep 2017 11:18:19 -0000", "ddd, d MMM yyyy H:m:s zzz", new CultureInfo("en-US"), DateTimeStyles.AllowInnerWhite, new DateTime(2017, 9, 8, 11, 18, 19, DateTimeKind.Utc) };
                yield return new object[] { "1234-05-06T07:00:00.8Z", "yyyy-MM-dd'T'HH:mm:ss.FFF'Z'", CultureInfo.InvariantCulture, style, new DateTime(1234, 5, 6, 7, 0, 0, 800) };
                yield return new object[] { "1234-05-06T07:00:00Z", "yyyy-MM-dd'T'HH:mm:ss.FFF'Z'", CultureInfo.InvariantCulture, style, new DateTime(1234, 5, 6, 7, 0, 0, 0) };
                yield return new object[] { "1234-05-06T07:00:00Z", "yyyy-MM-dd'T'HH:mm:ssFFF'Z'", CultureInfo.InvariantCulture, style, new DateTime(1234, 5, 6, 7, 0, 0, 0) };
                yield return new object[] { "1234-05-06T07:00:00Z", "yyyy-MM-dd'T'HH:mm:ssFFF'Z'", CultureInfo.InvariantCulture, style, new DateTime(1234, 5, 6, 7, 0, 0, 0) };
                yield return new object[] { "1234-05-06T07:00:00Z", "yyyy-MM-dd'T'HH:mm:ssFFFZ", CultureInfo.InvariantCulture, style, TimeZoneInfo.ConvertTimeFromUtc(new DateTime(1234, 5, 6, 7, 0, 0, DateTimeKind.Utc), TimeZoneInfo.Local) };
                yield return new object[] { "1234-05-06T07:00:00GMT", "yyyy-MM-dd'T'HH:mm:ssFFFZ", CultureInfo.InvariantCulture, style, TimeZoneInfo.ConvertTimeFromUtc(new DateTime(1234, 5, 6, 7, 0, 0, DateTimeKind.Utc), TimeZoneInfo.Local) };
            }

            yield return new object[] { "9", "\"  \"%d", CultureInfo.InvariantCulture, DateTimeStyles.AllowLeadingWhite, new DateTime(DateTime.Now.Year, 1, 9, 0, 0, 0) };
            yield return new object[] { "15", "\' \'dd", CultureInfo.InvariantCulture, DateTimeStyles.AllowLeadingWhite, new DateTime(DateTime.Now.Year, 1, 15, 0, 0, 0) };

            yield return new object[] { "9", "\"  \"%M", CultureInfo.InvariantCulture, DateTimeStyles.AllowLeadingWhite, new DateTime(DateTime.Now.Year, 9, 1, 0, 0, 0) };
            yield return new object[] { "09", "\" \"MM", CultureInfo.InvariantCulture, DateTimeStyles.AllowLeadingWhite, new DateTime(DateTime.Now.Year, 9, 1, 0, 0, 0) };
            yield return new object[] { "Sep", "\"  \"MMM", CultureInfo.InvariantCulture, DateTimeStyles.AllowLeadingWhite, new DateTime(DateTime.Now.Year, 9, 1, 0, 0, 0) };
            yield return new object[] { "September", "\' \'MMMM", CultureInfo.InvariantCulture, DateTimeStyles.AllowLeadingWhite, new DateTime(DateTime.Now.Year, 9, 1, 0, 0, 0) };

            yield return new object[] { "1", "\' \'%y", CultureInfo.InvariantCulture, DateTimeStyles.AllowLeadingWhite, new DateTime(2001, 1, 1, 0, 0, 0) };
            yield return new object[] { "01", "\"  \"yy", CultureInfo.InvariantCulture, DateTimeStyles.AllowLeadingWhite, new DateTime(2001, 1, 1, 0, 0, 0) };
            yield return new object[] { "2001", "\" \"yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AllowLeadingWhite, new DateTime(2001, 1, 1, 0, 0, 0) };

            yield return new object[] { "3", "\"  \"%H", CultureInfo.InvariantCulture, DateTimeStyles.AllowLeadingWhite, DateTime.Today + TimeSpan.FromHours(3) };
            yield return new object[] { "03", "\" \"HH", CultureInfo.InvariantCulture, DateTimeStyles.AllowLeadingWhite, DateTime.Today + TimeSpan.FromHours(3) };

            yield return new object[] { "3A", "\"  \"ht", CultureInfo.InvariantCulture, DateTimeStyles.AllowLeadingWhite, DateTime.Today + TimeSpan.FromHours(3) };
            yield return new object[] { "03A", "\" \"hht", CultureInfo.InvariantCulture, DateTimeStyles.AllowLeadingWhite, DateTime.Today + TimeSpan.FromHours(3) };
            yield return new object[] { "3P", "\'  \'ht", CultureInfo.InvariantCulture, DateTimeStyles.AllowLeadingWhite, DateTime.Today + TimeSpan.FromHours(12 + 3) };
            yield return new object[] { "03P", "\" \"hht", CultureInfo.InvariantCulture, DateTimeStyles.AllowLeadingWhite, DateTime.Today + TimeSpan.FromHours(12 + 3) };

            yield return new object[] { "2017-10-11 01:23:45Z", "u", CultureInfo.InvariantCulture, DateTimeStyles.AllowLeadingWhite, new DateTime(2017, 10, 11, 1, 23, 45) };
            yield return new object[] { "9/8/2017 10:11:12 AM", "\'  \'M/d/yyyy HH':'mm':'ss tt", CultureInfo.InvariantCulture, DateTimeStyles.AllowLeadingWhite, new DateTime(2017, 9, 8, 10, 11, 12) };
            yield return new object[] { "9/8/2017 20:11:12 PM", "\" \"M/d/yyyy HH':'mm':'ss tt", CultureInfo.InvariantCulture, DateTimeStyles.AllowLeadingWhite, new DateTime(2017, 9, 8, 20, 11, 12) };
            yield return new object[] { "1234-05-06T07:00:00.8Z", "\" \"yyyy-MM-dd'T'HH:mm:ss.FFF'Z'", CultureInfo.InvariantCulture, DateTimeStyles.AllowLeadingWhite, new DateTime(1234, 5, 6, 7, 0, 0, 800) };
            yield return new object[] { "1234-05-06T07:00:00Z", "\"  \"yyyy-MM-dd'T'HH:mm:ss.FFF'Z'", CultureInfo.InvariantCulture, DateTimeStyles.AllowLeadingWhite, new DateTime(1234, 5, 6, 7, 0, 0, 0) };
            yield return new object[] { "1234-05-06T07:00:00Z", "\' \'yyyy-MM-dd'T'HH:mm:ssFFF'Z'", CultureInfo.InvariantCulture, DateTimeStyles.AllowLeadingWhite, new DateTime(1234, 5, 6, 7, 0, 0, 0) };
            yield return new object[] { "1234-05-06T07:00:00Z", "\'  \'yyyy-MM-dd'T'HH:mm:ssFFF'Z'", CultureInfo.InvariantCulture, DateTimeStyles.AllowLeadingWhite, new DateTime(1234, 5, 6, 7, 0, 0, 0) };
            yield return new object[] { "1234-05-06T07:00:00Z", "\" \"yyyy-MM-dd'T'HH:mm:ssFFFZ", CultureInfo.InvariantCulture, DateTimeStyles.AllowLeadingWhite, TimeZoneInfo.ConvertTimeFromUtc(new DateTime(1234, 5, 6, 7, 0, 0, DateTimeKind.Utc), TimeZoneInfo.Local) };
            yield return new object[] { "1234-05-06T07:00:00GMT", "\"  \"yyyy-MM-dd'T'HH:mm:ssFFFZ", CultureInfo.InvariantCulture, DateTimeStyles.AllowLeadingWhite, TimeZoneInfo.ConvertTimeFromUtc(new DateTime(1234, 5, 6, 7, 0, 0, DateTimeKind.Utc), TimeZoneInfo.Local) };


            yield return new object[] { "9", "%d\"  \"", CultureInfo.InvariantCulture, DateTimeStyles.AllowTrailingWhite, new DateTime(DateTime.Now.Year, 1, 9, 0, 0, 0) };
            yield return new object[] { "15", "dd\' \'", CultureInfo.InvariantCulture, DateTimeStyles.AllowTrailingWhite, new DateTime(DateTime.Now.Year, 1, 15, 0, 0, 0) };

            yield return new object[] { "9", "%M\"  \"", CultureInfo.InvariantCulture, DateTimeStyles.AllowTrailingWhite, new DateTime(DateTime.Now.Year, 9, 1, 0, 0, 0) };
            yield return new object[] { "09", "MM\" \"", CultureInfo.InvariantCulture, DateTimeStyles.AllowTrailingWhite, new DateTime(DateTime.Now.Year, 9, 1, 0, 0, 0) };
            yield return new object[] { "Sep", "MMM\"  \"", CultureInfo.InvariantCulture, DateTimeStyles.AllowTrailingWhite, new DateTime(DateTime.Now.Year, 9, 1, 0, 0, 0) };
            yield return new object[] { "September", "MMMM\' \'", CultureInfo.InvariantCulture, DateTimeStyles.AllowTrailingWhite, new DateTime(DateTime.Now.Year, 9, 1, 0, 0, 0) };

            yield return new object[] { "1", "%y\' \'", CultureInfo.InvariantCulture, DateTimeStyles.AllowTrailingWhite, new DateTime(2001, 1, 1, 0, 0, 0) };
            yield return new object[] { "01", "yy\"  \"", CultureInfo.InvariantCulture, DateTimeStyles.AllowTrailingWhite, new DateTime(2001, 1, 1, 0, 0, 0) };
            yield return new object[] { "2001", "yyyy\" \"", CultureInfo.InvariantCulture, DateTimeStyles.AllowTrailingWhite, new DateTime(2001, 1, 1, 0, 0, 0) };

            yield return new object[] { "3", "%H\"  \"", CultureInfo.InvariantCulture, DateTimeStyles.AllowTrailingWhite, DateTime.Today + TimeSpan.FromHours(3) };
            yield return new object[] { "03", "HH\" \"", CultureInfo.InvariantCulture, DateTimeStyles.AllowTrailingWhite, DateTime.Today + TimeSpan.FromHours(3) };

            yield return new object[] { "3A", "ht\"  \"", CultureInfo.InvariantCulture, DateTimeStyles.AllowTrailingWhite, DateTime.Today + TimeSpan.FromHours(3) };
            yield return new object[] { "03A", "hht\" \"", CultureInfo.InvariantCulture, DateTimeStyles.AllowTrailingWhite, DateTime.Today + TimeSpan.FromHours(3) };
            yield return new object[] { "3P", "ht\'  \'", CultureInfo.InvariantCulture, DateTimeStyles.AllowTrailingWhite, DateTime.Today + TimeSpan.FromHours(12 + 3) };
            yield return new object[] { "03P", "hht\" \"", CultureInfo.InvariantCulture, DateTimeStyles.AllowTrailingWhite, DateTime.Today + TimeSpan.FromHours(12 + 3) };

            yield return new object[] { "2017-10-11 01:23:45Z", "u", CultureInfo.InvariantCulture, DateTimeStyles.AllowTrailingWhite, new DateTime(2017, 10, 11, 1, 23, 45) };
            yield return new object[] { "9/8/2017 10:11:12 AM", "M/d/yyyy HH':'mm':'ss tt\'  \'", CultureInfo.InvariantCulture, DateTimeStyles.AllowTrailingWhite, new DateTime(2017, 9, 8, 10, 11, 12) };
            yield return new object[] { "9/8/2017 20:11:12 PM", "M/d/yyyy HH':'mm':'ss tt\" \"", CultureInfo.InvariantCulture, DateTimeStyles.AllowTrailingWhite, new DateTime(2017, 9, 8, 20, 11, 12) };
            yield return new object[] { "1234-05-06T07:00:00.8Z", "yyyy-MM-dd'T'HH:mm:ss.FFF'Z'\" \"", CultureInfo.InvariantCulture, DateTimeStyles.AllowTrailingWhite, new DateTime(1234, 5, 6, 7, 0, 0, 800) };
            yield return new object[] { "1234-05-06T07:00:00Z", "yyyy-MM-dd'T'HH:mm:ss.FFF'Z'\"  \"", CultureInfo.InvariantCulture, DateTimeStyles.AllowTrailingWhite, new DateTime(1234, 5, 6, 7, 0, 0, 0) };
            yield return new object[] { "1234-05-06T07:00:00Z", "yyyy-MM-dd'T'HH:mm:ssFFF'Z'\' \'", CultureInfo.InvariantCulture, DateTimeStyles.AllowTrailingWhite, new DateTime(1234, 5, 6, 7, 0, 0, 0) };
            yield return new object[] { "1234-05-06T07:00:00Z", "yyyy-MM-dd'T'HH:mm:ssFFF'Z'\'  \'", CultureInfo.InvariantCulture, DateTimeStyles.AllowTrailingWhite, new DateTime(1234, 5, 6, 7, 0, 0, 0) };
            yield return new object[] { "1234-05-06T07:00:00Z", "yyyy-MM-dd'T'HH:mm:ssFFFZ\" \"", CultureInfo.InvariantCulture, DateTimeStyles.AllowTrailingWhite, TimeZoneInfo.ConvertTimeFromUtc(new DateTime(1234, 5, 6, 7, 0, 0, DateTimeKind.Utc), TimeZoneInfo.Local) };
            yield return new object[] { "1234-05-06T07:00:00GMT", "yyyy-MM-dd'T'HH:mm:ssFFFZ\"  \"", CultureInfo.InvariantCulture, DateTimeStyles.AllowTrailingWhite, TimeZoneInfo.ConvertTimeFromUtc(new DateTime(1234, 5, 6, 7, 0, 0, DateTimeKind.Utc), TimeZoneInfo.Local) };

            yield return new object[] { "9/8/2017 10:11:12 AM                                          ", "M/d/yyyy HH':'mm':'ss tt\'  \'", CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, new DateTime(2017, 9, 8, 10, 11, 12) };
            yield return new object[] { "9/8/2017 10:11:12 AM       ", "M/d/yyyy HH':'mm':'ss tt\'  \'", CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, new DateTime(2017, 9, 8, 10, 11, 12) };
            yield return new object[] { "9/ 8    /2017    10:11:12 AM       ", "M/d/yyyy HH':'mm':'ss tt\'  \'", CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, new DateTime(2017, 9, 8, 10, 11, 12) };
            yield return new object[] { "   9   /8/2017       10:11:12 AM", "M/d/yyyy HH':'mm':'ss tt\'  \'", CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, new DateTime(2017, 9, 8, 10, 11, 12) };
            yield return new object[] { "9/8/2017 10 : 11 : 12 AM", "M/d/yyyy HH':'mm':'ss tt\'  \'", CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, new DateTime(2017, 9, 8, 10, 11, 12) };
            yield return new object[] { " 9 / 8 / 2017    10 : 11 : 12 AM", "M/d/yyyy HH':'mm':'ss tt\'  \'", CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, new DateTime(2017, 9, 8, 10, 11, 12) };
            yield return new object[] { "   9   /   8   /   2017    10  :   11  :   12  AM", "M/d/yyyy HH':'mm':'ss tt\'  \'", CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, new DateTime(2017, 9, 8, 10, 11, 12) };

            var hebrewCulture = new CultureInfo("he-IL");
            hebrewCulture.DateTimeFormat.Calendar = new HebrewCalendar();
            DateTime today = DateTime.Today;
            foreach (string pattern in hebrewCulture.DateTimeFormat.GetAllDateTimePatterns())
            {
                yield return new object[] { today.ToString(pattern, hebrewCulture), pattern, hebrewCulture, DateTimeStyles.None, null };
            }
        }

        [Theory]
        [MemberData(nameof(ParseExact_ValidInput_Succeeds_MemberData))]
        public static void ParseExact_ValidInput_Succeeds(string input, string format, CultureInfo culture, DateTimeStyles style, DateTime? expected)
        {
            DateTime result1 = DateTime.ParseExact(input, format, culture, style);
            DateTime result2 = DateTime.ParseExact(input, new[] { format }, culture, style);

            Assert.True(DateTime.TryParseExact(input, format, culture, style, out DateTime result3));
            Assert.True(DateTime.TryParseExact(input, new[] { format }, culture, style, out DateTime result4));

            Assert.Equal(result1, result2);
            Assert.Equal(result1, result3);
            Assert.Equal(result1, result4);

            if (expected != null) // some inputs don't roundtrip well
            {
                // Normalize values to make comparison easier
                if (expected.Value.Kind != DateTimeKind.Utc)
                {
                    expected = expected.Value.ToUniversalTime();
                }
                if (result1.Kind != DateTimeKind.Utc)
                {
                    result1 = result1.ToUniversalTime();
                }

                Assert.Equal(expected, result1);
            }
        }

        public static IEnumerable<object[]> ParseExact_InvalidInputs_Fail_MemberData()
        {
            yield return new object[] { "6/28/2004 13:00:00 AM", "M/d/yyyy HH':'mm':'ss tt", CultureInfo.InvariantCulture, DateTimeStyles.None };
            yield return new object[] { "6/28/2004 03:00:00 PM", "M/d/yyyy HH':'mm':'ss tt", CultureInfo.InvariantCulture, DateTimeStyles.None };

            yield return new object[] { "1", "dd", CultureInfo.InvariantCulture, DateTimeStyles.None };
            yield return new object[] { "99", "dd", CultureInfo.InvariantCulture, DateTimeStyles.None };
            yield return new object[] { "123", "dd", CultureInfo.InvariantCulture, DateTimeStyles.None };

            yield return new object[] { "1", "mm", CultureInfo.InvariantCulture, DateTimeStyles.None };
            yield return new object[] { "99", "mm", CultureInfo.InvariantCulture, DateTimeStyles.None };
            yield return new object[] { "123", "mm", CultureInfo.InvariantCulture, DateTimeStyles.None };

            yield return new object[] { "1", "ss", CultureInfo.InvariantCulture, DateTimeStyles.None };
            yield return new object[] { "99", "ss", CultureInfo.InvariantCulture, DateTimeStyles.None };
            yield return new object[] { "123", "ss", CultureInfo.InvariantCulture, DateTimeStyles.None };

            yield return new object[] { "1", "MM", CultureInfo.InvariantCulture, DateTimeStyles.None };
            yield return new object[] { "99", "MM", CultureInfo.InvariantCulture, DateTimeStyles.None };
            yield return new object[] { "Fep", "MMM", CultureInfo.InvariantCulture, DateTimeStyles.None };
            yield return new object[] { "Jantember", "MMMM", CultureInfo.InvariantCulture, DateTimeStyles.None };

            yield return new object[] { "123", "YY", CultureInfo.InvariantCulture, DateTimeStyles.None };
            yield return new object[] { "12345", "YYYY", CultureInfo.InvariantCulture, DateTimeStyles.None };

            yield return new object[] { "1", "HH", CultureInfo.InvariantCulture, DateTimeStyles.None };
            yield return new object[] { "99", "HH", CultureInfo.InvariantCulture, DateTimeStyles.None };
            yield return new object[] { "123", "HH", CultureInfo.InvariantCulture, DateTimeStyles.None };

            yield return new object[] { "1", "hh", CultureInfo.InvariantCulture, DateTimeStyles.None };
            yield return new object[] { "99", "hh", CultureInfo.InvariantCulture, DateTimeStyles.None };
            yield return new object[] { "123", "hh", CultureInfo.InvariantCulture, DateTimeStyles.None };

            yield return new object[] { "1", "ff", CultureInfo.InvariantCulture, DateTimeStyles.None };
            yield return new object[] { "123", "ff", CultureInfo.InvariantCulture, DateTimeStyles.None };
            yield return new object[] { "123456", "fffff", CultureInfo.InvariantCulture, DateTimeStyles.None };
            yield return new object[] { "1234", "fffff", CultureInfo.InvariantCulture, DateTimeStyles.None };

            yield return new object[] { "AM", "t", CultureInfo.InvariantCulture, DateTimeStyles.None };
            yield return new object[] { "PM", "t", CultureInfo.InvariantCulture, DateTimeStyles.None };
            yield return new object[] { "PM", "ttt", CultureInfo.InvariantCulture, DateTimeStyles.None };
            yield return new object[] { "AAM", "tt", CultureInfo.InvariantCulture, DateTimeStyles.None };
            yield return new object[] { "CM", "tt", CultureInfo.InvariantCulture, DateTimeStyles.None };
        }

        [Theory]
        [MemberData(nameof(ParseExact_InvalidInputs_Fail_MemberData))]
        public static void ParseExact_InvalidInputs_Fail(string input, string format, CultureInfo culture, DateTimeStyles style)
        {
            Assert.Throws<FormatException>(() => DateTime.ParseExact(input, format, culture, style));
            Assert.Throws<FormatException>(() => DateTime.ParseExact(input, new[] { format }, culture, style));

            Assert.False(DateTime.TryParseExact(input, format, culture, style, out DateTime result));
            Assert.False(DateTime.TryParseExact(input, new[] { format }, culture, style, out result));
        }

        public static IEnumerable<object[]> ToString_MatchesExpected_MemberData()
        {
            // Randomly generated data on netfx with:
            //     using System;
            //     class Program
            //     {
            //         static void Main()
            //         {
            //             var rand = new Random(42);
            //             var bytes = new byte[8];
            //             int i = 0;
            //             while (i < 40)
            //             {
            //                 DateTimeKind kind = rand.Next(2) == 0 ? DateTimeKind.Utc : DateTimeKind.Unspecified;
            //                 string format;
            //                 switch (rand.Next(4))
            //                 {
            //                     case 0: format = "o"; break;
            //                     case 1: format = "O"; break;
            //                     case 2: format = "r"; break;
            //                     default: format = "R"; break;
            //                 }
            //
            //                 try
            //                 {
            //                     rand.NextBytes(bytes);
            //                     long seed = BitConverter.ToInt64(bytes, 0);
            //                     var dt = new DateTime(seed, kind);
            //                     Console.WriteLine($"yield return new object[] {{ new DateTime({seed}, DateTimeKind.{kind}), \"{format}\", \"{dt.ToString(format)}\" }};");
            //                     i++;
            //                 }
            //                 catch { }
            //             }
            //         }
            //}

            yield return new object[] { new DateTime(2688006240964947440, DateTimeKind.Utc), "O", "8518-12-15T08:01:36.4947440Z" };
            yield return new object[] { new DateTime(2461197105169450509, DateTimeKind.Utc), "r", "Sun, 23 Mar 7800 18:15:16 GMT" };
            yield return new object[] { new DateTime(71363981510699949, DateTimeKind.Unspecified), "R", "Fri, 23 Feb 0227 04:49:11 GMT" };
            yield return new object[] { new DateTime(1678426538898407093, DateTimeKind.Unspecified), "R", "Fri, 22 Sep 5319 07:24:49 GMT" };
            yield return new object[] { new DateTime(2689041307785948711, DateTimeKind.Utc), "o", "8522-03-27T07:52:58.5948711Z" };
            yield return new object[] { new DateTime(996610247053299209, DateTimeKind.Unspecified), "r", "Thu, 19 Feb 3159 01:58:25 GMT" };
            yield return new object[] { new DateTime(3105391438361510074, DateTimeKind.Unspecified), "R", "Fri, 06 Aug 9841 01:17:16 GMT" };
            yield return new object[] { new DateTime(946433487657072106, DateTimeKind.Utc), "R", "Mon, 17 Feb 3000 03:06:05 GMT" };
            yield return new object[] { new DateTime(2521748413631767931, DateTimeKind.Unspecified), "R", "Sat, 08 Feb 7992 07:02:43 GMT" };
            yield return new object[] { new DateTime(49349519375012969, DateTimeKind.Utc), "R", "Fri, 20 May 0157 11:58:57 GMT" };
            yield return new object[] { new DateTime(796677276139881359, DateTimeKind.Utc), "o", "2525-07-28T04:20:13.9881359Z" };
            yield return new object[] { new DateTime(3022911536338429542, DateTimeKind.Unspecified), "R", "Mon, 24 Mar 9580 04:53:53 GMT" };
            yield return new object[] { new DateTime(1144652135553351618, DateTimeKind.Utc), "R", "Tue, 04 Apr 3628 20:39:15 GMT" };
            yield return new object[] { new DateTime(2570858096011770291, DateTimeKind.Unspecified), "o", "8147-09-23T04:53:21.1770291" };
            yield return new object[] { new DateTime(15695724649124585, DateTimeKind.Unspecified), "R", "Tue, 27 Sep 0050 08:21:04 GMT" };
            yield return new object[] { new DateTime(1503933934291527034, DateTimeKind.Unspecified), "O", "4766-10-12T06:37:09.1527034" };
            yield return new object[] { new DateTime(2688603665097410101, DateTimeKind.Unspecified), "r", "Tue, 05 Nov 8520 19:08:29 GMT" };
            yield return new object[] { new DateTime(1310336900529542610, DateTimeKind.Unspecified), "r", "Tue, 17 Apr 4153 15:14:12 GMT" };
            yield return new object[] { new DateTime(2313720085584182693, DateTimeKind.Unspecified), "O", "7332-11-20T18:22:38.4182693" };
            yield return new object[] { new DateTime(2291958603891779335, DateTimeKind.Unspecified), "o", "7263-12-05T20:46:29.1779335" };
            yield return new object[] { new DateTime(262036413643976979, DateTimeKind.Unspecified), "o", "0831-05-12T21:16:04.3976979" };
            yield return new object[] { new DateTime(684781207384421044, DateTimeKind.Utc), "O", "2170-12-26T20:12:18.4421044Z" };
            yield return new object[] { new DateTime(1444462249169683325, DateTimeKind.Utc), "r", "Mon, 27 Apr 4578 07:21:56 GMT" };
            yield return new object[] { new DateTime(1155518137384061537, DateTimeKind.Unspecified), "r", "Sun, 10 Sep 3662 06:02:18 GMT" };
            yield return new object[] { new DateTime(2333390479532380569, DateTimeKind.Unspecified), "O", "7395-03-22T10:12:33.2380569" };
            yield return new object[] { new DateTime(2217528014591554502, DateTimeKind.Unspecified), "R", "Sat, 26 Jan 7028 08:24:19 GMT" };
            yield return new object[] { new DateTime(2764551324904480205, DateTimeKind.Utc), "O", "8761-07-08T04:21:30.4480205Z" };
            yield return new object[] { new DateTime(2880903932678729712, DateTimeKind.Utc), "O", "9130-03-23T13:14:27.8729712Z" };
            yield return new object[] { new DateTime(507699902578704433, DateTimeKind.Utc), "O", "1609-11-02T15:04:17.8704433Z" };
            yield return new object[] { new DateTime(2429953022324426129, DateTimeKind.Utc), "O", "7701-03-20T15:03:52.4426129Z" };
            yield return new object[] { new DateTime(603147512164908366, DateTimeKind.Unspecified), "O", "1912-04-20T09:33:36.4908366" };
            yield return new object[] { new DateTime(2900400428644841236, DateTimeKind.Utc), "R", "Thu, 02 Jan 9192 22:34:24 GMT" };
            yield return new object[] { new DateTime(1710845568474490805, DateTimeKind.Utc), "O", "5422-06-16T08:00:47.4490805Z" };
            yield return new object[] { new DateTime(2988999715803714268, DateTimeKind.Utc), "r", "Sun, 06 Oct 9472 09:53:00 GMT" };
            yield return new object[] { new DateTime(1068133489112689365, DateTimeKind.Utc), "r", "Wed, 12 Oct 3385 14:41:51 GMT" };
            yield return new object[] { new DateTime(798784044525059284, DateTimeKind.Unspecified), "R", "Mon, 31 Mar 2532 13:40:52 GMT" };
            yield return new object[] { new DateTime(2561736813034040593, DateTimeKind.Utc), "O", "8118-10-28T03:55:03.4040593Z" };
            yield return new object[] { new DateTime(1677975383149674547, DateTimeKind.Utc), "o", "5318-04-18T03:18:34.9674547Z" };
            yield return new object[] { new DateTime(1101778442151366156, DateTimeKind.Utc), "O", "3492-05-25T12:43:35.1366156Z" };
            yield return new object[] { new DateTime(221550163152616218, DateTimeKind.Utc), "r", "Sun, 25 Jan 0703 19:11:55 GMT" };
        }

        [Theory]
        [MemberData(nameof(ToString_MatchesExpected_MemberData))]
        public static void ToString_MatchesExpected(DateTime dateTime, string format, string expected)
        {
            Assert.Equal(expected, dateTime.ToString(format));
        }
    }
}
