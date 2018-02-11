// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Test.Common
{
    // TODO: Expose Accept call as member

        // TODO: Url stuff

    public sealed class LoopbackServer : IDisposable
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

        public void Dispose()
        {
            _listenSocket.Dispose();
            _listenSocket = null;

        }

        public static Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool> AllowAllCertificates = (_, __, ___, ____) => true;

        public class Options
        {
            public IPAddress Address { get; set; } = IPAddress.Loopback;
            public int ListenBacklog { get; set; } = 1;
            public bool UseSsl { get; set; } = false;
            public SslProtocols SslProtocols { get; set; } = SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12;
            public bool WebSocketEndpoint { get; set; } = false;
            public Func<Stream, Stream> ResponseStreamWrapper { get; set; }
        }

        public static async Task CreateServerAsync(Func<LoopbackServer, Uri, Task> funcAsync, Options options = null)
        {
            options = options ?? new Options();

            using (var listenSocket = new Socket(options.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
            {
                listenSocket.Bind(new IPEndPoint(options.Address, 0));
                listenSocket.Listen(options.ListenBacklog);

                using (var server = new LoopbackServer(listenSocket, options))
                {
                    await funcAsync(server, server._uri);
                }
            }
        }

        public static Task CreateServerAndClientAsync(Func<Uri, Task> clientFunc, Func<LoopbackServer, Task> serverFunc)
        {
            return CreateServerAsync(async (server, uri) =>
            {
                Task clientTask = clientFunc(uri);
                Task serverTask = serverFunc(server);

                await new Task[] { clientTask, serverTask }.WhenAllOrAnyFailed();
            });
        }

        public static string DefaultHttpResponse => $"HTTP/1.1 200 OK\r\nDate: {DateTimeOffset.UtcNow:R}\r\nContent-Length: 0\r\n\r\n";

        public static IPAddress GetIPv6LinkLocalAddress() =>
            NetworkInterface
                .GetAllNetworkInterfaces()
                .SelectMany(i => i.GetIPProperties().UnicastAddresses)
                .Select(a => a.Address)
                .Where(a => a.IsIPv6LinkLocal)
                .FirstOrDefault();

        public static async Task<List<string>> ReadRequestAndSendResponseAsync(LoopbackServer server, string response = null, Options options = null)
        {
            List<string> lines = null;

            await AcceptSocketAsync(server, async (s, stream, reader, writer) =>
            {
                lines = await ReadWriteAcceptedAsync(s, reader, writer, response);
            }, options);

            return lines;
        }

        // TODO: REmove s
        public static async Task<List<string>> ReadWriteAcceptedAsync(Socket s, StreamReader reader, StreamWriter writer, string response = null)
        {
            // Read request line and headers. Skip any request body.
            var lines = new List<string>();
            string line;
            while (!string.IsNullOrEmpty(line = await reader.ReadLineAsync().ConfigureAwait(false)))
            {
                lines.Add(line);
            }

            await writer.WriteAsync(response ?? DefaultHttpResponse).ConfigureAwait(false);

            return lines;
        }

        public static async Task AcceptSocketAsync(LoopbackServer server, Func<Socket, Stream, StreamReader, StreamWriter, Task> funcAsync, Options options = null)
        {
            // TODO: Use options from server here
            options = options ?? new Options();
            Socket s = await server._listenSocket.AcceptAsync().ConfigureAwait(false);
            s.NoDelay = true;
            try
            {
                Stream stream = new NetworkStream(s, ownsSocket: false);
                if (options.UseSsl)
                {
                    var sslStream = new SslStream(stream, false, delegate { return true; });
                    using (var cert = Configuration.Certificates.GetServerCertificate())
                    {
                        await sslStream.AuthenticateAsServerAsync(
                            cert,
                            clientCertificateRequired: true, // allowed but not required
                            enabledSslProtocols: options.SslProtocols,
                            checkCertificateRevocation: false).ConfigureAwait(false);
                    }
                    stream = sslStream;
                }

                using (var reader = new StreamReader(stream, Encoding.ASCII))
                using (var writer = new StreamWriter(options?.ResponseStreamWrapper?.Invoke(stream) ?? stream, Encoding.ASCII) { AutoFlush = true })
                {
                    await funcAsync(s, stream, reader, writer).ConfigureAwait(false);
                }
            }
            finally
            {
                try
                {
                    s.Shutdown(SocketShutdown.Send);
                    s.Dispose();
                }
                catch (ObjectDisposedException)
                {
                    // In case the test itself disposes of the socket
                }
            }
        }
    }
}
