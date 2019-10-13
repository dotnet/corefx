// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace System.Net.Test.Common
{
    public sealed partial class LoopbackServer : GenericLoopbackServer, IDisposable
    {
        private Socket _listenSocket;
        private Options _options;
        private Uri _uri;

        // Use CreateServerAsync or similar to create
        private LoopbackServer(Socket listenSocket, Options options)
        {
            _listenSocket = listenSocket;
            _options = options;

            var localEndPoint = (IPEndPoint)listenSocket.LocalEndPoint;
            string host = options.Address.AddressFamily == AddressFamily.InterNetworkV6 ?
                $"[{localEndPoint.Address}]" :
                localEndPoint.Address.ToString();

            string scheme = options.UseSsl ? "https" : "http";
            if (options.WebSocketEndpoint)
            {
                scheme = options.UseSsl ? "wss" : "ws";
            }

            _uri = new Uri($"{scheme}://{host}:{localEndPoint.Port}/");
        }

        public override void Dispose()
        {
            if (_listenSocket != null)
            {
                _listenSocket.Dispose();
                _listenSocket = null;
            }
        }

        public Socket ListenSocket => _listenSocket;
        public Uri Uri => _uri;

        public static async Task CreateServerAsync(Func<LoopbackServer, Task> funcAsync, Options options = null)
        {
            options = options ?? new Options();

            using (var listenSocket = new Socket(options.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
            {
                listenSocket.Bind(new IPEndPoint(options.Address, 0));
                listenSocket.Listen(options.ListenBacklog);

                using (var server = new LoopbackServer(listenSocket, options))
                {
                    await funcAsync(server).ConfigureAwait(false);
                }
            }
        }

        public static Task CreateServerAsync(Func<LoopbackServer, Uri, Task> funcAsync, Options options = null)
        {
            return CreateServerAsync(server => funcAsync(server, server.Uri), options);
        }

        public static Task CreateClientAndServerAsync(Func<Uri, Task> clientFunc, Func<LoopbackServer, Task> serverFunc, Options options = null)
        {
            return CreateServerAsync(async server =>
            {
                Task clientTask = clientFunc(server.Uri);
                Task serverTask = serverFunc(server);

                await new Task[] { clientTask, serverTask }.WhenAllOrAnyFailed().ConfigureAwait(false);
            }, options);
        }

        public async Task<Connection> EstablishConnectionAsync()
        {
            Socket s = await _listenSocket.AcceptAsync().ConfigureAwait(false);
            try
            {
                s.NoDelay = true;

                Stream stream = new NetworkStream(s, ownsSocket: false);
                if (_options.UseSsl)
                {
                    var sslStream = new SslStream(stream, false, delegate { return true; });
                    using (var cert = Configuration.Certificates.GetServerCertificate())
                    {
                        await sslStream.AuthenticateAsServerAsync(
                            cert,
                            clientCertificateRequired: true, // allowed but not required
                            enabledSslProtocols: _options.SslProtocols,
                            checkCertificateRevocation: false).ConfigureAwait(false);
                    }
                    stream = sslStream;
                }

                if (_options.StreamWrapper != null)
                {
                    stream = _options.StreamWrapper(stream);
                }

                return new Connection(s, stream);
            }
            catch (Exception)
            {
                s.Close();
                throw;
            }
        }

        public async Task AcceptConnectionAsync(Func<Connection, Task> funcAsync)
        {
            using (Connection connection = await EstablishConnectionAsync().ConfigureAwait(false))
            {
                await funcAsync(connection).ConfigureAwait(false);
            }
        }

        public async Task<List<string>> AcceptConnectionSendCustomResponseAndCloseAsync(string response)
        {
            List<string> lines = null;

            // Note, we assume there's no request body.
            // We'll close the connection after reading the request header and sending the response.
            await AcceptConnectionAsync(async connection =>
            {
                lines = await connection.ReadRequestHeaderAndSendCustomResponseAsync(response).ConfigureAwait(false);
            }).ConfigureAwait(false);

            return lines;
        }

        public async Task<List<string>> AcceptConnectionSendCustomResponseAndCloseAsync(byte[] response)
        {
            List<string> lines = null;

            // Note, we assume there's no request body.
            // We'll close the connection after reading the request header and sending the response.
            await AcceptConnectionAsync(async connection =>
            {
                lines = await connection.ReadRequestHeaderAndSendCustomResponseAsync(response).ConfigureAwait(false);
            }).ConfigureAwait(false);

            return lines;
        }

        public async Task<List<string>> AcceptConnectionSendResponseAndCloseAsync(HttpStatusCode statusCode = HttpStatusCode.OK, string additionalHeaders = null, string content = null)
        {
            List<string> lines = null;

            // Note, we assume there's no request body.
            // We'll close the connection after reading the request header and sending the response.
            await AcceptConnectionAsync(async connection =>
            {
                lines = await connection.ReadRequestHeaderAndSendResponseAsync(statusCode, additionalHeaders + "Connection: close\r\n", content).ConfigureAwait(false);
            }).ConfigureAwait(false);

            return lines;
        }

        public static string GetRequestHeaderValue(List<string> headers, string name)
        {
            var sep = new char[] { ':' };
            foreach (string line in headers)
            {
                string[] tokens = line.Split(sep, 2);
                if (name.Equals(tokens[0], StringComparison.InvariantCultureIgnoreCase))
                {
                    return tokens[1].Trim();
                }
            }
            return null;
        }

        public static string GetRequestMethod(List<string> headers)
        {

            if (headers != null && headers.Count > 1)
            {
                return headers[0].Split()[1].Trim();
            }
            return null;
        }

        // Stolen from HttpStatusDescription code in the product code
        private static string GetStatusDescription(HttpStatusCode code)
        {
            switch ((int)code)
            {
                case 100:
                    return "Continue";
                case 101:
                    return "Switching Protocols";
                case 102:
                    return "Processing";

                case 200:
                    return "OK";
                case 201:
                    return "Created";
                case 202:
                    return "Accepted";
                case 203:
                    return "Non-Authoritative Information";
                case 204:
                    return "No Content";
                case 205:
                    return "Reset Content";
                case 206:
                    return "Partial Content";
                case 207:
                    return "Multi-Status";

                case 300:
                    return "Multiple Choices";
                case 301:
                    return "Moved Permanently";
                case 302:
                    return "Found";
                case 303:
                    return "See Other";
                case 304:
                    return "Not Modified";
                case 305:
                    return "Use Proxy";
                case 307:
                    return "Temporary Redirect";

                case 400:
                    return "Bad Request";
                case 401:
                    return "Unauthorized";
                case 402:
                    return "Payment Required";
                case 403:
                    return "Forbidden";
                case 404:
                    return "Not Found";
                case 405:
                    return "Method Not Allowed";
                case 406:
                    return "Not Acceptable";
                case 407:
                    return "Proxy Authentication Required";
                case 408:
                    return "Request Timeout";
                case 409:
                    return "Conflict";
                case 410:
                    return "Gone";
                case 411:
                    return "Length Required";
                case 412:
                    return "Precondition Failed";
                case 413:
                    return "Request Entity Too Large";
                case 414:
                    return "Request-Uri Too Long";
                case 415:
                    return "Unsupported Media Type";
                case 416:
                    return "Requested Range Not Satisfiable";
                case 417:
                    return "Expectation Failed";
                case 422:
                    return "Unprocessable Entity";
                case 423:
                    return "Locked";
                case 424:
                    return "Failed Dependency";
                case 426:
                    return "Upgrade Required"; // RFC 2817

                case 500:
                    return "Internal Server Error";
                case 501:
                    return "Not Implemented";
                case 502:
                    return "Bad Gateway";
                case 503:
                    return "Service Unavailable";
                case 504:
                    return "Gateway Timeout";
                case 505:
                    return "Http Version Not Supported";
                case 507:
                    return "Insufficient Storage";
            }
            return null;
        }

        public enum ContentMode
        {
            ContentLength,
            SingleChunk,
            BytePerChunk,
            ConnectionClose
        }

        public static string GetContentModeResponse(ContentMode mode, string content, bool connectionClose = false)
        {
            switch (mode)
            {
                case ContentMode.ContentLength:
                    return GetHttpResponse(content: content, connectionClose: connectionClose);
                case ContentMode.SingleChunk:
                    return GetSingleChunkHttpResponse(content: content, connectionClose: connectionClose);
                case ContentMode.BytePerChunk:
                    return GetBytePerChunkHttpResponse(content: content, connectionClose: connectionClose);
                case ContentMode.ConnectionClose:
                    Assert.True(connectionClose);
                    return GetConnectionCloseResponse(content: content);
                default:
                    Assert.True(false, $"Unknown content mode: {mode}");
                    return null;
            }
        }

        public static string GetHttpResponse(HttpStatusCode statusCode = HttpStatusCode.OK, string additionalHeaders = null, string content = null, bool connectionClose = false) =>
            GetHttpResponseHeaders(statusCode, additionalHeaders, content, connectionClose) +
            content;

        public static string GetHttpResponseHeaders(HttpStatusCode statusCode = HttpStatusCode.OK, string additionalHeaders = null, string content = null, bool connectionClose = false) =>
            GetHttpResponseHeaders(statusCode, additionalHeaders, content == null ? 0 : content.Length, connectionClose);

        public static string GetHttpResponseHeaders(HttpStatusCode statusCode = HttpStatusCode.OK, string additionalHeaders = null, int contentLength = 0, bool connectionClose = false) =>
            $"HTTP/1.1 {(int)statusCode} {GetStatusDescription(statusCode)}\r\n" +
            (connectionClose ? "Connection: close\r\n" : "") +
            $"Date: {DateTimeOffset.UtcNow:R}\r\n" +
            $"Content-Length: {contentLength}\r\n" +
            additionalHeaders +
            "\r\n";

        public static string GetSingleChunkHttpResponse(HttpStatusCode statusCode = HttpStatusCode.OK, string additionalHeaders = null, string content = null, bool connectionClose = false) =>
            $"HTTP/1.1 {(int)statusCode} {GetStatusDescription(statusCode)}\r\n" +
            (connectionClose ? "Connection: close\r\n" : "") +
            $"Date: {DateTimeOffset.UtcNow:R}\r\n" +
            "Transfer-Encoding: chunked\r\n" +
            additionalHeaders +
            "\r\n" +
            (string.IsNullOrEmpty(content) ? "" :
                $"{content.Length:X}\r\n" +
                $"{content}\r\n") +
            $"0\r\n" +
            $"\r\n";

        public static string GetBytePerChunkHttpResponse(HttpStatusCode statusCode = HttpStatusCode.OK, string additionalHeaders = null, string content = null, bool connectionClose = false) =>
            $"HTTP/1.1 {(int)statusCode} {GetStatusDescription(statusCode)}\r\n" +
            (connectionClose ? "Connection: close\r\n" : "") +
            $"Date: {DateTimeOffset.UtcNow:R}\r\n" +
            "Transfer-Encoding: chunked\r\n" +
            additionalHeaders +
            "\r\n" +
            (string.IsNullOrEmpty(content) ? "" : string.Concat(content.Select(c => $"1\r\n{c}\r\n"))) +
            $"0\r\n" +
            $"\r\n";

        public static string GetConnectionCloseResponse(HttpStatusCode statusCode = HttpStatusCode.OK, string additionalHeaders = null, string content = null) =>
            $"HTTP/1.1 {(int)statusCode} {GetStatusDescription(statusCode)}\r\n" +
            "Connection: close\r\n" +
            $"Date: {DateTimeOffset.UtcNow:R}\r\n" +
            additionalHeaders +
            "\r\n" +
            content;

        public class Options : GenericLoopbackOptions
        {
            public int ListenBacklog { get; set; } = 1;
            public bool UseSsl { get; set; } = false;
            public SslProtocols SslProtocols { get; set; } =
#if !NETSTANDARD2_0
                SslProtocols.Tls13 |
#endif
                SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12;
            public bool WebSocketEndpoint { get; set; } = false;
            public Func<Stream, Stream> StreamWrapper { get; set; }
            public string Username { get; set; }
            public string Domain { get; set; }
            public string Password { get; set; }
            public bool IsProxy { get; set; } = false;
        }

        public sealed class Connection : GenericLoopbackConnection
        {
            private const int BufferSize = 4000;
            private Socket _socket;
            private Stream _stream;
            private StreamWriter _writer;
            private byte[] _readBuffer;
            private int _readStart;
            private int _readEnd;
            private int _contentLength = 0;
            private bool _bodyRead = false;

            public Connection(Socket socket, Stream stream)
            {
                _socket = socket;
                _stream = stream;

                _writer = new StreamWriter(stream, Encoding.ASCII) { AutoFlush = true };

                _readBuffer = new byte[BufferSize];
                _readStart = 0;
                _readEnd = 0;
            }

            public Socket Socket => _socket;
            public Stream Stream => _stream;
            public StreamWriter Writer => _writer;

            public async Task<int> ReadAsync(Memory<byte> buffer, int offset, int size)
            {
                if (_readEnd - _readStart > 0)
                {
                    // Use buffered data first.
                    int copyLength = Math.Min(size, _readEnd - _readStart);
                    Memory<byte> source = new Memory<byte>(_readBuffer).Slice(_readStart, copyLength);
                    source.CopyTo(buffer.Slice(offset));

                    _readStart += copyLength;
                    return copyLength;
                }
#if NETSTANDARD2_0
                // stream does not have Memory<t> overload
                byte[] tempBuffer = new byte[size];
                int readLength = await _stream.ReadAsync(tempBuffer, 0, size).ConfigureAwait(false);
                if (readLength > 0)
                {
                    tempBuffer.AsSpan(readLength).CopyTo(buffer.Span.Slice(offset, size));
                }

                return readLength;
#else
                return await _stream.ReadAsync(buffer.Slice(offset, size)).ConfigureAwait(false);
#endif
            }

            // Read until we either get requested data or we hit end of stream.
            public async Task<int> ReadBlockAsync(Memory<byte> buffer, int offset, int size)
            {
                int totalLength = 0;

                while (size != 0)
                {
                    int readLength = await ReadAsync(buffer, offset, size).ConfigureAwait(false);
                    if (readLength == 0)
                    {
                        throw new Exception("Unexpected EOF trying to read");
                    }

                    totalLength += readLength;
                    offset += readLength;
                    size -= readLength;
                }

                return totalLength;
            }

            public async Task<int> ReadBlockAsync(char[] result, int offset, int size)
            {
                byte[] buffer = new byte[size];
                int readLength = await ReadBlockAsync(buffer, 0, size).ConfigureAwait(false);

                string asString = System.Text.Encoding.ASCII.GetString(buffer, 0, readLength);

                for (int i = 0; i < readLength; i++)
                {
                    result[offset + i] = asString[i];
                }

                return readLength;
            }

            public async Task<string> ReadToEndAsync()
            {
                byte[] buffer = new byte[BufferSize];
                int offset = 0;
                int totalLength = 0;
                int bytesRead;

                do
                {
                    bytesRead = await ReadAsync(buffer, offset, buffer.Length - offset).ConfigureAwait(false);
                    totalLength += bytesRead;
                    offset += bytesRead;

                    if (bytesRead == buffer.Length)
                    {
                        byte[] newBuffer = new byte[buffer.Length + BufferSize];
                        buffer.CopyTo(newBuffer, 0);
                        offset = buffer.Length;
                        buffer = newBuffer;
                    }
                } while (bytesRead > 0);

                return System.Text.Encoding.ASCII.GetString(buffer, 0, totalLength);
            }

            public string ReadLine()
            {
                return ReadLineAsync().GetAwaiter().GetResult();
            }

            public async Task<string> ReadLineAsync()
            {
                int index = 0;
                int startSearch = _readStart;

                while (true)
                {
                    if (_readStart == _readEnd || index == -1)
                    {
                        // We either have no data or we did not find LF in stream.
                        // In either case, read more.
                        if (_readEnd + 2 > _readBuffer.Length)
                        {
                            // We no longer have space to read CRLF. Allocate new buffer and start over.
                            byte[] newBuffer = new byte[_readBuffer.Length + BufferSize];
                            int dataLength = _readEnd - _readStart;
                            if (dataLength > 0)
                            {
                                Array.Copy(_readBuffer, _readStart, newBuffer, 0, dataLength);
                                _readStart = 0;
                                _readEnd = dataLength;
                                _readBuffer = newBuffer;
                            }
                        }

                        int bytesRead = await _stream.ReadAsync(_readBuffer, _readEnd, _readBuffer.Length - _readEnd).ConfigureAwait(false);
                        if (bytesRead == 0)
                        {
                            break;
                        }

                        _readEnd += bytesRead;
                    }

                    index = Array.IndexOf(_readBuffer, (byte)'\n', startSearch, _readEnd - startSearch);
                    if (index == -1)
                    {
                        // We did not find it, look for more data.
                        startSearch = _readEnd;
                        continue;
                    }

                    int stringLength = index - _readStart;
                    // Consume CRLF if present.
                    if (_readBuffer[_readStart + stringLength] == '\n') { stringLength--; }
                    if (_readBuffer[_readStart + stringLength] == '\r') { stringLength--; }

                    string line = System.Text.Encoding.ASCII.GetString(_readBuffer, _readStart, stringLength + 1);
                    _readStart = index + 1;
                    return line;
                }

                return null;
            }

            public override void Dispose()
            {
                try
                {
                    // Try to shutdown the send side of the socket.
                    // This seems to help avoid connection reset issues caused by buffered data
                    // that has not been sent/acked when the graceful shutdown timeout expires.
                    // This may throw if the socket was already closed, so eat any exception.
                    _socket.Shutdown(SocketShutdown.Send);
                }
                catch (Exception) { }

                _writer.Dispose();
                _stream.Dispose();
                _socket.Dispose();
            }

            public async Task<List<string>> ReadRequestHeaderAsync()
            {
                var lines = new List<string>();
                string line;
                while (!string.IsNullOrEmpty(line = await ReadLineAsync().ConfigureAwait(false)))
                {
                    lines.Add(line);
                }

                if (line == null)
                {
                    throw new IOException("Unexpected EOF trying to read request header");
                }

                return lines;
            }

            public async Task SendResponseAsync(string response)
            {
                await _writer.WriteAsync(response).ConfigureAwait(false);
            }

            public async Task SendResponseAsync(HttpStatusCode statusCode = HttpStatusCode.OK, string additionalHeaders = null, string content = null)
            {
                await SendResponseAsync(GetHttpResponse(statusCode, additionalHeaders, content)).ConfigureAwait(false);
            }

            public async Task<List<string>> ReadRequestHeaderAndSendCustomResponseAsync(string response)
            {
                List<string> lines = await ReadRequestHeaderAsync().ConfigureAwait(false);
                await _writer.WriteAsync(response).ConfigureAwait(false);
                return lines;
            }

            public async Task<List<string>> ReadRequestHeaderAndSendCustomResponseAsync(byte[] response)
            {
                List<string> lines = await ReadRequestHeaderAsync().ConfigureAwait(false);
                await _stream.WriteAsync(response, 0, response.Length).ConfigureAwait(false);
                return lines;
            }

            public async Task<List<string>> ReadRequestHeaderAndSendResponseAsync(HttpStatusCode statusCode = HttpStatusCode.OK, string additionalHeaders = null, string content = null)
            {
                List<string> lines = await ReadRequestHeaderAsync().ConfigureAwait(false);
                await SendResponseAsync(statusCode, additionalHeaders, content).ConfigureAwait(false);
                return lines;
            }


            //
            // GenericLoopbackServer implementation
            //

            public override async Task<HttpRequestData> ReadRequestDataAsync(bool readBody = true)
            {
                List<string> headerLines = null;
                HttpRequestData requestData = new HttpRequestData();

                headerLines = await ReadRequestHeaderAsync().ConfigureAwait(false);

                // Parse method and path
                string[] splits = headerLines[0].Split(' ');
                requestData.Method = splits[0];
                requestData.Path = splits[1];

                // Convert header lines to key/value pairs
                // Skip first line since it's the status line
                foreach (var line in headerLines.Skip(1))
                {
                    int offset = line.IndexOf(':');
                    string name = line.Substring(0, offset);
                    string value = line.Substring(offset + 1).TrimStart();
                    requestData.Headers.Add(new HttpHeaderData(name, value));
                }

                if (requestData.Method != "GET")
                {
                    if (requestData.GetHeaderValueCount("Content-Length") != 0)
                    {
                        _contentLength = Int32.Parse(requestData.GetSingleHeaderValue("Content-Length"));
                    }
                    else if (requestData.GetHeaderValueCount("Transfer-Encoding") != 0 && requestData.GetSingleHeaderValue("Transfer-Encoding") == "chunked")
                    {
                        _contentLength = -1;
                    }
                }

                if (readBody)
                {
                    requestData.Body = await ReadRequestBodyAsync().ConfigureAwait(false);
                    _bodyRead = true;
                }

                return requestData;
            }

            public override async Task<Byte[]> ReadRequestBodyAsync()
            {
                byte[] buffer = null;

                if (_bodyRead)
                {
                    throw new InvalidOperationException("Body already done");
                }

                if (_contentLength == 0)
                {
                    buffer = new byte[0];
                }
                if (_contentLength > 0)
                {
                    buffer = new byte[_contentLength];
                    int bytesRead = await ReadBlockAsync(buffer, 0, _contentLength).ConfigureAwait(false);
                    Assert.Equal(_contentLength, bytesRead);
                }
                else if (_contentLength < 0)
                {
                    // Chunked Encoding
                    while (true)
                    {
                        string chunkHeader = await ReadLineAsync().ConfigureAwait(false);
                        int chunkLength = int.Parse(chunkHeader, System.Globalization.NumberStyles.HexNumber);
                        if (chunkLength == 0)
                        {
                            // Last chunk. Read CRLF and exit.
                            await ReadLineAsync().ConfigureAwait(false);
                            break;
                        }

                        byte[] chunk = new byte[chunkLength];
                        await ReadBlockAsync(chunk, 0, chunkLength).ConfigureAwait(false);
                        await ReadLineAsync().ConfigureAwait(false);
                        if (buffer == null)
                        {
                            buffer = chunk;
                        }
                        else
                        {
                            byte[] newBuffer = new byte[buffer.Length + chunkLength];

                            buffer.CopyTo(newBuffer, 0);
                            chunk.CopyTo(newBuffer, buffer.Length);
                            buffer = newBuffer;
                        }
                    }
                }

                return buffer;
            }

            public override async Task SendResponseAsync(HttpStatusCode? statusCode = HttpStatusCode.OK, IList<HttpHeaderData> headers = null, string content = null, bool isFinal = true, int requestId = 0)
            {
                string headerString = null;
                int contentLength = -1;
                bool isChunked = false;
                bool hasContentLength  = false;

                if (headers != null)
                {
                    // Process given headers and look for some well-known cases.
                    foreach (HttpHeaderData headerData in headers)
                    {
                        if (headerData.Name.Equals("Content-Length", StringComparison.OrdinalIgnoreCase))
                        {
                            hasContentLength = true;
                            if (headerData.Value == null)
                            {
                                continue;
                            }

                            contentLength = int.Parse(headerData.Value);
                        }
                        else if (headerData.Name.Equals("Transfer-Encoding", StringComparison.OrdinalIgnoreCase) && headerData.Value.Equals("chunked", StringComparison.OrdinalIgnoreCase))
                        {
                            isChunked = true;
                        }

                        headerString = headerString + $"{headerData.Name}: {headerData.Value}\r\n";
                    }
                }

                bool endHeaders = content != null || isFinal;
                if (statusCode != null)
                {
                    // If we need to send status line, prepped it to headers and possibly add missing headers to the end.
                    headerString =
                        $"HTTP/1.1 {(int)statusCode} {GetStatusDescription((HttpStatusCode)statusCode)}\r\n" +
                        (!hasContentLength && !isChunked && content != null ? $"Content-length: {content.Length}\r\n" : "") +
                        headerString +
                        (endHeaders ? "\r\n" : "");
                }

                await SendResponseAsync(headerString).ConfigureAwait(false);
                if (content != null)
                {
                    await SendResponseBodyAsync(content, isFinal: isFinal, requestId: requestId).ConfigureAwait(false);
                }
            }

            public override async Task SendResponseHeadersAsync(HttpStatusCode statusCode = HttpStatusCode.OK, IList<HttpHeaderData> headers = null, int requestId = 0)
            {
                string headerString = null;

                if (headers != null)
                {
                    foreach (HttpHeaderData headerData in headers)
                    {
                        headerString = headerString + $"{headerData.Name}: {headerData.Value}\r\n";
                    }
                }

                headerString = GetHttpResponseHeaders(statusCode, headerString, 0, connectionClose: true);

                await SendResponseAsync(headerString).ConfigureAwait(false);
            }

            public override async Task SendResponseBodyAsync(byte[] body, bool isFinal = true, int requestId = 0)
            {
                await SendResponseAsync(Encoding.UTF8.GetString(body)).ConfigureAwait(false);
            }

            public override async Task WaitForCancellationAsync(bool ignoreIncomingData = true, int requestId = 0)
            {
                var buffer = new byte[1024];
                while (true)
                {
                    int bytesRead = await ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);

                    if (!ignoreIncomingData)
                    {
                        Assert.Equal(0, bytesRead);
                    }

                    if (bytesRead == 0)
                    {
                        break;
                    }
                }
            }
        }

        public override async Task<HttpRequestData> HandleRequestAsync(HttpStatusCode statusCode = HttpStatusCode.OK, IList<HttpHeaderData> headers = null, string content = "")
        {
            using (Connection connection = await EstablishConnectionAsync().ConfigureAwait(false))
            {
                HttpRequestData requestData = await connection.ReadRequestDataAsync().ConfigureAwait(false);

                // For historical reasons, we added Date and "Connection: close" (to improve test reliability)
                bool hasDate = false;
                List<HttpHeaderData> newHeaders = new List<HttpHeaderData>();
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        newHeaders.Add(header);
                        if (header.Name.Equals("Date", StringComparison.OrdinalIgnoreCase))
                        {
                            hasDate = true;
                        }
                    }
                }

                newHeaders.Add(new HttpHeaderData("Connection", "Close"));
                if (!hasDate)
                {
                    newHeaders.Add(new HttpHeaderData("Date", "{DateTimeOffset.UtcNow:R}"));
                }

                await connection.SendResponseAsync(statusCode, newHeaders, content: content).ConfigureAwait(false);
                return requestData;
            }
        }

        public override Task AcceptConnectionAsync(Func<GenericLoopbackConnection, Task> funcAsync)
        {
            return AcceptConnectionAsync((Func<Connection, Task>)funcAsync);
        }
    }

    public sealed class Http11LoopbackServerFactory : LoopbackServerFactory
    {
        public static readonly Http11LoopbackServerFactory Singleton = new Http11LoopbackServerFactory();

        public override Task CreateServerAsync(Func<GenericLoopbackServer, Uri, Task> funcAsync, int millisecondsTimeout = 60_000, GenericLoopbackOptions options = null)
        {
            LoopbackServer.Options newOptions = new LoopbackServer.Options();
            if (options != null)
            {
                newOptions.Address = options.Address;
            }

            return LoopbackServer.CreateServerAsync((server, uri) => funcAsync(server, uri), options: newOptions);
        }

        public override bool IsHttp11 => true;
        public override bool IsHttp2 => false;
    }
}
