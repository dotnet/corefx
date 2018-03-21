// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

using Xunit;

namespace System.Net.Http.Functional.Tests
{
    public abstract class HttpClient_SelectedSites_Test : HttpClientTestBase
    {
        private readonly static object s_httpClientLock = new object();
        private static HttpClient s_httpClient;

        [Theory]
        [OuterLoop]
        [Trait("SelectedSites", "true")]
        [MemberData(nameof(GetSelectedSites))]
        public async Task RetrieveSite_Succeeds(string site)
        {
            // Not doing this in bulk for platform handlers.
            if (!UseSocketsHttpHandler)
                return;

            await VisitSite(site);
        }

        [Theory]
        [OuterLoop]
        [Trait("SelectedSites", "true")]
        [MemberData(nameof(GetSelectedSites))]
        public async Task RetrieveSite_SharedHttpClient_Succeeds(string site)
        {
            // Not doing this in bulk for platform handlers.
            if (!UseSocketsHttpHandler)
                return;

            // Xunit creates an instance of the test class per test run so perform the double-lock to ensure that
            // all sites share the same httpClient. In principle there is no worry about not disposing it.
            // ATTENTION: This only works as intended because we are running this test only for SocketsHttpHandler.
            if (s_httpClient == null)
            {
                lock (s_httpClientLock)
                {
                    if (s_httpClient == null)
                        s_httpClient = CreateHttpClientForSiteVisit();
                }
            }

            await VisitSiteWithClient(site, s_httpClient);
        }

        [Theory]
        [OuterLoop]
        [Trait("SiteInvestigation", "true")]
        [InlineData("https://www.macys.com")]
        [InlineData("https://www.bestbuy.com")]

        public async Task RetrieveSite_Debug_Helper(string site)
        {
            await VisitSite(site);
        }

        public static IEnumerable<object[]> GetSelectedSites()
        {
            const string resourceName = "SelectedSitesTest.txt";
            Assembly assembly = typeof(HttpClient_SelectedSites_Test).Assembly;
            Stream s = assembly.GetManifestResourceStream(resourceName);
            if (s == null)
            {
                throw new Exception("Couldn't find resource " + resourceName);
            }

            using (var reader = new StreamReader(s))
            {
                string site;
                while (null != (site = reader.ReadLine()))
                {
                    yield return new[] { site };
                }
            }
        }

        private async Task VisitSite(string site)
        {
            using (HttpClient httpClient = CreateHttpClientForSiteVisit())
            {
                await VisitSiteWithClient(site, httpClient);
            }
        }

        private async Task VisitSiteWithClient(string site, HttpClient httpClient)
        {
            using (HttpResponseMessage response = await httpClient.GetAsync(site))
            {
                switch (response.StatusCode)
                {
                    case HttpStatusCode.Redirect:
                    case HttpStatusCode.OK:
                        if (response.Content.Headers.ContentLength > 0)
                            Assert.True((await response.Content.ReadAsByteArrayAsync()).Length > 0);
                        break;
                    case HttpStatusCode.BadGateway:
                    case HttpStatusCode.Forbidden:
                    case HttpStatusCode.Moved:
                    case HttpStatusCode.NotFound:
                    case HttpStatusCode.ServiceUnavailable:
                    case HttpStatusCode.Unauthorized:
                    case HttpStatusCode.InternalServerError:
                        break;
                    default:
                        throw new Exception($"{site} returned: {response.StatusCode}");
                }
            }
        }

        private HttpClient CreateHttpClientForSiteVisit()
        {
            HttpClient httpClient = new HttpClient(CreateHttpClientHandler(UseSocketsHttpHandler));

            // User-Agent added since some sites only give proper responses with it present.
            httpClient.DefaultRequestHeaders.Add(
                "User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/64.0.3282.186 Safari/537.36");

            return httpClient;
        }
    }
}
