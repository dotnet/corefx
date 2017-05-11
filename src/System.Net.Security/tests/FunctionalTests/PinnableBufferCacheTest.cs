// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Net.Security.Tests
{
    public class PinnableBufferCacheTest
    {
        [Fact]
        public void PinnableBufferCache_AllocateBuffer_Ok()
        {
            var cacheName = "Test";
            var numberOfElements = 5;
            var p = new PinnableBufferCache(cacheName, numberOfElements);

            var a = p.AllocateBuffer();

            Assert.Equal(numberOfElements, a.Length);
            foreach (byte t in a)
            {
                Assert.Equal(0, t);
            }
        }
    }
}
