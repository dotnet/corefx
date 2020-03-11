﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http.Json
{
    public static class HttpContentJsonExtensions
    {
        public static Task<object?> ReadFromJsonAsync(
            this HttpContent content,
            Type type,
            JsonSerializerOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            return ReadFromJsonAsyncCore(content, type, options, cancellationToken);
        }

        public static Task<T> ReadFromJsonAsync<T>(
            this HttpContent content,
            JsonSerializerOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            return ReadFromJsonAsyncCore<T>(content, options, cancellationToken);
        }

        private static async Task<object?> ReadFromJsonAsyncCore(HttpContent content, Type type, JsonSerializerOptions? options, CancellationToken cancellationToken)
        {
            byte[] contentBytes = await GetUtf8JsonBytesFromContentAsync(content, cancellationToken).ConfigureAwait(false);
            // NOTE: I wanted to use DeserializeAsync and ReadAsStreamAsync, but if we need to transcode, we need the whole content, so it makes more sense to use ReadAsByteArrayAsync.
            //Stream utf8Stream = await ReadStreamFromContent(content).ConfigureAwait(false);
            //return await JsonSerializer.DeserializeAsync(utf8Stream, type, options, cancellationToken);
            return JsonSerializer.Deserialize(contentBytes, type, options);
        }

        private static async Task<T> ReadFromJsonAsyncCore<T>(HttpContent content, JsonSerializerOptions? options, CancellationToken cancellationToken)
        {
            byte[] contentBytes = await GetUtf8JsonBytesFromContentAsync(content, cancellationToken).ConfigureAwait(false);
            return JsonSerializer.Deserialize<T>(contentBytes, options)!;
        }

        private static async Task<byte[]> GetUtf8JsonBytesFromContentAsync(HttpContent content, CancellationToken cancellationToken)
        {
            string? mediaType = content.Headers.ContentType?.MediaType;
            // if Content-Type == null we assume it as "application/octet-stream" in accordance with section 7.2.1 of the HTTP spec.
            // This is how Formatting API works

            // And at the same time, it is contrary to how HttpContent.ReadAsStringAsync works (allows null content-type and tries to read content using UTF-8 in that case).
            // IMO, this should default to application/json
            if (mediaType != JsonContent.JsonMediaType &&
                mediaType != MediaTypeNames.Text.Plain)
            {
                throw new NotSupportedException("The provided ContentType is not supported; the supported types are 'application/json' and 'text/plain'.");
            }

            // https://source.dot.net/#System.Net.Http/System/Net/Http/HttpContent.cs,047409be2a4d70a8
            string? charset = content.Headers.ContentType!.CharSet;
            Encoding? encoding = null;
            if (charset != null)
            {
                try
                {
                    // Remove at most a single set of quotes.
                    if (charset.Length > 2 &&
                        charset[0] == '\"' &&
                        charset[charset.Length - 1] == '\"')
                    {
                        encoding = Encoding.GetEncoding(charset.Substring(1, charset.Length - 2));
                    }
                    else
                    {
                        encoding = Encoding.GetEncoding(charset);
                    }

                    // Byte-order-mark (BOM) characters may be present even if a charset was specified.
                    // bomLength = GetPreambleLength(buffer, encoding);
                }
                catch (ArgumentException e)
                {
                    throw new InvalidOperationException("The character set provided in ContentType is invalid.", e);
                }
            }

            byte[] contentBytes = await content.ReadAsByteArrayAsync().ConfigureAwait(false);

            // Transcode to UTF-8.
            if (encoding != null && encoding != Encoding.UTF8)
            {
                contentBytes = Encoding.Convert(encoding, Encoding.UTF8, contentBytes);
            }

            return contentBytes;
        }
    }
}
