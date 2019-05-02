// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;
using Xunit;

namespace System.Diagnostics.Tests
{
    public partial class ProcessThreadTests
    {
        [PlatformSpecific(TestPlatforms.AnyUnix)]
        [Fact]
        public void TestPriorityLevelProperty_Unix()
        {
            CreateDefaultProcess();

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Assert.Throws<PlatformNotSupportedException>(() => _process.Threads[0].PriorityLevel);
            }
            Assert.Throws<PlatformNotSupportedException>(() => _process.Threads[0].PriorityLevel = ThreadPriorityLevel.AboveNormal);
        }
    }
}
