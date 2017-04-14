// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;

using Xunit;

namespace System.Net.Primitives.Functional.Tests
{
    public static class CredentialCacheTest
    {
        private static readonly Uri uriPrefix1 = new Uri("http://microsoft:80");
        private static readonly Uri uriPrefix2 = new Uri("http://softmicro:80");

        private static readonly string host1 = "host1";
        private static readonly string host2 = "host2";

        private static readonly int port1 = 500;
        private static readonly int port2 = 700;

        private static readonly string authenticationType1 = "authenticationType1";
        private static readonly string authenticationType2 = "authenticationType2";

        private static readonly NetworkCredential credential1 = new NetworkCredential("username1", "password");
        private static readonly NetworkCredential credential2 = new NetworkCredential("username2", "password");
        private static readonly NetworkCredential credential3 = new NetworkCredential("username3", "password");
        private static readonly NetworkCredential credential4 = new NetworkCredential("username4", "password");

        private static readonly NetworkCredential credential5 = new NetworkCredential("username5", "password");
        private static readonly NetworkCredential credential6 = new NetworkCredential("username6", "password");
        private static readonly NetworkCredential credential7 = new NetworkCredential("username7", "password");
        private static readonly NetworkCredential credential8 = new NetworkCredential("username8", "password");

        private struct CredentialCacheCount
        {
            public CredentialCacheCount(CredentialCache cc, int count)
            {
                CredentialCache = cc;
                Count = count;
            }

            public CredentialCache CredentialCache { get; }
            public int Count { get; }
        }

        private static CredentialCache CreateUriCredentialCache() =>
            CreateUriCredentialCacheCount().CredentialCache;

        private static CredentialCacheCount CreateUriCredentialCacheCount(CredentialCache cc = null, int count = 0)
        {
            cc = cc ?? new CredentialCache();

            cc.Add(uriPrefix1, authenticationType1, credential1); count++;
            cc.Add(uriPrefix1, authenticationType2, credential2); count++;

            cc.Add(uriPrefix2, authenticationType1, credential3); count++;
            cc.Add(uriPrefix2, authenticationType2, credential4); count++;

            return new CredentialCacheCount(cc, count);
        }

        private static CredentialCache CreateHostPortCredentialCache() =>
            CreateHostPortCredentialCacheCount().CredentialCache;

        private static CredentialCacheCount CreateHostPortCredentialCacheCount(CredentialCache cc = null, int count = 0)
        {
            cc = cc ?? new CredentialCache();

            cc.Add(host1, port1, authenticationType1, credential1); count++;
            cc.Add(host1, port1, authenticationType2, credential2); count++;
            cc.Add(host1, port2, authenticationType1, credential3); count++;
            cc.Add(host1, port2, authenticationType2, credential4); count++;

            cc.Add(host2, port1, authenticationType1, credential5); count++;
            cc.Add(host2, port1, authenticationType2, credential6); count++;
            cc.Add(host2, port2, authenticationType1, credential7); count++;
            cc.Add(host2, port2, authenticationType2, credential8); count++;

            return new CredentialCacheCount(cc, count);
        }

        private static CredentialCacheCount CreateUriAndHostPortCredentialCacheCount()
        {
            CredentialCacheCount uri = CreateUriCredentialCacheCount();
            return CreateHostPortCredentialCacheCount(uri.CredentialCache, uri.Count);
        }

        private static IEnumerable<CredentialCacheCount> GetCredentialCacheCounts()
        {
            yield return new CredentialCacheCount(new CredentialCache(), 0);
            yield return CreateUriCredentialCacheCount();
            yield return CreateHostPortCredentialCacheCount();
            yield return CreateUriAndHostPortCredentialCacheCount();
        }

        [Fact]
        public static void Ctor_Empty_Success()
        {
            CredentialCache cc = new CredentialCache();
        }

        [Fact]
        public static void Add_UriAuthenticationTypeCredential_Success()
        {
            CredentialCache cc = CreateUriCredentialCache();

            Assert.Equal(credential1, cc.GetCredential(uriPrefix1, authenticationType1));
            Assert.Equal(credential2, cc.GetCredential(uriPrefix1, authenticationType2));

            Assert.Equal(credential3, cc.GetCredential(uriPrefix2, authenticationType1));
            Assert.Equal(credential4, cc.GetCredential(uriPrefix2, authenticationType2));
        }
        
        [Fact]
        public static void Add_UriAuthenticationTypeCredential_Invalid()
        {
            CredentialCache cc = CreateUriCredentialCache();

            Assert.Null(cc.GetCredential(new Uri("http://invalid.uri"), authenticationType1)); //No such uriPrefix
            Assert.Null(cc.GetCredential(uriPrefix1, "invalid-authentication-type")); //No such authenticationType

            Assert.Throws<ArgumentNullException>("uriPrefix", () => cc.Add(null, "some", new NetworkCredential())); //Null uriPrefix
            Assert.Throws<ArgumentNullException>("authType", () => cc.Add(new Uri("http://microsoft:80"), null, new NetworkCredential())); //Null authenticationType
        }

        [Fact]
        public static void Add_UriAuthenticationTypeCredential_DuplicateItem_Throws()
        {
            CredentialCache cc = new CredentialCache();
            cc.Add(uriPrefix1, authenticationType1, credential1);

            Assert.Throws<ArgumentException>(() => cc.Add(uriPrefix1, authenticationType1, credential1));
        }

        [Fact]
        public static void Add_HostPortAuthenticationTypeCredential_Success()
        {
            CredentialCache cc = CreateHostPortCredentialCache();

            Assert.Equal(credential1, cc.GetCredential(host1, port1, authenticationType1));
            Assert.Equal(credential2, cc.GetCredential(host1, port1, authenticationType2));
            Assert.Equal(credential3, cc.GetCredential(host1, port2, authenticationType1));
            Assert.Equal(credential4, cc.GetCredential(host1, port2, authenticationType2));

            Assert.Equal(credential5, cc.GetCredential(host2, port1, authenticationType1));
            Assert.Equal(credential6, cc.GetCredential(host2, port1, authenticationType2));
            Assert.Equal(credential7, cc.GetCredential(host2, port2, authenticationType1));
            Assert.Equal(credential8, cc.GetCredential(host2, port2, authenticationType2));
        }

        [Fact]
        public static void Add_HostPortAuthenticationTypeCredential_Invalid()
        {
            CredentialCache cc = CreateHostPortCredentialCache();
            
            Assert.Null(cc.GetCredential("invalid-host", port1, authenticationType1)); //No such host
            Assert.Null(cc.GetCredential(host1, 900, authenticationType1)); //No such port
            Assert.Null(cc.GetCredential(host1, port1, "invalid-authentication-type")); //No such authenticationType

            Assert.Throws<ArgumentNullException>("host", () => cc.Add(null, 500, "authenticationType", new NetworkCredential())); //Null host
            Assert.Throws<ArgumentNullException>("authenticationType", () => cc.Add("host", 500, null, new NetworkCredential())); //Null authenticationType

            var exception = Record.Exception(() => cc.Add("", 500, "authenticationType", new NetworkCredential())); 
            // On full framework we get exception.ParamName as null while it is "host" on netcore
            Assert.NotNull(exception);
            Assert.True(exception is ArgumentException);
            ArgumentException ae = exception as ArgumentException;
            Assert.True(ae.ParamName == "host" || ae.ParamName == null);

            Assert.Throws<ArgumentOutOfRangeException>("port", () => cc.Add("host", -1, "authenticationType", new NetworkCredential())); //Port < 0
        }

        [Fact]
        public static void Add_HostPortAuthenticationTypeCredential_DuplicateItem_Throws()
        {
            CredentialCache cc = new CredentialCache();
            cc.Add(host1, port1, authenticationType1, credential1);

            Assert.Throws<ArgumentException>(() => cc.Add(host1, port1, authenticationType1, credential1));
        }

        [Fact]
        public static void Remove_UriAuthenticationType_Success()
        {
            CredentialCache cc = CreateUriCredentialCache();
            
            cc.Remove(uriPrefix1, authenticationType1);
            Assert.Null(cc.GetCredential(uriPrefix1, authenticationType1));
        }

        [Fact]
        public static void Remove_UriAuthenticationType_Invalid()
        {
            CredentialCache cc = new CredentialCache();

            //Doesn't throw, just returns
            cc.Remove(null, "authenticationType");
            cc.Remove(new Uri("http://some.com"), null);
            cc.Remove(new Uri("http://some.com"), "authenticationType");
        }

        [Fact]
        public static void Remove_HostPortAuthenticationType_Success()
        {
            CredentialCache cc = CreateHostPortCredentialCache();

            cc.Remove(host1, port1, authenticationType1);
            Assert.Null(cc.GetCredential(host1, port1, authenticationType1));
        }

        [Fact]
        public static void Remove_HostPortAuthenticationType_Invalid()
        {
            CredentialCache cc = new CredentialCache();

            //Doesn't throw, just returns
            cc.Remove(null, 500, "authenticationType");
            cc.Remove("host", 500, null);
            cc.Remove("host", -1, "authenticationType");
            cc.Remove("host", 500, "authenticationType");
        }

        [Fact]
        public static void GetCredential_SimilarUriAuthenticationType_GetLongestUriPrefix()
        {
            CredentialCache cc = new CredentialCache();
            cc.Add(new Uri("http://microsoft:80/greaterpath"), authenticationType1, credential2);
            cc.Add(new Uri("http://microsoft:80/"), authenticationType1, credential1);

            NetworkCredential nc = cc.GetCredential(new Uri("http://microsoft:80"), authenticationType1);
            Assert.Equal(nc, credential2);
        }

        [Fact]
        public static void GetCredential_UriAuthenticationType_Invalid()
        {
            CredentialCache cc = new CredentialCache();

            Assert.Throws<ArgumentNullException>("uriPrefix", () => cc.GetCredential(null, "authenticationType")); //Null uriPrefix
            Assert.Throws<ArgumentNullException>("authType", () => cc.GetCredential(new Uri("http://microsoft:80"), null)); //Null authenticationType
        }

        [Fact]
        public static void GetCredential_HostPortAuthenticationType_Invalid()
        {
            CredentialCache cc = new CredentialCache();

            Assert.Throws<ArgumentNullException>("host", () => cc.GetCredential(null, 500, "authenticationType")); //Null host
            Assert.Throws<ArgumentNullException>("authenticationType", () => cc.GetCredential("host", 500, null)); //Null authenticationType

            var exception = Record.Exception(() => cc.GetCredential("", 500, "authenticationType")); //Empty host
            // On full framework we get exception.ParamName as null while it is "host" on netcore
            Assert.NotNull(exception);
            Assert.True(exception is ArgumentException);
            ArgumentException ae = exception as ArgumentException;
            Assert.True(ae.ParamName == "host" || ae.ParamName == null);

            Assert.Throws<ArgumentOutOfRangeException>("port", () => cc.GetCredential("host", -1, "authenticationType")); //Port < 0
        }

        public static IEnumerable<object[]> GetEnumeratorWithCountTestData
        {
            get
            {
                foreach (CredentialCacheCount ccc in GetCredentialCacheCounts())
                {
                    yield return new object[] { ccc.CredentialCache, ccc.Count };
                }
            }
        }

        [Theory]
        [MemberData(nameof(GetEnumeratorWithCountTestData))]
        public static void GetEnumerator_Enumerate_Success(CredentialCache cc, int count)
        {
            IEnumerator enumerator = cc.GetEnumerator();

            Assert.NotNull(enumerator);

            for (int iterations = 0; iterations < 2; iterations++)
            {
                Assert.Throws<InvalidOperationException>(() => enumerator.Current);

                for (int i = 0; i < count; i++)
                {
                    Assert.True(enumerator.MoveNext());
                    Assert.NotNull(enumerator.Current);
                }

                Assert.False(enumerator.MoveNext());
                Assert.Throws<InvalidOperationException>(() => enumerator.Current);

                enumerator.Reset();
            }
        }

        public static IEnumerable<object[]> GetEnumeratorThenAddTestData
        {
            get
            {
                foreach (bool addUri in new[] { true, false })
                {
                    foreach (CredentialCacheCount ccc in GetCredentialCacheCounts())
                    {
                        yield return new object[] { ccc.CredentialCache, addUri };
                    }
                }
            }
        }

        [Theory]
        [MemberData(nameof(GetEnumeratorThenAddTestData))]
        public static void GetEnumerator_MoveNextSynchronization_Invalid(CredentialCache cc, bool addUri)
        {
            //An InvalidOperationException is thrown when moving the enumerator
            //when a credential is added to the cache after getting the enumerator
            IEnumerator enumerator = cc.GetEnumerator();

            if (addUri)
            {
                cc.Add(new Uri("http://whatever:80"), authenticationType1, credential1);
            }
            else
            {
                cc.Add("whatever", 80, authenticationType1, credential1);
            }

            Assert.Throws<InvalidOperationException>(() => enumerator.MoveNext());
        }

        [Theory]
        [MemberData(nameof(GetEnumeratorThenAddTestData))]
        public static void GetEnumerator_CurrentSynchronization_Invalid(CredentialCache cc, bool addUri)
        {
            //An InvalidOperationException is thrown when getting the current enumerated object
            //when a credential is added to the cache after getting the enumerator
            IEnumerator enumerator = cc.GetEnumerator();

            enumerator.MoveNext();

            if (addUri)
            {
                cc.Add(new Uri("http://whatever:80"), authenticationType1, credential1);
            }
            else
            {
                cc.Add("whatever", 80, authenticationType1, credential1);
            }

            Assert.Throws<InvalidOperationException>(() => enumerator.Current);
        }

        public static IEnumerable<object[]> GetEnumeratorTestData
        {
            get
            {
                foreach (CredentialCacheCount ccc in GetCredentialCacheCounts())
                {
                    yield return new object[] { ccc.CredentialCache };
                }
            }
        }

        [Theory]
        [MemberData(nameof(GetEnumeratorTestData))]
        public static void GetEnumerator_ResetIndexGetCurrent_Invalid(CredentialCache cc)
        {
            IEnumerator enumerator = cc.GetEnumerator();
            enumerator.Reset();
            Assert.Throws<InvalidOperationException>(() => enumerator.Current);
        }

        [Fact]
        public static void GetEnumerator_MoveNextIndex_Invalid()
        {
            CredentialCache cc = new CredentialCache();
            IEnumerator enumerator = cc.GetEnumerator();
            enumerator.MoveNext();

            Assert.Throws<InvalidOperationException>(() => enumerator.Current);
        }

        [Fact]
        public static void DefaultCredentials_Get_Success()
        {
            NetworkCredential c = CredentialCache.DefaultCredentials as NetworkCredential;
            Assert.NotNull(c);

            Assert.Equal(String.Empty, c.UserName);
            Assert.Equal(String.Empty, c.Password);
            Assert.Equal(String.Empty, c.Domain);
        }

        [Fact]
        [SkipOnTargetFramework(TargetFrameworkMonikers.Netcoreapp | TargetFrameworkMonikers.Uap)]
        public static void AddRemove_UriAuthenticationTypeDefaultCredentials_Success_net46()
        {
            NetworkCredential nc = CredentialCache.DefaultNetworkCredentials as NetworkCredential;
            CredentialCache cc = new CredentialCache();

            // The full framework throw System.ArgumentException with the message
            // 'Default credentials cannot be supplied for the authenticationType1 authentication scheme.'
            Assert.Throws<ArgumentException>("authType", () => cc.Add(uriPrefix1, authenticationType1, nc));
        }

        [Fact]
        [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework)]
        public static void AddRemove_UriAuthenticationTypeDefaultCredentials_Success()
        {
            NetworkCredential nc = CredentialCache.DefaultNetworkCredentials as NetworkCredential;

            CredentialCache cc = new CredentialCache();
            cc.Add(uriPrefix1, authenticationType1, nc);

            Assert.Equal(nc, cc.GetCredential(uriPrefix1, authenticationType1));

            cc.Remove(uriPrefix1, authenticationType1);
            Assert.Null(cc.GetCredential(uriPrefix1, authenticationType1));
        }

        [Fact]
        [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework)]
        public static void AddRemove_HostPortAuthenticationTypeDefaultCredentials_Success()
        {
            NetworkCredential nc = CredentialCache.DefaultNetworkCredentials as NetworkCredential;

            CredentialCache cc = new CredentialCache();
            cc.Add(host1, port1, authenticationType1, nc);

            Assert.Equal(nc, cc.GetCredential(host1, port1, authenticationType1));

            cc.Remove(host1, port1, authenticationType1);
            Assert.Null(cc.GetCredential(host1, port1, authenticationType1));
        }

        [Fact]
        public static void DefaultNetworkCredentials_Get_Success()
        {
            NetworkCredential nc = CredentialCache.DefaultNetworkCredentials as NetworkCredential;
            Assert.NotNull(nc);

            Assert.Equal(String.Empty, nc.UserName);
            Assert.Equal(String.Empty, nc.Password);
            Assert.Equal(String.Empty, nc.Domain);
        }
    }
}
