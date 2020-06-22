﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http.Json
{
    public static partial class HttpClientJsonExtensions
    {
        public static Task<HttpResponseMessage> PutAsJsonAsync<TValue>(this HttpClient client, string? requestUri, TValue value, JsonSerializerOptions? options = null, CancellationToken cancellationToken = default)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            JsonContent content = JsonContent.Create(value, mediaType: null, options);
            return client.PutAsync(requestUri, content, cancellationToken);
        }

        public static Task<HttpResponseMessage> PutAsJsonAsync<TValue>(this HttpClient client, Uri? requestUri, TValue value, JsonSerializerOptions? options = null, CancellationToken cancellationToken = default)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            JsonContent content = JsonContent.Create(value, mediaType: null, options);
            return client.PutAsync(requestUri, content, cancellationToken);
        }

        public static Task<HttpResponseMessage> PutAsJsonAsync<TValue>(this HttpClient client, string? requestUri, TValue value, CancellationToken cancellationToken)
            => client.PutAsJsonAsync(requestUri, value, options: null, cancellationToken);

        public static Task<HttpResponseMessage> PutAsJsonAsync<TValue>(this HttpClient client, Uri? requestUri, TValue value, CancellationToken cancellationToken)
            => client.PutAsJsonAsync(requestUri, value, options: null, cancellationToken);
    }
}
