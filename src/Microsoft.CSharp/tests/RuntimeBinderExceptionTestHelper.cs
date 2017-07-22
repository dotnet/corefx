// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Xunit;

namespace Microsoft.CSharp.RuntimeBinder.Tests
{
    public static class RuntimeBinderExceptionTestHelper
    {
        public static void VerifyHelpLink(this RuntimeBinderException ex, int csError)
        {
            Assert.NotNull(ex);
            if (csError == 0)
            {
                Assert.Null(ex.HelpLink);
                return;
            }

            Assert.NotNull(ex.HelpLink);
            Uri uri = new Uri(ex.HelpLink, UriKind.Absolute);
            Assert.Equal("https", uri.Scheme);
            Assert.Equal("bingdev.cloudapp.net", uri.Host);
            Assert.Equal("/BingUrl.svc/Get", uri.AbsolutePath);
            Dictionary<string, string> queryParams = uri.Query.TrimStart('?').Split('&')
                .Select(par => par.Split(new[] { '=' }, 2))
                .ToDictionary(par => par[0], par => Uri.UnescapeDataString(par[1]));
            Assert.Equal(6, queryParams.Count);
            Assert.Equal("C#", queryParams["mainLanguage"]);
            Assert.Equal("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}", queryParams["projectType"]); // C# project
            Assert.True(Guid.TryParseExact(queryParams["requestId"], "D", out Guid _));

            // Even if we could obtain a value for selectedText, some people set the a registry value to block
            // roslyn setting it on static compilation for security reasons, so it is best left empty.
            Assert.Empty(queryParams["selectedText"]);
            Assert.Empty(queryParams["clientId"]);
            string errorCode = queryParams["errorCode"];
            Assert.True(new Regex(@"^CS\d{4}$").IsMatch(errorCode));
            Assert.Equal(csError, int.Parse(errorCode.Substring(2)));
        }

    }
}
