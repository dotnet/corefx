// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the https://aka.ms/api-review process.
// ------------------------------------------------------------------------------

namespace System.Net.Http.Json
{
    public static partial class HttpClientJsonExtensions
    {
        public static System.Threading.Tasks.Task<object?> GetFromJsonAsync(this System.Net.Http.HttpClient client, string? requestUri, System.Type type, System.Text.Json.JsonSerializerOptions? options, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        public static System.Threading.Tasks.Task<object?> GetFromJsonAsync(this System.Net.Http.HttpClient client, string? requestUri, System.Type type, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        public static System.Threading.Tasks.Task<object?> GetFromJsonAsync(this System.Net.Http.HttpClient client, System.Uri? requestUri, System.Type type, System.Text.Json.JsonSerializerOptions? options, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        public static System.Threading.Tasks.Task<object?> GetFromJsonAsync(this System.Net.Http.HttpClient client, System.Uri? requestUri, System.Type type, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        public static System.Threading.Tasks.Task<TValue> GetFromJsonAsync<TValue>(this System.Net.Http.HttpClient client, string? requestUri, System.Text.Json.JsonSerializerOptions? options, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        public static System.Threading.Tasks.Task<TValue> GetFromJsonAsync<TValue>(this System.Net.Http.HttpClient client, string? requestUri, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        public static System.Threading.Tasks.Task<TValue> GetFromJsonAsync<TValue>(this System.Net.Http.HttpClient client, System.Uri? requestUri, System.Text.Json.JsonSerializerOptions? options, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        public static System.Threading.Tasks.Task<TValue> GetFromJsonAsync<TValue>(this System.Net.Http.HttpClient client, System.Uri? requestUri, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        public static System.Threading.Tasks.Task<System.Net.Http.HttpResponseMessage> PostAsJsonAsync<TValue>(this System.Net.Http.HttpClient client, string? requestUri, TValue value, System.Text.Json.JsonSerializerOptions? options = null, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        public static System.Threading.Tasks.Task<System.Net.Http.HttpResponseMessage> PostAsJsonAsync<TValue>(this System.Net.Http.HttpClient client, string? requestUri, TValue value, System.Threading.CancellationToken cancellationToken) { throw null; }
        public static System.Threading.Tasks.Task<System.Net.Http.HttpResponseMessage> PostAsJsonAsync<TValue>(this System.Net.Http.HttpClient client, System.Uri? requestUri, TValue value, System.Text.Json.JsonSerializerOptions? options = null, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        public static System.Threading.Tasks.Task<System.Net.Http.HttpResponseMessage> PostAsJsonAsync<TValue>(this System.Net.Http.HttpClient client, System.Uri? requestUri, TValue value, System.Threading.CancellationToken cancellationToken) { throw null; }
        public static System.Threading.Tasks.Task<System.Net.Http.HttpResponseMessage> PutAsJsonAsync<TValue>(this System.Net.Http.HttpClient client, string? requestUri, TValue value, System.Text.Json.JsonSerializerOptions? options = null, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        public static System.Threading.Tasks.Task<System.Net.Http.HttpResponseMessage> PutAsJsonAsync<TValue>(this System.Net.Http.HttpClient client, string? requestUri, TValue value, System.Threading.CancellationToken cancellationToken) { throw null; }
        public static System.Threading.Tasks.Task<System.Net.Http.HttpResponseMessage> PutAsJsonAsync<TValue>(this System.Net.Http.HttpClient client, System.Uri? requestUri, TValue value, System.Text.Json.JsonSerializerOptions? options = null, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        public static System.Threading.Tasks.Task<System.Net.Http.HttpResponseMessage> PutAsJsonAsync<TValue>(this System.Net.Http.HttpClient client, System.Uri? requestUri, TValue value, System.Threading.CancellationToken cancellationToken) { throw null; }
    }
    public static partial class HttpContentJsonExtensions
    {
        public static System.Threading.Tasks.Task<object?> ReadFromJsonAsync(this System.Net.Http.HttpContent content, System.Type type, System.Text.Json.JsonSerializerOptions? options = null, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        public static System.Threading.Tasks.Task<T> ReadFromJsonAsync<T>(this System.Net.Http.HttpContent content, System.Text.Json.JsonSerializerOptions? options = null, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
    }
    public sealed partial class JsonContent : System.Net.Http.HttpContent
    {
        internal JsonContent() { }
        public System.Type ObjectType { get { throw null; } }
        public object? Value { get { throw null; } }
        public static System.Net.Http.Json.JsonContent Create(object? inputValue, System.Type inputType, System.Net.Http.Headers.MediaTypeHeaderValue? mediaType = null, System.Text.Json.JsonSerializerOptions? options = null) { throw null; }
        public static System.Net.Http.Json.JsonContent Create<T>(T inputValue, System.Net.Http.Headers.MediaTypeHeaderValue? mediaType = null, System.Text.Json.JsonSerializerOptions? options = null) { throw null; }
        protected override System.Threading.Tasks.Task SerializeToStreamAsync(System.IO.Stream stream, System.Net.TransportContext? context) { throw null; }
        protected override bool TryComputeLength(out long length) { throw null; }
    }
}
