// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace System.Net.Http.Functional.Tests
{
    using Configuration = System.Net.Test.Common.Configuration;

    // Note:  Disposing the HttpClient object automatically disposes the handler within. So, it is not necessary
    // to separately Dispose (or have a 'using' statement) for the handler.
    [SkipOnTargetFramework(TargetFrameworkMonikers.Uap, "dotnet/corefx #20010")]
    public abstract class PostScenarioTest : HttpClientTestBase
    {
        private const string ExpectedContent = "Test contest";
        private const string UserName = "user1";
        private const string Password = "password1";
        private static readonly Uri BasicAuthServerUri =
            Configuration.Http.BasicAuthUriForCreds(false, UserName, Password);
        private static readonly Uri SecureBasicAuthServerUri =
            Configuration.Http.BasicAuthUriForCreds(true, UserName, Password);

        private readonly ITestOutputHelper _output;

        public static readonly object[][] EchoServers = Configuration.Http.EchoServers;

        public static readonly object[][] BasicAuthEchoServers =
            new object[][]
                {
                    new object[] { BasicAuthServerUri },
                    new object[] { SecureBasicAuthServerUri }
                };

        public PostScenarioTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [OuterLoop] // TODO: Issue #11345
        [Theory, MemberData(nameof(EchoServers))]
        public async Task PostNoContentUsingContentLengthSemantics_Success(Uri serverUri)
        {
            await PostHelper(serverUri, string.Empty, null,
                useContentLengthUpload: true, useChunkedEncodingUpload: false);
        }

        [OuterLoop] // TODO: Issue #11345
        [Theory, MemberData(nameof(EchoServers))]
        public async Task PostEmptyContentUsingContentLengthSemantics_Success(Uri serverUri)
        {
            await PostHelper(serverUri, string.Empty, new StringContent(string.Empty),
                useContentLengthUpload: true, useChunkedEncodingUpload: false);
        }

        [OuterLoop] // TODO: Issue #11345
        [Theory, MemberData(nameof(EchoServers))]
        public async Task PostEmptyContentUsingChunkedEncoding_Success(Uri serverUri)
        {
            await PostHelper(serverUri, string.Empty, new StringContent(string.Empty),
                useContentLengthUpload: false, useChunkedEncodingUpload: true);
        }

        [OuterLoop] // TODO: Issue #11345
        [Theory, MemberData(nameof(EchoServers))]
        public async Task PostEmptyContentUsingConflictingSemantics_Success(Uri serverUri)
        {
            await PostHelper(serverUri, string.Empty, new StringContent(string.Empty),
                useContentLengthUpload: true, useChunkedEncodingUpload: true);
        }

        [OuterLoop] // TODO: Issue #11345
        [Theory, MemberData(nameof(EchoServers))]
        public async Task PostUsingContentLengthSemantics_Success(Uri serverUri)
        {
            await PostHelper(serverUri, ExpectedContent, new StringContent(ExpectedContent),
                useContentLengthUpload: true, useChunkedEncodingUpload: false);
        }

        [OuterLoop] // TODO: Issue #11345
        [Theory, MemberData(nameof(EchoServers))]
        public async Task PostUsingChunkedEncoding_Success(Uri serverUri)
        {
            await PostHelper(serverUri, ExpectedContent, new StringContent(ExpectedContent),
                useContentLengthUpload: false, useChunkedEncodingUpload: true);
        }

        [OuterLoop] // TODO: Issue #11345
        [Theory, MemberData(nameof(EchoServers))]
        public async Task PostSyncBlockingContentUsingChunkedEncoding_Success(Uri serverUri)
        {
            await PostHelper(serverUri, ExpectedContent, new SyncBlockingContent(ExpectedContent),
                useContentLengthUpload: false, useChunkedEncodingUpload: true);
        }

        [OuterLoop] // TODO: Issue #11345
        [Theory, MemberData(nameof(EchoServers))]
        public async Task PostRepeatedFlushContentUsingChunkedEncoding_Success(Uri serverUri)
        {
            await PostHelper(serverUri, ExpectedContent, new RepeatedFlushContent(ExpectedContent),
                useContentLengthUpload: false, useChunkedEncodingUpload: true);
        }

        [OuterLoop] // TODO: Issue #11345
        [Theory, MemberData(nameof(EchoServers))]
        public async Task PostUsingUsingConflictingSemantics_UsesChunkedSemantics(Uri serverUri)
        {
            await PostHelper(serverUri, ExpectedContent, new StringContent(ExpectedContent),
                useContentLengthUpload: true, useChunkedEncodingUpload: true);
        }

        [OuterLoop] // TODO: Issue #11345
        [Theory, MemberData(nameof(EchoServers))]
        [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework, "netfx behaves differently and will buffer content and use 'Content-Length' semantics")]
        public async Task PostUsingNoSpecifiedSemantics_UsesChunkedSemantics(Uri serverUri)
        {
            await PostHelper(serverUri, ExpectedContent, new StringContent(ExpectedContent),
                useContentLengthUpload: false, useChunkedEncodingUpload: false);
        }

        [OuterLoop] // TODO: Issue #11345
        [Theory]
        [InlineData(5 * 1024)]
        [InlineData(63 * 1024)]
        public async Task PostLongerContentLengths_UsesChunkedSemantics(int contentLength)
        {
            var rand = new Random(42);
            var sb = new StringBuilder(contentLength);
            for (int i = 0; i < contentLength; i++)
            {
                sb.Append((char)(rand.Next(0, 26) + 'a'));
            }
            string content = sb.ToString();

            await PostHelper(Configuration.Http.RemoteEchoServer, content, new StringContent(content),
                useContentLengthUpload: true, useChunkedEncodingUpload: false);
        }

        [OuterLoop] // TODO: Issue #11345
        [Theory, MemberData(nameof(BasicAuthEchoServers))]
        public async Task PostRewindableContentUsingAuth_NoPreAuthenticate_Success(Uri serverUri)
        {
            HttpContent content = CustomContent.Create(ExpectedContent, true);
            var credential = new NetworkCredential(UserName, Password);
            await PostUsingAuthHelper(serverUri, ExpectedContent, content, credential, false);
        }

        [OuterLoop] // TODO: Issue #11345
        [Theory, MemberData(nameof(BasicAuthEchoServers))]
        public async Task PostNonRewindableContentUsingAuth_NoPreAuthenticate_ThrowsHttpRequestException(Uri serverUri)
        {
            HttpContent content = CustomContent.Create(ExpectedContent, false);
            var credential = new NetworkCredential(UserName, Password);
            await Assert.ThrowsAsync<HttpRequestException>(() => 
                PostUsingAuthHelper(serverUri, ExpectedContent, content, credential, preAuthenticate: false));
        }

        [OuterLoop] // TODO: Issue #11345
        [Theory, MemberData(nameof(BasicAuthEchoServers))]
        public async Task PostNonRewindableContentUsingAuth_PreAuthenticate_Success(Uri serverUri)
        {
            if (IsWinHttpHandler)
            {
                // Issue #9228
                return;
            }

            HttpContent content = CustomContent.Create(ExpectedContent, false);
            var credential = new NetworkCredential(UserName, Password);
            await PostUsingAuthHelper(serverUri, ExpectedContent, content, credential, preAuthenticate: true);
        }

        [OuterLoop] // TODO: Issue #11345
        [Theory, MemberData(nameof(EchoServers))]
        public async Task PostAsync_EmptyContent_ContentTypeHeaderNotSent(Uri serverUri)
        {
            using (HttpClient client = CreateHttpClient())
            using (HttpResponseMessage response = await client.PostAsync(serverUri, null))
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                bool sentContentType = TestHelper.JsonMessageContainsKey(responseContent, "Content-Type");

                Assert.False(sentContentType);
            }
        }

        private async Task PostHelper(
            Uri serverUri,
            string requestBody,
            HttpContent requestContent,
            bool useContentLengthUpload,
            bool useChunkedEncodingUpload)
        {
            using (HttpClient client = CreateHttpClient())
            {
                if (requestContent != null)
                {
                    if (useContentLengthUpload)
                    {
                        // Ensure that Content-Length is populated (see issue #27245)
                        requestContent.Headers.ContentLength = requestContent.Headers.ContentLength;
                    }
                    else
                    {
                        requestContent.Headers.ContentLength = null;
                    }
                }
                
                if (useChunkedEncodingUpload)
                {
                    client.DefaultRequestHeaders.TransferEncodingChunked = true;
                }

                using (HttpResponseMessage response = await client.PostAsync(serverUri, requestContent))
                {
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                    string responseContent = await response.Content.ReadAsStringAsync();
                    _output.WriteLine(responseContent);

                    if (!useContentLengthUpload && !useChunkedEncodingUpload)
                    {
                        useChunkedEncodingUpload = true;
                    }

                    TestHelper.VerifyResponseBody(
                        responseContent,
                        response.Content.Headers.ContentMD5,
                        useChunkedEncodingUpload,
                        requestBody);
                }
            }          
        }

        private async Task PostUsingAuthHelper(
            Uri serverUri,
            string requestBody,
            HttpContent requestContent,
            NetworkCredential credential,
            bool preAuthenticate)
        {
            HttpClientHandler handler = CreateHttpClientHandler();
            handler.PreAuthenticate = preAuthenticate;
            handler.Credentials = credential;
            using (var client = new HttpClient(handler))
            {
                // Send HEAD request to help bypass the 401 auth challenge for the latter POST assuming
                // that the authentication will be cached and re-used later when PreAuthenticate is true.
                var request = new HttpRequestMessage(HttpMethod.Head, serverUri);
                using (HttpResponseMessage response = await client.SendAsync(request))
                {
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                }

                // Now send POST request.
                request = new HttpRequestMessage(HttpMethod.Post, serverUri);
                request.Content = requestContent;
                requestContent.Headers.ContentLength = null;
                request.Headers.TransferEncodingChunked = true;

                using (HttpResponseMessage response = await client.SendAsync(request))
                {
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                    string responseContent = await response.Content.ReadAsStringAsync();
                    _output.WriteLine(responseContent);

                    TestHelper.VerifyResponseBody(
                        responseContent,
                        response.Content.Headers.ContentMD5,
                        true,
                        requestBody);
                }
            }
        }
    }
}
