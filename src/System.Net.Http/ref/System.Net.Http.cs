// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------


namespace System.Net.Http
{
    public partial class ByteArrayContent : System.Net.Http.HttpContent
    {
        public ByteArrayContent(byte[] content) { }
        public ByteArrayContent(byte[] content, int offset, int count) { }
        protected override System.Threading.Tasks.Task<System.IO.Stream> CreateContentReadStreamAsync() { throw null; }
        protected override System.Threading.Tasks.Task SerializeToStreamAsync(System.IO.Stream stream, System.Net.TransportContext context) { throw null; }
        protected internal override bool TryComputeLength(out long length) { throw null; }
    }
    public enum ClientCertificateOption
    {
        Automatic = 1,
        Manual = 0,
    }
    public abstract partial class DelegatingHandler : System.Net.Http.HttpMessageHandler
    {
        protected DelegatingHandler() { }
        protected DelegatingHandler(System.Net.Http.HttpMessageHandler innerHandler) { }
        public System.Net.Http.HttpMessageHandler InnerHandler { get { throw null; } set { } }
        protected override void Dispose(bool disposing) { }
        protected internal override System.Threading.Tasks.Task<System.Net.Http.HttpResponseMessage> SendAsync(System.Net.Http.HttpRequestMessage request, System.Threading.CancellationToken cancellationToken) { throw null; }
    }
    public partial class FormUrlEncodedContent : System.Net.Http.ByteArrayContent
    {
        public FormUrlEncodedContent(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, string>> nameValueCollection) : base(default(byte[])) { }
    }
    public partial class HttpClient : System.Net.Http.HttpMessageInvoker
    {
        public HttpClient() : base(default(System.Net.Http.HttpMessageHandler)) { }
        public HttpClient(System.Net.Http.HttpMessageHandler handler) : base(default(System.Net.Http.HttpMessageHandler)) { }
        public HttpClient(System.Net.Http.HttpMessageHandler handler, bool disposeHandler) : base(default(System.Net.Http.HttpMessageHandler)) { }
        public System.Uri BaseAddress { get { throw null; } set { } }
        public System.Net.Http.Headers.HttpRequestHeaders DefaultRequestHeaders { get { throw null; } }
        public long MaxResponseContentBufferSize { get { throw null; } set { } }
        public System.TimeSpan Timeout { get { throw null; } set { } }
        public void CancelPendingRequests() { }
        public System.Threading.Tasks.Task<System.Net.Http.HttpResponseMessage> DeleteAsync(string requestUri) { throw null; }
        public System.Threading.Tasks.Task<System.Net.Http.HttpResponseMessage> DeleteAsync(string requestUri, System.Threading.CancellationToken cancellationToken) { throw null; }
        public System.Threading.Tasks.Task<System.Net.Http.HttpResponseMessage> DeleteAsync(System.Uri requestUri) { throw null; }
        public System.Threading.Tasks.Task<System.Net.Http.HttpResponseMessage> DeleteAsync(System.Uri requestUri, System.Threading.CancellationToken cancellationToken) { throw null; }
        protected override void Dispose(bool disposing) { }
        public System.Threading.Tasks.Task<System.Net.Http.HttpResponseMessage> GetAsync(string requestUri) { throw null; }
        public System.Threading.Tasks.Task<System.Net.Http.HttpResponseMessage> GetAsync(string requestUri, System.Net.Http.HttpCompletionOption completionOption) { throw null; }
        public System.Threading.Tasks.Task<System.Net.Http.HttpResponseMessage> GetAsync(string requestUri, System.Net.Http.HttpCompletionOption completionOption, System.Threading.CancellationToken cancellationToken) { throw null; }
        public System.Threading.Tasks.Task<System.Net.Http.HttpResponseMessage> GetAsync(string requestUri, System.Threading.CancellationToken cancellationToken) { throw null; }
        public System.Threading.Tasks.Task<System.Net.Http.HttpResponseMessage> GetAsync(System.Uri requestUri) { throw null; }
        public System.Threading.Tasks.Task<System.Net.Http.HttpResponseMessage> GetAsync(System.Uri requestUri, System.Net.Http.HttpCompletionOption completionOption) { throw null; }
        public System.Threading.Tasks.Task<System.Net.Http.HttpResponseMessage> GetAsync(System.Uri requestUri, System.Net.Http.HttpCompletionOption completionOption, System.Threading.CancellationToken cancellationToken) { throw null; }
        public System.Threading.Tasks.Task<System.Net.Http.HttpResponseMessage> GetAsync(System.Uri requestUri, System.Threading.CancellationToken cancellationToken) { throw null; }
        public System.Threading.Tasks.Task<byte[]> GetByteArrayAsync(string requestUri) { throw null; }
        public System.Threading.Tasks.Task<byte[]> GetByteArrayAsync(System.Uri requestUri) { throw null; }
        public System.Threading.Tasks.Task<System.IO.Stream> GetStreamAsync(string requestUri) { throw null; }
        public System.Threading.Tasks.Task<System.IO.Stream> GetStreamAsync(System.Uri requestUri) { throw null; }
        public System.Threading.Tasks.Task<string> GetStringAsync(string requestUri) { throw null; }
        public System.Threading.Tasks.Task<string> GetStringAsync(System.Uri requestUri) { throw null; }
        public System.Threading.Tasks.Task<System.Net.Http.HttpResponseMessage> PostAsync(string requestUri, System.Net.Http.HttpContent content) { throw null; }
        public System.Threading.Tasks.Task<System.Net.Http.HttpResponseMessage> PostAsync(string requestUri, System.Net.Http.HttpContent content, System.Threading.CancellationToken cancellationToken) { throw null; }
        public System.Threading.Tasks.Task<System.Net.Http.HttpResponseMessage> PostAsync(System.Uri requestUri, System.Net.Http.HttpContent content) { throw null; }
        public System.Threading.Tasks.Task<System.Net.Http.HttpResponseMessage> PostAsync(System.Uri requestUri, System.Net.Http.HttpContent content, System.Threading.CancellationToken cancellationToken) { throw null; }
        public System.Threading.Tasks.Task<System.Net.Http.HttpResponseMessage> PutAsync(string requestUri, System.Net.Http.HttpContent content) { throw null; }
        public System.Threading.Tasks.Task<System.Net.Http.HttpResponseMessage> PutAsync(string requestUri, System.Net.Http.HttpContent content, System.Threading.CancellationToken cancellationToken) { throw null; }
        public System.Threading.Tasks.Task<System.Net.Http.HttpResponseMessage> PutAsync(System.Uri requestUri, System.Net.Http.HttpContent content) { throw null; }
        public System.Threading.Tasks.Task<System.Net.Http.HttpResponseMessage> PutAsync(System.Uri requestUri, System.Net.Http.HttpContent content, System.Threading.CancellationToken cancellationToken) { throw null; }
        public System.Threading.Tasks.Task<System.Net.Http.HttpResponseMessage> SendAsync(System.Net.Http.HttpRequestMessage request) { throw null; }
        public System.Threading.Tasks.Task<System.Net.Http.HttpResponseMessage> SendAsync(System.Net.Http.HttpRequestMessage request, System.Net.Http.HttpCompletionOption completionOption) { throw null; }
        public System.Threading.Tasks.Task<System.Net.Http.HttpResponseMessage> SendAsync(System.Net.Http.HttpRequestMessage request, System.Net.Http.HttpCompletionOption completionOption, System.Threading.CancellationToken cancellationToken) { throw null; }
        public override System.Threading.Tasks.Task<System.Net.Http.HttpResponseMessage> SendAsync(System.Net.Http.HttpRequestMessage request, System.Threading.CancellationToken cancellationToken) { throw null; }
    }
    public partial class HttpClientHandler : System.Net.Http.HttpMessageHandler
    {
        public HttpClientHandler() { }
        public bool AllowAutoRedirect { get { throw null; } set { } }
        public System.Net.DecompressionMethods AutomaticDecompression { get { throw null; } set { } }
        public bool CheckCertificateRevocationList { get { throw null; } set { } }
        public System.Net.Http.ClientCertificateOption ClientCertificateOptions { get { throw null; } set { } }
        public System.Security.Cryptography.X509Certificates.X509CertificateCollection ClientCertificates { get { throw null; } }
        public System.Net.CookieContainer CookieContainer { get { throw null; } set { } }
        public System.Net.ICredentials Credentials { get { throw null; } set { } }
        public static System.Func<System.Net.Http.HttpRequestMessage, System.Security.Cryptography.X509Certificates.X509Certificate2, System.Security.Cryptography.X509Certificates.X509Chain, System.Net.Security.SslPolicyErrors, bool> DangerousAcceptAnyServerCertificateValidator { get { throw null; } }
        public System.Net.ICredentials DefaultProxyCredentials { get { throw null; } set { } }
        public int MaxAutomaticRedirections { get { throw null; } set { } }
        public int MaxConnectionsPerServer { get { throw null; } set { } }
        public long MaxRequestContentBufferSize { get { throw null; } set { } }
        public int MaxResponseHeadersLength { get { throw null; } set { } }
        public bool PreAuthenticate { get { throw null; } set { } }
        public System.Collections.Generic.IDictionary<string, object> Properties { get { throw null; } }
        public System.Net.IWebProxy Proxy { get { throw null; } set { } }
        public System.Func<System.Net.Http.HttpRequestMessage, System.Security.Cryptography.X509Certificates.X509Certificate2, System.Security.Cryptography.X509Certificates.X509Chain, System.Net.Security.SslPolicyErrors, bool> ServerCertificateCustomValidationCallback { get { throw null; } set { } }
        public System.Security.Authentication.SslProtocols SslProtocols { get { throw null; } set { } }
        public virtual bool SupportsAutomaticDecompression { get { throw null; } }
        public virtual bool SupportsProxy { get { throw null; } }
        public virtual bool SupportsRedirectConfiguration { get { throw null; } }
        public bool UseCookies { get { throw null; } set { } }
        public bool UseDefaultCredentials { get { throw null; } set { } }
        public bool UseProxy { get { throw null; } set { } }
        protected override void Dispose(bool disposing) { }
        protected internal override System.Threading.Tasks.Task<System.Net.Http.HttpResponseMessage> SendAsync(System.Net.Http.HttpRequestMessage request, System.Threading.CancellationToken cancellationToken) { throw null; }
    }
    public enum HttpCompletionOption
    {
        ResponseContentRead = 0,
        ResponseHeadersRead = 1,
    }
    public abstract partial class HttpContent : System.IDisposable
    {
        protected HttpContent() { }
        public System.Net.Http.Headers.HttpContentHeaders Headers { get { throw null; } }
        public System.Threading.Tasks.Task CopyToAsync(System.IO.Stream stream) { throw null; }
        public System.Threading.Tasks.Task CopyToAsync(System.IO.Stream stream, System.Net.TransportContext context) { throw null; }
        protected virtual System.Threading.Tasks.Task<System.IO.Stream> CreateContentReadStreamAsync() { throw null; }
        public void Dispose() { }
        protected virtual void Dispose(bool disposing) { }
        public System.Threading.Tasks.Task LoadIntoBufferAsync() { throw null; }
        public System.Threading.Tasks.Task LoadIntoBufferAsync(long maxBufferSize) { throw null; }
        public System.Threading.Tasks.Task<byte[]> ReadAsByteArrayAsync() { throw null; }
        public System.Threading.Tasks.Task<System.IO.Stream> ReadAsStreamAsync() { throw null; }
        public System.Threading.Tasks.Task<string> ReadAsStringAsync() { throw null; }
        protected abstract System.Threading.Tasks.Task SerializeToStreamAsync(System.IO.Stream stream, System.Net.TransportContext context);
        protected internal abstract bool TryComputeLength(out long length);
    }
    public abstract partial class HttpMessageHandler : System.IDisposable
    {
        protected HttpMessageHandler() { }
        public void Dispose() { }
        protected virtual void Dispose(bool disposing) { }
        protected internal abstract System.Threading.Tasks.Task<System.Net.Http.HttpResponseMessage> SendAsync(System.Net.Http.HttpRequestMessage request, System.Threading.CancellationToken cancellationToken);
    }
    public partial class HttpMessageInvoker : System.IDisposable
    {
        public HttpMessageInvoker(System.Net.Http.HttpMessageHandler handler) { }
        public HttpMessageInvoker(System.Net.Http.HttpMessageHandler handler, bool disposeHandler) { }
        public void Dispose() { }
        protected virtual void Dispose(bool disposing) { }
        public virtual System.Threading.Tasks.Task<System.Net.Http.HttpResponseMessage> SendAsync(System.Net.Http.HttpRequestMessage request, System.Threading.CancellationToken cancellationToken) { throw null; }
    }
    public partial class HttpMethod : System.IEquatable<System.Net.Http.HttpMethod>
    {
        public HttpMethod(string method) { }
        public static System.Net.Http.HttpMethod Delete { get { throw null; } }
        public static System.Net.Http.HttpMethod Get { get { throw null; } }
        public static System.Net.Http.HttpMethod Head { get { throw null; } }
        public string Method { get { throw null; } }
        public static System.Net.Http.HttpMethod Options { get { throw null; } }
        public static System.Net.Http.HttpMethod Post { get { throw null; } }
        public static System.Net.Http.HttpMethod Put { get { throw null; } }
        public static System.Net.Http.HttpMethod Trace { get { throw null; } }
        public bool Equals(System.Net.Http.HttpMethod other) { throw null; }
        public override bool Equals(object obj) { throw null; }
        public override int GetHashCode() { throw null; }
        public static bool operator ==(System.Net.Http.HttpMethod left, System.Net.Http.HttpMethod right) { throw null; }
        public static bool operator !=(System.Net.Http.HttpMethod left, System.Net.Http.HttpMethod right) { throw null; }
        public override string ToString() { throw null; }
    }
    public partial class HttpRequestException : System.Exception
    {
        public HttpRequestException() { }
        public HttpRequestException(string message) { }
        public HttpRequestException(string message, System.Exception inner) { }
    }
    public partial class HttpRequestMessage : System.IDisposable
    {
        public HttpRequestMessage() { }
        public HttpRequestMessage(System.Net.Http.HttpMethod method, string requestUri) { }
        public HttpRequestMessage(System.Net.Http.HttpMethod method, System.Uri requestUri) { }
        public System.Net.Http.HttpContent Content { get { throw null; } set { } }
        public System.Net.Http.Headers.HttpRequestHeaders Headers { get { throw null; } }
        public System.Net.Http.HttpMethod Method { get { throw null; } set { } }
        public System.Collections.Generic.IDictionary<string, object> Properties { get { throw null; } }
        public System.Uri RequestUri { get { throw null; } set { } }
        public System.Version Version { get { throw null; } set { } }
        public void Dispose() { }
        protected virtual void Dispose(bool disposing) { }
        public override string ToString() { throw null; }
    }
    public partial class HttpResponseMessage : System.IDisposable
    {
        public HttpResponseMessage() { }
        public HttpResponseMessage(System.Net.HttpStatusCode statusCode) { }
        public System.Net.Http.HttpContent Content { get { throw null; } set { } }
        public System.Net.Http.Headers.HttpResponseHeaders Headers { get { throw null; } }
        public bool IsSuccessStatusCode { get { throw null; } }
        public string ReasonPhrase { get { throw null; } set { } }
        public System.Net.Http.HttpRequestMessage RequestMessage { get { throw null; } set { } }
        public System.Net.HttpStatusCode StatusCode { get { throw null; } set { } }
        public System.Version Version { get { throw null; } set { } }
        public void Dispose() { }
        protected virtual void Dispose(bool disposing) { }
        public System.Net.Http.HttpResponseMessage EnsureSuccessStatusCode() { throw null; }
        public override string ToString() { throw null; }
    }
    public abstract partial class MessageProcessingHandler : System.Net.Http.DelegatingHandler
    {
        protected MessageProcessingHandler() { }
        protected MessageProcessingHandler(System.Net.Http.HttpMessageHandler innerHandler) { }
        protected abstract System.Net.Http.HttpRequestMessage ProcessRequest(System.Net.Http.HttpRequestMessage request, System.Threading.CancellationToken cancellationToken);
        protected abstract System.Net.Http.HttpResponseMessage ProcessResponse(System.Net.Http.HttpResponseMessage response, System.Threading.CancellationToken cancellationToken);
        protected internal sealed override System.Threading.Tasks.Task<System.Net.Http.HttpResponseMessage> SendAsync(System.Net.Http.HttpRequestMessage request, System.Threading.CancellationToken cancellationToken) { throw null; }
    }
    public partial class MultipartContent : System.Net.Http.HttpContent, System.Collections.Generic.IEnumerable<System.Net.Http.HttpContent>, System.Collections.IEnumerable
    {
        public MultipartContent() { }
        public MultipartContent(string subtype) { }
        public MultipartContent(string subtype, string boundary) { }
        public virtual void Add(System.Net.Http.HttpContent content) { }
        protected override void Dispose(bool disposing) { }
        public System.Collections.Generic.IEnumerator<System.Net.Http.HttpContent> GetEnumerator() { throw null; }
        protected override System.Threading.Tasks.Task SerializeToStreamAsync(System.IO.Stream stream, System.Net.TransportContext context) { throw null; }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { throw null; }
        protected internal override bool TryComputeLength(out long length) { throw null; }
    }
    public partial class MultipartFormDataContent : System.Net.Http.MultipartContent
    {
        public MultipartFormDataContent() { }
        public MultipartFormDataContent(string boundary) { }
        public override void Add(System.Net.Http.HttpContent content) { }
        public void Add(System.Net.Http.HttpContent content, string name) { }
        public void Add(System.Net.Http.HttpContent content, string name, string fileName) { }
    }
    public sealed partial class ReadOnlyMemoryContent : System.Net.Http.HttpContent
    {
        public ReadOnlyMemoryContent(System.ReadOnlyMemory<byte> content) { }
        protected override System.Threading.Tasks.Task SerializeToStreamAsync(System.IO.Stream stream, System.Net.TransportContext context) => throw null;
        protected internal override bool TryComputeLength(out long length) => throw null;
    }
    public partial class StreamContent : System.Net.Http.HttpContent
    {
        public StreamContent(System.IO.Stream content) { }
        public StreamContent(System.IO.Stream content, int bufferSize) { }
        protected override System.Threading.Tasks.Task<System.IO.Stream> CreateContentReadStreamAsync() { throw null; }
        protected override void Dispose(bool disposing) { }
        protected override System.Threading.Tasks.Task SerializeToStreamAsync(System.IO.Stream stream, System.Net.TransportContext context) { throw null; }
        protected internal override bool TryComputeLength(out long length) { throw null; }
    }
    public partial class StringContent : System.Net.Http.ByteArrayContent
    {
        public StringContent(string content) : base(default(byte[])) { }
        public StringContent(string content, System.Text.Encoding encoding) : base(default(byte[])) { }
        public StringContent(string content, System.Text.Encoding encoding, string mediaType) : base(default(byte[])) { }
    }
}
namespace System.Net.Http.Headers
{
    public partial class AuthenticationHeaderValue : System.ICloneable
    {
        public AuthenticationHeaderValue(string scheme) { }
        public AuthenticationHeaderValue(string scheme, string parameter) { }
        public string Parameter { get { throw null; } }
        public string Scheme { get { throw null; } }
        public override bool Equals(object obj) { throw null; }
        object System.ICloneable.Clone() { throw null; }
        public override int GetHashCode() { throw null; }
        public static System.Net.Http.Headers.AuthenticationHeaderValue Parse(string input) { throw null; }
        public override string ToString() { throw null; }
        public static bool TryParse(string input, out System.Net.Http.Headers.AuthenticationHeaderValue parsedValue) { throw null; }
    }
    public partial class CacheControlHeaderValue : System.ICloneable
    {
        public CacheControlHeaderValue() { }
        public System.Collections.Generic.ICollection<System.Net.Http.Headers.NameValueHeaderValue> Extensions { get { throw null; } }
        public System.Nullable<System.TimeSpan> MaxAge { get { throw null; } set { } }
        public bool MaxStale { get { throw null; } set { } }
        public System.Nullable<System.TimeSpan> MaxStaleLimit { get { throw null; } set { } }
        public System.Nullable<System.TimeSpan> MinFresh { get { throw null; } set { } }
        public bool MustRevalidate { get { throw null; } set { } }
        public bool NoCache { get { throw null; } set { } }
        public System.Collections.Generic.ICollection<string> NoCacheHeaders { get { throw null; } }
        public bool NoStore { get { throw null; } set { } }
        public bool NoTransform { get { throw null; } set { } }
        public bool OnlyIfCached { get { throw null; } set { } }
        public bool Private { get { throw null; } set { } }
        public System.Collections.Generic.ICollection<string> PrivateHeaders { get { throw null; } }
        public bool ProxyRevalidate { get { throw null; } set { } }
        public bool Public { get { throw null; } set { } }
        public System.Nullable<System.TimeSpan> SharedMaxAge { get { throw null; } set { } }
        object System.ICloneable.Clone() { throw null; }
        public override bool Equals(object obj) { throw null; }
        public override int GetHashCode() { throw null; }
        public static System.Net.Http.Headers.CacheControlHeaderValue Parse(string input) { throw null; }
        public override string ToString() { throw null; }
        public static bool TryParse(string input, out System.Net.Http.Headers.CacheControlHeaderValue parsedValue) { throw null; }
    }
    public partial class ContentDispositionHeaderValue : System.ICloneable
    {
        protected ContentDispositionHeaderValue(System.Net.Http.Headers.ContentDispositionHeaderValue source) { }
        public ContentDispositionHeaderValue(string dispositionType) { }
        public System.Nullable<System.DateTimeOffset> CreationDate { get { throw null; } set { } }
        public string DispositionType { get { throw null; } set { } }
        public string FileName { get { throw null; } set { } }
        public string FileNameStar { get { throw null; } set { } }
        public System.Nullable<System.DateTimeOffset> ModificationDate { get { throw null; } set { } }
        public string Name { get { throw null; } set { } }
        public System.Collections.Generic.ICollection<System.Net.Http.Headers.NameValueHeaderValue> Parameters { get { throw null; } }
        public System.Nullable<System.DateTimeOffset> ReadDate { get { throw null; } set { } }
        public System.Nullable<long> Size { get { throw null; } set { } }
        object System.ICloneable.Clone() { throw null; }
        public override bool Equals(object obj) { throw null; }
        public override int GetHashCode() { throw null; }
        public static System.Net.Http.Headers.ContentDispositionHeaderValue Parse(string input) { throw null; }
        public override string ToString() { throw null; }
        public static bool TryParse(string input, out System.Net.Http.Headers.ContentDispositionHeaderValue parsedValue) { throw null; }
    }
    public partial class ContentRangeHeaderValue : System.ICloneable
    {
        public ContentRangeHeaderValue(long length) { }
        public ContentRangeHeaderValue(long from, long to) { }
        public ContentRangeHeaderValue(long from, long to, long length) { }
        public System.Nullable<long> From { get { throw null; } }
        public bool HasLength { get { throw null; } }
        public bool HasRange { get { throw null; } }
        public System.Nullable<long> Length { get { throw null; } }
        public System.Nullable<long> To { get { throw null; } }
        public string Unit { get { throw null; } set { } }
        object System.ICloneable.Clone() { throw null; }
        public override bool Equals(object obj) { throw null; }
        public override int GetHashCode() { throw null; }
        public static System.Net.Http.Headers.ContentRangeHeaderValue Parse(string input) { throw null; }
        public override string ToString() { throw null; }
        public static bool TryParse(string input, out System.Net.Http.Headers.ContentRangeHeaderValue parsedValue) { throw null; }
    }
    public partial class EntityTagHeaderValue : System.ICloneable
    {
        public EntityTagHeaderValue(string tag) { }
        public EntityTagHeaderValue(string tag, bool isWeak) { }
        public static System.Net.Http.Headers.EntityTagHeaderValue Any { get { throw null; } }
        public bool IsWeak { get { throw null; } }
        public string Tag { get { throw null; } }
        object System.ICloneable.Clone() { throw null; }
        public override bool Equals(object obj) { throw null; }
        public override int GetHashCode() { throw null; }
        public static System.Net.Http.Headers.EntityTagHeaderValue Parse(string input) { throw null; }
        public override string ToString() { throw null; }
        public static bool TryParse(string input, out System.Net.Http.Headers.EntityTagHeaderValue parsedValue) { throw null; }
    }
    public sealed partial class HttpContentHeaders : System.Net.Http.Headers.HttpHeaders
    {
        internal HttpContentHeaders() { }
        public System.Collections.Generic.ICollection<string> Allow { get { throw null; } }
        public System.Net.Http.Headers.ContentDispositionHeaderValue ContentDisposition { get { throw null; } set { } }
        public System.Collections.Generic.ICollection<string> ContentEncoding { get { throw null; } }
        public System.Collections.Generic.ICollection<string> ContentLanguage { get { throw null; } }
        public System.Nullable<long> ContentLength { get { throw null; } set { } }
        public System.Uri ContentLocation { get { throw null; } set { } }
        public byte[] ContentMD5 { get { throw null; } set { } }
        public System.Net.Http.Headers.ContentRangeHeaderValue ContentRange { get { throw null; } set { } }
        public System.Net.Http.Headers.MediaTypeHeaderValue ContentType { get { throw null; } set { } }
        public System.Nullable<System.DateTimeOffset> Expires { get { throw null; } set { } }
        public System.Nullable<System.DateTimeOffset> LastModified { get { throw null; } set { } }
    }
    public abstract partial class HttpHeaders : System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, System.Collections.Generic.IEnumerable<string>>>, System.Collections.IEnumerable
    {
        protected HttpHeaders() { }
        public void Add(string name, System.Collections.Generic.IEnumerable<string> values) { }
        public void Add(string name, string value) { }
        public void Clear() { }
        public bool Contains(string name) { throw null; }
        public System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<string, System.Collections.Generic.IEnumerable<string>>> GetEnumerator() { throw null; }
        public System.Collections.Generic.IEnumerable<string> GetValues(string name) { throw null; }
        public bool Remove(string name) { throw null; }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { throw null; }
        public override string ToString() { throw null; }
        public bool TryAddWithoutValidation(string name, System.Collections.Generic.IEnumerable<string> values) { throw null; }
        public bool TryAddWithoutValidation(string name, string value) { throw null; }
        public bool TryGetValues(string name, out System.Collections.Generic.IEnumerable<string> values) { throw null; }
    }
    public sealed partial class HttpHeaderValueCollection<T> : System.Collections.Generic.ICollection<T>, System.Collections.Generic.IEnumerable<T>, System.Collections.IEnumerable where T : class
    {
        internal HttpHeaderValueCollection() { }
        public int Count { get { throw null; } }
        public bool IsReadOnly { get { throw null; } }
        public void Add(T item) { }
        public void Clear() { }
        public bool Contains(T item) { throw null; }
        public void CopyTo(T[] array, int arrayIndex) { }
        public System.Collections.Generic.IEnumerator<T> GetEnumerator() { throw null; }
        public void ParseAdd(string input) { }
        public bool Remove(T item) { throw null; }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { throw null; }
        public override string ToString() { throw null; }
        public bool TryParseAdd(string input) { throw null; }
    }
    public sealed partial class HttpRequestHeaders : System.Net.Http.Headers.HttpHeaders
    {
        internal HttpRequestHeaders() { }
        public System.Net.Http.Headers.HttpHeaderValueCollection<System.Net.Http.Headers.MediaTypeWithQualityHeaderValue> Accept { get { throw null; } }
        public System.Net.Http.Headers.HttpHeaderValueCollection<System.Net.Http.Headers.StringWithQualityHeaderValue> AcceptCharset { get { throw null; } }
        public System.Net.Http.Headers.HttpHeaderValueCollection<System.Net.Http.Headers.StringWithQualityHeaderValue> AcceptEncoding { get { throw null; } }
        public System.Net.Http.Headers.HttpHeaderValueCollection<System.Net.Http.Headers.StringWithQualityHeaderValue> AcceptLanguage { get { throw null; } }
        public System.Net.Http.Headers.AuthenticationHeaderValue Authorization { get { throw null; } set { } }
        public System.Net.Http.Headers.CacheControlHeaderValue CacheControl { get { throw null; } set { } }
        public System.Net.Http.Headers.HttpHeaderValueCollection<string> Connection { get { throw null; } }
        public System.Nullable<bool> ConnectionClose { get { throw null; } set { } }
        public System.Nullable<System.DateTimeOffset> Date { get { throw null; } set { } }
        public System.Net.Http.Headers.HttpHeaderValueCollection<System.Net.Http.Headers.NameValueWithParametersHeaderValue> Expect { get { throw null; } }
        public System.Nullable<bool> ExpectContinue { get { throw null; } set { } }
        public string From { get { throw null; } set { } }
        public string Host { get { throw null; } set { } }
        public System.Net.Http.Headers.HttpHeaderValueCollection<System.Net.Http.Headers.EntityTagHeaderValue> IfMatch { get { throw null; } }
        public System.Nullable<System.DateTimeOffset> IfModifiedSince { get { throw null; } set { } }
        public System.Net.Http.Headers.HttpHeaderValueCollection<System.Net.Http.Headers.EntityTagHeaderValue> IfNoneMatch { get { throw null; } }
        public System.Net.Http.Headers.RangeConditionHeaderValue IfRange { get { throw null; } set { } }
        public System.Nullable<System.DateTimeOffset> IfUnmodifiedSince { get { throw null; } set { } }
        public System.Nullable<int> MaxForwards { get { throw null; } set { } }
        public System.Net.Http.Headers.HttpHeaderValueCollection<System.Net.Http.Headers.NameValueHeaderValue> Pragma { get { throw null; } }
        public System.Net.Http.Headers.AuthenticationHeaderValue ProxyAuthorization { get { throw null; } set { } }
        public System.Net.Http.Headers.RangeHeaderValue Range { get { throw null; } set { } }
        public System.Uri Referrer { get { throw null; } set { } }
        public System.Net.Http.Headers.HttpHeaderValueCollection<System.Net.Http.Headers.TransferCodingWithQualityHeaderValue> TE { get { throw null; } }
        public System.Net.Http.Headers.HttpHeaderValueCollection<string> Trailer { get { throw null; } }
        public System.Net.Http.Headers.HttpHeaderValueCollection<System.Net.Http.Headers.TransferCodingHeaderValue> TransferEncoding { get { throw null; } }
        public System.Nullable<bool> TransferEncodingChunked { get { throw null; } set { } }
        public System.Net.Http.Headers.HttpHeaderValueCollection<System.Net.Http.Headers.ProductHeaderValue> Upgrade { get { throw null; } }
        public System.Net.Http.Headers.HttpHeaderValueCollection<System.Net.Http.Headers.ProductInfoHeaderValue> UserAgent { get { throw null; } }
        public System.Net.Http.Headers.HttpHeaderValueCollection<System.Net.Http.Headers.ViaHeaderValue> Via { get { throw null; } }
        public System.Net.Http.Headers.HttpHeaderValueCollection<System.Net.Http.Headers.WarningHeaderValue> Warning { get { throw null; } }
    }
    public sealed partial class HttpResponseHeaders : System.Net.Http.Headers.HttpHeaders
    {
        internal HttpResponseHeaders() { }
        public System.Net.Http.Headers.HttpHeaderValueCollection<string> AcceptRanges { get { throw null; } }
        public System.Nullable<System.TimeSpan> Age { get { throw null; } set { } }
        public System.Net.Http.Headers.CacheControlHeaderValue CacheControl { get { throw null; } set { } }
        public System.Net.Http.Headers.HttpHeaderValueCollection<string> Connection { get { throw null; } }
        public System.Nullable<bool> ConnectionClose { get { throw null; } set { } }
        public System.Nullable<System.DateTimeOffset> Date { get { throw null; } set { } }
        public System.Net.Http.Headers.EntityTagHeaderValue ETag { get { throw null; } set { } }
        public System.Uri Location { get { throw null; } set { } }
        public System.Net.Http.Headers.HttpHeaderValueCollection<System.Net.Http.Headers.NameValueHeaderValue> Pragma { get { throw null; } }
        public System.Net.Http.Headers.HttpHeaderValueCollection<System.Net.Http.Headers.AuthenticationHeaderValue> ProxyAuthenticate { get { throw null; } }
        public System.Net.Http.Headers.RetryConditionHeaderValue RetryAfter { get { throw null; } set { } }
        public System.Net.Http.Headers.HttpHeaderValueCollection<System.Net.Http.Headers.ProductInfoHeaderValue> Server { get { throw null; } }
        public System.Net.Http.Headers.HttpHeaderValueCollection<string> Trailer { get { throw null; } }
        public System.Net.Http.Headers.HttpHeaderValueCollection<System.Net.Http.Headers.TransferCodingHeaderValue> TransferEncoding { get { throw null; } }
        public System.Nullable<bool> TransferEncodingChunked { get { throw null; } set { } }
        public System.Net.Http.Headers.HttpHeaderValueCollection<System.Net.Http.Headers.ProductHeaderValue> Upgrade { get { throw null; } }
        public System.Net.Http.Headers.HttpHeaderValueCollection<string> Vary { get { throw null; } }
        public System.Net.Http.Headers.HttpHeaderValueCollection<System.Net.Http.Headers.ViaHeaderValue> Via { get { throw null; } }
        public System.Net.Http.Headers.HttpHeaderValueCollection<System.Net.Http.Headers.WarningHeaderValue> Warning { get { throw null; } }
        public System.Net.Http.Headers.HttpHeaderValueCollection<System.Net.Http.Headers.AuthenticationHeaderValue> WwwAuthenticate { get { throw null; } }
    }
    public partial class MediaTypeHeaderValue : System.ICloneable
    {
        protected MediaTypeHeaderValue(System.Net.Http.Headers.MediaTypeHeaderValue source) { }
        public MediaTypeHeaderValue(string mediaType) { }
        public string CharSet { get { throw null; } set { } }
        public string MediaType { get { throw null; } set { } }
        public System.Collections.Generic.ICollection<System.Net.Http.Headers.NameValueHeaderValue> Parameters { get { throw null; } }
        object System.ICloneable.Clone() { throw null; }
        public override bool Equals(object obj) { throw null; }
        public override int GetHashCode() { throw null; }
        public static System.Net.Http.Headers.MediaTypeHeaderValue Parse(string input) { throw null; }
        public override string ToString() { throw null; }
        public static bool TryParse(string input, out System.Net.Http.Headers.MediaTypeHeaderValue parsedValue) { throw null; }
    }
    public sealed partial class MediaTypeWithQualityHeaderValue : System.Net.Http.Headers.MediaTypeHeaderValue, System.ICloneable
    {
        public MediaTypeWithQualityHeaderValue(string mediaType) : base(default(System.Net.Http.Headers.MediaTypeHeaderValue)) { }
        public MediaTypeWithQualityHeaderValue(string mediaType, double quality) : base(default(System.Net.Http.Headers.MediaTypeHeaderValue)) { }
        public System.Nullable<double> Quality { get { throw null; } set { } }
        object System.ICloneable.Clone() { throw null; }
        public static new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue Parse(string input) { throw null; }
        public static bool TryParse(string input, out System.Net.Http.Headers.MediaTypeWithQualityHeaderValue parsedValue) { throw null; }
    }
    public partial class NameValueHeaderValue : System.ICloneable
    {
        protected NameValueHeaderValue(System.Net.Http.Headers.NameValueHeaderValue source) { }
        public NameValueHeaderValue(string name) { }
        public NameValueHeaderValue(string name, string value) { }
        public string Name { get { throw null; } }
        public string Value { get { throw null; } set { } }
        object System.ICloneable.Clone() { throw null; }
        public override bool Equals(object obj) { throw null; }
        public override int GetHashCode() { throw null; }
        public static System.Net.Http.Headers.NameValueHeaderValue Parse(string input) { throw null; }
        public override string ToString() { throw null; }
        public static bool TryParse(string input, out System.Net.Http.Headers.NameValueHeaderValue parsedValue) { throw null; }
    }
    public partial class NameValueWithParametersHeaderValue : System.Net.Http.Headers.NameValueHeaderValue, System.ICloneable
    {
        protected NameValueWithParametersHeaderValue(System.Net.Http.Headers.NameValueWithParametersHeaderValue source) : base(default(string)) { }
        public NameValueWithParametersHeaderValue(string name) : base(default(string)) { }
        public NameValueWithParametersHeaderValue(string name, string value) : base(default(string)) { }
        public System.Collections.Generic.ICollection<System.Net.Http.Headers.NameValueHeaderValue> Parameters { get { throw null; } }
        object System.ICloneable.Clone() { throw null; }
        public override bool Equals(object obj) { throw null; }
        public override int GetHashCode() { throw null; }
        public static new System.Net.Http.Headers.NameValueWithParametersHeaderValue Parse(string input) { throw null; }
        public override string ToString() { throw null; }
        public static bool TryParse(string input, out System.Net.Http.Headers.NameValueWithParametersHeaderValue parsedValue) { throw null; }
    }
    public partial class ProductHeaderValue : System.ICloneable
    {
        public ProductHeaderValue(string name) { }
        public ProductHeaderValue(string name, string version) { }
        public string Name { get { throw null; } }
        public string Version { get { throw null; } }
        object System.ICloneable.Clone() { throw null; }
        public override bool Equals(object obj) { throw null; }
        public override int GetHashCode() { throw null; }
        public static System.Net.Http.Headers.ProductHeaderValue Parse(string input) { throw null; }
        public override string ToString() { throw null; }
        public static bool TryParse(string input, out System.Net.Http.Headers.ProductHeaderValue parsedValue) { throw null; }
    }
    public partial class ProductInfoHeaderValue : System.ICloneable
    {
        public ProductInfoHeaderValue(System.Net.Http.Headers.ProductHeaderValue product) { }
        public ProductInfoHeaderValue(string comment) { }
        public ProductInfoHeaderValue(string productName, string productVersion) { }
        public string Comment { get { throw null; } }
        public System.Net.Http.Headers.ProductHeaderValue Product { get { throw null; } }
        object System.ICloneable.Clone() { throw null; }
        public override bool Equals(object obj) { throw null; }
        public override int GetHashCode() { throw null; }
        public static System.Net.Http.Headers.ProductInfoHeaderValue Parse(string input) { throw null; }
        public override string ToString() { throw null; }
        public static bool TryParse(string input, out System.Net.Http.Headers.ProductInfoHeaderValue parsedValue) { throw null; }
    }
    public partial class RangeConditionHeaderValue : System.ICloneable
    {
        public RangeConditionHeaderValue(System.DateTimeOffset date) { }
        public RangeConditionHeaderValue(System.Net.Http.Headers.EntityTagHeaderValue entityTag) { }
        public RangeConditionHeaderValue(string entityTag) { }
        public System.Nullable<System.DateTimeOffset> Date { get { throw null; } }
        public System.Net.Http.Headers.EntityTagHeaderValue EntityTag { get { throw null; } }
        object System.ICloneable.Clone() { throw null; }
        public override bool Equals(object obj) { throw null; }
        public override int GetHashCode() { throw null; }
        public static System.Net.Http.Headers.RangeConditionHeaderValue Parse(string input) { throw null; }
        public override string ToString() { throw null; }
        public static bool TryParse(string input, out System.Net.Http.Headers.RangeConditionHeaderValue parsedValue) { throw null; }
    }
    public partial class RangeHeaderValue : System.ICloneable
    {
        public RangeHeaderValue() { }
        public RangeHeaderValue(System.Nullable<long> from, System.Nullable<long> to) { }
        public System.Collections.Generic.ICollection<System.Net.Http.Headers.RangeItemHeaderValue> Ranges { get { throw null; } }
        public string Unit { get { throw null; } set { } }
        object System.ICloneable.Clone() { throw null; }
        public override bool Equals(object obj) { throw null; }
        public override int GetHashCode() { throw null; }
        public static System.Net.Http.Headers.RangeHeaderValue Parse(string input) { throw null; }
        public override string ToString() { throw null; }
        public static bool TryParse(string input, out System.Net.Http.Headers.RangeHeaderValue parsedValue) { throw null; }
    }
    public partial class RangeItemHeaderValue : System.ICloneable
    {
        public RangeItemHeaderValue(System.Nullable<long> from, System.Nullable<long> to) { }
        public System.Nullable<long> From { get { throw null; } }
        public System.Nullable<long> To { get { throw null; } }
        object System.ICloneable.Clone() { throw null; }
        public override bool Equals(object obj) { throw null; }
        public override int GetHashCode() { throw null; }
        public override string ToString() { throw null; }
    }
    public partial class RetryConditionHeaderValue : System.ICloneable
    {
        public RetryConditionHeaderValue(System.DateTimeOffset date) { }
        public RetryConditionHeaderValue(System.TimeSpan delta) { }
        public System.Nullable<System.DateTimeOffset> Date { get { throw null; } }
        public System.Nullable<System.TimeSpan> Delta { get { throw null; } }
        object System.ICloneable.Clone() { throw null; }
        public override bool Equals(object obj) { throw null; }
        public override int GetHashCode() { throw null; }
        public static System.Net.Http.Headers.RetryConditionHeaderValue Parse(string input) { throw null; }
        public override string ToString() { throw null; }
        public static bool TryParse(string input, out System.Net.Http.Headers.RetryConditionHeaderValue parsedValue) { throw null; }
    }
    public partial class StringWithQualityHeaderValue : System.ICloneable
    {
        public StringWithQualityHeaderValue(string value) { }
        public StringWithQualityHeaderValue(string value, double quality) { }
        public System.Nullable<double> Quality { get { throw null; } }
        public string Value { get { throw null; } }
        object System.ICloneable.Clone() { throw null; }
        public override bool Equals(object obj) { throw null; }
        public override int GetHashCode() { throw null; }
        public static System.Net.Http.Headers.StringWithQualityHeaderValue Parse(string input) { throw null; }
        public override string ToString() { throw null; }
        public static bool TryParse(string input, out System.Net.Http.Headers.StringWithQualityHeaderValue parsedValue) { throw null; }
    }
    public partial class TransferCodingHeaderValue : System.ICloneable
    {
        protected TransferCodingHeaderValue(System.Net.Http.Headers.TransferCodingHeaderValue source) { }
        public TransferCodingHeaderValue(string value) { }
        public System.Collections.Generic.ICollection<System.Net.Http.Headers.NameValueHeaderValue> Parameters { get { throw null; } }
        public string Value { get { throw null; } }
        object System.ICloneable.Clone() { throw null; }
        public override bool Equals(object obj) { throw null; }
        public override int GetHashCode() { throw null; }
        public static System.Net.Http.Headers.TransferCodingHeaderValue Parse(string input) { throw null; }
        public override string ToString() { throw null; }
        public static bool TryParse(string input, out System.Net.Http.Headers.TransferCodingHeaderValue parsedValue) { throw null; }
    }
    public sealed partial class TransferCodingWithQualityHeaderValue : System.Net.Http.Headers.TransferCodingHeaderValue, System.ICloneable
    {
        public TransferCodingWithQualityHeaderValue(string value) : base(default(System.Net.Http.Headers.TransferCodingHeaderValue)) { }
        public TransferCodingWithQualityHeaderValue(string value, double quality) : base(default(System.Net.Http.Headers.TransferCodingHeaderValue)) { }
        public System.Nullable<double> Quality { get { throw null; } set { } }
        object System.ICloneable.Clone() { throw null; }
        public static new System.Net.Http.Headers.TransferCodingWithQualityHeaderValue Parse(string input) { throw null; }
        public static bool TryParse(string input, out System.Net.Http.Headers.TransferCodingWithQualityHeaderValue parsedValue) { throw null; }
    }
    public partial class ViaHeaderValue : System.ICloneable
    {
        public ViaHeaderValue(string protocolVersion, string receivedBy) { }
        public ViaHeaderValue(string protocolVersion, string receivedBy, string protocolName) { }
        public ViaHeaderValue(string protocolVersion, string receivedBy, string protocolName, string comment) { }
        public string Comment { get { throw null; } }
        public string ProtocolName { get { throw null; } }
        public string ProtocolVersion { get { throw null; } }
        public string ReceivedBy { get { throw null; } }
        object System.ICloneable.Clone() { throw null; }
        public override bool Equals(object obj) { throw null; }
        public override int GetHashCode() { throw null; }
        public static System.Net.Http.Headers.ViaHeaderValue Parse(string input) { throw null; }
        public override string ToString() { throw null; }
        public static bool TryParse(string input, out System.Net.Http.Headers.ViaHeaderValue parsedValue) { throw null; }
    }
    public partial class WarningHeaderValue : System.ICloneable
    {
        public WarningHeaderValue(int code, string agent, string text) { }
        public WarningHeaderValue(int code, string agent, string text, System.DateTimeOffset date) { }
        public string Agent { get { throw null; } }
        public int Code { get { throw null; } }
        public System.Nullable<System.DateTimeOffset> Date { get { throw null; } }
        public string Text { get { throw null; } }
        object System.ICloneable.Clone() { throw null; }
        public override bool Equals(object obj) { throw null; }
        public override int GetHashCode() { throw null; }
        public static System.Net.Http.Headers.WarningHeaderValue Parse(string input) { throw null; }
        public override string ToString() { throw null; }
        public static bool TryParse(string input, out System.Net.Http.Headers.WarningHeaderValue parsedValue) { throw null; }
    }
}
