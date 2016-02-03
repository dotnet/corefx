// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using Xunit;

namespace System.Globalization.CalendarsTests
{
    public class JapaneseCalendarMaxSupportedDateTime
    {
        #region Positive Test Cases
        // PosTest1: Call MaxSupportedDateTime to get max supported date time
        [Fact]
        public void PosTest1()
        {
            DateTime actual = new JapaneseCalendar().MaxSupportedDateTime;
            DateTime expected = DateTime.MaxValue;
            Assert.Equal(expected, actual);
        }

        #endregion
    }
}
