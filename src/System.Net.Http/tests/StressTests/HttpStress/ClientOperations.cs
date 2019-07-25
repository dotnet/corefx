﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HttpStress
{
    /// <summary>Client context containing information pertaining to a single request.</summary>
    public sealed class RequestContext
    {
        private readonly Random _random;
        private readonly HttpClient _client;
        private readonly double _cancellationProbability;
        private readonly double _http2Probability;

        public RequestContext(HttpClient httpClient, Random random, int taskNum,
                                string contentSource, int maxRequestParameters, int maxRequestLineSize,
                                double cancellationProbability, double http2Probability)
        {
            _random = random;
            _client = httpClient;
            _cancellationProbability = cancellationProbability;
            _http2Probability = http2Probability;

            TaskNum = taskNum;
            IsCancellationRequested = false;
            MaxRequestParameters = maxRequestParameters;
            MaxRequestLineSize = maxRequestLineSize;
            ContentSource = contentSource;
        }

        public int TaskNum { get; }
        public bool IsCancellationRequested { get; set; }
        public string ContentSource { get; }
        public int MaxRequestParameters { get; }
        public int MaxRequestLineSize { get; }
        public int MaxContentLength => ContentSource.Length;

        // HttpClient.SendAsync() wrapper that wires randomized cancellation
        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption httpCompletion = HttpCompletionOption.ResponseContentRead, CancellationToken? token = null)
        {
            if (token != null)
            {
                // user-supplied cancellation token overrides random cancellation
                return await _client.SendAsync(request, httpCompletion, token.Value);
            }
            else if (GetRandomBoolean(_cancellationProbability))
            {
                // trigger a random cancellation
                using (var cts = new CancellationTokenSource())
                {
                    int delayMs = _random.Next(0, 2);
                    Task<HttpResponseMessage> task = _client.SendAsync(request, httpCompletion, cts.Token);
                    if (delayMs > 0)
                        await Task.Delay(delayMs);

                    cts.Cancel();
                    IsCancellationRequested = true;
                    return await task;
                }
            }
            else
            {
                // no cancellation
                return await _client.SendAsync(request, httpCompletion);
            }
        }

        public string GetRandomString(int maxLength)
        {
            int length = _random.Next(0, maxLength);
            var sb = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                sb.Append((char)(_random.Next(0, 26) + 'a'));
            }
            return sb.ToString();
        }

        public string GetRandomSubstring(string input)
        {
            int offset = _random.Next(0, input.Length);
            int length = _random.Next(0, input.Length - offset + 1);
            return input.Substring(offset, length);
        }

        public bool GetRandomBoolean(double probability = 0.5)
        {
            if (probability < 0 || probability > 1)
                throw new ArgumentOutOfRangeException(nameof(probability));

            return _random.NextDouble() < probability;
        }

        public int GetRandomInt32(int minValueInclusive, int maxValueExclusive) => _random.Next(minValueInclusive, maxValueExclusive);

        public Version GetRandomHttpVersion() => GetRandomBoolean(_http2Probability) ? new Version(2, 0) : new Version(1, 1);
    }

    public static class ClientOperations
    {
        // Set of operations that the client can select from to run.  Each item is a tuple of the operation's name
        // and the delegate to invoke for it, provided with the HttpClient instance on which to make the call and
        // returning asynchronously the retrieved response string from the server.  Individual operations can be
        // commented out from here to turn them off, or additional ones can be added.
        public static (string, Func<RequestContext, Task>)[] Operations =>
            new (string, Func<RequestContext, Task>)[] 
            {
                ("GET",
                async ctx =>
                {
                    Version httpVersion = ctx.GetRandomHttpVersion();
                    using (var req = new HttpRequestMessage(HttpMethod.Get, "/") { Version = httpVersion })
                    using (HttpResponseMessage m = await ctx.SendAsync(req))
                    {
                        ValidateResponse(m, httpVersion);
                        ValidateContent(ctx.ContentSource, await m.Content.ReadAsStringAsync());
                    }
                }),

                ("GET Partial",
                async ctx =>
                {
                    Version httpVersion = ctx.GetRandomHttpVersion();
                    using (var req = new HttpRequestMessage(HttpMethod.Get, "/slow") { Version = httpVersion })
                    using (HttpResponseMessage m = await ctx.SendAsync(req, HttpCompletionOption.ResponseHeadersRead))
                    {
                        ValidateResponse(m, httpVersion);
                        using (Stream s = await m.Content.ReadAsStreamAsync())
                        {
                            s.ReadByte(); // read single byte from response and throw the rest away
                        }
                    }
                }),

                ("GET Headers",
                async ctx =>
                {
                    Version httpVersion = ctx.GetRandomHttpVersion();
                    using (var req = new HttpRequestMessage(HttpMethod.Get, "/headers") { Version = httpVersion })
                    using (HttpResponseMessage m = await ctx.SendAsync(req))
                    {
                        ValidateResponse(m, httpVersion);
                        ValidateContent(ctx.ContentSource, await m.Content.ReadAsStringAsync());
                    }
                }),

                ("GET Parameters",
                async ctx =>
                {
                    Version httpVersion = ctx.GetRandomHttpVersion();
                    string uri = "/variables";
                    string expectedResponse = GetGetQueryParameters(ref uri, ctx.MaxRequestLineSize, ctx.ContentSource, ctx, ctx.MaxRequestParameters);
                    using (var req = new HttpRequestMessage(HttpMethod.Get, uri) { Version = httpVersion })
                    using (HttpResponseMessage m = await ctx.SendAsync(req))
                    {
                        ValidateResponse(m, httpVersion);
                        ValidateContent(expectedResponse, await m.Content.ReadAsStringAsync(), $"Uri: {uri}");
                    }
                }),

                ("GET Aborted",
                async ctx =>
                {
                    Version httpVersion = ctx.GetRandomHttpVersion();
                    try
                    {
                        using (var req = new HttpRequestMessage(HttpMethod.Get, "/abort") { Version = httpVersion })
                        {
                            await ctx.SendAsync(req);
                        }
                        throw new Exception("Completed unexpectedly");
                    }
                    catch (Exception e)
                    {
                        if (e is HttpRequestException hre && hre.InnerException is IOException)
                        {
                            e = hre.InnerException;
                        }

                        if (e is IOException ioe)
                        {
                            if (httpVersion < HttpVersion.Version20)
                            {
                                return;
                            }

                            string name = e.InnerException?.GetType().Name;
                            switch (name)
                            {
                                case "Http2ProtocolException":
                                case "Http2ConnectionException":
                                case "Http2StreamException":
                                    if (e.InnerException.Message.Contains("INTERNAL_ERROR") || // UseKestrel (https://github.com/aspnet/AspNetCore/issues/12256)
                                        e.InnerException.Message.Contains("CANCEL")) // UseHttpSys
                                    {
                                        return;
                                    }
                                    break;
                            }
                        }

                        throw;
                    }
                }),

                ("POST",
                async ctx =>
                {
                    string content = ctx.GetRandomSubstring(ctx.ContentSource);
                    Version httpVersion = ctx.GetRandomHttpVersion();

                    using (var req = new HttpRequestMessage(HttpMethod.Post, "/") { Version = httpVersion, Content = new StringDuplexContent(content) })
                    using (HttpResponseMessage m = await ctx.SendAsync(req))
                    {
                        ValidateResponse(m, httpVersion);
                        ValidateContent(content, await m.Content.ReadAsStringAsync());;
                    }
                }),

                ("POST Multipart Data",
                async ctx =>
                {
                    (string expected, MultipartContent formDataContent) formData = GetMultipartContent(ctx.ContentSource, ctx, ctx.MaxRequestParameters);
                    Version httpVersion = ctx.GetRandomHttpVersion();

                    using (var req = new HttpRequestMessage(HttpMethod.Post, "/") { Version = httpVersion, Content = formData.formDataContent })
                    using (HttpResponseMessage m = await ctx.SendAsync(req))
                    {
                        ValidateResponse(m, httpVersion);
                        ValidateContent($"{formData.expected}", await m.Content.ReadAsStringAsync());;
                    }
                }),

                ("POST Duplex",
                async ctx =>
                {
                    string content = ctx.GetRandomSubstring(ctx.ContentSource);
                    Version httpVersion = ctx.GetRandomHttpVersion();

                    using (var req = new HttpRequestMessage(HttpMethod.Post, "/duplex") { Version = httpVersion, Content = new StringDuplexContent(content) })
                    using (HttpResponseMessage m = await ctx.SendAsync(req, HttpCompletionOption.ResponseHeadersRead))
                    {
                        ValidateResponse(m, httpVersion);
                        ValidateContent(content, await m.Content.ReadAsStringAsync());
                    }
                }),

                ("POST Duplex Slow",
                async ctx =>
                {
                    string content = ctx.GetRandomSubstring(ctx.ContentSource);
                    Version httpVersion = ctx.GetRandomHttpVersion();

                    using (var req = new HttpRequestMessage(HttpMethod.Post, "/duplexSlow") { Version = httpVersion, Content = new ByteAtATimeNoLengthContent(Encoding.ASCII.GetBytes(content)) })
                    using (HttpResponseMessage m = await ctx.SendAsync(req, HttpCompletionOption.ResponseHeadersRead))
                    {
                        ValidateResponse(m, httpVersion);
                        ValidateContent(content, await m.Content.ReadAsStringAsync());
                    }
                }),

                ("POST ExpectContinue",
                async ctx =>
                {
                    string content = ctx.GetRandomSubstring(ctx.ContentSource);
                    Version httpVersion = ctx.GetRandomHttpVersion();

                    using (var req = new HttpRequestMessage(HttpMethod.Post, "/") { Version = httpVersion, Content = new StringContent(content) })
                    {
                        req.Headers.ExpectContinue = true;
                        using (HttpResponseMessage m = await ctx.SendAsync(req, HttpCompletionOption.ResponseHeadersRead))
                        {
                            ValidateResponse(m, httpVersion);
                            ValidateContent(content, await m.Content.ReadAsStringAsync());
                        }
                    }
                }),

                ("HEAD",
                async ctx =>
                {
                    Version httpVersion = ctx.GetRandomHttpVersion();
                    using (var req = new HttpRequestMessage(HttpMethod.Head, "/") { Version = httpVersion })
                    using (HttpResponseMessage m = await ctx.SendAsync(req))
                    {
                        ValidateResponse(m, httpVersion);
                        if (m.Content.Headers.ContentLength != ctx.MaxContentLength)
                        {
                            throw new Exception($"Expected {ctx.MaxContentLength}, got {m.Content.Headers.ContentLength}");
                        }
                        string r = await m.Content.ReadAsStringAsync();
                        if (r.Length > 0) throw new Exception($"Got unexpected response: {r}");
                    }
                }),

                ("PUT",
                async ctx =>
                {
                    string content = ctx.GetRandomSubstring(ctx.ContentSource);
                    Version httpVersion = ctx.GetRandomHttpVersion();

                    using (var req = new HttpRequestMessage(HttpMethod.Put, "/") { Version = httpVersion, Content = new StringContent(content) })
                    using (HttpResponseMessage m = await ctx.SendAsync(req))
                    {
                        ValidateResponse(m, httpVersion);
                        string r = await m.Content.ReadAsStringAsync();
                        if (r != "") throw new Exception($"Got unexpected response: {r}");
                    }
                }),
            };

        // Validation of a response message
        private static void ValidateResponse(HttpResponseMessage m, Version expectedVersion)
        {
            if (m.Version != expectedVersion)
            {
                throw new Exception($"Expected response version {expectedVersion}, got {m.Version}");
            }
        }

        private static void ValidateContent(string expectedContent, string actualContent, string details = null)
        {
            if (actualContent != expectedContent)
            {
                throw new Exception($"Expected response content \"{expectedContent}\", got \"{actualContent}\". {details}");
            }
        }

        private static string GetGetQueryParameters(ref string uri, int maxRequestLineSize, string contentSource, RequestContext clientContext, int numParameters)
        {
            if (maxRequestLineSize < uri.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(maxRequestLineSize));
            }
            if (numParameters <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(numParameters));
            }

            var expectedString = new StringBuilder();
            var uriSb = new StringBuilder(uri);
            maxRequestLineSize -= uri.Length;

            int appxMaxValueLength = Math.Max(maxRequestLineSize / numParameters, 1);

            int num = clientContext.GetRandomInt32(1, numParameters + 1);
            for (int i = 0; i < num; i++)
            {
                string key = $"{(i == 0 ? "?" : "&")}Var{i}=";

                int remainingLength = maxRequestLineSize - uriSb.Length - key.Length;
                if (remainingLength <= 0)
                {
                    break;
                }

                uriSb.Append(key);

                string value = clientContext.GetRandomString(Math.Min(appxMaxValueLength, remainingLength));
                expectedString.Append(value);
                uriSb.Append(value);
            }

            uri = uriSb.ToString();
            return expectedString.ToString();
        }

        private static (string, MultipartContent) GetMultipartContent(string contentSource, RequestContext clientContext, int numFormFields)
        {
            var multipartContent = new MultipartContent("prefix" + clientContext.GetRandomSubstring(contentSource), "test_boundary");
            StringBuilder sb = new StringBuilder();

            int num = clientContext.GetRandomInt32(1, numFormFields + 1);

            for (int i = 0; i < num; i++)
            {
                sb.Append("--test_boundary\r\nContent-Type: text/plain; charset=utf-8\r\n\r\n");
                string content = clientContext.GetRandomSubstring(contentSource);
                sb.Append(content);
                sb.Append("\r\n");
                multipartContent.Add(new StringContent(content));
            }

            sb.Append("--test_boundary--\r\n");
            return (sb.ToString(), multipartContent);
        }

        /// <summary>HttpContent that's similar to StringContent but that can be used with HTTP/2 duplex communication.</summary>
        private sealed class StringDuplexContent : HttpContent
        {
            private readonly byte[] _data;

            public StringDuplexContent(string value) => _data = Encoding.UTF8.GetBytes(value);

            protected override Task SerializeToStreamAsync(Stream stream, TransportContext context) =>
                stream.WriteAsync(_data, 0, _data.Length);

            protected override bool TryComputeLength(out long length)
            {
                length = _data.Length;
                return true;
            }
        }

        /// <summary>HttpContent that trickles out a byte at a time.</summary>
        private sealed class ByteAtATimeNoLengthContent : HttpContent
        {
            private readonly byte[] _buffer;

            public ByteAtATimeNoLengthContent(byte[] buffer) => _buffer = buffer;

            protected override async Task SerializeToStreamAsync(Stream stream, TransportContext context)
            {
                for (int i = 0; i < _buffer.Length; i++)
                {
                    await stream.WriteAsync(_buffer.AsMemory(i, 1));
                    await stream.FlushAsync();
                }
            }

            protected override bool TryComputeLength(out long length)
            {
                length = 0;
                return false;
            }
        }
    }
}
