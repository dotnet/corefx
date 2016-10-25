// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Net.Http;
using System.Net.Test.Common;
using System.Threading.Tasks;

using Xunit;
using Xunit.Abstractions;

namespace System.Net.Tests
{
    public class HttpWebResponseHeaderTest
    {
        [OuterLoop]
        [Fact]
        public async Task HttpHeader_Set_Success()
        {
            await LoopbackServer.CreateServerAsync(async (server, url) =>
            {
                HttpWebRequest request = WebRequest.CreateHttp(url);
                request.Method = HttpMethod.Get.Method;
                Task<WebResponse> getResponse = request.GetResponseAsync();
                DateTimeOffset utcNow = DateTimeOffset.UtcNow;
                await LoopbackServer.ReadRequestAndSendResponseAsync(server,
                        $"HTTP/1.1 200 OK\r\n" +
                        $"Date: {utcNow:R}\r\n" +
                        "Content-Type: application/json;charset=UTF-8\r\n" +
                        "Content-Length: 5\r\n" +
                        "\r\n" +
                        "12345");

                using (WebResponse response = await getResponse)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)response;
                    Assert.Equal("UTF-8", httpResponse.CharacterSet);
                    Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
                    Assert.Equal("OK", httpResponse.StatusDescription);
                    Assert.Equal(new CookieCollection(), httpResponse.Cookies);
                    Assert.Equal(5,httpResponse.ContentLength);
                    Assert.Equal(5, int.Parse(httpResponse.GetResponseHeader("Content-Length")));
                }
            });
        }
    }
}
