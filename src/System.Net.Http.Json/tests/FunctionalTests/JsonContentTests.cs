﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Net.Http.Headers;
using System.Net.Test.Common;
using System.Threading.Tasks;
using Xunit;

namespace System.Net.Http.Json.Functional.Tests
{
    public class JsonContentTests
    {

        private class Foo { }
        private class Bar { }

        [Fact]
        public void JsonContentObjectType()
        {
            Type fooType = typeof(Foo);
            Foo foo = new Foo();

            JsonContent content = JsonContent.Create(foo, fooType, null);
            Assert.Equal(fooType, content.ObjectType);
            Assert.Same(foo, content.Value);

            content = JsonContent.Create(foo, null);
            Assert.Equal(fooType, content.ObjectType);
            Assert.Same(foo, content.Value);

            object fooBoxed = foo;

            // ObjectType is the specified type when using the .ctor.
            content = JsonContent.Create(fooBoxed, fooType, null);
            Assert.Equal(fooType, content.ObjectType);
            Assert.Same(fooBoxed, content.Value);

            // ObjectType is the declared type when using the factory method.
            content = JsonContent.Create(fooBoxed, null);
            Assert.Equal(typeof(object), content.ObjectType);
            Assert.Same(fooBoxed, content.Value);
        }

        [Fact]
        public void TestJsonContentMediaType()
        {
            Type fooType = typeof(Foo);
            Foo foo = new Foo();

            // Use the default content-type if none is provided.
            JsonContent content = JsonContent.Create(foo, fooType, null);
            Assert.Equal("application/json", content.Headers.ContentType.MediaType);
            Assert.Equal("utf-8", content.Headers.ContentType.CharSet);

            content = JsonContent.Create(foo, null);
            Assert.Equal("application/json", content.Headers.ContentType.MediaType);
            Assert.Equal("utf-8", content.Headers.ContentType.CharSet);

            // Use the specified MediaTypeHeaderValue if provided.
            MediaTypeHeaderValue mediaType = MediaTypeHeaderValue.Parse("foo/bar; charset=utf-8");
            content = JsonContent.Create(foo, fooType, mediaType);
            Assert.Same(mediaType, content.Headers.ContentType);

            content = JsonContent.Create(foo, mediaType: mediaType);
            Assert.Same(mediaType, content.Headers.ContentType);
        }

        [Fact]
        public async Task SendQuotedCharsetAsync()
        {
            JsonContent content = JsonContent.Create<Foo>(null, null);
            content.Headers.ContentType.CharSet = "\"utf-8\"";

            HttpClient client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "http://example.com");
            request.Content = content;
            await client.SendAsync(request);
        }

        [Fact]
        public void TestJsonContentContentTypeIsNotTheSameOnMultipleInstances()
        {
            JsonContent jsonContent1 = JsonContent.Create<object>(null, null);
            JsonContent jsonContent2 = JsonContent.Create<object>(null, null);

            jsonContent1.Headers.ContentType.CharSet = "foo-bar";

            Assert.NotEqual(jsonContent1.Headers.ContentType.CharSet, jsonContent2.Headers.ContentType.CharSet);
            Assert.NotSame(jsonContent1.Headers.ContentType, jsonContent2.Headers.ContentType);
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
        public void JsonContentMediaTypeDefaultIfNull()
        {
            Type fooType = typeof(Foo);
            Foo foo = null;

            JsonContent content = JsonContent.Create(foo, fooType, mediaType: null);
            Assert.Equal("application/json", content.Headers.ContentType.MediaType);
            Assert.Equal("utf-8", content.Headers.ContentType.CharSet);

            content = JsonContent.Create(foo, mediaType: null);
            Assert.Equal("application/json", content.Headers.ContentType.MediaType);
            Assert.Equal("utf-8", content.Headers.ContentType.CharSet);
        }

        [Fact]
        public void JsonContentInputTypeIsNull()
        {
            string foo = "test";

            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(() => JsonContent.Create(foo, inputType: null, mediaType: null));
            Assert.Equal("inputType", ex.ParamName);

            ex = Assert.Throws<ArgumentNullException>(() => JsonContent.Create(foo, inputType: null, mediaType: null));
            Assert.Equal("inputType", ex.ParamName);
        }

        [Fact(Skip = "Should we throw when !inputType,IsAssignableFrom(inputValue.GetType()) on instantiation or let the JsonSerializer throw later?")]
        public async Task JsonContentThrowsOnIncompatibleTypeAsync()
        {
            HttpClient client = new HttpClient();
            var foo = new Foo();
            Type typeOfBar = typeof(Bar);

            var request = new HttpRequestMessage(HttpMethod.Post, "http://example.com");
            request.Content = JsonContent.Create(foo, typeOfBar, null);
            await Assert.ThrowsAsync<ArgumentException>(() => client.SendAsync(request));

            request = new HttpRequestMessage(HttpMethod.Post, "http://example.com");
            request.Content = JsonContent.Create(foo, typeOfBar, MediaTypeHeaderValue.Parse("application/json; charset=utf-8"));
            await Assert.ThrowsAsync<ArgumentException>(() => client.SendAsync(request));
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
                        request.Content = JsonContent.Create(Person.Create(), mediaType: mediaType);
                        await client.SendAsync(request);
                    }
                },
                async server => {
                    HttpRequestData req = await server.HandleRequestAsync();
                    Assert.Equal("application/json; charset=utf-16", req.GetSingleHeaderValue("Content-Type"));
                });
        }

        [Fact]
        public void EnsureDefaultJsonSerializerOptions()
        {
            HttpClient client = new HttpClient();
            EnsureDefaultOptions obj = new EnsureDefaultOptions();

            var request = new HttpRequestMessage(HttpMethod.Post, "http://example.com");
            request.Content = JsonContent.Create(obj, mediaType: null);
            client.SendAsync(request);
        }
    }
}
