// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http
{
    internal sealed class ManagedHandler : HttpMessageHandler
    {
        // Configuration settings
        private bool _useCookies = HttpHandlerDefaults.DefaultUseCookies;
        private CookieContainer _cookieContainer;
        private ClientCertificateOption _clientCertificateOptions = HttpHandlerDefaults.DefaultClientCertificateOption;
        private DecompressionMethods _automaticDecompression = HttpHandlerDefaults.DefaultAutomaticDecompression;
        private bool _useProxy = HttpHandlerDefaults.DefaultUseProxy;
        private IWebProxy _proxy;
        private ICredentials _defaultProxyCredentials;
        private bool _preAuthenticate = HttpHandlerDefaults.DefaultPreAuthenticate;
        private bool _useDefaultCredentials = HttpHandlerDefaults.DefaultUseDefaultCredentials;
        private ICredentials _credentials;
        private bool _allowAutoRedirect = HttpHandlerDefaults.DefaultAutomaticRedirection;
        private int _maxAutomaticRedirections = HttpHandlerDefaults.DefaultMaxAutomaticRedirections;
        private int _maxResponseHeadersLength = HttpHandlerDefaults.DefaultMaxResponseHeadersLength;
        private int _maxConnectionsPerServer = HttpHandlerDefaults.DefaultMaxConnectionsPerServer;
        private X509CertificateCollection _clientCertificates;
        private Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool> _serverCertificateCustomValidationCallback;
        private bool _checkCertificateRevocationList = false;
        private SslProtocols _sslProtocols = SslProtocols.None;
        private IDictionary<string, object> _properties;

        private HttpMessageHandler _handler;
        private bool _disposed;

        private void CheckInUse()
        {
            // Can't set props once in use
            if (_handler != null)
            {
                throw new InvalidOperationException();
            }
        }

        public bool SupportsAutomaticDecompression
        {
            get { return true; }
        }

        public bool SupportsProxy
        {
            get { return true; }
        }

        public bool SupportsRedirectConfiguration
        {
            get { return true; }
        }

        public bool UseCookies
        {
            get { return _useCookies; }
            set { CheckInUse(); _useCookies = value; }
        }

        public CookieContainer CookieContainer
        {
            get
            {
                if (_cookieContainer == null)
                {
                    _cookieContainer = new CookieContainer();
                }

                return _cookieContainer;
            }
            set { CheckInUse(); _cookieContainer = value; }
        }

        public ClientCertificateOption ClientCertificateOptions
        {
            get { return _clientCertificateOptions; }
            set
            {
                CheckInUse();
                if (value == ClientCertificateOption.Automatic || value == ClientCertificateOption.Manual)
                {
                    _clientCertificateOptions = value;
                    return;
                }

                throw new ArgumentOutOfRangeException(nameof(value));
            }
        }

        public DecompressionMethods AutomaticDecompression
        {
            get { return _automaticDecompression; }
            set { CheckInUse(); _automaticDecompression = value; }
        }

        public bool UseProxy
        {
            get { return _useProxy; }
            set { CheckInUse(); _useProxy = value; }
        }

        public IWebProxy Proxy
        {
            get { return _proxy; }
            set { CheckInUse(); _proxy = value; }
        }

        public ICredentials DefaultProxyCredentials
        {
            get { return _defaultProxyCredentials; }
            set { CheckInUse(); _defaultProxyCredentials = value; }
        }

        public bool PreAuthenticate
        {
            get { return _preAuthenticate; }
            set { CheckInUse(); _preAuthenticate = value; }
        }

        public bool UseDefaultCredentials
        {
            get { return _useDefaultCredentials; }
            set { CheckInUse(); _useDefaultCredentials = value; }
        }

        public ICredentials Credentials
        {
            get { return _credentials; }
            set { CheckInUse(); _credentials = value; }
        }

        public bool AllowAutoRedirect
        {
            get { return _allowAutoRedirect; }
            set { CheckInUse(); _allowAutoRedirect = value; }
        }

        public int MaxAutomaticRedirections
        {
            get { return _maxAutomaticRedirections; }
            set { CheckInUse(); _maxAutomaticRedirections = value; }
        }

        public int MaxConnectionsPerServer
        {
            get { return _maxConnectionsPerServer; }
            set
            {
                CheckInUse();
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                _maxConnectionsPerServer = value;
            }
        }

        public int MaxResponseHeadersLength
        {
            get { return _maxResponseHeadersLength; }
            set
            {
                CheckInUse();
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                _maxResponseHeadersLength = value;
            }
        }

        public X509CertificateCollection ClientCertificates
        {
            get
            {
                if (_clientCertificateOptions != ClientCertificateOption.Manual)
                {
                    throw new InvalidOperationException(SR.Format(SR.net_http_invalid_enable_first, nameof(ClientCertificateOptions), nameof(ClientCertificateOption.Manual)));
                }

                return _clientCertificates ?? (_clientCertificates = new X509Certificate2Collection());
            }
        }

        public Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool> ServerCertificateCustomValidationCallback
        {
            get { return _serverCertificateCustomValidationCallback; }
            set { CheckInUse(); _serverCertificateCustomValidationCallback = value; }
        }

        public bool CheckCertificateRevocationList
        {
            get { return _checkCertificateRevocationList; }
            set { CheckInUse(); _checkCertificateRevocationList = value; }
        }

        public SslProtocols SslProtocols
        {
            get { return _sslProtocols; }
            set
            {
                CheckInUse();
#pragma warning disable 0618 // obsolete warning
                if ((value & (SslProtocols.Ssl2 | SslProtocols.Ssl3)) != 0)
                {
                    throw new NotSupportedException("unsupported SSL protocols");
                }
#pragma warning restore 0618

                _sslProtocols = value;
            }
        }

        public IDictionary<String, object> Properties
        {
            get
            {
                if (_properties == null)
                {
                    _properties = new Dictionary<string, object>();
                }

                return _properties;
            }
        }

        protected override void Dispose(bool disposing)
        {

            if (disposing && !_disposed)
            {
                _disposed = true;

                _handler?.Dispose();
            }

            base.Dispose(disposing);
        }

        private void SetupHandlerChain()
        {
            Debug.Assert(_handler == null);

            HttpMessageHandler handler = new HttpConnectionHandler(
                _clientCertificates,
                _serverCertificateCustomValidationCallback,
                _checkCertificateRevocationList,
                _sslProtocols);

            if (_useProxy && _proxy != null)
            {
                handler = new HttpProxyConnectionHandler(_proxy, handler);
            }

            if (_credentials != null)
            {
                handler = new AuthenticationHandler(_preAuthenticate, _credentials, handler);
            }

            if (_useCookies)
            {
                handler = new CookieHandler(CookieContainer, handler);
            }

            if (_allowAutoRedirect)
            {
                handler = new AutoRedirectHandler(_maxAutomaticRedirections, handler);
            }

            if (_automaticDecompression != DecompressionMethods.None)
            {
                handler = new DecompressionHandler(_automaticDecompression, handler);
            }

            _handler = handler;
        }

        protected internal override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(ManagedHandler));
            }

            if (_handler == null)
            {
                SetupHandlerChain();
            }

            return _handler.SendAsync(request, cancellationToken);
        }
    }
}
