// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using Xunit;

namespace System.Globalization.Tests
{
    public class DateTimeFormatInfoIsReadOnly
    {
        // PosTest1: IsReadOnly should return false for DateTimeFormatInfo created from non invariant culture
        [Fact]
        public void TestNonInvariant()
        {
            DateTimeFormatInfo info = new CultureInfo("en-us").DateTimeFormat;
            Assert.False(info.IsReadOnly);
        }

        // PosTest2: IsReadOnly should return true for DateTimeFormatInfo created from invariant culture
        [Fact]
        public void TestInvariant()
        {
            DateTimeFormatInfo info = DateTimeFormatInfo.InvariantInfo;
            Assert.True(info.IsReadOnly);
        }

        // PosTest3: IsReadOnly should return true for DateTimeFormatInfo created by ReadOnly method
        [Fact]
        public void TestReadOnlyMethod()
        {
            DateTimeFormatInfo info = DateTimeFormatInfo.ReadOnly(new CultureInfo("en-us").DateTimeFormat);
            Assert.True(info.IsReadOnly);
        }
    }
}
