// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using Xunit;

namespace System.Globalization.Tests
{
    public class CultureInfoCompareInfo
    {
        [Fact]
        public void TestEsES()
        {
            CultureInfo myCIintl = new CultureInfo("es-ES");
            string compareString1 = "llegar";
            string compareString2 = "lugar";
            Assert.True(myCIintl.CompareInfo.Compare(compareString1, compareString2) < 0);
        }

        [Fact]
        [ActiveIssue(5463, PlatformID.AnyUnix)]
        public void TestEsESTraditional()
        {
            CultureInfo myCItrad = new CultureInfo("es-ES_tradnl");
            string compareString1 = "llegar";
            string compareString2 = "lugar";
            Assert.True(myCItrad.CompareInfo.Compare(compareString1, compareString2) > 0);
        }

        [Fact]
        public void TestCompareInfoName()
        {
            string expectedName = "en-US";
            CultureInfo myCultureInfo = new CultureInfo(expectedName);
            CompareInfo myCompareInfo = myCultureInfo.CompareInfo;
            Assert.True(myCompareInfo.Name.Equals(expectedName, StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public void TestCompareInfoNameWithInvariant()
        {
            string expectedName = "";
            CultureInfo myCultureInfo = new CultureInfo(expectedName);
            CompareInfo myCompareInfo = myCultureInfo.CompareInfo;
            Assert.True(myCompareInfo.Name.Equals(expectedName, StringComparison.OrdinalIgnoreCase));
        }
    }
}
