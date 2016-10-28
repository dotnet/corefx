// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Net.NameResolution.Tests
{
    public class GetHostNameTest
    {
        [Fact]
        public void Dns_GetHostName_Ok()
        {
            Assert.False(string.IsNullOrWhiteSpace(Dns.GetHostName()));
        }
    }
}
