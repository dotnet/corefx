﻿// Licensed to the .NET Foundation under one or more agreements.
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

        private class Foo { }
        private class Bar { }

        [Fact]
        public void JsonContentObjectType()
        {
            Type fooType = typeof(Foo);
            Foo foo = new Foo();

            JsonContent content = JsonContent.Create(foo, fooType);
            Assert.Equal(fooType, content.ObjectType);
            Assert.Same(foo, content.Value);

            content = JsonContent.Create(foo);
            Assert.Equal(fooType, content.ObjectType);
            Assert.Same(foo, content.Value);

            object fooBoxed = foo;

            // ObjectType is the specified type when using the .ctor.
            content = JsonContent.Create(fooBoxed, fooType);
            Assert.Equal(fooType, content.ObjectType);
            Assert.Same(fooBoxed, content.Value);

            // ObjectType is the declared type when using the factory method.
            content = JsonContent.Create(fooBoxed);
            Assert.Equal(typeof(object), content.ObjectType);
            Assert.Same(fooBoxed, content.Value);
        }

        [Fact]
        public void TestJsonContentMediaType()
        {
            Type fooType = typeof(Foo);
            Foo foo = new Foo();

            // Use the default content-type if none is provided.
            JsonContent content = JsonContent.Create(foo, fooType);
            Assert.Equal("application/json", content.Headers.ContentType.MediaType);
            Assert.Equal("utf-8", content.Headers.ContentType.CharSet);

            content = JsonContent.Create(foo);
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
        public void TestJsonContentContentTypeIsNotTheSameOnMultipleInstances()
        {
            JsonContent jsonContent1 = JsonContent.Create<object>(null);
            JsonContent jsonContent2 = JsonContent.Create<object>(null);

            jsonContent1.Headers.ContentType.CharSet = "foo-bar";

            Assert.NotEqual(jsonContent1.Headers.ContentType.CharSet, jsonContent2.Headers.ContentType.CharSet);
            Assert.NotSame(jsonContent1.Headers.ContentType, jsonContent2.Headers.ContentType);
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
            => AssertExtensions.Throws<ArgumentNullException>("inputType", () => JsonContent.Create(null, inputType: null, mediaType: null));

        [Fact]
        public void JsonContentThrowsOnIncompatibleTypeAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                var foo = new Foo();
                Type typeOfBar = typeof(Bar);

                Exception ex = Assert.Throws<ArgumentException>(() => JsonContent.Create(foo, typeOfBar));

                string strTypeOfBar = typeOfBar.ToString();
                Assert.Contains(strTypeOfBar, ex.Message);

                string afterInputTypeMessage = ex.Message.Split(strTypeOfBar.ToCharArray())[1];
                Assert.Contains(afterInputTypeMessage, ex.Message);
            }
        }
    }
}
