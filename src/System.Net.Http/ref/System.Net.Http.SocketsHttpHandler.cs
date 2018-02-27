// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------


namespace System.Net.Http
{
    public sealed class SocketsHttpHandler : HttpMessageHandler
    {
        public SocketsHttpHandler() { }
        public bool AllowAutoRedirect { get { throw null; } set { } }
        public System.Net.DecompressionMethods AutomaticDecompression { get { throw null; } set { } }
        public System.TimeSpan ConnectTimeout { get; set; }
        public System.Net.CookieContainer CookieContainer { get { throw null; } set { } }
        public System.Net.ICredentials Credentials { get { throw null; } set { } }
        public System.Net.ICredentials DefaultProxyCredentials { get { throw null; } set { } }
        public System.TimeSpan Expect100ContinueTimeout { get; set; }
        public int MaxAutomaticRedirections { get { throw null; } set { } }
        public int MaxConnectionsPerServer { get { throw null; } set { } }
        public int MaxResponseDrainSize { get { throw null; } set { } }
        public int MaxResponseHeadersLength { get { throw null; } set { } }
        public bool PreAuthenticate { get { throw null; } set { } }
        public System.TimeSpan PooledConnectionIdleTimeout { get { throw null; } set { } }
        public System.TimeSpan PooledConnectionLifetime { get { throw null; } set { } }
        public System.Collections.Generic.IDictionary<string, object> Properties { get { throw null; } }
        public System.Net.IWebProxy Proxy { get { throw null; } set { } }
        public System.Net.Security.SslClientAuthenticationOptions SslOptions { get { throw null; } set { } }
        public bool UseCookies { get { throw null; } set { } }
        public bool UseProxy { get { throw null; } set { } }
        protected override void Dispose(bool disposing) { }
        protected internal override System.Threading.Tasks.Task<System.Net.Http.HttpResponseMessage> SendAsync(System.Net.Http.HttpRequestMessage request, System.Threading.CancellationToken cancellationToken) { throw null; }
    }
}
