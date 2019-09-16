// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Globalization.Tests
{
    public class CultureInfoClone
    {
        public static IEnumerable<object[]> Clone_TestData()
        {
            yield return new object[] { new CultureInfo(CultureInfo.InvariantCulture.Name) };
            yield return new object[] { CultureInfo.InvariantCulture };
            yield return new object[] { new CultureInfo("fr-FR") };
            yield return new object[] { new CultureInfo("en") };
        }

        [Theory]
        [MemberData(nameof(Clone_TestData))]
        public void Clone(CultureInfo culture)
        {
            CultureInfo clone = (CultureInfo)culture.Clone();
            Assert.Equal(culture, clone);
            Assert.NotSame(clone, culture);
        }

        [Fact]
        public void TestCalendarAfterCloning()
        {
            CultureInfo ci = new CultureInfo("en-US");
            Assert.True(ci.Calendar == ci.DateTimeFormat.Calendar, "It is expected for newly created CultureInfo to have the Calendar and DateTimeFormat.Calendar pointing to same instance.");
            CultureInfo ci1 = (CultureInfo) ci.Clone();
            Assert.True(ci1.Calendar == ci1.DateTimeFormat.Calendar, "It is expected for cloned CultureInfo to have the Calendar and DateTimeFormat.Calendar pointing to same instance.");
            Assert.True(ci.Calendar != ci1.Calendar, "Cloning always clone the calendar, it is expected to the cloned object point to different calendar instance.");
            Assert.True(((CultureInfo)(ci.Clone())).Calendar != ((CultureInfo)(ci.Clone())).Calendar, "Cloning should always create a new calendar object.");
        }
    }
}
