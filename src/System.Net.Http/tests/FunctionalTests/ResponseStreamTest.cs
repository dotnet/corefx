// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Net.Tests;
using System.Threading.Tasks;

using Xunit;
using Xunit.Abstractions;

namespace System.Net.Http.Functional.Tests
{
    public class ResponseStreamTest
    {
        private readonly ITestOutputHelper _output;
        
        public ResponseStreamTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task GetStreamAsync_ReadToEnd_Success()
        {
            HttpClient client = GetHttpClient();
            
            Stream stream = await client.GetStreamAsync(HttpTestServers.RemoteGetServer);
            using (var reader = new StreamReader(stream))
            {
                string responseBody = reader.ReadToEnd();
                _output.WriteLine(responseBody);
                Assert.True(IsValidResponseBody(responseBody));
            }
        }

        [Fact]
        public async Task GetAsync_UseResponseHeadersReadAndCallLoadIntoBuffer_Success()
        {
            HttpClient client = GetHttpClient();
            
            HttpResponseMessage response =
                await client.GetAsync(HttpTestServers.RemoteGetServer, HttpCompletionOption.ResponseHeadersRead);
            await response.Content.LoadIntoBufferAsync();

            string responseBody = await response.Content.ReadAsStringAsync();
            _output.WriteLine(responseBody);
            Assert.True(IsValidResponseBody(responseBody));
        }

        [Fact]
        public async Task GetAsync_UseResponseHeadersReadAndCopyToMemoryStream_Success()
        {
            HttpClient client = GetHttpClient();
            
            HttpResponseMessage response =
                await client.GetAsync(HttpTestServers.RemoteGetServer, HttpCompletionOption.ResponseHeadersRead);

            var memoryStream = new MemoryStream();
            await response.Content.CopyToAsync(memoryStream);
            memoryStream.Position = 0;
            
            using (var reader = new StreamReader(memoryStream))
            {
                string responseBody = reader.ReadToEnd();
                _output.WriteLine(responseBody);
                Assert.True(IsValidResponseBody(responseBody));
            }
        }

        // These methods help to validate the response body since the endpoint will echo
        // all request headers.
        //
        // TODO: This validation will be improved in the future once the test server endpoint
        // is able to provide a custom response header with a SHA1 hash of the expected response body.
        private readonly string[] CustomHeaderValues = new string[] {"abcdefg", "12345678", "A1B2C3D4"};
        private HttpClient GetHttpClient()
        {
            var client = new HttpClient();
            int ndx = 1;
            foreach (string headerValue in CustomHeaderValues)
            {
                client.DefaultRequestHeaders.Add("X-Custom" + ndx.ToString(), headerValue);
                ndx++;
            }

            return client;
        }

        private bool IsValidResponseBody(string responseBody)
        {
            foreach (string headerValue in CustomHeaderValues)
            {
                if (!responseBody.Contains(headerValue))
                {
                    return false;
                }
            }            

            return true;
        }
    }
}
