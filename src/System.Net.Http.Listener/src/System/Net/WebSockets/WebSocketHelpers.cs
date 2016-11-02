// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.WebSockets
{
    internal static class WebSocketHelpers
    {
        internal const string SecWebSocketKeyGuid = "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";
        internal const string WebSocketUpgradeToken = "websocket";
        internal const int DefaultReceiveBufferSize = 16 * 1024;
        internal const int DefaultClientSendBufferSize = 16 * 1024;
        internal const int MaxControlFramePayloadLength = 123;

        // RFC 6455 requests WebSocket clients to let the server initiate the TCP close to avoid that client sockets 
        // end up in TIME_WAIT-state
        //
        // After both sending and receiving a Close message, an endpoint considers the WebSocket connection closed and 
        // MUST close the underlying TCP connection.  The server MUST close the underlying TCP connection immediately; 
        // the client SHOULD wait for the server to close the connection but MAY close the connection at any time after
        // sending and receiving a Close message, e.g., if it has not received a TCP Close from the server in a 
        // reasonable time period.
        internal const int ClientTcpCloseTimeout = 1000; // 1s

        private const int CloseStatusCodeAbort = 1006;
        private const int CloseStatusCodeFailedTLSHandshake = 1015;
        private const int InvalidCloseStatusCodesFrom = 0;
        private const int InvalidCloseStatusCodesTo = 999;
        private const string Separators = "()<>@,;:\\\"/[]?={} ";

        private static readonly ArraySegment<byte> s_EmptyPayload = new ArraySegment<byte>(new byte[] { }, 0, 0);
        private static readonly Random s_KeyGenerator = new Random();
        private static readonly bool s_HttpSysSupportsWebSockets = (Environment.OSVersion.Version >= new Version(6, 2));

        internal static ArraySegment<byte> EmptyPayload
        {
            get { return s_EmptyPayload; }
        }

        internal static Task<HttpListenerWebSocketContext> AcceptWebSocketAsync(HttpListenerContext context,
            string subProtocol,
            int receiveBufferSize,
            TimeSpan keepAliveInterval,
            ArraySegment<byte> internalBuffer)
        {
            WebSocketHelpers.ValidateOptions(subProtocol, receiveBufferSize, WebSocketBuffer.MinSendBufferSize, keepAliveInterval);
            WebSocketHelpers.ValidateArraySegment<byte>(internalBuffer, "internalBuffer");
            WebSocketBuffer.Validate(internalBuffer.Count, receiveBufferSize, WebSocketBuffer.MinSendBufferSize, true);

            return AcceptWebSocketAsyncCore(context, subProtocol, receiveBufferSize, keepAliveInterval, internalBuffer);
        }

        private static async Task<HttpListenerWebSocketContext> AcceptWebSocketAsyncCore(HttpListenerContext context,
            string subProtocol,
            int receiveBufferSize,
            TimeSpan keepAliveInterval,
            ArraySegment<byte> internalBuffer)
        {
            HttpListenerWebSocketContext webSocketContext = null;
            //if (NetEventSource.Log.IsEnabled())
            //{
            //    NetEventSource.Enter(NetEventSource.ComponentType.WebSocket, context, "AcceptWebSocketAsync", "");
            //}

            try
            {
                // get property will create a new response if one doesn't exist.
                HttpListenerResponse response = context.Response;
                HttpListenerRequest request = context.Request;
                ValidateWebSocketHeaders(context);

                string secWebSocketVersion = request.Headers[HttpKnownHeaderNames.SecWebSocketVersion];

                // Optional for non-browser client
                string origin = request.Headers[HttpKnownHeaderNames.Origin];

                List<string> secWebSocketProtocols = new List<string>();
                string outgoingSecWebSocketProtocolString;
                bool shouldSendSecWebSocketProtocolHeader =
                    WebSocketHelpers.ProcessWebSocketProtocolHeader(
                        request.Headers[HttpKnownHeaderNames.SecWebSocketProtocol],
                        subProtocol,
                        out outgoingSecWebSocketProtocolString);

                if (shouldSendSecWebSocketProtocolHeader)
                {
                    secWebSocketProtocols.Add(outgoingSecWebSocketProtocolString);
                    response.Headers.Add(HttpKnownHeaderNames.SecWebSocketProtocol,
                        outgoingSecWebSocketProtocolString);
                }

                // negotiate the websocket key return value
                string secWebSocketKey = request.Headers[HttpKnownHeaderNames.SecWebSocketKey];
                string secWebSocketAccept = WebSocketHelpers.GetSecWebSocketAcceptString(secWebSocketKey);

                response.Headers.Add(HttpKnownHeaderNames.Connection, HttpKnownHeaderNames.Upgrade);
                response.Headers.Add(HttpKnownHeaderNames.Upgrade, WebSocketUpgradeToken);
                response.Headers.Add(HttpKnownHeaderNames.SecWebSocketAccept, secWebSocketAccept);

                response.StatusCode = (int)HttpStatusCode.SwitchingProtocols; // HTTP 101                
                response.ComputeCoreHeaders();
                ulong hresult = SendWebSocketHeaders(response);
                if (hresult != 0)
                {
                    throw new WebSocketException((int)hresult,
                        SR.Format(SR.net_WebSockets_NativeSendResponseHeaders,
                        WebSocketHelpers.MethodNames.AcceptWebSocketAsync,
                        hresult));
                }

                //if (NetEventSource.Log.IsEnabled())
                //{
                //    NetEventSource.PrintInfo(NetEventSource.ComponentType.WebSocket, string.Format("{0} = {1}",
                //        HttpKnownHeaderNames.Origin, origin));
                //    NetEventSource.PrintInfo(NetEventSource.ComponentType.WebSocket, string.Format("{0} = {1}",
                //        HttpKnownHeaderNames.SecWebSocketVersion, secWebSocketVersion));
                //    NetEventSource.PrintInfo(NetEventSource.ComponentType.WebSocket, string.Format("{0} = {1}",
                //        HttpKnownHeaderNames.SecWebSocketKey, secWebSocketKey));
                //    NetEventSource.PrintInfo(NetEventSource.ComponentType.WebSocket, string.Format("{0} = {1}",
                //        HttpKnownHeaderNames.SecWebSocketAccept, secWebSocketAccept));
                //    NetEventSource.PrintInfo(NetEventSource.ComponentType.WebSocket, string.Format("Request  {0} = {1}",
                //        HttpKnownHeaderNames.SecWebSocketProtocol,
                //        request.Headers[HttpKnownHeaderNames.SecWebSocketProtocol]));
                //    NetEventSource.PrintInfo(NetEventSource.ComponentType.WebSocket, string.Format("Response {0} = {1}",
                //        HttpKnownHeaderNames.SecWebSocketProtocol, outgoingSecWebSocketProtocolString));
                //}

                await response.OutputStream.FlushAsync().SuppressContextFlow();

                HttpResponseStream responseStream = response.OutputStream as HttpResponseStream;
                Debug.Assert(responseStream != null, "'responseStream' MUST be castable to System.Net.HttpResponseStream.");
                ((HttpResponseStream)response.OutputStream).SwitchToOpaqueMode();
                HttpRequestStream requestStream = new HttpRequestStream(context);
                requestStream.SwitchToOpaqueMode();
                WebSocketHttpListenerDuplexStream webSocketStream =
                    new WebSocketHttpListenerDuplexStream(requestStream, responseStream, context);
                WebSocket webSocket = ServerWebSocket.Create(webSocketStream,
                    subProtocol,
                    receiveBufferSize,
                    keepAliveInterval,
                    internalBuffer);

                webSocketContext = new HttpListenerWebSocketContext(
                                                                    request.Url,
                                                                    request.Headers,
                                                                    request.Cookies,
                                                                    context.User,
                                                                    request.IsAuthenticated,
                                                                    request.IsLocal,
                                                                    request.IsSecureConnection,
                                                                    origin,
                                                                    secWebSocketProtocols.AsReadOnly(),
                                                                    secWebSocketVersion,
                                                                    secWebSocketKey,
                                                                    webSocket);

                //if (NetEventSource.Log.IsEnabled())
                //{
                //    NetEventSource.Associate(NetEventSource.ComponentType.WebSocket, context, webSocketContext);
                //    NetEventSource.Associate(NetEventSource.ComponentType.WebSocket, webSocketContext, webSocket);
                //}
            }
            //catch (Exception ex)
            //{
            //    if (NetEventSource.Log.IsEnabled())
            //    {
            //        NetEventSource.Exception(NetEventSource.ComponentType.WebSocket, context, "AcceptWebSocketAsync", ex);
            //    }
            //    throw;
            //}
            finally
            {
                //if (NetEventSource.Log.IsEnabled())
                //{
                //    NetEventSource.Exit(NetEventSource.ComponentType.WebSocket, context, "AcceptWebSocketAsync", "");
                //}
            }

            return webSocketContext;
        }

        [SuppressMessage("Microsoft.Security", "CA5350", Justification = "SHA1 used only for hashing purposes, not for crypto.")]
        internal static string GetSecWebSocketAcceptString(string secWebSocketKey)
        {
            string retVal;

            // SHA1 used only for hashing purposes, not for crypto. Check here for FIPS compat.
            using (SHA1 sha1 = SHA1.Create())
            {
                string acceptString = string.Concat(secWebSocketKey, WebSocketHelpers.SecWebSocketKeyGuid);
                byte[] toHash = Encoding.UTF8.GetBytes(acceptString);
                retVal = Convert.ToBase64String(sha1.ComputeHash(toHash));
            }

            return retVal;
        }

        internal static string GetTraceMsgForParameters(int offset, int count, CancellationToken cancellationToken)
        {
            return string.Format(CultureInfo.InvariantCulture,
                "offset: {0}, count: {1}, cancellationToken.CanBeCanceled: {2}",
                offset,
                count,
                cancellationToken.CanBeCanceled);
        }

        // return value here signifies if a Sec-WebSocket-Protocol header should be returned by the server. 
        internal static bool ProcessWebSocketProtocolHeader(string clientSecWebSocketProtocol,
            string subProtocol,
            out string acceptProtocol)
        {
            acceptProtocol = string.Empty;
            if (string.IsNullOrEmpty(clientSecWebSocketProtocol))
            {
                // client hasn't specified any Sec-WebSocket-Protocol header
                if (subProtocol != null)
                {
                    // If the server specified _anything_ this isn't valid.
                    throw new WebSocketException(WebSocketError.UnsupportedProtocol,
                        SR.Format(SR.net_WebSockets_ClientAcceptingNoProtocols, subProtocol));
                }
                // Treat empty and null from the server as the same thing here, server should not send headers. 
                return false;
            }

            // here, we know the client specified something and it's non-empty.

            if (subProtocol == null)
            {
                // client specified some protocols, server specified 'null'. So server should send headers.                 
                return true;
            }

            // here, we know that the client has specified something, it's not empty
            // and the server has specified exactly one protocol

            string[] requestProtocols = clientSecWebSocketProtocol.Split(new char[] { ',' },
                StringSplitOptions.RemoveEmptyEntries);
            acceptProtocol = subProtocol;

            // client specified protocols, serverOptions has exactly 1 non-empty entry. Check that 
            // this exists in the list the client specified. 
            for (int i = 0; i < requestProtocols.Length; i++)
            {
                string currentRequestProtocol = requestProtocols[i].Trim();
                if (string.Compare(acceptProtocol, currentRequestProtocol, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return true;
                }
            }

            throw new WebSocketException(WebSocketError.UnsupportedProtocol,
                SR.Format(SR.net_WebSockets_AcceptUnsupportedProtocol,
                    clientSecWebSocketProtocol,
                    subProtocol));
        }

        internal static ConfiguredTaskAwaitable SuppressContextFlow(this Task task)
        {
            // We don't flow the synchronization context within WebSocket.xxxAsync - but the calling application
            // can decide whether the completion callback for the task returned from WebSocket.xxxAsync runs
            // under the caller's synchronization context.
            return task.ConfigureAwait(false);
        }

        internal static ConfiguredTaskAwaitable<T> SuppressContextFlow<T>(this Task<T> task)
        {
            // We don't flow the synchronization context within WebSocket.xxxAsync - but the calling application
            // can decide whether the completion callback for the task returned from WebSocket.xxxAsync runs
            // under the caller's synchronization context.
            return task.ConfigureAwait(false);
        }

        internal static void ValidateBuffer(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            if (offset < 0 || offset > buffer.Length)
            {
                throw new ArgumentOutOfRangeException("offset");
            }

            if (count < 0 || count > (buffer.Length - offset))
            {
                throw new ArgumentOutOfRangeException("count");
            }
        }

        private static unsafe ulong SendWebSocketHeaders(HttpListenerResponse response)
        {
            return response.SendHeaders(null, null,
                Interop.HttpApi.HTTP_FLAGS.HTTP_SEND_RESPONSE_FLAG_OPAQUE |
                Interop.HttpApi.HTTP_FLAGS.HTTP_SEND_RESPONSE_FLAG_MORE_DATA |
                Interop.HttpApi.HTTP_FLAGS.HTTP_SEND_RESPONSE_FLAG_BUFFER_DATA,
                true);
        }

        private static void ValidateWebSocketHeaders(HttpListenerContext context)
        {
            EnsureHttpSysSupportsWebSockets();

            if (!context.Request.IsWebSocketRequest)
            {
                throw new WebSocketException(WebSocketError.NotAWebSocket,
                    SR.Format(SR.net_WebSockets_AcceptNotAWebSocket,
                    WebSocketHelpers.MethodNames.ValidateWebSocketHeaders,
                    HttpKnownHeaderNames.Connection,
                    HttpKnownHeaderNames.Upgrade,
                    WebSocketHelpers.WebSocketUpgradeToken,
                    context.Request.Headers[HttpKnownHeaderNames.Upgrade]));
            }

            string secWebSocketVersion = context.Request.Headers[HttpKnownHeaderNames.SecWebSocketVersion];
            if (string.IsNullOrEmpty(secWebSocketVersion))
            {
                throw new WebSocketException(WebSocketError.HeaderError,
                    SR.Format(SR.net_WebSockets_AcceptHeaderNotFound,
                    WebSocketHelpers.MethodNames.ValidateWebSocketHeaders,
                    HttpKnownHeaderNames.SecWebSocketVersion));
            }

            if (string.Compare(secWebSocketVersion, WebSocketProtocolComponent.SupportedVersion, StringComparison.OrdinalIgnoreCase) != 0)
            {
                throw new WebSocketException(WebSocketError.UnsupportedVersion,
                    SR.Format(SR.net_WebSockets_AcceptUnsupportedWebSocketVersion,
                    WebSocketHelpers.MethodNames.ValidateWebSocketHeaders,
                    secWebSocketVersion,
                    WebSocketProtocolComponent.SupportedVersion));
            }

            if (string.IsNullOrWhiteSpace(context.Request.Headers[HttpKnownHeaderNames.SecWebSocketKey]))
            {
                throw new WebSocketException(WebSocketError.HeaderError,
                    SR.Format(SR.net_WebSockets_AcceptHeaderNotFound,
                    WebSocketHelpers.MethodNames.ValidateWebSocketHeaders,
                    HttpKnownHeaderNames.SecWebSocketKey));
            }
        }

        internal static void PrepareWebRequest(ref HttpWebRequest request)
        {
            request.Connection = HttpKnownHeaderNames.Upgrade;
            request.Headers[HttpKnownHeaderNames.Upgrade] = WebSocketUpgradeToken;

            byte[] keyBlob = new byte[16];
            lock (s_KeyGenerator)
            {
                s_KeyGenerator.NextBytes(keyBlob);
            }

            request.Headers[HttpKnownHeaderNames.SecWebSocketKey] = Convert.ToBase64String(keyBlob);
            if (WebSocketProtocolComponent.IsSupported)
            {
                request.Headers[HttpKnownHeaderNames.SecWebSocketVersion] = WebSocketProtocolComponent.SupportedVersion;
            }
        }

        internal static void ValidateSubprotocol(string subProtocol)
        {
            if (string.IsNullOrWhiteSpace(subProtocol))
            {
                throw new ArgumentException(SR.net_WebSockets_InvalidEmptySubProtocol, "subProtocol");
            }

            char[] chars = subProtocol.ToCharArray();
            string invalidChar = null;
            int i = 0;
            while (i < chars.Length)
            {
                char ch = chars[i];
                if (ch < 0x21 || ch > 0x7e)
                {
                    invalidChar = string.Format(CultureInfo.InvariantCulture, "[{0}]", (int)ch);
                    break;
                }

                if (!char.IsLetterOrDigit(ch) &&
                    Separators.IndexOf(ch) >= 0)
                {
                    invalidChar = ch.ToString();
                    break;
                }

                i++;
            }

            if (invalidChar != null)
            {
                throw new ArgumentException(SR.Format(SR.net_WebSockets_InvalidCharInProtocolString, subProtocol, invalidChar),
                    "subProtocol");
            }
        }

        internal static void ValidateCloseStatus(WebSocketCloseStatus closeStatus, string statusDescription)
        {
            if (closeStatus == WebSocketCloseStatus.Empty && !string.IsNullOrEmpty(statusDescription))
            {
                throw new ArgumentException(SR.Format(SR.net_WebSockets_ReasonNotNull,
                    statusDescription,
                    WebSocketCloseStatus.Empty),
                    "statusDescription");
            }

            int closeStatusCode = (int)closeStatus;

            if ((closeStatusCode >= InvalidCloseStatusCodesFrom &&
                closeStatusCode <= InvalidCloseStatusCodesTo) ||
                closeStatusCode == CloseStatusCodeAbort ||
                closeStatusCode == CloseStatusCodeFailedTLSHandshake)
            {
                // CloseStatus 1006 means Aborted - this will never appear on the wire and is reflected by calling WebSocket.Abort
                throw new ArgumentException(SR.Format(SR.net_WebSockets_InvalidCloseStatusCode,
                    closeStatusCode),
                    "closeStatus");
            }

            int length = 0;
            if (!string.IsNullOrEmpty(statusDescription))
            {
                length = Encoding.UTF8.GetByteCount(statusDescription);
            }

            if (length > WebSocketHelpers.MaxControlFramePayloadLength)
            {
                throw new ArgumentException(SR.Format(SR.net_WebSockets_InvalidCloseStatusDescription,
                    statusDescription,
                    WebSocketHelpers.MaxControlFramePayloadLength),
                    "statusDescription");
            }
        }

        internal static void ValidateOptions(string subProtocol,
            int receiveBufferSize,
            int sendBufferSize,
            TimeSpan keepAliveInterval)
        {
            // We allow the subProtocol to be null. Validate if it is not null.
            if (subProtocol != null)
            {
                ValidateSubprotocol(subProtocol);
            }

            ValidateBufferSizes(receiveBufferSize, sendBufferSize);

            if (keepAliveInterval < Timeout.InfiniteTimeSpan) // -1
            {
                throw new ArgumentOutOfRangeException("keepAliveInterval", keepAliveInterval,
                    SR.Format(SR.net_WebSockets_ArgumentOutOfRange_TooSmall, Timeout.InfiniteTimeSpan.ToString()));
            }
        }

        internal static void ValidateBufferSizes(int receiveBufferSize, int sendBufferSize)
        {
            if (receiveBufferSize < WebSocketBuffer.MinReceiveBufferSize)
            {
                throw new ArgumentOutOfRangeException("receiveBufferSize", receiveBufferSize,
                    SR.Format(SR.net_WebSockets_ArgumentOutOfRange_TooSmall, WebSocketBuffer.MinReceiveBufferSize));
            }

            if (sendBufferSize < WebSocketBuffer.MinSendBufferSize)
            {
                throw new ArgumentOutOfRangeException("sendBufferSize", sendBufferSize,
                    SR.Format(SR.net_WebSockets_ArgumentOutOfRange_TooSmall, WebSocketBuffer.MinSendBufferSize));
            }

            if (receiveBufferSize > WebSocketBuffer.MaxBufferSize)
            {
                throw new ArgumentOutOfRangeException("receiveBufferSize", receiveBufferSize,
                    SR.Format(SR.net_WebSockets_ArgumentOutOfRange_TooBig,
                        "receiveBufferSize",
                        receiveBufferSize,
                        WebSocketBuffer.MaxBufferSize));
            }

            if (sendBufferSize > WebSocketBuffer.MaxBufferSize)
            {
                throw new ArgumentOutOfRangeException("sendBufferSize", sendBufferSize,
                    SR.Format(SR.net_WebSockets_ArgumentOutOfRange_TooBig,
                        "sendBufferSize",
                        sendBufferSize,
                        WebSocketBuffer.MaxBufferSize));
            }
        }

        internal static void ValidateInnerStream(Stream innerStream)
        {
            if (innerStream == null)
            {
                throw new ArgumentNullException("innerStream");
            }

            if (!innerStream.CanRead)
            {
                throw new ArgumentException(SR.net_writeonlystream, "innerStream");
            }

            if (!innerStream.CanWrite)
            {
                throw new ArgumentException(SR.net_readonlystream, "innerStream");
            }
        }

        internal static void ThrowIfConnectionAborted(Stream connection, bool read)
        {
            if ((!read && !connection.CanWrite) ||
                (read && !connection.CanRead))
            {
                throw new WebSocketException(WebSocketError.ConnectionClosedPrematurely);
            }
        }

        internal static void ThrowPlatformNotSupportedException_WSPC()
        {
            throw new PlatformNotSupportedException(SR.net_WebSockets_UnsupportedPlatform);
        }

        private static void ThrowPlatformNotSupportedException_HTTPSYS()
        {
            throw new PlatformNotSupportedException(SR.net_WebSockets_UnsupportedPlatform);
        }

        internal static void ValidateArraySegment<T>(ArraySegment<T> arraySegment, string parameterName)
        {
            Debug.Assert(!string.IsNullOrEmpty(parameterName), "'parameterName' MUST NOT be NULL or string.Empty");

            if (arraySegment.Array == null)
            {
                throw new ArgumentNullException(parameterName + ".Array");
            }

            if (arraySegment.Offset < 0 || arraySegment.Offset > arraySegment.Array.Length)
            {
                throw new ArgumentOutOfRangeException(parameterName + ".Offset");
            }
            if (arraySegment.Count < 0 || arraySegment.Count > (arraySegment.Array.Length - arraySegment.Offset))
            {
                throw new ArgumentOutOfRangeException(parameterName + ".Count");
            }
        }

        private static void EnsureHttpSysSupportsWebSockets()
        {
            if (!s_HttpSysSupportsWebSockets)
            {
                ThrowPlatformNotSupportedException_HTTPSYS();
            }
        }

        internal static class MethodNames
        {
            internal const string AcceptWebSocketAsync = "AcceptWebSocketAsync";
            internal const string ValidateWebSocketHeaders = "ValidateWebSocketHeaders";
        }
    }
}
