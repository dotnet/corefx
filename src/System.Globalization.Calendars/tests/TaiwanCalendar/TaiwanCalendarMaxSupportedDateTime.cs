// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using Xunit;

namespace System.Globalization.CalendarsTests
{
    // System.Globalization.TaiwanCalendar.MaxSupportedDateTime
    public class TaiwanCalendarMaxSupportedDateTime
    {
        #region Positive Tests
        // PosTest1: Verify the MaxSupportedDateTime is equivalent to 
        // the last moment of December 31, 9999 C.E. in the Gregorian calendar
        [Fact]
        public void PosTest1()
        {
            System.Globalization.Calendar tc = new TaiwanCalendar();
            System.Globalization.Calendar gc = new GregorianCalendar();
            DateTime desiretdDT = gc.ToDateTime(9999, 12, 31, 23, 59, 59, 999);
            desiretdDT = desiretdDT.AddTicks(9999);
            Assert.Equal(desiretdDT, tc.MaxSupportedDateTime);
        }
        #endregion
    }
}
