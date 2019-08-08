// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Globalization.Tests
{
    public class ThaiBuddhistCalendarToFourDigitYear
    {
        [Theory]
        [InlineData(0)]
        [InlineData(46)]
        [InlineData(5139)]
        [InlineData(10542)]
        public void ToFourDigitYear(int year)
        {
            ThaiBuddhistCalendar calendar = new ThaiBuddhistCalendar();
            calendar.TwoDigitYearMax = 2029;
            if (year > 99)
            {
                Assert.Equal(year, calendar.ToFourDigitYear(year));
            }
            else if (year > 29)
            {
                Assert.Equal(year + 1900, calendar.ToFourDigitYear(year));
            }
            else
            {
                Assert.Equal(year + 2000, calendar.ToFourDigitYear(year));
            }
        }
    }
}
