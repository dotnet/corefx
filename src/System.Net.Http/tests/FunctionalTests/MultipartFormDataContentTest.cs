﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Threading.Tasks;

using Xunit;

namespace System.Net.Http.Functional.Tests
{
    public class MultipartFormDataContentTest
    {
        [Fact]
        public void Ctor_NoParams_CorrectMediaType()
        {
            var content = new MultipartFormDataContent();
            Assert.Equal("multipart/form-data", content.Headers.ContentType.MediaType);
            Assert.Equal(1, content.Headers.ContentType.Parameters.Count);
        }

        [Fact]
        public void Ctor_NullBoundary_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new MultipartFormDataContent(null));
        }

        [Fact]
        public void Ctor_EmptyBoundary_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new MultipartFormDataContent(String.Empty));
        }

        [Fact]
        public void Add_NullContent_ThrowsArgumentNullException()
        {
            var content = new MultipartFormDataContent();
            Assert.Throws<ArgumentNullException>(() => content.Add(null));
        }

        [Fact]
        public void Add_NullName_ThrowsArgumentException()
        {
            var content = new MultipartFormDataContent();
            Assert.Throws<ArgumentException>(() => content.Add(new StringContent("Hello world"), null));
        }

        [Fact]
        public void Add_EmptyName_ThrowsArgumentException()
        {
            var content = new MultipartFormDataContent();
            Assert.Throws<ArgumentException>(() => content.Add(new StringContent("Hello world"), String.Empty));
        }

        [Fact]
        public void Add_NullFileName_ThrowsArgumentException()
        {
            var content = new MultipartFormDataContent();
            Assert.Throws<ArgumentException>(() => content.Add(new StringContent("Hello world"), "name", null));
        }

        [Fact]
        public void Add_EmptyFileName_ThrowsArgumentException()
        {
            var content = new MultipartFormDataContent();
            Assert.Throws<ArgumentException>(() => content.Add(new StringContent("Hello world"), "name", String.Empty));
        }

        [Fact]
        public async Task Serialize_EmptyList_Success()
        {
            var content = new MultipartFormDataContent("test_boundary");
            var output = new MemoryStream();
            await content.CopyToAsync(output);

            output.Seek(0, SeekOrigin.Begin);
            string result = new StreamReader(output).ReadToEnd();

            Assert.Equal("--test_boundary\r\n\r\n--test_boundary--\r\n", result);
        }

        [Fact]
        public async Task Serialize_StringContent_Success()
        {
            var content = new MultipartFormDataContent("test_boundary");
            content.Add(new StringContent("Hello World"));

            var output = new MemoryStream();
            await content.CopyToAsync(output);

            output.Seek(0, SeekOrigin.Begin);
            string result = new StreamReader(output).ReadToEnd();

            Assert.Equal(
                "--test_boundary\r\nContent-Type: text/plain; charset=utf-8\r\n"
                + "Content-Disposition: form-data\r\n\r\nHello World\r\n--test_boundary--\r\n",
                result);
        }

        [Fact]
        public async Task Serialize_NamedStringContent_Success()
        {
            var content = new MultipartFormDataContent("test_boundary");
            content.Add(new StringContent("Hello World"), "test_name");

            var output = new MemoryStream();
            await content.CopyToAsync(output);

            output.Seek(0, SeekOrigin.Begin);
            string result = new StreamReader(output).ReadToEnd();

            Assert.Equal(
                "--test_boundary\r\nContent-Type: text/plain; charset=utf-8\r\n"
                + "Content-Disposition: form-data; name=test_name\r\n\r\nHello World\r\n--test_boundary--\r\n",
                result);
        }

        [Fact]
        public async Task Serialize_FileNameStringContent_Success()
        {
            var content = new MultipartFormDataContent("test_boundary");
            content.Add(new StringContent("Hello World"), "test_name", "test_file_name");

            var output = new MemoryStream();
            await content.CopyToAsync(output);

            output.Seek(0, SeekOrigin.Begin);
            string result = new StreamReader(output).ReadToEnd();

            Assert.Equal(
                "--test_boundary\r\nContent-Type: text/plain; charset=utf-8\r\n"
                + "Content-Disposition: form-data; name=test_name; " 
                + "filename=test_file_name; filename*=utf-8\'\'test_file_name\r\n\r\n"
                + "Hello World\r\n--test_boundary--\r\n",
                result);
        }

        [Fact]
        public async Task Serialize_QuotedName_Success()
        {
            var content = new MultipartFormDataContent("test_boundary");
            content.Add(new StringContent("Hello World"), "\"test name\"");

            var output = new MemoryStream();
            await content.CopyToAsync(output);

            output.Seek(0, SeekOrigin.Begin);
            string result = new StreamReader(output).ReadToEnd();

            Assert.Equal(
                "--test_boundary\r\nContent-Type: text/plain; charset=utf-8\r\n"
                + "Content-Disposition: form-data; name=\"test name\"\r\n\r\nHello World\r\n--test_boundary--\r\n",
                result);
        }

        [Fact]
        public async Task Serialize_InvalidName_Encoded()
        {
            var content = new MultipartFormDataContent("test_boundary");
            content.Add(new StringContent("Hello World"), "testク\r\n namé");

            MemoryStream output = new MemoryStream();
            await content.CopyToAsync(output);

            output.Seek(0, SeekOrigin.Begin);
            string result = new StreamReader(output).ReadToEnd();

            Assert.Equal(
                "--test_boundary\r\nContent-Type: text/plain; charset=utf-8\r\n"
                + "Content-Disposition: form-data; name=\"=?utf-8?B?dGVzdOOCrw0KIG5hbcOp?=\""
                + "\r\n\r\nHello World\r\n--test_boundary--\r\n",
                result);
        }

        [Fact]
        public async Task Serialize_InvalidQuotedName_Encoded()
        {
            var content = new MultipartFormDataContent("test_boundary");
            content.Add(new StringContent("Hello World"), "\"testク\r\n namé\"");

            var output = new MemoryStream();
            await content.CopyToAsync(output);

            output.Seek(0, SeekOrigin.Begin);
            string result = new StreamReader(output).ReadToEnd();

            Assert.Equal(
                "--test_boundary\r\nContent-Type: text/plain; charset=utf-8\r\n"
                + "Content-Disposition: form-data; name=\"=?utf-8?B?dGVzdOOCrw0KIG5hbcOp?=\""
                + "\r\n\r\nHello World\r\n--test_boundary--\r\n",
                result);
        }

        [Fact]
        public async Task Serialize_InvalidNamedFileName_Encoded()
        {
            var content = new MultipartFormDataContent("test_boundary");
            content.Add(new StringContent("Hello World"), "testク\r\n namé", "fileク\r\n namé");

            MemoryStream output = new MemoryStream();
            await content.CopyToAsync(output);

            output.Seek(0, SeekOrigin.Begin);
            string result = new StreamReader(output).ReadToEnd();

            Assert.Equal(
                "--test_boundary\r\nContent-Type: text/plain; charset=utf-8\r\n"
                + "Content-Disposition: form-data; name=\"=?utf-8?B?dGVzdOOCrw0KIG5hbcOp?=\";"
                + " filename=\"=?utf-8?B?ZmlsZeOCrw0KIG5hbcOp?=\"; filename*=utf-8\'\'file%E3%82%AF%0D%0A%20nam%C3%A9" 
                + "\r\n\r\nHello World\r\n--test_boundary--\r\n",
                result);
        }
    }
}
