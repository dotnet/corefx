﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using System.Runtime.InteropServices;
using Xunit;
using XunitPlatformID = Xunit.PlatformID;

namespace System.IO.Tests
{
    public class Directory_GetLogicalDrives
    {
        [Fact]
        [PlatformSpecific(XunitPlatformID.AnyUnix)]
        public void GetsValidDriveStrings_Unix()
        {
            string[] drives = Directory.GetLogicalDrives();
            Assert.NotEmpty(drives);
            Assert.All(drives, d => Assert.NotNull(d));
            Assert.Contains(drives, d => d == "/");
        }

        [Fact]
        [PlatformSpecific(XunitPlatformID.Windows)]
        public void GetsValidDriveStrings_Windows()
        {
            string[] drives = Directory.GetLogicalDrives();
            Assert.NotEmpty(drives);
            Assert.All(drives, d => Assert.Matches(@"^[A-Z]:\\$", d));
        }
    }
}
