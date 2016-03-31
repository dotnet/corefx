// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Globalization.Tests
{
    public class DateTimeFormatInfoGetMonthName
    {
        private const int MinMonth = 1;
        private const int MaxMonth = 13;

        public static IEnumerable<object[]> GetMonthName_TestData()
        {
            string[] englishMonthNames = new string[] { "", "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December", "" };
            yield return new object[] { DateTimeFormatInfo.InvariantInfo, englishMonthNames };
            yield return new object[] { new DateTimeFormatInfo(), englishMonthNames };
            yield return new object[] { new CultureInfo("en-US").DateTimeFormat, englishMonthNames };

            // ActiveIssue(2103)
            if (!PlatformDetection.IsUbuntu1510)
            {
                yield return new object[] { new CultureInfo("fr-FR").DateTimeFormat, new string[] { "", "janvier", "f\u00E9vrier", "mars", "avril", "mai", "juin", "juillet", "ao\u00FBt", "septembre", "octobre", "novembre", "d\u00E9cembre", "" } };
            }
        }

        [Theory]
        [MemberData(nameof(GetMonthName_TestData))]
        public void GetMonthName(DateTimeFormatInfo format, string[] expected)
        {
            for (int i = MinMonth; i <= MaxMonth; ++i)
            {
                Assert.Equal(expected[i], format.GetMonthName(i));
            }
        }

        [Fact]
        public void GetMonthName_Invalid()
        {
            Assert.Throws<ArgumentOutOfRangeException>("month", () => new DateTimeFormatInfo().GetMonthName(MinMonth - 1));
            Assert.Throws<ArgumentOutOfRangeException>("month", () => new DateTimeFormatInfo().GetMonthName(MaxMonth + 1));
        }
    }
}
