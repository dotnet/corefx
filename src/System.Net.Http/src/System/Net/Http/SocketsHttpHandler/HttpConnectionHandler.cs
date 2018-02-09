// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http
{
    internal sealed class HttpConnectionHandler : HttpMessageHandler
    {
        private readonly HttpConnectionPools _connectionPools;

        public HttpConnectionHandler(HttpConnectionSettings settings)
        {
            _connectionPools = new HttpConnectionPools(settings, settings._maxConnectionsPerServer, usingProxy: false);
        }

        protected internal override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Uri uri = request.RequestUri;
            HttpConnectionKey key = new HttpConnectionKey(uri.IdnHost, uri.Port, ConnectHelper.GetSslHostName(request));
            HttpConnectionPool pool = _connectionPools.GetOrAddPool(key);
            return pool.SendAsync(request, cancellationToken);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _connectionPools.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
