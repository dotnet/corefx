// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Runtime.Tests
{
    public static class UriTests
    {
        [Fact]
        public static void TestCtor_String()
        {
            var uri = new Uri("http://foo/bar/baz#frag");

            Assert.Equal("http://foo/bar/baz#frag", uri.OriginalString);
            Assert.Equal("http://foo/bar/baz#frag", uri.AbsoluteUri);
            Assert.Equal("http://foo/bar/baz#frag", uri.ToString());

            Assert.Equal("http", uri.Scheme);

            Assert.Equal("foo", uri.Authority);
            Assert.Equal("foo", uri.Host);
            Assert.Equal("foo", uri.DnsSafeHost);
            Assert.Equal(UriHostNameType.Dns, uri.HostNameType);

            Assert.Equal(80, uri.Port);

            Assert.Equal("/bar/baz", uri.AbsolutePath);
            Assert.Equal("/bar/baz", uri.LocalPath);
            Assert.Equal("/bar/baz", uri.PathAndQuery);

            Assert.Equal(new string[] { "/", "bar/", "baz" }, uri.Segments);
            
            Assert.Equal("#frag", uri.Fragment);

            Assert.Equal("", uri.Query);
            Assert.Equal("", uri.UserInfo);

            Assert.True(uri.IsAbsoluteUri);
            Assert.True(uri.IsDefaultPort);

            Assert.False(uri.IsFile);
            Assert.False(uri.IsLoopback);
            Assert.False(uri.IsUnc);

            Assert.False(uri.UserEscaped);
        }

        [Fact]
        public static void TestCtor_Uri_String()
        {
            var uri = new Uri(new Uri("http://www.contoso.com/"), @"catalog/shownew.htm?date=today");

            Assert.Equal("http://www.contoso.com/catalog/shownew.htm?date=today", uri.OriginalString);
            Assert.Equal("http://www.contoso.com/catalog/shownew.htm?date=today", uri.AbsoluteUri);
            Assert.Equal("http://www.contoso.com/catalog/shownew.htm?date=today", uri.ToString());

            Assert.Equal("http", uri.Scheme);

            Assert.Equal("www.contoso.com", uri.Authority);
            Assert.Equal("www.contoso.com", uri.Host);
            Assert.Equal("www.contoso.com", uri.DnsSafeHost);
            Assert.Equal(UriHostNameType.Dns, uri.HostNameType);

            Assert.Equal(80, uri.Port);

            Assert.Equal("/catalog/shownew.htm", uri.AbsolutePath);
            Assert.Equal("/catalog/shownew.htm", uri.LocalPath);
            Assert.Equal("/catalog/shownew.htm?date=today", uri.PathAndQuery);

            Assert.Equal(new string[] { "/", "catalog/", "shownew.htm" }, uri.Segments);
            
            Assert.Equal("", uri.Fragment);

            Assert.Equal("?date=today", uri.Query);
            Assert.Equal("", uri.UserInfo);

            Assert.True(uri.IsAbsoluteUri);
            Assert.True(uri.IsDefaultPort);

            Assert.False(uri.IsFile);
            Assert.False(uri.IsLoopback);
            Assert.False(uri.IsUnc);

            Assert.False(uri.UserEscaped);
        }

        [Fact]
        public static void TestCtor_String_UriKind()
        {
            var uri = new Uri("catalog/shownew.htm?date=today", UriKind.Relative);

            Assert.Equal("catalog/shownew.htm?date=today", uri.OriginalString);
            Assert.Throws<InvalidOperationException>(() => uri.AbsoluteUri);
            Assert.Equal("catalog/shownew.htm?date=today", uri.ToString());

            Assert.Throws<InvalidOperationException>(() => uri.Scheme);

            Assert.Throws<InvalidOperationException>(() => uri.Authority);
            Assert.Throws<InvalidOperationException>(() => uri.Host);
            Assert.Throws<InvalidOperationException>(() => uri.DnsSafeHost);
            Assert.Throws<InvalidOperationException>(() => uri.HostNameType);

            Assert.Throws<InvalidOperationException>(() => uri.AbsolutePath);
            Assert.Throws<InvalidOperationException>(() => uri.LocalPath);
            Assert.Throws<InvalidOperationException>(() => uri.PathAndQuery);

            Assert.Throws<InvalidOperationException>(() => uri.Segments);

            Assert.Throws<InvalidOperationException>(() => uri.Fragment);

            Assert.Throws<InvalidOperationException>(() => uri.Query);
            Assert.Throws<InvalidOperationException>(() => uri.UserInfo);
            
            Assert.False(uri.IsAbsoluteUri);
            Assert.Throws<InvalidOperationException>(() => uri.IsDefaultPort);

            Assert.Throws<InvalidOperationException>(() => uri.IsFile);
            Assert.Throws<InvalidOperationException>(() => uri.IsLoopback);
            Assert.Throws<InvalidOperationException>(() => uri.IsUnc);

            Assert.False(uri.UserEscaped);
        }

        [Fact]
        public static void TestCtor_Uri_Uri()
        {
            var absoluteUri = new Uri("http://www.contoso.com");
            var relativeUri = new Uri("/catalog/shownew.htm?date=today", UriKind.Relative);

            var uri = new Uri(absoluteUri, relativeUri);

            Assert.Equal("http://www.contoso.com/catalog/shownew.htm?date=today", uri.OriginalString);
            Assert.Equal("http://www.contoso.com/catalog/shownew.htm?date=today", uri.AbsoluteUri);
            Assert.Equal("http://www.contoso.com/catalog/shownew.htm?date=today", uri.ToString());

            Assert.Equal("http", uri.Scheme);

            Assert.Equal("www.contoso.com", uri.Authority);
            Assert.Equal("www.contoso.com", uri.Host);
            Assert.Equal("www.contoso.com", uri.DnsSafeHost);
            Assert.Equal(UriHostNameType.Dns, uri.HostNameType);

            Assert.Equal(80, uri.Port);

            Assert.Equal("/catalog/shownew.htm", uri.AbsolutePath);
            Assert.Equal("/catalog/shownew.htm", uri.LocalPath);
            Assert.Equal("/catalog/shownew.htm?date=today", uri.PathAndQuery);

            Assert.Equal(new string[] { "/", "catalog/", "shownew.htm" }, uri.Segments);

            Assert.Equal("", uri.Fragment);

            Assert.Equal("?date=today", uri.Query);
            Assert.Equal("", uri.UserInfo);

            Assert.True(uri.IsAbsoluteUri);
            Assert.True(uri.IsDefaultPort);

            Assert.False(uri.IsFile);
            Assert.False(uri.IsLoopback);
            Assert.False(uri.IsUnc);

            Assert.False(uri.UserEscaped);
        }

        [Fact]
        public static void TestTryCreate_String_UriKind()
        {
            Uri uri;
            Assert.True(Uri.TryCreate("http://www.contoso.com/catalog/shownew.htm?date=today", UriKind.Absolute, out uri));

            Assert.Equal("http://www.contoso.com/catalog/shownew.htm?date=today", uri.OriginalString);
            Assert.Equal("http://www.contoso.com/catalog/shownew.htm?date=today", uri.AbsoluteUri);
            Assert.Equal("http://www.contoso.com/catalog/shownew.htm?date=today", uri.ToString());

            Assert.Equal("http", uri.Scheme);

            Assert.Equal("www.contoso.com", uri.Authority);
            Assert.Equal("www.contoso.com", uri.Host);
            Assert.Equal("www.contoso.com", uri.DnsSafeHost);
            Assert.Equal(UriHostNameType.Dns, uri.HostNameType);

            Assert.Equal(80, uri.Port);

            Assert.Equal("/catalog/shownew.htm", uri.AbsolutePath);
            Assert.Equal("/catalog/shownew.htm", uri.LocalPath);
            Assert.Equal("/catalog/shownew.htm?date=today", uri.PathAndQuery);

            Assert.Equal(new string[] { "/", "catalog/", "shownew.htm" }, uri.Segments);

            Assert.Equal("", uri.Fragment);

            Assert.Equal("?date=today", uri.Query);
            Assert.Equal("", uri.UserInfo);

            Assert.True(uri.IsAbsoluteUri);
            Assert.True(uri.IsDefaultPort);

            Assert.False(uri.IsFile);
            Assert.False(uri.IsLoopback);
            Assert.False(uri.IsUnc);

            Assert.False(uri.UserEscaped);
        }

        [Fact]
        public static void TestTryCreate_Uri_String()
        {
            Uri uri;
            Assert.True(Uri.TryCreate(new Uri("http://www.contoso.com/"), @"catalog/shownew.htm?date=today", out uri));

            Assert.Equal("http://www.contoso.com/catalog/shownew.htm?date=today", uri.OriginalString);
            Assert.Equal("http://www.contoso.com/catalog/shownew.htm?date=today", uri.AbsoluteUri);
            Assert.Equal("http://www.contoso.com/catalog/shownew.htm?date=today", uri.ToString());

            Assert.Equal("http", uri.Scheme);

            Assert.Equal("www.contoso.com", uri.Authority);
            Assert.Equal("www.contoso.com", uri.Host);
            Assert.Equal("www.contoso.com", uri.DnsSafeHost);
            Assert.Equal(UriHostNameType.Dns, uri.HostNameType);

            Assert.Equal(80, uri.Port);

            Assert.Equal("/catalog/shownew.htm", uri.AbsolutePath);
            Assert.Equal("/catalog/shownew.htm", uri.LocalPath);
            Assert.Equal("/catalog/shownew.htm?date=today", uri.PathAndQuery);

            Assert.Equal(new string[] { "/", "catalog/", "shownew.htm" }, uri.Segments);

            Assert.Equal("", uri.Fragment);

            Assert.Equal("?date=today", uri.Query);
            Assert.Equal("", uri.UserInfo);

            Assert.True(uri.IsAbsoluteUri);
            Assert.True(uri.IsDefaultPort);

            Assert.False(uri.IsFile);
            Assert.False(uri.IsLoopback);
            Assert.False(uri.IsUnc);

            Assert.False(uri.UserEscaped);
        }

        [Fact]
        public static void TestTryCreate_Uri_Uri()
        {
            var absoluteUri = new Uri("http://www.contoso.com");
            var relativeUri = new Uri("/catalog/shownew.htm?date=today", UriKind.Relative);

            Uri uri;
            Assert.True(Uri.TryCreate(absoluteUri, relativeUri, out uri));

            Assert.Equal("http://www.contoso.com/catalog/shownew.htm?date=today", uri.OriginalString);
            Assert.Equal("http://www.contoso.com/catalog/shownew.htm?date=today", uri.AbsoluteUri);
            Assert.Equal("http://www.contoso.com/catalog/shownew.htm?date=today", uri.ToString());

            Assert.Equal("http", uri.Scheme);

            Assert.Equal("www.contoso.com", uri.Authority);
            Assert.Equal("www.contoso.com", uri.Host);
            Assert.Equal("www.contoso.com", uri.DnsSafeHost);
            Assert.Equal(UriHostNameType.Dns, uri.HostNameType);

            Assert.Equal(80, uri.Port);

            Assert.Equal("/catalog/shownew.htm", uri.AbsolutePath);
            Assert.Equal("/catalog/shownew.htm", uri.LocalPath);
            Assert.Equal("/catalog/shownew.htm?date=today", uri.PathAndQuery);

            Assert.Equal(new string[] { "/", "catalog/", "shownew.htm" }, uri.Segments);

            Assert.Equal("", uri.Fragment);

            Assert.Equal("?date=today", uri.Query);
            Assert.Equal("", uri.UserInfo);

            Assert.True(uri.IsAbsoluteUri);
            Assert.True(uri.IsDefaultPort);

            Assert.False(uri.IsFile);
            Assert.False(uri.IsLoopback);
            Assert.False(uri.IsUnc);

            Assert.False(uri.UserEscaped);
        }

        [Fact]
        public static void TestMakeRelative()
        {
            var uri1 = new Uri("http://www.contoso.com/");
            var uri2 = new Uri("http://www.contoso.com/index.htm?date=today");
            
            Uri uri = uri1.MakeRelativeUri(uri2);

            Assert.Equal("index.htm?date=today", uri.OriginalString);
            Assert.Throws<InvalidOperationException>(() => uri.AbsoluteUri);
            Assert.Equal("index.htm?date=today", uri.ToString());
            
            Assert.Throws<InvalidOperationException>(() => uri.Scheme);

            Assert.Throws<InvalidOperationException>(() => uri.Authority);
            Assert.Throws<InvalidOperationException>(() => uri.Host);
            Assert.Throws<InvalidOperationException>(() => uri.DnsSafeHost);
            Assert.Throws<InvalidOperationException>(() => uri.HostNameType);

            Assert.Throws<InvalidOperationException>(() => uri.Port);

            Assert.Throws<InvalidOperationException>(() => uri.AbsolutePath);
            Assert.Throws<InvalidOperationException>(() => uri.LocalPath);
            Assert.Throws<InvalidOperationException>(() => uri.PathAndQuery);

            Assert.Throws<InvalidOperationException>(() => uri.Segments);

            Assert.Throws<InvalidOperationException>(() => uri.Fragment);

            Assert.Throws<InvalidOperationException>(() => uri.Query);
            Assert.Throws<InvalidOperationException>(() => uri.UserInfo);

            Assert.False(uri.IsAbsoluteUri);
            Assert.Throws<InvalidOperationException>(() => uri.IsDefaultPort);

            Assert.Throws<InvalidOperationException>(() => uri.IsFile);
            Assert.Throws<InvalidOperationException>(() => uri.IsLoopback);
            Assert.Throws<InvalidOperationException>(() => uri.IsUnc);

            Assert.False(uri.UserEscaped);
        }

        [Theory]
        [InlineData("www.contoso.com", UriHostNameType.Dns)]
        [InlineData("1.2.3.4", UriHostNameType.IPv4)]
        [InlineData(null, UriHostNameType.Unknown)]
        [InlineData("", UriHostNameType.Unknown)]
        [InlineData("!@*(@#&*#$&*#", UriHostNameType.Unknown)]
        public static void TestCheckHostName(string name, UriHostNameType expected)
        {
            Assert.Equal(expected, Uri.CheckHostName(name));
        }

        [Theory]
        [InlineData("http", true)]
        [InlineData("https", true)]
        [InlineData("file", true)]
        [InlineData(null, false)]
        [InlineData("", false)]
        [InlineData("!", false)]
        public static void TestCheckSchemeName(string schemeName, bool expected)
        {
            Assert.Equal(expected, Uri.CheckSchemeName(schemeName));
        }

        [Theory]
        [InlineData("http://host/path/path/file?query", "http://host/path/path/file/", true)]
        [InlineData("http://host/path/path/file?query", "http://host/path/path/#fragment", true)]
        [InlineData("http://host/path/path/file?query", "http://host/path/path/MoreDir/\"", true)]
        [InlineData("http://host/path/path/file?query", "http://host/path/path/OtherFile?Query", true)]
        [InlineData("http://host/path/path/file?query", "http://host/path/path/", true)]
        [InlineData("http://host/path/path/file?query", "http://host/path/path/file", true)]
        [InlineData("http://host/path/path/file?query", "http://host/path/path", false)]
        [InlineData("http://host/path/path/file?query", "http://host/path/path?query", false)]
        [InlineData("http://host/path/path/file?query", "http://host/path/path#fragment", false)]
        [InlineData("http://host/path/path/file?query", "http://host/path/path2", false)]
        [InlineData("http://host/path/path/file?query", "http://host/path/path2/path3", false)]
        [InlineData("http://host/path/path/file?query", "http://host/path/File", false)]
        public static void TestIsBaseOf(string uriString1, string uriString2, bool expected)
        {
            var uri1 = new Uri(uriString1);
            var uri2 = new Uri(uriString2);

            Assert.Equal(expected, uri1.IsBaseOf(uri2));
        }

        [Theory]
        [InlineData("http://www.contoso.com/path?name", true)]
        [InlineData("http://www.contoso.com/path???/file name", false)]
        [InlineData(@"c:\\directory\filename", false)]
        [InlineData("file://C:/directory/filename", false)]
        [InlineData(@"http:\\host/path/file", false)]
        public static void TestIsWellFormedOriginalString(string uriString, bool expected)
        {
            var uri = new Uri(uriString);
            Assert.Equal(expected, uri.IsWellFormedOriginalString());
        }

        [Theory]
        [InlineData("http://www.contoso.com/path?name", UriKind.RelativeOrAbsolute, true)]
        [InlineData("http://www.contoso.com/path???/file name", UriKind.RelativeOrAbsolute, false)]
        [InlineData("c:\\directory\filename", UriKind.RelativeOrAbsolute, false)]
        [InlineData(@"file://C:/directory/filename", UriKind.RelativeOrAbsolute, false)]
        [InlineData("http:\\host/path/file", UriKind.RelativeOrAbsolute, false)]
        public static void TestIsWellFormedUriString(string uriString, UriKind uriKind, bool expected)
        {
            Assert.Equal(expected, Uri.IsWellFormedUriString(uriString, uriKind));
        }
        
        private static readonly Uri s_uri1 = new Uri("http://www.contoso.com/path?name#frag");
        private static readonly Uri s_uri2 = new Uri("http://www.contosooo.com/path?name#slag");
        private static readonly Uri s_uri2a = new Uri("http://www.contosooo.com/path?name#slag");

        [Fact]
        public static void TestCompare()
        {
            Assert.Equal(-1, Uri.Compare(s_uri1, s_uri2, UriComponents.AbsoluteUri, UriFormat.UriEscaped, StringComparison.CurrentCulture));
            Assert.Equal(0, Uri.Compare(s_uri1, s_uri2, UriComponents.Query, UriFormat.UriEscaped, StringComparison.CurrentCulture));
            Assert.Equal(-1, Uri.Compare(s_uri1, s_uri2, UriComponents.Query | UriComponents.Fragment, UriFormat.UriEscaped, StringComparison.CurrentCulture));
        }

        [Fact]
        public static void TestEquals()
        {
            Assert.True(s_uri1.Equals(s_uri1));
            Assert.True(s_uri1 == s_uri1);
            Assert.False(s_uri1 != s_uri1);

            Assert.False(s_uri1.Equals(s_uri2));
            Assert.False(s_uri1 == s_uri2);
            Assert.True(s_uri1 != s_uri2);

            Assert.True(s_uri2.Equals(s_uri2a));
            Assert.True(s_uri2 == s_uri2a);
            Assert.False(s_uri2 != s_uri2a);

            Assert.True(s_uri2a.Equals(s_uri2));
            Assert.True(s_uri2a == s_uri2);
            Assert.False(s_uri2a != s_uri2);
        }

        [Fact]
        public static void TestGetHashCode()
        {
            Assert.NotEqual(s_uri1.GetHashCode(), s_uri2.GetHashCode());
            Assert.Equal(s_uri2.GetHashCode(), s_uri2a.GetHashCode());
            Assert.Equal(s_uri2.GetHashCode(), s_uri2a.GetHashCode());
        }

        [Theory]
        [InlineData("Hello", "Hello")]
        [InlineData(@"He\l/lo", @"He%5Cl%2Flo")]
        public static void TestEscapeDataString(string stringToEscape, string expected)
        {
            Assert.Equal(expected, Uri.EscapeDataString(stringToEscape));
        }

        [Theory]
        [InlineData("Hello", "Hello")]
        [InlineData(@"He%5Cl/lo", @"He\l/lo")]
        public static void TestUnescapeDataString(string stringToUnEscape, string expected)
        {
            Assert.Equal(expected, Uri.UnescapeDataString(stringToUnEscape));
        }

        [Theory]
        [InlineData("Hello", "Hello")]
        [InlineData(@"He\l/lo", @"He%5Cl/lo")]
        public static void TestEscapeUriString(string stringToEscape, string expected)
        {
            Assert.Equal(expected, Uri.EscapeUriString(stringToEscape));
        }

        [Theory]
        [InlineData("http://www.contoso.com/path?name#frag", UriComponents.Fragment, UriFormat.UriEscaped, "frag")]
        [InlineData("http://www.contoso.com/path?name#frag", UriComponents.Host, UriFormat.UriEscaped, "www.contoso.com")]
        public static void TestGetComponentParts(string uriString, UriComponents components, UriFormat format, string expected)
        {
            var uri = new Uri(uriString);
            Assert.Equal(expected, uri.GetComponents(components, format));
        }
    }
}
