// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Security;
using System.Runtime.CompilerServices;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

using CURLAUTH = Interop.Http.CURLAUTH;
using CURLcode = Interop.Http.CURLcode;
using CURLMcode = Interop.Http.CURLMcode;
using CURLoption = Interop.Http.CURLoption;

namespace System.Net.Http
{
    internal partial class CurlHandler : HttpMessageHandler
    {
        #region Constants

        private const char SpaceChar = ' ';
        private const int StatusCodeLength = 3;

        private const string UriSchemeHttp = "http";
        private const string UriSchemeHttps = "https";
        private const string EncodingNameGzip = "gzip";
        private const string EncodingNameDeflate = "deflate";
        
        private const int MaxRequestBufferSize = 16384; // Default used by libcurl
        private const string NoTransferEncoding = HttpKnownHeaderNames.TransferEncoding + ":";
        private const string NoContentType = HttpKnownHeaderNames.ContentType + ":";
        private const int CurlAge = 5;
        private const int MinCurlAge = 3;

        #endregion

        #region Fields

        private static readonly KeyValuePair<string,CURLAUTH>[] s_orderedAuthTypes = new KeyValuePair<string, CURLAUTH>[] {
            new KeyValuePair<string,CURLAUTH>("Negotiate", CURLAUTH.Negotiate),
            new KeyValuePair<string,CURLAUTH>("NTLM", CURLAUTH.NTLM),
            new KeyValuePair<string,CURLAUTH>("Digest", CURLAUTH.Digest),
            new KeyValuePair<string,CURLAUTH>("Basic", CURLAUTH.Basic),
        };

        // Max timeout value used by WinHttp handler, so mapping to that here.
        private static readonly TimeSpan s_maxTimeout = TimeSpan.FromMilliseconds(int.MaxValue);

        private static readonly char[] s_newLineCharArray = new char[] { HttpRuleParser.CR, HttpRuleParser.LF };
        private static readonly bool s_supportsAutomaticDecompression;
        private static readonly bool s_supportsSSL;
        private static readonly bool s_supportsHttp2Multiplexing;
        private static volatile StrongBox<CURLMcode> s_supportsMaxConnectionsPerServer;
        private static string s_curlVersionDescription;
        private static string s_curlSslVersionDescription;

        private static readonly DiagnosticListener s_diagnosticListener = new DiagnosticListener(HttpHandlerLoggingStrings.DiagnosticListenerName);

        private readonly MultiAgent _agent;
        private volatile bool _anyOperationStarted;
        private volatile bool _disposed;

        private IWebProxy _proxy = null;
        private ICredentials _serverCredentials = null;
        private bool _useProxy = HttpHandlerDefaults.DefaultUseProxy;
        private ICredentials _defaultProxyCredentials = CredentialCache.DefaultCredentials;
        private DecompressionMethods _automaticDecompression = HttpHandlerDefaults.DefaultAutomaticDecompression;
        private bool _preAuthenticate = HttpHandlerDefaults.DefaultPreAuthenticate;
        private CredentialCache _credentialCache = null; // protected by LockObject
        private CookieContainer _cookieContainer = new CookieContainer();
        private bool _useCookie = HttpHandlerDefaults.DefaultUseCookies;
        private TimeSpan _connectTimeout = Timeout.InfiniteTimeSpan;
        private bool _automaticRedirection = HttpHandlerDefaults.DefaultAutomaticRedirection;
        private int _maxAutomaticRedirections = HttpHandlerDefaults.DefaultMaxAutomaticRedirections;
        private int _maxConnectionsPerServer = HttpHandlerDefaults.DefaultMaxConnectionsPerServer;
        private int _maxResponseHeadersLength = HttpHandlerDefaults.DefaultMaxResponseHeaderLength;
        private ClientCertificateOption _clientCertificateOption = HttpHandlerDefaults.DefaultClientCertificateOption;
        private X509Certificate2Collection _clientCertificates;
        private Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool> _serverCertificateValidationCallback;
        private bool _checkCertificateRevocationList;
        private SslProtocols _sslProtocols = SecurityProtocol.DefaultSecurityProtocols;

        private object LockObject { get { return _agent; } }

        #endregion        

        static CurlHandler()
        {
            // curl_global_init call handled by Interop.LibCurl's cctor

            Interop.Http.CurlFeatures features = Interop.Http.GetSupportedFeatures();
            s_supportsSSL = (features & Interop.Http.CurlFeatures.CURL_VERSION_SSL) != 0;
            s_supportsAutomaticDecompression = (features & Interop.Http.CurlFeatures.CURL_VERSION_LIBZ) != 0;
            s_supportsHttp2Multiplexing = (features & Interop.Http.CurlFeatures.CURL_VERSION_HTTP2) != 0 && Interop.Http.GetSupportsHttp2Multiplexing();

            if (HttpEventSource.Log.IsEnabled())
            {
                EventSourceTrace($"libcurl: {CurlVersionDescription} {CurlSslVersionDescription} {features}");
            }
        }

        public CurlHandler()
        {
            _agent = new MultiAgent(this);
        }

        #region Properties

        private static string CurlVersionDescription => s_curlVersionDescription ?? (s_curlVersionDescription = Interop.Http.GetVersionDescription() ?? string.Empty);
        private static string CurlSslVersionDescription => s_curlSslVersionDescription ?? (s_curlSslVersionDescription = Interop.Http.GetSslVersionDescription() ?? string.Empty);

        internal bool AutomaticRedirection
        {
            get { return _automaticRedirection; }
            set
            {
                CheckDisposedOrStarted();
                _automaticRedirection = value;
            }
        }

        internal bool SupportsProxy => true;

        internal bool SupportsRedirectConfiguration => true;

        internal bool UseProxy
        {
            get { return _useProxy; }
            set
            {
                CheckDisposedOrStarted();
                _useProxy = value;
            }
        }

        internal IWebProxy Proxy
        {
            get { return _proxy; }
            set
            {
                CheckDisposedOrStarted();
                _proxy = value;
            }
        }

        internal ICredentials DefaultProxyCredentials
        {
            get { return _defaultProxyCredentials; }
            set
            {
                CheckDisposedOrStarted();
                _defaultProxyCredentials = value;
            }
        }

        internal ICredentials Credentials
        {
            get { return _serverCredentials; }
            set { _serverCredentials = value; }
        }

        internal ClientCertificateOption ClientCertificateOptions
        {
            get { return _clientCertificateOption; }
            set
            {
                CheckDisposedOrStarted();
                _clientCertificateOption = value;
            }
        }

        internal X509Certificate2Collection ClientCertificates => _clientCertificates ?? (_clientCertificates = new X509Certificate2Collection());

        internal Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool> ServerCertificateValidationCallback
        {
            get { return _serverCertificateValidationCallback; }
            set
            {
                CheckDisposedOrStarted();
                _serverCertificateValidationCallback = value;
            }
        }

        internal bool CheckCertificateRevocationList
        {
            get { return _checkCertificateRevocationList; }
            set
            {
                CheckDisposedOrStarted();
                _checkCertificateRevocationList = value;
            }
        }

        internal SslProtocols SslProtocols
        {
            get { return _sslProtocols; }
            set
            {
                SecurityProtocol.ThrowOnNotAllowed(value, allowNone: false);
                CheckDisposedOrStarted();
                _sslProtocols = value;
            }
        }

        internal bool SupportsAutomaticDecompression => s_supportsAutomaticDecompression;

        internal DecompressionMethods AutomaticDecompression
        {
            get { return _automaticDecompression; }
            set
            {
                CheckDisposedOrStarted();
                _automaticDecompression = value;
            }
        }

        internal bool PreAuthenticate
        {
            get { return _preAuthenticate; }
            set
            {
                CheckDisposedOrStarted();
                _preAuthenticate = value;
                if (value && _credentialCache == null)
                {
                    _credentialCache = new CredentialCache();
                }
            }
        }

        internal bool UseCookie
        {
            get { return _useCookie; }
            set
            {               
                CheckDisposedOrStarted();
                _useCookie = value;
            }
        }

        internal CookieContainer CookieContainer
        {
            get { return _cookieContainer; }
            set
            {
                CheckDisposedOrStarted();
                _cookieContainer = value;
            }
        }

        public TimeSpan ConnectTimeout
        {
            get { return _connectTimeout; }
            set
            {
                if (value != Timeout.InfiniteTimeSpan && (value <= TimeSpan.Zero || value > s_maxTimeout))
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                CheckDisposedOrStarted();
                _connectTimeout = value;
            }
        }

        internal int MaxAutomaticRedirections
        {
            get { return _maxAutomaticRedirections; }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, SR.Format(SR.net_http_value_must_be_greater_than, 0));
                }

                CheckDisposedOrStarted();
                _maxAutomaticRedirections = value;
            }
        }

        internal int MaxConnectionsPerServer
        {
            get { return _maxConnectionsPerServer; }
            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, SR.Format(SR.net_http_value_must_be_greater_than, 0));
                }

                // Make sure the libcurl version we're using supports the option, by setting the value on a temporary multi handle.
                // We do this once and cache the result.
                StrongBox<CURLMcode> supported = s_supportsMaxConnectionsPerServer; // benign race condition to read and set this
                if (supported == null)
                {
                    using (Interop.Http.SafeCurlMultiHandle multiHandle = Interop.Http.MultiCreate())
                    {
                        s_supportsMaxConnectionsPerServer = supported = new StrongBox<CURLMcode>(
                            Interop.Http.MultiSetOptionLong(multiHandle, Interop.Http.CURLMoption.CURLMOPT_MAX_HOST_CONNECTIONS, value));
                    }
                }
                if (supported.Value != CURLMcode.CURLM_OK)
                {
                    throw new PlatformNotSupportedException(CurlException.GetCurlErrorString((int)supported.Value, isMulti: true));
                }

                CheckDisposedOrStarted();
                _maxConnectionsPerServer = value;
            }
        }

        internal int MaxResponseHeadersLength
        {
            get { return _maxResponseHeadersLength; }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, SR.Format(SR.net_http_value_must_be_greater_than, 0));
                }

                CheckDisposedOrStarted();
                _maxResponseHeadersLength = value;
            }
        }

        internal bool UseDefaultCredentials
        {
            get { return false; }
            set { }
        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            _disposed = true;
            if (disposing)
            {
                _agent.Dispose();
            }
            base.Dispose(disposing);
        }

        protected internal override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request), SR.net_http_handler_norequest);
            }

            if (request.RequestUri.Scheme == UriSchemeHttps)
            {
                if (!s_supportsSSL)
                {
                    throw new PlatformNotSupportedException(SR.Format(SR.net_http_unix_https_support_unavailable_libcurl, CurlVersionDescription));
                }
            }
            else
            {
                Debug.Assert(request.RequestUri.Scheme == UriSchemeHttp, "HttpClient expected to validate scheme as http or https.");
            }

            if (request.Headers.TransferEncodingChunked.GetValueOrDefault() && (request.Content == null))
            {
                throw new InvalidOperationException(SR.net_http_chunked_not_allowed_with_empty_content);
            }

            if (_useCookie && _cookieContainer == null)
            {
                throw new InvalidOperationException(SR.net_http_invalid_cookiecontainer);
            }

            Guid loggingRequestId = s_diagnosticListener.LogHttpRequest(request);

            CheckDisposed();
            SetOperationStarted();

            // Do an initial cancellation check to avoid initiating the async operation if
            // cancellation has already been requested.  After this, we'll rely on CancellationToken.Register
            // to notify us of cancellation requests and shut down the operation if possible.
            if (cancellationToken.IsCancellationRequested)
            {
                return Task.FromCanceled<HttpResponseMessage>(cancellationToken);
            }

            EventSourceTrace("{0}", request);

            // Create the easy request.  This associates the easy request with this handler and configures
            // it based on the settings configured for the handler.
            var easy = new EasyRequest(this, request, cancellationToken);
            try
            {
                easy.InitializeCurl();
                easy._requestContentStream?.Run();
                _agent.Queue(new MultiAgent.IncomingRequest { Easy = easy, Type = MultiAgent.IncomingRequestType.New });
            }
            catch (Exception exc)
            {
                easy.FailRequest(exc);
                easy.Cleanup(); // no active processing remains, so we can cleanup
            }

            s_diagnosticListener.LogHttpResponse(easy.Task, loggingRequestId);

            return easy.Task;
        }

        #region Private methods

        private void SetOperationStarted()
        {
            if (!_anyOperationStarted)
            {
                _anyOperationStarted = true;
            }
        }

        private KeyValuePair<NetworkCredential, CURLAUTH> GetCredentials(Uri requestUri)
        {
            // If preauthentication is enabled, we may have populated our internal credential cache,
            // so first check there to see if we have any credentials for this uri.
            if (_preAuthenticate)
            {
                KeyValuePair<NetworkCredential, CURLAUTH> ncAndScheme;
                lock (LockObject)
                {
                    Debug.Assert(_credentialCache != null, "Expected non-null credential cache");
                    ncAndScheme = GetCredentials(requestUri, _credentialCache, s_orderedAuthTypes);
                }
                if (ncAndScheme.Key != null)
                {
                    return ncAndScheme;
                }
            }

            // We either weren't preauthenticating or we didn't have any cached credentials
            // available, so check the credentials on the handler.
            return GetCredentials(requestUri, _serverCredentials, s_orderedAuthTypes);
        }

        private void TransferCredentialsToCache(Uri serverUri, CURLAUTH serverAuthAvail)
        {
            if (_serverCredentials == null)
            {
                // No credentials, nothing to put into the cache.
                return;
            }

            lock (LockObject)
            {
                // For each auth type we allow, check whether it's one supported by the server.
                KeyValuePair<string, CURLAUTH>[] validAuthTypes = s_orderedAuthTypes;
                for (int i = 0; i < validAuthTypes.Length; i++)
                {
                    // Is it supported by the server?
                    if ((serverAuthAvail & validAuthTypes[i].Value) != 0)
                    {
                        // And do we have a credential for it?
                        NetworkCredential nc = _serverCredentials.GetCredential(serverUri, validAuthTypes[i].Key);
                        if (nc != null)
                        {
                            // We have a credential for it, so add it, and we're done.
                            Debug.Assert(_credentialCache != null, "Expected non-null credential cache");
                            try
                            {
                                _credentialCache.Add(serverUri, validAuthTypes[i].Key, nc);
                            }
                            catch (ArgumentException)
                            {
                                // Ignore the case of key already present
                            }
                            break;
                        }
                    }
                }
            }
        }

        private void AddResponseCookies(EasyRequest state, string cookieHeader)
        {
            if (!_useCookie)
            {
                return;
            }

            try
            {
                _cookieContainer.SetCookies(state._targetUri, cookieHeader);
                state.SetCookieOption(state._requestMessage.RequestUri);
            }
            catch (CookieException e)
            {
                EventSourceTrace(
                    "Malformed cookie parsing failed: {0}, server: {1}, cookie: {2}", 
                    e.Message, state._requestMessage.RequestUri, cookieHeader);
            }
        }

        private static KeyValuePair<NetworkCredential, CURLAUTH> GetCredentials(Uri requestUri, ICredentials credentials, KeyValuePair<string, CURLAUTH>[] validAuthTypes)
        {
            NetworkCredential nc = null;
            CURLAUTH curlAuthScheme = CURLAUTH.None;

            if (credentials != null)
            {
                // For each auth type we consider valid, try to get a credential for it.
                // Union together the auth types for which we could get credentials, but validate
                // that the found credentials are all the same, as libcurl doesn't support differentiating
                // by auth type.
                for (int i = 0; i < validAuthTypes.Length; i++)
                {
                    NetworkCredential networkCredential = credentials.GetCredential(requestUri, validAuthTypes[i].Key);
                    if (networkCredential != null)
                    {
                        curlAuthScheme |= validAuthTypes[i].Value;
                        if (nc == null)
                        {
                            nc = networkCredential;
                        }
                        else if(!AreEqualNetworkCredentials(nc, networkCredential))
                        {
                            throw new PlatformNotSupportedException(SR.Format(SR.net_http_unix_invalid_credential, CurlVersionDescription));
                        }
                    }
                }
            }

            EventSourceTrace("Authentication scheme: {0}", curlAuthScheme);
            return new KeyValuePair<NetworkCredential, CURLAUTH>(nc, curlAuthScheme); ;
        }

        private void CheckDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }

        private void CheckDisposedOrStarted()
        {
            CheckDisposed();
            if (_anyOperationStarted)
            {
                throw new InvalidOperationException(SR.net_http_operation_started);
            }
        }

        private static void ThrowIfCURLEError(CURLcode error)
        {
            if (error != CURLcode.CURLE_OK) // success
            {
                string msg = CurlException.GetCurlErrorString((int)error, isMulti: false);
                EventSourceTrace(msg);
                switch (error)
                {
                    case CURLcode.CURLE_OPERATION_TIMEDOUT:
                        throw new OperationCanceledException(msg);

                    case CURLcode.CURLE_OUT_OF_MEMORY:
                        throw new OutOfMemoryException(msg);

                    default:
                        throw new CurlException((int)error, msg);
                }
            }
        }

        private static void ThrowIfCURLMError(CURLMcode error)
        {
            if (error != CURLMcode.CURLM_OK && // success
                error != CURLMcode.CURLM_CALL_MULTI_PERFORM) // success + a hint to try curl_multi_perform again
            {
                string msg = CurlException.GetCurlErrorString((int)error, isMulti: true);
                EventSourceTrace(msg);
                switch (error)
                {
                    case CURLMcode.CURLM_ADDED_ALREADY:
                    case CURLMcode.CURLM_BAD_EASY_HANDLE:
                    case CURLMcode.CURLM_BAD_HANDLE:
                    case CURLMcode.CURLM_BAD_SOCKET:
                        throw new ArgumentException(msg);
                    case CURLMcode.CURLM_UNKNOWN_OPTION:
                        throw new ArgumentOutOfRangeException(msg);
                    case CURLMcode.CURLM_OUT_OF_MEMORY:
                        throw new OutOfMemoryException(msg);
                    case CURLMcode.CURLM_INTERNAL_ERROR:
                    default:
                        throw new CurlException((int)error, msg);
                }
            }
        }

        private static bool AreEqualNetworkCredentials(NetworkCredential credential1, NetworkCredential credential2)
        {
            Debug.Assert(credential1 != null && credential2 != null, "arguments are non-null in network equality check");
            return credential1.UserName == credential2.UserName &&
                credential1.Domain == credential2.Domain &&
                string.Equals(credential1.Password, credential2.Password, StringComparison.Ordinal);
        }

        private static bool EventSourceTracingEnabled { get { return HttpEventSource.Log.IsEnabled(); } }

        // PERF NOTE:
        // These generic overloads of EventSourceTrace (and similar wrapper methods in some of the other CurlHandler
        // nested types) exist to allow call sites to call EventSourceTrace without boxing and without checking
        // EventSourceTracingEnabled.  Do not remove these without fixing the call sites accordingly.

        private static void EventSourceTrace<TArg0>(
            string formatMessage, TArg0 arg0,
            MultiAgent agent = null, EasyRequest easy = null, [CallerMemberName] string memberName = null)
        {
            if (EventSourceTracingEnabled)
            {
                EventSourceTraceCore(string.Format(formatMessage, arg0), agent, easy, memberName);
            }
        }

        private static void EventSourceTrace<TArg0, TArg1, TArg2>
            (string formatMessage, TArg0 arg0, TArg1 arg1, TArg2 arg2,
            MultiAgent agent = null, EasyRequest easy = null, [CallerMemberName] string memberName = null)
        {
            if (EventSourceTracingEnabled)
            {
                EventSourceTraceCore(string.Format(formatMessage, arg0, arg1, arg2), agent, easy, memberName);
            }
        }

        private static void EventSourceTrace(
            string message, 
            MultiAgent agent = null, EasyRequest easy = null, [CallerMemberName] string memberName = null)
        {
            if (EventSourceTracingEnabled)
            {
                EventSourceTraceCore(message, agent, easy, memberName);
            }
        }

        private static void EventSourceTraceCore(string message, MultiAgent agent, EasyRequest easy, string memberName)
        {
            // If we weren't handed a multi agent, see if we can get one from the EasyRequest
            if (agent == null && easy != null)
            {
                agent = easy._associatedMultiAgent;
            }

            HttpEventSource.Log.HandlerMessage(
                (agent?.RunningWorkerId).GetValueOrDefault(),
                easy != null ? easy.Task.Id : 0,
                memberName,
                message);
        }

        private static HttpRequestException CreateHttpRequestException(Exception inner)
        {
            return new HttpRequestException(SR.net_http_client_execution_error, inner);
        }

        private static IOException MapToReadWriteIOException(Exception error, bool isRead)
        {
            return new IOException(
                isRead ? SR.net_http_io_read : SR.net_http_io_write,
                error is HttpRequestException && error.InnerException != null ? error.InnerException : error);
        }

        private static void HandleRedirectLocationHeader(EasyRequest state, string locationValue)
        {
            Debug.Assert(state._isRedirect);
            Debug.Assert(state._handler.AutomaticRedirection);

            string location = locationValue.Trim();
            //only for absolute redirects
            Uri forwardUri;
            if (Uri.TryCreate(location, UriKind.RelativeOrAbsolute, out forwardUri) && forwardUri.IsAbsoluteUri)
            {
                // Just as with WinHttpHandler, for security reasons, we drop the server credential if it is 
                // anything other than a CredentialCache. We allow credentials in a CredentialCache since they 
                // are specifically tied to URIs.
                var creds = state._handler.Credentials as CredentialCache;
                KeyValuePair<NetworkCredential, CURLAUTH> ncAndScheme = GetCredentials(forwardUri, creds, s_orderedAuthTypes);
                if (ncAndScheme.Key != null)
                {
                    state.SetCredentialsOptions(ncAndScheme);
                }
                else
                {
                    state.SetCurlOption(CURLoption.CURLOPT_USERNAME, IntPtr.Zero);
                    state.SetCurlOption(CURLoption.CURLOPT_PASSWORD, IntPtr.Zero);
                }

                // reset proxy - it is possible that the proxy has different credentials for the new URI
                state.SetProxyOptions(forwardUri);

                if (state._handler._useCookie)
                {
                    // reset cookies.
                    state.SetCurlOption(CURLoption.CURLOPT_COOKIE, IntPtr.Zero);

                    // set cookies again
                    state.SetCookieOption(forwardUri);
                }

                state.SetRedirectUri(forwardUri);
            }

            // set the headers again. This is a workaround for libcurl's limitation in handling headers with empty values
            state.SetRequestHeaders();
        }

        private static void SetChunkedModeForSend(HttpRequestMessage request)
        {
            bool chunkedMode = request.Headers.TransferEncodingChunked.GetValueOrDefault();
            HttpContent requestContent = request.Content;
            Debug.Assert(requestContent != null, "request is null");

            // Deal with conflict between 'Content-Length' vs. 'Transfer-Encoding: chunked' semantics.
            // libcurl adds a Transfer-Encoding header by default and the request fails if both are set.
            if (requestContent.Headers.ContentLength.HasValue)
            {
                if (chunkedMode)
                {
                    // Same behaviour as WinHttpHandler
                    requestContent.Headers.ContentLength = null;
                }
                else
                {
                    // Prevent libcurl from adding Transfer-Encoding header
                    request.Headers.TransferEncodingChunked = false;
                }
            }
            else if (!chunkedMode)
            {
                // Make sure Transfer-Encoding: chunked header is set, 
                // as we have content to send but no known length for it.
                request.Headers.TransferEncodingChunked = true;
            }
        }

        #endregion
    }
}
