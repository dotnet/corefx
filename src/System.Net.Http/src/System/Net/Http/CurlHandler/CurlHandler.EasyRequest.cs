// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
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
            /// <summary>Maximum content length where we'll use COPYPOSTFIELDS to let libcurl dup the content.</summary>
            private const int InMemoryPostContentLimit = 32 * 1024; // arbitrary limit; could be tweaked in the future based on experimentation
            /// <summary>Debugging flag used to enable CURLOPT_VERBOSE to dump to stderr when not redirecting it to the event source.</summary>
            private static readonly bool s_curlDebugLogging = Environment.GetEnvironmentVariable("CURLHANDLER_DEBUG_VERBOSE") == "true";

            internal readonly CurlHandler _handler;
            internal readonly MultiAgent _associatedMultiAgent;
            internal readonly HttpRequestMessage _requestMessage;
            internal readonly CurlResponseMessage _responseMessage;
            internal readonly CancellationToken _cancellationToken;
            internal Stream _requestContentStream;
            internal long? _requestContentStreamStartingPosition;
            internal bool _inMemoryPostContent;

            internal SafeCurlHandle _easyHandle;
            private SafeCurlSListHandle _requestHeaders;

            internal SendTransferState _sendTransferState;
            internal StrongToWeakReference<EasyRequest> _selfStrongToWeakReference;

            private SafeCallbackHandle _callbackHandle;

            public EasyRequest(CurlHandler handler, MultiAgent agent, HttpRequestMessage requestMessage, CancellationToken cancellationToken) :
                base(TaskCreationOptions.RunContinuationsAsynchronously)
            {
                Debug.Assert(handler != null, $"Expected non-null {nameof(handler)}");
                Debug.Assert(agent != null, $"Expected non-null {nameof(agent)}");
                Debug.Assert(requestMessage != null, $"Expected non-null {nameof(requestMessage)}");

                _handler = handler;
                _associatedMultiAgent = agent;
                _requestMessage = requestMessage;

                _cancellationToken = cancellationToken;
                _responseMessage = new CurlResponseMessage(this);
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

                EventSourceTrace("Configuring request.");

                // Before setting any other options, turn on curl's debug tracing
                // if desired.  CURLOPT_VERBOSE may also be set subsequently if
                // EventSource tracing is enabled.
                if (s_curlDebugLogging)
                {
                    SetCurlOption(CURLoption.CURLOPT_VERBOSE, 1L);
                }

                // Before actually configuring the handle based on the state of the request,
                // do any necessary cleanup of the request object.
                SanitizeRequestMessage();
                
                // Configure the handle
                SetUrl();
                SetNetworkingOptions();
                SetMultithreading();
                SetTimeouts();
                SetRedirection();
                SetVerb();
                SetVersion();
                SetDecompressionOptions();
                SetProxyOptions(_requestMessage.RequestUri);
                SetCredentialsOptions(_handler._useDefaultCredentials ? GetDefaultCredentialAndAuth() : _handler.GetCredentials(_requestMessage.RequestUri));
                SetCookieOption(_requestMessage.RequestUri);
                SetRequestHeaders(copyAuthHeaders:true);
                SetSslOptions();

                EventSourceTrace("Done configuring request.");
            }

            public void EnsureResponseMessagePublished()
            {
                // If the response message hasn't been published yet, do any final processing of it before it is.
                if (!Task.IsCompleted)
                {
                    EventSourceTrace("Publishing response message");

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
                }

                // Now ensure it's published.
                bool completedTask = TrySetResult(_responseMessage);
                Debug.Assert(completedTask || Task.IsCompletedSuccessfully,
                    "If the task was already completed, it should have been completed successfully; " +
                    "we shouldn't be completing as successful after already completing as failed.");

                // If we successfully transitioned it to be completed, we also handed off lifetime ownership
                // of the response to the owner of the task.  Transition our reference on the EasyRequest
                // to be weak instead of strong, so that we don't forcibly keep it alive.
                if (completedTask)
                {
                    Debug.Assert(_selfStrongToWeakReference != null, "Expected non-null wrapper");
                    _selfStrongToWeakReference.MakeWeak();
                }
            }

            public void CleanupAndFailRequest(Exception error)
            {
                try
                {
                    Cleanup();
                }
                catch (Exception exc)
                {
                    // This would only happen in an aggregious case, such as a Stream failing to seek when
                    // it claims to be able to, but in case something goes very wrong, make sure we don't
                    // lose the exception information.
                    error = new AggregateException(error, exc);
                }
                finally
                {
                    FailRequest(error);
                }
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
                    if (error is InvalidOperationException || error is IOException || error is CurlException || error == null)
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
                // Don't dispose of the ResponseMessage.ResponseStream as it may still be in use
                // by code reading data stored in the stream. Also don't dispose of the request content
                // stream; that'll be handled by the disposal of the request content by the HttpClient,
                // and doing it here prevents reuse by an intermediate handler sitting between the client
                // and this handler.

                // However, if we got an original position for the request stream, we seek back to that position,
                // for the corner case where the stream does get reused before it's disposed by the HttpClient
                // (if the same request object is used multiple times from an intermediate handler, we'll be using
                // ReadAsStreamAsync, which on the same request object will return the same stream object, which
                // we've already advanced).
                if (_requestContentStream != null && _requestContentStream.CanSeek)
                {
                    Debug.Assert(_requestContentStreamStartingPosition.HasValue, "The stream is seekable, but we don't have a starting position?");
                    _requestContentStream.Position = _requestContentStreamStartingPosition.GetValueOrDefault();
                }

                // Dispose of the underlying easy handle.  We're no longer processing it.
                _easyHandle?.Dispose();

                // Dispose of the request headers if we had any.  We had to keep this handle
                // alive as long as the easy handle was using it.  We didn't need to do any
                // ref counting on the safe handle, though, as the only processing happens
                // in Process, which ensures the handle will be rooted while libcurl is
                // doing any processing that assumes it's valid.
                _requestHeaders?.Dispose();

                // Dispose native callback resources
                _callbackHandle?.Dispose();

                // Release any send transfer state, which will return its buffer to the pool
                _sendTransferState?.Dispose();
            }

            private void SanitizeRequestMessage()
            {
                // Make sure Transfer-Encoding and Content-Length make sense together.
                if (_requestMessage.Content != null)
                {
                    SetChunkedModeForSend(_requestMessage);
                }
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
                string idnHost = requestUri.IdnHost;
                string url = requestUri.Host == idnHost ? 
                                requestUri.AbsoluteUri : 
                                new UriBuilder(requestUri) { Host = idnHost }.Uri.AbsoluteUri;

                SetCurlOption(CURLoption.CURLOPT_URL, url);
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

            private void SetNetworkingOptions()
            {
                // Disable the TCP Nagle algorithm.  It's disabled by default starting with libcurl 7.50.2,
                // and when enabled has a measurably negative impact on latency in key scenarios
                // (e.g. POST'ing small-ish data).
                SetCurlOption(CURLoption.CURLOPT_TCP_NODELAY, 1L);
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

            /// <summary>
            /// When a Location header is received along with a 3xx status code, it's an indication
            /// that we're likely to redirect.  Prepare the easy handle in case we do.
            /// </summary>
            internal void SetPossibleRedirectForLocationHeader(string location)
            {
                // Reset cookies in case we redirect.  Below we'll set new cookies for the
                // new location if we have any.
                if (_handler._useCookies)
                {
                    SetCurlOption(CURLoption.CURLOPT_COOKIE, IntPtr.Zero);
                }

                // Parse the location string into a relative or absolute Uri, then combine that
                // with the current request Uri to get the new location.
                var updatedCredentials = default(KeyValuePair<NetworkCredential, CURLAUTH>);
                Uri newUri;
                if (Uri.TryCreate(_requestMessage.RequestUri, location.Trim(), out newUri))
                {
                    // Just as with WinHttpHandler, for security reasons, we drop the server credential if it is 
                    // anything other than a CredentialCache. We allow credentials in a CredentialCache since they 
                    // are specifically tied to URIs.
                    updatedCredentials = _handler._useDefaultCredentials ?
                        GetDefaultCredentialAndAuth() : 
                        GetCredentials(newUri, _handler.Credentials as CredentialCache, s_orderedAuthTypes);

                    // Reset proxy - it is possible that the proxy has different credentials for the new URI
                    SetProxyOptions(newUri);

                    // Set up new cookies
                    if (_handler._useCookies)
                    {
                        SetCookieOption(newUri);
                    }

                    if (newUri.Scheme == Uri.UriSchemeHttp && _requestMessage.RequestUri.Scheme == Uri.UriSchemeHttps)
                    {
                        EventSourceTrace("Insecure https to http redirect: {0}", (_requestMessage.RequestUri, newUri));
                    }
                }

                // Set up the new credentials, either for the new Uri if we were able to get it, 
                // or to empty creds if we couldn't.
                SetCredentialsOptions(updatedCredentials);

                // Set the headers again. This is a workaround for libcurl's limitation in handling 
                // headers with empty values.
                SetRequestHeaders(copyAuthHeaders:false);
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
                    EventSourceTrace("Set content length: {0}", contentLength);
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

                    // Set the content length if we have one available. We must set POSTFIELDSIZE before setting
                    // COPYPOSTFIELDS, as the setting of COPYPOSTFIELDS uses the size to know how much data to read
                    // out; if POSTFIELDSIZE is not done before, COPYPOSTFIELDS will look for a null terminator, and
                    // we don't necessarily have one.
                    SetContentLength(CURLoption.CURLOPT_POSTFIELDSIZE);

                    // For most content types and most HTTP methods, we use a callback that lets libcurl
                    // get data from us if/when it wants it.  However, as an optimization, for POSTs that
                    // use content already known to be entirely in memory, we hand that data off to libcurl
                    // ahead of time.  This not only saves on costs associated with all of the async transfer
                    // between the content and libcurl, it also lets libcurl do larger writes that can, for
                    // example, enable fewer packets to be sent on the wire.
                    var inMemContent = _requestMessage.Content as ByteArrayContent;
                    ArraySegment<byte> contentSegment;
                    if (inMemContent != null &&
                        inMemContent.TryGetBuffer(out contentSegment) &&
                        contentSegment.Count <= InMemoryPostContentLimit) // skip if we'd be forcing libcurl to allocate/copy a large buffer
                    {
                        // Only pre-provide the content if the content still has its ContentLength
                        // and if that length matches the segment.  If it doesn't, something has been overridden,
                        // and we should rely on reading from the content stream to get the data.
                        long? contentLength = inMemContent.Headers.ContentLength;
                        if (contentLength.HasValue && contentLength.GetValueOrDefault() == contentSegment.Count)
                        {
                            _inMemoryPostContent = true;

                            // Debug double-check array segment; this should all have been validated by the ByteArrayContent
                            Debug.Assert(contentSegment.Array != null, "Expected non-null byte content array");
                            Debug.Assert(contentSegment.Count >= 0, $"Expected non-negative byte content count {contentSegment.Count}");
                            Debug.Assert(contentSegment.Offset >= 0, $"Expected non-negative byte content offset {contentSegment.Offset}");
                            Debug.Assert(contentSegment.Array.Length - contentSegment.Offset >= contentSegment.Count,
                                $"Expected offset {contentSegment.Offset} + count {contentSegment.Count} to be within array length {contentSegment.Array.Length}");

                            // Hand the data off to libcurl with COPYPOSTFIELDS for it to copy out the data. (The alternative
                            // is to use POSTFIELDS, which would mean we'd need to pin the array in the ByteArrayContent for the
                            // duration of the request.  Often with a ByteArrayContent, the data will be small and the copy cheap.)
                            unsafe
                            {
                                fixed (byte* inMemContentPtr = contentSegment.Array)
                                {
                                    SetCurlOption(CURLoption.CURLOPT_COPYPOSTFIELDS, new IntPtr(inMemContentPtr + contentSegment.Offset));
                                    EventSourceTrace("Set post fields rather than using send content callback");
                                }
                            }
                        }
                    }
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
                    // Only allow HTTP/2 when making https requests.
                    var curlVersion =
                        (v.Major == 1 && v.Minor == 1) ? Interop.Http.CurlHttpVersion.CURL_HTTP_VERSION_1_1 :
                        (v.Major == 1 && v.Minor == 0) ? Interop.Http.CurlHttpVersion.CURL_HTTP_VERSION_1_0 :
                        (v.Major == 2 && v.Minor == 0) ? Interop.Http.CurlHttpVersion.CURL_HTTP_VERSION_2TLS :
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
                        WebUtility.UrlEncode(credentials.UserName) + ":" + WebUtility.UrlEncode(credentials.Password) :
                        string.Format("{2}\\{0}:{1}", WebUtility.UrlEncode(credentials.UserName), WebUtility.UrlEncode(credentials.Password), WebUtility.UrlEncode(credentials.Domain));

                    EventSourceTrace("Proxy credentials set.");
                    SetCurlOption(CURLoption.CURLOPT_PROXYUSERPWD, credentialText);
                }
            }

            internal void SetCredentialsOptions(KeyValuePair<NetworkCredential, CURLAUTH> credentialSchemePair)
            {
                if (credentialSchemePair.Key == null)
                {
                    EventSourceTrace("Credentials cleared.");
                    SetCurlOption(CURLoption.CURLOPT_USERNAME, IntPtr.Zero);
                    SetCurlOption(CURLoption.CURLOPT_PASSWORD, IntPtr.Zero);
                    return;
                }

                NetworkCredential credentials = credentialSchemePair.Key;
                CURLAUTH authScheme = credentialSchemePair.Value;
                string userName = string.IsNullOrEmpty(credentials.Domain) ?
                    credentials.UserName :
                    credentials.Domain + "\\" + credentials.UserName;

                SetCurlOption(CURLoption.CURLOPT_USERNAME, userName);
                SetCurlOption(CURLoption.CURLOPT_HTTPAUTH, (long)authScheme);
                if (credentials.Password != null)
                {
                    SetCurlOption(CURLoption.CURLOPT_PASSWORD, credentials.Password);
                }

                EventSourceTrace("Credentials set.");
            }

            private static KeyValuePair<NetworkCredential, CURLAUTH> GetDefaultCredentialAndAuth() =>
                new KeyValuePair<NetworkCredential, CURLAUTH>(CredentialCache.DefaultNetworkCredentials, CURLAUTH.Negotiate);

            internal void SetCookieOption(Uri uri)
            {
                if (!_handler._useCookies)
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

            internal void SetRequestHeaders(bool copyAuthHeaders)
            {
                var slist = new SafeCurlSListHandle();

                bool suppressContentType;
                if (_requestMessage.Content != null)
                {
                    // Add content request headers
                    AddRequestHeaders(_requestMessage.Content.Headers, slist, copyAuthHeaders);
                    suppressContentType = _requestMessage.Content.Headers.ContentType == null;
                }
                else
                {
                    suppressContentType = true;
                }

                if (suppressContentType)
                {
                    // Remove the Content-Type header libcurl adds by default.
                    ThrowOOMIfFalse(Interop.Http.SListAppend(slist, NoContentType));
                }

                // Add request headers
                AddRequestHeaders(_requestMessage.Headers, slist, copyAuthHeaders);

                // Since libcurl always adds a Transfer-Encoding header, we need to explicitly block
                // it if caller specifically does not want to set the header
                if (_requestMessage.Headers.TransferEncodingChunked.HasValue &&
                    !_requestMessage.Headers.TransferEncodingChunked.Value)
                {
                    ThrowOOMIfFalse(Interop.Http.SListAppend(slist, NoTransferEncoding));
                }

                // Since libcurl adds an Expect header if it sees enough post data, we need to explicitly block
                // it unless the caller has explicitly opted-in to it.
                if (!_requestMessage.Headers.ExpectContinue.GetValueOrDefault())
                {
                    ThrowOOMIfFalse(Interop.Http.SListAppend(slist, NoExpect));
                }

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

            internal bool ServerCertificateValidationCallbackAcceptsAll => ReferenceEquals(
                _handler.ServerCertificateCustomValidationCallback,
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator);

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
                ThrowOOMIfInvalid(_callbackHandle);

                // If we're sending data as part of the request and it wasn't already added as
                // in-memory data, add callbacks for sending request data.
                if (!_inMemoryPostContent && _requestMessage.Content != null)
                {
                    Interop.Http.RegisterReadWriteCallback(
                        _easyHandle,
                        ReadWriteFunction.Read,
                        sendCallback,
                        easyGCHandle,
                        ref _callbackHandle);
                    Debug.Assert(!_callbackHandle.IsInvalid, $"Should have been allocated (or failed) when originally adding handlers");

                    Interop.Http.RegisterSeekCallback(
                        _easyHandle,
                        seekCallback,
                        easyGCHandle,
                        ref _callbackHandle);
                    Debug.Assert(!_callbackHandle.IsInvalid, $"Should have been allocated (or failed) when originally adding handlers");
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
                    Debug.Assert(!_callbackHandle.IsInvalid, $"Should have been allocated (or failed) when originally adding handlers");
                }

                if (NetEventSource.IsEnabled)
                {
                    SetCurlOption(CURLoption.CURLOPT_VERBOSE, 1L);
                    CURLcode curlResult = Interop.Http.RegisterDebugCallback(
                        _easyHandle, 
                        debugCallback,
                        easyGCHandle,
                        ref _callbackHandle);
                    Debug.Assert(!_callbackHandle.IsInvalid, $"Should have been allocated (or failed) when originally adding handlers");
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

                return Interop.Http.RegisterSslCtxCallback(_easyHandle, callback, userPointer, ref _callbackHandle);
            }

            private static void AddRequestHeaders(HttpHeaders headers, SafeCurlSListHandle handle, bool copyAuthHeaders)
            {
                foreach (KeyValuePair<string, IEnumerable<string>> header in headers)
                {
                    if (string.Equals(header.Key, HttpKnownHeaderNames.ContentLength, StringComparison.OrdinalIgnoreCase) ||
                        (!copyAuthHeaders && string.Equals(header.Key, HttpKnownHeaderNames.Authorization, StringComparison.OrdinalIgnoreCase)))
                    {
                        // avoid overriding libcurl's handling via INFILESIZE/POSTFIELDSIZE
                        continue;
                    }

                    string headerKeyAndValue;
                    string[] values = header.Value as string[];
                    Debug.Assert(values != null, "Implementation detail, but expected Value to be a string[]");
                    if (values != null && values.Length < 2)
                    {
                        // 0 or 1 values
                        headerKeyAndValue = values.Length == 0 || string.IsNullOrEmpty(values[0]) ?
                            header.Key + ";" : // semicolon used by libcurl to denote empty value that should be sent
                            header.Key + ": " + values[0];
                    }
                    else
                    {
                        // Either Values wasn't a string[], or it had 2 or more items. Both are handled by GetHeaderString.
                        string headerValue = headers.GetHeaderString(header.Key);
                        headerKeyAndValue = string.IsNullOrEmpty(headerValue) ?
                            header.Key + ";" : // semicolon needed by libcurl; see above
                            header.Key + ": " + headerValue;
                    }

                    ThrowOOMIfFalse(Interop.Http.SListAppend(handle, headerKeyAndValue));
                }
            }

            internal void SetCurlOption(CURLoption option, string value)
            {
                ThrowIfCURLEError(Interop.Http.EasySetOptionString(_easyHandle, option, value));
            }

            internal CURLcode TrySetCurlOption(CURLoption option, string value)
            {
                return Interop.Http.EasySetOptionString(_easyHandle, option, value);
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
                {
                    ThrowOOM();
                }
            }

            private static void ThrowOOMIfInvalid(SafeHandle handle)
            {
                if (handle.IsInvalid)
                {
                    ThrowOOM();
                }
            }

            private static void ThrowOOM()
            {
                throw CreateHttpRequestException(new CurlException((int)CURLcode.CURLE_OUT_OF_MEMORY, isMulti: false));
            }

            internal sealed class SendTransferState : IDisposable
            {
                internal byte[] Buffer { get; private set; }
                internal int Offset { get; set; }
                internal int Count { get; set; }
                internal Task<int> Task { get; private set; }

                public SendTransferState(int bufferLength)
                {
                    Debug.Assert(bufferLength > 0 && bufferLength <= MaxRequestBufferSize, $"Expected 0 < bufferLength <= {MaxRequestBufferSize}, got {bufferLength}");
                    Buffer = ArrayPool<byte>.Shared.Rent(bufferLength);
                }

                public void Dispose()
                {
                    byte[] b = Buffer;
                    if (b != null)
                    {
                        Buffer = null;
                        ArrayPool<byte>.Shared.Return(b);
                    }
                }

                public void SetTaskOffsetCount(Task<int> task, int offset, int count)
                {
                    Debug.Assert(offset >= 0, "Offset should never be negative");
                    Debug.Assert(count >= 0, "Count should never be negative");
                    Debug.Assert(offset <= count, "Offset should never be greater than count");

                    Task = task;
                    Offset = offset;
                    Count = count;
                }
            }

            internal void StoreLastEffectiveUri()
            {
                IntPtr urlCharPtr; // do not free; will point to libcurl private memory
                CURLcode urlResult = Interop.Http.EasyGetInfoPointer(_easyHandle, Interop.Http.CURLINFO.CURLINFO_EFFECTIVE_URL, out urlCharPtr);
                if (urlResult == CURLcode.CURLE_OK && urlCharPtr != IntPtr.Zero)
                {
                    string url = Marshal.PtrToStringAnsi(urlCharPtr);
                    if (url != _requestMessage.RequestUri.OriginalString)
                    {
                        Uri finalUri;
                        if (Uri.TryCreate(url, UriKind.Absolute, out finalUri))
                        {
                            _requestMessage.RequestUri = finalUri;
                        }
                    }
                    return;
                }

                Debug.Fail("Expected to be able to get the last effective Uri from libcurl");
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
