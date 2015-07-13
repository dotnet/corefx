// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using Xunit;

namespace System.Globalization.Tests
{
    public class RegionInfoTest
    {
        [Fact]
        [ActiveIssue(846, PlatformID.AnyUnix)]
        public void Test1()
        {
            RegionInfo riUS = new RegionInfo("en-US");
            RegionInfo riUK = new RegionInfo("en-GB");
            Assert.NotEqual(riUK, riUS);
        }

        [Fact]
        [ActiveIssue(846, PlatformID.AnyUnix)]
        public void Test2()
        {
            RegionInfo riUS = new RegionInfo("en-US");
            RegionInfo riUK = new RegionInfo("en-GB");
            Assert.NotEqual(riUS.GetHashCode(), riUK.GetHashCode());
        }

        [Fact]
        [ActiveIssue(846, PlatformID.AnyUnix)]
        public void Test3()
        {
            RegionInfo riUS = new RegionInfo("en-US");
            Assert.Equal(riUS.ToString(), "US");
        }

        [Fact]
        [ActiveIssue(846, PlatformID.AnyUnix)]
        public void Test4()
        {
            RegionInfo riUS = new RegionInfo("en-US");
            Assert.Equal("United States", riUS.DisplayName);
        }
    }
}