// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace System.Net.Test.Common
{
    public class Http2LoopbackServer : IDisposable
    {
        private Socket _listenSocket;
        private Stream _connectionStream;
        private Http2Options _options;
        private Uri _uri;

        public Http2LoopbackServer(Http2Options options)
        {
            _options = options;
        }

        public Uri CreateServer()
        {
            _listenSocket = new Socket(_options.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _listenSocket.Bind(new IPEndPoint(_options.Address, 0));
            _listenSocket.Listen(_options.ListenBacklog);

            var localEndPoint = (IPEndPoint)_listenSocket.LocalEndPoint;
            string host = _options.Address.AddressFamily == AddressFamily.InterNetworkV6 ?
                $"[{localEndPoint.Address}]" :
                localEndPoint.Address.ToString();

            string scheme = _options.UseSsl ? "https" : "http";

            _uri = new Uri($"{scheme}://{host}:{localEndPoint.Port}/");

            return _uri;
        }

        public async Task SendConnectionPrefaceAsync()
        {
            Frame emptySettings = new Frame(0, FrameType.Settings, FrameFlags.None, 0);
            await WriteFrameAsync(emptySettings).ConfigureAwait(false);

            // We also need to ack the client settings header sent with the preface.
            // We could piggyback along with another frame, but this is simpler.
            Frame ack = new Frame(0, FrameType.Settings, FrameFlags.Ack, 0);
            await WriteFrameAsync(emptySettings).ConfigureAwait(false);
        }

        public async Task WriteFrameAsync(Frame frame)
        {
            byte[] writeBuffer = new byte[Frame.FrameHeaderLength + frame.Length];
            frame.WriteTo(writeBuffer);
            await _connectionStream.WriteAsync(writeBuffer, 0, writeBuffer.Length).ConfigureAwait(false);
        }

        public async Task<Frame> ReadFrameAsync(TimeSpan timeout)
        {
            // Prep the timeout cancellation token.
            CancellationTokenSource timeoutCts = new CancellationTokenSource(timeout);

            // First read the frame headers, which should tell us how long the rest of the frame is.
            byte[] headerBytes = new byte[Frame.FrameHeaderLength];

            int readBytes = 0;
            while(readBytes < Frame.FrameHeaderLength)
            {
                readBytes += await _connectionStream.ReadAsync(headerBytes, readBytes, Frame.FrameHeaderLength - readBytes, timeoutCts.Token);
            }

            Frame header = Frame.ReadFrom(headerBytes);

            // Read the data segment of the frame, if it is present.
            byte[] data = new byte[header.Length];

            readBytes = 0;
            while(readBytes < header.Length)
            {
                readBytes += await _connectionStream.ReadAsync(data, readBytes, header.Length - readBytes, timeoutCts.Token);
            }

            // Construct the correct frame type and return it.
            switch (header.Type)
            {
                case FrameType.Data:
                    return DataFrame.ReadFrom(header, data);
                case FrameType.Headers:
                    return HeadersFrame.ReadFrom(header, data);
                case FrameType.Priority:
                    return PriorityFrame.ReadFrom(header, data);
                case FrameType.RstStream:
                    return RstStreamFrame.ReadFrom(header, data);
                default:
                    return header;
            }
        }

        // Returns the first 24 bytes read, which should be the connection preface.
        public async Task<string> AcceptConnectionAsync()
        {
            _connectionStream = new NetworkStream(await _listenSocket.AcceptAsync().ConfigureAwait(false), true);

            if (_options.UseSsl)
            {
                var sslStream = new SslStream(_connectionStream, false, delegate
                { return true; });
                using (var cert = Configuration.Certificates.GetServerCertificate())
                {
                    SslServerAuthenticationOptions options = new SslServerAuthenticationOptions();

                    options.EnabledSslProtocols = _options.SslProtocols;

                    var protocols = new List<SslApplicationProtocol>();
                    protocols.Add(SslApplicationProtocol.Http2);
                    protocols.Add(SslApplicationProtocol.Http11);
                    options.ApplicationProtocols = protocols;

                    options.ServerCertificate = cert;

                    options.ClientCertificateRequired = false;

                    await sslStream.AuthenticateAsServerAsync(options, CancellationToken.None).ConfigureAwait(false);
                }
                _connectionStream = sslStream;
            }

            StreamReader reader = new StreamReader(_connectionStream, Encoding.ASCII);
            char[] prefix = new char[24];
            await reader.ReadBlockAsync(prefix, 0, prefix.Length);
            return new string(prefix);
        }

        public void Dispose()
        {
            if (_listenSocket != null)
            {
                _listenSocket.Dispose();
                _listenSocket = null;
            }
        }
    }

    public class Http2Options
    {
        public IPAddress Address { get; set; } = IPAddress.Loopback;
        public int ListenBacklog { get; set; } = 1;
        public bool UseSsl { get; set; } = true;
        public SslProtocols SslProtocols { get; set; } = SslProtocols.Tls12;
    }
}