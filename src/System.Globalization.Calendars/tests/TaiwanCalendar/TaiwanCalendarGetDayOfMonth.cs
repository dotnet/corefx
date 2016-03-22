// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Globalization.Tests
{
    public class TaiwanCalendarGetDayOfMonth
    {
        public static IEnumerable<object[]> GetDayOfMonth_TestData()
        {
            yield return new object[] { new TaiwanCalendar().MinSupportedDateTime };
            yield return new object[] { new TaiwanCalendar().MaxSupportedDateTime };
            yield return new object[] { TaiwanCalendarUtilities.RandomDateTime() };
            for (int i = 1; i <= 29; i++)
            {
                yield return new object[] { new DateTime(2000, 2, i) };
            }
        }

        [Theory]
        [MemberData(nameof(GetDayOfMonth_TestData))]
        public void GetDayOfMonth(DateTime time)
        {
            Assert.Equal(time.Day, new TaiwanCalendar().GetDayOfMonth(time));
        }
    }
}
