// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using Xunit;

namespace System.Net.NetworkInformation.Tests
{
    public class DnsParsingTests : FileCleanupTestBase
    {
        [InlineData("resolv.conf")]
        [InlineData("resolv_nonewline.conf")]
        [Theory]
        public void DnsSuffixParsing(string file)
        {
            string normalizedFile = $"{GetTestFilePath()}_{file}_normalized0";
            FileUtil.NormalizeLineEndings(file, normalizedFile);

            string suffix = StringParsingHelpers.ParseDnsSuffixFromResolvConfFile(normalizedFile);
            Assert.Equal("fake.suffix.net", suffix);
        }

        [InlineData("resolv.conf")]
        [InlineData("resolv_nonewline.conf")]
        [Theory]
        public void DnsAddressesParsing(string file)
        {
            string normalizedFile = $"{GetTestFilePath()}_{file}_normalized1";
            FileUtil.NormalizeLineEndings(file, normalizedFile);

            var dnsAddresses = StringParsingHelpers.ParseDnsAddressesFromResolvConfFile(normalizedFile);
            Assert.Equal(1, dnsAddresses.Count);
            Assert.Equal(IPAddress.Parse("127.0.1.1"), dnsAddresses[0]);
        }
    }
}
