// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Net.Security;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http
{
    public sealed class SocketsHttpHandler : HttpMessageHandler
    {
        private readonly HttpConnectionSettings _settings = new HttpConnectionSettings();
        private HttpMessageHandler _handler;
        private bool _disposed;

        private void CheckDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(SocketsHttpHandler));
            }
        }

        private void CheckDisposedOrStarted()
        {
            CheckDisposed();
            if (_handler != null)
            {
                throw new InvalidOperationException(SR.net_http_operation_started);
            }
        }

        public bool UseCookies
        {
            get => _settings._useCookies;
            set
            {
                CheckDisposedOrStarted();
                _settings._useCookies = value;
            }
        }

        public CookieContainer CookieContainer
        {
            get => _settings._cookieContainer ?? (_settings._cookieContainer = new CookieContainer());
            set
            {
                CheckDisposedOrStarted();
                _settings._cookieContainer = value;
            }
        }

        public DecompressionMethods AutomaticDecompression
        {
            get => _settings._automaticDecompression;
            set
            {
                CheckDisposedOrStarted();
                _settings._automaticDecompression = value;
            }
        }

        public bool UseProxy
        {
            get => _settings._useProxy;
            set
            {
                CheckDisposedOrStarted();
                _settings._useProxy = value;
            }
        }

        public IWebProxy Proxy
        {
            get => _settings._proxy;
            set
            {
                CheckDisposedOrStarted();
                _settings._proxy = value;
            }
        }

        public ICredentials DefaultProxyCredentials
        {
            get => _settings._defaultProxyCredentials;
            set
            {
                CheckDisposedOrStarted();
                _settings._defaultProxyCredentials = value;
            }
        }

        public bool PreAuthenticate
        {
            get => _settings._preAuthenticate;
            set
            {
                CheckDisposedOrStarted();
                _settings._preAuthenticate = value;
            }
        }

        public ICredentials Credentials
        {
            get => _settings._credentials;
            set
            {
                CheckDisposedOrStarted();
                _settings._credentials = value;
            }
        }

        public bool AllowAutoRedirect
        {
            get => _settings._allowAutoRedirect;
            set
            {
                CheckDisposedOrStarted();
                _settings._allowAutoRedirect = value;
            }
        }

        public int MaxAutomaticRedirections
        {
            get => _settings._maxAutomaticRedirections;
            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, SR.Format(SR.net_http_value_must_be_greater_than, 0));
                }

                CheckDisposedOrStarted();
                _settings._maxAutomaticRedirections = value;
            }
        }

        public int MaxConnectionsPerServer
        {
            get => _settings._maxConnectionsPerServer;
            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, SR.Format(SR.net_http_value_must_be_greater_than, 0));
                }

                CheckDisposedOrStarted();
                _settings._maxConnectionsPerServer = value;
            }
        }

        public int MaxResponseHeadersLength
        {
            get => _settings._maxResponseHeadersLength;
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, SR.Format(SR.net_http_value_must_be_greater_than, 0));
                }

                CheckDisposedOrStarted();
                _settings._maxResponseHeadersLength = value;
            }
        }

        public SslClientAuthenticationOptions SslOptions
        {
            get => _settings._sslOptions ?? (_settings._sslOptions = new SslClientAuthenticationOptions());
            set
            {
                CheckDisposedOrStarted();
                _settings._sslOptions = value;
            }
        }

        public TimeSpan PooledConnectionLifetime
        {
            get => _settings._pooledConnectionLifetime;
            set
            {
                if (value < TimeSpan.Zero && value != Timeout.InfiniteTimeSpan)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                CheckDisposedOrStarted();
                _settings._pooledConnectionLifetime = value;
            }
        }

        public TimeSpan PooledConnectionIdleTimeout
        {
            get => _settings._pooledConnectionIdleTimeout;
            set
            {
                if (value < TimeSpan.Zero && value != Timeout.InfiniteTimeSpan)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                CheckDisposedOrStarted();
                _settings._pooledConnectionIdleTimeout = value;
            }
        }

        public IDictionary<string, object> Properties =>
            _settings._properties ?? (_settings._properties = new Dictionary<string, object>());

        protected override void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                _disposed = true;
                _handler?.Dispose();
            }

            base.Dispose(disposing);
        }

        private HttpMessageHandler SetupHandlerChain()
        {
            // Clone the settings to get a relatively consistent view that won't change after this point.
            // (This isn't entirely complete, as some of the collections it contains aren't currently deeply cloned.)
            HttpConnectionSettings settings = _settings.Clone();

            HttpMessageHandler handler = new HttpConnectionHandler(settings);

            if (settings._useProxy && (settings._proxy != null || HttpProxyConnectionHandler.DefaultProxyConfigured))
            {
                handler = new HttpProxyConnectionHandler(settings, handler);
            }

            if (settings._useCookies)
            {
                handler = new CookieHandler(CookieContainer, handler);
            }

            if (settings._credentials != null || settings._allowAutoRedirect)
            {
                handler = new AuthenticateAndRedirectHandler(settings._preAuthenticate, settings._credentials, settings._allowAutoRedirect, settings._maxAutomaticRedirections, handler);
            }

            if (settings._automaticDecompression != DecompressionMethods.None)
            {
                handler = new DecompressionHandler(settings._automaticDecompression, handler);
            }

            // Ensure a single handler is used for all requests.
            if (Interlocked.CompareExchange(ref _handler, handler, null) != null)
            {
                handler.Dispose();
            }

            return _handler;
        }

        protected internal override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            CheckDisposed();
            HttpMessageHandler handler = _handler ?? SetupHandlerChain();

            if (request.Version.Major == 0)
            {
                return Task.FromException<HttpResponseMessage>(new NotSupportedException(SR.net_http_unsupported_version));
            }

            // Add headers to define content transfer, if not present
            if (request.Content != null &&
                (!request.HasHeaders || request.Headers.TransferEncodingChunked != true) &&
                request.Content.Headers.ContentLength == null)
            {
                // We have content, but neither Transfer-Encoding or Content-Length is set.
                request.Headers.TransferEncodingChunked = true;
            }

            if (request.Version.Minor == 0 && request.Version.Major == 1 && request.HasHeaders)
            {
                // HTTP 1.0 does not support chunking
                if (request.Headers.TransferEncodingChunked == true)
                {
                    return Task.FromException<HttpResponseMessage>(new NotSupportedException(SR.net_http_unsupported_chunking));
                }

                // HTTP 1.0 does not support Expect: 100-continue; just disable it.
                if (request.Headers.ExpectContinue == true)
                {
                    request.Headers.ExpectContinue = false;
                }
            }

            return handler.SendAsync(request, cancellationToken);
        }
    }
}
