// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Runtime.InteropServices;
using Xunit;

namespace System.Tests
{
    public static partial class TimeZoneInfoTests
    {
        [Fact]
        public static void ClearCachedData()
        {
            TimeZoneInfo cst = TimeZoneInfo.FindSystemTimeZoneById(s_strSydney);
            TimeZoneInfo local = TimeZoneInfo.Local;

            TimeZoneInfo.ClearCachedData();
            Assert.ThrowsAny<ArgumentException>(() =>
            {
                TimeZoneInfo.ConvertTime(DateTime.Now, local, cst);
            });
        }

        [Fact]
        public static void ConvertTime_DateTimeOffset_NullDestination_ArgumentNullException()
        {
            DateTimeOffset time1 = new DateTimeOffset(2006, 5, 12, 0, 0, 0, TimeSpan.Zero);
            VerifyConvertException<ArgumentNullException>(time1, null);
        }

        public static IEnumerable<object[]> ConvertTime_DateTimeOffset_InvalidDestination_TimeZoneNotFoundException_MemberData()
        {
            yield return new object[] { string.Empty };
            yield return new object[] { "    " };
            yield return new object[] { "\0" };
            yield return new object[] { s_strPacific.Substring(0, s_strPacific.Length / 2) }; // whole string must match
            yield return new object[] { s_strPacific + " Zone" }; // no extra characters
            yield return new object[] { " " + s_strPacific }; // no leading space
            yield return new object[] { s_strPacific + " " }; // no trailing space
            yield return new object[] { "\0" + s_strPacific }; // no leading null
            yield return new object[] { s_strPacific + "\0" }; // no trailing null
            yield return new object[] { s_strPacific + "\\  " }; // no trailing null
            yield return new object[] { s_strPacific + "\\Display" };
            yield return new object[] { s_strPacific + "\n" }; // no trailing newline
            yield return new object[] { new string('a', 100) }; // long string
        }

        [Theory]
        [MemberData(nameof(ConvertTime_DateTimeOffset_InvalidDestination_TimeZoneNotFoundException_MemberData))]
        public static void ConvertTime_DateTimeOffset_InvalidDestination_TimeZoneNotFoundException(string destinationId)
        {
            DateTimeOffset time1 = new DateTimeOffset(2006, 5, 12, 0, 0, 0, TimeSpan.Zero);
            VerifyConvertException<TimeZoneNotFoundException>(time1, destinationId);
        }

        [Fact]
        public static void ConvertTimeFromUtc()
        {
            // destination timezone is null
            Assert.Throws<ArgumentNullException>(() =>
            {
                DateTime dt = TimeZoneInfo.ConvertTimeFromUtc(new DateTime(2007, 5, 3, 11, 8, 0), null);
            });

            // destination timezone is UTC
            DateTime now = DateTime.UtcNow;
            DateTime convertedNow = TimeZoneInfo.ConvertTimeFromUtc(now, TimeZoneInfo.Utc);
            Assert.Equal(now, convertedNow);
        }

        [Fact]
        public static void ConvertTimeToUtc()
        {
            // null source
            VerifyConvertToUtcException<ArgumentNullException>(new DateTime(2007, 5, 3, 12, 16, 0), null);

            TimeZoneInfo london = CreateCustomLondonTimeZone();

            // invalid DateTime
            DateTime invalidDate = new DateTime(2007, 3, 25, 1, 30, 0);
            VerifyConvertToUtcException<ArgumentException>(invalidDate, london);

            // DateTimeKind and source types don't match
            VerifyConvertToUtcException<ArgumentException>(new DateTime(2007, 5, 3, 12, 8, 0, DateTimeKind.Utc), london);

            // correct UTC conversion
            DateTime date = new DateTime(2007, 01, 01, 0, 0, 0);
            Assert.Equal(date.ToUniversalTime(), TimeZoneInfo.ConvertTimeToUtc(date));
        }

        [Fact]
        public static void ConvertTimeFromToUtc()
        {
            TimeZoneInfo london = CreateCustomLondonTimeZone();

            DateTime utc = DateTime.UtcNow;
            Assert.Equal(DateTimeKind.Utc, utc.Kind);

            DateTime converted = TimeZoneInfo.ConvertTimeFromUtc(utc, TimeZoneInfo.Utc);
            Assert.Equal(DateTimeKind.Utc, converted.Kind);
            DateTime back = TimeZoneInfo.ConvertTimeToUtc(converted, TimeZoneInfo.Utc);
            Assert.Equal(DateTimeKind.Utc, back.Kind);
            Assert.Equal(utc, back);

            converted = TimeZoneInfo.ConvertTimeFromUtc(utc, TimeZoneInfo.Local);
            DateTimeKind expectedKind = (TimeZoneInfo.Local == TimeZoneInfo.Utc) ? DateTimeKind.Utc : DateTimeKind.Local;
            Assert.Equal(expectedKind, converted.Kind);
            back = TimeZoneInfo.ConvertTimeToUtc(converted, TimeZoneInfo.Local);
            Assert.Equal(back.Kind, DateTimeKind.Utc);
            Assert.Equal(utc, back);
        }

        [Fact]
        [PlatformSpecific(TestPlatforms.AnyUnix)]
        public static void ConvertTimeFromToUtc_UnixOnly()
        {
            // DateTime Kind is Local
            Assert.ThrowsAny<ArgumentException>(() =>
            {
                DateTime dt = TimeZoneInfo.ConvertTimeFromUtc(new DateTime(2007, 5, 3, 11, 8, 0, DateTimeKind.Local), TimeZoneInfo.Local);
            });

            TimeZoneInfo london = CreateCustomLondonTimeZone();

            // winter (no DST)
            DateTime winter = new DateTime(2007, 12, 25, 12, 0, 0);
            DateTime convertedWinter = TimeZoneInfo.ConvertTimeFromUtc(winter, london);
            Assert.Equal(winter, convertedWinter);

            // summer (DST)
            DateTime summer = new DateTime(2007, 06, 01, 12, 0, 0);
            DateTime convertedSummer = TimeZoneInfo.ConvertTimeFromUtc(summer, london);
            Assert.Equal(summer + new TimeSpan(1, 0, 0), convertedSummer);

            // Kind and source types don't match
            VerifyConvertToUtcException<ArgumentException>(new DateTime(2007, 5, 3, 12, 8, 0, DateTimeKind.Local), london);

            // convert to London time and back
            DateTime utc = DateTime.UtcNow;
            Assert.Equal(DateTimeKind.Utc, utc.Kind);
            DateTime converted = TimeZoneInfo.ConvertTimeFromUtc(utc, london);
            Assert.Equal(converted.Kind, DateTimeKind.Unspecified);
            DateTime back = TimeZoneInfo.ConvertTimeToUtc(converted, london);
            Assert.Equal(back.Kind, DateTimeKind.Utc);
            Assert.Equal(utc, back);
        }

        [Fact]
        public static void CreateCustomTimeZone()
        {
            TimeZoneInfo.TransitionTime s1 = TimeZoneInfo.TransitionTime.CreateFloatingDateRule(new DateTime(1, 1, 1, 4, 0, 0), 3, 2, DayOfWeek.Sunday);
            TimeZoneInfo.TransitionTime e1 = TimeZoneInfo.TransitionTime.CreateFloatingDateRule(new DateTime(1, 1, 1, 4, 0, 0), 10, 2, DayOfWeek.Sunday);
            TimeZoneInfo.AdjustmentRule r1 = TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(new DateTime(2000, 1, 1), new DateTime(2005, 1, 1), new TimeSpan(1, 0, 0), s1, e1);

            // supports DST
            TimeZoneInfo tz1 = TimeZoneInfo.CreateCustomTimeZone("mytimezone", new TimeSpan(6, 0, 0), null, null, null, new TimeZoneInfo.AdjustmentRule[] { r1 });
            Assert.True(tz1.SupportsDaylightSavingTime);

            // doesn't support DST
            TimeZoneInfo tz2 = TimeZoneInfo.CreateCustomTimeZone("mytimezone", new TimeSpan(4, 0, 0), null, null, null, new TimeZoneInfo.AdjustmentRule[] { r1 }, true);
            Assert.False(tz2.SupportsDaylightSavingTime);

            TimeZoneInfo tz3 = TimeZoneInfo.CreateCustomTimeZone("mytimezone", new TimeSpan(6, 0, 0), null, null, null, null);
            Assert.False(tz3.SupportsDaylightSavingTime);
        }

        [Fact]
        public static void CreateCustomTimeZone_Invalid()
        {
            VerifyCustomTimeZoneException<ArgumentNullException>(null, new TimeSpan(0), null, null);                // null Id
            VerifyCustomTimeZoneException<ArgumentException>("", new TimeSpan(0), null, null);                      // empty string Id
            VerifyCustomTimeZoneException<ArgumentException>("mytimezone", new TimeSpan(0, 0, 55), null, null);     // offset not minutes
            VerifyCustomTimeZoneException<ArgumentException>("mytimezone", new TimeSpan(14, 1, 0), null, null);     // offset too big
            VerifyCustomTimeZoneException<ArgumentException>("mytimezone", - new TimeSpan(14, 1, 0), null, null);   // offset too small
        }

        [Fact]
        public static void CreateCustomTimeZone_InvalidTimeZone()
        {
            TimeZoneInfo.TransitionTime s1 = TimeZoneInfo.TransitionTime.CreateFloatingDateRule(new DateTime(1, 1, 1, 4, 0, 0), 3, 2, DayOfWeek.Sunday);
            TimeZoneInfo.TransitionTime e1 = TimeZoneInfo.TransitionTime.CreateFloatingDateRule(new DateTime(1, 1, 1, 4, 0, 0), 10, 2, DayOfWeek.Sunday);
            TimeZoneInfo.TransitionTime s2 = TimeZoneInfo.TransitionTime.CreateFloatingDateRule(new DateTime(1, 1, 1, 4, 0, 0), 2, 2, DayOfWeek.Sunday);
            TimeZoneInfo.TransitionTime e2 = TimeZoneInfo.TransitionTime.CreateFloatingDateRule(new DateTime(1, 1, 1, 4, 0, 0), 11, 2, DayOfWeek.Sunday);

            TimeZoneInfo.AdjustmentRule r1 = TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(new DateTime(2000, 1, 1), new DateTime(2005, 1, 1), new TimeSpan(1, 0, 0), s1, e1);
            
            // AdjustmentRules overlap
            TimeZoneInfo.AdjustmentRule r2 = TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(new DateTime(2004, 1, 1), new DateTime(2007, 1, 1), new TimeSpan(1, 0, 0), s2, e2);
            VerifyCustomTimeZoneException<InvalidTimeZoneException>("mytimezone", new TimeSpan(6, 0, 0), null, null, null, new TimeZoneInfo.AdjustmentRule[] { r1, r2 });

            // AdjustmentRules not ordered
            TimeZoneInfo.AdjustmentRule r3 = TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(new DateTime(2006, 1, 1), new DateTime(2007, 1, 1), new TimeSpan(1, 0, 0), s2, e2);
            VerifyCustomTimeZoneException<InvalidTimeZoneException>("mytimezone", new TimeSpan(6, 0, 0), null, null, null, new TimeZoneInfo.AdjustmentRule[] { r3, r1 });

            // Offset out of range
            TimeZoneInfo.AdjustmentRule r4 = TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(new DateTime(2000, 1, 1), new DateTime(2005, 1, 1), new TimeSpan(3, 0, 0), s1, e1);
            VerifyCustomTimeZoneException<InvalidTimeZoneException>("mytimezone", new TimeSpan(12, 0, 0), null, null, null, new TimeZoneInfo.AdjustmentRule[] { r4 });

            // overlapping AdjustmentRules for a date
            TimeZoneInfo.AdjustmentRule r5 = TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(new DateTime(2005, 1, 1), new DateTime(2007, 1, 1), new TimeSpan(1, 0, 0), s2, e2);
            VerifyCustomTimeZoneException<InvalidTimeZoneException>("mytimezone", new TimeSpan(6, 0, 0), null, null, null, new TimeZoneInfo.AdjustmentRule[] { r1, r5 });

            // null AdjustmentRule
            VerifyCustomTimeZoneException<InvalidTimeZoneException>("mytimezone", new TimeSpan(12, 0, 0), null, null, null, new TimeZoneInfo.AdjustmentRule[] { null });
        }

        [Fact]
        [PlatformSpecific(TestPlatforms.AnyUnix)]
        public static void HasSameRules_RomeAndVatican()
        {
            TimeZoneInfo rome = TimeZoneInfo.FindSystemTimeZoneById("Europe/Rome");
            TimeZoneInfo vatican = TimeZoneInfo.FindSystemTimeZoneById("Europe/Vatican");
            Assert.True(rome.HasSameRules(vatican));
        }

        [Fact]
        public static void HasSameRules_NullAdjustmentRules()
        {
            TimeZoneInfo utc = TimeZoneInfo.Utc;
            TimeZoneInfo custom = TimeZoneInfo.CreateCustomTimeZone("Custom", new TimeSpan(0), "Custom", "Custom");
            Assert.True(utc.HasSameRules(custom));
        }

        private static TimeZoneInfo CreateCustomLondonTimeZone()
        {
            TimeZoneInfo.TransitionTime start = TimeZoneInfo.TransitionTime.CreateFloatingDateRule(new DateTime(1, 1, 1, 1, 0, 0), 3, 5, DayOfWeek.Sunday);
            TimeZoneInfo.TransitionTime end = TimeZoneInfo.TransitionTime.CreateFloatingDateRule(new DateTime(1, 1, 1, 2, 0, 0), 10, 5, DayOfWeek.Sunday);
            TimeZoneInfo.AdjustmentRule rule = TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(DateTime.MinValue.Date, DateTime.MaxValue.Date, new TimeSpan(1, 0, 0), start, end);
            return TimeZoneInfo.CreateCustomTimeZone("Europe/London", new TimeSpan(0), "Europe/London", "British Standard Time", "British Summer Time", new TimeZoneInfo.AdjustmentRule[] { rule });
        }

        private static void VerifyConvertToUtcException<EXCTYPE>(DateTime dateTime, TimeZoneInfo sourceTimeZone) where EXCTYPE : Exception
        {
            Assert.ThrowsAny<EXCTYPE>(() =>
            {
                DateTime dt = TimeZoneInfo.ConvertTimeToUtc(dateTime, sourceTimeZone);
            });
        }

        private static void VerifyCustomTimeZoneException<ExceptionType>(string id, TimeSpan baseUtcOffset, string displayName, string standardDisplayName, string daylightDisplayName = null, TimeZoneInfo.AdjustmentRule[] adjustmentRules = null) where ExceptionType : Exception
        {
            Assert.ThrowsAny<ExceptionType>(() =>
            {
                if (daylightDisplayName == null && adjustmentRules == null)
                {
                    TimeZoneInfo tz = TimeZoneInfo.CreateCustomTimeZone(id, baseUtcOffset, displayName, standardDisplayName);
                }
                else
                {
                    TimeZoneInfo tz = TimeZoneInfo.CreateCustomTimeZone(id, baseUtcOffset, displayName, standardDisplayName, daylightDisplayName, adjustmentRules);
                }
            });
        }
    }
}