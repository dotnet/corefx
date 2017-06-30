﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Concurrent;
using System.IO;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http
{
    internal sealed class HttpProxyConnectionHandler : HttpMessageHandler
    {
        private readonly IWebProxy _proxy;
        private readonly HttpMessageHandler _innerHandler;

        private readonly ConcurrentDictionary<HttpConnectionKey, HttpConnectionPool> _connectionPoolTable;
        private bool _disposed;

        public HttpProxyConnectionHandler(IWebProxy proxy, HttpMessageHandler innerHandler)
        {
            _proxy = proxy ?? throw new ArgumentNullException(nameof(proxy));
            _innerHandler = innerHandler ?? throw new ArgumentNullException(nameof(innerHandler));
            _connectionPoolTable = new ConcurrentDictionary<HttpConnectionKey, HttpConnectionPool>();
        }

        protected internal override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Uri proxyUri = null;
            try
            {
                if (!_proxy.IsBypassed(request.RequestUri))
                {
                    proxyUri = _proxy.GetProxy(request.RequestUri);
                }
            }
            catch (Exception)
            {
                // Eat any exception from the IWebProxy and just treat it as no proxy.
                // TODO: This seems a bit questionable, but it's what the tests expect
            }

            return proxyUri == null ?
                _innerHandler.SendAsync(request, cancellationToken) :
                SendWithProxyAsync(proxyUri, request, cancellationToken);
        }

        private async Task<HttpResponseMessage> SendWithProxyAsync(
            Uri proxyUri, HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (proxyUri.Scheme != UriScheme.Http)
            {
                throw new InvalidOperationException($"invalid scheme {proxyUri.Scheme} for proxy");
            }

            if (request.RequestUri.Scheme == UriScheme.Https)
            {
                // TODO: Implement SSL tunneling through proxy
                throw new NotImplementedException("no support for SSL tunneling through proxy");
            }

            HttpConnection connection = await GetOrCreateConnection(request, proxyUri).ConfigureAwait(false);

            HttpResponseMessage response = await connection.SendAsync(request, cancellationToken).ConfigureAwait(false);

            // Handle proxy authentication
            if (response.StatusCode == HttpStatusCode.ProxyAuthenticationRequired &&
                _proxy.Credentials != null)
            {
                foreach (AuthenticationHeaderValue h in response.Headers.ProxyAuthenticate)
                {
                    // We only support Basic auth, ignore others
                    const string Basic = "Basic";
                    if (h.Scheme == Basic)
                    {
                        NetworkCredential credential = _proxy.Credentials.GetCredential(proxyUri, Basic);
                        if (credential != null)
                        {
                            response.Dispose();

                            request.Headers.ProxyAuthorization = new AuthenticationHeaderValue(Basic,
                                BasicAuthenticationHelper.GetBasicTokenForCredential(credential));

                            connection = await GetOrCreateConnection(request, proxyUri).ConfigureAwait(false);
                            response = await connection.SendAsync(request, cancellationToken).ConfigureAwait(false);
                        }

                        break;
                    }
                }
            }

            return response;
        }

        private async ValueTask<HttpConnection> GetOrCreateConnection(HttpRequestMessage request, Uri proxyUri)
        {
            HttpConnectionKey key = new HttpConnectionKey(proxyUri);

            HttpConnectionPool pool;
            if (_connectionPoolTable.TryGetValue(key, out pool))
            {
                HttpConnection poolConnection = pool.GetConnection();
                if (poolConnection != null)
                {
                    return poolConnection;
                }
            }

            Stream stream = await ConnectHelper.ConnectAsync(proxyUri.Host, proxyUri.Port).ConfigureAwait(false);

            if (pool == null)
            {
                pool = _connectionPoolTable.GetOrAdd(key, _ => new HttpConnectionPool());
            }

            return new HttpConnection(pool, key, stream, null, true);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                _disposed = true;

                // Close all open connections
                // TODO: There's a timing issue here
                // Revisit when we improve the connection pooling implementation
                foreach (HttpConnectionPool connectionPool in _connectionPoolTable.Values)
                {
                    connectionPool.Dispose();
                }
            }

            base.Dispose(disposing);
        }
    }
}
