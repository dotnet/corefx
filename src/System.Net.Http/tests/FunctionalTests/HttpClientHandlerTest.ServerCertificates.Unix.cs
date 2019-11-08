// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Security;
using System.Net.Test.Common;
using System.Runtime.InteropServices;
using System.Security.Authentication.ExtendedProtection;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.DotNet.RemoteExecutor;
using Xunit;

namespace System.Net.Http.Functional.Tests
{
    public abstract partial class HttpClientHandler_ServerCertificates_Test
    {
        internal bool BackendSupportsCustomCertificateHandling
        {
            get
            {
                if (UseSocketsHttpHandler)
                {
                    return true;
                }

                return TestHelper.NativeHandlerSupportsSslConfiguration();
            }
        }

        [Fact]
        [PlatformSpecific(~TestPlatforms.OSX)] // Not implemented
        public void HttpClientUsesSslCertEnvironmentVariables()
        {
            // We set SSL_CERT_DIR and SSL_CERT_FILE to empty locations.
            // The HttpClient should fail to validate the server certificate.

            var psi = new ProcessStartInfo();
            string sslCertDir = GetTestFilePath();
            Directory.CreateDirectory(sslCertDir);
            psi.Environment.Add("SSL_CERT_DIR", sslCertDir);

            string sslCertFile = GetTestFilePath();
            File.WriteAllText(sslCertFile, "");
            psi.Environment.Add("SSL_CERT_FILE", sslCertFile);

            RemoteExecutor.Invoke(async (useSocketsHttpHandlerString, useHttp2String) =>
            {
                const string Url = "https://www.microsoft.com";

                using (HttpClient client = CreateHttpClient(useSocketsHttpHandlerString, useHttp2String))
                {
                    await Assert.ThrowsAsync<HttpRequestException>(() => client.GetAsync(Url));
                }
            }, UseSocketsHttpHandler.ToString(), UseHttp2.ToString(), new RemoteInvokeOptions { StartInfo = psi }).Dispose();
        }
    }
}
