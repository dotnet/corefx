// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Net.Http;
using Xunit;
using Xunit.Abstractions;
using System.Diagnostics;

namespace System.Net.Http.Tests
{
    public class HttpEnvironmentProxyTest : RemoteExecutorTestBase
    {
        private readonly ITestOutputHelper _output;
        private static readonly Uri fooHttp = new Uri("http://foo.com");
        private static readonly Uri fooHttps = new Uri("https://foo.com");

        // This will clean specific environmental variables
        // to be sure they do not interfere with the test.
        private void cleanEnv()
        {
            List<string>  vars = new List<string>() { "http_proxy", "HTTPS_PROY", "https_proxy",
                                                      "all_proxy", "ALL_PROXY", 
                                                      "NO_PROXY" };
            foreach (string v in vars)
            {
                Environment.SetEnvironmentVariable(v, null);
            }
        }

        public HttpEnvironmentProxyTest(ITestOutputHelper output)
        {
            _output = output;
            cleanEnv();
        }

        [Fact]
        public void getBasicProxy()
        {
            RemoteInvoke(() =>
            {

                IWebProxy p;
                Uri u;

                // It should not return object if there are no variables set.
                Assert.True(HttpEnvironmentProxy.TryToCreate() == null);

                Environment.SetEnvironmentVariable("all_proxy", "http://1.1.1.1:3000");
                p = HttpEnvironmentProxy.TryToCreate();
                Assert.True(p != null);
                Assert.True(p.Credentials == null);

                u = p.GetProxy(fooHttp);
                Assert.True(u != null && u.Host == "1.1.1.1");
                u = p.GetProxy(fooHttps);
                Assert.True(u != null && u.Host == "1.1.1.1");

                Environment.SetEnvironmentVariable("http_proxy", "http://1.1.1.2:3001");
                p = HttpEnvironmentProxy.TryToCreate();
                Assert.True(p != null);

                // Protocol specific variables should take precedence over all_
                // and https should still use all_proxy.
                u = p.GetProxy(fooHttp);
                Assert.True(u != null && u.Host == "1.1.1.2" && u.Port == 3001);
                u = p.GetProxy(fooHttps);
                Assert.True(u != null && u.Host == "1.1.1.1" && u.Port == 3000);

                // Set https to invalid strings and use only IP & port for http.
                Environment.SetEnvironmentVariable("http_proxy", "1.1.1.3:3003");
                Environment.SetEnvironmentVariable("https_proxy", "ab!cd");
                p = HttpEnvironmentProxy.TryToCreate();
                Assert.True(p != null);

                u = p.GetProxy(fooHttp);
                Assert.True(u != null && u.Host == "1.1.1.3" && u.Port == 3003);
                u = p.GetProxy(fooHttps);
                Assert.True(u != null && u.Host == "1.1.1.1" && u.Port == 3000);

                // Try valid URI with unsupported protocol. It will be ignored
                // to mimic curl behavior.
                Environment.SetEnvironmentVariable("https_proxy", "socks5://1.1.1.4:3004");
                p = HttpEnvironmentProxy.TryToCreate();
                Assert.True(p != null);
                u = p.GetProxy(fooHttps);
                Assert.True(u != null && u.Host == "1.1.1.1" && u.Port == 3000);

                // Set https to valid URI but different from http.
                Environment.SetEnvironmentVariable("https_proxy", "http://1.1.1.5:3005");
                p = HttpEnvironmentProxy.TryToCreate();
                Assert.True(p != null);

                u = p.GetProxy(fooHttp);
                Assert.True(u != null && u.Host == "1.1.1.3" && u.Port == 3003);
                u = p.GetProxy(fooHttps);
                Assert.True(u != null && u.Host == "1.1.1.5" && u.Port == 3005);

                return SuccessExitCode;
            }).Dispose();
        }

        [Fact]
        public void parseCredientials()
        {

            RemoteInvoke(() =>
            {
                IWebProxy p;

                Environment.SetEnvironmentVariable("all_proxy", "http://foo:bar@1.1.1.1:3000");
                p = HttpEnvironmentProxy.TryToCreate();
                Assert.True(p != null);
                Assert.True(p.Credentials != null);

                // Use user only without password.
                Environment.SetEnvironmentVariable("all_proxy", "http://foo@1.1.1.1:3000");
                p = HttpEnvironmentProxy.TryToCreate();
                Assert.True(p != null);
                Assert.True(p.Credentials != null);

                // Use different user for http and https
                Environment.SetEnvironmentVariable("https_proxy", "http://foo1:bar1@1.1.1.1:3000");
                p = HttpEnvironmentProxy.TryToCreate();
                Assert.True(p != null);
                Assert.True(p.Credentials != null);

                Assert.True(p.Credentials.GetCredential(fooHttp, "Basic") != p.Credentials.GetCredential(fooHttps, "Basic"));

                return SuccessExitCode;
            }).Dispose();
        }

        [Fact]
        public void exceptionList()
        {
            RemoteInvoke(() =>
            {
                IWebProxy p;

                Environment.SetEnvironmentVariable("no_proxy", ".test.com,, foo.com");
                Environment.SetEnvironmentVariable("all_proxy", "http://foo:bar@1.1.1.1:3000");
                p = HttpEnvironmentProxy.TryToCreate();
                Assert.True(p != null);

                Assert.True(p.IsBypassed(fooHttp));
                Assert.True(p.IsBypassed(fooHttps));
                Assert.True(p.IsBypassed(new Uri("http://test.com")));
                Assert.False(p.IsBypassed(new Uri("http://1test.com")));
                Assert.True(p.IsBypassed(new Uri("http://www.test.com")));

                return SuccessExitCode;
           }).Dispose();
        }
    }
}
