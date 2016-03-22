// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;

using Xunit;

namespace System.Net.Primitives.Functional.Tests
{
    public partial class CookieContainerTest
    {
        private const string u1 = ".url1.com"; // Basic domain
        private const string u2 = "127.0.0.1"; // Local domain
        private const string u3 = "url3.com"; // Basic domain without a leading dot
        private static readonly Uri u4 = new Uri("http://url4.com"); // Basic uri
        private static readonly Uri u5 = new Uri("http://url5.com/path"); // Set explicit path
        private static readonly Uri u6 = new Uri("http://url6.com"); // No cookies should be added with this uri

        private static readonly Cookie c1 = new Cookie("name1", "value", "", u1);
        private static readonly Cookie c2 = new Cookie("name2", "value", "", u1) { Secure = true };
        private static readonly Cookie c3 = new Cookie("name3", "value", "", u1) { Port = "\"80, 90, 100, 443\"" };
        private static readonly Cookie c4 = new Cookie("name4", "value", "", u2);
        private static readonly Cookie c5 = new Cookie("name5", "value", "/path", u2);
        private static readonly Cookie c6 = new Cookie("name6", "value", "", "." + u3);
        private static readonly Cookie c7 = new Cookie("name7", "value", "", "." + u3);
        private static readonly Cookie c8 = new Cookie("name8", "value");
        private static readonly Cookie c9 = new Cookie("name9", "value");
        private static readonly Cookie c10 = new Cookie("name10", "value");
        private static readonly Cookie c11 = new Cookie("name11", "value") { Port = "\"80, 90, 100\"" };

        private static CookieContainer CreateCount11Container()
        {
            CookieContainer cc1 = new CookieContainer();
            // Add(Cookie)
            cc1.Add(c1);
            cc1.Add(c2);
            cc1.Add(c3);
            cc1.Add(c4);

            // Add(CookieCollection)
            CookieCollection cc2 = new CookieCollection();
            cc2.Add(c5);
            cc2.Add(c6);
            cc2.Add(c7);
            cc1.Add(cc2);

            // Add(Uri, Cookie)
            cc1.Add(u4, c8);
            cc1.Add(u4, c9);

            // Add(Uri, CookieCollection)
            cc2 = new CookieCollection();
            cc2.Add(c10);
            cc2.Add(c11);
            cc1.Add(u5, cc2);

            return cc1;
        }

        [Fact]
        public static void Ctor_Empty_Success()
        {
            CookieContainer cc = new CookieContainer();
            Assert.Equal(0, cc.Count);
        }

        [Fact]
        public static void Ctor_Capacity_Success()
        {
            CookieContainer cc = new CookieContainer(5);
            Assert.Equal(5, cc.Capacity);
        }

        [Fact]
        public static void Ctor_Capacity_Invalid()
        {
            Assert.Throws<ArgumentException>(() => new CookieContainer(0)); // Capacity <= 0
        }

        [Fact]
        public static void Ctor_CapacityPerDomainCapacityMaxCookieSize_Success()
        {
            CookieContainer cc = new CookieContainer(5, 4, 3);
            Assert.Equal(5, cc.Capacity);
            Assert.Equal(4, cc.PerDomainCapacity);
            Assert.Equal(3, cc.MaxCookieSize);

            cc = new CookieContainer(10, int.MaxValue, 4); // Even though PerDomainCapacity > Capacity, this shouldn't throw
        }

        [Fact]
        public static void Ctor_CapacityPerDomainCapacityMaxCookieSize_Invalid()
        {
            Assert.Throws<ArgumentException>(() => new CookieContainer(0, 10, 5)); // Capacity <= 0
            Assert.Throws<ArgumentOutOfRangeException>(() => new CookieContainer(5, 0, 5)); // Per domain capacity <= 0
            Assert.Throws<ArgumentOutOfRangeException>(() => new CookieContainer(5, 10, 5)); // Per domain capacity > Capacity

            Assert.Throws<ArgumentException>(() => new CookieContainer(15, 10, 0)); // Max cookie size <= 0
        }

        [Fact]
        public static void Capacity_GetSet_Success()
        {
            CookieContainer cc = new CookieContainer();
            Assert.Equal(CookieContainer.DefaultCookieLimit, cc.Capacity);

            cc.Capacity = 900;
            Assert.Equal(900, cc.Capacity);

            cc.Capacity = 40; // Shrink
            Assert.Equal(40, cc.Capacity);
        }

        [Fact]
        public static void Capacity_ShrinkNoneExpired_RemovesAll()
        {
            Cookie c1 = new Cookie("name1", "value", "", ".url1.com");
            Cookie c2 = new Cookie("name2", "value", "", ".url2.com");
            Cookie c3 = new Cookie("name3", "value", "", ".url3.com");
            Cookie c4 = new Cookie("name4", "value", "", ".url4.com");

            CookieContainer cc = new CookieContainer() { PerDomainCapacity = 1 };
            cc.Add(c1);
            cc.Add(c2);
            cc.Add(c3);
            cc.Add(c4);

            // Shrink to a number less than the current count - since we have no cookies that have expired,
            // we should just clear the whole thing except for a single cookie
            cc.Capacity = 2;
            Assert.Equal(2, cc.Capacity);
            Assert.Equal(0, cc.Count);
        }

        [Fact]
        public static void Capacity_Set_Invalid()
        {
            CookieContainer cc = new CookieContainer();

            Assert.Throws<ArgumentOutOfRangeException>(() => cc.Capacity = 0); // <= 0
            Assert.Throws<ArgumentOutOfRangeException>(() => cc.Capacity = cc.PerDomainCapacity - 1); // < per domain capacity
        }

        [Fact]
        public static void MaxCookieSize_GetSet_Success()
        {
            CookieContainer cc = new CookieContainer();
            Assert.Equal(CookieContainer.DefaultCookieLengthLimit, cc.MaxCookieSize);

            cc.MaxCookieSize = 8192;
            Assert.Equal(8192, cc.MaxCookieSize);
        }

        [Fact]
        public static void MaxCookieSize_Set_Invalid()
        {
            CookieContainer cc = new CookieContainer();
            Assert.Throws<ArgumentOutOfRangeException>(() => cc.MaxCookieSize = 0); // Max cookie size <= 0
        }

        [Fact]
        public static void PerDomainCapacity_GetSet()
        {
            CookieContainer cc = new CookieContainer();
            Assert.Equal(CookieContainer.DefaultPerDomainCookieLimit, cc.PerDomainCapacity);

            cc.PerDomainCapacity = 50;
            Assert.Equal(50, cc.PerDomainCapacity);

            cc.PerDomainCapacity = 40; // Shrink
            Assert.Equal(40, cc.PerDomainCapacity);

            // Shrink to one - this should get rid of all cookies since there are no possible cookies that can be expired
            cc.PerDomainCapacity = 1;
            Assert.Equal(1, cc.PerDomainCapacity);
            Assert.Equal(0, cc.Count);
        }

        [Fact]
        public static void PerDomainCapacity_Set_Invalid()
        {
            CookieContainer cc = new CookieContainer();

            Assert.Throws<ArgumentOutOfRangeException>(() => cc.PerDomainCapacity = 0); // Per domain capacity <= 0
            Assert.Throws<ArgumentOutOfRangeException>(() => cc.PerDomainCapacity = cc.Capacity + 1); // Per domain capacity >= Capacity
        }
        [Fact]
        public static void Add_Cookies_Success()
        {
            CookieContainer cc = CreateCount11Container();
            Assert.Equal(11, cc.Count);
        }

        [Fact]
        public static void Add_ExpiredCookie_NotAdded()
        {
            CookieContainer cc = new CookieContainer();
            Cookie c1 = new Cookie("name1", "value", "", ".domain.com") { Expired = true };
            Cookie c2 = new Cookie("name2", "value", "", ".domain.com");

            cc.Add(c1);
            cc.Add(c2);
            // Ignores adding expired cookies
            Assert.Equal(1, cc.Count);
            Assert.Equal(c2, cc.GetCookies(new Uri("http://domain.com"))[0]);

            // Manually expire cookie
            c2.Expired = true;
            cc.Add(c2);
            Assert.Equal(0, cc.Count);
        }

        [Fact]
        public static void Add_ReachedMaxDomainCapacity_NotAdded()
        {
            CookieContainer cc = new CookieContainer() { PerDomainCapacity = 1 };
            cc.Add(c1); // Add a cookie to fill up the capacity
            Assert.Equal(1, cc.Count);
            cc.Add(c2); // Should not be added
            Assert.Equal(1, cc.Count);
        }

        [Fact]
        public static void Add_ReachedMaxCount_NotAdded()
        {
            CookieContainer cc = new CookieContainer(4);
            for (int i = 1; i <= 4; i++)
            {
                cc.Add(new Cookie("name" + i.ToString(), "value", "", ".domain.com"));
            }

            Assert.Equal(4, cc.Count);
            cc.Add(new Cookie("name5", "value", "", ".domain.com"));
            Assert.Equal(4, cc.Count);
        }

        [Fact]
        public static async void Add_ReachedMaxCountWithExpiredCookies_Added()
        {
            Cookie c1 = new Cookie("name1", "value", "", ".domain1.com");
            Cookie c2 = new Cookie("name2", "value", "", ".domain2.com");
            Cookie c3 = new Cookie("name3", "value", "", ".domain3.com");
            c1.Expires = DateTime.Now.AddSeconds(1); // The cookie will expire in 1 second

            CookieContainer cc = new CookieContainer(2);
            cc.Add(c1);
            cc.Add(c2);

            await Task.Delay(2000); // Sleep for 2 seconds to wait for the cookie to expire
            cc.Add(c3);
            Assert.Equal(0, cc.GetCookies(new Uri("http://domain1.com")).Count);
            Assert.Equal(c3, cc.GetCookies(new Uri("http://domain3.com"))[0]);
        }

        [Fact]
        public static void Add_SameCookieDifferentVairants_OverridesOlderVariant()
        {
            Uri uri = new Uri("http://domain.com");

            Cookie c1 = new Cookie("name1", "value", "", ".domain.com"); // Variant = Plain
            Cookie c2 = new Cookie("name1", "value", "", ".domain.com") { Port = "\"80\"" }; // Variant = RFC2965 (should override)
            Cookie c3 = new Cookie("name1", "value", "", ".domain.com") { Port = "\"80, 90\"" }; // Variant = RFC2965 (should override)
            Cookie c4 = new Cookie("name1", "value", "", ".domain.com") { Version = 1 }; // Variant = RFC2109 (should be rejected)

            CookieContainer cc = new CookieContainer();
            cc.Add(c1);

            // Adding a newer variant should override an older one
            cc.Add(c2);
            Assert.Equal(c2.Port, cc.GetCookies(uri)[0].Port);

            // Adding the same variant should override the existing one
            cc.Add(c3);
            Assert.Equal(c3.Port, cc.GetCookies(uri)[0].Port);

            // Adding an older variant shold be rejected
            cc.Add(c4);
            Assert.Equal(c3.Port, cc.GetCookies(uri)[0].Port);

            // Ensure that although we added 3 cookies, only 1 was actually added (the others were overriden or rejected)
            Assert.Equal(1, cc.Count);
        }

        [Fact]
        public static void Add_Cookie_Invalid()
        {
            CookieContainer cc = new CookieContainer();
            Assert.Throws<ArgumentNullException>(() => cc.Add((Cookie)null)); // Null cookie
            Assert.Throws<ArgumentException>(() => cc.Add(new Cookie("name", "value", "", ""))); // Empty domain

            cc.MaxCookieSize = 1;
            Assert.Throws<CookieException>(() => cc.Add(new Cookie("name", "long-text", "", "contoso.com"))); // Value.Length > MaxCookieSize
        }

        public static IEnumerable<object[]> InvalidCookies()
        {
            const string DefaultName = "name";
            const string DefaultValue = "value";
            const string DefaultPath = "path";
            const string DefaultDomain = ".domain.com";

            yield return new[] { new Cookie(DefaultName, null, DefaultPath, DefaultDomain) }; // Null value
            yield return new[] { new Cookie(DefaultName, ";", DefaultPath, DefaultDomain) }; // Value contains reserved characters
            yield return new[] { new Cookie(DefaultName, DefaultValue, "\"/a", DefaultDomain) }; // Semi escaped path

            yield return new[] { new Cookie(DefaultName, DefaultValue, ";", DefaultDomain) }; // Path contains reserved characters

            yield return new[] { new Cookie(DefaultName, DefaultValue, DefaultPath, " ") }; // Invalid domain
            yield return new[] { new Cookie(DefaultName, DefaultValue, DefaultPath, "П.com") }; // Invalid domain

            yield return new[] { new Cookie(DefaultName, DefaultValue, DefaultPath, "domain") }; // Plain cookie, explicit domain without version doesn't start with '.'
            yield return new[] { new Cookie(DefaultName, DefaultValue, DefaultPath, "domain") { Version = 100 } }; // Rfc2965 cookie, explcit domain with version doesn't start with '.'

            yield return new[] { new Cookie(DefaultName, DefaultValue, DefaultPath, DefaultDomain) { Comment = ";" } }; // Comment contains reserved characters
        }

        [Theory]
        [MemberData(nameof(InvalidCookies))]
        public static void Add_VerificationFailedCookie_Throws(Cookie c)
        {
            CookieContainer cc = new CookieContainer();
            Assert.Throws<CookieException>(() => cc.Add(c));
        }

        [Fact]
        public static void Add_NullCookieCollection_Throws()
        {
            CookieContainer cc = new CookieContainer();
            Assert.Throws<ArgumentNullException>(() => cc.Add((CookieCollection)null)); // Null cookie
        }

        [Fact]
        public static void Add_CookieUri_Invalid()
        {
            CookieContainer cc = new CookieContainer();
            Assert.Throws<ArgumentNullException>(() => cc.Add(null, new Cookie("name", "value"))); // Null uri
            Assert.Throws<ArgumentNullException>(() => cc.Add(new Uri("http://contoso.com"), (Cookie)null)); // Null cookie
        }

        [Fact]
        public static void Add_CookieCollectionUri_Invalid()
        {
            CookieContainer cc = new CookieContainer();
            Assert.Throws<ArgumentNullException>(() => cc.Add(null, new CookieCollection())); //Null uri
            Assert.Throws<ArgumentNullException>(() => cc.Add(new Uri("http://contoso.com"), (CookieCollection)null)); //Null collection
        }

        private static IEnumerable<object[]> GetCookiesData()
        {
            yield return new object[] { new Uri("http://url1.com"), new Cookie[] { c1, c3 } };
            yield return new object[] { new Uri("https://url1.com"), new Cookie[] { c1, c2, c3 } };
            yield return new object[] { new Uri("http://127.0.0.1"), new Cookie[] { c4 } };
            yield return new object[] { new Uri("http://127.0.0.1/path"), new Cookie[] { c5, c4 } };
            yield return new object[] { new Uri("http://url3.com"), new Cookie[] { c6, c7 } };
            yield return new object[] { u4, new Cookie[] { c8, c9 } };
            yield return new object[] { u5, new Cookie[] { c10, c11 } };
            yield return new object[] { u6, new Cookie[0] };
        }

        private static IEnumerable<object[]> GetCookieHeaderData()
        {
            yield return new object[] { new Uri("http://url1.com"), "name1=value; $Version=1; name3=value; $Domain=.url1.com; $Port=\"80, 90, 100, 443\"" };
            yield return new object[] { new Uri("https://url1.com"), "name1=value; name2=value; $Version=1; name3=value; $Domain=.url1.com; $Port=\"80, 90, 100, 443\"" };
            yield return new object[] { new Uri("http://127.0.0.1"), "name4=value" };
            yield return new object[] { new Uri("http://127.0.0.1/path"), "name5=value; name4=value" };
            yield return new object[] { new Uri("http://url3.com"), "name6=value; name7=value" };
            yield return new object[] { u4, "name8=value; name9=value" };
            yield return new object[] { u5, "name10=value; $Version=1; name11=value; $Port=\"80, 90, 100\"" };
            yield return new object[] { u6, string.Empty };
        }

        [Theory]
        [MemberData(nameof(GetCookiesData))]
        public static void GetCookies_Success(Uri uri, Cookie[] expected)
        {
            CookieContainer cc = CreateCount11Container();
            VerifyGetCookies(cc, uri, expected);
        }

        private static void VerifyGetCookies(CookieContainer cc1, Uri uri, Cookie[] expected)
        {
            CookieCollection cc2 = cc1.GetCookies(uri);
            Assert.Equal(expected.Length, cc2.Count);

            for (int i = 0; i < expected.Length; i++)
            {
                Cookie c1 = expected[i];
                Cookie c2 = cc2[i];
                Assert.Equal(c1.Name, c2.Name); // Primitive check for equality
                Assert.Equal(c1.Value, c2.Value);
            }
        }

        [Fact]
        public static void GetCookies_DifferentPaths_GetsMatchedPathsIncludingEmptyPath()
        {
            // Internally, paths are stored alphabetically sorted - so expect a cookie collection sorted by the path of each cookie
            // This test ensures that all cookies with paths (that the path specified by the uri starts with) are returned
            Cookie c1 = new Cookie("name1", "value", "/aa", ".url.com");
            Cookie c2 = new Cookie("name2", "value", "/a", ".url.com");
            Cookie c3 = new Cookie("name3", "value", "/b", ".url.com"); // Should be ignored - no match with the URL's path 
            Cookie c4 = new Cookie("name4", "value", "/", ".url.com"); // Should NOT be ignored (has no path specified)

            CookieContainer cc1 = new CookieContainer();
            cc1.Add(c1);
            cc1.Add(c2);
            cc1.Add(c3);
            cc1.Add(c4);

            CookieCollection cc2 = cc1.GetCookies(new Uri("http://url.com/aaa"));
            Assert.Equal(3, cc2.Count);
            Assert.Equal(c1, cc2[0]);
            Assert.Equal(c2, cc2[1]);
            Assert.Equal(c4, cc2[2]);
        }

        [Fact]
        public static async void GetCookies_RemovesExpired_Cookies()
        {
            Cookie c1 = new Cookie("name1", "value", "", ".url1.com");
            Cookie c2 = new Cookie("name2", "value", "", ".url2.com");
            c1.Expires = DateTime.Now.AddSeconds(1); // The cookie will expire in 1 second

            CookieContainer cc = new CookieContainer();
            cc.Add(c1);
            cc.Add(c2);

            await Task.Delay(2000); // Sleep for 2 seconds to wait for the cookie to expire
            Assert.Equal(0, cc.GetCookies(new Uri("http://url1.com")).Count); // There should no longer be such a cookie
        }

        [Fact]
        public static void GetCookies_NonExistant_NoResults()
        {
            CookieContainer cc = CreateCount11Container();
            Assert.Equal(0, cc.GetCookies(new Uri("http://non.existant.uri.com")).Count);
        }

        [Fact]
        public static void GetCookies_Invalid()
        {
            CookieContainer cc = new CookieContainer();
            Assert.Throws<ArgumentNullException>(() => cc.GetCookies(null));
        }

        [Theory]
        [MemberData(nameof(GetCookieHeaderData))]
        public static void GetCookieHeader_Success(Uri uri, string expected)
        {
            CookieContainer cc = CreateCount11Container();
            Assert.Equal(expected, cc.GetCookieHeader(uri));
        }

        [Fact]
        public static void GetCookieHeader_Invalid()
        {
            CookieContainer cc = new CookieContainer();
            Assert.Throws<ArgumentNullException>(() => cc.GetCookieHeader(null));
        }

        private static IEnumerable<object[]> SetCookiesData()
        {
            yield return new object[]
            {
                u5,
                "name98=value98, name99=value99",
                new Cookie[]
                {
                    c10,
                    new Cookie("name98", "value98"),
                    new Cookie("name99", "value99"),
                    c11
                }
            }; // Simple

            Uri u = new Uri("http://uri.com");
            Uri uSecure = new Uri("https://uri.com");

            yield return new object[]
            {
                u,
                "name98=value98; path=/; domain=.uri.com; expires=Wed, 09 Jun 2021 10:18:14 GMT, name99=value99",
                new Cookie[]
                {
                    new Cookie("name99", "value99"),
                    new Cookie("name98", "value98", "/", ".uri.com") { Expires = new DateTime(2021, 6, 9, 10, 18, 14) }
                }
            }; // Version0

            yield return new object[]
            {
                uSecure,
                "name98=value98; comment=comment; domain=.uri.com; max-age=400; path=/; secure; port; Version=100, name99=value99",
                new Cookie[]
                {
                    new Cookie("name99", "value99"),
                    new Cookie("name98", "value98", "/", ".uri.com") { Port = "", Version = 100, Secure = true, Comment = "comment" }
                }
            }; // RFC 2109

            yield return new object[]
            {
                uSecure,
                "name98=value98; comment=comment; commentURL=http://url.com; discard; secure; domain=.uri.com; max-age=400; path=/; port=\"80, 90, 443\"; httponly; Version=100, name99=value99",
                new Cookie[]
                {
                    new Cookie("name99", "value99"),
                    new Cookie("name98", "value98", "/", ".uri.com") { Port = "\"80, 90, 443\"", Version = 100, Secure = true, HttpOnly = true, Discard = true, Comment = "comment", CommentUri = new Uri("http://url.com") },
                }
            }; // RFC 2965

            yield return new object[] { u,
                "name98=value98; port=\"80, 90\", name99=value99",
                new Cookie[]
                {
                    new Cookie("name99", "value99"),
                    new Cookie("name98", "value98", "/", ".uri.com") { Port = "\"80, 90\"" },
                }
            }; // RFC 2965 (no path)

            yield return new object[] {
                uSecure,
                "name98=value98; name98=value98; comment=comment; comment=comment2; commentURL=http://url.com; commentURL=commentURL2; discard; discard; domain=.uri.com; domain=domain2; max-age=400; max-age=400; path=/; path=path; port=\"80, 90, 443\"; port=port2; path=path; expires=Wed, 09 Jun 2021 10:18:14 GMT; expires=expires2; secure; secure; httponly; httponly; Version=100; Version=100, name99=value99",
                new Cookie[]
                {
                    new Cookie("name99", "value99"),
                    new Cookie("name98", "value98", "/", ".uri.com") { Port = "\"80, 90, 443\"", Version = 100, Secure = true, HttpOnly = true, Discard = true, Expires = new DateTime(2021, 6, 9, 10, 18, 14), Comment = "comment", CommentUri = new Uri("http://url.com") }
                }
            }; // Double entries

            yield return new object[] {
                u,
                "name98=value98; commentURL=invalidurl",
                new Cookie[]
                {
                    new Cookie("name98", "value98")
                }
            }; // Ignore invalid comment url

            yield return new object[] {
                u6,
                "name98=value98; unknown1; unknown2=unknown",
                new Cookie[]
                {
                    new Cookie("name98", "value98")
                }
            }; // Ignore unknown tokens

            yield return new object[] {
                u6,
                "name98=value98; =; token=",
                new Cookie[]
                {
                    new Cookie("name98", "value98")
                }
            }; // Ignore invalid tokens

            yield return new object[] {
                u6,
                "name98=\"value; domain=\".domain\"; max-age=\"400\"",
                new Cookie[]
                {
                    new Cookie("name98", "\"value; domain=\"")
                }
            }; // Use escaped values (1)

            yield return new object[] {
                u6,
                "name98=\"\"",
                new Cookie[]
                {
                    new Cookie("name98", "\"\"")
                }
            }; // Use escaped values (2)
        }

        [Theory]
        [MemberData(nameof(SetCookiesData))]
        public static void SetCookies_Success(Uri uri, string cookieHeader, Cookie[] expected)
        {
            CookieContainer cc = CreateCount11Container();
            cc.SetCookies(uri, cookieHeader);
            // Now that we've set cookie headers, we should now check to see if they're added
            VerifyGetCookies(cc, uri, expected);
        }

        private static IEnumerable<object[]> SetCookiesInvalidData()
        {
            yield return new object[] { u5, "=value" }; // No name
            yield return new object[] { u5, "$=value" }; // Invalid name
            yield return new object[] { new Uri("http://url.com"), "na\tme=value; domain=.domain.com" }; // Invalid name
            yield return new object[] { new Uri("http://url.com"), "name=value; domain=.domain.com" }; // Domain not the same
            yield return new object[] { new Uri("http://url.com/path"), "name=value; domain=.url.com; path=/root" }; // Path not the same
            yield return new object[] { new Uri("http://url.com:90"), "name=value; port=\"80\"" }; // Port not the same
            yield return new object[] { new Uri("http://url.com"), "name=value; domain=" }; // Empty domain
            yield return new object[] { u6, "name11=value11; version=invalidversion" }; // Invalid version
            yield return new object[] { u6, "name11=value11; expires=invaliddate" }; // Invalid date
            yield return new object[] { u6, "name11=value11; max-age=invalidmaxage" }; // Invalid max age
            yield return new object[] { u6, "name11=value11; domain=invaliddomain" }; // Invalid domain
            yield return new object[] { u6, "name11=value11; port=invalidport" }; // Invalid port
        }

        [Theory]
        [MemberData(nameof(SetCookiesInvalidData))]
        public static void SetCookies_InvalidData_Throws(Uri uri, string cookieHeader)
        {
            CookieContainer cc = new CookieContainer();
            Assert.Throws<CookieException>(() => cc.SetCookies(uri, cookieHeader));
        }

        [Fact]
        public static void SetCookies_InvalidInput_Throws()
        {
            CookieContainer cc = new CookieContainer();
            Assert.Throws<ArgumentNullException>(() => cc.SetCookies(null, "")); // Null uri
            Assert.Throws<ArgumentNullException>(() => cc.SetCookies(u5, null)); // Null header
        }
    }
}
