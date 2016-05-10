// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Net.Tests;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http.Functional.Tests
{
    public class LoopbackServer
    {
        public static Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool> AllowAllCertificates = (_, __, ___, ____) => true;

        public class Options
        {
            public int ListenBacklog { get; set; } = 1;
            public bool UseSsl { get; set; } = false;
            public SslProtocols SslProtocols { get; set; } = SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12;
        }

        public static Task CreateServerAsync(Func<Socket, Uri, Task> funcAsync, Options options = null)
        {
            IPEndPoint ignored;
            return CreateServerAsync(funcAsync, out ignored, options);
        }

        public static Task CreateServerAsync(Func<Socket, Uri, Task> funcAsync, out IPEndPoint localEndPoint, Options options = null)
        {
            options = options ?? new Options();
            try
            {
                var server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                server.Bind(new IPEndPoint(IPAddress.Loopback, 0));
                server.Listen(options.ListenBacklog);

                localEndPoint = (IPEndPoint)server.LocalEndPoint;
                var url = new Uri($"{(options.UseSsl ? "https" : "http")}://{localEndPoint.Address}:{localEndPoint.Port}/");

                return funcAsync(server, url).ContinueWith(t =>
                {
                    server.Dispose();
                    t.GetAwaiter().GetResult();
                }, CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.Default);
            }
            catch (Exception e)
            {
                localEndPoint = null;
                return Task.FromException(e);
            }
        }

        public static string DefaultHttpResponse => $"HTTP/1.1 200 OK\r\nDate: {DateTimeOffset.UtcNow:R}\r\nContent-Length: 0\r\n\r\n";

        public static Task ReadRequestAndSendResponseAsync(Socket server, string response = null, Options options = null)
        {
            return AcceptSocketAsync(server, async (s, stream, reader, writer) =>
            {
                while (!string.IsNullOrEmpty(await reader.ReadLineAsync().ConfigureAwait(false))) ;
                await writer.WriteAsync(response ?? DefaultHttpResponse).ConfigureAwait(false);
                await writer.FlushAsync().ConfigureAwait(false);
                s.Shutdown(SocketShutdown.Send);
            }, options);
        }

        public static async Task AcceptSocketAsync(Socket server, Func<Socket, Stream, StreamReader, StreamWriter, Task> funcAsync, Options options = null)
        {
            options = options ?? new Options();
            using (Socket s = await server.AcceptAsync().ConfigureAwait(false))
            {
                Stream stream = new NetworkStream(s, ownsSocket: false);
                if (options.UseSsl)
                {
                    var sslStream = new SslStream(stream);
                    using (var cert = CertificateConfiguration.GetServerCertificate())
                    {
                        await sslStream.AuthenticateAsServerAsync(
                            cert, 
                            clientCertificateRequired: false, 
                            enabledSslProtocols: options.SslProtocols, 
                            checkCertificateRevocation: false).ConfigureAwait(false);
                    }
                    stream = sslStream;
                }

                using (var reader = new StreamReader(stream, Encoding.ASCII))
                using (var writer = new StreamWriter(stream, Encoding.ASCII))
                {
                    await funcAsync(s, stream, reader, writer).ConfigureAwait(false);
                }
            }
        }

        public enum TransferType
        {
            None = 0,
            ContentLength,
            Chunked
        }

        public enum TransferError
        {
            None = 0,
            ContentLengthTooLarge,
            ChunkSizeTooLarge,
            MissingChunkTerminator
        }

        public static Task StartTransferTypeAndErrorServer(
            TransferType transferType,
            TransferError transferError,
            out IPEndPoint localEndPoint)
        {
            return CreateServerAsync((server, url) => AcceptSocketAsync(server, async (client, stream, reader, writer) =>
            {
                // Read past request headers.
                string line;
                while (!string.IsNullOrEmpty(line = reader.ReadLine())) ;

                // Determine response transfer headers.
                string transferHeader = null;
                string content = "This is some response content.";
                if (transferType == TransferType.ContentLength)
                {
                    transferHeader = transferError == TransferError.ContentLengthTooLarge ?
                        $"Content-Length: {content.Length + 42}\r\n" :
                        $"Content-Length: {content.Length}\r\n";
                }
                else if (transferType == TransferType.Chunked)
                {
                    transferHeader = "Transfer-Encoding: chunked\r\n";
                }

                // Write response header
                await writer.WriteAsync("HTTP/1.1 200 OK\r\n").ConfigureAwait(false);
                await writer.WriteAsync($"Date: {DateTimeOffset.UtcNow:R}\r\n").ConfigureAwait(false);
                await writer.WriteAsync("Content-Type: text/plain\r\n").ConfigureAwait(false);
                if (!string.IsNullOrEmpty(transferHeader))
                {
                    await writer.WriteAsync(transferHeader).ConfigureAwait(false);
                }
                await writer.WriteAsync("\r\n").ConfigureAwait(false);

                // Write response body
                if (transferType == TransferType.Chunked)
                {
                    string chunkSizeInHex = string.Format(
                        "{0:x}\r\n",
                        content.Length + (transferError == TransferError.ChunkSizeTooLarge ? 42 : 0));
                    await writer.WriteAsync(chunkSizeInHex).ConfigureAwait(false);
                    await writer.WriteAsync($"{content}\r\n").ConfigureAwait(false);
                    if (transferError != TransferError.MissingChunkTerminator)
                    {
                        await writer.WriteAsync("0\r\n\r\n").ConfigureAwait(false);
                    }
                }
                else
                {
                    await writer.WriteAsync($"{content}\r\n").ConfigureAwait(false);
                }
                await writer.FlushAsync().ConfigureAwait(false);

                client.Shutdown(SocketShutdown.Both);
            }), out localEndPoint);
        }
    }
}
