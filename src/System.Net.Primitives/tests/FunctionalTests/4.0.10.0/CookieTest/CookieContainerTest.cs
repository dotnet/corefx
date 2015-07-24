using System;
using System.Net;
#if VS_UNIT_TESTS
using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
using CoreFXTestLibrary;
#endif
using NCLTest.Common;

namespace NetPrimitivesUnitTests
{
    [TestClass]
    public class CookieContainerTest
    {
        private const string CookieName1 = "CookieName1";
        private const string CookieName2 = "CookieName2";
        private const string CookieValue1 = "CookieValue1";
        private const string CookieValue2 = "CookieValue2";

        [TestMethod]
        [ExpectedException(typeof(System.Net.CookieException))]
        public void Add_CookieVersion1AndRootDomainWithNoLeadingDot_ThrowsCookieException()
        {
            const string SchemePrefix = "http://";
            const string OriginalDomain = "contoso.com";

            var container = new CookieContainer();
            var cookie = new Cookie(CookieName1, CookieValue1) { Version = 1, Domain = OriginalDomain };
            var uri = new Uri(SchemePrefix + OriginalDomain);
            container.Add(uri, cookie);
        }

        [TestMethod]
        public void Add_CookieWithExplicitDomainUsingSingleLabelUri_CookieAddedToContainer()
        {
            var host = TestRequirements.GetHostName();
            var domain = TestRequirements.GetDomainName();
            Logger.LogInformation("Test Machine: Host={0}, Domain={1}", host, domain);
            
            // This test only works on domain joined machines, so we'll skip the test
            // if the test machine is not domain joined.
            if (TestRequirements.IsHostDomainJoined())
            {
                var container = new CookieContainer();
                var cookie = new Cookie(CookieName1, CookieValue1, "/", "." + domain);
                var uri = new Uri(string.Format("http://{0}", host));
                
                container.Add(uri, cookie);
                
                Assert.AreEqual(1, container.Count);
            }
            else
            {
                Logger.LogInformation("Skipping test since machine is not domain joined.");
            }
        }
        
        [TestMethod]
        public void GetCookies_AddCookiesWithImplicitDomain_CookiesReturnedOnlyForExactDomainMatch()
        {
            const string SchemePrefix = "http://";
            const string OriginalDomain = "contoso.com";

            var container = new CookieContainer();
            var cookie1 = new Cookie(CookieName1, CookieValue1);
            var cookie2 = new Cookie(CookieName2, CookieValue2) { Version = 1 };
            var uri = new Uri(SchemePrefix + OriginalDomain);
            container.Add(uri, cookie1);
            container.Add(uri, cookie2);

            var cookies = container.GetCookies(uri);
            Assert.AreEqual(2, cookies.Count);
            Assert.AreEqual(OriginalDomain, cookies[CookieName1].Domain);
            Assert.AreEqual(OriginalDomain, cookies[CookieName2].Domain);

            uri = new Uri(SchemePrefix + "www." + OriginalDomain);
            cookies = container.GetCookies(uri);
            Assert.AreEqual(0, cookies.Count);

            uri = new Uri(SchemePrefix + "x.www." + OriginalDomain);
            cookies = container.GetCookies(uri);
            Assert.AreEqual(0, cookies.Count);

            uri = new Uri(SchemePrefix + "y.x.www." + OriginalDomain);
            cookies = container.GetCookies(uri);
            Assert.AreEqual(0, cookies.Count);

            uri = new Uri(SchemePrefix + "z.y.x.www." + OriginalDomain);
            cookies = container.GetCookies(uri);
            Assert.AreEqual(0, cookies.Count);
        }

        [TestMethod]
        public void GetCookies_AddCookieVersion0WithExplicitDomain_CookieReturnedForDomainAndSubdomains()
        {
            const string SchemePrefix = "http://";
            const string OriginalDomain = "contoso.com";

            var container = new CookieContainer();
            var cookie1 = new Cookie(CookieName1, CookieValue1) { Domain = OriginalDomain };
            container.Add(new Uri(SchemePrefix + OriginalDomain), cookie1);

            var uri = new Uri(SchemePrefix + OriginalDomain);
            var cookies = container.GetCookies(uri);
            Assert.AreEqual(1, cookies.Count);
            Assert.AreEqual(OriginalDomain, cookies[CookieName1].Domain);

            uri = new Uri(SchemePrefix + "www." + OriginalDomain);
            cookies = container.GetCookies(uri);
            Assert.AreEqual(1, cookies.Count);

            uri = new Uri(SchemePrefix + "x.www." + OriginalDomain);
            cookies = container.GetCookies(uri);
            Assert.AreEqual(1, cookies.Count);

            uri = new Uri(SchemePrefix + "y.x.www." + OriginalDomain);
            cookies = container.GetCookies(uri);
            Assert.AreEqual(1, cookies.Count);

            uri = new Uri(SchemePrefix + "z.y.x.www." + OriginalDomain);
            cookies = container.GetCookies(uri);
            Assert.AreEqual(1, cookies.Count);
        }

        [TestMethod]
        public void GetCookies_AddCookieVersion1WithExplicitDomain_CookieReturnedForDomainAndOneLevelSubDomain()
        {
            const string SchemePrefix = "http://";
            const string OriginalDomain = "contoso.com";
            const string OriginalDomainWithLeadingDot = "." + OriginalDomain;

            var container = new CookieContainer();
            var cookie1 = new Cookie(CookieName1, CookieValue1) { Domain = OriginalDomainWithLeadingDot, Version = 1 };
            container.Add(new Uri(SchemePrefix + OriginalDomain), cookie1);

            var uri = new Uri(SchemePrefix + OriginalDomain);
            var cookies = container.GetCookies(uri);
            Assert.AreEqual(1, cookies.Count);
            Assert.AreEqual(OriginalDomainWithLeadingDot, cookies[CookieName1].Domain);

            uri = new Uri(SchemePrefix + "www." + OriginalDomain);
            cookies = container.GetCookies(uri);
            Assert.AreEqual(1, cookies.Count);

            uri = new Uri(SchemePrefix + "x.www." + OriginalDomain);
            cookies = container.GetCookies(uri);
            Assert.AreEqual(0, cookies.Count);

            uri = new Uri(SchemePrefix + "y.x.www." + OriginalDomain);
            cookies = container.GetCookies(uri);
            Assert.AreEqual(0, cookies.Count);

            uri = new Uri(SchemePrefix + "z.y.x.www." + OriginalDomain);
            cookies = container.GetCookies(uri);
            Assert.AreEqual(0, cookies.Count);
        }
    }
}
