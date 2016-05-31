// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

using CURLAUTH = Interop.Http.CURLAUTH;
using CURLcode = Interop.Http.CURLcode;
using CURLoption = Interop.Http.CURLoption;
using CurlProtocols = Interop.Http.CurlProtocols;
using CURLProxyType = Interop.Http.curl_proxytype;
using SafeCurlHandle = Interop.Http.SafeCurlHandle;
using SafeCurlSListHandle = Interop.Http.SafeCurlSListHandle;
using SafeCallbackHandle = Interop.Http.SafeCallbackHandle;
using SeekCallback = Interop.Http.SeekCallback;
using ReadWriteCallback = Interop.Http.ReadWriteCallback;
using ReadWriteFunction = Interop.Http.ReadWriteFunction;
using SslCtxCallback = Interop.Http.SslCtxCallback;
using DebugCallback = Interop.Http.DebugCallback;

namespace System.Net.Http
{
    internal partial class CurlHandler : HttpMessageHandler
    {
        /// <summary>Provides all of the state associated with a single request/response, referred to as an "easy" request in libcurl parlance.</summary>
        private sealed class EasyRequest : TaskCompletionSource<HttpResponseMessage>
        {
            internal readonly CurlHandler _handler;
            internal readonly HttpRequestMessage _requestMessage;
            internal readonly CurlResponseMessage _responseMessage;
            internal readonly CancellationToken _cancellationToken;
            internal readonly HttpContentAsyncStream _requestContentStream;

            internal SafeCurlHandle _easyHandle;
            private SafeCurlSListHandle _requestHeaders;

            internal MultiAgent _associatedMultiAgent;
            internal SendTransferState _sendTransferState;
            internal bool _isRedirect = false;
            internal bool _ensureResponseMessagePublishedInvoked = false;
            internal Uri _targetUri;
            internal StrongToWeakReference<EasyRequest> _selfStrongToWeakReference;

            private SafeCallbackHandle _callbackHandle;

            public EasyRequest(CurlHandler handler, HttpRequestMessage requestMessage, CancellationToken cancellationToken) :
                base(TaskCreationOptions.RunContinuationsAsynchronously)
            {
                _handler = handler;
                _requestMessage = requestMessage;
                _cancellationToken = cancellationToken;

                if (requestMessage.Content != null)
                {
                    _requestContentStream = new HttpContentAsyncStream(this);
                }

                _responseMessage = new CurlResponseMessage(this);
                _targetUri = requestMessage.RequestUri;
            }

            /// <summary>
            /// Initialize the underlying libcurl support for this EasyRequest.
            /// This is separated out of the constructor so that we can take into account
            /// any additional configuration needed based on the request message
            /// after the EasyRequest is configured and so that error handling
            /// can be better handled in the caller.
            /// </summary>
            internal void InitializeCurl()
            {
                // Create the underlying easy handle
                SafeCurlHandle easyHandle = Interop.Http.EasyCreate();
                if (easyHandle.IsInvalid)
                {
                    throw new OutOfMemoryException();
                }
                _easyHandle = easyHandle;

                // Configure the handle
                SetUrl();
                SetMultithreading();
                SetTimeouts();
                SetRedirection();
                SetVerb();
                SetVersion();
                SetDecompressionOptions();
                SetProxyOptions(_requestMessage.RequestUri);
                SetCredentialsOptions(_handler.GetCredentials(_requestMessage.RequestUri));
                SetCookieOption(_requestMessage.RequestUri);
                SetRequestHeaders();
                SetSslOptions();
            }

            public void EnsureResponseMessagePublished()
            {
                // This method is called once all response headers have been received, and it may be called
                // multiple times after that point.  Once all request data has been sent and the response
                // headers have been received, we need to complete the Task (which was returned from SendAsync).
                // However, while not common, it's possible for a server to send all of the response headers
                // before all of the request data has been sent.  To deal with this, we track whether this
                // function has been called, separately from whether the Task is completed.  The first time
                // it's called, we either publish the response message if all request data has been sent,
                // or we schedule the response message to be published when the request data is finished being sent.
                EventSourceTrace("Already invoked: {0}", _ensureResponseMessagePublishedInvoked);

                // Ensure we only publish once.
                if (_ensureResponseMessagePublishedInvoked) return;
                _ensureResponseMessagePublishedInvoked = true;

                // If request data is still being copied, schedule the publishing of the response for when it completes.
                // Otherwise, just publish it now.
                Task copyTask = _requestContentStream?.CopyTask;
                if (copyTask != null && !copyTask.IsCompleted)
                {
                    EventSourceTrace("CopyTask: {0}", copyTask.Status);
                    copyTask.ContinueWith((_, s) => ((EasyRequest)s).PublishResponseMessage(), this,
                        CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
                }
                else
                {
                    PublishResponseMessage();
                }
            }

            private void PublishResponseMessage()
            {
                EventSourceTrace("Status: {0}", Task.Status);

                // On Windows, if the response was automatically decompressed, Content-Encoding and Content-Length
                // headers are removed from the response. Do the same thing here.
                DecompressionMethods dm = _handler.AutomaticDecompression;
                if (dm != DecompressionMethods.None)
                {
                    HttpContentHeaders contentHeaders = _responseMessage.Content.Headers;
                    IEnumerable<string> encodings;
                    if (contentHeaders.TryGetValues(HttpKnownHeaderNames.ContentEncoding, out encodings))
                    {
                        foreach (string encoding in encodings)
                        {
                            if (((dm & DecompressionMethods.GZip) != 0 && string.Equals(encoding, EncodingNameGzip, StringComparison.OrdinalIgnoreCase)) ||
                                ((dm & DecompressionMethods.Deflate) != 0 && string.Equals(encoding, EncodingNameDeflate, StringComparison.OrdinalIgnoreCase)))
                            {
                                contentHeaders.Remove(HttpKnownHeaderNames.ContentEncoding);
                                contentHeaders.Remove(HttpKnownHeaderNames.ContentLength);
                                break;
                            }
                        }
                    }
                }

                // Now ensure it's published.
                bool completedTask = TrySetResult(_responseMessage);
                Debug.Assert(completedTask || Task.IsFaulted || Task.IsCanceled,
                    "Expected either to successfully complete the task or have it already be faulted/canceled");

                // We handed off lifetime ownership of the response to the owner of the task.
                // Transition our reference on the EasyRequest to be weak instead of strong, so 
                // that we don't forcibly keep it alive.
                Debug.Assert(_selfStrongToWeakReference != null, "Expected non-null wrapper");
                _selfStrongToWeakReference.MakeWeak();
            }

            public void FailRequestAndCleanup(Exception error)
            {
                FailRequest(error);
                Cleanup();
            }

            public void FailRequest(Exception error)
            {
                Debug.Assert(error != null, "Expected non-null exception");
                EventSourceTrace("Failing request: {0}", error);

                var oce = error as OperationCanceledException;
                if (oce != null)
                {
                    TrySetCanceled(oce.CancellationToken);
                }
                else
                {
                    if (error is IOException || error is CurlException || error == null)
                    {
                        error = CreateHttpRequestException(error);
                    }
                    TrySetException(error);
                }
                // There's not much we can reasonably assert here about the result of TrySet*.
                // It's possible that the task wasn't yet completed (e.g. a failure while initiating the request),
                // it's possible that the task was already completed as success (e.g. a failure sending back the response),
                // and it's possible that the task was already completed as failure (e.g. we handled the exception and
                // faulted the task, but then tried to fault it again while finishing processing in the main loop).

                // Make sure the exception is available on the response stream so that it's propagated
                // from any attempts to read from the stream.
                _responseMessage.ResponseStream.SignalComplete(error);
            }

            public void Cleanup() // not called Dispose because the request may still be in use after it's cleaned up
            {
                _responseMessage.ResponseStream.SignalComplete(); // No more callbacks so no more data
                // Don't dispose of the ResponseMessage.ResponseStream as it may still be in use
                // by code reading data stored in the stream.

                // Dispose of the input content stream if there was one.  Nothing should be using it any more.
                _requestContentStream?.Dispose();

                // Dispose of the underlying easy handle.  We're no longer processing it.
                _easyHandle?.Dispose();

                // Dispose of the request headers if we had any.  We had to keep this handle
                // alive as long as the easy handle was using it.  We didn't need to do any
                // ref counting on the safe handle, though, as the only processing happens
                // in Process, which ensures the handle will be rooted while libcurl is
                // doing any processing that assumes it's valid.
                _requestHeaders?.Dispose();

                _callbackHandle?.Dispose();
            }

            private void SetUrl()
            {
                Uri requestUri = _requestMessage.RequestUri;

                long scopeId;
                if (IsLinkLocal(requestUri, out scopeId))
                {
                    // Uri.AbsoluteUri doesn't include the ScopeId/ZoneID, so if it is link-local,
                    // we separately pass the scope to libcurl.
                    EventSourceTrace("ScopeId: {0}", scopeId);
                    SetCurlOption(CURLoption.CURLOPT_ADDRESS_SCOPE, scopeId);
                }

                EventSourceTrace("Url: {0}", requestUri);
                SetCurlOption(CURLoption.CURLOPT_URL, requestUri.AbsoluteUri);
                SetCurlOption(CURLoption.CURLOPT_PROTOCOLS, (long)(CurlProtocols.CURLPROTO_HTTP | CurlProtocols.CURLPROTO_HTTPS));
            }

            private static bool IsLinkLocal(Uri url, out long scopeId)
            {
                IPAddress ip;
                if (IPAddress.TryParse(url.DnsSafeHost, out ip) && ip.IsIPv6LinkLocal)
                {
                    scopeId = ip.ScopeId;
                    return true;
                }

                scopeId = 0;
                return false;
            }

            private void SetMultithreading()
            {
                SetCurlOption(CURLoption.CURLOPT_NOSIGNAL, 1L);
            }

            private void SetTimeouts()
            {
                // Set timeout limit on the connect phase.
                SetCurlOption(CURLoption.CURLOPT_CONNECTTIMEOUT_MS, int.MaxValue);

                // Override the default DNS cache timeout.  libcurl defaults to a 1 minute
                // timeout, but we extend that to match the Windows timeout of 10 minutes.
                const int DnsCacheTimeoutSeconds = 10 * 60;
                SetCurlOption(CURLoption.CURLOPT_DNS_CACHE_TIMEOUT, DnsCacheTimeoutSeconds);
            }

            private void SetRedirection()
            {
                if (!_handler._automaticRedirection)
                {
                    return;
                }

                SetCurlOption(CURLoption.CURLOPT_FOLLOWLOCATION, 1L);

                CurlProtocols redirectProtocols = string.Equals(_requestMessage.RequestUri.Scheme, UriSchemeHttps, StringComparison.OrdinalIgnoreCase) ?
                    CurlProtocols.CURLPROTO_HTTPS : // redirect only to another https
                    CurlProtocols.CURLPROTO_HTTP | CurlProtocols.CURLPROTO_HTTPS; // redirect to http or to https
                SetCurlOption(CURLoption.CURLOPT_REDIR_PROTOCOLS, (long)redirectProtocols);

                SetCurlOption(CURLoption.CURLOPT_MAXREDIRS, _handler._maxAutomaticRedirections);
                EventSourceTrace("Max automatic redirections: {0}", _handler._maxAutomaticRedirections);
            }

            private void SetContentLength(CURLoption lengthOption)
            {
                Debug.Assert(lengthOption == CURLoption.CURLOPT_POSTFIELDSIZE || lengthOption == CURLoption.CURLOPT_INFILESIZE);

                if (_requestMessage.Content == null)
                {
                    // Tell libcurl there's no data to be sent.
                    SetCurlOption(lengthOption, 0L);
                    return;
                }

                long? contentLengthOpt = _requestMessage.Content.Headers.ContentLength;
                if (contentLengthOpt != null)
                {
                    long contentLength = contentLengthOpt.GetValueOrDefault();
                    if (contentLength <= int.MaxValue)
                    {
                        // Tell libcurl how much data we expect to send.
                        SetCurlOption(lengthOption, contentLength);
                    }
                    else
                    {
                        // Similarly, tell libcurl how much data we expect to send.  However,
                        // as the amount is larger than a 32-bit value, switch to the "_LARGE"
                        // equivalent libcurl options.
                        SetCurlOption(
                            lengthOption == CURLoption.CURLOPT_INFILESIZE ? CURLoption.CURLOPT_INFILESIZE_LARGE : CURLoption.CURLOPT_POSTFIELDSIZE_LARGE,
                            contentLength);
                    }
                    return;
                }

                // There is content but we couldn't determine its size.  Don't set anything.
            }

            private void SetVerb()
            {
                EventSourceTrace<string>("Verb: {0}", _requestMessage.Method.Method);

                if (_requestMessage.Method == HttpMethod.Put)
                {
                    SetCurlOption(CURLoption.CURLOPT_UPLOAD, 1L);
                    SetContentLength(CURLoption.CURLOPT_INFILESIZE);
                }
                else if (_requestMessage.Method == HttpMethod.Head)
                {
                    SetCurlOption(CURLoption.CURLOPT_NOBODY, 1L);
                }
                else if (_requestMessage.Method == HttpMethod.Post)
                {
                    SetCurlOption(CURLoption.CURLOPT_POST, 1L);
                    SetContentLength(CURLoption.CURLOPT_POSTFIELDSIZE);
                }
                else if (_requestMessage.Method == HttpMethod.Trace)
                {
                    SetCurlOption(CURLoption.CURLOPT_CUSTOMREQUEST, _requestMessage.Method.Method);
                    SetCurlOption(CURLoption.CURLOPT_NOBODY, 1L);
                }
                else
                {
                    SetCurlOption(CURLoption.CURLOPT_CUSTOMREQUEST, _requestMessage.Method.Method);
                    if (_requestMessage.Content != null)
                    {
                        SetCurlOption(CURLoption.CURLOPT_UPLOAD, 1L);
                        SetContentLength(CURLoption.CURLOPT_INFILESIZE);
                    }
                }
            }

            private void SetVersion()
            {
                Version v = _requestMessage.Version;
                if (v != null)
                {
                    // Try to use the requested version, if a known version was explicitly requested.
                    // If an unknown version was requested, we simply use libcurl's default.
                    var curlVersion =
                        (v.Major == 1 && v.Minor == 1) ? Interop.Http.CurlHttpVersion.CURL_HTTP_VERSION_1_1 :
                        (v.Major == 1 && v.Minor == 0) ? Interop.Http.CurlHttpVersion.CURL_HTTP_VERSION_1_0 :
                        (v.Major == 2 && v.Minor == 0) ? Interop.Http.CurlHttpVersion.CURL_HTTP_VERSION_2_0 :
                        Interop.Http.CurlHttpVersion.CURL_HTTP_VERSION_NONE;

                    if (curlVersion != Interop.Http.CurlHttpVersion.CURL_HTTP_VERSION_NONE)
                    {
                        // Ask libcurl to use the specified version if possible.
                        CURLcode c = Interop.Http.EasySetOptionLong(_easyHandle, CURLoption.CURLOPT_HTTP_VERSION, (long)curlVersion);
                        if (c == CURLcode.CURLE_OK)
                        {
                            // Success.  The requested version will be used.
                            EventSourceTrace("HTTP version: {0}", v);
                        }
                        else if (c == CURLcode.CURLE_UNSUPPORTED_PROTOCOL)
                        {
                            // The requested version is unsupported.  Fall back to using the default version chosen by libcurl.
                            EventSourceTrace("Unsupported protocol: {0}", v);
                        }
                        else
                        {
                            // Some other error. Fail.
                            ThrowIfCURLEError(c);
                        }
                    }
                }
            }

            private void SetDecompressionOptions()
            {
                if (!_handler.SupportsAutomaticDecompression)
                {
                    return;
                }

                DecompressionMethods autoDecompression = _handler.AutomaticDecompression;
                bool gzip = (autoDecompression & DecompressionMethods.GZip) != 0;
                bool deflate = (autoDecompression & DecompressionMethods.Deflate) != 0;
                if (gzip || deflate)
                {
                    string encoding = (gzip && deflate) ? EncodingNameGzip + "," + EncodingNameDeflate :
                                       gzip ? EncodingNameGzip :
                                       EncodingNameDeflate;
                    SetCurlOption(CURLoption.CURLOPT_ACCEPT_ENCODING, encoding);
                    EventSourceTrace<string>("Encoding: {0}", encoding);
                }
            }

            internal void SetProxyOptions(Uri requestUri)
            {
                if (!_handler._useProxy)
                {
                    // Explicitly disable the use of a proxy.  This will prevent libcurl from using
                    // any proxy, including ones set via environment variable.
                    SetCurlOption(CURLoption.CURLOPT_PROXY, string.Empty);
                    EventSourceTrace("UseProxy false, disabling proxy");
                    return;
                }

                if (_handler.Proxy == null)
                {
                    // UseProxy was true, but Proxy was null.  Let libcurl do its default handling, 
                    // which includes checking the http_proxy environment variable.  
                    EventSourceTrace("UseProxy true, Proxy null, using default proxy");

                    // Since that proxy set in an environment variable might require a username and password,
                    // use the default proxy credentials if there are any.  Currently only NetworkCredentials 
                    // are used, as we can't query by the proxy Uri, since we don't know it.
                    SetProxyCredentials(_handler.DefaultProxyCredentials as NetworkCredential);

                    return;
                }

                // Custom proxy specified.
                Uri proxyUri;
                try
                {
                    // Should we bypass a proxy for this URI?
                    if (_handler.Proxy.IsBypassed(requestUri))
                    {
                        SetCurlOption(CURLoption.CURLOPT_PROXY, string.Empty);
                        EventSourceTrace("Proxy's IsBypassed returned true, bypassing proxy");
                        return;
                    }

                    // Get the proxy Uri for this request.
                    proxyUri = _handler.Proxy.GetProxy(requestUri);
                    if (proxyUri == null)
                    {
                        EventSourceTrace("GetProxy returned null, using default.");
                        return;
                    }
                }
                catch (PlatformNotSupportedException)
                {
                    // WebRequest.DefaultWebProxy throws PlatformNotSupportedException,
                    // in which case we should use the default rather than the custom proxy.
                    EventSourceTrace("PlatformNotSupportedException from proxy, using default");
                    return;
                }

                // Configure libcurl with the gathered proxy information

                // uri.AbsoluteUri/ToString() omit IPv6 scope IDs.  SerializationInfoString ensures these details 
                // are included, but does not properly handle international hosts.  As a workaround we check whether 
                // the host is a link-local IP address, and based on that either return the SerializationInfoString 
                // or the AbsoluteUri. (When setting the request Uri itself, we instead use CURLOPT_ADDRESS_SCOPE to
                // set the scope id and the url separately, avoiding these issues and supporting versions of libcurl
                // prior to v7.37 that don't support parsing scope IDs out of the url's host.  As similar feature
                // doesn't exist for proxies.)
                IPAddress ip;
                string proxyUrl = IPAddress.TryParse(proxyUri.DnsSafeHost, out ip) && ip.IsIPv6LinkLocal ?
                    proxyUri.GetComponents(UriComponents.SerializationInfoString, UriFormat.UriEscaped) :
                    proxyUri.AbsoluteUri;

                EventSourceTrace<string>("Proxy: {0}", proxyUrl);
                SetCurlOption(CURLoption.CURLOPT_PROXYTYPE, (long)CURLProxyType.CURLPROXY_HTTP);
                SetCurlOption(CURLoption.CURLOPT_PROXY, proxyUrl);
                SetCurlOption(CURLoption.CURLOPT_PROXYPORT, proxyUri.Port);

                KeyValuePair<NetworkCredential, CURLAUTH> credentialScheme = GetCredentials(
                    proxyUri, _handler.Proxy.Credentials, s_orderedAuthTypes);
                SetProxyCredentials(credentialScheme.Key);
            }

            private void SetProxyCredentials(NetworkCredential credentials)
            {
                if (credentials == CredentialCache.DefaultCredentials)
                {
                    // No "default credentials" on Unix; nop just like UseDefaultCredentials.
                    EventSourceTrace("DefaultCredentials set for proxy. Skipping.");
                }
                else if (credentials != null)
                {
                    if (string.IsNullOrEmpty(credentials.UserName))
                    {
                        throw new ArgumentException(SR.net_http_argument_empty_string, "UserName");
                    }

                    // Unlike normal credentials, proxy credentials are URL decoded by libcurl, so we URL encode 
                    // them in order to allow, for example, a colon in the username.
                    string credentialText = string.IsNullOrEmpty(credentials.Domain) ?
                        string.Format("{0}:{1}", WebUtility.UrlEncode(credentials.UserName), WebUtility.UrlEncode(credentials.Password)) :
                        string.Format("{2}\\{0}:{1}", WebUtility.UrlEncode(credentials.UserName), WebUtility.UrlEncode(credentials.Password), WebUtility.UrlEncode(credentials.Domain));

                    EventSourceTrace("Proxy credentials set.");
                    SetCurlOption(CURLoption.CURLOPT_PROXYUSERPWD, credentialText);
                }
            }

            internal void SetCredentialsOptions(KeyValuePair<NetworkCredential, CURLAUTH> credentialSchemePair)
            {
                if (credentialSchemePair.Key == null)
                {
                    return;
                }

                NetworkCredential credentials = credentialSchemePair.Key;
                CURLAUTH authScheme = credentialSchemePair.Value;
                string userName = string.IsNullOrEmpty(credentials.Domain) ?
                    credentials.UserName :
                    string.Format("{0}\\{1}", credentials.Domain, credentials.UserName);

                SetCurlOption(CURLoption.CURLOPT_USERNAME, userName);
                SetCurlOption(CURLoption.CURLOPT_HTTPAUTH, (long)authScheme);
                if (credentials.Password != null)
                {
                    SetCurlOption(CURLoption.CURLOPT_PASSWORD, credentials.Password);
                }

                EventSourceTrace("Credentials set.");
            }

            internal void SetCookieOption(Uri uri)
            {
                if (!_handler._useCookie)
                {
                    return;
                }

                string cookieValues = _handler.CookieContainer.GetCookieHeader(uri);
                if (!string.IsNullOrEmpty(cookieValues))
                {
                    SetCurlOption(CURLoption.CURLOPT_COOKIE, cookieValues);
                    EventSourceTrace<string>("Cookies: {0}", cookieValues);
                }
            }

            internal void SetRequestHeaders()
            {
                var slist = new SafeCurlSListHandle();

                // Add content request headers
                if (_requestMessage.Content != null)
                {
                    SetChunkedModeForSend(_requestMessage);

                    _requestMessage.Content.Headers.Remove(HttpKnownHeaderNames.ContentLength); // avoid overriding libcurl's handling via INFILESIZE/POSTFIELDSIZE
                    AddRequestHeaders(_requestMessage.Content.Headers, slist);

                    if (_requestMessage.Content.Headers.ContentType == null)
                    {
                        // Remove the Content-Type header libcurl adds by default.
                        ThrowOOMIfFalse(Interop.Http.SListAppend(slist, NoContentType));
                    }
                }

                // Add request headers
                AddRequestHeaders(_requestMessage.Headers, slist);

                // Since libcurl always adds a Transfer-Encoding header, we need to explicitly block
                // it if caller specifically does not want to set the header
                if (_requestMessage.Headers.TransferEncodingChunked.HasValue &&
                    !_requestMessage.Headers.TransferEncodingChunked.Value)
                {
                    ThrowOOMIfFalse(Interop.Http.SListAppend(slist, NoTransferEncoding));
                }

                // And disable the "Expect: 100-continue" header that libcurl sends by default for some
                // requests. This is done both to match the WinHttpHandler behavior as well as to enable 
                // us to match the behavior of completing the SendAsync task when response headers have
                // been received and once all request data has been sent (without this, we can't tell the
                // difference between whether no request data has been sent because libcurl simply hasn't yet
                // but will or because it's never going to).
                ThrowOOMIfFalse(Interop.Http.SListAppend(slist, NoExpect));

                if (!slist.IsInvalid)
                {
                    SafeCurlSListHandle prevList = _requestHeaders;
                    _requestHeaders = slist;
                    SetCurlOption(CURLoption.CURLOPT_HTTPHEADER, slist);
                    prevList?.Dispose();
                }
                else
                {
                    slist.Dispose();
                }
            }

            private void SetSslOptions()
            {
                // SSL Options should be set regardless of the type of the original request,
                // in case an http->https redirection occurs.
                //
                // While this does slow down the theoretical best path of the request the code
                // to decide that we need to register the callback is more complicated than, and
                // potentially more expensive than, just always setting the callback.
                SslProvider.SetSslOptions(this, _handler.ClientCertificateOptions);
            }

            internal void SetCurlCallbacks(
                IntPtr easyGCHandle,
                ReadWriteCallback receiveHeadersCallback,
                ReadWriteCallback sendCallback,
                SeekCallback seekCallback,
                ReadWriteCallback receiveBodyCallback,
                DebugCallback debugCallback)
            {
                if (_callbackHandle == null)
                {
                    _callbackHandle = new SafeCallbackHandle();
                }

                // Add callback for processing headers
                Interop.Http.RegisterReadWriteCallback(
                    _easyHandle,
                    ReadWriteFunction.Header,
                    receiveHeadersCallback,
                    easyGCHandle,
                    ref _callbackHandle);

                // If we're sending data as part of the request, add callbacks for sending request data
                if (_requestMessage.Content != null)
                {
                    Interop.Http.RegisterReadWriteCallback(
                        _easyHandle,
                        ReadWriteFunction.Read,
                        sendCallback,
                        easyGCHandle,
                        ref _callbackHandle);

                    Interop.Http.RegisterSeekCallback(
                        _easyHandle,
                        seekCallback,
                        easyGCHandle,
                        ref _callbackHandle);
                }

                // If we're expecting any data in response, add a callback for receiving body data
                if (_requestMessage.Method != HttpMethod.Head)
                {
                    Interop.Http.RegisterReadWriteCallback(
                        _easyHandle,
                        ReadWriteFunction.Write,
                        receiveBodyCallback,
                        easyGCHandle,
                        ref _callbackHandle);
                }

                if (EventSourceTracingEnabled)
                {
                    SetCurlOption(CURLoption.CURLOPT_VERBOSE, 1L);
                    CURLcode curlResult = Interop.Http.RegisterDebugCallback(
                        _easyHandle, 
                        debugCallback,
                        easyGCHandle,
                        ref _callbackHandle);
                    if (curlResult != CURLcode.CURLE_OK)
                    {
                        EventSourceTrace("Failed to register debug callback.");
                    }
                }
            }

            internal CURLcode SetSslCtxCallback(SslCtxCallback callback, IntPtr userPointer)
            {
                if (_callbackHandle == null)
                {
                    _callbackHandle = new SafeCallbackHandle();
                }

                CURLcode result = Interop.Http.RegisterSslCtxCallback(_easyHandle, callback, userPointer, ref _callbackHandle);
                return result;
            }

            private static void AddRequestHeaders(HttpHeaders headers, SafeCurlSListHandle handle)
            {
                foreach (KeyValuePair<string, IEnumerable<string>> header in headers)
                {
                    string headerValue = headers.GetHeaderString(header.Key);
                    string headerKeyAndValue = string.IsNullOrEmpty(headerValue) ?
                        header.Key + ";" : // semicolon used by libcurl to denote empty value that should be sent
                        header.Key + ": " + headerValue;
                    ThrowOOMIfFalse(Interop.Http.SListAppend(handle, headerKeyAndValue));
                }
            }

            internal void SetCurlOption(CURLoption option, string value)
            {
                ThrowIfCURLEError(Interop.Http.EasySetOptionString(_easyHandle, option, value));
            }

            internal void SetCurlOption(CURLoption option, long value)
            {
                ThrowIfCURLEError(Interop.Http.EasySetOptionLong(_easyHandle, option, value));
            }

            internal void SetCurlOption(CURLoption option, IntPtr value)
            {
                ThrowIfCURLEError(Interop.Http.EasySetOptionPointer(_easyHandle, option, value));
            }

            internal void SetCurlOption(CURLoption option, SafeHandle value)
            {
                ThrowIfCURLEError(Interop.Http.EasySetOptionPointer(_easyHandle, option, value));
            }

            private static void ThrowOOMIfFalse(bool appendResult)
            {
                if (!appendResult)
                    throw CreateHttpRequestException(new CurlException((int)CURLcode.CURLE_OUT_OF_MEMORY, isMulti: false));
            }

            internal sealed class SendTransferState
            {
                internal readonly byte[] _buffer;
                internal int _offset;
                internal int _count;
                internal Task<int> _task;

                internal SendTransferState(int bufferLength)
                {
                    Debug.Assert(bufferLength > 0 && bufferLength <= MaxRequestBufferSize, $"Expected 0 < bufferLength <= {MaxRequestBufferSize}, got {bufferLength}");
                    _buffer = new byte[bufferLength];
                }

                internal void SetTaskOffsetCount(Task<int> task, int offset, int count)
                {
                    Debug.Assert(offset >= 0, "Offset should never be negative");
                    Debug.Assert(count >= 0, "Count should never be negative");
                    Debug.Assert(offset <= count, "Offset should never be greater than count");

                    _task = task;
                    _offset = offset;
                    _count = count;
                }
            }

            internal void  SetRedirectUri(Uri redirectUri)
            {
                _targetUri = _requestMessage.RequestUri;
                _requestMessage.RequestUri = redirectUri;
            }

            private void EventSourceTrace<TArg0>(string formatMessage, TArg0 arg0, [CallerMemberName] string memberName = null)
            {
                CurlHandler.EventSourceTrace(formatMessage, arg0, easy: this, memberName: memberName);
            }

            private void EventSourceTrace(string message, [CallerMemberName] string memberName = null)
            {
                CurlHandler.EventSourceTrace(message, easy: this, memberName: memberName);
            }
        }
    }
}
