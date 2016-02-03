// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using Xunit;

namespace System.Globalization.Tests
{
    public class DateTimeFormatInfoGetFormat
    {
        // PosTest1: Call GetFormat to to get an valid DateTimeFormatInfo instance
        [Fact]
        public void PosTest1()
        {
            DateTimeFormatInfo expected = new DateTimeFormatInfo();
            object obj = expected.GetFormat(typeof(DateTimeFormatInfo));

            Assert.True(obj is DateTimeFormatInfo);

            DateTimeFormatInfo actual = obj as DateTimeFormatInfo;
            Assert.Equal(expected, actual);
        }

        // PosTest2: If the format type is not supported, null reference should be return
        [Fact]
        public void TestUnsupportedFormatType()
        {
            DateTimeFormatInfo info = new DateTimeFormatInfo();

            Assert.Null(info.GetFormat(typeof(Object)));
        }
    }
}
