using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Security.Permissions;
using System.Security;
using System.Diagnostics.CodeAnalysis;

namespace System.Net.Http
{
    public class HttpClientHandler : HttpMessageHandler
    {
        #region Fields

        private static readonly Action<object> onCancel = OnCancel;

        private readonly Action<object> startRequest;
        private readonly AsyncCallback getRequestStreamCallback;
        private readonly AsyncCallback getResponseCallback;

        private volatile bool operationStarted;
        private volatile bool disposed;

        private long maxRequestContentBufferSize;
        private CookieContainer cookieContainer;
        private bool useCookies;
        private DecompressionMethods automaticDecompression;
        private IWebProxy proxy;
        private bool useProxy;
        private bool preAuthenticate;
        private bool useDefaultCredentials;
        private ICredentials credentials;
        private bool allowAutoRedirect;
        private int maxAutomaticRedirections;
        private string connectionGroupName;
        private ClientCertificateOption clientCertOptions;
        private X509Certificate2Collection clientCertificates;
        private IDictionary<String, Object> properties; // Only create dictionary when required.

#if DEBUG
        // The following delegate is only used for unit-testing: It allows tests to create a custom HttpWebRequest
        // instance.
        internal delegate HttpWebRequest WebRequestCreatorDelegate(HttpRequestMessage request, string connectionGroupName);
        internal WebRequestCreatorDelegate WebRequestCreator = null;
#endif

        #endregion Fields

        #region Properties

        public virtual bool SupportsAutomaticDecompression
        {
            get { return true; }
        }

        public virtual bool SupportsProxy
        {
            get { return true; }
        }

        public virtual bool SupportsRedirectConfiguration
        {
            get { return true; }
        }

        public bool UseCookies
        {
            get { return useCookies; }
            set
            {
                CheckDisposedOrStarted();
                useCookies = value;
            }
        }

        public bool CheckCertificateRevocationList
        {
            get { return ServicePointManager.CheckCertificateRevocationList; }
            set
            {
                CheckDisposedOrStarted();
                throw new PlatformNotSupportedException(String.Format(CultureInfo.InvariantCulture,
                    SR.net_http_value_not_supported, value, nameof(CheckCertificateRevocationList)));
            }
        }

        public CookieContainer CookieContainer
        {
            get { return cookieContainer; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                if (!UseCookies)
                {
                    throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture,
                        SR.net_http_invalid_enable_first, "UseCookies", "true"));
                }
                CheckDisposedOrStarted();
                cookieContainer = value;
            }
        }

        public ClientCertificateOption ClientCertificateOptions
        {
            get { return clientCertOptions; }
            set
            {
                if (value != ClientCertificateOption.Manual
                    && value != ClientCertificateOption.Automatic)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                CheckDisposedOrStarted();
                clientCertOptions = value;
            }
        }

        public X509CertificateCollection ClientCertificates
        {
            get
            {
                if (clientCertificates == null)
                {
                    clientCertificates = new X509Certificate2Collection();
                }

                return clientCertificates;
            }
        }

        public ICredentials DefaultProxyCredentials
        {
            get
            {
                return null;
            }
            set
            {
                throw new PlatformNotSupportedException(String.Format(CultureInfo.InvariantCulture,
                    SR.net_http_value_not_supported, value, nameof(DefaultProxyCredentials)));
            }
        }

        public DecompressionMethods AutomaticDecompression
        {
            get { return automaticDecompression; }
            set
            {
                CheckDisposedOrStarted();
                automaticDecompression = value;
            }
        }

        public bool UseProxy
        {
            get { return useProxy; }
            set
            {
                CheckDisposedOrStarted();
                useProxy = value;
            }
        }

        public IWebProxy Proxy
        {
            get { return proxy; }
            [SecuritySafeCritical]
            set
            {
                if (!UseProxy && value != null)
                {
                    throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture,
                        SR.net_http_invalid_enable_first, "UseProxy", "true"));
                }
                CheckDisposedOrStarted();
                ExceptionHelper.WebPermissionUnrestricted.Demand();
                proxy = value;
            }
        }

        public bool PreAuthenticate
        {
            get { return preAuthenticate; }
            set
            {
                CheckDisposedOrStarted();
                preAuthenticate = value;
            }
        }

        public bool UseDefaultCredentials
        {
            get { return useDefaultCredentials; }
            set
            {
                CheckDisposedOrStarted();
                useDefaultCredentials = value;
            }
        }

        public ICredentials Credentials
        {
            get { return credentials; }
            set
            {
                CheckDisposedOrStarted();
                credentials = value;
            }
        }

        public bool AllowAutoRedirect
        {
            get { return allowAutoRedirect; }
            set
            {
                CheckDisposedOrStarted();
                allowAutoRedirect = value;
            }
        }

        public int MaxAutomaticRedirections
        {
            get { return maxAutomaticRedirections; }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                CheckDisposedOrStarted();
                maxAutomaticRedirections = value;
            }
        }

        public int MaxConnectionsPerServer
        {
            get
            {
                return ServicePointManager.DefaultConnectionLimit;
            }
            set
            {
                CheckDisposedOrStarted();
                throw new PlatformNotSupportedException(String.Format(CultureInfo.InvariantCulture,
                    SR.net_http_value_not_supported, value, nameof(MaxConnectionsPerServer)));
            }
        }

        public long MaxRequestContentBufferSize
        {
            get { return maxRequestContentBufferSize; }
            set
            {
                // Setting the value to 0 is OK: It means the user doesn't want the handler to buffer content.
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                if (value > HttpContent.MaxBufferSize)
                {
                    throw new ArgumentOutOfRangeException("value", value,
                        string.Format(CultureInfo.InvariantCulture, SR.net_http_content_buffersize_limit,
                        HttpContent.MaxBufferSize));
                }
                CheckDisposedOrStarted();
                maxRequestContentBufferSize = value;
            }
        }

        public int MaxResponseHeadersLength
        {
            get
            {
                return 0;
            }
            set
            {
                throw new PlatformNotSupportedException(String.Format(CultureInfo.InvariantCulture,
                    SR.net_http_value_not_supported, value, nameof(MaxResponseHeadersLength)));
            }
        }

        public IDictionary<String, Object> Properties
        {
            get
            {
                if (properties == null)
                {
                    properties = new Dictionary<String, object>();
                }

                return properties;
            }
        }

        public Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool> ServerCertificateCustomValidationCallback
        {
            get
            {
                return null;
            }
            set
            {
                CheckDisposedOrStarted();
                if (value != null)
                {
                    throw new PlatformNotSupportedException(String.Format(CultureInfo.InvariantCulture,
                        SR.net_http_value_not_supported, value, nameof(ServerCertificateCustomValidationCallback)));
                }
            }
        }

        public SslProtocols SslProtocols
        {
            get
            {
                return SslProtocols.None;
            }
            set
            {
                CheckDisposedOrStarted();
                if (value != SslProtocols.None)
                {
                    throw new PlatformNotSupportedException(String.Format(CultureInfo.InvariantCulture,
                        SR.net_http_value_not_supported, value, nameof(SslProtocols)));
                }
            }
        }

        #endregion Properties

        #region De/Constructors

        public HttpClientHandler()
        {
            this.startRequest = StartRequest;
            this.getRequestStreamCallback = GetRequestStreamCallback;
            this.getResponseCallback = GetResponseCallback;

            this.connectionGroupName = RuntimeHelpers.GetHashCode(this).ToString(NumberFormatInfo.InvariantInfo);

            // Set HWR default values
            this.allowAutoRedirect = true;
            this.maxRequestContentBufferSize = HttpContent.MaxBufferSize;
            this.automaticDecompression = DecompressionMethods.None;
            this.cookieContainer = new CookieContainer(); // default container used for dealing with auto-cookies.
            this.credentials = null;
            this.maxAutomaticRedirections = 50;
            this.preAuthenticate = false;
            this.proxy = null;
            this.useProxy = true;
            this.useCookies = true; // deal with cookies by default.
            this.useDefaultCredentials = false;
            this.clientCertOptions = ClientCertificateOption.Manual;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && !disposed)
            {
                disposed = true;
                // Close all connection groups created by the current handler instance. Since every instance uses a
                // unique connection group name, disposing a handler will remove all these unique connection groups to
                // save resources.
                ServicePointManager.CloseConnectionGroups(connectionGroupName);
            }
            base.Dispose(disposing);
        }

        #endregion De/Constructors

        #region Request Setup

        private HttpWebRequest CreateAndPrepareWebRequest(HttpRequestMessage request)
        {
            HttpWebRequest webRequest = null;

            // If we have a request-content, make sure to provide HWR with a delegate to CopyTo(). This allows HWR
            // to serialize the content multiple times in case of redirect/authentication.
            // Also note that the connection group name provided is considered an 'internal' connection group. I.e.
            // HWR will add 'I>' after the string we provided. I.e. by default the actual connection group name looks 
            // like '123456S>I>' or '123456U>I>' if UnsafeAuthenticatedConnectionSharing is true. Even is users use the
            // same hashcode for their HWR connection group, they'll end up using a different one, since 'I>' is not
            // added ('123456S>' or '123456U>').
            if (request.Content != null)
            {
                webRequest = new HttpWebRequest(request.RequestUri, true, connectionGroupName, request.Content.CopyTo);
            }
            else
            {
                webRequest = new HttpWebRequest(request.RequestUri, true, connectionGroupName, null);
            }

#if DEBUG
            // For testing purposes only: If the delegate is assigned, it is used to create an instance of 
            // HttpWebRequest. Tests can derive from HttpWebRequest and implement their own behavior.
            if (WebRequestCreator != null)
            {
                webRequest = WebRequestCreator(request, connectionGroupName);
                Contract.Assert(webRequest != null);
            }
#endif

            if (Logging.On) Logging.Associate(Logging.Http, request, webRequest);

            webRequest.Method = request.Method.Method;
            webRequest.ProtocolVersion = request.Version;

            SetDefaultOptions(webRequest);
            SetConnectionOptions(webRequest, request);
            SetServicePointOptions(webRequest, request);
            SetRequestHeaders(webRequest, request);
            SetContentHeaders(webRequest, request);

            // For Extensibility
            InitializeWebRequest(request, webRequest);

            return webRequest;
        }

        // Needs to be internal so that WebRequestHandler can access it from a different assembly.
        internal virtual void InitializeWebRequest(HttpRequestMessage request, HttpWebRequest webRequest)
        {
        }

        private void SetDefaultOptions(HttpWebRequest webRequest)
        {
            webRequest.Timeout = Timeout.Infinite; // Timeouts are handled by HttpClient.

            webRequest.AllowAutoRedirect = allowAutoRedirect;
            webRequest.AutomaticDecompression = automaticDecompression;
            webRequest.PreAuthenticate = preAuthenticate;

            if (useDefaultCredentials)
            {
                webRequest.UseDefaultCredentials = true;
            }
            else
            {
                webRequest.Credentials = credentials;
            }

            if (allowAutoRedirect)
            {
                webRequest.MaximumAutomaticRedirections = maxAutomaticRedirections;
            }

            if (useProxy)
            {
                // If 'UseProxy' is true and 'Proxy' is null (default), let HWR figure out the proxy to use. Otherwise
                // set the custom proxy.
                if (proxy != null)
                {
                    webRequest.Proxy = proxy;
                }
            }
            else
            {
                // The use explicitly specified to not use a proxy. Set HWR.Proxy to null to make sure HWR doesn't use
                // a proxy for this request.
                webRequest.Proxy = null;
            }

            if (useCookies)
            {
                webRequest.CookieContainer = cookieContainer;
            }

            if (clientCertOptions == ClientCertificateOption.Automatic && ComNetOS.IsWin7orLater)
            {
                X509CertificateCollection automaticClientCerts
                    = UnsafeNclNativeMethods.NativePKI.FindClientCertificates();
                if (automaticClientCerts.Count > 0)
                {
                    webRequest.ClientCertificates = automaticClientCerts;
                }
            }
        }

        private static void SetConnectionOptions(HttpWebRequest webRequest, HttpRequestMessage request)
        {
            if (request.Version <= HttpVersion.Version10)
            {
                // HTTP 1.0 had some support for persistent connections by allowing "Connection: Keep-Alive". Check
                // whether this value is set.
                bool keepAliveSet = false;
                foreach (string item in request.Headers.Connection)
                {
                    if (string.Compare(item, "Keep-Alive", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        keepAliveSet = true;
                        break;
                    }
                }
                webRequest.KeepAlive = keepAliveSet;
            }
            else
            {
                // HTTP 1.1 uses persistent connections by default. If the user doesn't want to use persistent 
                // connections, he can set 'ConnectionClose' to true (equivalent to header "Connection: close").
                if (request.Headers.ConnectionClose == true)
                {
                    webRequest.KeepAlive = false;
                }
            }
        }

        private void SetServicePointOptions(HttpWebRequest webRequest, HttpRequestMessage request)
        {
            HttpRequestHeaders headers = request.Headers;
            ServicePoint currentServicePoint = null;

            // We have to update the ServicePoint in order to support "Expect: 100-continue". This setting may affect
            // also requests sent by other HWR instances (or HttpClient instances). This is a known limitation.
            bool? expectContinue = headers.ExpectContinue;
            if (expectContinue != null)
            {
                currentServicePoint = webRequest.ServicePoint;
                currentServicePoint.Expect100Continue = (bool)expectContinue;
            }
        }

        private static void SetRequestHeaders(HttpWebRequest webRequest, HttpRequestMessage request)
        {
            WebHeaderCollection webRequestHeaders = webRequest.Headers;
            HttpRequestHeaders headers = request.Headers;

            // Most headers are just added directly to HWR's internal headers collection. But there are some exceptions
            // requiring different handling.
            // The following bool vars are used to skip string comparison when not required: E.g. if the 'Host' header
            // was not set, we don't need to compare every header in the collection with 'Host' to make sure we don't
            // add it to HWR's header collection.
            bool isHostSet = headers.Contains(HttpKnownHeaderNames.Host);
            bool isExpectSet = headers.Contains(HttpKnownHeaderNames.Expect);
            bool isTransferEncodingSet = headers.Contains(HttpKnownHeaderNames.TransferEncoding);
            bool isConnectionSet = headers.Contains(HttpKnownHeaderNames.Connection);

            if (isHostSet)
            {
                string host = headers.Host;
                if (host != null)
                {
                    webRequest.Host = host;
                }
            }

            // The following headers (Expect, Transfer-Encoding, Connection) have both a collection property and a 
            // bool property indicating a special value. Internally (in HttpHeaders) we don't distinguish between 
            // "special" values and other values. So we must make sure that we add all but the special value to HWR.
            // E.g. the 'Transfer-Encoding: chunked' value must be set using HWR.SendChunked, whereas all other values
            // can be added to the 'Transfer-Encoding'.
            if (isExpectSet)
            {
                string expectHeader = headers.Expect.GetHeaderStringWithoutSpecial();
                // Was at least one non-special value set?
                if (!String.IsNullOrEmpty(expectHeader) || !headers.Expect.IsSpecialValueSet)
                {
                    webRequestHeaders.AddInternal(HttpKnownHeaderNames.Expect, expectHeader);
                }
            }

            if (isTransferEncodingSet)
            {
                string transferEncodingHeader = headers.TransferEncoding.GetHeaderStringWithoutSpecial();
                // Was at least one non-special value set?
                if (!String.IsNullOrEmpty(transferEncodingHeader) || !headers.TransferEncoding.IsSpecialValueSet)
                {
                    webRequestHeaders.AddInternal(HttpKnownHeaderNames.TransferEncoding, transferEncodingHeader);
                }
            }

            if (isConnectionSet)
            {
                string connectionHeader = headers.Connection.GetHeaderStringWithoutSpecial();

                // Was at least one non-special value set?
                if (!String.IsNullOrEmpty(connectionHeader) || !headers.Connection.IsSpecialValueSet)
                {
                    webRequestHeaders.AddInternal(HttpKnownHeaderNames.Connection, connectionHeader);
                }
            }

            foreach (var header in request.Headers.GetHeaderStrings())
            {
                string headerName = header.Key;

                if ((isHostSet && AreEqual(HttpKnownHeaderNames.Host, headerName)) ||
                    (isExpectSet && AreEqual(HttpKnownHeaderNames.Expect, headerName)) ||
                    (isTransferEncodingSet && AreEqual(HttpKnownHeaderNames.TransferEncoding, headerName)) ||
                    (isConnectionSet && AreEqual(HttpKnownHeaderNames.Connection, headerName)))
                {
                    continue; // Header was already added.
                }

                // Use AddInternal() to skip validation.
                webRequestHeaders.AddInternal(header.Key, header.Value);
            }
        }

        private static void SetContentHeaders(HttpWebRequest webRequest, HttpRequestMessage request)
        {
            if (request.Content != null)
            {
                HttpContentHeaders headers = request.Content.Headers;

                // All content headers besides Content-Length can be added directly to HWR. So just check whether we 
                // have the Content-Length header set. If not, add all headers, otherwise skip the Content-Length 
                // header.
                // Note that this method is called _before_ PrepareWebRequestForContentUpload(): I.e. in most scenarios
                // this means that no one accessed Headers.ContentLength property yet, thus there will be no 
                // Content-Length header in the store. I.e. we'll end up in the 'else' block providing better perf, 
                // since no string comparison is required.
                if (headers.Contains(HttpKnownHeaderNames.ContentLength))
                {
                    foreach (var header in request.Content.Headers)
                    {
                        if (string.Compare(HttpKnownHeaderNames.ContentLength, header.Key, StringComparison.OrdinalIgnoreCase) != 0)
                        {
                            // Use AddInternal() to skip validation.
                            webRequest.Headers.AddInternal(header.Key, string.Join(", ", header.Value));
                        }
                    }
                }
                else
                {
                    foreach (var header in request.Content.Headers)
                    {
                        // Use AddInternal() to skip validation.
                        webRequest.Headers.AddInternal(header.Key, string.Join(", ", header.Value));                        
                    }
                }
            }
        }

        #endregion Message Setup

        #region Request Processing

        protected internal override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request", SR.net_http_handler_norequest);
            }
            CheckDisposed();

            if (Logging.On) Logging.Enter(Logging.Http, this, "SendAsync", request);

            SetOperationStarted();

            TaskCompletionSource<HttpResponseMessage> tcs = new TaskCompletionSource<HttpResponseMessage>();
            RequestState state = new RequestState();
            state.tcs = tcs;
            state.cancellationToken = cancellationToken;
            state.requestMessage = request;

            try
            {
                // Cancellation: Note that there is no race here: If the token gets canceled before we register the
                // callback, the token will invoke the callback immediately. I.e. HWR gets aborted before we use it.
                HttpWebRequest webRequest = CreateAndPrepareWebRequest(request);
                state.webRequest = webRequest;
                cancellationToken.Register(onCancel, webRequest);

                // Preserve context for authentication
                if (ExecutionContext.IsFlowSuppressed()) 
                {
                    // Check for proxy auth
                    IWebProxy currentProxy = null;
                    if (useProxy)
                    {
                        currentProxy = proxy ?? WebRequest.DefaultWebProxy;
                    }

                    if ((UseDefaultCredentials || Credentials != null
                        || (currentProxy != null && currentProxy.Credentials != null)))
                    {
                        SafeCaptureIdenity(state);
                    }
                }

                // BeginGetResponse/BeginGetRequestStream have a lot of setup work to do before becoming async
                // (proxy, dns, connection pooling, etc).  Run these on a separate thread.
                // Do not provide a cancellation token; if this helper task could be canceled before starting then 
                // nobody would complete the tcs.
                Task.Factory.StartNew(startRequest, state);
            }
            catch (Exception e)
            {
                HandleAsyncException(state, e);
            }

            if (Logging.On) Logging.Exit(Logging.Http, this, "SendAsync", tcs.Task);
            return tcs.Task;
        }

        private void StartRequest(object obj)
        {
            RequestState state = obj as RequestState;
            Contract.Assert(state != null);

            try
            {
                if (state.requestMessage.Content != null)
                {
                    PrepareAndStartContentUpload(state);
                }
                else
                {
                    state.webRequest.ContentLength = 0;
                    StartGettingResponse(state);
                }
            }
            catch (Exception e)
            {
                HandleAsyncException(state, e);
            }
        }

        private void PrepareAndStartContentUpload(RequestState state)
        {
            HttpContent requestContent = state.requestMessage.Content;
            Contract.Assert(requestContent != null);

            try
            {
                // Determine how to communicate the length of the request content.
                if (state.requestMessage.Headers.TransferEncodingChunked == true)
                {
                    state.webRequest.SendChunked = true;
                    StartGettingRequestStream(state);
                }
                else
                {
                    long? contentLength = requestContent.Headers.ContentLength;
                    if (contentLength != null)
                    {
                        state.webRequest.ContentLength = (long)contentLength;
                        StartGettingRequestStream(state);
                    }
                    else
                    {
                        // If we don't have a content length and we don't use chunked, then we must buffer the content.
                        // If the user specified a zero buffer size, we throw.
                        if (maxRequestContentBufferSize == 0)
                        {
                            throw new HttpRequestException(SR.net_http_handler_nocontentlength);
                        }

                        // HttpContent couldn't calculate the content length. Chunked is not specified. Buffer the 
                        // content to get the content length.
                        requestContent.LoadIntoBufferAsync(maxRequestContentBufferSize).ContinueWithStandard(task =>
                        {
                            if (task.IsFaulted)
                            {
                                HandleAsyncException(state, task.Exception.GetBaseException());
                                return;
                            }

                            try
                            {
                                contentLength = requestContent.Headers.ContentLength;
                                Contract.Assert(contentLength != null, "After buffering content, ContentLength must not be null.");
                                state.webRequest.ContentLength = (long)contentLength;
                                StartGettingRequestStream(state);
                            }
                            catch (Exception e)
                            {
                                HandleAsyncException(state, e);
                            }
                        });
                    }
                }
            }
            catch (Exception e)
            {
                HandleAsyncException(state, e);
            }
        }

        private void StartGettingRequestStream(RequestState state)
        {
            // Manually flow identity context if captured.
            if (state.identity != null)
            {
                using (state.identity.Impersonate())
                {
                    state.webRequest.BeginGetRequestStream(getRequestStreamCallback, state);
                }
            }
            else
            {
                state.webRequest.BeginGetRequestStream(getRequestStreamCallback, state);
            }
        }

        private void GetRequestStreamCallback(IAsyncResult ar)
        {
            RequestState state = ar.AsyncState as RequestState;
            Contract.Assert(state != null);

            try
            {
                TransportContext context = null;
                Stream requestStream = state.webRequest.EndGetRequestStream(ar, out context) as Stream;
                state.requestStream = requestStream;
                state.requestMessage.Content.CopyToAsync(requestStream, context).ContinueWithStandard(task =>
                {
                    try
                    {
                        if (task.IsFaulted)
                        {
                            HandleAsyncException(state, task.Exception.GetBaseException());
                            return;
                        }

                        if (task.IsCanceled)
                        {
                            state.tcs.TrySetCanceled();
                            return;
                        }

                        state.requestStream.Close();
                        StartGettingResponse(state);
                    }
                    catch (Exception e)
                    {
                        HandleAsyncException(state, e);
                    }

                });
            }
            catch (Exception e)
            {
                HandleAsyncException(state, e);
            }
        }

        private void StartGettingResponse(RequestState state)
        {
            // Manually flow identity context if captured.
            if (state.identity != null)
            {
                using (state.identity.Impersonate())
                {
                    state.webRequest.BeginGetResponse(getResponseCallback, state);
                }
            }
            else
            {
                state.webRequest.BeginGetResponse(getResponseCallback, state);
            }
        }

        private void GetResponseCallback(IAsyncResult ar)
        {
            RequestState state = ar.AsyncState as RequestState;
            Contract.Assert(state != null);

            try
            {
                HttpWebResponse webResponse = state.webRequest.EndGetResponse(ar) as HttpWebResponse;
                state.tcs.TrySetResult(CreateResponseMessage(webResponse, state.requestMessage));
            }
            catch (Exception e)
            {
                HandleAsyncException(state, e);
            }
        }

        private HttpResponseMessage CreateResponseMessage(HttpWebResponse webResponse, HttpRequestMessage request)
        {
            HttpResponseMessage response = new HttpResponseMessage(webResponse.StatusCode);
            response.ReasonPhrase = webResponse.StatusDescription;
            response.Version = webResponse.ProtocolVersion;
            response.RequestMessage = request;
            response.Content = new StreamContent(new WebExceptionWrapperStream(webResponse.GetResponseStream()));

            // Update Request-URI to reflect the URI actually leading to the response message.
            request.RequestUri = webResponse.ResponseUri;

            WebHeaderCollection webResponseHeaders = webResponse.Headers;
            HttpContentHeaders contentHeaders = response.Content.Headers;
            HttpResponseHeaders responseHeaders = response.Headers;

            // HttpWebResponse.ContentLength is set to -1 if no Content-Length header is provided.
            if (webResponse.ContentLength >= 0)
            {
                contentHeaders.ContentLength = webResponse.ContentLength;
            }

            for (int i = 0; i < webResponseHeaders.Count; i++)
            {
                string currentHeader = webResponseHeaders.GetKey(i);

                // We already set Content-Length
                if (string.Compare(currentHeader, HttpKnownHeaderNames.ContentLength,
                    StringComparison.OrdinalIgnoreCase) == 0)
                {
                    continue;
                }

                string[] values = webResponseHeaders.GetValues(i);

                if (!responseHeaders.TryAddWithoutValidation(currentHeader, values))
                {
                    bool result = contentHeaders.TryAddWithoutValidation(currentHeader, values);
                    // WebHeaderCollection should never return us invalid header names.
                    Contract.Assert(result, "Invalid header name.");
                }
            }

            return response;
        }

        private void HandleAsyncException(RequestState state, Exception e)
        {
            // Use 'SendAsync' as method name, since this method is only called by methods in the async code path. Using
            // 'SendAsync' as method name helps relate the exception to the operation in log files.
            if (Logging.On) Logging.Exception(Logging.Http, this, "SendAsync", e);

            // If the WebException was due to the cancellation token being canceled, throw cancellation exception.
            if (state.cancellationToken.IsCancellationRequested)
            {
                state.tcs.TrySetCanceled();
            }
            // Wrap expected exceptions as HttpRequestExceptions since this is considered an error during 
            // execution. All other exception types, including ArgumentExceptions and ProtocolViolationExceptions
            // are 'unexpected' or caused by user error and should not be wrapped.
            else if (e is WebException || e is IOException)
            {
                state.tcs.TrySetException(new HttpRequestException(SR.net_http_client_execution_error, e));
            }
            else
            {
                state.tcs.TrySetException(e);
            }
        }

        private static void OnCancel(object state)
        {
            HttpWebRequest webRequest = state as HttpWebRequest;
            Contract.Assert(webRequest != null);

            webRequest.Abort();
        }

        #endregion Request Processing

        #region Helpers

        private void SetOperationStarted()
        {
            if (!operationStarted)
            {
                operationStarted = true;
            }
        }

        private void CheckDisposed()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }

        internal void CheckDisposedOrStarted()
        {
            CheckDisposed();
            if (operationStarted)
            {
                throw new InvalidOperationException(SR.net_http_operation_started);
            }
        }

        private static bool AreEqual(string x, string y)
        {
            return (string.Compare(x, y, StringComparison.OrdinalIgnoreCase) == 0);
        }

        // Security: We need an assert for a call into WindowsIdentity.GetCurrent
        [SecuritySafeCritical]
        [SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.ControlPrincipal)]
        [SuppressMessage("Microsoft.Security", "CA2106:SecureAsserts", Justification="Needed for identity flow.")]
        private void SafeCaptureIdenity(RequestState state)
        {
            state.identity = WindowsIdentity.GetCurrent();
        }

        #endregion Helpers

        private class RequestState
        {
            internal HttpWebRequest webRequest;
            internal TaskCompletionSource<HttpResponseMessage> tcs;
            internal CancellationToken cancellationToken;
            internal HttpRequestMessage requestMessage;
            internal Stream requestStream;
            internal WindowsIdentity identity;
        }

        // The ConnectStream returned by HttpWebResponse may throw a WebException when aborted. Wrap them in 
        // IOExceptions. The ConnectStream will be read-only so we don't need to wrap the write methods.
        private class WebExceptionWrapperStream : DelegatingStream
        {
            internal WebExceptionWrapperStream(Stream innerStream)
                : base(innerStream)
            {
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                try
                {
                    return base.Read(buffer, offset, count);
                }
                catch (WebException wex)
                {
                    throw new IOException(SR.net_http_io_read, wex);
                }
            }

            public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
            {
                try
                {
                    return base.BeginRead(buffer, offset, count, callback, state);
                }
                catch (WebException wex)
                {
                    throw new IOException(SR.net_http_io_read, wex);
                }
            }

            public override int EndRead(IAsyncResult asyncResult)
            {
                try
                {
                    return base.EndRead(asyncResult);
                }
                catch (WebException wex)
                {
                    throw new IOException(SR.net_http_io_read, wex);
                }
            }

            public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, 
                CancellationToken cancellationToken)
            {
                try
                {
                    return await base.ReadAsync(buffer, offset, count, cancellationToken).ConfigureAwait(false);
                }
                catch (WebException wex)
                {
                    throw new IOException(SR.net_http_io_read, wex);
                }
            }

            public override int ReadByte()
            {
                try
                {
                    return base.ReadByte();
                }
                catch (WebException wex)
                {
                    throw new IOException(SR.net_http_io_read, wex);
                }
            }
        }
    }
}
