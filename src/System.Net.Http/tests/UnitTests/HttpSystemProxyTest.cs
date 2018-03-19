// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Net.Http;
using Xunit;
using Xunit.Abstractions;
using System.Diagnostics;
using System.Net.Http.WinHttpHandlerUnitTests;

namespace System.Net.Http.Tests
{
    public class HttpSystemProxyTest : RemoteExecutorTestBase
    {
        private readonly ITestOutputHelper _output;
        private const string FakeProxyString = "http://proxy.contoso.com";
        private readonly Uri fakeProxyUri = new Uri("http://proxy.contoso.com");
        private readonly Uri fooHttp = new Uri("http://foo.com");
        private readonly Uri fooHttps = new Uri("https://foo.com");

        public HttpSystemProxyTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void HttpProxy_SystemProxy_Loaded()
        {
            IWebProxy p;

            FakeRegistry.Reset();
            Assert.False(HttpSystemProxy.TryCreate(out p));

            FakeRegistry.WinInetProxySettings.Proxy = FakeProxyString;
            WinInetProxyHelper proxyHelper = new WinInetProxyHelper();

            Assert.True(HttpSystemProxy.TryCreate(out p));
            Assert.NotNull(p);
            Assert.Equal(fakeProxyUri, p.GetProxy(fooHttp));
            Assert.Equal(fakeProxyUri, p.GetProxy(fooHttps));
        }

        [Theory]
        [InlineData("http://localhost/", true)]
        [InlineData("http://127.0.0.1/", true)]
        [InlineData("http://128.0.0.1/", false)]
        [InlineData("http://[::1]/", true)]
        [InlineData("http://foo/", true)]
        [InlineData("http://www.foo.com/", true)]
        [InlineData("http://WWW.FOO.COM/", true)]
        [InlineData("http://foo.com/", false)]
        [InlineData("http://bar.com/", true)]
        [InlineData("http://BAR.COM/", true)]
        [InlineData("http://162.1.1.1/", true)]
        [InlineData("http://[2a01:5b40:0:248::52]/", false)]
        [InlineData("http://[2002::11]/", true)]
        [InlineData("http://[2607:f8b0:4005:80a::200e]/", true)]
        [InlineData("http://[2607:f8B0:4005:80A::200E]/", true)]
        public void HttpProxy_Local_Bypassed(string name, bool shouldBypass)
        {

            RemoteInvoke((url, expected) =>
            {
                bool expectedResult = Boolean.Parse(expected);
                IWebProxy p;

                FakeRegistry.Reset();
                FakeRegistry.WinInetProxySettings.Proxy = FakeProxyString;
                FakeRegistry.WinInetProxySettings.ProxyBypass = "23.23.86.44;*.foo.com;<local>;BAR.COM; ; 162*;[2002::11];[*:f8b0:4005:80a::200e]";
                WinInetProxyHelper proxyHelper = new WinInetProxyHelper();

                Assert.True(HttpSystemProxy.TryCreate(out p));
                Assert.NotNull(p);

                Uri u = new Uri(url);
                Assert.Equal(expectedResult, p.GetProxy(u) == null);

                return SuccessExitCode;
           }, name, shouldBypass.ToString()).Dispose();
        }
    }
}
