// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Xunit;

namespace System.Tests
{
    public class EnvironmentTickCount
    {
        [Fact]
        public void TickCountTest()
        {
            int start = Environment.TickCount;
            HashSet<int> times = new HashSet<int>();
            Assert.True(
                SpinWait.SpinUntil(() =>
                {
                    int time = Environment.TickCount;
                    times.Add(time);
                    return time - start > 0;
                },
                TimeSpan.FromSeconds(1)),
                $"TickCount did not increase after one second. start: {start}, values tested: {string.Join(", ", times.ToArray())}.");
        }
    }
}
