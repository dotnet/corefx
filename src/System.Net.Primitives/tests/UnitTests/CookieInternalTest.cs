// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Net;

using Xunit;

namespace NetPrimitivesUnitTests
{
    public class CookieInternalTest
    {
        private const string SchemePrefix = "http://";
        private const string OriginalDomain = "contoso.com";
        private const string CookieName = "CookieName";
        private const string CookieValue = "CookieValue";

        [Fact]
        public void DomainImplicit_AddCookieWithImplicitDomain_DomainImplicitTrue()
        {
            var container = new CookieContainer();
            var cookie = new Cookie(CookieName, CookieValue);
            var uri = new Uri(SchemePrefix + OriginalDomain);
            container.Add(uri, cookie);

            var cookies = container.GetCookies(uri);
            Assert.Equal(1, cookies.Count);
            Assert.True(cookies[CookieName].DomainImplicit);
        }

        [Fact]
        public void DomainImplicit_AddCookieWithExplicitDomain_DomainImplicitFalse()
        {
            var container = new CookieContainer();
            var cookie1 = new Cookie(CookieName, CookieValue) { Domain = OriginalDomain };
            var uri = new Uri(SchemePrefix + OriginalDomain);
            container.Add(uri, cookie1);

            var cookies = container.GetCookies(uri);
            Assert.Equal(1, cookies.Count);
            Assert.False(cookies[CookieName].DomainImplicit);
        }

        [Fact]
        public void CookieParser_GetString()
        {
            string cookieString = "cookie_token=cookie_value";
            var parser = new CookieParser(cookieString);

            string actual = parser.GetString();
            Assert.Equal(cookieString, actual);
        }

        [Fact]
        public void CookieParser_GetString_TwoTokens()
        {
            string cookieString = "cookie_token1=cookie_value1;cookie_token2=cookie_value2";
            var parser = new CookieParser(cookieString);

            string actual = parser.GetString();
            Assert.Equal(cookieString, actual);
        }

        [Fact]
        public void CookieParser_GetString_WithTrailingSemicolon()
        {
            string cookieString = "cookie_token=cookie_value";
            string cookieStringWithSemicolon = $"{cookieString};";
            var parser = new CookieParser(cookieStringWithSemicolon);

            string actual = parser.GetString();
            Assert.Equal(cookieStringWithSemicolon, actual);
        }

        [Fact]
        public void CookieParser_GetString_TwoTokens_WithTrailingSemicolon()
        {
            string cookieString = "cookie_token1=cookie_value1;cookie_token2=cookie_value2;";
            var parser = new CookieParser(cookieString);

            string actual = parser.GetString();
            Assert.Equal(cookieString, actual);
        }

        [Fact]
        public void CookieParser_Get()
        {
            string name = "cookie_token";
            string value = "cookie_value";
            string cookieString = $"{name}={value}";
            var parser = new CookieParser(cookieString);
            Cookie cookie= parser.Get();

            Assert.NotNull(cookie);
            Assert.Equal(name, cookie.Name);
            Assert.Equal(value, cookie.Value);
        }

        [Fact]
        public void CookieParser_Get_WithTrailingSemicolon()
        {
            string name = "cookie_token";
            string value = "cookie_value";
            string cookieString = $"{name}={value};"; // There's a semicolon at the end of the string
            var parser = new CookieParser(cookieString);
            Cookie cookie = parser.Get();

            Assert.NotNull(cookie);
            Assert.Equal(name, cookie.Name);
            Assert.Equal(value, cookie.Value);
        }
    }
}
