// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Net.Http.Headers;
using System.Net.Test.Common;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace System.Net.Http.Json.Functional.Tests
{
    public partial class JsonContentTests
    {
        [Fact]
        public async Task SendQuotedCharsetAsync()
        {

            await LoopbackServer.CreateClientAndServerAsync(
                async uri =>
                {
                    using (HttpClient client = new HttpClient())
                    {
                        JsonContent content = JsonContent.Create<Foo>(null);
                        content.Headers.ContentType.CharSet = "\"utf-8\"";

                        var request = new HttpRequestMessage(HttpMethod.Post, uri);
                        request.Content = content;
                        await client.SendAsync(request);
                    }
                },
                async server => {
                    HttpRequestData req = await server.HandleRequestAsync();
                    Assert.Equal("application/json; charset=\"utf-8\"", req.GetSingleHeaderValue("Content-Type"));
                });
        }

        [Fact]
        public async Task JsonContentMediaTypeValidateOnServerAsync()
        {
            await LoopbackServer.CreateClientAndServerAsync(
                async uri =>
                {
                    using (HttpClient client = new HttpClient())
                    {
                        var request = new HttpRequestMessage(HttpMethod.Post, uri);
                        MediaTypeHeaderValue mediaType = MediaTypeHeaderValue.Parse("foo/bar; charset=utf-8");
                        request.Content = JsonContent.Create(Person.Create(), mediaType: mediaType);
                        await client.SendAsync(request);
                    }
                },
                async server => {
                    HttpRequestData req = await server.HandleRequestAsync();
                    Assert.Equal("foo/bar; charset=utf-8", req.GetSingleHeaderValue("Content-Type"));
                });
        }

        [Fact]
        public static async Task ValidateUtf16IsTranscodedAsync()
        {
            await LoopbackServer.CreateClientAndServerAsync(
                async uri =>
                {
                    using (HttpClient client = new HttpClient())
                    {
                        var request = new HttpRequestMessage(HttpMethod.Post, uri);
                        MediaTypeHeaderValue mediaType = MediaTypeHeaderValue.Parse("application/json; charset=utf-16");
                        // Pass new options to avoid using the Default Web Options that use camelCase.
                        request.Content = JsonContent.Create(Person.Create(), mediaType: mediaType, options: new JsonSerializerOptions());
                        await client.SendAsync(request);
                    }
                },
                async server => {
                    HttpRequestData req = await server.HandleRequestAsync();
                    Assert.Equal("application/json; charset=utf-16", req.GetSingleHeaderValue("Content-Type"));
                    Person per = JsonSerializer.Deserialize<Person>(Encoding.Unicode.GetString(req.Body));
                    per.Validate();
                });
        }

        [Fact]
        public async Task EnsureDefaultJsonSerializerOptionsAsync()
        {
            await LoopbackServer.CreateClientAndServerAsync(
                async uri =>
                {
                    using (HttpClient client = new HttpClient())
                    {
                        // EnsureDefaultOptions uses a JsonConverter where we validate the JsonSerializerOptions when not provided to JsonContent.Create.
                        EnsureDefaultOptions dummyObj = new EnsureDefaultOptions();
                        var request = new HttpRequestMessage(HttpMethod.Post, uri);
                        request.Content = JsonContent.Create(dummyObj);
                        await client.SendAsync(request);
                    }
                },
                server => server.HandleRequestAsync());
        }

        [Fact]
        public async Task TestJsonContentNullContentTypeAsync()
        {
            await LoopbackServer.CreateClientAndServerAsync(
                async uri =>
                {
                    using (HttpClient client = new HttpClient())
                    {
                        var request = new HttpRequestMessage(HttpMethod.Post, uri);
                        MediaTypeHeaderValue mediaType = MediaTypeHeaderValue.Parse("application/json; charset=utf-16");
                        JsonContent content = JsonContent.Create(Person.Create(), mediaType: mediaType);
                        content.Headers.ContentType = null;

                        request.Content = content;
                        await client.SendAsync(request);
                    }
                },
                async server => {
                    HttpRequestData req = await server.HandleRequestAsync();
                    Assert.Equal(0, req.GetHeaderValueCount("Content-Type"));
                });
        }
    }
}
