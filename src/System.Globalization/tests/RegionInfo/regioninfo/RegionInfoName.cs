// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using Xunit;

namespace System.Globalization.Tests
{
    public class RegionInfoName
    {
        // PosTest1:Return the property Name in RegionInfo object 1
        [Fact]
        [ActiveIssue(846, PlatformID.AnyUnix)]
        public void PosTest1()
        {
            RegionInfo regionInfo = new RegionInfo("en-US");
            string strName = regionInfo.Name;
            Assert.Equal(strName, "US");
        }

        // PosTest2:Return the property Name in RegionInfo object 2
        [Fact]
        [ActiveIssue(846, PlatformID.AnyUnix)]
        public void PosTest2()
        {
            RegionInfo regionInfo = new RegionInfo("zh-CN");
            string strName = regionInfo.Name;
            Assert.Equal(strName, "CN");
        }

        // PosTest3:Return the property Name in RegionInfo object 3
        [Fact]
        [ActiveIssue(846, PlatformID.AnyUnix)]
        public void PosTest3()
        {
            RegionInfo regionInfo = new RegionInfo("US");
            string strName = regionInfo.Name;
            Assert.Equal("US", strName);
        }

        // PosTest4:Return the property Name in RegionInfo object 4
        [Fact]
        [ActiveIssue(846, PlatformID.AnyUnix)]
        public void PosTest4()
        {
            RegionInfo regionInfo = new RegionInfo("CN");
            string strName = regionInfo.Name;
            Assert.Equal("CN", strName);
        }
    }
}