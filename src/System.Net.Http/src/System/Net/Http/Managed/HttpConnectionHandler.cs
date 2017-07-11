﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Concurrent;
using System.IO;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http
{
    internal sealed class HttpConnectionHandler : HttpMessageHandler
    {
        private readonly X509CertificateCollection _clientCertificates;
        private readonly Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool> _serverCertificateCustomValidationCallback;
        private readonly bool _checkCertificateRevocationList;
        private readonly SslProtocols _sslProtocols;

        private readonly ConcurrentDictionary<HttpConnectionKey, HttpConnectionPool> _connectionPoolTable;
        private bool _disposed;

        public HttpConnectionHandler(
            X509CertificateCollection clientCertificates,
            Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool> serverCertificateCustomValidationCallback,
            bool checkCertificateRevocationList,
            SslProtocols sslProtocols)
        {
            _clientCertificates = clientCertificates;
            _serverCertificateCustomValidationCallback = serverCertificateCustomValidationCallback;
            _checkCertificateRevocationList = checkCertificateRevocationList;
            _sslProtocols = sslProtocols;

            _connectionPoolTable = new ConcurrentDictionary<HttpConnectionKey, HttpConnectionPool>();
        }

        protected internal override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpConnection connection = null;

            // Try to get a connection from the connection pool
            HttpConnectionKey key = new HttpConnectionKey(request.RequestUri);
            if (_connectionPoolTable.TryGetValue(key, out HttpConnectionPool pool))
            {
                connection = pool.GetConnection();
            }

            return connection != null ?
                connection.SendAsync(request, cancellationToken) :
                SendAsyncWithNewConnection(key, pool, request, cancellationToken);
        }

        private async Task<HttpResponseMessage> SendAsyncWithNewConnection(
            HttpConnectionKey key, HttpConnectionPool pool,
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var connection = await CreateConnection(request, key, pool).ConfigureAwait(false);
            return await connection.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }

        private async ValueTask<SslStream> EstablishSslConnection(string host, HttpRequestMessage request, Stream stream)
        {
            RemoteCertificateValidationCallback callback = null;
            if (_serverCertificateCustomValidationCallback != null)
            {
                callback = (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) =>
                {
                    return _serverCertificateCustomValidationCallback(request, certificate as X509Certificate2, chain, sslPolicyErrors);
                };
            }

            SslStream sslStream = new SslStream(stream, false, callback);

            try
            {
                // TODO #21452: No cancellationToken?
                await sslStream.AuthenticateAsClientAsync(host, _clientCertificates, _sslProtocols, _checkCertificateRevocationList).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                sslStream.Dispose();
                if (e is AuthenticationException || e is IOException)
                {
                    // TODO #21452: Tests expect HttpRequestException here.  Is that correct behavior?
                    throw new HttpRequestException("could not establish SSL connection", e);
                }
                throw;
            }

            return sslStream;
        }

        private async ValueTask<HttpConnection> CreateConnection(HttpRequestMessage request, HttpConnectionKey key, HttpConnectionPool pool)
        {
            Uri uri = request.RequestUri;

            Stream stream = await ConnectHelper.ConnectAsync(uri.IdnHost, uri.Port).ConfigureAwait(false);

            TransportContext transportContext = null;

            if (uri.Scheme == UriScheme.Https)
            {
                SslStream sslStream = await EstablishSslConnection(uri.IdnHost, request, stream).ConfigureAwait(false);

                stream = sslStream;
                transportContext = sslStream.TransportContext;
            }

            if (pool == null)
            {
                pool = _connectionPoolTable.GetOrAdd(key, _ => new HttpConnectionPool());
            }

            var connection = new HttpConnection(pool, key, uri.IdnHost, stream, transportContext, false);

            return connection;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                _disposed = true;

                // Close all open connections
                // TODO #21452: There's a timing issue here
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
