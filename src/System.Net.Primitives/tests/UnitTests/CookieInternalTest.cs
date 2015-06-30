// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NetPrimitivesUnitTests
{
    [TestClass]
    public class CookieInternalTest
    {
        private const string SchemePrefix = "http://";
        private const string OriginalDomain = "contoso.com";
        private const string CookieName = "CookieName";
        private const string CookieValue = "CookieValue";

        [TestMethod]
        public void DomainImplicit_AddCookieWithImplicitDomain_DomainImplicitIsTrue()
        {
            var container = new CookieContainer();
            var cookie = new Cookie(CookieName, CookieValue);
            var uri = new Uri(SchemePrefix + OriginalDomain);
            container.Add(uri, cookie);

            var cookies = container.GetCookies(uri);
            Assert.AreEqual(1, cookies.Count);
            Assert.IsTrue(cookies[CookieName].DomainImplicit);
        }

        [TestMethod]
        public void DomainImplicit_AddCookieWithExplicitDomain_DomainImplicitIsFalse()
        {
            var container = new CookieContainer();
            var cookie1 = new Cookie(CookieName, CookieValue) { Domain = OriginalDomain };
            var uri = new Uri(SchemePrefix + OriginalDomain);
            container.Add(uri, cookie1);

            var cookies = container.GetCookies(uri);
            Assert.AreEqual(1, cookies.Count);
            Assert.IsFalse(cookies[CookieName].DomainImplicit);
        }
    }
}